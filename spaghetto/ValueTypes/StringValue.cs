using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class StringValue : Value {
        new public string value;

        public StringValue(string value) {
            this.value = value;
        }

        public override Value Copy() {
            return new StringValue(value).SetPosition(posStart, posEnd).SetContext(context);
        }

        public override (Value, SpaghettoException) AddedTo(Value other) {
            if (other is StringValue) {
                return (new StringValue(value + (other as StringValue).value).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform AddedTo with string to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) MultipliedBy(Value other) {
            if (other is Number) {
                return (new StringValue(value.Repeat((int)(other as Number).value)).SetContext(context), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform MultiplyBy with string to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsEqualTo(Value other) {
            if (other is StringValue) {
                return (new Number(value == (other as StringValue).value ? 1 : 0), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsEqualTo with string to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IsNotEqualTo(Value other) {
            if (other is StringValue) {
                return (new Number(value == (other as StringValue).value ? 0 : 1), null);
            }

            return (null, new TypeError(posStart, posEnd, "Can not perform IsEqualTo with string to " + other.GetType().Name));
        }

        public override bool IsTrue() {
            return (value.Length > 0);
        }

        public override string ToString() {
            return $"\"{value}\"";
        }
    }
}
