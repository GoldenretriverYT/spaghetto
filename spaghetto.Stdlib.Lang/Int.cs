using spaghetto.Helpers;

namespace spaghetto.Stdlib.Lang {
    public class Int {
        private readonly static Random rng = new();

        public static SClass CreateClass() {
            var @class = new SClass("int");

            @class.StaticTable.AddNativeFunc<SString>(NativeFuncGenType.Static, "parse", (scope, str) => {
                if (!int.TryParse(str.Value, out int valInt)) throw new Exception("Invalid number!");

                return new SInt(valInt);
            }, "toParse");

            @class.StaticTable.AddNativeFunc<SInt, SInt>(NativeFuncGenType.Static, "rand", (scope, min, max) => {
                return new SInt(rng.Next(min.Value, max.Value));
            }, "min", "max");

            @class.StaticTable.Add(("MIN_VALUE", new SInt(int.MinValue) { IsConstant = true }));
            @class.StaticTable.Add(("MAX_VALUE", new SInt(int.MaxValue) { IsConstant = true }));

            return @class;
        }
    }
}
