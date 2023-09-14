﻿namespace spaghetto.Stdlib.Lang {
    public class String {
        public static SClass CreateClass() {
            var @class = new SClass("string");

            @class.StaticTable.Add(("getChars", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");

                    return new SList() {
                        Value = str.Value.ToCharArray().Select((v) => new SString(v.ToString())).ToList<SValue>()
                    };
                },
                expectedArgs: new() { "string" }
            )));

            @class.StaticTable.Add(("split", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (args[1] is not SString splitter) throw new Exception("Expected argument 1 to be a string");

                    return new SList() {
                        Value = str.Value.Split(splitter.Value).Select((v) => new SString(v.ToString())).ToList<SValue>()
                    };
                },
                expectedArgs: new() { "string", "char" }
            )));

            @class.StaticTable.Add(("replace", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (args[1] is not SString match) throw new Exception("Expected argument 1 to be a string");
                    if (args[2] is not SString replacer) throw new Exception("Expected argument 2 to be a string");

                    return new SString(str.Value.Replace(match.Value, replacer.Value));
                },
                expectedArgs: new() { "string", "match", "replacer" }
            )));

            @class.StaticTable.Add(("length", new SNativeFunction(
               impl: (Scope scope, List<SValue> args) => {
                   if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");

                   return new SInt(str.Value.Length);
               },
               expectedArgs: new() { "string" }
            )));

            @class.StaticTable.Add(("substring", new SNativeFunction(
               impl: (Scope scope, List<SValue> args) => {
                   if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                   if (args[1] is not SInt offset) throw new Exception("Expected argument 1 to be an int");
                   if (args[2] is not SInt length) throw new Exception("Expected argument 2 to be an int");

                   return new SString(str.Value.Substring(offset.Value, length.Value));
               },
               expectedArgs: new() { "string", "start", "length" }
            )));

            @class.StaticTable.Add(("takeafter", new SNativeFunction(
                impl: (Scope scope, List<SValue> args) => {
                    if (args[0] is not SString str) throw new Exception("Expected argument 0 to be a string");
                    if (args[1] is not SInt offset) throw new Exception("Expected argument 1 to be an int");

                    return new SString(str.Value.Substring(offset.Value));
                },
                expectedArgs: new() { "string", "start" }
            )));

            @class.StaticTable.Add(("EMPTY", new SString(string.Empty) { IsConstant = true }));

            return @class;
        }
    }
}
