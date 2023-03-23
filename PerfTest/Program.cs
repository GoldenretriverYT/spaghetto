using spaghetto;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using spaghetto.Parsing.Nodes;

namespace PerfTest
{
    internal class Program {
        const string text = @"for(var i = 0; i < 100000; i = i + 1) { var x = i + (i * 2); };";
        static void Main(string[] args) {
            for (int i = 0; i < 50; i++) {
                Lexer lexer = new(text);
                List<SyntaxToken> tokens = lexer.Lex();

                Parser p = new(tokens);
                SyntaxNode parsed = p.Parse();

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
            }
        }
    }
}