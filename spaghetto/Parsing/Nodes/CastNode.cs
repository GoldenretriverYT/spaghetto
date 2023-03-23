namespace spaghetto.Parsing.Nodes
{
    internal class CastNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode node;

        public CastNode(SyntaxToken ident, SyntaxNode node)
        {
            this.ident = ident;
            this.node = node;
        }

        public override NodeType Type => NodeType.Cast;

        public override SValue Evaluate(Scope scope)
        {
            // TODO: maybe improve this
            switch (ident.Text)
            {
                case "int":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.Int);
                case "float":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.Float);
                case "string":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.String);
                case "list":
                    return node.Evaluate(scope).CastToBuiltin(SBuiltinType.List);
                default: throw new InvalidOperationException("INTERNAL: Cast was parsed successfully, but cast is not implemented for that!");
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return node;
        }
    }
}
