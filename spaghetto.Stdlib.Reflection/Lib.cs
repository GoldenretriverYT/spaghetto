/// <summary>
/// Reflection library provides functions to dynamically interact with SClasses and possibly other types.
/// The name might be wrong and could be changed in the future.
/// </summary>
namespace spaghetto.Stdlib.Reflection {
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter$$reflection", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
        }
    }
}