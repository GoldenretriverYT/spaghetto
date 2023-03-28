namespace spaghetto.Stdlib.Lang {
    public class List {
        public static SClass CreateClass() {
            var @class = new SClass("list");

            @class.StaticTable.Add((new SString("length"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SList lst) throw new Exception("Expected argument 0 to be a list");
                    return new SInt(lst.Value.Count);
                },
                expectedArgs: new() { "list" }
            )));

            @class.StaticTable.Add((new SString("contains"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SList lst) throw new Exception("Expected argument 0 to be a list");
                    
                    foreach(var val in lst.Value) {
                        if (val.Equals(args[1]).IsTruthy()) {
                            return SInt.One;
                        }
                    }

                    return SInt.Zero;
                },
                expectedArgs: new() { "list", "val" }
            )));

            return @class;
        }
    }
}
