namespace spaghetto.Parsing.Nodes
{
    internal class ClassFunctionDefinitionNode : SyntaxNode
    {
        public SyntaxToken name;
        public List<SyntaxToken> args;
        public SyntaxNode body;
        public bool isStatic;

        public ClassFunctionDefinitionNode(SyntaxToken name, List<SyntaxToken> args, SyntaxNode body, bool isStatic) : base(name.Position, body.EndPosition)
        {
            this.name = name;
            this.args = args;
            this.body = body;
            this.isStatic = isStatic;
        }

        public override NodeType Type => NodeType.ClassFunctionDefinition;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(name);
            foreach (var tok in args) yield return new TokenNode(tok);
            yield return body;
        }
    }
}
