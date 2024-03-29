﻿namespace spaghetto {
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
            if (other is not SString @string) throw NotSupportedBetween(other, "Add");
            return new SString(Value + @string.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SInt @int) throw NotSupportedBetween(other, "Mul");
            return new SString(string.Concat(Enumerable.Repeat(Value, @int.Value)));
        }

        public override SValue Idx(SValue other)
        {
            if (other is not SInt idx) throw NotSupportedBetween(other, "Add");
            return new SString(Value[idx.Value].ToString());
        }

        public override SValue Equals(SValue other) {
            if (other is not SString otherString) return SInt.Zero;
            return Value == otherString.Value ? SInt.One : SInt.Zero;
        }

        public bool SurelyStringEquals(string other)
        {
            return Value == other;
        }

        public override bool IsTruthy() {
            return Value != null;
        }
    }
}
