using spaghetto.Helpers;

namespace spaghetto.Stdlib.Lang {
    public class List {
        public static SClass CreateClass() {
            var @class = new SClass("list");

            @class.StaticTable.AddNativeFunc<SList>(NativeFuncGenType.Static, "length", (scope, lst) => {
                return new SInt(lst.Value.Count);
            }, "list");

            @class.StaticTable.AddNativeFunc<SList, SValue>(NativeFuncGenType.Static, "contains", (scope, lst, val) => {
                foreach (var lVal in lst.Value) {
                    if (lVal.Equals(val).IsTruthy()) {
                        return SInt.One;
                    }
                }

                return SInt.Zero;
            }, "list", "val");

            return @class;
        }
    }
}
