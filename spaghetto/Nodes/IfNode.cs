namespace spaghetto {
    public class IfNode : Node {
        public List<(Node cond, Node expr, bool ret)> cases;

        public IfNode(List<(Node cond, Node expr, bool shouldReturnNull)> cases) {
            this.cases = cases;
            this.posStart = cases.First().cond.posStart;
            this.posEnd = cases.Last().expr.posEnd;
        }

        public override string ToString() {
            List<string> casesStrings = new();

            foreach((Node cond, Node expr, bool ret) cas in cases) {
                casesStrings.Add("case[cond=" + cas.cond + "&expr=" + cas.expr + "&ret=" + cas.ret + "]");
            }

            return "if(" + casesStrings.Join(", ") + ")";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            foreach((Node cond, Node expr, bool shouldReturnNull) in cases) {
                Value conditionValue = res.Register(cond.Visit(context));
                if (res.ShouldReturn()) return res;

                if(conditionValue.IsTrue()) {
                    Value expressionValue = res.Register(expr.Visit(context));
                    if (res.ShouldReturn()) return res;
                    return res.Success((shouldReturnNull ? new Number(0) : expressionValue));
                } 
            }

            return res.Success(null);
        }
    }
}
