using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class NativeFunction : BaseFunction {
        public string functionName;
        public Func<List<Value>, Position, Position, Context, Value> func;
        public override List<string> ArgNames { get; set; }

        public override bool IsStatic { get; set; }

        public NativeFunction(string? functionName, Func<List<Value>, Position, Position, Context, Value> func, List<string> argNames, bool isStatic) {
            this.functionName = (functionName ?? "<anon func>");
            this.func = func;
            this.ArgNames = argNames;
            this.IsStatic = isStatic;
        }

        /*public void Init(string? functionName, MethodInfo method, bool isStatic)
        {
            this.IsStatic = isStatic;
            this.functionName = (functionName ?? "<anon func>");
            this.UseReflection = true;
            this.method = method;

            ArgNames = new();

            foreach(ParameterInfo param in method.GetParameters())
            {
                if(param.ParameterType.IsSubclassOf(typeof(Value)))
                {
                    ArgNames.Add(param.Name);
                }
            }

            if ((!method.ReturnType.IsSubclassOf(typeof(Value))) && !(method.ReturnType == typeof(Value))) throw new Exception("ReturnType not subclass of Value");
        }*/

        public override Value Copy() {
            /*if(this.UseReflection)
                return new NativeFunction(functionName, method, IsStatic).SetContext(context).SetPosition(posStart, posEnd);
            else*/
                return new NativeFunction(functionName, func, ArgNames, IsStatic).SetContext(context).SetPosition(posStart, posEnd);
        }

        public override RuntimeResult Execute(List<Value> args) {
            RuntimeResult res = new();

            Context newContext;

            try {
                newContext = new(functionName, context, posStart);
            } catch (StackOverflowException ex) {
                return res.Failure(new SpaghettoException(posStart, posEnd, "Stack Overflow", ex.Message));
            }

            //newContext.symbolTable = new((SymbolTable)newContext.parentContext.symbolTable.Clone());
            newContext.symbolTable = new();
            newContext.symbolTable.parent = newContext.parentContext.symbolTable;

            if (args.Count > ArgNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too many arguments passed into {functionName}, List: " + ArgNames.Join(", ") + ", Provided: " + args.Join(", "), context));
            }else if (args.Count < ArgNames.Count) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"Too few arguments passed into {functionName}", context));
            }

            Value retValue = null;

            try {
                /*if (UseReflection)
                {
                    retValue = (Value)method.Invoke(null, args.ToArray<object>());
                }else
                {*/
                    retValue = func.Invoke(args, posStart, posEnd, context);
                //}
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
            return this.Represent();
        }

        public override string Represent() {
            return $"<native {functionName}({ArgNames.Join(", ")})>";
        }
    }
}
