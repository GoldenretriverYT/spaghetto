using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using spaghetto.Parsing.Nodes;

namespace spaghetto.Parsing {
    public class Parser {
        public List<SyntaxToken> Tokens { get; set; }
        public string Code { get; }

        public int Position = 0;

        public SyntaxToken Current => Peek(0);
        public static SyntaxToken EmptyToken => new(SyntaxType.EOF, 0, null, null);

        public SyntaxToken Peek(int off = 0) {
            if (Position + off >= Tokens.Count || Position + off < 0) return new(SyntaxType.BadToken, 0, null, "");
            return Tokens[Position + off];
        }

        public SyntaxToken MatchToken(SyntaxType type) {
            if(Current.Type == type) {
                Position++;
                return Peek(-1);
            }

            throw MakeException("Unexpected token " + Current.Type + "; expected " + type);
        }

        public bool MatchTokenOptionally(SyntaxType type, out SyntaxToken tok) {
            if (Current.Type == type) {
                Position++;
                tok = Peek(-1);
                return true;
            }

            tok = default;
            return false;
        }

        public SyntaxToken MatchTokenWithValue(SyntaxType type, object value)
        {
            if (Current.Type == type && Current.Value == value) {
                Position++;
                return Peek(-1);
            }

            throw MakeException("Unexpected token " + Current.Type + "; expected " + type + " with value " + value);
        }

        public SyntaxToken MatchKeyword(string value) {
            if (Current.Type == SyntaxType.Keyword && Current.Text == value) {
                Position++;
                return Peek(-1);
            }

            throw MakeException("Unexpected token " + Current.Type + "; expected Keyword with value " + value);
        }

        public Parser(List<SyntaxToken> tokens, string code) {
            Tokens = tokens;
            Code = code;
        }

        public SyntaxNode Parse() {
            return ParseStatements();
        }

        public SyntaxNode ParseStatements() {
            List<SyntaxNode> nodes = new();

            while(Current.Type != SyntaxType.EOF) {
                nodes.Add(ParseStatement());
            }

            return new BlockNode(Tokens.FirstOrDefault(EmptyToken), Tokens.LastOrDefault(EmptyToken), nodes, false);
        }

        public SyntaxNode ParseScopedStatements() {
            var startingBrace = MatchToken(SyntaxType.LBraces);
            List<SyntaxNode> nodes = new();

            while(Current.Type != SyntaxType.RBraces) {
                if (Current.Type == SyntaxType.EOF) throw MakeException("Unclosed block at " + Current.Position);

                nodes.Add(ParseStatement());
            }

            var endingBrace = MatchToken(SyntaxType.RBraces);

            return new BlockNode(startingBrace, endingBrace, nodes);
        }

