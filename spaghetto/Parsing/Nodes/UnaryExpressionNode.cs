namespace spaghetto.Parsing.Nodes
{
    internal class UnaryExpressionNode : SyntaxNode
    {
        public SyntaxToken token;
        public SyntaxNode rhs;

        public UnaryExpressionNode(SyntaxToken token, SyntaxNode rhs) : base(token.Position, rhs.EndPosition)
        {
            this.token = token;
            this.rhs = rhs;
        }

        public override NodeType Type => NodeType.UnaryExpression;

        public override SValue Evaluate(Scope scope)
        {
            switch (token.Type)
            {
                case SyntaxType.Bang: return rhs.EvaluateWithErrorCheck(scope).Not();
                case SyntaxType.Minus: return rhs.EvaluateWithErrorCheck(scope).ArithNot();
                case SyntaxType.Plus: return rhs.EvaluateWithErrorCheck(scope);
                default: throw new InvalidOperationException();
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(token);
            yield return rhs;
        }

        public override string ToString()
        {
            return "UnaryExpressionNode:";
        }
    }
}
