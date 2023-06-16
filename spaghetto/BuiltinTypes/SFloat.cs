namespace spaghetto {
    public class SFloat : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.Float;
        public float Value { get; set; }

        public SFloat() {
            Value = 0;
        }

        public SFloat(float value) {
            Value = value;
        }

        public override SValue Add(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Add on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SFloat(Value + otherInt.Value);
        }

        public override SValue Sub(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Sub on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SFloat(Value - otherInt.Value);
        }

        public override SValue Mul(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Mul on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SFloat(Value * otherInt.Value);
        }

        public override SValue Div(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Div on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SFloat(Value / otherInt.Value);
        }

        public override SValue Mod(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform Mod on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SFloat(Value % otherInt.Value);
        }

        public override SValue Equals(SValue other) {
            if (other is not SFloat otherInt) throw new Exception("Can not perform EqualsCheck on SFloat and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
            return new SInt(Value == otherInt.Value ? 1 : 0);
        }

        public override SValue ArithNot() {
            return new SFloat(-Value);
        }

        public override SValue CastToBuiltin(SBuiltinType other) {
            switch (other) {
                case SBuiltinType.Int:
                    return new SInt((int)Value);
                case SBuiltinType.Long:
                    return new SLong((long)Value);
                case SBuiltinType.Float:
                    return new SFloat(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy() {
            return Value == 1;
        }

        public override string ToString() {
            return $"<{SBuiltinTypeHelper.ToStr(BuiltinName)} value={Value}>";
        }

        public override SString ToSpagString() {
            return new SString(Value.ToString());
        }
    }
}
