namespace spaghetto.Parsing.Nodes
{
    internal class CallNode : SyntaxNode
    {
        public SyntaxNode ToCallNode { get; set; }
        private List<SyntaxNode> argumentNodes;

        public CallNode(SyntaxNode atomNode, List<SyntaxNode> argumentNodes)
        {
            ToCallNode = atomNode;
            this.argumentNodes = argumentNodes;
        }

        public override NodeType Type => NodeType.Call;

        public override SValue Evaluate(Scope scope)
        {
            var toCall = ToCallNode.Evaluate(scope);
            var args = EvaluateArgs(scope);

            return toCall.Call(scope, args);
        }

        public List<SValue> EvaluateArgs(Scope scope)
        {
            var args = new List<SValue>();

            foreach (var n in argumentNodes) args.Add(n.Evaluate(scope));
            return args;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ToCallNode;
            foreach (var n in argumentNodes) yield return n;
        }

        public override string ToString()
        {
            return "CallNode:";
        }
    }
}
