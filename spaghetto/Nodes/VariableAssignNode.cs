namespace spaghetto {
    public class VariableAssignNode : Node {
        public Token varNameToken;
        public Node valueNode;
        public string varName;

        public VariableAssignNode(Token varNameToken, Node valueNode) {
            this.varNameToken = varNameToken;
            this.valueNode = valueNode;
            this.varName = varNameToken?.value as string;

            this.posStart = varNameToken.posStart;
            this.posEnd = valueNode.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            
            Value value = res.Register(valueNode.Visit(context));
            if (res.ShouldReturn()) return res;

            context.symbolTable.Set(varName, value);
            return res.Success(value);
        }

        public override string ToString() {
            return "varAssign(valueNode: " + valueNode + ")";
        }
    }
}
