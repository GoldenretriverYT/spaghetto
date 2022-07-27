using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.builtin.classes.reflection {
    public class ClassWrapper {
        public static Class @class = new("ClassWrapper", new()
        {
            {
                "getInstance",
                new NativeFunction("getInstance", (args, posStart, posEnd, ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = args[0] as ClassInstance;
                    Class cl = (classInstance.hiddenValues["class"]) as Class;
                    string targetName = (args[1] as StringValue).value;

                    Value v = cl.instanceTable.Get(targetName);

                    return v;
                }, new() { "self", "name" }, false)
            },
            {
                "setInstance",
                new NativeFunction("setInstance", (args, posStart, posEnd, ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = args[0] as ClassInstance;
                    Class cl = (classInstance.hiddenValues["class"]) as Class;
                    string targetName = (args[1] as StringValue).value;

                    cl.instanceTable.Set(targetName, args[2]);

                    return new Number(0);
                }, new() { "self", "name", "value" }, false)
            },
             {
                "getStatic",
                new NativeFunction("getStatic", (args, posStart, posEnd, ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = args[0] as ClassInstance;
                    Class cl = (classInstance.hiddenValues["class"]) as Class;
                    string targetName = (args[1] as StringValue).value;

                    Value v = cl.staticTable.Get(targetName);

                    return v;
                }, new() { "self", "name" }, false)
            },
            {
                "setStatic",
                new NativeFunction("setStatic", (args, posStart, posEnd, ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = args[0] as ClassInstance;
                    Class cl = (classInstance.hiddenValues["class"]) as Class;
                    string targetName = (args[1] as StringValue).value;
                    Value targetValue = args[2];

                    if(targetValue is Function || targetValue is NativeFunction) {
                        targetValue = (targetValue.Copy() as BaseFunction).SetStatic(true);
                        Debug.WriteLine("Making static");
                    }

                    cl.staticTable.Set(targetName, targetValue);

                    return new Number(0);
                }, new() { "self", "name", "value" }, false)
            },
            {
                "getName",
                new NativeFunction("getName", (args, posStart, posEnd, ctx) =>
                {
                    if (args[0] is Class) throw new RuntimeError(posStart, posEnd, "Called method is an instance method and not static.", ctx);
                    ClassInstance classInstance = args[0] as ClassInstance;
                    Class cl = (classInstance.hiddenValues["class"]) as Class;

                    return new StringValue(cl.name);
                }, new() { "self" }, false)
            },
        }, new() { }, new NativeFunction("ctor", (args, posStart, posEnd, ctx) => {
            (ctx.symbolTable.Get("this") as ClassInstance).hiddenValues.Add("class", args[0]);
            return ctx.symbolTable.Get("this");
        }, new() { "class" }, true));

        public static void InitStatics() {

        }
    }
}