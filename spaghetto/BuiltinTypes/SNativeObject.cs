namespace spaghetto {
    /// <summary>
    /// A wrapper class for native C# objects that shouldn't become their own type.
    /// </summary>
    public class SNativeObject : SValue
    {
        public override SBuiltinType BuiltinName => SBuiltinType.NativeObject;
        public object Value { get; set; }

        public SNativeObject() {
            Value = 0;
        }

        public SNativeObject(object value) {
            Value = value;
        }

        public override bool IsTruthy() {
            return Value != null;
        }

        public override string ToString() {
            return $"<NativeObject<T={Value.GetType().Name}> value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString("<native object>");
        }
    }
}
