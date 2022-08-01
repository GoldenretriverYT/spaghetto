using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class Function : BaseFunction
    {
        public string functionName;
        public Node bodyNode;
        public override List<string> ArgNames { get; set; }
        public bool shouldAutoReturn;
        public override bool IsStatic { get; set; }

        public Function(string? functionName, Node bodyNode, List<string> argNames, bool shouldAutoReturn) {
            this.functionName = (functionName ?? "<anon func>");
            this.bodyNode = bodyNode;
            this.ArgNames = argNames;
            this.shouldAutoReturn = shouldAutoReturn;
        }

        public override Value Copy() {
            return new Function(functionName, bodyNode, ArgNames, shouldAutoReturn).SetContext(context).SetPosition(posStart, posEnd);
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

            newContext.symbolTable = new((SymbolTable)newContext.parentContext.symbolTable.Clone());

            if (args.Count > ArgNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too many arguments passed into {functionName} > Got {args.Count} but expected {ArgNames.Count}", context));
            }else if (args.Count < ArgNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too few arguments passed into {functionName} > Got {args.Count} but expected {ArgNames.Count}", context));
            }

            for(int i = 0; i < args.Count; i++) {
                string argName = ArgNames[i];
                Value argValue = args[i];
                argValue.SetContext(newContext);
                newContext.symbolTable.Set(argName, argValue);
            }

            Value value = res.Register(Intepreter.Visit(bodyNode, newContext));
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

        public override BaseFunction SetStatic(bool st) {
            IsStatic = st;
            return this;
        }
    }
}
