namespace spaghetto {
    public enum SBuiltinType
    {
        String,
        Int,
        Float,
        List,
        Null,
        NativeFunc,
        Function,
        NativeLibraryImporter,
        Dictionary,
        Class,
        NativeObject,
        ClassInstance,
        Long,
    }

    public class SBuiltinTypeHelper {
        public static string ToStr(SBuiltinType type) {
            if (type == SBuiltinType.String) return "String";
            if (type == SBuiltinType.Int) return "Int";
            if (type == SBuiltinType.Float) return "Float";
            if (type == SBuiltinType.List) return "List";
            if (type == SBuiltinType.Null)return "Null";
            if (type == SBuiltinType.NativeFunc) return "NativeFunc";
            if (type == SBuiltinType.Function) return "Function";
            if (type == SBuiltinType.NativeLibraryImporter)return "NativeLibraryImporter";
            if (type == SBuiltinType.Dictionary) return "Dictionary";
            if (type == SBuiltinType.Class) return "Class";
            if (type == SBuiltinType.NativeObject) return "NativeObject";
            if (type == SBuiltinType.ClassInstance)return "ClassInstance";
            if (type == SBuiltinType.Long) return "Long";
            return "???";
        }
    }
}
