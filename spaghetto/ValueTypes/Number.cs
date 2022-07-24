using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    [Serializable]
    public class Number : Value {
        public static Class ClassImpl = new("Number", new(), new()
        {
            { "null", new Number(0) },
            { "true", new Number(1) },
            { "false", new Number(0) },

        }, new NativeFunction("ctor", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
        {
            if(args[0] is Number num) return num;
            throw new RuntimeError(posStart, posEnd, "Argument 0 was not a Number", ctx);
        }, new() { "str" }, true));
        new public double value;

        public Number(double value) {
            this.value = value;
        }

        public Number(object value) {
            if (value is double) {
                this.value = (double)value;
            } else if(value is float) {
                this.value = (float)value;
            } else if(value is short) {
                this.value = (short)value;
            } else if (value is byte) {
                this.value = (byte)value;
            } else if (value is int) {
                this.value = (int)value;
            }
        }

        public override Value Copy() {
            return new Number(value);
        }

        public override (Value, SpaghettoException) AddedTo(Value other) {
            if (other is Number) {
                return (new Number(value + (other as Number).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not add number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) SubtractedBy(Value other) {
            if (other is Number) {
                return (new Number(value - (other as Number).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not add number to " + other.GetType().Name));
        }
        public override (Value, SpaghettoException) MultipliedBy(Value other) {
            if (other is Number) {
                return (new Number(value * (other as Number).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not add number to " + other.GetType().Name));
        }
        public override (Value, SpaghettoException) DividedBy(Value other) {
            if (other is Number) {
                if ((other as Number).value == 0) {
                    return (null, new RuntimeError(other.posStart, other.posEnd, "Division by zero is not permitted", context));
                }

                return (new Number(value / (other as Number).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not add number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) PoweredBy(Value other) {
            if (other is Number) {
                return (new Number(Math.Pow(value, (other as Number).value)).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not power number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) Modulo(Value other) {
            if (other is Number) {
                return (new Number(value % (other as Number).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform modulo with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsEqualTo(Value other) {
            if (other is Number) {
                return (new Number(value == (other as Number).value ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsEqualTo with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsNotEqualTo(Value other) {
            if (other is Number) {
                return (new Number((value == (other as Number).value) ? 0 : 1).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsNotEqualTo with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsLessThan(Value other) {
            if (other is Number) {
                return (new Number(value < (other as Number).value ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsLessThan with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsLessThanOrEquals(Value other) {
            if (other is Number) {
                return (new Number(value <= (other as Number).value ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsLessThanOrEquals with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsGreaterThan(Value other) {
            if (other is Number) {
                return (new Number(value > (other as Number).value ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsGreaterThan with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsGreaterThanOrEquals(Value other) {
            if (other is Number) {
                return (new Number(value >= (other as Number).value ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsGreaterThanOrEquals with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) AndBy(Value other) {
            if (other is Number) {
                return (new Number((value != 0) && ((other as Number).value != 0) ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform AndBy with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) OrBy(Value other) {
            if (other is Number) {
                return (new Number((value != 0) || ((other as Number).value != 0) ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform OrBy with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) Not() {
            return (new Number((value == 0 ? 1 : 0)).SetContext(context), null);
        }

        public override bool IsTrue() {
            return (value != 0);
        }

        public static explicit operator Number(int i) => new(i);
        public static explicit operator Number(double d) => new(d);
        public static explicit operator Number(float f) => new(f);


        public override string ToString() {
            return this.Represent();
        }

        public override string Represent() {
            if (double.IsInfinity(value)) return (double.IsNegativeInfinity(value) ? "-Inf" : "Inf");

            return value.ToString();
        }

        public override Value Get(string identifier)
        {
            return ClassImpl.Get(identifier);
        }
    }
}
