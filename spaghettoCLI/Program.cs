using spaghetto;
using System.Runtime.InteropServices;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = true;

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

                    continue;
                }

                RunCode(text);
            }
        }

        public static void RunCode(string text) {
            //try {
                Lexer lexer = new(text);
                List<SyntaxToken> tokens = lexer.Lex();

                if(showLexOutput) {
                    foreach (var tok in tokens) Console.WriteLine(tok.ToString());    
                }

                Parser p = new(tokens);
                SyntaxNode parsed = p.Parse();

                if(showParseOutput) PrintTree(parsed);

                var globalScope = new Scope();
                globalScope.Set("print", new SNativeFunction((List<SValue> args) => {
                    if (args.Count == 0) throw new Exception("Expected 1 argument on print call");
                    //if (args[0] is not SString str) throw new Exception("Argument 0 was expected to be a SString");
                    Console.WriteLine(args[0].ToString());
                    return args[0];
                }));
                Console.WriteLine(parsed.Evaluate(globalScope).ToString());
            //} catch (Exception ex) {
            //    Console.WriteLine("Error: " + ex.Message);
            //}
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