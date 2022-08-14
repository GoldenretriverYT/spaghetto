namespace spaghetto {
    public class BinaryOperationNode : Node {
        public Node leftNode, rightNode;
        public Token op;
        public Value cache = null;

        public BinaryOperationNode(Node leftNode, Token op, Node rightNode) {
            this.leftNode = leftNode;
            this.op = op;
            this.rightNode = rightNode;

            this.posStart = leftNode.posStart;
            this.posEnd = rightNode.posEnd;

            if((leftNode is NumberNode && rightNode is NumberNode)) { // Static nodes, why not optimize?
                var l = (leftNode as NumberNode).cache as Number;
                var r = (rightNode as NumberNode).cache as Number;

                CacheBinop(l, r);
            } else if((leftNode is BinaryOperationNode && rightNode is NumberNode) || (rightNode is BinaryOperationNode && leftNode is NumberNode)) {
                var binopNode = (leftNode is BinaryOperationNode ? leftNode : rightNode) as BinaryOperationNode;
                var numNode = (leftNode is NumberNode ? leftNode : rightNode) as NumberNode;

                if(binopNode.cache != null) {
                    var l = binopNode.cache as Number;
                    var r = numNode.cache as Number;

                    CacheBinop(l, r);
                }
            }else if ((leftNode is BinaryOperationNode && rightNode is BinaryOperationNode)) { // If both are binop nodes and cached, you can cache them too!
                var bLeft = leftNode as BinaryOperationNode;
                var bRight = rightNode as BinaryOperationNode;

                if(bLeft.cache != null && bRight.cache != null) {
                    var l = bLeft.cache as Number;
                    var r = bRight.cache as Number;

                    CacheBinop(l, r);
                }
            }
        }

        public void CacheBinop(Number l, Number r) {
            switch (op.type) {
                case TokenType.Plus:
                    cache = new Number(l.value + r.value).SetPosition(posStart, posEnd);
                    break;
                case TokenType.Minus:
                    cache = new Number(l.value - r.value).SetPosition(posStart, posEnd);
                    break;
                case TokenType.Mul:
                    cache = new Number(l.value * r.value).SetPosition(posStart, posEnd);
                    break;
                case TokenType.Div:
                    if (r.value == 0) throw new DivideByZeroException("OptimizationFailure: Division by zero detected");
                    cache = new Number(l.value / r.value).SetPosition(posStart, posEnd);
                    break;
                case TokenType.Mod:
                    if (r.value == 0) throw new DivideByZeroException("OptimizationFailure: Division by zero detected");
                    cache = new Number(l.value % r.value).SetPosition(posStart, posEnd);
                    break;

            }
        }

        public override string ToString() {
            return $"binop({leftNode}, {op}, {rightNode}, cached: {(cache != null ? "true (" + cache + ")" : "false")})";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new RuntimeResult();
            if (cache != null) return res.Success(cache.SetContext(context));

            Value result = null;
            SpaghettoException error = null;

            Value left = res.Register(leftNode.Visit(context));
            if (res.ShouldReturn()) return res;
            Value right = res.Register(rightNode.Visit(context));
            if (res.ShouldReturn()) return res;

            switch(op.type) {
                case TokenType.Plus:
                    (result, error) = left.AddedTo(right); break;
                case TokenType.Minus:
                    (result, error) = left.SubtractedBy(right); break;
                case TokenType.Mul:
                    (result, error) = left.MultipliedBy(right); break;
                case TokenType.Div:
                    (result, error) = left.DividedBy(right); break;
                case TokenType.Pow:
                    (result, error) = left.PoweredBy(right); break;
                case TokenType.Mod:
                    (result, error) = left.Modulo(right); break;
                case TokenType.EqualsEquals:
                    (result, error) = left.IsEqualTo(right); break;
                case TokenType.NotEquals:
                    (result, error) = left.IsNotEqualTo(right); break;
                case TokenType.GreaterThan:
                    (result, error) = left.IsGreaterThan(right); break;
                case TokenType.GreaterThanOrEquals:
                    (result, error) = left.IsGreaterThanOrEquals(right); break;
                case TokenType.LessThan:
                    (result, error) = left.IsLessThan(right); break;
                case TokenType.LessThanOrEquals:
                    (result, error) = left.IsLessThanOrEquals(right); break;
                case TokenType.Index:
                    (result, error) = left.IndexedBy(right); break;
                default:
                    if (op.Matches(TokenType.Keyword, "and")) {
                        (result, error) = left.AndBy(right);
                    } else if (op.Matches(TokenType.Keyword, "or")) {
                        (result, error) = left.OrBy(right);
                    } else {
                        throw new Exception("Internal error occurred. Unknown operator type " + op.type);
                    }

                    break;
            }

            if (error) return res.Failure(error);

            return res.Success(result.SetPosition(this.posStart, this.posEnd));
        }
    }
}
