namespace spaghetto.Parsing.Nodes
{
    internal class NativeImportNode : SyntaxNode
    {
        private SyntaxToken ident;

        public NativeImportNode(SyntaxToken ident)
        {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override SValue Evaluate(Scope scope)
        {
            var val = scope.Get("nlimporter$$" + ident.Text);

            if (val == null || val is not SNativeLibraryImporter importer)
            {
                throw new Exception("Native library " + ident.Text + " not found!");
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
