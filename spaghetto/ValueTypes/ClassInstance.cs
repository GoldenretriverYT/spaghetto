using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class ClassInstance : Value
    {
        public Class clazz;
        public SymbolTable instanceValues;
        private List<Value> args
        {
            get;
            set;
        }

        public ClassInstance(Class clazz, List<Value> args = null, SymbolTable instanceValues = null, bool construct = true)
        {
            this.args = args;
            this.clazz = clazz;
            this.instanceValues = (instanceValues == null ? new()
            {
                { "this", this }
            } : instanceValues);
            this.instanceValues.parent = this.clazz.instanceTable;
            this.clazz.instanceTable.parent = this.clazz.staticTable;

            Context newCtx = MakeContext();

            if (this.clazz.constructor is not null && construct) 
            {
                if(args != null && this.clazz.constructor.ArgNames.Count == args.Count)
                {
                    clazz.constructor.SetContext(newCtx).Execute(args);
                }else
                {
                    throw new RuntimeError(this.posStart, this.posEnd, "Passed arguments to not match expected constructor argument count (got " + args.Count + ", expected " + this.clazz.constructor.ArgNames.Count + ")", newCtx);
                }
            }

        }

        public Context MakeContext()
        {
            Context instanceContext = new Context("<instanceof @" + clazz.name + ">", context);
            instanceContext.symbolTable = this.instanceValues;

            return instanceContext;
        }

        public override Value Get(string identifier)
        {
            Value ins = this.instanceValues.Get(identifier);
            if (ins != null) return ins;

            return new Number(0);
        }

        public override Value Copy()
        {
            return new ClassInstance(clazz, args.ToList(), (SymbolTable)instanceValues.Clone(), false);
        }

        public override string Represent()
        {
            return "<instanceof @" + clazz.name + ">";
        }
    }
}
