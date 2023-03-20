namespace spaghetto {
    public class SList : SValue {
        public List<SValue> Value { get; set; } = new();
        public override SBuiltinType BuiltinName => SBuiltinType.List;

        public SList() { }

        public override SString ToSpagString() {
            return new SString("[" + string.Join(", ", Value.Select((v) => v.ToSpagString().Value)) + "]");
        }

        public override string ToString() {
            return $"<SString Value={string.Join(", ", Value)}>";
        }

        public override SValue Idx(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can only index SList with integers, got " + other.BuiltinName.ToString());

            if (otherInt.Value < 0 || otherInt.Value > Value.Count - 1) throw new Exception("Out of bounds access. SList had " + Value.Count + " elements, but index " + otherInt.Value + " was accessed");
            return Value[otherInt.Value];
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }
}
