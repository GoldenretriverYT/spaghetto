namespace spaghetto {
    public class NumberNode : Node {
        public Token token;

        public NumberNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(new Number((double)token.value).SetContext(context).SetPosition(this.posStart, this.posEnd));
        }

        public override string GenerateCSharp() {
            return token.value.ToString();
        }
    }
}
