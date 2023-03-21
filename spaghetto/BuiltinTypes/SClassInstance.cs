namespace spaghetto {
    public class SClassInstance : SValue {
        public SClass Class { get; set; }
        public List<(SValue key, SValue val)> InstanceTable { get; set; } = new();

        public override SBuiltinType BuiltinName => SBuiltinType.ClassInstance;

        public SClassInstance(SClass @class) {
            Class = @class;
            InstanceTable = Class.InstanceBaseTable.ToList();
        }

        public void CallConstructor(Scope scope, List<SValue> args) {
            var ctor = Dot(new SString("$$ctor"));

            if (ctor == null) throw new Exception("Class " + Class.Name + " does not have a constructor and can therefore not be instantiated.");

            var newArgs = new List<SValue> { this };
            newArgs.AddRange(args);

            ctor.Call(scope, args);
        }

        public override SString ToSpagString() {
            return new SString("<instance of class " + Class.Name + ">");
        }

        public override string ToString() {
            return $"<SClassInstance ClassName={Class.Name}>";
        }

        public override SValue Dot(SValue other) {
            foreach (var kvp in Class.StaticTable) {
                if (kvp.key.Equals(other).IsTruthy()) return kvp.val;
            }

            foreach (var kvp in InstanceTable) {
                if (kvp.key.Equals(other).IsTruthy()) return kvp.val;
            }



            return SValue.Null;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
