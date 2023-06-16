namespace spaghetto.Parsing.Nodes
{
    internal class NativeImportNode : SyntaxNode
    {
        public SyntaxToken ident;

        public NativeImportNode(SyntaxToken ident) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }
    }
}
