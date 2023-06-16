namespace spaghetto {
    public class SError : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Error;

        public override bool IsNull() {
            return true;
        }

        public override bool IsTruthy() {
            return false;
        }

        public override SString ToSpagString() {
            return new("error");
        }

        public override string ToString() {
            return "<SError>";
        }
    }
}
