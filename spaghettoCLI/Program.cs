using spaghetto;
using spaghetto.Parsing.Nodes;
using System.Reflection;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = false, timings = false, rethrow = false, csstack = false;
        static Interpreter interpreter;

        static List<Assembly> assemblies = new();

        static void Main(string[] args) {
            for (var i = 0; i < args.Length; i++) {
                string[] subArgs = args[i].Split('=');
                if (args[i].StartsWith("--lib=")) {
                    assemblies.Add(Assembly.LoadFrom(subArgs[1]));
                }
            }

            InitInterpreter();

            while (true) {
                Console.ForegroundColor = ConsoleColor.Gray;
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

                    if (text.StartsWith("#cs")) {
                        csstack = !csstack;
                        Console.WriteLine("csstack: " + csstack);
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
            spaghetto.Stdlib.Interop.Lib.Mount(interpreter.GlobalScope);

            foreach(var assembly in assemblies) {
                foreach(var type in assembly.GetTypes().Where((x) => x.BaseType == typeof(ExternLibMain))) {
                    var instance = Activator.CreateInstance(type);
                    
                    if(instance is not ExternLibMain elm) {
                        Console.WriteLine("Fatal failure when importing " + type.Name + " from " + assembly.FullName);
                        continue;
                    }

                    elm.Mount(interpreter.GlobalScope);
                } 
            }
        }

        public static void RunCode(Interpreter interpreter, string text) {
            TimingInterpreterResult res = new();

            try {
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
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                if (csstack) Console.WriteLine("C# Stacktrace: " + ex.StackTrace);
                if (rethrow) throw;
            }

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