        public SyntaxNode ParseStatement() {
            if (Current.Type == SyntaxType.Keyword && Current.Text == "return") {
                var retTok = Current;

                if (Peek(1).Type == SyntaxType.Semicolon) {
                    Position += 2;
                    var ret = new ReturnNode(retTok);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return ret;
                } else {
                    Position++;
                    var ret = new ReturnNode(retTok, ParseExpression());
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return ret;
                }
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "continue") {
                var n = new ContinueNode(Current);

                Position++;
                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                return n;
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "break") {
                var n = new BreakNode(Current);

                Position++;
                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                return n;
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "import") {
                Position++;

                if (Current.Type == SyntaxType.Keyword && Current.Text == "native") {
                    Position++;
                    var ident = MatchToken(SyntaxType.Identifier);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);

                    return new NativeImportNode(ident);
                } else {
                    var path = MatchToken(SyntaxType.String);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);

                    return new ImportNode(path);
                }
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "export") {
                Position++;

                var ident = MatchToken(SyntaxType.Identifier);
                MatchTokenOptionally(SyntaxType.Semicolon, out _);
                return new ExportNode(ident);
            } else if (Current.Type == SyntaxType.Keyword && Current.Text == "class") {
                var c = ParseClassDefinition();
                MatchTokenOptionally(SyntaxType.Semicolon, out _);
                return c;
            } else {
                var exprNode = ParseExpression();
                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                return exprNode;
            }
        }

        private SyntaxNode ParseClassDefinition() {
            MatchKeyword("class");
            var fixedProps = false;

            if(Current.Type == SyntaxType.Keyword && Current.Text == "fixedprops") {
                Position++;
                fixedProps = true;
            }

            var className = MatchToken(SyntaxType.Identifier);

            MatchToken(SyntaxType.LBraces);
            var body = ParseClassBody();
            MatchToken(SyntaxType.RBraces);

            return new ClassDefinitionNode(className, body, fixedProps);
        }

        private List<SyntaxNode> ParseClassBody() {
            List<SyntaxNode> nodes = new();

            while(Current.Type == SyntaxType.Keyword && (Current.Text == "func" || Current.Text == "prop")) {
                if (Current.Text == "func") {
                    Position++;
                    var isStatic = false;

                    if (Current.Type == SyntaxType.Keyword && Current.Text == "static") {
                        Position++;
                        isStatic = true;
                    }

                    var name = MatchToken(SyntaxType.Identifier);
                    var args = ParseFunctionArgs();
                    var body = ParseScopedStatements();

                    nodes.Add(new ClassFunctionDefinitionNode(name, args, body, isStatic));
                } else if (Current.Text == "prop") {
                    Position++;
                    var isStatic = false;

                    if (Current.Type == SyntaxType.Keyword && Current.Text == "static") {
                        Position++;
                        isStatic = true;
                    }

                    var name = MatchToken(SyntaxType.Identifier);
                    MatchToken(SyntaxType.Equals);
                    var expr = ParseExpression();

                    nodes.Add(new ClassPropDefinitionNode(name, expr, isStatic));
                }

                while(Current.Type == SyntaxType.Semicolon) {
                    Position++;
                }
            }

            return nodes;
        }

        public SyntaxNode ParseExpression() {
            if(Current.Type == SyntaxType.Keyword && Current.Text == "var" ||
                Current.Type == SyntaxType.Keyword && Current.Text == "const") {
                return ParseVariableDefinition();
            }else if(Current.Type == SyntaxType.Identifier && Peek(1).Type == SyntaxType.Equals) {
                var ident = MatchToken(SyntaxType.Identifier);
                MatchToken(SyntaxType.Equals);
                var expr = ParseExpression();
                return new AssignVariableNode(ident, expr);
            } else if (Current.Type == SyntaxType.Identifier && (Peek(1).Type is SyntaxType.Equals or SyntaxType.PlusEqu or SyntaxType.MinusEqu or SyntaxType.ModEqu or SyntaxType.DivEqu or SyntaxType.MulEqu)) {
                var ident = MatchToken(SyntaxType.Identifier);

                var assignTok = Current;
                assignTok.Type = MapEqualsTokens(assignTok.Type);
                Position++;

                var expr = ParseExpression();
                return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, expr));
            } else if (Current.Type == SyntaxType.Identifier && (Peek(1).Type is SyntaxType.PlusPlus or SyntaxType.MinusMinus)) {
                var ident = MatchToken(SyntaxType.Identifier);

                var assignTok = Current;
                assignTok.Type = MapDoubleTokens(assignTok.Type);
                Position++;

                return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, new IntLiteralNode(new SyntaxToken(SyntaxType.Int, assignTok.Position, 1, "1"))));
            } else {
                return BinaryOperation(() => ParseCompExpression(), new List<SyntaxType>() { SyntaxType.AndAnd, SyntaxType.OrOr });
            }
        }

        public SyntaxNode ParseVariableDefinition() {
            bool isConst = false;

            if(Current.Type == SyntaxType.Keyword && Current.Text == "const") {
                isConst = true;
                Position++;
            }

            bool fixedType = true;
            MatchKeyword("var");

            if (Current.Type == SyntaxType.Mod) {
                fixedType = false;
                Position++;
            }

            var ident = MatchToken(SyntaxType.Identifier);

            if (Current.Type == SyntaxType.Equals) {
                Position++;
                var expr = ParseExpression();
                return new InitVariableNode(ident, expr, fixedType, isConst);
            } else {
                return new InitVariableNode(ident, fixedType, isConst);
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
                var tok = Current;
                Position++;
                var factor = ParseFactorExpression();
                return new UnaryExpressionNode(tok, factor);
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
                        if (Peek(1).Type is SyntaxType.Equals) {
                            var ident = MatchToken(SyntaxType.Identifier);
                            MatchToken(SyntaxType.Equals);
                            var expr = ParseExpression();

                            accessStack.NextNodes.Add(new AssignVariableNode(ident, expr));
                        } else if (Peek(1).Type is SyntaxType.Equals or SyntaxType.PlusEqu or SyntaxType.MinusEqu or SyntaxType.ModEqu or SyntaxType.DivEqu or SyntaxType.MulEqu) {
                            var ident = MatchToken(SyntaxType.Identifier);

                            var assignTok = Current;
                            assignTok.Type = MapEqualsTokens(assignTok.Type);
                            Position++;

                            var expr = ParseExpression();
                            // TODO: Check if we can do this in a better way
                            var binOpDot = accessStack.Clone();
                            binOpDot.NextNodes.Add(new IdentifierNode(ident));

                            accessStack.NextNodes.Add(new AssignVariableNode(ident, new BinaryExpressionNode(binOpDot, assignTok, expr)));
                        } else if (Peek(1).Type is SyntaxType.PlusPlus or SyntaxType.MinusMinus) {
                            var ident = MatchToken(SyntaxType.Identifier);

                            var assignTok = Current;
                            assignTok.Type = MapDoubleTokens(assignTok.Type);
                            Position++;

                            var binOpDot = accessStack.Clone();
                            binOpDot.NextNodes.Add(new IdentifierNode(ident));
                            accessStack.NextNodes.Add(new AssignVariableNode(ident, new BinaryExpressionNode(binOpDot, assignTok, new IntLiteralNode(new SyntaxToken(SyntaxType.Int, assignTok.Position, 1, "1")))));
                        } else {
                            var n = ParseCallExpression();
                            accessStack.NextNodes.Add(n);
                        }
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
            } else if (Current.Type is SyntaxType.LParen) {
                Position++;
                var expr = ParseExpression();

                MatchToken(SyntaxType.RParen);

                return expr;
            } else if (Current.Type is SyntaxType.LSqBracket) {
                return ParseListExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "if") {
                return ParseIfExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "for") {
                return ParseForExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "repeat") {
                return ParseRepeatExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "while") {
                return ParseWhileExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "func") {
                return ParseFunctionExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Text == "new") {
                return ParseInstantiateExpression();
            } else {
                throw MakeException($"Unexpected token {Current.Type} at pos {Current.Position} in atom expression!");
            }
        }

        public SyntaxNode ParseListExpression() {
            var lsq = MatchToken(SyntaxType.LSqBracket);
            SyntaxToken rsq;

            List<SyntaxNode> list = new();

            if (Current.Type == SyntaxType.RSqBracket) {
                rsq = MatchToken(SyntaxType.RSqBracket);
            } else {
                var expr = ParseExpression();
                list.Add(expr);

                while (Current.Type == SyntaxType.Comma) {
                    Position++;
                    expr = ParseExpression();
                    list.Add(expr);
                }

                rsq = MatchToken(SyntaxType.RSqBracket);
            }

            return new ListNode(list, lsq, rsq);
        }

        public SyntaxNode ParseIfExpression() {
            var kw = MatchKeyword("if");

            IfNode node = new(kw);

            MatchToken(SyntaxType.LParen);
            var initialCond = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var lastBlock = ParseScopedOrStatement();

            node.AddCase(initialCond, lastBlock);

            while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif") {
                Position++;

                MatchToken(SyntaxType.LParen);
                var cond = ParseExpression();
                MatchToken(SyntaxType.RParen);
                lastBlock = ParseScopedOrStatement();

                node.AddCase(cond, lastBlock);
            }

            if(Current.Type == SyntaxType.Keyword && Current.Text == "else") {
                Position++;
                lastBlock = ParseScopedOrStatement();

                node.AddCase(new IntLiteralNode(new SyntaxToken(SyntaxType.Int, 0, 1, "1")), lastBlock);
            }

            node.EndPosition = lastBlock.EndPosition;

            return node;
        }

        public SyntaxNode ParseScopedOrStatement() {
            if (Current.Type == SyntaxType.LBraces)
                return ParseScopedStatements();
            else {
                var expr = ParseStatement();
                MatchTokenOptionally(SyntaxType.Semicolon, out _);
                return expr;
            }
        }

        public SyntaxNode ParseForExpression() {
            MatchKeyword("for");

            MatchToken(SyntaxType.LParen);
            var initialExpressionNode = ParseExpression();
            MatchTokenOptionally(SyntaxType.Semicolon, out _);
            var condNode = ParseExpression();
            MatchTokenOptionally(SyntaxType.Semicolon, out _);
            var stepNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedOrStatement();

            return new ForNode(initialExpressionNode, condNode, stepNode, block);
        }

        public SyntaxNode ParseRepeatExpression() {
            MatchKeyword("repeat");
            var keepScope = MatchTokenOptionally(SyntaxType.Bang, out _);

            MatchToken(SyntaxType.LParen);
            var timesExpr = ParseExpression();
            var timesTok = MatchKeyword("times");
            MatchToken(SyntaxType.RParen);

            var block = ParseScopedOrStatement();

            return new RepeatNode(timesExpr, block, keepScope);
        }

        public SyntaxNode ParseWhileExpression() {
            MatchKeyword("while");

            MatchToken(SyntaxType.LParen);
            var condNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedOrStatement();

            return new WhileNode(condNode, block);
        }

        public SyntaxNode ParseFunctionExpression() {
            MatchKeyword("func");

            SyntaxToken? nameToken = null;
            if(Current.Type == SyntaxType.Identifier)
                nameToken = MatchToken(SyntaxType.Identifier);

            var args = ParseFunctionArgs();

            SyntaxNode block;
            
            if(Current.Type == SyntaxType.LBraces) {
                block = ParseScopedStatements();
            } else {
                var arrow = MatchToken(SyntaxType.Arrow);
                block = ParseScopedOrStatement();
                block = new ReturnNode(arrow, block);
            }

            return new FunctionDefinitionNode(nameToken, args, block);
        }

        public SyntaxNode ParseInstantiateExpression() {
            Position++;
            var ident = MatchToken(SyntaxType.Identifier);

            List<SyntaxNode> argumentNodes = new();

            if (Current.Type is SyntaxType.LParen) {
                Position++;

                if (Current.Type is SyntaxType.RParen) {
                    Position++;
                } else {
                    argumentNodes.Add(ParseExpression());

                    while (Current.Type is SyntaxType.Comma) {
                        Position++;

                        argumentNodes.Add(ParseExpression());
                    }

                    MatchToken(SyntaxType.RParen);
                }
            }

            return new InstantiateNode(ident, argumentNodes);
        }

        public List<SyntaxToken> ParseFunctionArgs() {
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

            return args;
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

        public ParsingException MakeException(string msg) {
            return new ParsingException(msg, this, Position);
        }

        public SyntaxType MapEqualsTokens(SyntaxType type) => type switch {
            SyntaxType.PlusEqu => SyntaxType.Plus,
            SyntaxType.MinusEqu => SyntaxType.Minus,
            SyntaxType.ModEqu => SyntaxType.Mod,
            SyntaxType.MulEqu => SyntaxType.Mul,
            SyntaxType.DivEqu => SyntaxType.Div,
            _ => throw new Exception(type + " is not a equals token.")
        };

        public SyntaxType MapDoubleTokens(SyntaxType type) => type switch {
            SyntaxType.PlusPlus => SyntaxType.Plus,
            SyntaxType.MinusMinus => SyntaxType.Minus,
            _ => throw new Exception(type + " is not a double token.")
        };
    }

    internal class RepeatNode : SyntaxNode {
        private SyntaxNode timesExpr;
        private SyntaxNode block;
        private bool keepScope = false;

        public RepeatNode(SyntaxNode timesExpr, SyntaxNode block, bool keepScope = false) : base(timesExpr.StartPosition, block.EndPosition) {
            this.timesExpr = timesExpr;
            this.block = block;
            this.keepScope = keepScope;
        }

        public override NodeType Type => NodeType.Repeat;

        public override SValue Evaluate(Scope scope) {
            var timesRaw = timesExpr.Evaluate(scope);
            if (timesRaw is not SInt timesSInt) throw new Exception("Repeat x times expression must evaluate to SInt");
            var times = timesSInt.Value;

            if (keepScope) {
                if (block is not BlockNode blockNode) throw new Exception("Kept-scope repeat expressions must have a full body.");

                for (int i = 0; i < times; i++) {
                    foreach(var n in blockNode.Nodes) {
                        n.Evaluate(scope);
                    }
                }
            } else {
                for (int i = 0; i < times; i++) {
                    block.Evaluate(scope);
                }
            }

            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return timesExpr;
            yield return block;
        }
    }

    public class ParsingException : Exception {
        public override string Message => GetMessage();

        private string parseMessage;
        private Parser context;
        private int tokenIdx;

        private SyntaxToken CausingToken => context.Tokens[tokenIdx];
        private string SurroundingText => ""; // TODO

        public ParsingException(string parseMessage, Parser context, int tokenIdx = -1) {
            this.parseMessage = parseMessage;
            this.context = context;
            this.tokenIdx = tokenIdx == -1 ? context.Position : tokenIdx;
        }

        const string NORMAL = "\u001b[38;5;15m";
        const string ERROR = "\u001b[38;5;160m";

        // TODO: Clean up this shit
        public string GetMessage() {
            return $@"{parseMessage}
Position: {CausingToken.Position}";
        }

        public override string ToString() {
            return Message;
        }
    }
}
