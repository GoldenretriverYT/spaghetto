namespace spaghetto.Parsing.Nodes
{
    internal class IfNode : SyntaxNode
    {
        public IfNode(SyntaxToken startTok) : base(startTok.Position, startTok.Position) { } // We expect the parser to properly define the endpos

        public List<(SyntaxNode cond, SyntaxNode block)> Conditions { get; private set; } = new();

        public override NodeType Type => NodeType.If;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var (cond, block) in Conditions)
            {
                yield return cond;
                yield return block;
            }
        }

        internal void AddCase(SyntaxNode cond, SyntaxNode block)
        {
            Conditions.Add((cond, block));
        }

        public override string ToString()
        {
            return "IfNode:";
        }
    }
}
