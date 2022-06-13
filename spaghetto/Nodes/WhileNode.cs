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

            int iter = 0;

            while (res.Register(condition.Visit(context)).IsTrue()) {
                if (res.ShouldReturn()) return res;

                Context newCtx = new("<while:iter" + iter + ">", context);
                newCtx.symbolTable = new((SymbolTable<Value>)newCtx.parentContext.symbolTable.Clone());
                lastValue = res.Register(func.Visit(newCtx));

                if (res.ShouldReturn() && res.loopShouldBreak == false && res.loopShouldContinue == false) return res;

                if (res.loopShouldContinue) continue;
                if (res.loopShouldBreak) break;

                iter++;
            }

            return res.Success((shouldReturnNull ? new Number(0) : lastValue));
        }
    }
}
