﻿namespace spaghetto.Parsing.Nodes
{
    internal class StringLiteralNode : SyntaxNode
    {
        private SyntaxToken syntaxToken;

        public StringLiteralNode(SyntaxToken syntaxToken)
        {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.StringLiteral;

        public override SValue Evaluate(Scope scope)
        {
            return new SString((string)syntaxToken.Value);
        }

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
