using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class Class : Value
    {
        public string name;
        public SymbolTable instanceTable; // The class itself may only define instance functions
        public SymbolTable staticTable;
        public BaseFunction? constructor;

        public Class(string name, SymbolTable instanceTable, SymbolTable staticTable, BaseFunction constructor = null)
        {
            this.name = name;
            this.instanceTable = instanceTable;
            this.staticTable = staticTable;
            this.constructor = constructor;

            if(instanceTable.Get("toString") == null) // Class has not defined a toString method, so we should insert one
            {
                instanceTable.Add("toString", new NativeFunction("toString", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    return new StringValue(args[0].ToString());
                }, new() { "self" }, false));
            }

            if(staticTable.Get("new") == null && constructor != null)
            {
                staticTable.Add("new", new NativeFunction("new", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    var res = new RuntimeResult();
                    Value ret = res.Register(constructor.Execute(args));
                    if (res.error) throw res.error;

                    return ret;
                }, constructor.ArgNames, true));
            }
        }

        public override Value Get(string identifier)
        {
            Value st = this.staticTable.Get(identifier);
            if (st != null) return st;
            Value ins = this.instanceTable.Get(identifier);
            if (ins != null) return ins;

            return new Number(0);
        }

        public override Value Copy()
        {
            return new Class(name, instanceTable, staticTable);
        }

        public override string Represent()
        {
            return "<class @" + name + ">";
        }
    }
}
