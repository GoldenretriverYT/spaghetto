﻿namespace spaghetto.Parsing.Nodes
{
    internal class IntLiteralNode : SyntaxNode
    {
        public SyntaxToken intToken;

        public IntLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.intToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(intToken);
        }

        public override string ToString()
        {
            return "IntLitNode:";
        }
    }
}
