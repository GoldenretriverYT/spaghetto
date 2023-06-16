using spaghetto.Helpers;

namespace spaghetto.Parsing.Nodes
{
    internal class FunctionDefinitionNode : SyntaxNode
    {
        public SyntaxToken? nameToken;
        public List<SyntaxToken> args;
        public SyntaxNode block;

        public FunctionDefinitionNode(SyntaxToken? nameToken, List<SyntaxToken> args, SyntaxNode block)
            : base(nameToken != null ? nameToken.Value.Position : args.GetStartingPosition(block.StartPosition), block.EndPosition) // either nametoken start, args start or finally block start
        {
            this.nameToken = nameToken;
            this.args = args;
            this.block = block;
        }

        public override NodeType Type => NodeType.FunctionDefinition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (nameToken != null) yield return new TokenNode(nameToken.Value);
            foreach (var t in args) yield return new TokenNode(t);
            yield return block;
        }
    }
}
