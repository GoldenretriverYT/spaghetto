﻿using System.Globalization;
using System.Runtime.CompilerServices;

namespace spaghetto {
    public class Lexer {
        public string Code { get; set; }
        public int Position { get; set; } = 0;

        public int Line { get; set; } = 1;
        public int Column { get; set; } = 1;

        public string FileName { get; set; } = "unknown";

        public char Current => Peek(0);

        public char Peek(int off = 0) {
            if (Position + off >= Code.Length || Position + off < 0) return '\0';
            return Code[Position + off];
        }

        public Lexer(string code, string fileName = "unknown") {
            Code = code;
            FileName = fileName;
        }

        public List<SyntaxToken> Lex() {
            List<SyntaxToken> tokens = new();

            while(Current != '\0') {
                Column++;

                SyntaxToken insertToken = new(SyntaxType.BadToken, Position, null, Current.ToString(), Column, Line, FileName);
                switch(Current) {
                    case ';':
                        insertToken = (new(SyntaxType.Semicolon, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '=':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.EqualsEquals, Position, null, "==", Column, Line, FileName));
                        } else if (Peek(1) == '>') {
                            Position++;
                            insertToken = (new(SyntaxType.Arrow, Position, null, "=>", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Equals, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '<':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.LessThanEqu, Position, null, "<=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.LessThan, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '>':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.GreaterThanEqu, Position, null, ">=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.GreaterThan, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '|':
                        if (Peek(1) == '|') {
                            Position++;
                            insertToken = (new(SyntaxType.OrOr, Position, null, "||", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.BadToken, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '&':
                        if (Peek(1) == '&') {
                            Position++;
                            insertToken = (new(SyntaxType.AndAnd, Position, null, "&&", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.BadToken, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '+':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.PlusEqu, Position, null, "+=", Column, Line, FileName));
                        } else if (Peek(1) == '+') {
                            Position++;
                            insertToken = (new(SyntaxType.PlusPlus, Position, null, "++", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Plus, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '-':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.MinusEqu, Position, null, "-=", Column, Line, FileName));
                        } else if (Peek(1) == '-') {
                            Position++;
                            insertToken = (new(SyntaxType.MinusMinus, Position, null, "--", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Minus, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '%':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.ModEqu, Position, null, "%=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Mod, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '*':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.MulEqu, Position, null, "*=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Mul, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '/':
                        if(Peek(1) == '/') {
                            SkipComment();
                            continue;
                        }

                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.DivEqu, Position, null, "/=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Div, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case '#':
                        insertToken = (new(SyntaxType.Idx, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '.':
                        insertToken = (new(SyntaxType.Dot, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case ',':
                        insertToken = (new(SyntaxType.Comma, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '(':
                        insertToken = (new(SyntaxType.LParen, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case ')':
                        insertToken = (new(SyntaxType.RParen, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '[':
                        insertToken = (new(SyntaxType.LSqBracket, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case ']':
                        insertToken = (new(SyntaxType.RSqBracket, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '{':
                        insertToken = (new(SyntaxType.LBraces, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '}':
                        insertToken = (new(SyntaxType.RBraces, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                    case '!':
                        if (Peek(1) == '=') {
                            Position++;
                            insertToken = (new(SyntaxType.BangEquals, Position, null, "!=", Column, Line, FileName));
                        } else {
                            insertToken = (new(SyntaxType.Bang, Position, null, Current.ToString(), Column, Line, FileName));
                        }

                        break;
                    case ':':
                        insertToken = (new(SyntaxType.Colon, Position, null, Current.ToString(), Column, Line, FileName));
                        break;
                }

                if (insertToken.Type == SyntaxType.BadToken) {
                    if (char.IsDigit(Current)) {
                        tokens.Add(ParseNumber());
                    } else if (Current == '"') {
                        tokens.Add(ParseString());
                    } else if (char.IsLetter(Current)) {
                        tokens.Add(ParseIdentifierOrKeyword());
                    } else if (char.IsWhiteSpace(Current)) Position++;
                    else {
                        throw new Exception("Bad token at pos " + insertToken.Position + " with text " + insertToken.Text);
                    }
                } else {
                    tokens.Add(insertToken);
                    Position++;
                }
            }

            tokens.Add(new SyntaxToken(SyntaxType.EOF, Position, null, "<EOF>", Column, Line, FileName));
            return tokens;
        }

        private void SkipComment() {
            while(Current != '\0' && Current != '\n') {
                if(Current == '\n') {
                    Line++;
                    Column = 1;
                } else {
                    Column++;
                }

                Position++;
            }
        }

        private SyntaxToken ParseIdentifierOrKeyword() {
            string str = "";
            int startPos = Position;

            while (Current != '\0' && Current != ' ' && (char.IsLetterOrDigit(Current) || Current == '_')) {
                str += Current;
                Position++;
            }

            var token = new SyntaxToken(SyntaxType.Identifier, startPos, str, str, Column, Line, FileName);
            SyntaxFacts.ClassifyIdentifier(ref token);

            return token;
        }


        private SyntaxToken ParseString() {
            string str = "";
            int startPos = Position;

            Position++;
            Column++;
            
            while(!(Current == '"' && Peek(-1) != '\\') && Current != '\0') {
                if (Current == '\\') {
                    anotherEscapeChar:
                    Position++;
                    Column++;

                    switch (Current) {
                        case '"': str += "\""; break;
                        case 'n': str += "\n"; break;
                        case '\\': {
                            str += "\\";
                            Position++;
                            Column++;
                            if (Current == '"' || Current == '\0') goto done;
                            if (Current == '\\') goto anotherEscapeChar;
                            str += Current;
                            break;
                        }
                        case '0': str += "\0"; break;
                        default: throw new Exception("Invalid escape sequence");
                    }

                    Position++;
                    Column++;
                } else {
                    str += Current;
                    Position++;
                    Column++;
                }
            }

            done:

            Position++;
            Column++;

            if (Current == 'i') {
                Position++;
                return new(SyntaxType.Identifier, startPos, str, str, Column, Line, FileName);
            }else {
                return new(SyntaxType.String, startPos, str, str, Column, Line, FileName);
            }
        }

        private SyntaxToken ParseNumber() {
            string numStr = "";
            bool isDecimal = false;
            int startPos = Position;

            while((char.IsDigit(Current) || Current == '.') && Current != '\0') {
                numStr += Current;

                if(Current == '.') {
                    isDecimal = true;
                }

                Position++;
                Column++;
            }

            if(isDecimal) {
                if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                return new(SyntaxType.Float, startPos, floatVal, numStr, Column, Line, FileName);
            }else {
                if (!int.TryParse(numStr, out int intVal)) throw new Exception("Invalid number!");
                return new(SyntaxType.Int, startPos, intVal, numStr, Column, Line, FileName);
            }
        }
    }

    public struct SyntaxToken {
        public SyntaxType Type { get; set; }
        public int Position { get; set; }
        public int EndPosition => Position + Text.Length;
        public object Value { get; set; }
        public string Text { get; set; }

        public int Line { get; set; }
        public int Column { get; set; }
        public string FileName { get; set; }

        public SyntaxToken(SyntaxType type, int pos, object val, string txt, int col, int line, string file) {
            Type = type;
            Position = pos;
            Value = val;
            Text = txt;
            Line = line;
            Column = col;
            FileName = file;
        }

        public override string ToString() {
            return Type.ToString().PadRight(16) + " at " + Position.ToString().PadRight(3) + " with val: " + (Value ?? "null").ToString().PadRight(16) + " text: " + Text.ToString().PadRight(16);
        }
    }

    public enum SyntaxType {
        Semicolon,
        Keyword,
        Identifier,
        Equals,
        EqualsEquals,
        Arrow,
        AndAnd,
        OrOr,
        LessThan,
        GreaterThan,
        GreaterThanEqu,
        LessThanEqu,

        Plus, Minus,
        PlusEqu, MinusEqu,
        PlusPlus, MinusMinus,

        Mod, Mul, Div,
        ModEqu, MulEqu, DivEqu,
        Idx,

        Pow,
        Dot,
        LParen,
        RParen,
        Int,
        Float,
        String,
        LSqBracket,
        RSqBracket,
        LBraces,
        RBraces,
        Bang,
        EOF,
        BadToken,
        Comma,
        Colon,
        BangEquals,
    }

    public static class SyntaxFacts {
        public static void ClassifyIdentifier(ref SyntaxToken token) {
            if (token.Text.ToString() is "return" or "continue" or "break"
                                       or "if" or "elseif" or "else"
                                       or "for" or "while" or "func" or "var"
                                       or "import" or "native" or "new"
                                       or "class" or "static" or "export"
                                       or "const" or "fixedprops" or "prop"
                                       or "repeat" or "times" or "op"
                                       or "as") {
                token.Type = SyntaxType.Keyword;
            }
        }
    }
}
