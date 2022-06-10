using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace spaghetto {
    internal class Intepreter {
        public static SymbolTable<Value> globalSymbolTable = new() {
            { "null", new Number(0) },
            { "true", new Number(1) },
            { "false", new Number(0) },

            { "printLine", new NativeFunction("printLine", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.WriteLine((args[0]).ToString());
                return null;
            }, new() {"str"}) },

            { "print", new NativeFunction("print", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.Write(args[0].ToString());
                return null;
            }, new() {"str"}) },

            { "readLine", new NativeFunction("readLine", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                return new StringValue(Console.ReadLine());
            }, new()) },

            { "readNumber", new NativeFunction("readNumber", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                double result = 0;

                while(true) {
                    bool success = double.TryParse(Console.ReadLine(), out result);

                    if(success) break;
                }

                return new Number(result);
            }, new()) },

            { "isType", new NativeFunction("isType", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                switch((args[1] as StringValue).value) {
                    case "Number":
                        return new Number(args[0] is Number ? 1 : 0);
                    case "String":
                        return new Number(args[0] is StringValue ? 1 : 0);
                    case "List":
                        return new Number(args[0] is ListValue ? 1 : 0);
                    case "Function":
                        return new Number((args[0] is Function || args[0] is NativeFunction) ? 1 : 0);
                    default:
                        throw new RuntimeError(posStart, posEnd, "Invalid type. Native types are Number, String, List and Function", ctx);
                }
            }, new() { "val", "type" })},

            { "getType", new NativeFunction("isType", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                if(args[0] is Number)
                    return new StringValue("Number");
                else if(args[0] is StringValue)
                    return new StringValue("String");
                else if(args[0] is ListValue)
                    return new StringValue("List");
                else if (args[0] is Function || args[0] is NativeFunction)
                    return new StringValue("Function");
                else if (args[0] is null)
                    return new StringValue("null");
                else
                    throw new Exception(args[0] + " is of unknown type.");
            }, new() { "val" })},

            { "clear", new NativeFunction("clear", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.Clear();
                return null;
            }, new() { })},

            {"toString",
                new NativeFunction("toString", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    return new StringValue(args[0].ToString());
                }, new() { "val "})
            },

            { "run",
                new NativeFunction("run", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    try
                    {
                        if (!(args[0] is StringValue)) throw new RuntimeError(posStart, posEnd, "Argument path must be of type String", ctx);

                        if (!File.Exists((args[0] as StringValue).value))
                        {
                            throw new RuntimeError(posStart, posEnd, "File not found", ctx);
                        }

                        //temporarily running line by line
                        string[] code = File.ReadAllLines((args[0] as StringValue).value);
                        Value lastVal = null;

                        foreach (string line in code)
                        {
                            System.Diagnostics.Debug.WriteLine(line);
                            (RuntimeResult res, SpaghettoException err) = Run((args[0] as StringValue).value, line);

                            if (err != null)
                            {
                                throw err;
                            }

                            lastVal = (res.value != null ? res.value.Copy() : null);
                        }

                        return lastVal;
                    }
                    catch (Exception ex)
                    {
                        if (ex is SpaghettoException || ex is IOException)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            return null;
                        }
                        else throw;
                    }
                }, new() { "path" })
            },
        };

        public static (RuntimeResult, SpaghettoException) Run(string fileName, string text) {
            Stopwatch sw = new();
            sw.Start();

            Lexer lexer = new Lexer(text, fileName);
            List<Token> tokens = lexer.MakeTokens();

            Debug.WriteLine("Lexer took " + sw.ElapsedMilliseconds + "ms");
            sw.Reset();
            System.Diagnostics.Debug.WriteLine("");
            foreach (Token token in tokens) System.Diagnostics.Debug.Write(token.ToString() + " ");
            System.Diagnostics.Debug.WriteLine("");

            sw.Start();
            Parser parser = new Parser(tokens);
            ParseResult ast = parser.Parse();
            if (ast.error != null) return (null, ast.error);

            Debug.WriteLine("Parser took " + sw.ElapsedMilliseconds + "ms");
            sw.Reset();

            Intepreter intepreter = new Intepreter();
            Context context = new Context("<global>");
            context.symbolTable = globalSymbolTable;
            RuntimeResult result = intepreter.Visit(ast.node, context);

            return (result, result.error);
        }

        public RuntimeResult Visit(Node node, Context context) {
            return node.Visit(context);
        }
    }

    internal class Context {
        public string displayName = null;
        public Context parentContext = null;
        public Position parentEntryPosition = null;
        public SymbolTable<Value> symbolTable = new();

        public Context(string displayName, Context parentContext = null, Position parentEntryPosition = null) {
            this.displayName = displayName;
            this.parentContext = parentContext;
            this.parentEntryPosition = parentEntryPosition;
        }
    } 

    internal class SymbolTable<T> : IEnumerable<Value> {
        public Dictionary<string, T> symbols = new(); // Second type must be changed later on
        public SymbolTable<T> parent = null;

        public SymbolTable(SymbolTable<T> parent = null) {
            this.parent = parent;
        }

        public T Get(string name) {
            if(!symbols.ContainsKey(name)) {
                if (parent != null) {
                    return parent.Get(name);
                }else {
                    return default(T);
                }
            }

            return symbols[name];
        }

        public void Set(string name, T value) {
            symbols[name] = value;
        }

        public void Add(string name, T value) {
            Set(name, value);
        }

        public void Remove(string name) {
            symbols.Remove(name);
        }

        public IEnumerator<Value> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }

    internal class RuntimeResult {
        public Value value;
        public SpaghettoException error;

        public Value Register(RuntimeResult res) {
            if (res.error != null) this.error = res.error;
            return res.value;
        }

        public RuntimeResult Success(Value value) {
            this.value = value;
            return this;
        }

        public RuntimeResult Failure(SpaghettoException error) {
            this.error = error;
            return this;
        }

        public override string ToString() {
            return @$"Value: {value}
Error: {error}";
        }
    }
}
