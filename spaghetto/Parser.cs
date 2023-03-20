using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class Parser {
        public List<SyntaxToken> Tokens { get; set; }
        public int Position = 0;

        public SyntaxToken Current => Peek(0);

        public SyntaxToken Peek(int off = 0) {
            if (Position + off >= Tokens.Count || Position + off < 0) return new(SyntaxType.BadToken, 0, null, "");
            return Tokens[Position + off];
        }

        public SyntaxToken MatchToken(SyntaxType type) {
            if(Current.Type == type) {
                Position++;
                return Peek(-1);
            }

            throw new Exception("Unexpected token " + Current.Type + "; expected " + type);
        }

        public SyntaxToken MatchTokenWithValue(SyntaxType type, object value)
        {
            if (Current.Type == type && Current.Value == value) {
                Position++;
                return Peek(-1);
            }

            throw new Exception("Unexpected token " + Current.Type + "; expected " + type + " with value " + value);
        }

        public SyntaxToken MatchKeyword(string value) {
            if (Current.Type == SyntaxType.Keyword && Current.Text == value) {
                Position++;
                return Peek(-1);
            }

            throw new Exception("Unexpected token " + Current.Type + "; expected Keyword with value " + value);
        }

        public Parser(List<SyntaxToken> tokens) {
            Tokens = tokens;
        }

        public SyntaxNode Parse() {
            return ParseStatements();
        }

        public SyntaxNode ParseStatements() {
            List<SyntaxNode> nodes = new();

            while(Current.Type != SyntaxType.EOF) {
                nodes.Add(ParseStatement());
            }

            return new BlockNode(nodes, false);
        }

        public SyntaxNode ParseScopedStatements() {
            MatchToken(SyntaxType.LBraces);
            List<SyntaxNode> nodes = new();

            while(Current.Type != SyntaxType.RBraces) {
                if (Current.Type == SyntaxType.EOF) throw new Exception("Unclosed block at " + Current.Position);

                nodes.Add(ParseStatement());
            }

            MatchToken(SyntaxType.RBraces);

            return new BlockNode(nodes);
        }

        public SyntaxNode ParseStatement() {
            if (Current.Type == SyntaxType.Keyword && Current.Text == "return") {
                if (Peek(1).Type == SyntaxType.Semicolon) {
                    Position += 2;
                    var ret = new ReturnNode();
                    MatchToken(SyntaxType.Semicolon);
                    return ret;
                } else {
                    Position++;
                    var ret = new ReturnNode(ParseExpression());
                    MatchToken(SyntaxType.Semicolon);
                    return ret;
                }
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "continue") {
                Position++;
                MatchToken(SyntaxType.Semicolon);
                return new ContinueNode();
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "break") {
                Position++;
                MatchToken(SyntaxType.Semicolon);
                return new BreakNode();
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "import") {
                Position++;
                
                if(Current.Type == SyntaxType.Keyword && Current.Text == "native") {
                    Position++;
                    var ident = MatchToken(SyntaxType.Identifier);
                    MatchToken(SyntaxType.Semicolon);

                    return new NativeImportNode(ident);
                }else {
                    throw new NotImplementedException("Importing other files is not supported yet.");
                }
            } else {
                var exprNode = ParseExpression();
                MatchToken(SyntaxType.Semicolon);

                return exprNode;
            }
        }

        public SyntaxNode ParseExpression() {
            if(Current.Type == SyntaxType.Keyword && Current.Text == "var") {
                bool fixedType = true;
                Position++;

                if(Current.Type == SyntaxType.Mod) {
                    fixedType = false;
                    Position++;
                }

                var ident = MatchToken(SyntaxType.Identifier);

                if(Current.Type == SyntaxType.Equals) {
                    Position++;
                    var expr = ParseExpression();
                    return new InitVariableNode(ident, expr, fixedType);
                }else {
                    return new InitVariableNode(ident, fixedType);
                }
            }else if(Current.Type == SyntaxType.Identifier && Peek(1).Type == SyntaxType.Equals) {
                var ident = MatchToken(SyntaxType.Identifier);
                MatchToken(SyntaxType.Equals);
                var expr = ParseExpression();
                return new AssignVariableNode(ident, expr);
            }else {
                return ParseCompExpression();
            }
        }

        public SyntaxNode ParseCompExpression() {
            if(Current.Type == SyntaxType.Bang) {
                Position++;
                return new UnaryExpressionNode(Peek(-1), ParseCompExpression());
            }else {
                return BinaryOperation(() => { return ParseArithmeticExpression(); },
                    new List<SyntaxType>() {
                        SyntaxType.EqualsEquals, SyntaxType.LessThan, SyntaxType.LessThanEqu, SyntaxType.GreaterThan, SyntaxType.GreaterThanEqu
                    });
            }
        }

        public SyntaxNode ParseArithmeticExpression() {
            return BinaryOperation(() => { return ParseTermExpression(); }, new() { SyntaxType.Plus, SyntaxType.Minus });
        }

        public SyntaxNode ParseTermExpression() {
            return BinaryOperation(() => { return ParseFactorExpression(); }, new() { SyntaxType.Mul, SyntaxType.Div, SyntaxType.Mod, SyntaxType.Idx });
        }

        public SyntaxNode ParseFactorExpression() {
            if(Current.Type is SyntaxType.Plus or SyntaxType.Minus or SyntaxType.Bang) {
                Position++;
                var factor = ParseFactorExpression();
                return new UnaryExpressionNode(Peek(-1), factor);
            }

            return ParsePowerExpression();
        }

        public SyntaxNode ParsePowerExpression() {
            return BinaryOperation(() => { return ParseDotExpression(); }, new() { SyntaxType.Pow }, () => { return ParseFactorExpression(); });
        }

        public SyntaxNode ParseDotExpression() {
            var callNode = ParseCallExpression();
            DotNode accessStack = new(callNode);

            if (Current.Type is SyntaxType.Dot) {
                while (Current.Type is SyntaxType.Dot) {
                    Position++;

                    if (Current.Type is SyntaxType.Identifier) {
                        var n = ParseCallExpression();

                        accessStack.NextNodes.Add(n);
                    }
                }
            } else return callNode;

            return accessStack;
        }

        public SyntaxNode ParseCallExpression() {
            var atomNode = ParseCastExpression();

            if(Current.Type is SyntaxType.LParen) {
                Position++;

                List<SyntaxNode> argumentNodes = new();

                if(Current.Type is SyntaxType.RParen) {
                    Position++;
                }else {
                    argumentNodes.Add(ParseExpression());

                    while(Current.Type is SyntaxType.Comma) {
                        Position++;

                        argumentNodes.Add(ParseExpression());
                    }

                    MatchToken(SyntaxType.RParen);
                }

                return new CallNode(atomNode, argumentNodes);
            }

            return atomNode;
        }

        public SyntaxNode ParseCastExpression() {
            if(Current.Type is SyntaxType.LessThan) {
                MatchToken(SyntaxType.LessThan);
                var ident = MatchToken(SyntaxType.Identifier);

                if (ident.Text is not "int" and not "float" and not "list" and not "string") throw new Exception("Can not cast to " + ident.Text);

                MatchToken(SyntaxType.GreaterThan);

                var node = ParseCastExpression();
                return new CastNode(ident, node);
            }else {
                return ParseAtomExpression();
            }
        }

        public SyntaxNode ParseAtomExpression() {
            if (Current.Type is SyntaxType.Int) {
                Position++;
                return new IntLiteralNode(Peek(-1));
            } else if (Current.Type is SyntaxType.Float) {
                Position++;
                return new FloatLiteralNode(Peek(-1));
            } else if (Current.Type is SyntaxType.String) {
                Position++;
                return new StringLiteralNode(Peek(-1));
            } else if (Current.Type is SyntaxType.Identifier) {
                Position++;
                return new IdentifierNode(Peek(-1));
            } else if(Current.Type is SyntaxType.LParen) {
                Position++;
                var expr = ParseExpression();

                MatchToken(SyntaxType.RParen);

                return expr; // TODO: Do we have to create a ParenthisizedExpr? (probably not, but what if we do?)
            } else if (Current.Type is SyntaxType.LSqBracket) {
                return ParseListExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "if") {
                return ParseIfExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "for") {
                return ParseForExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "while") {
                return ParseWhileExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "func") {
                return ParseFunctionExpression();
            }else {
                throw new Exception("Unexpected token in atom expression!");
            }
        }

        public SyntaxNode ParseListExpression() {
            MatchToken(SyntaxType.LSqBracket);

            List<SyntaxNode> list = new();

            if (Current.Type == SyntaxType.RSqBracket) {
                MatchToken(SyntaxType.RSqBracket);
            } else {
                var expr = ParseExpression();
                list.Add(expr);

                while (Current.Type == SyntaxType.Comma) {
                    Position++;
                    expr = ParseExpression();
                    list.Add(expr);
                }

                MatchToken(SyntaxType.RSqBracket);
            }

            return new ListNode(list);
        }

        public SyntaxNode ParseIfExpression() {
            MatchKeyword("if");

            IfNode node = new();

            MatchToken(SyntaxType.LParen);
            var initialCond = ParseExpression();
            MatchToken(SyntaxType.RParen);

            var initialBlock = ParseScopedStatements();

            node.AddCase(initialCond, initialBlock);

            while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif") {
                Position++;

                MatchToken(SyntaxType.LParen);
                var cond = ParseExpression();
                MatchToken(SyntaxType.RParen);
                var block = ParseScopedStatements();

                node.AddCase(cond, block);
            }

            if(Current.Type == SyntaxType.Keyword && Current.Text == "else") {
                Position++;
                var block = ParseScopedStatements();

                node.AddCase(new BoolNode(true), block);
            }

            return node;
        }

        public SyntaxNode ParseForExpression() {
            MatchKeyword("for");

            MatchToken(SyntaxType.LParen);
            var initialExpressionNode = ParseExpression();
            MatchToken(SyntaxType.Semicolon);
            var condNode = ParseExpression();
            MatchToken(SyntaxType.Semicolon);
            var stepNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedStatements();

            return new ForNode(initialExpressionNode, condNode, stepNode, block);
        }

        public SyntaxNode ParseWhileExpression() {
            MatchKeyword("while");

            MatchToken(SyntaxType.LParen);
            var condNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedStatements();

            return new WhileNode(condNode, block);
        }

        public SyntaxNode ParseFunctionExpression() {
            MatchKeyword("func");

            SyntaxToken? nameToken = null;
            if(Current.Type == SyntaxType.Identifier)
                nameToken = MatchToken(SyntaxType.Identifier);

            MatchToken(SyntaxType.LParen);

            List<SyntaxToken> args = new();

            if (Current.Type == SyntaxType.RParen) {
                MatchToken(SyntaxType.RParen);
            } else {
                var ident = MatchToken(SyntaxType.Identifier);
                args.Add(ident);

                while (Current.Type == SyntaxType.Comma) {
                    Position++;
                    ident = MatchToken(SyntaxType.Identifier);
                    args.Add(ident);
                }

                MatchToken(SyntaxType.RParen);
            }

            var block = ParseScopedStatements();

            return new FunctionDefinitionNode(nameToken, args, block);
        }

        public SyntaxNode BinaryOperation(Func<SyntaxNode> leftParse, List<SyntaxType> allowedTypes, Func<SyntaxNode> rightParse = null) {
            var left = leftParse();
            SyntaxNode right;
            while (allowedTypes.Contains(Current.Type)) {
                SyntaxToken operatorToken = Current;
                Position++;
                right = (rightParse ?? leftParse)();

                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }
    }

    internal class NativeImportNode : SyntaxNode {
        private SyntaxToken ident;

        public NativeImportNode(SyntaxToken ident) {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override SValue Evaluate(Scope scope) {
            var val = scope.Get("nlimporter__" + ident.Text);

            if(val == null || val is not SNativeLibraryImporter importer) {
                throw new Exception("Native library " + ident.Text + " not found!");
            }

            importer.Import(scope);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(ident);
        }
    }

    internal class FunctionDefinitionNode : SyntaxNode {
        private SyntaxToken? nameToken;
        private List<SyntaxToken> args;
        private SyntaxNode block;

        public FunctionDefinitionNode(SyntaxToken? nameToken, List<SyntaxToken> args, SyntaxNode block) {
            this.nameToken = nameToken;
            this.args = args;
            this.block = block;
        }

        public override NodeType Type => NodeType.FunctionDefinition;

        public override SValue Evaluate(Scope scope) {
            var f = new SFunction(scope, nameToken?.Text ?? "<anonymous>", args.Select((v) => v.Text).ToList(), block);
            if(nameToken != null) scope.Set(nameToken.Value.Text, f);
            return f;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            if(nameToken != null) yield return new TokenNode(nameToken.Value);
            foreach (var t in args) yield return new TokenNode(t);
            yield return block;
        }
    }

    internal class WhileNode : SyntaxNode {
        private SyntaxNode condNode;
        private SyntaxNode block;

        public WhileNode(SyntaxNode condNode, SyntaxNode block) {
            this.condNode = condNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.While;

        public override SValue Evaluate(Scope scope) {
            Scope whileScope = new(scope);
            SValue lastVal = SValue.Null;

            while (true) {
                if (!condNode.Evaluate(whileScope).IsTruthy()) break;
                var whileBlockRes = block.Evaluate(whileScope);
                if (!whileBlockRes.IsNull()) lastVal = whileBlockRes;

                if (whileScope.State == ScopeState.ShouldBreak) break;
                if(whileScope.State != ScopeState.None) whileScope.SetState(ScopeState.None);
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return condNode;
            yield return block;
        }
    }

    internal class CastNode : SyntaxNode {
        private SyntaxToken ident;
        private SyntaxNode node;

        public CastNode(SyntaxToken ident, SyntaxNode node) {
            this.ident = ident;
            this.node = node;
        }

        public override NodeType Type => NodeType.Cast;

        public override SValue Evaluate(Scope scope) {
            // TODO: maybe improve this
            switch(ident.Text) {
                case "int":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.Int);
                case "float":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.Float);
                case "string":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.String);
                case "list":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.List);
                default: throw new InvalidOperationException("INTERNAL: Cast was parsed successfully, but cast is not implemented for that!");
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(ident);
            yield return node;
        }
    }

    internal class ForNode : SyntaxNode {
        private SyntaxNode initialExpressionNode;
        private SyntaxNode condNode;
        private SyntaxNode stepNode;
        private SyntaxNode block;

        public ForNode(SyntaxNode initialExpressionNode, SyntaxNode condNode, SyntaxNode stepNode, SyntaxNode block) {
            this.initialExpressionNode = initialExpressionNode;
            this.condNode = condNode;
            this.stepNode = stepNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.For;

        public override SValue Evaluate(Scope scope) {
            Scope forScope = new(scope);
            SValue lastVal = SValue.Null;
            initialExpressionNode.Evaluate(forScope);

            while(true) {
                if (!condNode.Evaluate(forScope).IsTruthy()) break;
                var forBlockRes = block.Evaluate(forScope);
                if (!forBlockRes.IsNull()) lastVal = forBlockRes;

                if (forScope.State == ScopeState.ShouldBreak) break;
                if(forScope.State != ScopeState.None) forScope.SetState(ScopeState.None);

                stepNode.Evaluate(forScope);
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return initialExpressionNode;
            yield return condNode;
            yield return stepNode;
            yield return block;
        }
    }

    internal class IfNode : SyntaxNode {
        public List<(SyntaxNode cond, SyntaxNode block)> Conditions { get; private set; } = new();

        public override NodeType Type => NodeType.If;

        public override SValue Evaluate(Scope scope) {
            foreach((SyntaxNode cond, SyntaxNode block) in Conditions) {
                var condRes = cond.Evaluate(scope);

                if(condRes.IsTruthy()) {
                    return block.Evaluate(new Scope(scope));
                }
            }

            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            foreach(var (cond, block) in Conditions) {
                yield return cond;
                yield return block;
            }
        }

        internal void AddCase(SyntaxNode cond, SyntaxNode block) {
            Conditions.Add((cond, block));
        }

        public override string ToString() {
            return "IfNode:";
        }
    }

    internal class ListNode : SyntaxNode {
        private List<SyntaxNode> list;

        public ListNode(List<SyntaxNode> list) {
            this.list = list;
        }

        public override NodeType Type => NodeType.List;

        public override SValue Evaluate(Scope scope) {
            SList sList = new();

            foreach(var n in list) {
                sList.Value.Add(n.Evaluate(scope));
            }

            return sList;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            foreach (var n in list) yield return n;
        }

        public override string ToString() {
            return "ListNode:";
        }
    }

    internal class IdentifierNode : SyntaxNode {
        public SyntaxToken Token { get; private set; }

        public IdentifierNode(SyntaxToken syntaxToken) {
            this.Token = syntaxToken;
        }

        public override NodeType Type => NodeType.Identifier;

        public override SValue Evaluate(Scope scope) {
            return scope.Get((string)Token.Value) ?? SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(Token);
        }

        public override string ToString() {
            return "IdentNode:";
        }
    }

    internal class IntLiteralNode : SyntaxNode {
        private SyntaxToken syntaxToken;

        public IntLiteralNode(SyntaxToken syntaxToken) {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override SValue Evaluate(Scope scope) {
            var sint = new SInt((int)syntaxToken.Value);
            return sint;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString() {
            return "IntLitNode:";
        }
    }

    internal class FloatLiteralNode : SyntaxNode {
        private SyntaxToken syntaxToken;

        public FloatLiteralNode(SyntaxToken syntaxToken) {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.FloatLiteral;

        public override SValue Evaluate(Scope scope) {
            return new SFloat((float)syntaxToken.Value);
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString() {
            return "FloatLitNode:";
        }
    }

    internal class StringLiteralNode : SyntaxNode {
        private SyntaxToken syntaxToken;

        public StringLiteralNode(SyntaxToken syntaxToken) {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.StringLiteral;

        public override SValue Evaluate(Scope scope) {
            return new SString((string)syntaxToken.Value);
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString() {
            return "StringLitNode:";
        }
    }

    internal class CallNode : SyntaxNode {
        public SyntaxNode ToCallNode { get; set; }
        private List<SyntaxNode> argumentNodes;

        public CallNode(SyntaxNode atomNode, List<SyntaxNode> argumentNodes) {
            ToCallNode = atomNode;
            this.argumentNodes = argumentNodes;
        }

        public override NodeType Type => NodeType.Call;

        public override SValue Evaluate(Scope scope) {
            var toCall = ToCallNode.Evaluate(scope);
            var args = EvaluateArgs(scope);

            return toCall.Call(scope, args);
        }

        public List<SValue> EvaluateArgs(Scope scope) {
            var args = new List<SValue>();

            foreach (var n in argumentNodes) args.Add(n.Evaluate(scope));
            return args;
        }
            
        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return ToCallNode;
            foreach (var n in argumentNodes) yield return n;
        }

        public override string ToString() {
            return "CallNode:";
        }
    }

    internal class DotNode : SyntaxNode
    {
        public DotNode(SyntaxNode callNode)
        {
            CallNode = callNode;
        }

        public SyntaxNode CallNode { get; }
        public List<SyntaxNode> NextNodes { get; internal set; } = new();

        public override NodeType Type => NodeType.Dot;

        public override SValue Evaluate(Scope scope)
        {
            var currentValue = CallNode.Evaluate(scope);

            foreach(var node in NextNodes) {
                if(node is IdentifierNode rvn) {
                    var ident = rvn.Token;
                    currentValue = currentValue.Dot(new SString((string)ident.Value));
                }else if(node is CallNode cn) {
                    if(cn.ToCallNode is IdentifierNode cnIdentNode) {
                        var ident = cnIdentNode.Token;
                        currentValue = currentValue.Dot(new SString((string)ident.Value)).Call(scope, cn.EvaluateArgs(scope));
                    }else {
                        throw new Exception("Tried to call a non identifier in dot not stack.");
                    }
                }else {
                    throw new Exception("Unexpected node in dot node stack!");
                }
            }

            return currentValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CallNode;

            foreach (var node in NextNodes) yield return node;
        }

        public override string ToString() {
            return "DotNode:";
        }
    }

    internal class UnaryExpressionNode : SyntaxNode
    {
        private SyntaxToken token;
        private SyntaxNode rhs;

        public UnaryExpressionNode(SyntaxToken token, SyntaxNode rhs)
        {
            this.token = token;
            this.rhs = rhs;
        }

        public override NodeType Type => NodeType.UnaryExpression;

        public override SValue Evaluate(Scope scope)
        {
            switch(token.Type) {
                case SyntaxType.Bang: return rhs.Evaluate(scope).Not();
                case SyntaxType.Minus: return rhs.Evaluate(scope).ArithNot();
                case SyntaxType.Plus: return rhs.Evaluate(scope);
                default: throw new InvalidOperationException();
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(token);
            yield return rhs;
        }

        public override string ToString() {
            return "UnaryExpressionNode:";
        }
    }

    internal class AssignVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;

        public AssignVariableNode(SyntaxToken ident, SyntaxNode expr)
        {
            this.ident = ident;
            this.expr = expr;
        }

        public override NodeType Type => NodeType.AssignVariable;

        public override SValue Evaluate(Scope scope)
        {
            if(scope.Get(ident.Value.ToString()) == null) {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = expr.Evaluate(scope);
            var key = ident.Value.ToString();
            if (!scope.Update(key, val, out Exception ex)) throw ex;
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return expr;
        }

        public override string ToString() {
            return "AssignVariableNode:";
        }
    }

    internal class InitVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        private readonly bool isFixedType = true;

        public InitVariableNode(SyntaxToken ident, bool isFixedType)
        {
            this.ident = ident;
            this.isFixedType = isFixedType;
        }

        public InitVariableNode(SyntaxToken ident, SyntaxNode expr, bool isFixedType)
        {
            this.ident = ident;
            this.expr = expr;
            this.isFixedType = isFixedType;
        }

        public override NodeType Type => NodeType.InitVariable;

        public override SValue Evaluate(Scope scope)
        {
            if(scope.Get(ident.Value.ToString()) != null) {
                throw new InvalidOperationException("Can not initiliaze the same variable twice!");
            }

            if (expr != null) {
                var val = expr.Evaluate(scope);
                val.TypeIsFixed = isFixedType;

                scope.Set(ident.Value.ToString(), val);
                return val;
            }else {
                if (isFixedType) throw new InvalidOperationException("Tried to initiliaze a fixed type variable with no value; this is not permitted. Use var% instead.");

                scope.Set(ident.Value.ToString(), SValue.Null);
                return SValue.Null;
            }

        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            if(expr != null) yield return expr;
        }

        public override string ToString() {
            return "InitVariableNode:";
        }
    }

    internal class BreakNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Break;

        public override SValue Evaluate(Scope scope)
        {
            scope.SetState(ScopeState.ShouldBreak);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString() {
            return "BreakNode:";
        }
    }

    internal class ContinueNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Continue;

        public override SValue Evaluate(Scope scope)
        {
            scope.SetState(ScopeState.ShouldContinue);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString() {
            return "ContinueNode:";
        }
    }

    internal class ReturnNode : SyntaxNode
    {
        public ReturnNode()
        {
        }

        public ReturnNode(SyntaxNode returnValueNode)
        {
            ReturnValueNode = returnValueNode;
        }

        public SyntaxNode ReturnValueNode { get; }

        public override NodeType Type => NodeType.Return;

        public override SValue Evaluate(Scope scope)
        {
            scope.SetState(ScopeState.ShouldReturn);

            if(ReturnValueNode != null) {
                var v = ReturnValueNode.Evaluate(scope);
                scope.SetReturnValue(v);
            }

            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (ReturnValueNode == null) yield break;
            else yield return ReturnValueNode;
        }

        public override string ToString() {
            return "ReturnNode:";
        }
    }

    internal class BlockNode : SyntaxNode
    {
        private List<SyntaxNode> nodes;
        private readonly bool createNewScope;

        public BlockNode(List<SyntaxNode> nodes, bool createNewScope = true)
        {
            this.nodes = nodes;
            this.createNewScope = createNewScope;
        }

        public override NodeType Type => NodeType.Block;

        public override SValue Evaluate(Scope scope)
        {
            var lastVal = SValue.Null;
            var blockScope = scope;
            
            if(createNewScope) blockScope = new Scope(scope);

            foreach (var node in nodes) {
                var res = node.Evaluate(blockScope);

                if (!res.IsNull()) {
                    lastVal = res;
                }

                if (scope.State == ScopeState.ShouldBreak
                    || scope.State == ScopeState.ShouldContinue) return lastVal;

                if (scope.State == ScopeState.ShouldReturn) {
                    var v = scope.ReturnValue;
                    return v;
                }
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach(var node in nodes) yield return node;
        }

        public override string ToString() {
            return "BlockNode:";
        }
    }

    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }

        public abstract SValue Evaluate(Scope scope);
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    internal class BinaryExpressionNode : SyntaxNode
    {
        private SyntaxNode left;
        private SyntaxToken operatorToken;
        private SyntaxNode right;

        public BinaryExpressionNode(SyntaxNode left, SyntaxToken operatorToken, SyntaxNode right)
        {
            this.left = left;
            this.operatorToken = operatorToken;
            this.right = right;
        }

        public override NodeType Type => NodeType.BinaryExpression;

        public override SValue Evaluate(Scope scope)
        {
            var leftRes = left.Evaluate(scope);
            var rightRes = right.Evaluate(scope);

            switch(operatorToken.Type) {
                case SyntaxType.Plus:
                    return leftRes.Add(rightRes);
                case SyntaxType.Minus:
                    return leftRes.Sub(rightRes);
                case SyntaxType.Div:
                    return leftRes.Div(rightRes);
                case SyntaxType.Mul:
                    return leftRes.Mul(rightRes);
                case SyntaxType.Mod:
                    return leftRes.Mod(rightRes);
                case SyntaxType.EqualsEquals:
                    return leftRes.Equals(rightRes);
                case SyntaxType.Idx:
                    return leftRes.Idx(rightRes);
                case SyntaxType.LessThan:
                    return leftRes.LessThan(rightRes);
                case SyntaxType.LessThanEqu:
                    return leftRes.LessThanEqu(rightRes);
                case SyntaxType.GreaterThan:
                    return leftRes.GreaterThan(rightRes);
                case SyntaxType.GreaterThanEqu:
                    return leftRes.GreaterThanEqu(rightRes);

                default:
                    throw new NotImplementedException();
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return left;
            yield return new TokenNode(operatorToken);
            yield return right;
        }

        public override string ToString() {
            return "BinaryExprNode: op=" + operatorToken.Type;
        }
    }

    public class BoolNode : SyntaxNode
    {
        public override NodeType Type => NodeType.BooleanLiteral;

        public bool Value { get; set; }

        public BoolNode(bool value)
        {
            Value = value;
        }

        public override SValue Evaluate(Scope scope)
        {
            return new SInt(Value ? 1 : 0);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString() {
            return "BoolNode: " + Value;
        }
    }

    // dummy node for tree view
    public class TokenNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Token;
        public SyntaxToken Token { get; set; }

        public TokenNode(SyntaxToken token)
        {
            Token = token;
        }

        public override SValue Evaluate(Scope scope)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString() {
            return "TokenNode: " + Token.Type.ToString() + " val=" + Token.Value?.ToString() + " text=" + Token.Text.ToString();
        }
    }

    public abstract class SValue
    {
        public static SValue Null => new SNull();
        public abstract SBuiltinType BuiltinName { get; }

        #region Metadata
        internal bool TypeIsFixed { get; set; } = true;
        #endregion

        public virtual SValue Add(SValue other)
        {
            throw NotSupportedOn("Add");
        }

        public virtual SValue Sub(SValue other)
        {
            throw NotSupportedOn("Sub");
        }

        public virtual SValue Mul(SValue other)
        {
            throw NotSupportedOn("Mul");
        }

        public virtual SValue Div(SValue other)
        {
            throw NotSupportedOn("Div");
        }

        public virtual SValue Mod(SValue other)
        {
            throw NotSupportedOn("Mod");
        }

        public virtual SValue Idx(SValue other)
        {
            throw NotSupportedOn("Idx");
        }

        public virtual SValue Dot(SValue other) {
            throw NotSupportedOn("Dot");
        }

        public virtual SValue Equals(SValue other) {
            throw NotSupportedOn("Equals");
        }

        public virtual SValue LessThan(SValue other) {
            throw NotSupportedOn("LessThan");
        }

        public virtual SValue LessThanEqu(SValue other) {
            throw NotSupportedOn("LessThanEqu");
        }

        public virtual SValue GreaterThan(SValue other) {
            throw NotSupportedOn("GreaterThan");
        }

        public virtual SValue GreaterThanEqu(SValue other) {
            throw NotSupportedOn("GreaterThanEqu");
        }

        public virtual SValue CastToBuiltin(SBuiltinType other) {
            throw NotSupportedOn("CastToBuiltin");
        }

        public virtual SValue Call(Scope scope, List<SValue> args) {
            throw NotSupportedOn("Call");
        }

        public virtual bool IsNull() {
            return false;
        }

        public virtual SValue Not()
        {
            throw NotSupportedOn("Not");
        }

        public virtual SValue ArithNot()
        {
            throw NotSupportedOn("ArithNot");
        }

        public abstract bool IsTruthy();

        public override string ToString()
        {
            return BuiltinName.ToString();
        }

        public virtual SString ToSpagString()
        {
            return new SString("<unknown of type " + BuiltinName.ToString() + ">");
        }

        protected NotImplementedException NotSupportedBetween(SValue other, string type) {
            return new NotImplementedException(type + " not supported between " + GetType().Name + " and " + other.GetType().Name);
        }

        protected NotImplementedException NotSupportedOn(string type) {
            return new NotImplementedException(type + " is not supported on " + GetType().Name);
        }

        protected ArgumentException CastInvalid(string type) {
            return new ArgumentException(GetType().Name + " can not be cast to " + type);
        }

        internal void CopyMeta(ref SValue other) {
            other.TypeIsFixed = TypeIsFixed;
        }
    }

    public class SNativeLibraryImporter : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.NativeLibraryImporter;
        public Action<Scope> Import { get; set; } = (Scope scope) => { };

        public SNativeLibraryImporter(Action<Scope> import) {
            Import = import;
        }

        public override bool IsTruthy() {
            return true;
        }
    }

    public class SNativeFunction : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.NativeFunc;
        public Func<Scope, List<SValue>, SValue> Impl { get; set; }

        public SNativeFunction(Func<Scope, List<SValue>, SValue> impl) {
            Impl = impl;
        }

        /// <summary>
        /// NOTE: The scope in SNativeFunction is the calling scope, but not in SFunction!
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override SValue Call(Scope scope, List<SValue> args) {
            return Impl(scope, args);
        }

        public override bool IsTruthy() {
            return true;
        }
    }

    public class SFunction : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Function;
        public string FunctionName { get; set; }
        public List<string> Args { get; set; }
        public SyntaxNode Callback { get; set; }
        public Scope DefiningScope { get; set; }

        public SFunction(Scope definingScope, string functionName, List<string> args, SyntaxNode callback) {
            DefiningScope = definingScope;
            FunctionName = functionName;
            Args = args;
            Callback = callback;
        }

        public override SValue Call(Scope scope, List<SValue> args) {
            if (args.Count != Args.Count) throw new Exception(FunctionName + " expected " + Args.Count + " arguments. (" + string.Join(", ", Args) + ")");

            Scope funcScope = new(DefiningScope);
            
            for(int i = 0; i < Args.Count; i++) {
                funcScope.Set(Args[i], args[i]);
            }

            Callback.Evaluate(funcScope);

            return funcScope.ReturnValue;
        }

        public override bool IsTruthy() {
            return true;
        }
    }

    public class SNull : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Null;

        public override bool IsNull() {
            return true;
        }

        public override bool IsTruthy() {
            return false;
        }

        public override SString ToSpagString() {
            return new("null");
        }

        public override string ToString() {
            return "<SNull>";
        }
    }

    public class SInt : SValue
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Int;
        public int Value { get; set; }

        public SInt() {
            Value = 0;
        }

        public SInt(int value)
        {
            Value = value;
        }

        public override SValue Add(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Add on SInt and " + other.GetType().Name);

            return new SInt(Value + otherInt.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Sub on SInt and " + other.GetType().Name);
            return new SInt(Value - otherInt.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mul on SInt and " + other.GetType().Name);
            return new SInt(Value * otherInt.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Div on SInt and " + other.GetType().Name);
            return new SInt(Value / otherInt.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mod on SInt and " + other.GetType().Name);
            return new SInt(Value % otherInt.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform EqualsCheck on SInt and " + other.GetType().Name);
            return new SInt(Value == otherInt.Value ? 1 : 0);
        }

        public override SValue LessThan(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform LessThanCheck on SInt and " + other.GetType().Name);
            return new SInt(Value < otherInt.Value ? 1 : 0);
        }

        public override SValue LessThanEqu(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform LessThanEquCheck on SInt and " + other.GetType().Name);
            return new SInt(Value <= otherInt.Value ? 1 : 0);
        }

        public override SValue GreaterThan(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform GreaterThanCheck on SInt and " + other.GetType().Name);
            return new SInt(Value > otherInt.Value ? 1 : 0);
        }

        public override SValue GreaterThanEqu(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform GreaterThanEquCheck on SInt and " + other.GetType().Name);
            return new SInt(Value >= otherInt.Value ? 1 : 0);
        }

        public override SValue CastToBuiltin(SBuiltinType other) {
            switch(other) {
                case SBuiltinType.Int:
                    return new SInt(Value);
                case SBuiltinType.Float:
                    return new SFloat(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy() {
            return Value == 1;
        }

        public override string ToString() {
            return $"<{GetType().Name} value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString(Value.ToString());
        }
    }

    public class SFloat : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Float;
        public float Value { get; set; }

        public SFloat() {
            Value = 0;
        }

        public SFloat(float value) {
            Debug.WriteLine("Setting to " + value);
            Value = value;
            Debug.WriteLine("Set to " + Value);
        }

        public override SValue Add(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Add on SFloat and " + other.GetType().Name);
            return new SFloat(Value + otherInt.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Sub on SFloat and " + other.GetType().Name);
            return new SFloat(Value - otherInt.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Mul on SFloat and " + other.GetType().Name);
            return new SFloat(Value * otherInt.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Div on SFloat and " + other.GetType().Name);
            return new SFloat(Value / otherInt.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Mod on SFloat and " + other.GetType().Name);
            return new SFloat(Value % otherInt.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform EqualsCheck on SFloat and " + other.GetType().Name);
            return new SInt(Value == otherInt.Value ? 1 : 0);
        }

        public override SValue CastToBuiltin(SBuiltinType other) {
            switch (other) {
                case SBuiltinType.Int:
                    return new SInt((int)Value);
                case SBuiltinType.Float:
                    return new SFloat(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy() {
            return Value == 1;
        }

        public override string ToString() {
            return $"<{GetType().Name} value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString(Value.ToString());
        }
    }

    public class SString : SValue
    {
        public string Value { get; set; }
        public override SBuiltinType BuiltinName => SBuiltinType.String;

        public SString() { }
        public SString(string value)
        {
            Value = value;
        }

        public override SString ToSpagString()
        {
            return new SString(Value);
        }

        public override string ToString()
        {
            return $"<SString Value={Value}>";
        }

        public override SValue Add(SValue other)
        {
            if (other is not SString @string) throw new NotImplementedException();
            return new SString(Value + @string.Value);
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }

    public class SList : SValue {
        public List<SValue> Value { get; set; } = new();
        public override SBuiltinType BuiltinName => SBuiltinType.List;

        public SList() { }

        public override SString ToSpagString() {
            return new SString("[" + string.Join(", ", Value.Select((v) => v.ToSpagString().Value)) + "]");
        }

        public override string ToString() {
            return $"<SString Value={string.Join(", ", Value)}>";
        }

        public override SValue Idx(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can only index SList with integers, got " + other.GetType().Name);

            if (otherInt.Value < 0 || otherInt.Value > Value.Count - 1) throw new Exception("Out of bounds access. SList had " + Value.Count + " elements, but index " + otherInt.Value + " was accessed");
            return Value[otherInt.Value];
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }

    public struct PrimitiveCompatibility
    {
        public SBuiltinType LHS, RHS;
        public SyntaxType Operator;

        public PrimitiveCompatibility(SBuiltinType lhs, SyntaxType op, SBuiltinType rhs)
        {
            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }
    }

    public enum NodeType
    {
        Return,
        BinaryExpression,
        Token,
        BooleanLiteral,
        Block,
        Continue,
        Break,
        InitVariable,
        AssignVariable,
        UnaryExpression,
        Dot,
        Call,
        IntLiteral,
        FloatLiteral,
        StringLiteral,
        Identifier,
        List,
        If,
        For,
        Cast,
        While,
        FunctionDefinition,
        NativeImport
    }

    public enum SBuiltinType
    {
        String,
        Int,
        Float,
        List,
        Null,
        NativeFunc,
        Function,
        NativeLibraryImporter,
    }
}
