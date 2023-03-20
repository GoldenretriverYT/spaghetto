using Cosmos.System;
using Cosmos.System.ScanMaps;
using spaghetto;
using spaghetto.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using Console = System.Console;
using Sys = Cosmos.System;

namespace SpaghettoCosmosTestKernel
{
    public class Kernel : Sys.Kernel {
        static bool showLexOutput = false, showParseOutput = false, timings = false;
        static Interpreter interpreter;
        protected override void BeforeRun() {
            Console.WriteLine("select kb layout (1=us 2=de)");
            string sel = Console.ReadLine();    

            if(sel == "1") {
                KeyboardManager.SetKeyLayout(new US_Standard());
            }else if(sel == "2") {
                KeyboardManager.SetKeyLayout(new DE_Standard());
            }

            interpreter = new Interpreter();
            spaghetto.Stdlib.Lang.Lib.Mount(interpreter.GlobalScope);
            spaghetto.Stdlib.IO.Lib.Mount(interpreter.GlobalScope);
        }

        protected override void Run() {
            Console.Write("spaghetto > ");
            string text = Console.ReadLine();

            if (text.Trim() == String.Empty) return;

            if (text.StartsWith("#")) {
                if (text.StartsWith("#lex")) {
                    showLexOutput = !showLexOutput;
                    Console.WriteLine("Showing Lex Output: " + showLexOutput);
                }

                if (text.StartsWith("#parse")) {
                    showParseOutput = !showParseOutput;
                    Console.WriteLine("Showing Parse Output: " + showParseOutput);
                }

                if (text.StartsWith("#time")) {
                    timings = !timings;
                    Console.WriteLine("Timings: " + timings);
                }

                return;
            }

            mDebugger.SendMessageBox("ok");
            RunCode(interpreter, text);
        }


        public static void RunCode(Interpreter interpreter, string text) {
            try {
                TimingInterpreterResult res = new();

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
                Trace.DoTrace();
            }
        }

        public static void PrintTree(SyntaxNode node, int ident = 0) {
            Console.WriteLine(Ident(ident) + node.ToString());

            foreach (var n in node.GetChildren()) {
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
