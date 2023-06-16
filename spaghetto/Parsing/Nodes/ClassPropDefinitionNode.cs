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

        public override IEnumerable<SyntaxNode> GetChildren() {
            yield return new TokenNode(Name);
            yield return Expression;
        }
    }
}
