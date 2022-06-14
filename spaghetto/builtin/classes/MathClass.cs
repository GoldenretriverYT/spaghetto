using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class MathClass
    {
        private static readonly Random rnd = new();

        public static Class @class = new("Math", new(), new()
        {
            { "PI", new Number(Math.PI) },
            { "E", new Number(Math.E) },

            {
                "random", new NativeFunction("random", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    return new Number(rnd.NextDouble());
                }, new(), true)
            },

            { "floor", new NativeFunction("floor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
                {
                    return new Number(Math.Floor((args[0] as Number).value));
                }, new() {"num"}, true) },
        });
    }
}
