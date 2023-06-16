namespace spaghetto.Parsing.Nodes
{
    internal class ForNode : SyntaxNode
    {
        private SyntaxNode initialExpressionNode;
        private SyntaxNode condNode;
        private SyntaxNode stepNode;
        private SyntaxNode block;

        public ForNode(SyntaxNode initialExpressionNode, SyntaxNode condNode, SyntaxNode stepNode, SyntaxNode block) : base(initialExpressionNode.StartPosition, block.EndPosition)
        {
            this.initialExpressionNode = initialExpressionNode;
            this.condNode = condNode;
            this.stepNode = stepNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.For;

        public override SValue Evaluate(Scope scope)
        {
            Scope forScope = new(scope, StartPosition);
            SValue lastVal = SValue.Null;
            initialExpressionNode.EvaluateWithErrorCheck(forScope);

            while (true)
            {
                if (!condNode.EvaluateWithErrorCheck(forScope).IsTruthy()) break;
                var forBlockRes = block.EvaluateWithErrorCheck(forScope);
                if (!forBlockRes.IsNull()) lastVal = forBlockRes;

                if (forScope.State == ScopeState.ShouldBreak) break;
                if (forScope.State != ScopeState.None) forScope.SetState(ScopeState.None);

                stepNode.EvaluateWithErrorCheck(forScope);
            }

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return initialExpressionNode;
            yield return condNode;
            yield return stepNode;
            yield return block;
        }
    }
}
