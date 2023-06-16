namespace spaghetto.Parsing.Nodes
{
    internal class BlockNode : SyntaxNode
    {
        private List<SyntaxNode> nodes;
        public readonly bool createNewScope;

        public BlockNode(SyntaxToken startTok, SyntaxToken endTok, List<SyntaxNode> nodes, bool createNewScope = true) : base(startTok.Position, endTok.Position)
        {
            this.Nodes = nodes;
            this.createNewScope = createNewScope;
        }

        public override NodeType Type => NodeType.Block;

        public List<SyntaxNode> Nodes { get => nodes; set => nodes = value; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var node in Nodes) yield return node;
        }

        public override string ToString()
        {
            return "BlockNode:";
        }
    }
}
