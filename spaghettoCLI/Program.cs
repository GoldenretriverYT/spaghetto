using spaghetto;

namespace spaghettoCLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> opts = new();

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("--"))
                {
                    if(args.Length-1 == i)
                    {
                        opts[args[i].Substr(2, args[i].Length)] = "true";
                    }
                    else
                    {
                        opts[args[i].Substr(2, args[i].Length)] = args[i+1];
                        i++;
                    }
                }
            }

            if(opts.Count != 0)
            {
                if(!opts.ContainsKey("file"))
                {
                    Console.WriteLine("Missing option --file <path>\nOptions can not directly be used in CLI mode, which is why the file option is required.");
                    Environment.Exit(0);
                }

                foreach(KeyValuePair<string, string> kvp in opts)
                {
                    if(Intepreter.runtimeOptions.ContainsKey(kvp.Key))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[Warn] Option " + kvp.Key + " might not be applied as it would override an already defined value which is not allowed.");
                    }else
                    {
                        Intepreter.runtimeOptions.Add(kvp.Key, kvp.Value);
                    }

                }

                RunCode(File.ReadAllText(opts["file"]));

                return;
            }

            Thread t = new(() => {
                while (true)
                {
                    Console.Write("spaghetto > ");
                    string text = Console.ReadLine();

                    if (text.Trim() == String.Empty) continue;

                    RunCode(text);
                }
            }, 1024 * 1024 * 10);

            t.Start();
            t.Join();
        }

        public static void RunCode(string text)
        {
            try
            {
                (RuntimeResult res, SpaghettoException err) = Intepreter.Run("<spaghetto_cli>", text);

                if (err != null) throw err;
                if (res.error != null) throw res.error;

                if (res.value != null)
                {
                    Console.WriteLine(((res.value as ListValue).value.Count == 1 ? ((res.value as ListValue).value[0]?.Represent()) : res.value.Represent()));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    //                          Console.WriteLine("\x001B[3mNothing was returned");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                if (ex is SpaghettoException)
                    Console.WriteLine("Error: " + ex.Message);
                else
                    throw;
            }
        }

#pragma warning disable IDE0051 // New CLI is work in progress (probably forever:tm:)
        static void StartNewCLI()
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen
        {
            string currentCommand = "";
            int cursorPos = 0;

            while (true)
            {
                Console.SetCursorPosition(cursorPos, 0);
                ConsoleKeyInfo cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Enter)
                {
                    if((cki.Modifiers & ConsoleModifiers.Shift) != 0)
                    {
                        currentCommand += "\n";
                        continue;
                    }

                    try
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.Write(" ".Repeat(Console.WindowWidth * (Console.WindowHeight - 1)));
                        Console.SetCursorPosition(0, 1);

                        (RuntimeResult res, SpaghettoException err) = Intepreter.Run("<spaghetto_cli>", currentCommand);

                        if (err != null) throw err;
                        if (res.error != null) throw res.error;

                        if (res.value != null)
                        {
                            Console.WriteLine(((res.value as ListValue).value.Count == 1 ? ((res.value as ListValue).value[0]?.Represent()) : res.value.Represent()));
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine("\x001B[3mNothing was returned");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is SpaghettoException)
                            Console.WriteLine("Error: " + ex.Message);
                        else
                            throw;
                    }

                    Console.SetCursorPosition(0, 0);
                }
                else if (cki.Key == ConsoleKey.LeftArrow)
                {
                    cursorPos = Math.Max(0, cursorPos - 1);
                    Console.SetCursorPosition(cursorPos, 0);
                }
                else if (cki.Key == ConsoleKey.RightArrow)
                {
                    cursorPos = Math.Min(currentCommand.Length, cursorPos + 1);
                    Console.SetCursorPosition(cursorPos, 0);
                }
                else if (cki.Key == ConsoleKey.Backspace)
                {
                    if (cursorPos - 1 >= 0 && currentCommand.Length > 0)
                    {
                        currentCommand = currentCommand.Remove(cursorPos - 1, 1);
                        cursorPos = Math.Max(0, Math.Min(currentCommand.Length, cursorPos - 1));
                        Console.SetCursorPosition(cursorPos, 0);
                    }
                }
                else if (cursorPos < Console.WindowWidth - 1)
                {
                    currentCommand = currentCommand.Insert(cursorPos, cki.KeyChar.ToString());
                    cursorPos++;
                }

                Console.SetCursorPosition(0, 0);

                Lexer lex = new(currentCommand, "<cli_preview>");
                List<Token> tokens = lex.MakeTokens(true);
                int totalPrinted = 0;

                int prevEnd = 0;
                foreach (Token token in tokens)
                {
                    string o = currentCommand.SubstrInBounds(prevEnd, token.posStart.idx);

                    if (token.posStart != null) o += currentCommand.SubstrInBounds(token.posStart.idx, token.posEnd.idx + (token.posEnd == token.posStart ? 1 : 0));
                    switch (token.type)
                    {
                        case (TokenType.Keyword):
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case (TokenType.Identifier):
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case (TokenType.Plus):
                        case (TokenType.Minus):
                        case (TokenType.Mul):
                        case (TokenType.Div):
                        case (TokenType.Mod):
                        case (TokenType.Pow):
                        case (TokenType.Float):
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case TokenType.String:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }

                    prevEnd = token.posEnd.idx;
                    Console.Write(o);
                    totalPrinted += (o).Length;
                }

                Console.Write(" ".Repeat(Console.WindowWidth - totalPrinted));

                Console.SetCursorPosition(currentCommand.Length, 0);
            }
        }
    }
}