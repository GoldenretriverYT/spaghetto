namespace spaghetto.Parsing.Nodes
{
    internal class BlockNode : SyntaxNode
    {
        private List<SyntaxNode> nodes;
        private readonly bool createNewScope;

        public BlockNode(List<SyntaxNode> nodes, bool createNewScope = true)
        {
            this.nodes = nodes;
            this.createNewScope = createNewScope;
        }

        public override NodeType Type => NodeType.Block;

        public override SValue Evaluate(Scope scope)
        {
            var lastVal = SValue.Null;
            var blockScope = scope;

            if (createNewScope) blockScope = new Scope(scope);

            foreach (var node in nodes)
            {
                var res = node.Evaluate(blockScope);

                if (!res.IsNull())
                {
                    lastVal = res;
                }

                if (scope.State == ScopeState.ShouldBreak
                    || scope.State == ScopeState.ShouldContinue) return lastVal;

                if (scope.State == ScopeState.ShouldReturn)
                {
                    var v = scope.ReturnValue;
                    return v;
                }
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var node in nodes) yield return node;
        }

        public override string ToString()
        {
            return "BlockNode:";
        }
    }
}
