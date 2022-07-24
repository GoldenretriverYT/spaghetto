using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class DictionaryValue : Value {
        public static Class ClassImpl = new("Dictionary", new()
        {
            {
                "size",
                new NativeFunction("size", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    return new Number((args[0] as DictionaryValue).value.Count);
                }, new() { "self" }, false)
            },
            {
                "set",
                new NativeFunction("set", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    foreach(KeyValuePair<Value, Value> vals in (args[0] as DictionaryValue).value) {
                        (Value val, SpaghettoException err) = vals.Key.IsEqualTo(args[1]);
                        if(err) throw err;

                        if((val as Number).value == 1) {
                            (args[0] as DictionaryValue).value[vals.Key] = args[2];
                            return new Number(0);
                        }
                    }

                    (args[0] as DictionaryValue).value[args[1]] = args[2];
                    return new Number(0);
                }, new() { "self", "name", "value" }, false)
            },
            {
                "get",
                new NativeFunction("get", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    foreach(KeyValuePair<Value, Value> vals in (args[0] as DictionaryValue).value) {
                        (Value val, SpaghettoException err) = vals.Key.IsEqualTo(args[1]);
                        if(err) throw err;

                        if((val as Number).value == 1) {
                            return vals.Value;
                        }
                    }

                    throw new RuntimeError(posStart, posEnd, "Key not found", ctx);
                }, new() { "self", "name" }, false)
            },
            {
                "hasKey",
                new NativeFunction("hasKey", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    foreach(KeyValuePair<Value, Value> vals in (args[0] as DictionaryValue).value) {
                        (Value val, SpaghettoException err) = vals.Key.IsEqualTo(args[1]);
                        if(err) throw err;

                        if((val as Number).value == 1) {
                            return new Number(1);
                        }
                    }

                    return new Number(0);
                }, new() { "self", "name" }, false)
            },
            {
                "remove",
                new NativeFunction("remove", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    if((args[0] as DictionaryValue).value.ContainsKey(args[1])) {
                        (args[0] as DictionaryValue).value.Remove(args[1]);
                        return new Number(0);
                    }else {
                        throw new RuntimeError(posStart, posEnd, "Key not found", ctx);
                    }
                }, new() { "self", "idx" }, false)
            },
        }, new()
        {
            {
                "new",
                new NativeFunction("new", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    return new DictionaryValue(new Dictionary<Value, Value>());
                }, new() { }, true)
            }
        });

        new public Dictionary<Value, Value> value;

        public DictionaryValue(Dictionary<Value, Value> value) {
            this.value = value;
        }

        public override Value Copy() {
            return new DictionaryValue(value).SetPosition(posStart, posEnd).SetContext(context);
        }


        public override (Value, SpaghettoException) IndexedBy(Value other) {
            foreach (KeyValuePair<Value, Value> vals in value) {
                (Value val, SpaghettoException err) = vals.Key.IsEqualTo(other);
                if (err) return (null, err);

                if ((val as Number).value == 1) {
                    return (vals.Value, null);
                }
            }

            return (null, new RuntimeError(posStart, posEnd, "Key not found", context));

        }

        public override string ToString() {
            return this.Represent();
        }

        public override string Represent() {
            return $"Dictionary";
        }

        public override Value Get(string identifier)
        {
            return ClassImpl.Get(identifier);
        }
    }
}
