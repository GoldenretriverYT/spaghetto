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
            {
                "get",
                new NativeFunction("get", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    if (args[1] is Number)
                    {
                        int idx = (int)(args[1] as Number).value;

                        if (idx < 0 || idx > (args[0] as ListValue).value.Count - 1)
                        {
                            throw new RuntimeError(posStart, posEnd, "Index " + idx + " was out of range. (0 to " + ((args[0] as ListValue).value.Count - 1) + ")", ctx);
                        }

                        return (args[0] as ListValue).value[idx];
                    }else
                    {
                        throw new RuntimeError(posStart, posEnd, "Argument idx must be a number", ctx);
                    }
                }, new() { "self", "idx" }, false)
            },
            {
                "has",
                new NativeFunction("has", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    foreach(Value entry in (args[0] as ListValue).value) {
                        if((entry.IsEqualTo(args[1]).Item1 as Number).value == 1) {
                            return new Number(1);
                        }
                    }

                    return new Number(0);
                }, new() { "self", "value" }, false)
            },
            {
                "remove",
                new NativeFunction("remove", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    if (args[1] is Number)
                    {
                        int idx = (int)(args[1] as Number).value;

                        if (idx < 0 || idx > (args[0] as ListValue).value.Count - 1)
                        {
                            throw new RuntimeError(posStart, posEnd, "Index " + idx + " was out of range. (0 to " + ((args[0] as ListValue).value.Count - 1) + ")", ctx);
                        }

                        (args[0] as ListValue).value.RemoveAt(idx);
                        return new Number(1);
                    }
                    else
                    {
                        throw new RuntimeError(posStart, posEnd, "Argument idx must be a number", ctx);
                    }
                }, new() { "self", "idx" }, false)
            },
            {
                "setStrictType",
                new NativeFunction("setStrictType", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    if (args[1] is StringValue)
                    {
                        switch((args[1] as StringValue).value)
                        {
                            case "Number":
                                (args[0] as ListValue).StrictEnforcedType = typeof(Number);
                                break;
                            case "String":
                                (args[0] as ListValue).StrictEnforcedType = typeof(StringValue);
                                break;
                            case "List":
                                (args[0] as ListValue).StrictEnforcedType = typeof(ListValue);
                                break;
                            case "Function":
                                (args[0] as ListValue).StrictEnforcedType = typeof(BaseFunction);
                                break;
                            case "Class":
                                (args[0] as ListValue).StrictEnforcedType = typeof(Class);
                                break;
                            case "ClassInstance":
                                (args[0] as ListValue).StrictEnforcedType = typeof(ClassInstance);
                                break;
                            default:
                                throw new RuntimeError(posStart, posEnd, "Invalid type. Native types are Number, String, List, Class, ClassInstance and Function", ctx);
                        }

                        return new Number(0);
                    }
                    else
                    {
                        throw new RuntimeError(posStart, posEnd, "Argument nativeType must be a string", ctx);
                    }
                }, new() { "self", "nativeType" }, false)
            },
        }, new()
        {
        });

        new public List<Value> value;
        public Type StrictEnforcedType { get; set; } = null;

        public ListValue(List<Value> value, Type strictEnforcedType = null) {
            this.StrictEnforcedType = strictEnforcedType;
            this.value = value;
        }

        public override Value Copy() {
            return new ListValue(value, StrictEnforcedType).SetPosition(posStart, posEnd).SetContext(context);
        }

        public override (Value, SpaghettoException) AddedTo(Value other) {
            if (StrictEnforcedType != null)
                if (!(other.GetType().FullName == StrictEnforcedType.FullName))
                    return (null, new RuntimeError(posStart, posEnd, "List has strict type and given argument did not match it.", context));

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

        public override Value Get(string identifier)
        {
            return ClassImpl.Get(identifier);
        }
    }
}
