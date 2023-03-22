using spaghetto.BuiltinTypes;
using spaghetto.Parsing.Nodes;

namespace spaghetto
{
    public class SFunction : SBaseFunction {
        public override SBuiltinType BuiltinName => SBuiltinType.Function;
        public string FunctionName { get; set; }
        public SyntaxNode Callback { get; set; }
        public Scope DefiningScope { get; set; }


        public SFunction(Scope definingScope, string functionName, List<string> args, SyntaxNode callback) {
            DefiningScope = definingScope;
            FunctionName = functionName;
            ExpectedArgs = args;
            Callback = callback;
        }

        public override SValue Call(Scope scope, List<SValue> args) {
            if (args.Count != ExpectedArgs.Count) throw new Exception(FunctionName + " expected " + ExpectedArgs.Count + " arguments. (" + string.Join(", ", ExpectedArgs) + ")");

            Scope funcScope = new(DefiningScope);
            
            for(int i = 0; i < ExpectedArgs.Count; i++) {
                funcScope.Set(ExpectedArgs[i], args[i]);
            }

            Callback.Evaluate(funcScope);

            return funcScope.ReturnValue;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
