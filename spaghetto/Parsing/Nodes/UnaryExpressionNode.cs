namespace spaghetto.Parsing.Nodes
{
    internal class UnaryExpressionNode : SyntaxNode
    {
        private SyntaxToken token;
        private SyntaxNode rhs;

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

        public override string ToString()
        {
            return "UnaryExpressionNode:";
        }

        public override string GenerateSource(int depth) {
            return token.Text + "(" + rhs.GenerateSource(depth + 1) + ")";
        }
    }
}
