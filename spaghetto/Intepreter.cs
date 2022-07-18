using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace spaghetto {
    public class Intepreter {
        public static Random rnd = new();
        public static bool isInited = false;

        public static SymbolTable globalSymbolTable = new() { // These methods pretend to be non-static so the first argument is preserved, a better solution will be added at some point
            { "null", new Number(0) },
            { "true", new Number(1) },
            { "false", new Number(0) },
            { "String", StringValue.ClassImpl },
            { "Number", Number.ClassImpl },
            { "Math", MathClass.@class },
            { "TestObject", TestObject.@class },
            { "File", FileClass.@class },
            { "Path", Path.@class },


            {
                "printLine", new NativeFunction("printLine", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.WriteLine((args[0]).ToString());
                return new Number(0);
            }, new() {"str"}, false) },

            { "print", new NativeFunction("print", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.Write(args[0].ToString());
                return new Number(0);
            }, new() {"str"}, false) },

            { "readLine", new NativeFunction("readLine", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                return new StringValue(Console.ReadLine());
            }, new(), false) },

            { "readNumber", new NativeFunction("readNumber", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                double result = 0;

                while(true) {
                    bool success = double.TryParse(Console.ReadLine(), out result);

                    if(success) break;
                }

                return new Number(result);
            }, new(), false)},

            {
                "getNull",
                new NativeFunction("getNull", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    return null;
                }, new() {}, false)
            },

            { "isType", new NativeFunction("isType", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                return (args[1] as StringValue).value switch
                {
                    "Number" => new Number(args[0] is Number ? 1 : 0),
                    "String" => new Number(args[0] is StringValue ? 1 : 0),
                    "List" => new Number(args[0] is ListValue ? 1 : 0),
                    "Function" => new Number((args[0] is Function || args[0] is NativeFunction) ? 1 : 0),
                    "Class" => new Number((args[0] is Class) ? 1 : 0),
                    "ClassInstance" => new Number((args[0] is ClassInstance) ? 1 : 0),

                    _ => throw new RuntimeError(posStart, posEnd, "Invalid type. Native types are Number, String, List, Class, ClassInstance and Function", ctx),
                };
            }, new() { "val", "type" }, false)},

            { "getType", new NativeFunction("isType", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                if (args[0] is Number)
                    return new StringValue("Number");
                else if (args[0] is StringValue)
                    return new StringValue("String");
                else if (args[0] is ListValue)
                    return new StringValue("List");
                else if (args[0] is Function || args[0] is NativeFunction)
                    return new StringValue("Function");
                else if (args[0] is Class)
                    return new StringValue("Class");
                else if (args[0] is ClassInstance)
                    return new StringValue((args[0] as ClassInstance).clazz.name);
                else if (args[0] is null)
                    return new StringValue("null");
                else
                    throw new Exception(args[0] + " is of unknown type.");
            }, new() { "val" }, false)},

            { "clear", new NativeFunction("clear", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                Console.Clear();
                return new Number(0);
            }, new() { }, false )},

            {
                "randomBetween",
                new NativeFunction("randomBetween", (List<Value> args, Position posStart, Position posEnd, Context ctx) => { // func e() -> if(randomBetween(1, 1000000) == 8) then randomBetween("e", "a") else e()
                    if(args[0] is not Number) throw new RuntimeError(posStart, posEnd, "Argument min must be of type Number but is " + args[0].GetType().Name, ctx);
                    if(args[1] is not Number) throw new RuntimeError(posStart, posEnd, "Argument max must be of type Number", ctx);

                    return (Number)rnd.Next((int)(args[0] as Number).value, (int)(args[1] as Number).value);
                }, new() { "min", "max"}, false)
            },

            { "run",
                new NativeFunction("run", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    try
                    {
                        if (args[0] is not StringValue) throw new RuntimeError(posStart, posEnd, "Argument path must be of type String", ctx);

                        if (!File.Exists((args[0] as StringValue).value))
                        {
                            throw new RuntimeError(posStart, posEnd, "File not found", ctx);
                        }

                        string code = File.ReadAllText((args[0] as StringValue).value);

                        (RuntimeResult res, SpaghettoException err) = Run((args[0] as StringValue).value, code);

                        if (err != null)
                        {
                            throw err;
                        }

                        return (res.value?.Copy());
                    }
                    catch (Exception ex)
                    {
                        if (ex is SpaghettoException || ex is IOException)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            return new Number(0);
                        }
                        else throw;
                    }
                }, new() { "path" }, false)
            },
            {
                "loadcslib",
                new NativeFunction("loadcslib", (List<Value> args, Position posStart, Position posEnd, Context ctx) => {
                    try
                    {
                        if (args[0] is not StringValue) throw new RuntimeError(posStart, posEnd, "Argument path must be of type String", ctx);

                        if (!File.Exists((args[0] as StringValue).value))
                        {
                            throw new RuntimeError(posStart, posEnd, "File not found", ctx);
                        }

                        var dll = Assembly.Load(File.ReadAllBytes(((args[0] as StringValue).value)));

                        foreach(AssemblyName refAsm in dll.GetReferencedAssemblies())
                        {
                            Debug.WriteLine("Loading " + refAsm.FullName);
                            Assembly.Load(refAsm);
                        }

                        var libMainType = dll.GetType("LibMain");
                        if (libMainType == null) throw new RuntimeError(posStart, posEnd, "Library has no LibMain class", ctx);
                        if (!libMainType.IsSubclassOf(typeof(CSSpaghettoLibBase.CSSpagLib))) throw new RuntimeError(posStart, posEnd, "Library LibMain class doesn't inherit CSSpagLib", ctx);
                        var libMain = Activator.CreateInstance(libMainType);
                        
                        var method = libMain.GetType().GetMethod("Initiliaze");
                        if (method == null) throw new RuntimeError(posStart, posEnd, "Library has no LibMain.Initiliaze(SpaghettoBridge) method", ctx);

                        method.Invoke(libMain, new object[] { new CSSpaghettoLibBase.SpaghettoBridge() });

                        return new Number(1);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SpaghettoException || ex is IOException)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            return new Number(0);
                        }
                        else throw;
                    }
                }, new() { "path" }, false)
            },
        };

        public static void Init()
        {
            if (isInited) return;

            TestObject.InitStatics();
            isInited = true;
        }

        public static (RuntimeResult, SpaghettoException) Run(string fileName, string text) {
            Init();

            long lexTime, parseTime, interpretTime;

            Stopwatch sw = new();
            sw.Start();

            Lexer lexer = new(text, fileName);
            List<Token> tokens = lexer.MakeTokens();

            lexTime = sw.ElapsedMilliseconds;
            sw.Restart();

            Parser parser = new(tokens);
            ParseResult ast = parser.Parse();
            if (ast.error != null) return (null, ast.error);

            parseTime = sw.ElapsedMilliseconds;
            sw.Restart();

            Context context = new("<global>");
            context.symbolTable = globalSymbolTable;
            RuntimeResult result = Intepreter.Visit(ast.node, context);

            interpretTime = sw.ElapsedMilliseconds;
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("[ -> TIMINGS] Lexing took: " + lexTime + "ms\n[ -> TIMINGS] Parsing took: " + parseTime + "ms\n[ -> TIMINGS] Interpreting took: " + interpretTime + "ms");

            return (result, result.error);
        }


        public static RuntimeResult Visit(Node node, Context context) {
            return node.Visit(context);
        }
    }

    public class Context {
        public string displayName = null;
        public Context parentContext = null;
        public Position parentEntryPosition = null;
        public SymbolTable symbolTable = new();
        public int depth = 0;

        public const int MaximumDepth = 5000;

        public Context(string displayName, Context parentContext = null, Position parentEntryPosition = null) {
            this.displayName = displayName;
            this.parentContext = parentContext;
            if (parentContext is not null) depth = parentContext.depth+1;
            if (depth > MaximumDepth) throw new StackOverflowException($"Attempted to create a context deeper than {MaximumDepth} layers. Your code might contain recursive code without a break-condition");
            this.parentEntryPosition = parentEntryPosition;
        }
    } 

    public class SymbolTable : IEnumerable<Value>, ICloneable {
        public Dictionary<string, Value> symbols;// Second type must be changed later on
        public SymbolTable parent = null;

        public SymbolTable(SymbolTable parent = null) {
            this.symbols = new();
            this.parent = parent;
        }

        public Value Get(string name) {
            if(!symbols.ContainsKey(name)) {
                if (parent != null) {
                    return parent.Get(name);
                }else {
                    return default;
                }
            }

            return symbols[name];
        }

        public void Set(string name, Value value) {
            symbols[name] = value;
        }

        public void Add(string name, Value value) {
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

        public object Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }

    public class RuntimeResult {
        public Value value;
        public SpaghettoException error;
        public Value functionReturnValue = null;
        public bool loopShouldContinue = false, loopShouldBreak = false;

        public Value Register(RuntimeResult res) {
            if (res.error != null) this.error = res.error;
            if (res.functionReturnValue != null) this.functionReturnValue = res.functionReturnValue;
            this.loopShouldContinue = res.loopShouldContinue;
            this.loopShouldBreak = res.loopShouldBreak;
            return res.value;
        }

        public void Reset() {
            this.value = null;
            this.error = null;
            this.functionReturnValue = null;
            this.loopShouldBreak = false;
            this.loopShouldContinue = false;
        }

        public RuntimeResult Success(Value value) {
            Reset();
            this.value = value;
            return this;
        }

        public RuntimeResult SuccessReturn(Value value) {
            this.Reset();
            this.functionReturnValue = value;
            return this;
        }

        public RuntimeResult SuccessContinue() {
            this.Reset();
            this.loopShouldContinue = true;
            return this;
        }

        public RuntimeResult SuccessBreak() {
            this.Reset();
            this.loopShouldBreak = true;
            return this;
        }

        public RuntimeResult Failure(SpaghettoException error) {
            Reset();
            this.error = error;
            return this;
        }

        public override string ToString() {
            return @$"Value: {value}
Error: {error}";
        }

        public bool ShouldReturn() {
            return (error != null || functionReturnValue != null | loopShouldContinue || loopShouldBreak);
        }
    }
}
