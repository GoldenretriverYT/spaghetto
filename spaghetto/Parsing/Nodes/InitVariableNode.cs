namespace spaghetto.Parsing.Nodes
{
    internal class InitVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        private readonly bool isFixedType = true;
        private readonly bool isConst = false;

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

        public override SValue Evaluate(Scope scope)
        {
            if (scope.Get(ident.Value.ToString()) != null)
            {
                return Scope.Error("Can not initiliaze the same variable twice!");
            }

            if (expr != null)
            {
                var val = expr.EvaluateWithErrorCheck(scope);
                val.TypeIsFixed = isFixedType;
                val.IsConstant = isConst;

                scope.Set(ident.Value.ToString(), val);
                return val;
            }
            else
            {
                if (isFixedType) return Scope.Error("Tried to initiliaze a fixed type variable with no value; this is not permitted. Use var% instead.");
                var nul = new SNull();
                nul.TypeIsFixed = isFixedType;
                nul.IsConstant = isConst;

                scope.Set(ident.Value.ToString(), nul);
                return nul;
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
