using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto
{
    public class Path
    {
        public static Class @class = new("Path", new(){}, new() {
            { "desktop", new StringValue(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) },
            { "documents", new StringValue(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) },
            { "appdata", new StringValue(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) },

        }, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            /*(ctx.symbolTable.Get("this") as ClassInstance).instanceValues.Set("str", args[0]);
            System.Diagnostics.Debug.WriteLine((ctx.symbolTable.Get("this") as ClassInstance).instanceValues.Get("str"));
            return ctx.symbolTable.Get("this");*/

            throw new Exception("Path parsing soon!");
        }, new() { "str" }, true));

        public static void InitStatics()
        {
        }
    }
}