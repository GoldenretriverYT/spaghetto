namespace spaghetto.Parsing.Nodes
{
    internal class ExportNode : SyntaxNode
    {
        public SyntaxToken ident;

        public ExportNode(SyntaxToken ident) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.Export;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }
    }
}
