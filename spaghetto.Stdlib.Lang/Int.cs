namespace spaghetto.Stdlib.Lang {
    public class Int {
        public static SClass CreateClass() {
            var @class = new SClass("int");

            @class.StaticTable.Add((new SString("parse"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (!int.TryParse(str.Value, out int valInt)) throw new Exception("Invalid number!");

                    return new SInt(valInt);
                },
                expectedArgs: new() { "toParse" }
            )));

            @class.StaticTable.Add((new SString("MIN_VALUE"), new SInt(int.MinValue) { IsConstant = true }));
            @class.StaticTable.Add((new SString("MAX_VALUE"), new SInt(int.MaxValue) { IsConstant = true }));

            return @class;
        }
    }
}
