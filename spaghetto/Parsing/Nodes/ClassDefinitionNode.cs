using spaghetto.Helpers;
using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class ClassDefinitionNode : SyntaxNode
    {
        private SyntaxToken className;
        private IEnumerable<SyntaxNode> body;
        private readonly bool fixedProps;

        public ClassDefinitionNode(SyntaxToken className, IEnumerable<SyntaxNode> body, bool fixedProps) : base(className.Position, body.GetEndingPosition(className.EndPosition))
        {
            this.className = className;
            this.body = body;
            this.fixedProps = fixedProps;
        }

        public override NodeType Type => NodeType.ClassDefinition;

        public override SValue Evaluate(Scope scope)
        {
            var @class = new SClass();
            @class.Name = className.Text;
            @class.FixedProps = fixedProps;

            foreach (var bodyNode in body)
            {
                if (bodyNode is ClassFunctionDefinitionNode cfdn) {
                    var funcRaw = cfdn.Evaluate(scope);
                    if (funcRaw is not SFunction func) throw new Exception("Expected ClassFunctionDefinitionNode to return SFunction");

                    if (func.IsClassInstanceMethod) {
                        if (func.ExpectedArgs.IndexOf("self") == -1) func.ExpectedArgs.Insert(0, "self");

                        @class.InstanceBaseTable.Add((func.FunctionName, func));
                    } else {
                        @class.StaticTable.Add((func.FunctionName, func));
                    }
                } else if (bodyNode is ClassPropDefinitionNode cpdn) {
                    var val = cpdn.Expression.Evaluate(scope);

                    if(!cpdn.IsStatic) {
                        @class.InstanceBaseTable.Add((cpdn.Name.Text, val));
                    }else {
                        @class.StaticTable.Add((cpdn.Name.Text, val));
                    }
                } else {
                    throw new Exception("Unexpected node in class definition");
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

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder();

            sb.Append("class ");
            if (fixedProps) sb.Append("fixedprops ");

            sb.Append(className.Text);

            sb.Append("{");

            foreach (var n in body) sb.Append(n.GenerateSource(depth + 1));

            return sb.Append("}").ToString();
        }
    }
}
