using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Helpers {
    public static class NativeFuncGen {
        public static SNativeFunction New<T>(string name, Func<Scope, T, SValue> func, params string[] argNames) where T : SValue {
            List<Type> ts = new() { typeof(T) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T>(this List<(string key, SValue value)> table, string name, Func<Scope, T, SValue> func, params string[] argNames) where T : SValue {
            table.Add((name, New(name, func, argNames)));
        }

        public static SNativeFunction New<T, T2>(string name, Func<Scope, T, T2, SValue> func, params string[] argNames) where T : SValue where T2 : SValue {
            List<Type> ts = new() { typeof(T), typeof(T2) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0], (T2)args[1]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T, T2>(this List<(string key, SValue value)> table, string name, Func<Scope, T, T2, SValue> func, params string[] argNames) where T : SValue where T2 : SValue {
            table.Add((name, New(name, func, argNames)));
        }
        public static SNativeFunction New<T, T2, T3>(string name, Func<Scope, T, T2, T3, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue {
            List<Type> ts = new() { typeof(T), typeof(T2), typeof(T3) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0], (T2)args[1], (T3)args[2]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T, T2, T3>(this List<(string key, SValue value)> table, string name, Func<Scope, T, T2, T3, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue {
            table.Add((name, New(name, func, argNames)));
        }

        public static SNativeFunction New<T, T2, T3, T4>(string name, Func<Scope, T, T2, T3, T4, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue {
            List<Type> ts = new() { typeof(T), typeof(T2), typeof(T3), typeof(T4) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T, T2, T3, T4>(this List<(string key, SValue value)> table, string name, Func<Scope, T, T2, T3, T4, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue {
            table.Add((name, New(name, func, argNames)));
        }

        public static SNativeFunction New<T, T2, T3, T4, T5>(string name, Func<Scope, T, T2, T3, T4, T5, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue where T5 : SValue {
            List<Type> ts = new() { typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T, T2, T3, T4, T5>(this List<(string key, SValue value)> table, string name, Func<Scope, T, T2, T3, T4, T5, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue where T5 : SValue {
            table.Add((name, New(name, func, argNames)));
        }

        public static SNativeFunction New<T, T2, T3, T4, T5, T6>(string name, Func<Scope, T, T2, T3, T4, T5, T6, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue where T5 : SValue where T6 : SValue {
            List<Type> ts = new() { typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };

            return new SNativeFunction((Scope scope, List<SValue> args) => {
                // Check all arg types to match the T type
                for (int i = 0; i < args.Count; i++) {
                    if (i >= ts.Count) {
                        throw new Exception($"Too many arguments passed to function {name}");
                    }

                    if (args[i].GetType() != ts[i]) {
                        throw new Exception($"Expected argument {i} to be a {typeof(T).Name}");
                    }
                }

                return func(scope, (T)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]);
            }, argNames.ToList());
        }

        public static void AddNativeFunc<T, T2, T3, T4, T5, T6>(this List<(string key, SValue value)> table, string name, Func<Scope, T, T2, T3, T4, T5, T6, SValue> func, params string[] argNames) where T : SValue where T2 : SValue where T3 : SValue where T4 : SValue where T5 : SValue where T6 : SValue {
            table.Add((name, New(name, func, argNames)));
        }
    }
}
