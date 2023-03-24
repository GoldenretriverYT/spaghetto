namespace spaghetto.Parsing.Nodes
{
    internal class FloatLiteralNode : SyntaxNode
    {
        private SyntaxToken syntaxToken;

        public FloatLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.FloatLiteral;

        public override SValue Evaluate(Scope scope)
        {
            return new SFloat((float)syntaxToken.Value);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString()
        {
            return "FloatLitNode:";
        }
    }
}
