namespace spaghetto.Parsing.Nodes
{
    internal class WhileNode : SyntaxNode
    {
        public SyntaxNode condNode;
        public SyntaxNode block;

        public WhileNode(SyntaxNode condNode, SyntaxNode block) : base(condNode.EndPosition, block.EndPosition)
        {
            this.condNode = condNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.While;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return condNode;
            yield return block;
        }
    }
}
