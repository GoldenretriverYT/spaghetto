using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class ListValue : Value {
        public static Class ClassImpl = new("List", new()
        {
            {
                "size",
                new NativeFunction("size", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    return new Number((args[0] as ListValue).value.Count);
                }, new() { "self" }, false)
            },
            {
                "set",
                new NativeFunction("set", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    (args[0] as ListValue).value[(int)(args[1] as Number).value] = args[2];
                    return new Number(1);
                }, new() { "self", "idx", "value" }, false)
            },
        }, new()
        {
        });

        new public List<Value> value;

        public ListValue(List<Value> value) {
            this.value = value;
        }

        public override Value Copy() {
            return new ListValue(value).SetPosition(posStart, posEnd).SetContext(context);
        }

        public override (Value, SpaghettoException) AddedTo(Value other) {
            ListValue val = Copy() as ListValue;
            val.value.Add(other);
            return (val, null);
        }

        public override (Value, SpaghettoException) SubtractedBy(Value other) {
            if(other is Number) {
                ListValue val = Copy() as ListValue;
                int idx = (int)((other as Number).value);

                if (idx < 0 || idx > val.value.Count-1) {
                    return (null, new RuntimeError(posStart, posEnd, "Index " + idx + " was out of range. (0 to " + (val.value.Count-1) + ")", context));
                }

                val.value.RemoveAt(idx);
                return (val, null);
            }
            

            return (null, new TypeError(posStart, posEnd, "Can not perform SubtractedBy with List to " + other.GetType().Name));
        }

        public override (Value, SpaghettoException) IndexedBy(Value other) {
            if (other is Number) {
                int idx = (int)((other as Number).value);

                if (idx < 0 || idx > value.Count-1) {
                    return (null, new RuntimeError(posStart, posEnd, "Index " + idx + " was out of range. (0 to " + (value.Count-1) + ")", context));
                }

                return (value[idx], null);
            }


            return (null, new TypeError(posStart, posEnd, "Can not perform IndexedBy with List to " + other.GetType().Name));
        }

        public override string ToString() {
            return this.Represent();
        }

        public override string Represent() {
            return $"[{value.Join(", ")}]";
        }
    }
}
