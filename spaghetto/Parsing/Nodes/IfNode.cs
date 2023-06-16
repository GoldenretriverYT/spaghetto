namespace spaghetto.Parsing.Nodes
{
    internal class IfNode : SyntaxNode
    {
        public IfNode(SyntaxToken startTok) : base(startTok.Position, startTok.Position) { } // We expect the parser to properly define the endpos

        public List<(SyntaxNode cond, SyntaxNode block)> Conditions { get; private set; } = new();

        public override NodeType Type => NodeType.If;

        public override SValue Evaluate(Scope scope)
        {
            foreach ((SyntaxNode cond, SyntaxNode block) in Conditions)
            {
                var condRes = cond.EvaluateWithErrorCheck(scope);

                if (condRes.IsTruthy())
                {
                    return block.EvaluateWithErrorCheck(new Scope(scope, StartPosition));
                }
            }

            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var (cond, block) in Conditions)
            {
                yield return cond;
                yield return block;
            }
        }

        internal void AddCase(SyntaxNode cond, SyntaxNode block)
        {
            Conditions.Add((cond, block));
        }

        public override string ToString()
        {
            return "IfNode:";
        }
    }
}
