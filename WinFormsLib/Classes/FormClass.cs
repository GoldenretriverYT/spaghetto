using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spaghetto
{
    public class FormClass
    {
        public static Class @class = new("Form", new()
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
            }
        }, new() {}, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            ClassInstance classInstance = (ctx.symbolTable.Get("this") as ClassInstance);
            classInstance.instanceValues.Set("title", args[0]);
            classInstance.hiddenValues.Add("form", new Form());
            (classInstance.hiddenValues["form"] as Form).Text = (args[0] as StringValue).value;
            return ctx.symbolTable.Get("title");
        }, new() { "title" }, true));

        public static void InitStatics()
        {
           
        }
    }
}