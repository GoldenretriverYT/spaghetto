using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class DictNode : SyntaxNode
    {
        private List<(SyntaxToken tok, SyntaxNode expr)> dict;
        private SyntaxToken lsq;
        private SyntaxToken rsq;

        public DictNode(List<(SyntaxToken tok, SyntaxNode expr)> dict, SyntaxToken lsq, SyntaxToken rsq) : base(lsq.Position, rsq.EndPosition)
        {
            this.dict = dict;
            this.lsq = lsq;
            this.rsq = rsq;
        }

        public override NodeType Type => NodeType.Dict;

        public override SValue Evaluate(Scope scope)
        {
            var dict = new SDictionary();
            
            foreach(var ent in this.dict) {
                dict.Value.Add((new SString(ent.tok.Text), ent.expr.Evaluate(scope)));
            }

            return dict;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return "DictNode:";
        }

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder();

            sb.Append("{");

            foreach(var dictEl in dict) {
                sb.Append("\"");
                sb.Append(dictEl.tok.Text);
                sb.Append("\":");
                sb.Append(dictEl.expr.GenerateSource(depth + 1));
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append("}");

            return sb.ToString();
        }
    }
}