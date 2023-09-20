namespace spaghetto.Parsing.Nodes
{
    internal class CastNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode node;
        public SBuiltinType type;

        public CastNode(SyntaxToken ident, SyntaxNode node) : base(ident.Position, node.EndPosition)
        {
            this.ident = ident;
            this.node = node;

            // TODO: Allow for cast to classes
            if (!Enum.TryParse<SBuiltinType>(ident.Text, true, out type)) throw new Exception("Unknown type " + ident.Text + "; only builtin types supported right now.");
        }

        public override NodeType Type => NodeType.Cast;

        public override SValue Evaluate(Scope scope)
        {
            return node.Evaluate(scope).CastToBuiltin(type);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return node;
        }

        public override string GenerateSource(int depth) {
            return "<" + ident.Text + ">(" + node.GenerateSource(depth + 1) + ")";
        }
    }
}
