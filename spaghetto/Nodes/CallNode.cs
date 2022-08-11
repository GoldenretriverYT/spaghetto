using System.Diagnostics;

namespace spaghetto {
    public class CallNode : Node {
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

        public override string ToString() {
            string str = "call(toCall: " + nodeToCall.ToString() + ", args: ";
            str += argNodes.Join(", ");
            return str + ")";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            List<Value> args = new();

            Value valueToCall = res.Register(nodeToCall.Visit(context));
            if (res.ShouldReturn()) return res;
            if (valueToCall is Number && (valueToCall as Number).value == 0) return res.Failure(new RuntimeError(posStart, posEnd, "Can not execute NULL", context));
            valueToCall = valueToCall.Copy().SetPosition(posStart, posEnd).SetContext(context);

            if (valueToCall is BaseFunction)
                if ((valueToCall as BaseFunction).IsStatic) {
                    //Debug.WriteLine(argNodes.Join("; "));
                    //Debug.WriteLine("Method is static");
                    argNodes.RemoveAt(0);
                }

            foreach (Node argNode in argNodes)
            {
                args.Add(res.Register(argNode.Visit(context)));
                if (res.ShouldReturn()) return res;
            }

            Value retValue = res.Register(valueToCall.Execute(args));

            if(retValue == null && Intepreter.GetOpt("disable-null-warning") != "true")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] A method seems to have returned nothing (NULL) - this is not supported. Crashes caused by null are not an issue. See https://github.com/GoldenretriverYT/spaghetto/wiki/Warning-about-NULL-return-value for more details");
                Console.ResetColor();
            }

            if (res.ShouldReturn()) return res;
            if (retValue != null) retValue = retValue.Copy().SetPosition(posStart, posEnd).SetContext(context);

            return res.Success(retValue);
        }
    }
}
