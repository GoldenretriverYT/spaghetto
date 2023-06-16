namespace spaghetto.Parsing.Nodes
{
    internal class InitVariableNode : SyntaxNode
    {
        public SyntaxToken ident;
        public SyntaxNode expr;
        public readonly bool isFixedType = true;
        public readonly bool isConst = false;

        public InitVariableNode(SyntaxToken ident, bool isFixedType, bool isConst = false) : base(ident.Position, ident.EndPosition) {
            this.ident = ident;
            this.isFixedType = isFixedType;
            this.isConst = isConst;
        }

        public InitVariableNode(SyntaxToken ident, SyntaxNode expr, bool isFixedType, bool isConst = false) : base(ident.Position, expr.EndPosition)
        {
            this.ident = ident;
            this.expr = expr;
            this.isFixedType = isFixedType;
            this.isConst = isConst;
        }

        public override NodeType Type => NodeType.InitVariable;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            if (expr != null) yield return expr;
        }

        public override string ToString()
        {
            return "InitVariableNode:";
        }
    }
}
