namespace spaghetto {
    public class SString : SValue
    {
        public string Value { get; set; }
        public override SBuiltinType BuiltinName => SBuiltinType.String;

        public SString() { }
        public SString(string value)
        {
            Value = value;
        }

        public override SString ToSpagString()
        {
            return new SString(Value);
        }

        public override string ToString()
        {
            return $"<SString Value={Value}>";
        }

        public override SValue Add(SValue other)
        {
            if (other is not SString @string) throw new NotImplementedException();
            return new SString(Value + @string.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SString otherString) return SInt.Zero;
            return Value == otherString.Value ? SInt.One : SInt.Zero;
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }
}
