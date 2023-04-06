namespace spaghetto.Stdlib.IO {
    public class EnumVal {
        public static SClass CreateClass() {
            var @class = new SClass("EnumVal");

            @class.InstanceBaseTable.Add(("$$ctor", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (args[1] is not SList entries) throw new Exception("Expected argument 0 to be a list");

                    var enumToStringLookup = new List<(int, SString)>();

                    var i = 0;

                    foreach(var v in entries.Value) {
                        if (v is not SString str) throw new Exception("Expected argument 0s list to only be strings");

                        self.InstanceTable.Add((str.Value, new SInt(i)));
                        enumToStringLookup.Add((i, str));

                        i++;
                    }

                    self.NativeProperties["enumToStringLookup"] = enumToStringLookup;
                    return self;
                },
                expectedArgs: new() { "self", "valueList" }
            )));

            @class.InstanceBaseTable.Add(("getName", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SClassInstance self) throw new Exception("unexpected error!");
                    if (args[1] is not SInt val) throw new Exception("Expected argument 0 to be an int");

                    if (self.NativeProperties["enumToStringLookup"] is not List<(int, SString)> enumToStringLookup) throw new Exception("unexpected error!");

                    foreach(var ent in enumToStringLookup) {
                        if (val.Value == ent.Item1) return ent.Item2;
                    }

                    return new SString("Unknown");
                },
                expectedArgs: new() { "self", "val" },
                isClassInstanceFunc: true
            )));

            return @class;
        }
    }
}
