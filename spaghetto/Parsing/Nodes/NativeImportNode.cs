using System.Diagnostics;
using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class NativeImportNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxToken? alias;

        public NativeImportNode(SyntaxToken ident, SyntaxToken? alias = null) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
            this.alias = alias;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override SValue Evaluate(Scope scope)
        {
            if(ident.Text == "all") {
                var rootScope = scope.GetRoot();

                foreach(var kvp in rootScope.Table.ToList()) {
                    if(kvp.Key.StartsWith("nlimporter$$")) {
                        if (kvp.Value is not SNativeLibraryImporter importerFromAllLoop) throw new Exception("Found unexpexted type in root tables nlimporters!");
                        importerFromAllLoop.Import(scope);
                    }
                }

                return SValue.Null;
            }

            var val = scope.Get("nlimporter$$" + ident.Text);

            if (val == null || val is not SNativeLibraryImporter importer)
            {
                throw new Exception("Native library " + ident.Text + " not found!");
            }

            // To import with an alias, we first import it into a new empty scope,
            // of which we then copy all elements into a new holder object that
            // is then added to the scope.

            if (alias.HasValue) {
                var newScope = new Scope(0);
                importer.Import(newScope);

                var holder = new SObject();
                foreach (var kvp in newScope.Table) {
                    holder.Value[kvp.Key] = kvp.Value;
                    holder.Value[kvp.Key].IsConstant = true;
                }

                scope.Set((string)alias.Value.Value, holder);
                return SValue.Null;
            }

            importer.Import(scope);
            return SValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }

        override public string ToString() {
            return "NativeImportNode:";
        }

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder("import native ");

            sb.Append(ident.Text);

            if(alias.HasValue) {
                sb.Append(" as ");
                sb.Append(alias.Value.Text);
            }

            return sb.Append(";").ToString();
        }
    }
}
