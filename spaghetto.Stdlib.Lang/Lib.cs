namespace spaghetto.Stdlib.Lang {
    /// <summary>
    /// Provides basic language functions & types
    /// </summary>
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter__lang", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("typeof", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                if (args.Count == 0) throw new Exception("Expected 1 argument on typeof call");

                return new SString(args[0].BuiltinName.ToString());
            }));

            scope.Set("toString", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                if (args.Count == 0) throw new Exception("Expected 1 argument on typeof toString");

                return args[0].ToSpagString();
            }));
        }
    }
}