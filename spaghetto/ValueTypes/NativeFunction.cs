using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class NativeFunction : BaseFunction {
        public string functionName
        public Func<List<Value>, Position, Position, Context, Value> func;
        public List<string> argNames;

        public NativeFunction(string? functionName, Func<List<Value>, Position, Position, Context, Value> func, List<string> argNames) {
            this.functionName = (functionName ?? "<anon func>");
            this.func = func;
            this.argNames = argNames;
        }

        public override Value Copy() {
            return new NativeFunction(functionName, func, argNames).SetContext(context).SetPosition(posStart, posEnd);
        }

        public override RuntimeResult Execute(List<Value> args) {
            RuntimeResult res = new();
            Context newContext = new(functionName, context, posStart);
            newContext.symbolTable = new(newContext.parentContext.symbolTable);

            if(args.Count > argNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too many arguments passed into {functionName}", context));
            }else if (args.Count < argNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too few arguments passed into {functionName}", context));
            }

            Value retValue = null;

            try {
                retValue = func.Invoke(args, posStart, posEnd, context);
            }catch(Exception ex) {
                if(ex is SpaghettoException) {
                    return res.Failure(ex as SpaghettoException);
                }else {
                    throw;
                }
            }
            
            return res.Success(retValue);
        }

        public override string ToString() {
            return $"<native {functionName}>";
        }
    }
}
