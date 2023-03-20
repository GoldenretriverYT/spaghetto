namespace spaghetto.Parsing.Nodes
{
    internal class IntLiteralNode : SyntaxNode
    {
        private SyntaxToken syntaxToken;

        public IntLiteralNode(SyntaxToken syntaxToken)
        {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override SValue Evaluate(Scope scope)
        {
            var sint = new SInt((int)syntaxToken.Value);
            return sint;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString()
        {
            return "IntLitNode:";
        }
    }
}
