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
            }else {
                // TODO: parse || and && lmao; totally forgot that :skull:
                return ParseCompExpression();
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
            var lastBlock = ParseScopedOrExpression();

            node.AddCase(initialCond, lastBlock);

            while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif") {
                Position++;

                MatchToken(SyntaxType.LParen);
                var cond = ParseExpression();
                MatchToken(SyntaxType.RParen);
                lastBlock = ParseScopedOrExpression();

                node.AddCase(cond, lastBlock);
            }

            if(Current.Type == SyntaxType.Keyword && Current.Text == "else") {
                Position++;
                lastBlock = ParseScopedOrExpression();

                node.AddCase(new IntLiteralNode(new SyntaxToken(SyntaxType.Int, 0, 1, "1")), lastBlock);
            }

            node.EndPosition = lastBlock.EndPosition;

            return node;
        }

        public SyntaxNode ParseScopedOrExpression() {
            if (Current.Type == SyntaxType.LBraces)
                return ParseScopedStatements();
            else {
                var expr = ParseExpression();
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
            var block = ParseScopedOrExpression();

            return new ForNode(initialExpressionNode, condNode, stepNode, block);
        }

        public SyntaxNode ParseWhileExpression() {
            MatchKeyword("while");

            MatchToken(SyntaxType.LParen);
            var condNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedOrExpression();

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
                block = ParseScopedOrExpression();
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
