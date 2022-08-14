namespace spaghetto {
    public class NumberNode : Node {
        public Token token;
        public Value cache;

        public NumberNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
            this.cache = new Number((double)token.value).SetPosition(this.posStart, this.posEnd); // cant logically error anyways, so cache it
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(cache.SetContext(context));
        }

        public override string GenerateCSharp() {
            return token.value.ToString();
        }
    }
}
