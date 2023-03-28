using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Stdlib.Reflection {
    public class ClassWrapper {
        public static SClass CreateClass() {
            var @class = new SClass("ClassWrapper");

            @class.InstanceBaseTable.Add((new SString("$$ctor"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (args[1] is not SClass @class) throw new Exception("Expected argument 0 to be a class");

                    self.NativeProperties.Add("class", @class);
                    return self;
                },
                expectedArgs: new() { "self", "class" }
            )));

            // TODO: Add functionality
            /*@class.InstanceBaseTable.Add((new SString("getSize"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (self.NativeProperties["fileInfo"] is not FileInfo fi) throw new Exception("unexpected error!");
                    return new SInt((int)(fi).Length);
                },
                expectedArgs: new() { "self" },
                isClassInstanceFunc: true
            )));*/

            return @class;
        }
    }
}
