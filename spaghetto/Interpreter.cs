using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spaghetto.Parsing;
using spaghetto.Parsing.Nodes;

namespace spaghetto
{
    public class Interpreter {
        public Scope GlobalScope { get; private set; }

        public Interpreter() {
            GlobalScope = new();
        }

        public void Interpret(string text, ref InterpreterResult res) {
            Lexer lexer = new(text);
            res.LexedTokens = lexer.Lex();

            Parser p = new(res.LexedTokens);
            res.AST = p.Parse();

            res.LastValue = res.AST.Evaluate(GlobalScope);
        }

        public void Interpret(string text, ref TimingInterpreterResult res) {
            Stopwatch sw = new();

            sw.Start();
            Lexer lexer = new(text);
            res.Result.LexedTokens = lexer.Lex();
            res.LexTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            Parser p = new(res.Result.LexedTokens);
            res.Result.AST = p.Parse();
            res.ParseTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            res.Result.LastValue = res.Result.AST.Evaluate(GlobalScope);
            res.EvalTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();
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
