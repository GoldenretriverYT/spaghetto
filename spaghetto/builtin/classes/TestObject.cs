using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class TestObject
    {
        public static Class @class = new("TestObject", new()
        {
            {
                "printme",
                new NativeFunction("printme", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    ClassInstance classInstance = (args[0] as ClassInstance);
                    Value val = classInstance.Get("str");
                    string str = (val as StringValue).value;
                    Console.WriteLine(str);
                    return new StringValue(str);
                }, new() { "self" }, false)
            },
        }, new() {}, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            (ctx.symbolTable.Get("this") as ClassInstance).instanceValues.Set("str", args[0]);
            System.Diagnostics.Debug.WriteLine((ctx.symbolTable.Get("this") as ClassInstance).instanceValues.Get("str"));
            return ctx.symbolTable.Get("this");
        }, new() { "str" }, true));

        public static void InitStatics()
        {
            @class.staticTable.Add("default", new ClassInstance(@class, null, null, new() { new StringValue("wow") }));
        }
    }
}