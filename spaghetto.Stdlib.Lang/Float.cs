using spaghetto.Helpers;
using System.Globalization;

namespace spaghetto.Stdlib.Lang {
    public class Float {
        public static SClass CreateClass() {
            var @class = new SClass("float");

            @class.StaticTable.AddNativeFunc<SString>(NativeFuncGenType.Static, "parse", (scope, str) => {
                if (!float.TryParse(str.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float valFloat)) throw new Exception("Invalid number!");

                return new SFloat(valFloat);
            }, "toParse");

            @class.StaticTable.Add(("MIN_VALUE", new SFloat(float.MinValue) { IsConstant = true }));
            @class.StaticTable.Add(("MAX_VALUE", new SFloat(float.MaxValue) { IsConstant = true }));

            return @class;
        }
    }
}
