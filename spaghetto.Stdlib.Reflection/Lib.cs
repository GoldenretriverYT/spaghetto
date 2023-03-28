namespace spaghetto.Stdlib.Interop {
    public class Lib {
        public static void Mount(Scope scope) {
            scope.Set("nlimporter$$interop", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("NETInterop", NETInterop.CreateClass());
        }
    }
}