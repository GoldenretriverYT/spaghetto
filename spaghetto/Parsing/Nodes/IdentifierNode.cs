namespace spaghetto.Parsing.Nodes
{
    internal class IdentifierNode : SyntaxNode
    {
        public SyntaxToken Token { get; private set; }
        public bool NonNull { get; }

        public IdentifierNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            Token = syntaxToken;
        }

        public IdentifierNode(SyntaxToken syntaxToken, bool nonNull) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            Token = syntaxToken;
            NonNull = nonNull;
        }

        public override NodeType Type => NodeType.Identifier;


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Token);
        }

        public override string ToString()
        {
            return "IdentNode:";
        }
    }
}
