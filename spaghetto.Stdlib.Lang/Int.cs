namespace spaghetto.Stdlib.Lang {
    public class Int {
        private readonly static Random rng = new();

        public static SClass CreateClass() {
            var @class = new SClass("int");

            @class.StaticTable.Add(("parse", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (!int.TryParse(str.Value, out int valInt)) throw new Exception("Invalid number!");

                    return new SInt(valInt);
                },
                expectedArgs: new() { "toParse" }
            )));

            @class.StaticTable.Add(("rand", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SInt min) throw new Exception("Expected argument 0 to be a int");
                    if (args[1] is not SInt max) throw new Exception("Expected argument 1 to be a int");

                    
                    return new SInt(rng.Next(min.Value, max.Value));
                },
                expectedArgs: new() { "min", "max" }
            )));

            @class.StaticTable.Add(("MIN_VALUE", new SInt(int.MinValue) { IsConstant = true }));
            @class.StaticTable.Add(("MAX_VALUE", new SInt(int.MaxValue) { IsConstant = true }));

            return @class;
        }
    }
}
