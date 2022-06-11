namespace spaghetto {
    internal class WhileNode : Node {
        public Node condition, func;
        public bool shouldReturnNull;

        public WhileNode(Node condition, Node func, bool shouldReturnNull) {
            this.condition = condition;
            this.func = func;
            this.shouldReturnNull = shouldReturnNull;

            this.posStart = condition.posStart;
            this.posEnd = func.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            Value lastValue = null;
            
            while (res.Register(condition.Visit(context)).IsTrue()) {
                if (res.ShouldReturn()) return res;
                lastValue = res.Register(func.Visit(context));
                if (res.ShouldReturn() && res.loopShouldBreak == false && res.loopShouldContinue == false) return res;

                if (res.loopShouldContinue) continue;
                if (res.loopShouldBreak) break;
            }

            return res.Success((shouldReturnNull ? new Number(0) : lastValue));
        }
    }
}
