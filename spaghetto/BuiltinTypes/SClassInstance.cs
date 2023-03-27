using spaghetto.BuiltinTypes;

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

            if (ctor.IsNull()) throw new Exception("Class " + Class.Name + " does not have a constructor and can therefore not be instantiated.");

            var newArgs = new List<SValue> { this };
            newArgs.AddRange(args);

            ctor.Call(scope, args);
        }

        public override SString ToSpagString() {
            var toStringVal = GetValue(new SString("$$toString"));

            if (toStringVal is not SBaseFunction toStringFunc) {
                return new SString("<instance of class " + Class.Name + ">");
            }else {
                // TODO: Find a solution to pass the scope; maybe keep a "DefiningScope" on each value?
                // For now, just use an empty scope
                var ret = toStringFunc.Call(new Scope(0), new() { this });

                if (ret is not SString str) throw new Exception("A classes toString function must return a string!");
                return str;
            }
        }

        public override string ToString() {
            return $"<SClassInstance ClassName={Class.Name}>";
        }

        public override SValue Dot(SValue other) {
            var val = GetValue(other);
            if (Class.FixedProps && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not found!");
             
            return val;
        }


        public override SValue DotAssignment(SValue key, SValue other) {
            // todo: get rid of the code duplication
            foreach (var kvp in Class.StaticTable) {
                if (kvp.key.Equals(key).IsTruthy()) {
                    if (kvp.val.IsConstant) throw new Exception($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    InstanceTable.Add((key, other));
                    return other;
                }
            }

            foreach (var kvp in InstanceTable) {
                if (kvp.key.Equals(key).IsTruthy()) {
                    if (kvp.val.IsConstant) throw new Exception($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    InstanceTable.Add((key, other));
                    return other;
                }
            }

            if (Class.FixedProps) throw new Exception($"Property {key.ToSpagString().Value} not found!");
            InstanceTable.Add((key, other));
            return other;
        }

        public SValue GetValue(SValue key) {
            foreach (var kvp in Class.StaticTable) {
                if (kvp.key.Equals(key).IsTruthy()) return kvp.val;
            }

            foreach (var kvp in InstanceTable) {
                if (kvp.key.Equals(key).IsTruthy()) return kvp.val;
            }

            return SValue.Null;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
