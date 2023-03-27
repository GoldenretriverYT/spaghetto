namespace spaghetto.Stdlib.IO {
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter$$io", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("println", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    Console.WriteLine(args[0].ToSpagString().Value);
                    return args[0];
                },
                expectedArgs: new() { "text" }
            ));

            scope.Set("print", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    Console.Write(args[0].ToSpagString().Value);
                    return args[0];
                },
                expectedArgs: new() { "text" }
            ));

            scope.Set("read", new SNativeFunction(
                impl: (Scope callingScope, List<SValue> args) => {
                    return new SString(Console.ReadKey().KeyChar.ToString());
                }
            ));

            scope.Set("readline", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                return new SString(Console.ReadLine());
            }));

            scope.Set("File", File.CreateClass());
        }
    }
}