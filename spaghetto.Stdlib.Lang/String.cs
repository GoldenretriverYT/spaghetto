using spaghetto.Helpers;

namespace spaghetto.Stdlib.Lang {
    public class String {
        public static SClass CreateClass() {
            var @class = new SClass("string");

            @class.StaticTable.AddNativeFunc<SString>("getChars", (scope, str) => {
                return new SList() {
                    Value = str.Value.ToCharArray().Select((v) => new SString(v.ToString())).ToList<SValue>()
                };
            }, "string");

            @class.StaticTable.AddNativeFunc<SString, SString>("split", (scope, str, splitter) => {
                return new SList() {
                    Value = str.Value.Split(splitter.Value).Select((v) => new SString(v.ToString())).ToList<SValue>()
                };
            }, "string", "splitter");

            @class.StaticTable.AddNativeFunc<SString, SString, SString>("replace", (scope, str, match, replacer) => {
                return new SString(str.Value.Replace(match.Value, replacer.Value));
            }, "string", "match", "replacer");

            @class.StaticTable.AddNativeFunc<SString>("length", (scope, str) => {
                return new SInt(str.Value.Length);
            }, "length");

            @class.StaticTable.AddNativeFunc<SString, SInt, SInt>("substring", (scope, str, offset, length) => {
                return new SString(str.Value.Substring(offset.Value, length.Value));
            }, "string", "start", "length");

            @class.StaticTable.AddNativeFunc<SString, SInt>("takeafter", (scope, str, offset) => {
                return new SString(str.Value.Substring(offset.Value));
            }, "string", "start");

            @class.StaticTable.Add(("EMPTY", new SString(string.Empty) { IsConstant = true }));

            return @class;
        }
    }
}
