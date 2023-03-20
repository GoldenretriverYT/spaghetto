namespace spaghetto {
    // TODO: Allow creation if Dictionaries
    public class SDictionary : SValue {
        public List<(SValue key, SValue val)> Value { get; set; } = new();
        public override SBuiltinType BuiltinName => SBuiltinType.Dictionary;

        public SDictionary() { }

        public override SString ToSpagString() {
            return new SString("{\n" + string.Join(",\n  ", Value.Select((v) => v.key.ToSpagString().Value + ": " + v.val.ToSpagString().Value)) + "\n}");
        }

        public override string ToString() {
            return $"<SDictionary Value={string.Join(", ", Value)}>";
        }

        public override SValue Idx(SValue other) {
            foreach(var kvp in Value) {
                if (kvp.key.Equals(other).IsTruthy()) return kvp.val;
            }

            return SValue.Null;
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }
}
