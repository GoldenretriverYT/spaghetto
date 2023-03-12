using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public SyntaxNode ParseScopedStatements() {
            MatchToken(SyntaxType.LBraces);
            List<SyntaxNode> nodes = new();

            while(Current.Type != SyntaxType.RBraces) {
                if (Current.Type == SyntaxType.EOF) throw new Exception("Unclosed block at " + Current.Position);

                nodes.Add(ParseStatement());
            }

            MatchToken(SyntaxType.RBraces);
        }

        public SyntaxNode ParseStatement() {
            if(Current.Type == SyntaxType.Keyword && Current.Value == "return") {
                if(Peek(1).Type == SyntaxType.Semicolon) {
                    Position++;
                    return new ReturnNode();
                }else {
                    Position++;
                    return new ReturnNode(ParseExpression());
                }
            }else if(Current.Type == SyntaxType.Keyword && Current.Value == "continue") {
                Position++;
                return new ContinueNode();
            } else if (Current.Type == SyntaxType.Keyword && Current.Value == "break") {
                Position++;
                return new BreakNode();
            }else {
                Position++;
                var exprNode = ParseExpression();
                MatchToken(SyntaxType.Semicolon);

                return exprNode;
            }
        }

        public SyntaxNode ParseExpression() {
            if(Current.Type == SyntaxType.Keyword && Current.Value == "var") {
                Position++;
                var ident = MatchToken(SyntaxType.Identifier);

                if(Current.Type == SyntaxType.Equals) {
                    Position++;
                    var expr = ParseExpression();
                    return new InitVariableNode(ident, expr);
                }else {
                    return new InitVariableNode(ident);
                }
            }else if(Current.Type == SyntaxType.Identifier) {
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
                return new UnaryExpressionNode(SyntaxType.Bang, ParseCompExpression());
            }else {
                return BinaryOperation(() => { return ParseArithmeticExpression() },
                    new List<SyntaxType>() {
                        SyntaxType.EqualsEquals, SyntaxType.LessThan, SyntaxType.LessThanEqu, SyntaxType.GreaterThan, SyntaxType.GreatherThanEqu
                    });
            }
        }

        public SyntaxNode ParseArithmeticExpression() {
            return BinaryOperation(() => { return ParseTermExpression(); }, new() { SyntaxType.Plus, SyntaxType.Minus });
        }

        public SyntaxNode ParseTermExpression() {
            return BinaryOperation(() => { return ParseFactorExpression(); }, new() { SyntaxType.Mul, SyntaxType.Div, SyntaxType.Mod, SyntaxType.Index });
        }

        public SyntaxNode ParseFactorExpression() {
            if(Current.Type is SyntaxType.Plus or SyntaxType.Minus) {
                Position++;
                var factor = ParseFactorExpression();
                return new UnaryExpressionNode(Peek(-1).Type, factor);
            }

            return ParsePowerExpression();
        }

        public SyntaxNode ParsePowerExpression() {
            return BinaryOperation(() => { return ParseDotExpression(); }, new() { SyntaxType.Pow }, () => { return ParseFactorExpression(); });
        }

        public SyntaxNode ParseDotExpression() {
            var callNode = ParseCallExpression();
            DotNode accessStack = new(callNode);

            if(Current.Type is SyntaxType.Dot) {
                while(Current.Type is SyntaxType.Dot) {
                    Position++;

                    if(Current.Type is SyntaxType.Identifier) {
                        var n = ParseCallExpression();

                        accessStack.NextNodes.Add(n);
                    }
                }
            }

            return accessStack;
        }

        public SyntaxNode ParseCallExpression() {
            var atomNode = ParseAtomExpression();

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

        public SyntaxNode ParseAtomExpression() {
            if(Current.Type is SyntaxType.Int or SyntaxType.Float or SyntaxType.String or SyntaxType.Identifier) {
                Position++;
                return new LiteralNode(Peek(-1));
            }else if(Current.Type is SyntaxType.LParen) {
                var expr = ParseExpression();

                MatchToken(SyntaxType.RParen);
            } else if (Current.Type is SyntaxType.LSqBracket) {
                return ParseListExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "if") {
                return ParseIfExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "for") {
                return ParseForExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "while") {
                return ParseWhileExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "func") {
                return ParseFuncExpression();
            }
        }

        public SyntaxNode ParseListExpression() {
            var lsqTok = MatchToken(SyntaxType.LSqBracket);

            List<SyntaxNode> list = new();

            do {
                var expr = ParseExpression();
                list.Add(expr);

                Position++;
            } while (Current.Type == SyntaxType.Comma);

            MatchToken(SyntaxType.RSqBracket);

            return new ListNode(list);
        }

        public SyntaxNode ParseIfExpression() {

        }

        public SyntaxNode BinaryOperation(Func<SyntaxNode> leftParse, List<SyntaxType> allowedTypes, Func<SyntaxNode> rightParse) {
            var left = leftParse();
            SyntaxNode right;
            SyntaxToken operatorToken = default;

            while(allowedTypes.Contains(Current.Type)) {
                operatorToken = Current;
                Position++;
                right = (rightParse ?? leftParse)();

                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }
    }
}
