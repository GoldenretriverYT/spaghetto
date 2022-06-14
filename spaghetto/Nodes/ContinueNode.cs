namespace spaghetto {
    public class ContinueNode : Node {
        public ContinueNode(Position posStart, Position posEnd) {
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().SuccessContinue();
        }
    }
}
