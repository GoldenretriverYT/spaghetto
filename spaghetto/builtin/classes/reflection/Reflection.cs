using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.builtin.classes.reflection {
    public class ReflectionClass {
        public static Class @class = new("Reflection", new()
        {
            
        }, new() {
            {
                "getEmptyClass",
                new NativeFunction("getEmptyClass", (args, posStart, posEnd, ctx) =>
                {
                    return new Class("empty", new(), new());
                }, new() { }, true)
            }
        }, new NativeFunction("ctor", (args, posStart, posEnd, ctx) => {
            return ctx.symbolTable.Get("this");
        }, new() { }, true));

        public static void InitStatics() {

        }
    }
}