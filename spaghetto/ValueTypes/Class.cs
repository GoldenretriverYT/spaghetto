using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    internal class Class : Value
    {
        public string name;
        public SymbolTable<Value> instanceTable;
        public SymbolTable<Value> staticTable;

        public Class(string name, SymbolTable<Value> instanceTable, SymbolTable<Value> staticTable)
        {
            this.name = name;
            this.instanceTable = instanceTable;
            this.staticTable = staticTable;

            if(instanceTable.Get("toString") == null) // Class has not defined a toString method, so we should insert one
            {
                instanceTable.Add("toString", new NativeFunction("toString", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    return new StringValue(args[0].ToString());
                }, new() { "self" }, false));
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
