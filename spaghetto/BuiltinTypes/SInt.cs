namespace spaghetto {
    public class SInt : SValue
    {
        public override SBuiltinType BuiltinName => SBuiltinType.Int;
        public int Value { get; set; }

        public SInt() {
            Value = 0;
        }

        public SInt(int value)
        {
            Value = value;
        }

        public override SValue Add(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Add on SInt and " + other.GetType().Name);

            return new SInt(Value + otherInt.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Sub on SInt and " + other.GetType().Name);
            return new SInt(Value - otherInt.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mul on SInt and " + other.GetType().Name);
            return new SInt(Value * otherInt.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Div on SInt and " + other.GetType().Name);
            return new SInt(Value / otherInt.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform Mod on SInt and " + other.GetType().Name);
            return new SInt(Value % otherInt.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform EqualsCheck on SInt and " + other.GetType().Name);
            return new SInt(Value == otherInt.Value ? 1 : 0);
        }

        public override SValue LessThan(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform LessThanCheck on SInt and " + other.GetType().Name);
            return new SInt(Value < otherInt.Value ? 1 : 0);
        }

        public override SValue LessThanEqu(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform LessThanEquCheck on SInt and " + other.GetType().Name);
            return new SInt(Value <= otherInt.Value ? 1 : 0);
        }

        public override SValue GreaterThan(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform GreaterThanCheck on SInt and " + other.GetType().Name);
            return new SInt(Value > otherInt.Value ? 1 : 0);
        }

        public override SValue GreaterThanEqu(SValue other) {
            if (other is not SInt otherInt) throw new Exception("Can not perform GreaterThanEquCheck on SInt and " + other.GetType().Name);
            return new SInt(Value >= otherInt.Value ? 1 : 0);
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

        public override string ToString() {
            return $"<{GetType().Name} value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString(Value.ToString());
        }
    }
}
