using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Stdlib.Lang {
    public class Float {
        public static SClass CreateClass() {
            var @class = new SClass("float");

            @class.StaticTable.Add((new SString("parse"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (!float.TryParse(str.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float valFloat)) throw new Exception("Invalid number!");

                    return new SFloat(valFloat);
                },
                expectedArgs: new() { "toParse" }
            )));

            return @class;
        }
    }
}
