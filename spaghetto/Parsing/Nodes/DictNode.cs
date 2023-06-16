namespace spaghetto.Parsing.Nodes
{
    internal class DictNode : SyntaxNode
    {
        public List<(SyntaxToken tok, SyntaxNode expr)> dict;
        public SyntaxToken lsq;
        public SyntaxToken rsq;

        public DictNode(List<(SyntaxToken tok, SyntaxNode expr)> dict, SyntaxToken lsq, SyntaxToken rsq) : base(lsq.Position, rsq.EndPosition)
        {
            this.dict = dict;
            this.lsq = lsq;
            this.rsq = rsq;
        }

        public override NodeType Type => NodeType.Dict;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}