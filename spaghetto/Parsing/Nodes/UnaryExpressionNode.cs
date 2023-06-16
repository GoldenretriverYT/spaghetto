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
