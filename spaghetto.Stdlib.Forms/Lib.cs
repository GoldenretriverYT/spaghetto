namespace spaghetto.Stdlib.Interop {
    public class Lib : ExternLibMain {
        public override void Mount(Scope scope) {
            scope.Set("nlimporter$$forms", new SNativeLibraryImporter(Import));
        }

        public static void Import(Scope scope) {
            scope.Set("Form", Form.CreateClass());
        }
    }
}