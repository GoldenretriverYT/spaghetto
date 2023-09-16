namespace spaghetto {
    public class SObject : SValue {
        public Dictionary<string, SValue> Value { get; set; } = new();
        public override SBuiltinType BuiltinName => SBuiltinType.Object;

        public SObject() { }

        public override SString ToSpagString() {
            return new SString("SObject {\n" + string.Join(",\n  ", Value.Select((v) => v.Key + ": " + v.Value.ToSpagString().Value)) + "\n}");
        }

        public override string ToString() {
            return $"<SObject Value={string.Join(", ", Value)}>";
        }

        public override SValue Idx(SValue other) {
            if(other is not SString key) {
                throw new Exception("Objects can only be indexed with strings");
            }

            if (!Value.TryGetValue(key.Value, out var retVal)) return SValue.Null;
            return retVal;
        }

        public override SValue Dot(SValue other) {
            if(other is not SString key) {
                throw new Exception("Internal error: Dot was called with non-string value, this should normally never happen");
            }

            if (!Value.TryGetValue(key.Value, out var retVal)) return SValue.Null;
            return retVal;
        }

        /* SObjects are currently only used by aliased imports, and instead of implementing costly IsConstant checks, we should disable assignments for SObjects. SDictionary can be used for that instead.
        public override SValue DotAssignment(SValue keySValue, SValue value) {
            if (keySValue is not SString key) {
                throw new Exception("Internal error: Dot was called with non-string value, this should normally never happen");
            }

            Value[key.Value] = value;
            return value;
        }*/

        public override bool IsTruthy() {
            return Value != null;
        }
    }
}
