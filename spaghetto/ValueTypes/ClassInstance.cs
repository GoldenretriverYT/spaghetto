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
        private List<Value> args;

        public ClassInstance(Class clazz, List<Value> args = null)
        {
            this.args = args;
            this.clazz = clazz;
            this.instanceValues = new();
            this.instanceValues.parent = this.clazz.instanceTable;
            this.clazz.instanceTable.parent = this.clazz.staticTable;

            Context newCtx = MakeContext();

            if (this.clazz.constructor is not null)
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
            Value st = this.clazz.staticTable.Get(identifier);
            if (st != null) return st;
            Value ins = this.instanceValues.Get(identifier);
            if (ins != null) return ins;

            return new Number(0);
        }

        public override Value Copy()
        {
            return new ClassInstance(clazz, args);
        }

        public override string Represent()
        {
            return "<instanceof @" + clazz.name + ">";
        }
    }
}
