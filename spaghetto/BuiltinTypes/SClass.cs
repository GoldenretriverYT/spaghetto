namespace spaghetto {
    // TODO: Allow creation if Dictionaries
    public class SClass : SValue {
        public List<(SValue key, SValue val)> StaticTable { get; set; } = new();
        public List<(SValue key, SValue val)> InstanceTable { get; set; } = new();

        public string Name { get; set; } = "";
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
            foreach(var kvp in StaticTable) {
                if (kvp.key.Equals(other).IsTruthy()) return kvp.val;
            }

            return SValue.Null;
        }

        public override bool IsTruthy() {
            return StaticTable != null;
        }
    }
}
