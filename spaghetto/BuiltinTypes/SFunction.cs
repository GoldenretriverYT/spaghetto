﻿using spaghetto.BuiltinTypes;
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

            // If the scope is the global scope, we need to clone it as otherwise we would have a reference to the global scope
            // and any changes to the scope would be reflected in the global scope
            //if (DefiningScope.ParentScope == null) {
            //    DefiningScope = DefiningScope.Clone();
            //} // What, this makes no sense as the functions runs in another scope, so it doesn't matter if we change the global scope
        }

        public override SValue Call(Scope scope, List<SValue> args) {
            if (args.Count != ExpectedArgs.Count) throw new Exception(FunctionName + " expected " + ExpectedArgs.Count + " arguments. (" + string.Join(", ", ExpectedArgs) + ")");

            Scope funcScope = new(DefiningScope, DefiningScope.CreatedPosition);
            
            for(int i = 0; i < ExpectedArgs.Count; i++) {
                funcScope.Set(ExpectedArgs[i], args[i]);
            }

            Callback.Evaluate(funcScope);
            scope.SetState(ScopeState.None);

            return funcScope.ReturnValue;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
