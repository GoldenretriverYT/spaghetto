using spaghetto.BuiltinTypes;
using static System.Formats.Asn1.AsnWriter;

namespace spaghetto {
    public class SClassInstance : SValue {
        public SClass Class { get; set; }
        public List<(string key, SValue val)> InstanceTable { get; set; } = new();

        /// <summary>
        /// These are native properties intended to allow native functions/classes to store/read information faster than using InstanceTable.
        /// However, these can not be directly accessed through the Dot operator.
        /// </summary>
        public Dictionary<object, object> NativeProperties { get; set; } = new();

        public override SBuiltinType BuiltinName => SBuiltinType.ClassInstance;

        public SClassInstance(SClass @class) {
            Class = @class;
            InstanceTable = Class.InstanceBaseTable.ToList();
        }

        public void CallConstructor(Scope scope, List<SValue> args) {
            var ctor = Dot(new SString("$$ctor"));

            if (ctor.IsNull()) Scope.Error("Class " + Class.Name + " does not have a constructor and can therefore not be instantiated.");

            var newArgs = new List<SValue> { this };
            newArgs.AddRange(args);

            ctor.Call(scope, args);

            if(scope.State != ScopeState.None) {
                scope.SetState(ScopeState.None);
                scope.SetReturnValue(SValue.Null);
            }
        }

        public override SString ToSpagString() {
            var toStringVal = GetValue("$$toString");

            if (toStringVal is not SBaseFunction toStringFunc) {
                return new SString("<instance of class " + Class.Name + ">");
            }else {
                // TODO: Find a solution to pass the scope; maybe keep a "DefiningScope" on each value?
                // For now, just use an empty scope
                //  SUBTODO: If this is done, dont forget to reset Scope.State!
                var ret = toStringFunc.Call(new Scope(0), new() { this });

                if (ret is not SString str) {
                    Scope.Error("A classes toString function must return a string!");
                    return new SString();
                }

                return str;
            }
        }

        public override string ToString() {
            return $"<SClassInstance ClassName={Class.Name}>";
        }

        public override SValue Dot(SValue other) {
            if (other is not SString key) return NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value);
            if (Class.FixedProps && val.IsNull()) return Scope.Error($"Property {other.ToSpagString().Value} not found!");
             
            return val;
        }


        public override SValue DotAssignment(SValue key, SValue other) {
            if (key is not SString keyVal) return NotSupportedBetween(key, "DotAssignment");

            // todo: get rid of the code duplication
            foreach (var kvp in Class.StaticTable) {
                if (kvp.key == keyVal.Value) {
                    if (kvp.val.IsConstant) return Scope.Error($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    InstanceTable.Add((keyVal.Value, other));
                    return other;
                }
            }

            foreach (var kvp in InstanceTable) {
                if (kvp.key == keyVal.Value) {
                    if (kvp.val.IsConstant) return Scope.Error($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    InstanceTable.Add((keyVal.Value, other));
                    return other;
                }
            }

            if (Class.FixedProps) return Scope.Error($"Property {key.ToSpagString().Value} not found!");
            InstanceTable.Add((keyVal.Value, other));
            return other;
        }

        public SValue GetValue(string key) {
            foreach (var kvp in Class.StaticTable) {
                if (kvp.key == key) return kvp.val;
            }

            foreach (var kvp in InstanceTable) {
                if (kvp.key == key) return kvp.val;
            }

            return SValue.Null;
        }

        public override bool IsTruthy() {
            return true;
        }

        public override SValue Add(SValue other)
        {
            var overload = GetValue("$$op+");
            if (overload == null) base.Add(other);

            var ret = overload.Call(new Scope(0), new List<SValue>() { this, this, other }); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override SValue Sub(SValue other)
        {
            var overload = GetValue("$$op-");
            if (overload == null) base.Sub(other);

            var ret = overload.Call(new Scope(0), new List<SValue>() { this, this, other }); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override SValue Mul(SValue other)
        {
            var overload = GetValue("$$op*");
            if (overload == null) base.Mul(other);

            var ret = overload.Call(new Scope(0), new List<SValue>() { this, this, other }); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override SValue Div(SValue other)
        {
            var overload = GetValue("$$op/");
            if (overload == null) base.Div(other);

            var ret = overload.Call(new Scope(0), new List<SValue>() { this, this, other }); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }
    }
}
