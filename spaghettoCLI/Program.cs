using spaghetto;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = false, timings = false;

        static void Main(string[] args) {
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

                    continue;
                }

                RunCode(text);
            }
        }

        public static void RunCode(string text) {
            try {
                Stopwatch sw = new();

                sw.Start();
                Lexer lexer = new(text);
                List<SyntaxToken> tokens = lexer.Lex();

                var lexingTime = sw.Elapsed.TotalMilliseconds;

                if(showLexOutput) {
                    foreach (var tok in tokens) Console.WriteLine(tok.ToString());    
                }

                sw.Restart();

                Parser p = new(tokens);
                SyntaxNode parsed = p.Parse();

                var parsingTime = sw.Elapsed.TotalMilliseconds;

                if(showParseOutput) PrintTree(parsed);

                sw.Restart();
                var globalScope = new Scope();

                #region init scope
                globalScope.Set("print", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                    if (args.Count == 0) throw new Exception("Expected 1 argument on print call");
                    //if (args[0] is not SString str) throw new Exception("Argument 0 was expected to be a SString");
                    Console.WriteLine(args[0].ToSpagString().Value);
                    return args[0];
                }));

                globalScope.Set("typeof", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                    if (args.Count == 0) throw new Exception("Expected 1 argument on typeof call");

                    return new SString(args[0].BuiltinName.ToString());
                }));

                globalScope.Set("toString", new SNativeFunction((Scope callingScope, List<SValue> args) => {
                    if (args.Count == 0) throw new Exception("Expected 1 argument on typeof toString");

                    return args[0].ToSpagString();
                }));
                #endregion

                var evalRes = parsed.Evaluate(globalScope);

                sw.Stop();
                var evalTime = sw.Elapsed.TotalMilliseconds;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(evalRes.ToString());

                if(timings) {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Timings:");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Lex: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lexingTime + "ms");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Parse: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(parsingTime + "ms");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  Eval: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(evalTime + "ms");
                }

                Console.ResetColor();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
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