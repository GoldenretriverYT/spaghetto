namespace spaghetto {
    // TODO: Allow creation if Dictionaries
    public class SClass : SValue {
        public List<(string key, SValue val)> StaticTable { get; set; } = new();
        public List<(string key, SValue val)> InstanceBaseTable { get; set; } = new();

        public string Name { get; set; } = "";
        public bool FixedProps { get; set; } = false;
        public override SBuiltinType BuiltinName => SBuiltinType.Class;

        public SClass() {
        }

        public SClass(string name) {
            Name = name;
        }

        public override SString ToSpagString() {
            return new SString("<class " + Name + ">");
        }

        public override string ToString() {
            return $"<SClass Name={Name}>";
        }

        public override SValue Dot(SValue other) {
            if (other is not SString key) throw NotSupportedBetween(other, "Dot");

            foreach(var kvp in StaticTable) {
                if (kvp.key == key.Value) return kvp.val;
            }

            return SValue.Null;
        }

        public override bool IsTruthy() {
            return StaticTable != null;
        }
    }
}
