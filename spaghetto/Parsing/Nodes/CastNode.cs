namespace spaghetto.Parsing.Nodes
{
    internal class CastNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode node;

        public CastNode(SyntaxToken ident, SyntaxNode node) : base(ident.Position, node.EndPosition)
        {
            this.ident = ident;
            this.node = node;
        }

        public override NodeType Type => NodeType.Cast;

        public override SValue Evaluate(Scope scope)
        {
            // TODO: Allow for cast to classes
            if (!Enum.TryParse<SBuiltinType>(ident.Text, true, out var type)) throw new Exception("Unknown type " + ident.Text + "; only builtin types supported right now.");
            return node.Evaluate(scope).CastToBuiltin(type);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return node;
        }
    }
}
