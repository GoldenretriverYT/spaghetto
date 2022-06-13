namespace spaghetto {
    internal class Dot : Node {
        public Node? baseNode;
        public List<Node> nextNodes = new();

        public Dot(Node baseNode) {
            this.baseNode = baseNode;
            this.posStart = baseNode.posStart;
            this.posEnd = baseNode.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            Value value = res.Register(baseNode.Visit(context));
            if (res.error) return res;

            foreach(Node node in nextNodes.Select(node => node.Clone()).ToList())
            {
                if(node is VariableAccessNode)
                {
                    value = value.Get((node as VariableAccessNode).varNameToken.value.ToString());
                }else
                {
                    if (node is CallNode)
                    {
                        CallNode temp = node as CallNode;
                        temp.argNodes.Insert(0, new ValueNode(value, value.posStart, value.posEnd));
                        if (temp.nodeToCall is VariableAccessNode) temp.nodeToCall = new ValueNode(value.Get((temp.nodeToCall as VariableAccessNode).varNameToken.value.ToString()), temp.nodeToCall.posStart, temp.nodeToCall.posEnd);
                    }

                    Value v = res.Register((node as CallNode).Visit(context));

                    if(node is CallNode)
                    {
                        CallNode temp = node as CallNode;
                        temp.argNodes.RemoveAt(0);
                    }
                    if (res.error) return res;
                    value = v;
                }
            }

            if (value == null) value = new Number(0);

            value = value.Copy().SetPosition(posStart, posEnd).SetContext(context);
            return res.Success(value);
        }
    }
}
