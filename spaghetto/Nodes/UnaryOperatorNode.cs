namespace spaghetto {
    public class UnaryOperatorNode : Node {
        public Node node;
        public Token opToken;

        public UnaryOperatorNode(Token opToken, Node node) {
            this.node = node;
            this.opToken = opToken;

            this.posStart = opToken.posStart;
            this.posEnd = node.posEnd;
        }

        public override string ToString() {
            return $"({opToken}, {node})";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new RuntimeResult();

            Value number = res.Register(node.Visit(context));
            if (res.ShouldReturn()) return res;

            SpaghettoException error = null;

            if (opToken.type == TokenType.Minus)
                (number, error) = number.MultipliedBy(new Number(-1));
            else if (opToken.Matches(TokenType.Keyword, "not"))
                (number, error) = number.Not();

            if (error) return res.Failure(error);
            return res.Success(number.SetPosition(this.posStart, this.posEnd));
        }
    }
}
