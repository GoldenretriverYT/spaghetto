using spaghetto.Helpers;

namespace spaghetto.Parsing.Nodes
{
    internal class ClassDefinitionNode : SyntaxNode
    {
        public SyntaxToken className;
        public IEnumerable<SyntaxNode> body;
        public readonly bool fixedProps;

        public ClassDefinitionNode(SyntaxToken className, IEnumerable<SyntaxNode> body, bool fixedProps) : base(className.Position, body.GetEndingPosition(className.EndPosition))
        {
            this.className = className;
            this.body = body;
            this.fixedProps = fixedProps;
        }

        public override NodeType Type => NodeType.ClassDefinition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(className);
            foreach (var n in body) yield return n;
        }
    }
}
