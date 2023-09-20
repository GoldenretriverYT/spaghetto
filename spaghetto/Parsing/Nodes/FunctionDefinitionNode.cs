using spaghetto.Helpers;
using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class FunctionDefinitionNode : SyntaxNode
    {
        private SyntaxToken? nameToken;
        private List<SyntaxToken> args;
        private SyntaxNode block;

        public FunctionDefinitionNode(SyntaxToken? nameToken, List<SyntaxToken> args, SyntaxNode block)
            : base(nameToken != null ? nameToken.Value.Position : args.GetStartingPosition(block.StartPosition), block.EndPosition) // either nametoken start, args start or finally block start
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

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder();
            
            if (nameToken != null) {
                sb.Append("func ");
                sb.Append(nameToken.Value.Text);
            } else {
                sb.Append("func");
            }

            sb.Append("(");
            sb.Append(string.Join(", ", args.Select((v) => v.Text)));
            sb.Append(") ");

            sb.Append(block.GenerateSource(depth + 1));

            return sb.ToString();
        }
    }
}
