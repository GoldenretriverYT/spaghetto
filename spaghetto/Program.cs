namespace spaghetto {
    internal class Program {
        static void Main(string[] args) {
            while(true) {
                Console.Write("spaghetto > ");
                string text = Console.ReadLine();

                try {
                    (RuntimeResult res, SpaghettoException err) = Intepreter.Run("<spaghetto_cli>", text);

                    if (err != null) throw err;
                    if (res.error != null) throw res.error;

                    if (res.value != null) {
                        Console.WriteLine(res.value.ToString());
                    }else {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\x001B[3mNothing was returned");
                        Console.ResetColor();
                    }
                }catch(Exception ex) {
                    if (ex is SpaghettoException)
                        Console.WriteLine("Error: " + ex.Message);
                    else
                        throw;
                }
            }
        }
    }
}