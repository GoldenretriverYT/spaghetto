﻿namespace spaghetto.Parsing.Nodes
{
    internal class ExportNode : SyntaxNode
    {
        private SyntaxToken ident;

        public ExportNode(SyntaxToken ident) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
        }

        public override NodeType Type => NodeType.Export;

        public override SValue Evaluate(Scope scope)
        {
            var val = scope.Get(ident.Text);
            if (val == null) throw new Exception("Can not export value of non-existent identifier");

            scope.GetRoot().ExportTable.Add(ident.Text, val);
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }

        public override string GenerateSource(int depth) {
            return "export " + ident.Text + ";";
        }
    }
}
