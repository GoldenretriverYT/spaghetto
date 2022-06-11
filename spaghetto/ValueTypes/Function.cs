using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class Function : BaseFunction
    {
        public string functionName;
        public Node bodyNode;
        public List<string> argNames;
        public bool shouldAutoReturn;

        public Function(string? functionName, Node bodyNode, List<string> argNames, bool shouldAutoReturn) {
            this.functionName = (functionName ?? "<anon func>");
            this.bodyNode = bodyNode;
            this.argNames = argNames;
            this.shouldAutoReturn = shouldAutoReturn;
        }

        public override Value Copy() {
            return new Function(functionName, bodyNode, argNames, shouldAutoReturn).SetContext(context).SetPosition(posStart, posEnd);
        }

        public override RuntimeResult Execute(List<Value> args) {
            RuntimeResult res = new();
            Intepreter intepreter = new();
            Context newContext;

            try {
                newContext = new(functionName, context, posStart);
            }catch(StackOverflowException ex) {
                return res.Failure(new SpaghettoException(posStart, posEnd, "Stack Overflow", ex.Message));
            }

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
            if (res.ShouldReturn() && res.functionReturnValue == null) return res;

            Value retValue = ((shouldAutoReturn ? value : null) ?? res.functionReturnValue) ?? new Number(0);
            return res.Success(retValue);
        }

        public override string ToString() {
            return this.Represent();
        }

        public override string Represent() {
            return $"<function {functionName}>";
        }
    }
}
