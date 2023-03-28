namespace spaghetto.Parsing.Nodes
{
    internal class BlockNode : SyntaxNode
    {
        private List<SyntaxNode> nodes;
        private readonly bool createNewScope;

        public BlockNode(SyntaxToken startTok, SyntaxToken endTok, List<SyntaxNode> nodes, bool createNewScope = true) : base(startTok.Position, endTok.Position)
        {
            this.Nodes = nodes;
            this.createNewScope = createNewScope;
        }

        public override NodeType Type => NodeType.Block;

        public List<SyntaxNode> Nodes { get => nodes; set => nodes = value; }

        public override SValue Evaluate(Scope scope)
        {
            var lastVal = SValue.Null;
            var blockScope = scope;

            if (createNewScope) blockScope = new Scope(scope, StartPosition);

            foreach (var node in Nodes)
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
                    //Debug.WriteLine("Returning from block node at range " + StartPosition + ".." + EndPosition + " with value " + scope.ReturnValue.ToString());
                    var v = scope.ReturnValue;
                    return v;
                }
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var node in Nodes) yield return node;
        }

        public override string ToString()
        {
            return "BlockNode:";
        }
    }
}
