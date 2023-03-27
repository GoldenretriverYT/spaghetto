using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Stdlib.IO {
    public class File {
        public static SClass CreateClass() {
            var @class = new SClass("File");

            @class.StaticTable.Add((new SString("exists"), new SNativeFunction(
               impl: (Scope scope, List<SValue> args) => {
                   if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                   if (!System.IO.File.Exists(str.Value)) return SInt.Zero;

                   return SInt.One;
               },
               expectedArgs: new() { "path" }
           )));

            @class.StaticTable.Add((new SString("readtext"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (!System.IO.File.Exists(str.Value)) throw new Exception("File not found!");

                    return new SString(System.IO.File.ReadAllText(str.Value));
                },
                expectedArgs: new() { "path" }
            )));

            @class.StaticTable.Add((new SString("writetext"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (args[1] is not SString strData) throw new Exception("Expected argument 1 to be a string");


                    System.IO.File.WriteAllText(str.Value, strData.Value);
                    return new SInt(1);
                },
                expectedArgs: new() { "path", "data" }
            )));

            @class.InstanceBaseTable.Add((new SString("$$ctor"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (args[1] is not SString path) throw new Exception("Expected argument 0 to be a string");

                    if (!System.IO.File.Exists(path.Value)) throw new Exception("File not found!");

                    self.NativeProperties.Add("fileInfo", new FileInfo(path.Value));
                    return self;
                },
                expectedArgs: new() { "self", "path" }
            )));

            @class.InstanceBaseTable.Add((new SString("getSize"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (self.NativeProperties["fileInfo"] is not FileInfo fi) throw new Exception("unexpected error!");
                    return new SInt((int)(fi).Length);
                },
                expectedArgs: new() { "self" },
                isClassInstanceFunc: true
            )));

            return @class;
        }
    }
}
