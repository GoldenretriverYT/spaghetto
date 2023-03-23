namespace spaghetto.Parsing.Nodes
{
    internal class FunctionDefinitionNode : SyntaxNode
    {
        private SyntaxToken? nameToken;
        private List<SyntaxToken> args;
        private SyntaxNode block;

        public FunctionDefinitionNode(SyntaxToken? nameToken, List<SyntaxToken> args, SyntaxNode block)
        {
            this.nameToken = nameToken;
            this.args = args;
            this.block = block;
        }

        public override NodeType Type => NodeType.FunctionDefinition;

        public override SValue Evaluate(Scope scope)
        {
            var f = new SFunction(scope, nameToken?.Text ?? "<anonymous>", args.Select((v) => v.Text).ToList(), block);
            if (nameToken != null) scope.Set(nameToken.Value.Text, f);
            return f;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (nameToken != null) yield return new TokenNode(nameToken.Value);
            foreach (var t in args) yield return new TokenNode(t);
            yield return block;
        }
    }
}
