namespace spaghetto {
    internal class IfCasesListNode : Node {
        public List<(Node cond, Node expr, bool)> cases;

        public IfCasesListNode(List<(Node cond, Node expr, bool)> cases) {
            this.cases = cases;
        }

        public override RuntimeResult Visit(Context context) {
            throw new Exception("Should never reach this");
        }
    }
}
