using System;
using System.Collections.Generic;
using System.Linq;
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
            UpdateCurrentToken();
            return currentToken;
        }

        public Token Advance(ParseResult res) {
            res.RegisterAdvancement();
            return Advance();
        }

        public Token Reverse(int by=1) {
            tokenIdx -= by;
            UpdateCurrentToken();
            return currentToken;
        }

        public void UpdateCurrentToken() {
            if (tokenIdx >= 0 && tokenIdx < tokens.Count) {
                currentToken = tokens[tokenIdx];
            }
        }

        public ParseResult Parse() {
            ParseResult res = Statements();

            if(res.error == null && currentToken.type != TokenType.EndOfFile) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '+', '-', '*', '/', '^', '==', '!=', '<', '>', <=', '>=', 'and' or 'or'"));
            }

            return res;
        }

        public ParseResult Statements() {
            ParseResult res = new();
            List<Node> statements = new();
            var posStart = currentToken.posStart.Copy();

            while(currentToken.type == TokenType.NewLine) {
                res.RegisterAdvancement();
                Advance();
            }

            Node statement = res.Register(Statement());
            if (res.error is not null) return res;
            statements.Add(statement);

            bool moreStatements = true;

            while(true) {
                int newlineCount = 0;

                while(currentToken.type == TokenType.NewLine) {
                    res.RegisterAdvancement();
                    Advance();
                    newlineCount++;
                }

                if (newlineCount == 0) moreStatements = false;
                if (!moreStatements) break;

                statement = res.TryRegister(Statement());

                if (statement == null) {
                    Reverse(res.toReverseCount);
                    moreStatements = false;
                    continue;
                }

                statements.Add(statement);
            }

            return res.Success(new ListNode(statements, posStart, currentToken.posEnd.Copy()));
        }

        public ParseResult Statement() {
            ParseResult res = new();
            Position posStart = currentToken.posStart.Copy();

            if (currentToken.Matches(TokenType.Keyword, "return")) {
                Advance(res);

                Node expression = res.TryRegister(Expression());
                if (expression == null) {
                    Reverse(res.toReverseCount);
                }

                return res.Success(new ReturnNode(expression, posStart, currentToken.posStart.Copy()));
            } else if (currentToken.Matches(TokenType.Keyword, "continue")) {
                Advance(res);
                return res.Success(new ContinueNode(posStart, currentToken.posStart.Copy()));
            } else if (currentToken.Matches(TokenType.Keyword, "break")) {
                Advance(res);
                return res.Success(new BreakNode(posStart, currentToken.posStart.Copy()));
            }

            Node expr = res.Register(Expression());
            if (res.error) return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'var', numeric value, identifier, '[', '-', '+', 'not', 'if', 'for', 'while', 'func', 'return', 'continue', 'break' or parentheses")); ;

            return res.Success(expr);
        }

        public ParseResult Call() {
            //System.Diagnostics.Debug.WriteLine("[parse] Call");

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
            //System.Diagnostics.Debug.WriteLine("[parse] Atom");

            ParseResult res = new();
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
            //System.Diagnostics.Debug.WriteLine("[parse] ListExpression");

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
            //System.Diagnostics.Debug.WriteLine("[parse] FunctionDefinition");

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

            if (currentToken.type == TokenType.Arrow) {
                res.RegisterAdvancement();
                Advance();

                Node nodeToReturn = res.Register(Expression());
                if (res.error) return res;

                return res.Success(new FunctionDefinitionNode(varNameToken, argNameTokens, nodeToReturn, true));
            }

            if(currentToken.type != TokenType.NewLine) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected '->' or NewLine"));
            }

            Advance(res);

            Node body = res.Register(Statements());
            if (res.error) return res;

            if(!(currentToken.Matches(TokenType.Keyword, "end"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'end'"));
            }

            Advance(res);

            return res.Success(new FunctionDefinitionNode(varNameToken, argNameTokens, body, false));
        }

        public ParseResult IfExpression() {
            ParseResult res = new();
            IfCasesListNode cases = res.Register(IfExpressionCases("if")) as IfCasesListNode;
            if (res.error) return res;

            return res.Success(new IfNode(cases.cases));
        }

        public ParseResult IfExpressionCases(string keyword) {
            ParseResult res = new();
            List<(Node cond, Node statements, bool)> cases = new();

            if(!(currentToken.Matches(TokenType.Keyword, keyword))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, $"Expected '{keyword}'"));
            }

            Advance(res);

            Node condition = res.Register(Expression());
            if (res.error) return res;

            if (!(currentToken.Matches(TokenType.Keyword, "then"))) {
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, $"Expected '{keyword}'"));
            }

            Advance(res);

            if (currentToken.type == TokenType.NewLine) {
                Advance(res);

                Node statements = res.Register(Statements());
                if (res.error) return res;

                cases.Add((condition, statements, true));

                if(currentToken.Matches(TokenType.Keyword, "end")) {
                    Advance(res);
                }else {
                    List<(Node cond, Node statements, bool)> newCases = (res.Register(IfExpressionElseIfOrElse()) as IfCasesListNode).cases;
                    if (res.error) return res;

                    cases = cases.Concat(newCases).ToList();
                }
            }else {
                Node expr = res.Register(Statement());
                if (res.error) return res;
                cases.Add((condition, expr, false));

                List<(Node cond, Node statements, bool)> newCases = (res.Register(IfExpressionElseIfOrElse()) as IfCasesListNode).cases;
                if (res.error) return res;

                cases = cases.Concat(newCases).ToList();
            }

            return res.Success(new IfCasesListNode(cases));
        }

        public ParseResult IfExpressionElseIfOrElse() {
            ParseResult res = new();
            IfCasesListNode cases = null;

            if(currentToken.Matches(TokenType.Keyword, "elseif")) {
                cases = res.Register(IfExpressionElseIf()) as IfCasesListNode;
                if (res.error) return res;
            }else {
                cases = res.Register(IfExpressionElse()) as IfCasesListNode;
                if (cases.cases[0].expr == null) cases.cases.RemoveAt(0);
                if (res.error) return res;
            }

            return res.Success(cases);
        }

        public ParseResult IfExpressionElseIf() {
            return IfExpressionCases("elseif");
        }

        public ParseResult IfExpressionElse() {
            ParseResult res = new();
            (Node cond, Node statements, bool) elseCase = (null, null, false);
            Position startPos = currentToken.posStart.Copy();
            NumberNode condition = new NumberNode(new Token(TokenType.Float, 1, posStart: startPos));

            if (currentToken.Matches(TokenType.Keyword, "else")) {
                Advance(res);

                if(currentToken.type == TokenType.NewLine) {
                    Advance(res);

                    Node statements = res.Register(Statements());
                    if (res.error) return res;
                    elseCase = (condition, statements, true);

                    if(currentToken.Matches(TokenType.Keyword, "end")) {
                        Advance(res);
                    }else {
                        return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'end'"));
                    }
                } else {
                    Node expr = res.Register(Statement());
                    if (res.error) return res;
                    elseCase = (condition, expr, false);
                }
            }

            return res.Success(new IfCasesListNode(new() { elseCase }));
        }

        public ParseResult ForExpression() {
            //System.Diagnostics.Debug.WriteLine("[parse] ForExpression");

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

            if(currentToken.type == TokenType.NewLine) {
                Advance(res);

                Node funcMultiLine = res.Register(Statements());
                if(res.error) return res;

                if(!(currentToken.Matches(TokenType.Keyword, "end"))) {
                    System.Diagnostics.Debug.WriteLine(currentToken);
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'end'"));
                }

                Advance(res);

                return res.Success(new ForNode(varName, varStartExpression, condition, continuationExpression, isStep, funcMultiLine, true));
            }

            Node func = res.Register(Statement());
            if (res.error) return res;

            return res.Success(new ForNode(varName, varStartExpression, condition, continuationExpression, isStep, func, false));
        }

        public ParseResult WhileExpression() {
            //System.Diagnostics.Debug.WriteLine("[parse] WhileExpression");

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

            if (currentToken.type == TokenType.NewLine) {
                Advance(res);

                Node funcMultiLine = res.Register(Statements());
                if (res.error) return res;

                if (!(currentToken.Matches(TokenType.Keyword, "end"))) {
                    return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'end'"));
                }

                Advance(res);

                return res.Success(new WhileNode(condition, funcMultiLine, true));
            }

            Node func = res.Register(Statement());
            if (res.error) return res;

            return res.Success(new WhileNode(condition, func, false));
        }

        public ParseResult Power() {
            //System.Diagnostics.Debug.WriteLine("[parse] Power");

            return BinaryOperation(() => { return Call(); }, new List<TokenType>() { TokenType.Pow }, () => { return Factor(); });
        }

        public ParseResult Factor() {
            //System.Diagnostics.Debug.WriteLine("[parse] Factor");

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
            //System.Diagnostics.Debug.WriteLine("[parse] Term");

            return BinaryOperation(() => { return Factor(); }, new List<TokenType>() { TokenType.Mul, TokenType.Div, TokenType.Mod, TokenType.Index });
        }

        public ParseResult Expression() {
            //System.Diagnostics.Debug.WriteLine("[parse] Expression");

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
                return res.Failure(new IllegalSyntaxError(currentToken.posStart, currentToken.posEnd, "Expected 'var', numeric value, identifier, '[', '-', '+', 'not', 'if', 'func', 'for', 'while' or parentheses"));
            }

            return res.Success(node);
        }

        public ParseResult CompExpression() {
            //System.Diagnostics.Debug.WriteLine("[parse] CompExpression");

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
            //System.Diagnostics.Debug.WriteLine("[parse] ArithmeticExpression");

            return BinaryOperation(() => { return Term(); }, new List<TokenType>() { TokenType.Plus, TokenType.Minus });
        }


        //List<Token> exactTokens = null
        public ParseResult BinaryOperation(Func<ParseResult> funcLeft, List<TokenType> tokenTypes, Func<ParseResult> funcRight = null) {
            //System.Diagnostics.Debug.WriteLine("[parse] BinaryOperation");

            if (tokenTypes == null) tokenTypes = new();

            ParseResult res = new ParseResult();

            Node left = res.Register(funcLeft());
            Node right;
            Token opToken;

            if (res.error != null) return res;


            while (tokenTypes.Contains(currentToken.type)) {
                opToken = currentToken;
                res.RegisterAdvancement();
                Advance();
                right = res.Register((funcRight == null ? funcLeft() : funcRight()));

                if (res.error != null) return res;

                left = new BinaryOperationNode(left, opToken, right);
            }

            return res.Success(left);
        }

        public ParseResult BinaryOperation(Func<ParseResult> funcLeft, List<Token> exactTokens, Func<ParseResult> funcRight = null) {
            //System.Diagnostics.Debug.WriteLine("[parse] BinaryOperation");

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
}
