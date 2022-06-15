using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class TestObject
    {
        public static Class @class = new("TestObject", new(), new() {}, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            Console.WriteLine(args[1].value);
            return args[0];
        }, new() { "str" }, false));

        public static void InitStatics()
        {
            @class.staticTable.Add("default", new ClassInstance(@class, new() { new StringValue("wow") }));
        }
    }
}