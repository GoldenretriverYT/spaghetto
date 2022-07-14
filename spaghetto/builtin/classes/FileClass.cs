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
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;
                    
                    return new StringValue(File.ReadAllText(str));
                }, new() { "self" }, false)
            },
            {
                "exists",
                new NativeFunction("exists", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;

                    return new Number(File.Exists(str) ? 1 : 0);
                }, new() { "self" }, false)
            },
            {
                "create",
                new NativeFunction("create", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;

                    File.Create(str).Close();
                    return new Number(1);
                }, new() { "self" }, false)
            },
            {
                "write",
                new NativeFunction("write", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;
                    string data = (args[1] as StringValue).value;

                    File.WriteAllText(str, data);
                    return new Number(1);
                }, new() { "self: File", "data: String" }, false)
            },
            {
                "append",
                new NativeFunction("append", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = (args[0] as ClassInstance);

                    Value val = classInstance.Get("path");
                    string str = (val as StringValue).value;
                    string data = (args[1] as StringValue).value;

                    File.AppendAllText(str, data);
                    return new Number(1);
                }, new() { "self: File", "data: String" }, false)
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