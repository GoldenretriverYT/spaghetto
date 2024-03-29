﻿namespace spaghetto.Parsing.Nodes
{
    internal class WhileNode : SyntaxNode
    {
        private SyntaxNode condNode;
        private SyntaxNode block;

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

        public override string ToString() {
            return "WhileNode:";
        }

        public override string GenerateSource(int depth) {
            return $"while ({condNode.GenerateSource(depth + 1)}) {block.GenerateSource(depth + 1)}";
        }
    }
}
