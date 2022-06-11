namespace spaghetto {
    internal class BinaryOperationNode : Node {
        public Node leftNode, rightNode;
        public Token op;

        public BinaryOperationNode(Node leftNode, Token op, Node rightNode) {
            this.leftNode = leftNode;
            this.op = op;
            this.rightNode = rightNode;

            this.posStart = leftNode.posStart;
            this.posEnd = rightNode.posEnd;
        }

        public override string ToString() {
            return $"({leftNode}, {op}, {rightNode})";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new RuntimeResult();

            Value result = null;
            SpaghettoException error = null;

            Value left = res.Register(leftNode.Visit(context));
            if (res.ShouldReturn()) return res;
            Value right = res.Register(rightNode.Visit(context));
            if (res.ShouldReturn()) return res;

            if (op.type == TokenType.Plus) {
                (result, error) = left.AddedTo(right);
            } else if (op.type == TokenType.Minus) {
                (result, error) = left.SubtractedBy(right);
            } else if (op.type == TokenType.Mul) {
                (result, error) = left.MultipliedBy(right);
            } else if (op.type == TokenType.Div) {
                (result, error) = left.DividedBy(right);
            } else if (op.type == TokenType.Pow) {
                (result, error) = left.PoweredBy(right);
            } else if (op.type == TokenType.Mod) {
                (result, error) = left.Modulo(right);
            } else if (op.type == TokenType.EqualsEquals) {
                (result, error) = left.IsEqualTo(right);
            } else if (op.type == TokenType.NotEquals) {
                (result, error) = left.IsNotEqualTo(right);
            } else if (op.type == TokenType.GreaterThan) {
                (result, error) = left.IsGreaterThan(right);
            } else if (op.type == TokenType.GreaterThanOrEquals) {
                (result, error) = left.IsGreaterThanOrEquals(right);
            } else if (op.type == TokenType.LessThan) {
                (result, error) = left.IsLessThan(right);
            } else if (op.type == TokenType.LessThanOrEquals) {
                (result, error) = left.IsLessThanOrEquals(right);
            } else if (op.type == TokenType.LessThanOrEquals) {
                (result, error) = left.IsLessThanOrEquals(right);
            } else if (op.type == TokenType.Index) {
                (result, error) = left.IndexedBy(right);
            } else if (op.Matches(TokenType.Keyword, "and")) {
                (result, error) = left.AndBy(right);
            } else if (op.Matches(TokenType.Keyword, "or")) {
                (result, error) = left.OrBy(right);
            } else {
                throw new Exception("Internal error occurred. Unknown operator type " + op.type);
            }

            if (error) return res.Failure(error);

            return res.Success(result.SetPosition(this.posStart, this.posEnd));
        }
    }
}
