namespace spaghetto {
    public class StringNode : Node {
        public Token token;
        public Value cache;

        public StringNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
            this.cache = new StringValue(token.value.ToString()).SetPosition(this.posStart, this.posEnd);
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(cache.SetContext(context));
        }
    }
}
