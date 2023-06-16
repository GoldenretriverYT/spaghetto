namespace spaghetto.Parsing.Nodes
{
    public abstract class SyntaxNode
    {
        public virtual int StartPosition { get; internal set; }
        public virtual int EndPosition { get; internal set; }

        public abstract NodeType Type { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public SyntaxNode(int startPos, int endPos) {
            StartPosition = startPos;
            EndPosition = endPos;
        }
    }
}
