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
