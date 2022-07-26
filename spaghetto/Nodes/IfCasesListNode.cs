namespace spaghetto {
    public class IfCasesListNode : Node {
        public List<(Node cond, Node expr, bool)> cases;

        public IfCasesListNode(List<(Node cond, Node expr, bool)> cases) {
            this.cases = cases;
        }

        public override RuntimeResult Visit(Context context) {
            throw new Exception("Should never reach this");
        }

        public override string ToString() {
            List<string> casesStrings = new();

            foreach ((Node cond, Node expr, bool ret) cas in cases) {
                casesStrings.Add("case[cond=" + cas.cond + "&expr=" + cas.expr + "&ret=" + cas.ret + "]");
            }

            return "ifCasesList(" + casesStrings.Join(", ") + ")";
        }
    }
}
