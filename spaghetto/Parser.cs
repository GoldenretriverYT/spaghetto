using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public SyntaxToken MatchToken(SyntaxType type, object value)
        {
            if (Current.Type == type && Current.Value == value) {
                Position++;
                return Peek(-1);
            }

            throw new Exception("Unexpected token " + Current.Type + "; expected " + type + " with value " + value);
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
                throw new NotImplementedException();
                //return ParseForExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "while") {
                throw new NotImplementedException();
                //return ParseWhileExpression();
            } else if (Current.Type is SyntaxType.Keyword && Current.Value == "func") {
                throw new NotImplementedException();
                //return ParseFuncExpression();
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
            MatchToken(SyntaxType.Keyword, "if");

            IfNode node = new();

            MatchToken(SyntaxType.LParen);
            var initialCond = ParseExpression();
            MatchToken(SyntaxType.RParen);

            var initialBlock = ParseScopedStatements();

            node.AddCase(initialCond, initialBlock);

            do {
                Position++;

                MatchToken(SyntaxType.LParen);
                var cond = ParseExpression();
                MatchToken(SyntaxType.RParen);
                var block = ParseScopedStatements();

                node.AddCase(cond, block);
            } while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif");

            if(Current.Type == SyntaxType.Keyword && Current.Value == "else") {
                Position++;
                var block = ParseScopedStatements();

                node.AddCase(new BoolNode(true), block);
            }
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

    public abstract class SyntaxNode
    {
        public abstract NodeType NodeType { get; }

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

        public override NodeType NodeType => NodeType.BinaryExpression;

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
    }

    public class BoolNode : SyntaxNode
    {

    }

    // dummy node for tree view
    public class TokenNode : SyntaxNode
    {
        public override NodeType NodeType => NodeType.Token;
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
    }

    public abstract class SValue
    {
        public static SValue Null => new SInt(0);
        public abstract SBuiltinType BuiltinName { get; }

        public virtual SValue Add(SValue other)
        {
            throw new NotImplementedException();
        }

        public virtual SValue Sub(SValue other)
        {
            throw new NotImplementedException();
        }

        public virtual SValue Mul(SValue other)
        {
            throw new NotImplementedException();
        }

        public virtual SValue Div(SValue other)
        {
            throw new NotImplementedException();
        }

        public virtual SValue Mod(SValue other)
        {
            throw new NotImplementedException();
        }

        public virtual SValue Idx(SValue other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return BuiltinName.ToString();
        }

        public virtual SString ToSpagString()
        {
            return new SString("<unknown of type " + BuiltinName.ToString() + ">");
        }
    }

    public abstract class SNumber<T, TSelf> : SValue
        where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>, IModulusOperators<T, T, T>, new()
        where TSelf : SNumber<T, TSelf>, new()
    {
        public T Value { get; set; }

        public virtual SValue Add(SValue other)
        {
            if (other is not SNumber<T, TSelf> otherNumber) throw new NotImplementedException();
            var res = otherNumber.Value + Value;

            var resSVal = new TSelf();
            resSVal.Value = res;

            return resSVal;
        }

        public virtual SValue Sub(SValue other)
        {
            if (other is not SNumber<T, TSelf> otherNumber) throw new NotImplementedException();
            var res = otherNumber.Value - Value;

            var resSVal = new TSelf();
            resSVal.Value = res;

            return resSVal;
        }

        public virtual SValue Div(SValue other)
        {
            if (other is not SNumber<T, TSelf> otherNumber) throw new NotImplementedException();
            var res = otherNumber.Value / Value;

            var resSVal = new TSelf();
            resSVal.Value = res;

            return resSVal;
        }

        public virtual SValue Mul(SValue other)
        {
            if (other is not SNumber<T, TSelf> otherNumber) throw new NotImplementedException();
            var res = otherNumber.Value * Value;

            var resSVal = new TSelf();
            resSVal.Value = res;

            return resSVal;
        }

        public virtual SValue Mod(SValue other)
        {
            if (other is not SNumber<T, TSelf> otherNumber) throw new NotImplementedException();
            var res = otherNumber.Value % Value;

            var resSVal = new TSelf();
            resSVal.Value = res;

            return resSVal;
        }

        public override string ToString()
        {
            return $"<{GetType().Name} value={Value}>";
        }

        public override SString ToSpagString()
        {
            return new SString(Value.ToString());
        }
    }

    public class SInt : SNumber<int, SInt>
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Int;
        public new int Value { get; set; } = 0;

        public SInt() { }

        public SInt(int value)
        {
            Value = value;
        }
    }

    public class SFloat : SNumber<float, SFloat>
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Float;
        public new float Value { get; set; } = 0;

        public SFloat() { }

        public SFloat(float value)
        {
            Value = value;
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
        Token
    }

    public enum SBuiltinType
    {
        String,
        Int,
        Float,
    }
}
