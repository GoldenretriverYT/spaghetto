namespace spaghetto.Parsing.Nodes
{
    public class BoolNode : SyntaxNode
    {
        public override NodeType Type => NodeType.BooleanLiteral;

        public bool Value { get; set; }

        public BoolNode(bool value)
        {
            Value = value;
        }

        public override SValue Evaluate(Scope scope)
        {
            return new SInt(Value ? 1 : 0);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "BoolNode: " + Value;
        }
    }
}
