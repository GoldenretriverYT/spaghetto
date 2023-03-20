namespace spaghetto.Parsing.Nodes
{
    internal class AssignVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;

        public AssignVariableNode(SyntaxToken ident, SyntaxNode expr)
        {
            this.ident = ident;
            this.expr = expr;
        }

        public override NodeType Type => NodeType.AssignVariable;

        public override SValue Evaluate(Scope scope)
        {
            if (scope.Get(ident.Value.ToString()) == null)
            {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = expr.Evaluate(scope);
            var key = ident.Value.ToString();
            if (!scope.Update(key, val, out Exception ex)) throw ex;
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return expr;
        }

        public override string ToString()
        {
            return "AssignVariableNode:";
        }
    }
}
