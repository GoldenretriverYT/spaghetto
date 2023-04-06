using spaghetto;

namespace PerfTest {
    internal class Program {
        static void Main(string[] args) {
            for(int i = 0; i < 100; i++) {
                var interpreter = new Interpreter();
                spaghetto.Stdlib.Lang.Lib.Mount(interpreter.GlobalScope);
                spaghetto.Stdlib.IO.Lib.Mount(interpreter.GlobalScope);

                TimingInterpreterResult res = new();
                interpreter.Interpret(@"import native all;

var startTime = time();

class fixedprops X {
    prop a = 0;
    prop b = 0;
    prop c = 0;

    func ctor (a, b, c) {
        self.a = a;
        self.b = b;
        self.c = c;
    }

    func foo () {
        return self.a + self.b + self.c;
    }

    func bar () {
        return self.a * self.b / self.c;
    }

    func baz () {
        return <long>(self.a) - <long>(self.b) + <long>(self.c);
    }
}

for (var i = 0; i < 5000; i++) {
    var x = new X(i, i + 1, i + 2);
    x.foo();
    x.bar();
    x.baz();
}", ref res);
            }
        }
    }
}