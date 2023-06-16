using spaghetto.BuiltinTypes;
using spaghetto.Helpers;

namespace spaghetto.Parsing.Nodes
{
    internal class DotNode : SyntaxNode
    {
        public DotNode(SyntaxNode callNode) : base(callNode.StartPosition, -1) // ending pos is overwritten
        {
            CallNode = callNode;
        }

        public SyntaxNode CallNode { get; }
        public List<SyntaxNode> NextNodes { get; internal set; } = new();
        public override int EndPosition => NextNodes.GetEndingPosition(CallNode.EndPosition);

        public override NodeType Type => NodeType.Dot;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CallNode;

            foreach (var node in NextNodes) yield return node;
        }

        public override string ToString()
        {
            return "DotNode:";
        }

        public DotNode Clone()
        {
            var dn = new DotNode(CallNode);
            dn.NextNodes = this.NextNodes.ToList(); return dn;
        }
    }
}
