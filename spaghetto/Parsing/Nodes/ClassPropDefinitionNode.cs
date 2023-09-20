namespace spaghetto.Parsing.Nodes
{
    internal class ClassPropDefinitionNode : SyntaxNode
    {
        public SyntaxToken Name { get; }
        public SyntaxNode Expression { get; }
        public bool IsStatic { get; }

        public ClassPropDefinitionNode(SyntaxToken name, SyntaxNode expr, bool isStatic) : base(name.Position, expr.EndPosition)
        {
            this.Name = name;
            this.Expression = expr;
            this.IsStatic = isStatic;
        }

        public override NodeType Type => NodeType.ClassPropertyDefinition;

        public override SValue Evaluate(Scope scope) {
            throw new NotImplementedException("This should not be called!");
        }

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(Name);
            yield return Expression;
        }

        public override string GenerateSource(int depth) {
            return (IsStatic ? "static " : "") + "prop " + Name.Text + " = " + Expression.GenerateSource(depth + 1);
        }
    }
}
