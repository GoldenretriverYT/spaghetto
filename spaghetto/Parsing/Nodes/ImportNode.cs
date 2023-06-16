namespace spaghetto.Parsing.Nodes
{
    internal class ImportNode : SyntaxNode
    {
        public SyntaxToken path;

        public ImportNode(SyntaxToken path) : base(path.Position, path.EndPosition)
        {
            this.path = path;
        }

        public override NodeType Type => NodeType.Import;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(path);
        }
    }
}
