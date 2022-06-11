namespace spaghetto {
    internal class ListNode : Node {
        public List<Node> elementNodes = new();

        public ListNode(List<Node> elementNodes, Position posStart, Position posEnd) {
            this.elementNodes = elementNodes;
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            List<Value> elements = new();

            foreach(Node elementNode in elementNodes) {
                elements.Add(res.Register(elementNode.Visit(context)));
                if (res.ShouldReturn()) return res;
            }

            return res.Success(new ListValue(elements).SetContext(context).SetPosition(posStart, posEnd));
        }
    }
}
