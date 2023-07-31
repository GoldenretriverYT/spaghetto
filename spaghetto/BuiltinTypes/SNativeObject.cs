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
            return $"<NativeObject<T={Value?.GetType().Name ?? "Null"}> value={Value ?? "Null"}>";
        }

        public override SValue Dot(SValue other)
        {
            if (other is not SString ident) throw NotSupportedBetween(other, "Dot");

            var field = Value.GetType().GetField(ident.Value);
            if (field == null) return SNull.Null;

            return new SNativeObject(field.GetValue(Value));
        }

        public override SString ToSpagString() {
            return new SString("<native object>");
        }
    }

    /// <summary>
    /// A wrapper class for native C# pointers
    /// </summary>
    public unsafe class SNativePointer : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.NativePointer;
        public void* Value { get; set; }

        public SNativePointer() {
            Value = null;
        }

        public SNativePointer(void* value) {
            Value = value;
        }

        public override bool IsTruthy() {
            return Value != null;
        }

        public override string ToString() {
            if (Value == null) return "<NativePointer value=nullptr>";
            return $"<NativePointer value={new IntPtr(Value).ToString("X16")}>";
        }

        public override SString ToSpagString() {
            return new SString("<native object>");
        }
    }
}
