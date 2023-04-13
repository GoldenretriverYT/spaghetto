using System.Reflection;
using WinForm = System.Windows.Forms.Form;
namespace spaghetto.Stdlib.Interop
{
    public class Form
    {
        public static void Main(string[] args) { }
        public static SClass CreateClass()
        {
            var @class = new SClass("Form");

            @class.InstanceBaseTable.Add(("$$ctor", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");

                    self.NativeProperties["form"] = new WinForm();
                    return self;
                },
                expectedArgs: new() { "self" }
            )));

            @class.InstanceBaseTable.Add(("show", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");

                    (self.NativeProperties["form"] as WinForm).Show();
                    return SValue.Null;
                },
                expectedArgs: new() { "self" }
            )));

            return @class;
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
