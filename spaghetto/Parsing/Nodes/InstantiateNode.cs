using spaghetto.Helpers;

namespace spaghetto.Parsing.Nodes
{
    internal class InstantiateNode : SyntaxNode
    {
        private SyntaxToken ident;
        private List<SyntaxNode> argumentNodes;

        public InstantiateNode(SyntaxToken ident, List<SyntaxNode> argumentNodes) : base(ident.Position, argumentNodes.GetEndingPosition(ident.EndPosition))
        {
            this.ident = ident;
            this.argumentNodes = argumentNodes;
        }

        public override NodeType Type => NodeType.Instantiate;

        public override SValue Evaluate(Scope scope)
        {
            var @class = scope.Get(ident.Text);
            if (@class == null || @class is not SClass sclass) throw new Exception("Class not found!");


            var instance = new SClassInstance(sclass);

            List<SValue> args = new() { instance };
            foreach (var n in argumentNodes) args.Add(n.Evaluate(scope));

            instance.CallConstructor(scope, args);

            return instance;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}
