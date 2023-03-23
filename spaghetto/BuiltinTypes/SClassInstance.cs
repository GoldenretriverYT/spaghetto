﻿using spaghetto.BuiltinTypes;

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
            var toStringVal = Dot(new SString("$$toString"));

            if (toStringVal is not SBaseFunction toStringFunc) {
                return new SString("<instance of class " + Class.Name + ">");
            }else {
                // TODO: Find a solution to pass the scope; maybe keep a "DefiningScope" on each value?
                var ret = toStringFunc.Call(null, new() { this });

                if (ret is not SString str) throw new Exception("A classes toString function must return a string!");
                return str;
            }
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

        public override SValue DotAssignment(SValue key, SValue other) {
            foreach (var kvp in InstanceTable) {
                if (kvp.key.Equals(key).IsTruthy()) {
                    if (kvp.val.IsConstant) throw new Exception($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    break;
                }
            }

            InstanceTable.Add((key, other));
            return other;
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
