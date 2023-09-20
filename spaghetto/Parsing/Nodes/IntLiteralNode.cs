namespace spaghetto.Parsing.Nodes
{
    internal class IntLiteralNode : SyntaxNode
    {
        public SyntaxToken intToken;

        public IntLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.intToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override SValue Evaluate(Scope scope)
        {
            var sint = new SInt((int)intToken.Value);
            return sint;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(intToken);
        }

        public override string ToString()
        {
            return "IntLitNode:";
        }

        public override string GenerateSource(int depth) {
            return intToken.Text;
        }
    }
}
