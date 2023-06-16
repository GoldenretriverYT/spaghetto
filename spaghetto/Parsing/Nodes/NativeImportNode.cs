namespace spaghetto.Parsing.Nodes
{
    internal class NativeImportNode : SyntaxNode
    {
        private SyntaxToken ident;

        public NativeImportNode(SyntaxToken ident) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override SValue Evaluate(Scope scope)
        {
            if(ident.Text == "all") {
                var rootScope = scope.GetRoot();

                foreach(var kvp in rootScope.Table.ToList()) {
                    if(kvp.Key.StartsWith("nlimporter$$")) {
                        if (kvp.Value is not SNativeLibraryImporter importerFromAllLoop) return Scope.Error("Found unexpexted type in root tables nlimporters!");
                        importerFromAllLoop.Import(scope);
                    }
                }

                return SValue.Null;
            }

            var val = scope.Get("nlimporter$$" + ident.Text);

            if (val == null || val is not SNativeLibraryImporter importer)
            {
                return Scope.Error("Native library " + ident.Text + " not found!");
            }

            importer.Import(scope);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }
    }
}
