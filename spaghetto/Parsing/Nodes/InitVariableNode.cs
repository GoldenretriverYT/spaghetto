namespace spaghetto.Parsing.Nodes
{
    internal class InitVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        private readonly bool isFixedType = true;

        public InitVariableNode(SyntaxToken ident, bool isFixedType)
        {
            this.ident = ident;
            this.isFixedType = isFixedType;
        }

        public InitVariableNode(SyntaxToken ident, SyntaxNode expr, bool isFixedType)
        {
            this.ident = ident;
            this.expr = expr;
            this.isFixedType = isFixedType;
        }

        public override NodeType Type => NodeType.InitVariable;

        public override SValue Evaluate(Scope scope)
        {
            if (scope.Get(ident.Value.ToString()) != null)
            {
                throw new InvalidOperationException("Can not initiliaze the same variable twice!");
            }

            if (expr != null)
            {
                var val = expr.Evaluate(scope);
                val.TypeIsFixed = isFixedType;

                scope.Set(ident.Value.ToString(), val);
                return val;
            }
            else
            {
                if (isFixedType) throw new InvalidOperationException("Tried to initiliaze a fixed type variable with no value; this is not permitted. Use var% instead.");

                scope.Set(ident.Value.ToString(), SValue.Null);
                return SValue.Null;
            }

        }

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
