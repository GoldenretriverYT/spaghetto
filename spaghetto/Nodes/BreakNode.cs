namespace spaghetto {
    public class BreakNode : Node {
        public BreakNode(Position posStart, Position posEnd) {
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().SuccessBreak();
        }
    }
}
