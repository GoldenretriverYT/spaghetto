using spaghetto;
using static System.Net.Mime.MediaTypeNames;

namespace PerfTest {
    internal class Program {
        static void Main(string[] args) {
            for(int i = 0; i < 50; i++) {
                var interpreter = new Interpreter();
                spaghetto.Stdlib.Lang.Lib.Mount(interpreter.GlobalScope);
                spaghetto.Stdlib.IO.Lib.Mount(interpreter.GlobalScope);

                TimingInterpreterResult res = new();
                interpreter.Interpret("import native io; import native lang; import \"vector2.spag\"; import \"vector2test.spag\"; runVectorTests();", ref res);
            }
        }
    }
}