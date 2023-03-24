namespace spaghetto.Parsing.Nodes
{
    internal class AssignVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;

        public AssignVariableNode(SyntaxToken ident, SyntaxNode expr) : base(ident.Position, expr.EndPosition)
        {
            this.Ident = ident;
            this.Expr = expr;
        }

        public override NodeType Type => NodeType.AssignVariable;

        public SyntaxToken Ident { get => ident; set => ident = value; }
        public SyntaxNode Expr { get => expr; set => expr = value; }

        public override SValue Evaluate(Scope scope)
        {
            if (scope.Get(Ident.Value.ToString()) == null)
            {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = Expr.Evaluate(scope);
            var key = Ident.Value.ToString();

            if (!scope.Update(key, val, out Exception ex)) throw ex;
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Ident);
            yield return Expr;
        }

        public override string ToString()
        {
            return "AssignVariableNode:";
        }
    }
}
