namespace spaghetto.Stdlib.IO {
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter__io", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("println", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                if (args.Count == 0) throw new Exception("Expected 1 argument on println call");
                Console.WriteLine(args[0].ToSpagString().Value);
                return args[0];
            }));

            scope.Set("print", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                if (args.Count == 0) throw new Exception("Expected 1 argument on print call");
                Console.WriteLine(args[0].ToSpagString().Value);
                return args[0];
            }));
        }
    }
}