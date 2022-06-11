namespace spaghetto {
    internal class VariableAccessNode : Node {
        public Token varNameToken;

        public VariableAccessNode(Token varNameToken) {
            this.varNameToken = varNameToken;
            this.posStart = varNameToken.posStart;
            this.posEnd = varNameToken.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            string varName = (string)varNameToken.value;
            Value value = context.symbolTable.Get(varName);

            if(value == null) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"{varName} is not defined", context));
            }

            value = value.Copy().SetPosition(posStart, posEnd).SetContext(context);
            return res.Success(value);
        }
    }
}
