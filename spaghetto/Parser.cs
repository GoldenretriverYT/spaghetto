using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class Parser {
        public List<Token> tokens;
        public int tokenIdx = -1;
        public Token currentToken;

        public Parser(List<Token> tokens) {
            this.tokens = tokens;
            Advance();
        }

        public Token Advance() {
            tokenIdx++;

            if(tokenIdx < tokens.Count) {
                currentToken = tokens[tokenIdx];
            }

            return currentToken;
        }

        public ParseResult Parse() {
            ParseResult res = Expression();

            if(res.error == null && currentToken.type != TokenType.EndOfFile) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '+', '-', '*', '/', '^', '==', '!=', '<', '>', <=', '>=', 'and' or 'or'"));
            }

            return res;
        }

        public ParseResult Call() {
            System.Diagnostics.Debug.WriteLine("[parse] Call");

            ParseResult res = new();
            Node atom = res.Register(Atom());
            if (res.error) return res;

            if(currentToken.type == TokenType.LeftParen) {
                res.RegisterAdvancement();
                Advance();

                List<Node> argNodes = new();

                if (currentToken.type == TokenType.RightParen) {
                    res.RegisterAdvancement();
                    Advance();
                }else {
                    argNodes.Add(res.Register(Expression()));
                    if (res.error) return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected a numeric value, identifier, '[', ')', '+', '-', 'if', 'for', 'var', 'while', 'func' or parentheses"));

                    while(currentToken.type == TokenType.Comma) {
                        res.RegisterAdvancement();
                        Advance();

                        argNodes.Add(res.Register(Expression()));
                        if (res.error) return res;
                    }

                    if (currentToken.type != TokenType.RightParen) {
                        return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected ',', ')'"));
                    }

                    res.RegisterAdvancement();
                    Advance();
                }

                return res.Success(new CallNode(atom, argNodes));
            }

            return res.Success(atom);
        }

        public ParseResult Atom() {
            System.Diagnostics.Debug.WriteLine("[parse] Atom");

            ParseResult res = new ParseResult();
            Token tok = currentToken;

            if (tok.type == TokenType.Int || tok.type == TokenType.Float) {
                res.RegisterAdvancement();
                Advance();
                return res.Success(new NumberNode(tok));

            } else if (tok.type == TokenType.String) {
                res.RegisterAdvancement();
                Advance();
                return res.Success(new StringNode(tok));

            } else if (tok.type == TokenType.Identifier) {
                res.RegisterAdvancement();
                Advance();
                return res.Success(new VariableAccessNode(tok));

            } else if (tok.type == TokenType.LeftParen) {
                res.RegisterAdvancement();
                Advance();
                Node expr = res.Register(Expression());
                if (res.error != null) return res;

                if (currentToken.type == TokenType.RightParen) {
                    res.RegisterAdvancement();
                    Advance();
                    return res.Success(expr);
                } else {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected closing parantherese"));
                }

            } else if (tok.type == TokenType.LeftSqBracket) {
                Node listExpr = res.Register(ListExpression());
                if (res.error != null) return res;
                return res.Success(listExpr);

            } else if (tok.Matches(TokenType.Keyword, "if")) {
                Node ifExpr = res.Register(IfExpression());
                if (res.error != null) return res;
                return res.Success(ifExpr);

            } else if (tok.Matches(TokenType.Keyword, "for")) {
                Node forExpr = res.Register(ForExpression());
                if (res.error != null) return res;
                return res.Success(forExpr);

            } else if (tok.Matches(TokenType.Keyword, "while")) {
                Node whileExpr = res.Register(WhileExpression());
                if (res.error != null) return res;
                return res.Success(whileExpr);

            } else if (tok.Matches(TokenType.Keyword, "func")) {
                Node funcDefExpr = res.Register(FunctionDefinition());
                if (res.error != null) return res;
                return res.Success(funcDefExpr);
            }

            return res.Failure(new IllegalSyntaxError(tok.posStart, tok.posEnd, "Expected a numeric value, identifier, '+', '-', '( , '[', 'if', 'for', 'var', 'while', 'func' or parentheses"));
        }

        public ParseResult ListExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] ListExpression");

            ParseResult res = new();
            List<Node> elementNodes = new();
            Position posStart = currentToken.posStart.Copy();

            if(currentToken.type != TokenType.LeftSqBracket) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '["));
            }

            res.RegisterAdvancement();
            Advance();

            if (currentToken.type == TokenType.RightSqBracket) {
                res.RegisterAdvancement();
                Advance();
            }else {
                elementNodes.Add(res.Register(Expression()));
                if (res.error) return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected a numeric value, identifier, '[', ']', '+', '-', 'if', 'for', 'var', 'while', 'func' or parentheses"));

                while (currentToken.type == TokenType.Comma) {
                    res.RegisterAdvancement();
                    Advance();

                    elementNodes.Add(res.Register(Expression()));
                    if (res.error) return res;
                }

                if (currentToken.type != TokenType.RightSqBracket) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected ',', ']'"));
                }

                res.RegisterAdvancement();
                Advance();
            }

            return res.Success(new ListNode(elementNodes, posStart, currentToken.posEnd.Copy()));
        }

        public ParseResult FunctionDefinition() {
            System.Diagnostics.Debug.WriteLine("[parse] FunctionDefinition");

            ParseResult res = new();

            if (!(currentToken.Matches(TokenType.Keyword, "func"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'func'"));
            }

            res.RegisterAdvancement();
            Advance();

            Token varNameToken = null;

            if(currentToken.type == TokenType.Identifier) {
                varNameToken = currentToken;
                res.RegisterAdvancement();
                Advance();

                if (!(currentToken.type == TokenType.LeftParen)) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '('"));
                }
            }else {
                if (!(currentToken.type == TokenType.LeftParen)) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'identifier' or '('"));
                }
            }

            res.RegisterAdvancement();
            Advance();

            List<Token> argNameTokens = new();

            if (currentToken.type == TokenType.Identifier) {
                argNameTokens.Add(currentToken);
                res.RegisterAdvancement();
                Advance();

                while(currentToken.type == TokenType.Comma) {
                    res.RegisterAdvancement();
                    Advance();

                    if (!(currentToken.type == TokenType.Identifier)) {
                        return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected identifier"));
                    }

                    argNameTokens.Add(currentToken);
                    res.RegisterAdvancement();
                    Advance();
                }

                if (!(currentToken.type == TokenType.RightParen)) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected ',' or ')'"));
                }
            }else {
                if (!(currentToken.type == TokenType.RightParen)) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected identifier or ')'"));
                }
            }

            res.RegisterAdvancement();
            Advance();

            if(currentToken.type != TokenType.Arrow) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '->'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node nodeToReturn = res.Register(Expression());
            if (res.error) return res;

            return res.Success(new FunctionDefinitionNode(varNameToken, argNameTokens, nodeToReturn));
        }

        public ParseResult IfExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] IfExpression");

            ParseResult res = new();
            List<(Node, Node)> cases = new();

            if(!(currentToken.Matches(TokenType.Keyword, "if"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'if'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node condition = res.Register(Expression());
            if (res.error) return res;

            if(!(currentToken.Matches(TokenType.Keyword, "then"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'then'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node expr = res.Register(Expression());
            if (res.error) return res;
            cases.Add((condition, expr));

            while(currentToken.Matches(TokenType.Keyword, "elseif")) {
                res.RegisterAdvancement();
                Advance();

                condition = res.Register(Expression());
                if (res.error) return res;

                if (!(currentToken.Matches(TokenType.Keyword, "then"))) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'then'"));
                }

                res.RegisterAdvancement();
                Advance();

                expr = res.Register(Expression());
                if (res.error) return res;
                cases.Add((condition, expr));
            }

            if (currentToken.Matches(TokenType.Keyword, "else")) {
                res.RegisterAdvancement();
                Advance();

                expr = res.Register(Expression());
                if (res.error) return res;
                cases.Add((new NumberNode(new Token(TokenType.Float, 1d)), expr));
            }

            return res.Success(new IfNode(cases));
        }

        public ParseResult ForExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] ForExpression");

            ParseResult res = new();

            res.RegisterAdvancement();
            Advance();

            if (currentToken.type != TokenType.Identifier) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected identifier"));
            }

            Token varName = currentToken;
            res.RegisterAdvancement();
            Advance();

            if (currentToken.type != TokenType.Equals) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected equals sign"));
            }

            res.RegisterAdvancement();
            Advance();

            Node varStartExpression = res.Register(Expression());
            if (res.error != null) return res;

            if(!(currentToken.Matches(TokenType.Keyword, "while"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'while'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node condition = res.Register(Expression());
            if (res.error) return res;

            if (!((currentToken.Matches(TokenType.Keyword, "also")) || (currentToken.Matches(TokenType.Keyword, "step")))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'also' or 'step'"));
            }

            bool isStep = currentToken.Matches(TokenType.Keyword, "step");

            res.RegisterAdvancement();
            Advance();

            Node continuationExpression = res.Register(Expression());
            if (res.error) return res;

            if (!(currentToken.Matches(TokenType.Keyword, "then"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'then'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node func = res.Register(Expression());
            if (res.error) return res;

            return res.Success(new ForNode(varName, varStartExpression, condition, continuationExpression, isStep, func));
        }

        public ParseResult WhileExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] WhileExpression");

            ParseResult res = new();

            res.RegisterAdvancement();
            Advance();

            Node condition = res.Register(Expression());
            if (res.error) return res;

            if (!(currentToken.Matches(TokenType.Keyword, "then"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'then'"));
            }

            res.RegisterAdvancement();
            Advance();

            Node func = res.Register(Expression());
            if (res.error) return res;

            return res.Success(new WhileNode(condition, func));
        }

        public ParseResult Power() {
            System.Diagnostics.Debug.WriteLine("[parse] Power");

            return BinaryOperation(() => { return Call(); }, new List<TokenType>() { TokenType.Pow }, () => { return Factor(); });
        }

        public ParseResult Factor() {
            System.Diagnostics.Debug.WriteLine("[parse] Factor");

            ParseResult res = new ParseResult();
            Token tok = currentToken;

            if(tok.type == TokenType.Plus || tok.type == TokenType.Minus) {
                res.RegisterAdvancement();
                Advance();
                Node factor = res.Register(Factor());
                if (res.error != null) return res;
                return res.Success(new UnaryOperatorNode(tok, factor));
            }

            return Power();
        }

        public ParseResult Term() {
            System.Diagnostics.Debug.WriteLine("[parse] Term");

            return BinaryOperation(() => { return Factor(); }, new List<TokenType>() { TokenType.Mul, TokenType.Div, TokenType.Mod, TokenType.Index });
        }

        public ParseResult Expression() {
            System.Diagnostics.Debug.WriteLine("[parse] Expression");

            ParseResult res = new();

            if(currentToken.Matches(TokenType.Keyword, "var")) {
                res.RegisterAdvancement();
                Advance();

                if (currentToken.type != TokenType.Identifier) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected identifier"));
                }

                Token varName = currentToken;
                res.RegisterAdvancement();
                Advance();

                if (currentToken.type != TokenType.Equals) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected equals sign"));
                }

                res.RegisterAdvancement();
                Advance();

                Node expr = res.Register(Expression());
                if (res.error != null) return res;
                return res.Success(new VariableAssignNode(varName, expr));
            }
            
            Node node = res.Register(BinaryOperation(() => { return CompExpression(); }, exactTokens: new() { new Token(TokenType.Keyword, "and"), new Token(TokenType.Keyword, "or") }));

            if (res.error) {
                System.Diagnostics.Debug.WriteLine(res.error.Message);
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'var', numeric value, identifier, '[', '-', '+', 'not' or parentheses"));
            }

            return res.Success(node);
        }

        public ParseResult CompExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] CompExpression");

            ParseResult res = new();
            Node node = null;
            if (currentToken.Matches(TokenType.Keyword, "not")) {
                Token opToken = currentToken;
                res.RegisterAdvancement();
                Advance();

                node = res.Register(CompExpression());

                if (res.error != null) return res;
                return res.Success(new UnaryOperatorNode(opToken, node));
            }

            node = res.Register(BinaryOperation(() => { return ArithmeticExpression(); },
                new List<TokenType>() { TokenType.EqualsEquals, TokenType.NotEquals, TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanOrEquals, TokenType.GreaterThanOrEquals }));

            if(res.error != null) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected a numeric value, identifier, '[', '+', '-', 'not' or parentheses"));
            }

            return res.Success(node);
        }

        public ParseResult ArithmeticExpression() {
            System.Diagnostics.Debug.WriteLine("[parse] ArithmeticExpression");

            return BinaryOperation(() => { return Term(); }, new List<TokenType>() { TokenType.Plus, TokenType.Minus });
        }


        //List<Token> exactTokens = null
        public ParseResult BinaryOperation(Func<ParseResult> funcLeft, List<TokenType> tokenTypes, Func<ParseResult> funcRight = null) {
            System.Diagnostics.Debug.WriteLine("[parse] BinaryOperation");

            if (tokenTypes == null) tokenTypes = new();

            ParseResult res = new ParseResult();

            Node left = res.Register(funcLeft());
            Node right;
            Token opToken;

            if (res.error != null) return res;

            System.Diagnostics.Debug.WriteLine(currentToken.type);
            System.Diagnostics.Debug.WriteLine(tokenTypes.Join(", "));

            while (tokenTypes.Contains(currentToken.type)) {
                System.Diagnostics.Debug.WriteLine("TOKENTYPE " + currentToken.type + " FOUND! L: " + funcLeft + " | R: " + funcRight);

                opToken = currentToken;
                res.RegisterAdvancement();
                Advance();
                right = res.Register((funcRight == null ? funcLeft() : funcRight()));
                if (right == null) System.Diagnostics.Debug.WriteLine("EEEE!!!!!!! RIGHT IS NULL");

                System.Diagnostics.Debug.WriteLine("531 | Checking error");
                if (res.error != null) return res;
                System.Diagnostics.Debug.WriteLine("533 | No error!");

                left = new BinaryOperationNode(left, opToken, right);

                System.Diagnostics.Debug.WriteLine("Set left to BinOpNode(" + left + ", " + opToken + ", " + right + ")");
            }

            return res.Success(left);
        }

        public ParseResult BinaryOperation(Func<ParseResult> funcLeft, List<Token> exactTokens, Func<ParseResult> funcRight = null) {
            System.Diagnostics.Debug.WriteLine("[parse] BinaryOperation");

            if (funcRight == null) funcRight = funcLeft;

            ParseResult res = new ParseResult();

            Node left = res.Register(funcLeft());
            Node right;
            Token opToken;

            if (res.error != null) return res;

            while (TokenListContains(exactTokens, currentToken)) {
                opToken = currentToken;
                res.RegisterAdvancement();
                Advance();
                right = res.Register(funcRight());

                if (res.error != null) return res;

                left = new BinaryOperationNode(left, opToken, right);
            }

            return res.Success(left);
        }

        public bool TokenListContains(List<Token> exactTokens, Token currentToken) {
            foreach(Token tok in exactTokens) {
                if (currentToken.Matches(tok.type, (string)tok.value)) return true;
            }
            
            return false;
        }
    }

    internal class ParseResult {
        public SpaghettoException error;
        public Node node;
        public int advanceCount = 0;

        public Node Register(ParseResult parseResult) {
            advanceCount += parseResult.advanceCount;
            if (parseResult.error != null) this.error = parseResult.error;
            return parseResult.node;
        }

        public void RegisterAdvancement() {
            advanceCount++;
        }

        public ParseResult Success(Node node) {
            this.node = node;
            return this;
        }

        public ParseResult Failure(SpaghettoException error, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) {
            System.Diagnostics.Debug.WriteLine("Failure at " + memberName + " in " + fileName + ":" + lineNumber);

            System.Diagnostics.Debug.WriteLine("ParserResult Failure: " + error.Message);
            if(this.error == null || advanceCount == 0)
                this.error = error;
            return this;
        }
    }
}
