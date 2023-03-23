namespace spaghettoPacker {
    /// <summary>
    /// spaghettoPacker is an idea that will be implemented at some point.
    /// 
    /// Its goal is to generate a .NET Project that basically invokes the interpreter.
    /// It should generate a single file .exe (not self contained(=.NET installation required to run))
    /// 
    /// Maybe(!) we could pre parse the AST and just evaluate the AST in the packed exe. This would
    /// get rid of the lexing & parsing time.
    /// 
    /// Even better would obviously be to emit IL; but that is not planned right now.
    /// </summary>
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello, World!");
        }
    }
}