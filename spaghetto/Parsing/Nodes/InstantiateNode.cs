using spaghetto.Helpers;

namespace spaghetto.Parsing.Nodes
{
    internal class InstantiateNode : SyntaxNode
    {
        public SyntaxToken ident;
        public List<SyntaxNode> argumentNodes;

        public InstantiateNode(SyntaxToken ident, List<SyntaxNode> argumentNodes) : base(ident.Position, argumentNodes.GetEndingPosition(ident.EndPosition))
        {
            this.ident = ident;
            this.argumentNodes = argumentNodes;
        }

        public override NodeType Type => NodeType.Instantiate;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            foreach (var n in argumentNodes) yield return n;
        }
    }
}
