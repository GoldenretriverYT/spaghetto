using spaghetto;
using spaghetto.Parsing.Nodes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = false, timings = false;

        static void Main(string[] args) {
            var interpreter = new Interpreter();
            spaghetto.Stdlib.Lang.Lib.Mount(interpreter.GlobalScope);
            spaghetto.Stdlib.IO.Lib.Mount(interpreter.GlobalScope);

            var tdict = new SDictionary();
            tdict.Value.Add((new SString("ok"), new SString("works string key")));
            tdict.Value.Add((new SInt(0), new SString("works int key")));

            interpreter.GlobalScope.Set("test", tdict);

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

                RunCode(interpreter, text);
            }
        }

        public static void RunCode(Interpreter interpreter, string text) {
            TimingInterpreterResult res = new();

            try {

                interpreter.Interpret(text, ref res);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(res.Result.LastValue.ToString());

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
            }

            if(showParseOutput && res.Result.AST != null) {
                PrintTree(res.Result.AST);
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