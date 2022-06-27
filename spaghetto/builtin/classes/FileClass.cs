using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class FileClass
    {
        public static Class @class = new("File", new()
        {
            {
                "read",
                new NativeFunction("read", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;
                    
                    return new StringValue(File.ReadAllText(str));
                }, new() { "self" }, false)
            },
        }, new() {}, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            (ctx.symbolTable.Get("this") as ClassInstance).instanceValues.Set("path", args[0]);
            return ctx.symbolTable.Get("this");
        }, new() { "path" }, true));

        public static void InitStatics()
        {
           
        }
    }
}