using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using spaghetto.Parsing;
using spaghetto.Parsing.Nodes;

namespace spaghetto
{
    public class Interpreter {
        public Scope GlobalScope { get; private set; }

        public Interpreter() {
            GlobalScope = new(0);

            // Fixed default imports; should be kept as minimal as possible
            GlobalScope.Table["true"] = new SInt(1) { IsConstant = true };
            GlobalScope.Table["false"] = new SInt(0) { IsConstant = true };
            GlobalScope.Table["null"] = new SNull() { IsConstant = true };
        }

        public void Interpret(string text, ref InterpreterResult res) {
            Lexer lexer = new(text);
            res.LexedTokens = lexer.Lex();

            Parser p = new(res.LexedTokens, text);
            res.AST = p.Parse();

            res.LastValue = res.AST.EvaluateWithErrorCheck(GlobalScope);
            var hasErrored = Scope.HasErrored;
            Scope.HasErrored = false;
            if (hasErrored) {
                throw new Exception(Scope.ErrorMessage);
            }
        }

        public void Interpret(string text, ref TimingInterpreterResult res) {
            Stopwatch sw = new();

            sw.Start();
            Lexer lexer = new(text);
            res.Result.LexedTokens = lexer.Lex();
            res.LexTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            Parser p = new(res.Result.LexedTokens, text);
            res.Result.AST = p.Parse();
            res.ParseTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            res.Result.LastValue = res.Result.AST.EvaluateWithErrorCheck(GlobalScope);
            res.EvalTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();

            var hasErrored = Scope.HasErrored;
            Scope.HasErrored = false;
            if (hasErrored) {
                throw new Exception(Scope.ErrorMessage);
            }
        }
    }

    public struct InterpreterResult {
        public List<SyntaxToken> LexedTokens = null;
        public SyntaxNode AST = null;
        public SValue LastValue = null;

        public InterpreterResult() { }
    }

    public struct TimingInterpreterResult {
        public InterpreterResult Result = new();

        public double LexTime = 0, ParseTime = 0, EvalTime = 0;

        public TimingInterpreterResult() { }
    }
}
