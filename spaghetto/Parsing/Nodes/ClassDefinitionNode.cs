namespace spaghetto.Parsing.Nodes
{
    internal class ClassDefinitionNode : SyntaxNode
    {
        private SyntaxToken className;
        private IEnumerable<SyntaxNode> body;

        public ClassDefinitionNode(SyntaxToken className, IEnumerable<SyntaxNode> body)
        {
            this.className = className;
            this.body = body;
        }

        public override NodeType Type => NodeType.ClassDefinition;

        public override SValue Evaluate(Scope scope)
        {
            var @class = new SClass();
            @class.Name = className.Text;

            foreach (var bodyNode in body)
            {
                if (bodyNode is not ClassFunctionDefinitionNode cfdn) throw new Exception("Unexpected node in class definition");

                var funcRaw = cfdn.Evaluate(scope);

                if (funcRaw is not SFunction func) throw new Exception("Expected ClassFunctionDefinitionNode to return SFunction");

                if (func.IsClassInstanceMethod)
                {
                    @class.InstanceBaseTable.Add((new SString(func.FunctionName), func));
                }
                else
                {
                    @class.StaticTable.Add((new SString(func.FunctionName), func));
                }
            }

            scope.Set(className.Text, @class);
            return @class;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(className);
            foreach (var n in body) yield return n;
        }
    }
}
