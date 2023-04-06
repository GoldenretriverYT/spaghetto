namespace spaghetto {
    public class SLong : SValue
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Long;
        public long Value { get; set; }

        public static SLong Zero => new SLong(0);
        public static SLong One => new SLong(1);

        public SLong() {
            Value = 0;
        }

        public SLong(long value)
        {
            Value = value;
        }

        public override SValue Add(SValue other) {
            if (other is not SLong otherLong) throw new Exception("Can not perform Add on SLong and " + other.BuiltinName.ToString());

            return new SLong(Value + otherLong.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SLong otherLong) throw new Exception("Can not perform Sub on SLong and " + other.BuiltinName.ToString());
            return new SLong(Value - otherLong.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SLong otherLong) throw new Exception("Can not perform Mul SLong and " + other.BuiltinName.ToString());
            return new SLong(Value * otherLong.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SLong otherLong) throw new Exception("Can not perform Div SLong and " + other.BuiltinName.ToString());
            return new SLong(Value / otherLong.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SLong otherLong) throw new Exception("Can not perform Mod SLong and " + other.BuiltinName.ToString());
            return new SLong(Value % otherLong.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SLong otherLong) return SInt.Zero;
            return Value == otherLong.Value ? SInt.One : SInt.Zero;
        }

        public override SValue LessThan(SValue other) {
            if (other is not SLong otherLong) return SInt.Zero;
            return Value < otherLong.Value ? SInt.One : SInt.Zero;
        }

        public override SValue LessThanEqu(SValue other) {
            if (other is not SLong otherLong) return SInt.Zero;
            return Value <= otherLong.Value ? SInt.One : SInt.Zero;
        }

        public override SValue GreaterThan(SValue other) {
            if (other is not SLong otherLong) return SInt.Zero;
            return Value > otherLong.Value ? SInt.One : SInt.Zero;
        }

        public override SValue GreaterThanEqu(SValue other) {
            if (other is not SLong otherLong) return SInt.Zero;
            return Value >= otherLong.Value ? SInt.One : SInt.Zero;
        }

        public override SValue ArithNot() {
            return new SLong(-Value);
        }

        public override SValue CastToBuiltin(SBuiltinType other) {
            switch(other) {
                case SBuiltinType.Int:
                    return new SInt((int)Value);
                case SBuiltinType.Long:
                    return new SLong(Value);
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
