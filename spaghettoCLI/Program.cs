using spaghetto;
using System.Text;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false, showParseOutput = false;

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

                PrintTree(parsed);

                Console.WriteLine(parsed.Evaluate(new Scope()).ToString());
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