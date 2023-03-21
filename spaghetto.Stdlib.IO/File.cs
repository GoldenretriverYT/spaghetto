using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Stdlib.IO {
    public class File {
        public static SClass CreateClass() {
            var @class = new SClass("file");

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

            return @class;
        }
    }
}
