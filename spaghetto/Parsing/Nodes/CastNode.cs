namespace spaghetto.Parsing.Nodes
{
    internal class CastNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode node;
        public SBuiltinType type;

        public CastNode(SyntaxToken ident, SyntaxNode node) : base(ident.Position, node.EndPosition)
        {
            this.ident = ident;
            this.node = node;


            // Int,
            // Float,
            // List,
            // Null,
            // NativeFunc,
            // Function,
            // NativeLibraryImporter,
            // Dictionary,
            // Class,
            // NativeObject,
            // ClassInstance,
            // Long,

            switch(ident.Text.ToLower()) {
                case "string":
                    type = SBuiltinType.String;
                    break;
                case "int":
                    type = SBuiltinType.Int;
                    break;
                case "list":
                    type = SBuiltinType.List;
                    break;
                case "null":
                    type = SBuiltinType.Null;
                    break;
                case "nativefunc":
                    type = SBuiltinType.NativeFunc;
                    break;
                case "function":
                    type = SBuiltinType.Function;
                    break;
                case "nativelibraryimporter":
                    type = SBuiltinType.NativeLibraryImporter;
                    break;
                case "dictionary":
                    type = SBuiltinType.Dictionary;
                    break;
                case "class":
                    type = SBuiltinType.Class;
                    break;
                case "nativeobject":
                    type = SBuiltinType.NativeObject;
                    break;
                case "classinstance":
                    type = SBuiltinType.ClassInstance;
                    break;
                case "long":
                    type = SBuiltinType.Long;
                    break;
                default:
                    throw new Exception("Unknown type " + ident.Text + "; only builtin types supported right now.");
            }
        }

        public override NodeType Type => NodeType.Cast;

        public override SValue Evaluate(Scope scope)
        {
            return node.Evaluate(scope).CastToBuiltin(type);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return node;
        }
    }
}
