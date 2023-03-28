namespace spaghetto.Parsing.Nodes
{
    internal class ListNode : SyntaxNode
    {
        private List<SyntaxNode> list;

        public ListNode(List<SyntaxNode> list, SyntaxToken lsqBracket, SyntaxToken rsqBracket) : base(lsqBracket.Position, rsqBracket.EndPosition)
        {
            this.list = list;
        }

        public override NodeType Type => NodeType.List;

        public override SValue Evaluate(Scope scope)
        {
            SList sList = new();

            foreach (var n in list)
            {
                sList.Value.Add(n.Evaluate(scope));
            }

            return sList;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var n in list) yield return n;
        }

        public override string ToString()
        {
            return "ListNode:";
        }
    }
}
