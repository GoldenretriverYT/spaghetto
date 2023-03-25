namespace spaghetto.Parsing.Nodes
{
    internal class ImportNode : SyntaxNode
    {
        private SyntaxToken path;

        public ImportNode(SyntaxToken path) : base(path.Position, path.EndPosition)
        {
            this.path = path;
        }

        public override NodeType Type => NodeType.Import;

        public override SValue Evaluate(Scope scope)
        {
            if (!File.Exists(path.Text)) throw new Exception($"Failed to import '{path.Text}': File not found");
            var text = File.ReadAllText(path.Text);

            Interpreter ip = new();
            Scope rootScope = scope.GetRoot();

            // copy available namespaces provided by runtime
            foreach (var kvp in rootScope.Table)
            {
                if (kvp.Key.StartsWith("nlimporter$$"))
                {
                    ip.GlobalScope.Table[kvp.Key] = kvp.Value;
                }
            }

            InterpreterResult res = new();

            try
            {
                ip.Interpret(text, ref res);

                // copy export table

                foreach (var kvp in ip.GlobalScope.ExportTable)
                {
                    if (scope.Get(kvp.Key) != null) throw new Exception($"Failed to import '{path.Text}': Import conflict; file exports '{kvp.Key}' but that identifier is already present in the current scope.");

                    scope.Set(kvp.Key, kvp.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import '{path.Text}': {ex.Message}");
            }

            return res.LastValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}
