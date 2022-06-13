namespace spaghetto {
    internal class Program {
        static void Main() {
            Thread t = new(() => {
                while (true) {
                    Console.Write("spaghetto > ");
                    string text = Console.ReadLine();

                    if(text.Trim() == String.Empty) continue;

                    try {
                        (RuntimeResult res, SpaghettoException err) = Intepreter.Run("<spaghetto_cli>", text);

                        if (err != null) throw err;
                        if (res.error != null) throw res.error;

                        if (res.value != null) {
                            Console.WriteLine(((res.value as ListValue).value.Count == 1 ? ((res.value as ListValue).value[0]?.Represent()) : res.value.Represent()));
                        } else {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
//                          Console.WriteLine("\x001B[3mNothing was returned");
                            Console.ResetColor();
                        }
                    } catch (Exception ex) {
                        if (ex is SpaghettoException)
                            Console.WriteLine("Error: " + ex.Message);
                        else
                            throw;
                    }
                }
            }, 1024 * 1024 * 10);

            t.Start();
            t.Join();
        }

#pragma warning disable IDE0051 // Nicht verwendete private Member entfernen
        static void StartNewCLI()
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen
        {
            string currentCommand = "";
            int cursorPos = 0;

            while(true)
            {
                Console.SetCursorPosition(cursorPos, 0);
                ConsoleKeyInfo cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Enter)
                {
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
                            Console.WriteLine(res.value.ToString());
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
                else if(cursorPos < Console.WindowWidth-1)
                {
                    currentCommand = currentCommand.Insert(cursorPos, cki.KeyChar.ToString());
                    cursorPos++;
                }

                Console.SetCursorPosition(0, 0);

                Lexer lex = new(currentCommand, "<cli_preview>");
                List<Token> tokens = lex.MakeTokens(true);
                int totalPrinted = 0;

                foreach (Token token in tokens)
                {
                    string o = "";

                    switch (token.type)
                    {
                        case (TokenType.Keyword):
                            if (token.posStart != null) o += currentCommand.SubstrInBounds(token.posStart.idx, token.posEnd.idx + (token.posEnd == token.posStart ? 1 : 0));
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            break;
                        case (TokenType.Identifier):
                            if (token.posStart != null) o += currentCommand.SubstrInBounds(token.posStart.idx, token.posEnd.idx + (token.posEnd == token.posStart ? 1 : 0));
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        default:
                            if (token.posStart != null) o += currentCommand.SubstrInBounds(token.posStart.idx, token.posEnd.idx + (token.posEnd == token.posStart ? 1 : 0));
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }

                    Console.Write(o + " ");
                    totalPrinted += (o + " ").Length;
                }

                Console.Write(" ".Repeat(Console.WindowWidth-totalPrinted));

                Console.SetCursorPosition(currentCommand.Length, 0);
            }
        }
    }
}