namespace spaghetto {
    public class FunctionDefinitionNode : Node {
        public Token varNameToken = null;
        public List<Token> argNameTokens;
        public Node bodyNode;
        public bool shouldAutoReturn;

        public FunctionDefinitionNode(Token varNameToken, List<Token> argNameTokens, Node bodyNode, bool shouldAutoReturn) {
            this.varNameToken = varNameToken;
            this.argNameTokens = argNameTokens;
            this.bodyNode = bodyNode;
            this.shouldAutoReturn = shouldAutoReturn;

            if (varNameToken != null) {
                posStart = varNameToken.posStart;
            } else if(argNameTokens.Count > 0) {
                posStart = argNameTokens[0].posStart;
            } else {
                posStart = bodyNode.posStart;
            }

            posEnd = bodyNode.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            string funcName = (varNameToken == null ? null : (string)varNameToken.value);
            List<string> argNames = (from tok in argNameTokens select (string)tok.value).ToList();
            Value funcValue = new Function(funcName, bodyNode, argNames, shouldAutoReturn).SetContext(context).SetPosition(posStart, posEnd);

            if(varNameToken != null) {
                context.symbolTable.Set(funcName, funcValue);
            }

            return res.Success(funcValue);
        }
    }
}
