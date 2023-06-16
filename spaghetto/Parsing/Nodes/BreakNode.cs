namespace spaghetto.Parsing.Nodes
{
    internal class BreakNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Break;

        public BreakNode(SyntaxToken breakToken) : base(breakToken.Position, breakToken.EndPosition) { }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "BreakNode:";
        }
    }
}
