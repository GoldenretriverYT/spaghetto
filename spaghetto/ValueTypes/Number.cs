using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    [Serializable]
    internal class Number : Value {
        new public double value;

        public Number(double value) {
            this.value = value;
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
            System.Diagnostics.Debug.WriteLine("Call to IsNotEqualTo");

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
                return (new Number((value == 0 ? false : true) && ((other as Number).value == 0 ? false : true) ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform AndBy with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) OrBy(Value other) {
            if (other is Number) {
                return (new Number((value == 0 ? false : true) || ((other as Number).value == 0 ? false : true) ? 1 : 0).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform OrBy with number to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) Not() {
            return (new Number((value == 0 ? 1 : 0)).SetContext(context), null);
        }

        public override bool IsTrue() {
            return (value != 0 ? true : false);
        }

        public override string ToString() {
            if (double.IsInfinity(value)) return (double.IsNegativeInfinity(value) ? "-Inf" : "Inf");

            return value.ToString();
        }
    }
}
