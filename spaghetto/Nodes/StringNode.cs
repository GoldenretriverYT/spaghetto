namespace spaghetto {
    public class StringNode : Node {
        public Token token;

        public StringNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(new StringValue((string)token.value).SetContext(context).SetPosition(this.posStart, this.posEnd));
        }
    }
}
