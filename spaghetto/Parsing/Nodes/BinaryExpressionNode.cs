namespace spaghetto.Parsing.Nodes
{
    internal class BinaryExpressionNode : SyntaxNode
    {
        public SyntaxNode left;
        public SyntaxToken operatorToken;
        public SyntaxNode right;

        public BinaryExpressionNode(SyntaxNode left, SyntaxToken operatorToken, SyntaxNode right) : base(left.StartPosition, right.EndPosition)
        {
            this.left = left;
            this.operatorToken = operatorToken;
            this.right = right;
        }

        public override NodeType Type => NodeType.BinaryExpression;

        public override SValue Evaluate(Scope scope)
        {
            var leftRes = left.EvaluateWithErrorCheck(scope);
            if (leftRes == SValue.Error) return leftRes;
            var rightRes = right.EvaluateWithErrorCheck(scope);
            if (rightRes == SValue.Error) return leftRes;

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
                    return Scope.Error($"Operator {operatorToken.Type} does not have an implementation for binary expressions.");
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
    }
}
