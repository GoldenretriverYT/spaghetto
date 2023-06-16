namespace spaghetto.Parsing.Nodes
{
    internal class ForNode : SyntaxNode
    {
        public SyntaxNode initialExpressionNode;
        public SyntaxNode condNode;
        public SyntaxNode stepNode;
        public SyntaxNode block;

        public ForNode(SyntaxNode initialExpressionNode, SyntaxNode condNode, SyntaxNode stepNode, SyntaxNode block) : base(initialExpressionNode.StartPosition, block.EndPosition)
        {
            this.initialExpressionNode = initialExpressionNode;
            this.condNode = condNode;
            this.stepNode = stepNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.For;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return initialExpressionNode;
            yield return condNode;
            yield return stepNode;
            yield return block;
        }
    }
}
