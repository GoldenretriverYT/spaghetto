namespace spaghetto.Parsing.Nodes
{
    internal class ContinueNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Continue;

        public override SValue Evaluate(Scope scope)
        {
            scope.SetState(ScopeState.ShouldContinue);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "ContinueNode:";
        }
    }
}
