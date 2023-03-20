namespace spaghetto {
    public class SNull : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Null;

        public override bool IsNull() {
            return true;
        }

        public override bool IsTruthy() {
            return false;
        }

        public override SString ToSpagString() {
            return new("null");
        }

        public override string ToString() {
            return "<SNull>";
        }
    }
}
