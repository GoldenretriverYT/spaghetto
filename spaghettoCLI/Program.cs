using spaghetto;
using spaghetto.Parsing.Nodes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = false, timings = false, rethrow = false;
        static Interpreter interpreter;

        static void Main(string[] args) {
            InitInterpreter();

            while (true) {
                Console.Write("spaghetto > ");
                string text = Console.ReadLine();

                if (text.Trim() == String.Empty) continue;

                if (text.StartsWith("#")) {
                    if (text.StartsWith("#lex")) {
                        showLexOutput = !showLexOutput;
                        Console.WriteLine("Showing Lex Output: " + showLexOutput);
                    }

                    if (text.StartsWith("#parse")) {
                        showParseOutput = !showParseOutput;
                        Console.WriteLine("Showing Parse Output: " + showParseOutput);
                    }

                    if(text.StartsWith("#time")) {
                        timings = !timings;
                        Console.WriteLine("Timings: " + timings);
                    }

                    if (text.StartsWith("#rethrow")) {
                        rethrow = !rethrow;
                        Console.WriteLine("Rethrow: " + rethrow);
                    }

                    if (text.StartsWith("#reset")) {
                        InitInterpreter();
                        Console.WriteLine("Reset interpreter");
                    }

                    continue;
                }

                RunCode(interpreter, text);
            }
        }

        public static void InitInterpreter() {
            interpreter = new Interpreter();
            spaghetto.Stdlib.Lang.Lib.Mount(interpreter.GlobalScope);
            spaghetto.Stdlib.IO.Lib.Mount(interpreter.GlobalScope);

            var tdict = new SDictionary();
            tdict.Value.Add((new SString("ok"), new SString("works string key")));
            tdict.Value.Add((new SInt(0), new SString("works int key")));

            var classInstTest = new SClass("color");
            classInstTest.InstanceBaseTable.Add((new SString("$$ctor"),
                new SNativeFunction(
                    impl: (Scope scope, List<SValue> args) => {
                        // TODO: Add dot stack assignment; not possible yet

                        // TODO: Remove this code and replace it by safe methods
                        (args[0] as SClassInstance).InstanceTable.Add((new SString("r"), args[1] as SInt));
                        (args[0] as SClassInstance).InstanceTable.Add((new SString("g"), args[2] as SInt));
                        (args[0] as SClassInstance).InstanceTable.Add((new SString("b"), args[3] as SInt));

                        return args[0];
                    },
                    expectedArgs: new() { "self", "r", "g", "b" }
                )
            ));

            classInstTest.InstanceBaseTable.Add((new SString("mul"),
                new SNativeFunction(
                    impl: (Scope scope, List<SValue> args) => {
                        if (args[1] is not SClassInstance inst || inst.Class.Name != "color") throw new Exception("Expected argument 0 to be of type 'color'");

                        var current = args[0] as SClassInstance;

                        SClassInstance newInst = new(inst.Class);
                        newInst.CallConstructor(scope, new() { newInst, current.Dot(new SString("r")).Mul(inst.Dot(new SString("r"))), current.Dot(new SString("g")).Mul(inst.Dot(new SString("g"))), current.Dot(new SString("b")).Mul(inst.Dot(new SString("b"))) });

                        return newInst;
                    },
                    expectedArgs: new() { "self", "other" },
                    isClassInstanceFunc: true
                )
            ));

            classInstTest.InstanceBaseTable.Add((new SString("$$toString"),
                new SNativeFunction(
                    impl: (Scope scope, List<SValue> args) => {
                        var current = args[0] as SClassInstance;
                        return new SString("<Color R=" + args[0].Dot(new SString("r")).SpagToCsString() + " G=" + args[0].Dot(new SString("g")).SpagToCsString() + " B=" + args[0].Dot(new SString("b")).SpagToCsString() + ">");
                    },
                    expectedArgs: new() { "self" },
                    isClassInstanceFunc: true
                )
            ));

            interpreter.GlobalScope.Set("test", tdict);
            interpreter.GlobalScope.Set("color", classInstTest);
        }

        public static void RunCode(Interpreter interpreter, string text) {
            TimingInterpreterResult res = new();

            //try {

                interpreter.Interpret(text, ref res);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n  C#: " + res.Result.LastValue.ToString());
                Console.WriteLine("  Spag: " + res.Result.LastValue.ToSpagString().Value);


                if (timings) {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Timings:");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Lex: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(res.LexTime + "ms");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Parse: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(res.ParseTime + "ms");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Eval: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(res.EvalTime + "ms");
                }

                Console.ResetColor();
            //} catch (Exception ex) {
            //    Console.WriteLine("Error: " + ex.Message);
            //    if (rethrow) throw;
            //}

            if(showParseOutput && res.Result.AST != null) {
                PrintTree(res.Result.AST);
            }

            if(showLexOutput && res.Result.LexedTokens != null) {
                foreach (var tok in res.Result.LexedTokens) Console.WriteLine("  " + tok.ToString());
            }
        }

        public static void PrintTree(SyntaxNode node, int ident = 0) {
            Console.WriteLine(Ident(ident) + node.ToString());
            
            foreach(var n in node.GetChildren()) {
                PrintTree(n, ident + 2);
            }
        }

        public static string Ident(int ident) {
            StringBuilder b = new();
            for (int i = 0; i < ident; i++) b.Append(" ");
            return b.ToString();
        }
    }
}