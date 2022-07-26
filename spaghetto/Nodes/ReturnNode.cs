namespace spaghetto {
    public class ReturnNode : Node {
        public Node nodeToReturn;

        public ReturnNode(Node nodeToReturn, Position posStart, Position posEnd) {
            this.nodeToReturn = nodeToReturn;
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            if(nodeToReturn != null) {
                Value value = res.Register(nodeToReturn.Visit(context));
                if (res.ShouldReturn()) return res;
                return res.SuccessReturn(value);
            }

            return res.SuccessReturn(new Number(0));
        }

        public override string ToString() {
            return "return(" + nodeToReturn + ")";
        }
    }
}
