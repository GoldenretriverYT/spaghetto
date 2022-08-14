namespace spaghetto {
    public class ForNode : Node {
        public Token varNameToken;
        public Node varStartExpression, condition, continuationExpression, func;
        public bool isStepMode, shouldReturnNull;

        public ForNode(Token varNameToken, Node varStartExpression, Node condition, Node continuationExpression, bool isStep, Node func, bool shouldReturnNull) {
            this.varNameToken = varNameToken;
            this.varStartExpression = varStartExpression;
            this.condition = condition;
            this.continuationExpression = continuationExpression;
            this.isStepMode = isStep;
            this.func = func;
            this.shouldReturnNull = shouldReturnNull;

            this.posStart = varNameToken.posStart;
            this.posEnd = func.posEnd;
        }

        public override string ToString() {
            return "for(varStartExpr: " + varStartExpression + ", cond: " + condition + ", contExpr: " + continuationExpression + ", func:" + func + ")";
        }

        public override RuntimeResult Visit(Context context) {
            System.Diagnostics.Stopwatch sw = new();

            sw.Start();

            RuntimeResult res = new();
            string varName = varNameToken.value as string;
            Value value = res.Register(varStartExpression.Visit(context));
            Value lastValue = null;

            if (res.ShouldReturn()) return res;
            context.symbolTable.Set(varName, value);

            double currentIteratorValue = (value as Number).value;

            bool isStepStatic = (continuationExpression is NumberNode);

            double stepStaticValue = (isStepStatic ? (continuationExpression.Visit(context).value as Number).value : 0);

            while(res.Register(condition.Visit(context)).IsTrue()) {
                lastValue = res.Register(func.Visit(context));
                if (res.ShouldReturn()) return res;

                if (isStepMode) {
                    double stepResValue = 0;

                    if (!isStepStatic) {
                        stepResValue = (res.Register(continuationExpression.Visit(context)) as Number).value;
                        if (res.ShouldReturn() && !res.loopShouldBreak && !res.loopShouldContinue) return res;
                    }

                    currentIteratorValue += (isStepStatic ? stepStaticValue : stepResValue);
                    context.symbolTable.Set(varName, new Number(currentIteratorValue));
                } else {
                    res.Register(continuationExpression.Visit(context));
                }

                if (res.ShouldReturn() && !res.loopShouldBreak && !res.loopShouldContinue) continue;

                if (res.loopShouldContinue) continue;
                if (res.loopShouldBreak) break;
            }

            sw.Stop();

            return res.Success((shouldReturnNull ? new Number(0) : lastValue));
        }
    }
}
