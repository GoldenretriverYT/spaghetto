namespace spaghetto.Parsing.Nodes
{
    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }

        public abstract SValue Evaluate(Scope scope);
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
