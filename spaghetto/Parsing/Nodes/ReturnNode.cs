namespace spaghetto.Parsing.Nodes
{
    internal class ReturnNode : SyntaxNode
    {
        public ReturnNode(SyntaxToken tok) : base(tok.Position, tok.EndPosition)
        {
        }

        public ReturnNode(SyntaxToken tok, SyntaxNode returnValueNode) : base(tok.Position, returnValueNode.EndPosition)
        {
            ReturnValueNode = returnValueNode;
        }

        public SyntaxNode ReturnValueNode { get; }

        public override NodeType Type => NodeType.Return;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (ReturnValueNode == null) yield break;
            else yield return ReturnValueNode;
        }

        public override string ToString()
        {
            return "ReturnNode:";
        }
    }
}
