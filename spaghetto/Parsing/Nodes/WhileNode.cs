namespace spaghetto.Parsing.Nodes
{
    internal class WhileNode : SyntaxNode
    {
        public SyntaxNode condNode;
        public SyntaxNode block;

        public WhileNode(SyntaxNode condNode, SyntaxNode block) : base(condNode.EndPosition, block.EndPosition)
        {
            this.condNode = condNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.While;

        public override SValue Evaluate(Scope scope)
        {
            Scope whileScope = new(scope, StartPosition);
            SValue lastVal = SValue.Null;

            while (true)
            {
                if (!condNode.Evaluate(whileScope).IsTruthy()) break;
                var whileBlockRes = block.Evaluate(whileScope);
                if (!whileBlockRes.IsNull()) lastVal = whileBlockRes;

                if (whileScope.State == ScopeState.ShouldBreak) break;
                if (whileScope.State != ScopeState.None) whileScope.SetState(ScopeState.None);
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return condNode;
            yield return block;
        }
    }
}
