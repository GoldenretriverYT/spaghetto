using spaghetto.Helpers;

namespace spaghetto.Stdlib.IO {
    public class EnumVal {
        public static SClass CreateClass() {
            var @class = new SClass("EnumVal");

            @class.InstanceBaseTable.AddNativeFunc<SClassInstance, SList>(NativeFuncGenType.Instance, "$$ctor", (scope, self, entries) => {
                var enumToStringLookup = new List<(int, SString)>();

                var i = 0;

                foreach (var v in entries.Value) {
                    if (v is not SString str) throw new Exception("Expected argument 0s list to only be strings");

                    self.InstanceTable.Add((str.Value, new SInt(i)));
                    enumToStringLookup.Add((i, str));

                    i++;
                }

                self.NativeProperties["enumToStringLookup"] = enumToStringLookup;
                return self;
            }, "self", "valueList");

            @class.InstanceBaseTable.AddNativeFunc<SClassInstance, SInt>(NativeFuncGenType.Instance, "getName", (scope, self, val) => {
                if (self.NativeProperties["enumToStringLookup"] is not List<(int, SString)> enumToStringLookup) throw new Exception("unexpected error!");

                foreach (var ent in enumToStringLookup) {
                    if (val.Value == ent.Item1) return ent.Item2;
                }

                return new SString("Unknown");
            }, "self", "val");

            return @class;
        }
    }
}
