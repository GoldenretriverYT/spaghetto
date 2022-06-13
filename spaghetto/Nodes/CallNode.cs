namespace spaghetto {
    internal class CallNode : Node {
        public Node nodeToCall;
        public List<Node> argNodes;

        public CallNode(Node nodeToCall, List<Node> argNodes) {
            this.nodeToCall = nodeToCall;
            this.argNodes = argNodes;

            posStart = nodeToCall.posStart;

            if (argNodes.Count > 0) {
                posEnd = argNodes.Last().posEnd;
            } else {
                posEnd = nodeToCall.posEnd;
            }
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            List<Value> args = new();

            Value valueToCall = res.Register(nodeToCall.Visit(context));
            if (res.ShouldReturn()) return res;
            if (valueToCall is Number && (valueToCall as Number).value == 0) return res.Failure(new RuntimeError(posStart, posEnd, "Can not execute NULL", context));
            valueToCall = valueToCall.Copy().SetPosition(posStart, posEnd).SetContext(context);

            if (valueToCall is BaseFunction)
                if ((valueToCall as BaseFunction).IsStatic)
                    argNodes.RemoveAt(0);

            foreach (Node argNode in argNodes)
            {
                args.Add(res.Register(argNode.Visit(context)));
                if (res.ShouldReturn()) return res;
            }

            Value retValue = res.Register(valueToCall.Execute(args));
            if (res.ShouldReturn()) return res;
            if (retValue != null) retValue = retValue.Copy().SetPosition(posStart, posEnd).SetContext(context);

            return res.Success(retValue);
        }
    }
}
