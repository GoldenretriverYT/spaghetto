namespace spaghetto.Parsing.Nodes
{
    internal class StringLiteralNode : SyntaxNode
    {
        public SyntaxToken syntaxToken;

        public StringLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.StringLiteral;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString()
        {
            return "StringLitNode:";
        }
    }
}
