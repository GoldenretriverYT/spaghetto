﻿using spaghetto.Helpers;
using spaghetto.Stdlib.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Schema;

namespace spaghetto.Stdlib.Lang {
    /// <summary>
    /// Provides basic language functions & types
    /// </summary>
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter$$lang", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("typeof", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    var builtinType = args[0].BuiltinName;

                    if(builtinType == SBuiltinType.ClassInstance) {
                        if (args[0] is not SClassInstance inst) throw new Exception("Unexpected value! BuiltinName was set to ClassInstance but it was not of type SClassInstance!");
                        return new SString(inst.Class.Name);
                    }else {
                        return new SString(builtinType.ToString());
                    }
                    
                },
                expectedArgs: new() { "value" }
            ));

            scope.Set("toString", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    return args[0].ToSpagString();
                },
                expectedArgs: new() { "value" }
            ));

            scope.Set("time", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    return new SLong(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                },
                expectedArgs: new() { }
            ));

            scope.Set("unsafe__scopedelete", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    WarnHelpers.PrintOnce("Warning: Using unsafe function 'unsafe__scopedeleted' is not recommended unless you know what you are doing. Usage can produce unexpected behaviour.");
                    if (args[0] is not SString ident) throw new Exception("Expected argument 0 to be of type string");

                    if(callingScope.Get(ident.Value) != null) {
                        callingScope.Table.Remove(ident.Value);
                        return SInt.One;
                    }

                    return SInt.Zero;
                },
                expectedArgs: new() { "identifierName" }
            ));

            scope.Set("eval", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    if (args[0] is not SString code) throw new Exception("Expected argument 0 to be of type string");

                    Interpreter ip = new();
                    Scope rootScope = callingScope.GetRoot();

                    // copy available namespaces provided by runtime
                    foreach(var kvp in rootScope.Table) {
                        if(kvp.Key.StartsWith("nlimporter$$")) {
                            ip.GlobalScope.Table[kvp.Key] = kvp.Value;
                        }
                    }

                    InterpreterResult res = new();
                    ip.Interpret(code.Value, ref res);

                    return res.LastValue;
                },
                expectedArgs: new() { "code" }
            ));

            scope.Set("int", Int.CreateClass());
            scope.Set("float", Float.CreateClass());
            scope.Set("string", String.CreateClass());
            scope.Set("list", List.CreateClass());
            scope.Set("EnumVal", EnumVal.CreateClass());

        }
    }
}