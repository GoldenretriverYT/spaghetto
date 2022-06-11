﻿namespace spaghetto {
    internal class VariableAssignNode : Node {
        public Token varNameToken;
        public Node valueNode;
        public string varName;

        public VariableAssignNode(Token varNameToken, Node valueNode) {
            this.varNameToken = varNameToken;
            this.valueNode = valueNode;
            this.varName = (string)varNameToken.value;

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
    }
}
