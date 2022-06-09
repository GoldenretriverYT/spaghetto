using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class Function : Value {
        public string functionName;
        public Node bodyNode;
        public List<string> argNames;

        public Function(string? functionName, Node bodyNode, List<string> argNames) {
            this.functionName = (functionName ?? "<anon func>");
            this.bodyNode = bodyNode;
            this.argNames = argNames;
        }

        public override Value Copy() {
            return new Function(functionName, bodyNode, argNames).SetContext(context).SetPosition(posStart, posEnd);
        }

        public override RuntimeResult Execute(List<Value> args) {
            RuntimeResult res = new();
            Intepreter intepreter = new();
            Context newContext = new(functionName, context, posStart);
            newContext.symbolTable = new(newContext.parentContext.symbolTable);

            if(args.Count > argNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too many arguments passed into {functionName}", context));
            }else if (args.Count < argNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too few arguments passed into {functionName}", context));
            }

            for(int i = 0; i < args.Count; i++) {
                string argName = argNames[i];
                Value argValue = args[i];
                argValue.SetContext(newContext);
                newContext.symbolTable.Set(argName, argValue);
            }

            Value value = res.Register(intepreter.Visit(bodyNode, newContext));
            if (res.error) return res;

            return res.Success(value);
        }

        public override string ToString() {
            return $"<function {functionName}>";
        }
    }
}
