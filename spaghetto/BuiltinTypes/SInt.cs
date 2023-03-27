namespace spaghetto {
    public class SInt : SValue
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Int;
        public int Value { get; set; }

        public static SInt Zero => new SInt(0);
        public static SInt One => new SInt(1);

        public SInt() {
            Value = 0;
        }

        public SInt(int value)
        {
            Value = value;
        }

        public override SValue Add(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Add on SInt and " + other.BuiltinName.ToString());

            return new SInt(Value + otherInt.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Sub on SInt and " + other.BuiltinName.ToString());
            return new SInt(Value - otherInt.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mul on SInt and " + other.BuiltinName.ToString());
            return new SInt(Value * otherInt.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Div on SInt and " + other.BuiltinName.ToString());
            return new SInt(Value / otherInt.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mod on SInt and " + other.BuiltinName.ToString());
            return new SInt(Value % otherInt.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SInt otherInt) return SInt.Zero;
            return Value == otherInt.Value ? SInt.One : SInt.Zero;
        }

        public override SValue LessThan(SValue other) {
            if (other is not SInt otherInt) return SInt.Zero;
            return Value < otherInt.Value ? SInt.One : SInt.Zero;
        }

        public override SValue LessThanEqu(SValue other) {
            if (other is not SInt otherInt) return SInt.Zero;
            return Value <= otherInt.Value ? SInt.One : SInt.Zero;
        }

        public override SValue GreaterThan(SValue other) {
            if (other is not SInt otherInt) return SInt.Zero;
            return Value > otherInt.Value ? SInt.One : SInt.Zero;
        }

        public override SValue GreaterThanEqu(SValue other) {
            if (other is not SInt otherInt) return SInt.Zero;
            return Value >= otherInt.Value ? SInt.One : SInt.Zero;
        }

        public override SValue ArithNot() {
            return new SInt(-Value);
        }

        public override SValue CastToBuiltin(SBuiltinType other) {
            switch(other) {
                case SBuiltinType.Int:
                    return new SInt(Value);
                case SBuiltinType.Float:
                    return new SFloat(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy() {
            return Value == 1;
        }

        public override SValue Not()
        {
            return (!IsTruthy() ? One : Zero);
        }

        public override string ToString() {
            return $"<{BuiltinName.ToString()} value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString(Value.ToString());
        }
    }
}
