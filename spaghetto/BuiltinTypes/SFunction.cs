using spaghetto.BuiltinTypes;
using spaghetto.Parsing.Nodes;

namespace spaghetto
{
    public class SFunction : SBaseFunction {
        public override SBuiltinType BuiltinName => SBuiltinType.Function;
        public string FunctionName { get; set; }
        public List<string> Args { get; set; }
        public SyntaxNode Callback { get; set; }
        public Scope DefiningScope { get; set; }


        public SFunction(Scope definingScope, string functionName, List<string> args, SyntaxNode callback) {
            DefiningScope = definingScope;
            FunctionName = functionName;
            Args = args;
            Callback = callback;
        }

        public override SValue Call(Scope scope, List<SValue> args) {
            if (args.Count != Args.Count) throw new Exception(FunctionName + " expected " + Args.Count + " arguments. (" + string.Join(", ", Args) + ")");

            Scope funcScope = new(DefiningScope);
            
            for(int i = 0; i < Args.Count; i++) {
                funcScope.Set(Args[i], args[i]);
            }

            Callback.Evaluate(funcScope);

            return funcScope.ReturnValue;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
