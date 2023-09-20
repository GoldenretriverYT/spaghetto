using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class BinaryExpressionNode : SyntaxNode
    {
        private SyntaxNode left;
        private SyntaxToken operatorToken;
        private SyntaxNode right;

        public BinaryExpressionNode(SyntaxNode left, SyntaxToken operatorToken, SyntaxNode right) : base(left.StartPosition, right.EndPosition)
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

            switch (operatorToken.Type)
            {
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
                case SyntaxType.BangEquals:
                    return leftRes.Equals(rightRes).Not();
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
                case SyntaxType.AndAnd:
                    return new SInt((leftRes.IsTruthy() && rightRes.IsTruthy()) ? 1 : 0);
                case SyntaxType.OrOr:
                    return new SInt((leftRes.IsTruthy() || rightRes.IsTruthy()) ? 1 : 0);
                default:
                    throw new NotImplementedException($"Operator {operatorToken.Type} does not have an implementation for binary expressions.");
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return left;
            yield return new TokenNode(operatorToken);
            yield return right;
        }

        public override string ToString()
        {
            return "BinaryExprNode: op=" + operatorToken.Type;
        }

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder();

            string op = operatorToken.Text;

            // if ++, --, etc. we still emit the 1 on the rhs, so remove the second sign

            if (op == "++" || op == "--") {
                op = op.Substring(0, 1);
            }

            sb.Append(left.GenerateSource(depth + 1));
            sb.Append(op);
            sb.Append(right.GenerateSource(depth + 1));

            return sb.ToString();
        }
    }
}
