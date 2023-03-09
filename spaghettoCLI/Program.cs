using spaghetto;

namespace spaghettoCLI
{
    public class Program {
        static bool showLexOutput = false;

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

                    continue;
                }

                RunCode(text);
            }
        }

        public static void RunCode(string text) {
            try {
                Lexer lexer = new(text);
                List<SyntaxToken> tokens = lexer.Lex();

                if(showLexOutput) {
                    foreach (var tok in tokens) Console.WriteLine(tok.ToString());    
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}