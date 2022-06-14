namespace spaghetto {
    public class ValueNode : Node {
        public Value value;

        public ValueNode(Value value, Position posStart, Position posEnd) {
            this.value = value;
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(value);
        }

        public override string ToString()
        {
            return "<ValueNode " + value + ">";
        }
    }
}
