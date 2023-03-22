using spaghetto.BuiltinTypes;

namespace spaghetto.Parsing.Nodes
{
    internal class DotNode : SyntaxNode
    {
        public DotNode(SyntaxNode callNode)
        {
            CallNode = callNode;
        }

        public SyntaxNode CallNode { get; }
        public List<SyntaxNode> NextNodes { get; internal set; } = new();

        public override NodeType Type => NodeType.Dot;

        public override SValue Evaluate(Scope scope)
        {
            var currentValue = CallNode.Evaluate(scope);

            foreach (var node in NextNodes)
            {
                if (node is IdentifierNode rvn)
                {
                    var ident = rvn.Token;
                    currentValue = currentValue.Dot(new SString((string)ident.Value));
                }
                else if (node is CallNode cn)
                {
                    if (cn.ToCallNode is IdentifierNode cnIdentNode)
                    {
                        var ident = cnIdentNode.Token;
                        var lhs = currentValue.Dot(new SString((string)ident.Value));

                        var args = cn.EvaluateArgs(scope);
                        if (lhs is SBaseFunction func && func.IsClassInstanceMethod) args.Insert(0, currentValue);

                        currentValue = lhs.Call(scope, args);
                    }
                    else
                    {
                        throw new Exception("Tried to call a non identifier in dot node stack.");
                    }
                }
                else
                {
                    throw new Exception("Unexpected node in dot node stack!");
                }
            }

            return currentValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CallNode;

            foreach (var node in NextNodes) yield return node;
        }

        public override string ToString()
        {
            return "DotNode:";
        }
    }
}
