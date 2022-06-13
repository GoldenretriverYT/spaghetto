
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class Lexer {
        public string text, fileName;
        public Position pos = null;
        public char currentChar = '\0';

        public Lexer(string text, string fileName) {
            this.fileName = fileName;
            this.pos = new Position(-1, 0, -1, fileName, text);
            this.text = text;
            Advance();
        }

        public void Advance() {
            pos.Advance(currentChar);

            if (pos.idx < text.Length)
                currentChar = text[pos.idx];
            else
                currentChar = '\0';
        }

        public List<Token> MakeTokens(bool ignoreErrors = false) {
            List<Token> tokens = new();

            while(currentChar != '\0') {
                if (currentChar == ' ' || currentChar == '\t' || currentChar == '\r')
                {
                    Advance();
                } else if (Token.SEPERATORS.Contains(currentChar)) {
                    tokens.Add(new Token(TokenType.NewLine, posStart: pos));
                    Advance();
                } else if (Token.DIGITS.Contains(currentChar))
                {
                    tokens.Add(MakeNumber());
                }
                else if (Token.LETTERS.Contains(currentChar))
                {
                    tokens.Add(MakeIdentifier());
                } else if (currentChar == '"')
                {
                    tokens.Add(MakeString());
                }
                else if (currentChar == '+')
                {
                    tokens.Add(new Token(TokenType.Plus, posStart: pos));
                    Advance();
                }
                else if (currentChar == '-')
                {
                    tokens.Add(MakeMinusOrArrow());
                }
                else if (currentChar == '*')
                {
                    tokens.Add(new Token(TokenType.Mul, posStart: pos));
                    Advance();
                }
                else if (currentChar == '^')
                {
                    tokens.Add(new Token(TokenType.Pow, posStart: pos));
                    Advance();
                }
                else if (currentChar == '%')
                {
                    tokens.Add(new Token(TokenType.Mod, posStart: pos));
                    Advance();
                }
                else if (currentChar == '/')
                {
                    Token ret = MakeCommentOrDivision();
                    if(ret != null) tokens.Add(ret);
                    Advance();
                } else if (currentChar == '.') {
                    tokens.Add(new Token(TokenType.Dot, posStart: pos));
                    Advance();
                } else if (currentChar == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParen, posStart: pos));
                    Advance();
                }
                else if (currentChar == ')')
                {
                    tokens.Add(new Token(TokenType.RightParen, posStart: pos));
                    Advance();
                }
                else if (currentChar == '[')
                {
                    tokens.Add(new Token(TokenType.LeftSqBracket, posStart: pos));
                    Advance();
                }
                else if (currentChar == ']')
                {
                    tokens.Add(new Token(TokenType.RightSqBracket, posStart: pos));
                    Advance();
                }
                else if (currentChar == '#')
                {
                    tokens.Add(new Token(TokenType.Index, posStart: pos));
                    Advance();
                }
                else if (currentChar == '{')
                {
                    tokens.Add(new Token(TokenType.LeftBraces, posStart: pos));
                    Advance();
                }
                else if (currentChar == '}')
                {
                    tokens.Add(new Token(TokenType.RightBraces, posStart: pos));
                    Advance();
                }
                else if (currentChar == '!')
                {
                    (Token tok, SpaghettoException error) = MakeNotEquals();

                    if (error != null) throw error;
                    tokens.Add(tok);
                }
                else if (currentChar == '=')
                {
                    tokens.Add(MakeEquals());
                }
                else if (currentChar == '<')
                {
                    tokens.Add(MakeLessThan());
                }
                else if (currentChar == '>')
                {
                    tokens.Add(MakeGreaterThan());
                }
                else if (currentChar == ',')
                {
                    tokens.Add(new Token(TokenType.Comma, posStart: pos));
                    Advance();
                }
                else
                {
                    if (ignoreErrors)
                    {
                        tokens.Add(new Token(TokenType.Unknown, currentChar));
                        Advance();
                        continue;
                    }

                    Position posStart = pos.Copy();
                    char chr = currentChar;
                    Advance();

                    throw new IllegalCharError(posStart, pos, $"'{chr}'");
                }
            }

            tokens.Add(new Token(TokenType.EndOfFile, posStart: pos));
            return tokens;
        }

        public Token MakeString() {
            string str = "";
            Position posStart = pos.Copy();
            bool escapeCharacter = false;

            Advance();

            while(currentChar != '\0' && (currentChar != '"' || escapeCharacter)) {
                if (escapeCharacter) {
                    str += (Token.ESCAPE_CHARS.ContainsKey(currentChar) ? Token.ESCAPE_CHARS[currentChar] : currentChar);
                    escapeCharacter = false;
                } else {
                    if (currentChar == '\\') {
                        escapeCharacter = true;
                    } else {
                        str += currentChar;
                    }
                }

                Advance();
            }

            Advance();

            return new Token(TokenType.String, str, posStart, pos);
        }

        public Token MakeMinusOrArrow() {
            TokenType tokenType = TokenType.Minus;
            Position posStart = pos.Copy();

            Advance();

            if(currentChar == '>') {
                Advance();
                tokenType = TokenType.Arrow;
            }

            return new Token(tokenType, posStart, pos);
        }

        public Token MakeCommentOrDivision() {
            TokenType tokenType = TokenType.Div;
            Position posStart = pos.Copy();

            Advance();

            if (currentChar == '/') {
                while(currentChar != '\n' && currentChar != '\0') { Advance(); };

                return null;
            }

            return new Token(tokenType, posStart, pos);
        }

        public Token MakeNumber() {
            string numStr = "";
            int dotCount = 0;
            Position posStart = pos.Copy();

            while(currentChar != '\0' && Token.DIGITS_WITH_SPECIAL.Contains(currentChar)) {
                if(currentChar == '.') {
                    if (dotCount == 1) break;

                    dotCount++;
                    numStr += ".";
                } else {
                    numStr += currentChar;
                }

                Advance();
            }

            bool succededAsFloat = double.TryParse(numStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double resultFloat);

            if (succededAsFloat)
                return new Token(TokenType.Float, resultFloat, posStart, pos);
            else 
                throw new InvalidNumericalValueError(posStart, pos, "Value was not a valid Double.");
        }

        public Token MakeIdentifier() {
            string idStr = "";
            Position posStart = pos.Copy();

            while(currentChar != '\0' && (Token.LETTERS_DIGITS + "_").Contains(currentChar)) {
                idStr += currentChar;
                Advance();
            }

            TokenType tokenType = (Token.KEYWORDS.Contains(idStr) ? TokenType.Keyword : TokenType.Identifier);

            return new Token(tokenType, idStr, posStart, pos);
        }

        public (Token, SpaghettoException) MakeNotEquals() {
            Position posStart = pos.Copy();
            Advance();

            if(currentChar == '=') {
                Advance();
                return (new Token(TokenType.NotEquals, posStart: pos), null);
            }

            Advance();
            return (null, new ExpectedCharError(posStart, pos, "'=' (after '!')"));
        }

        public Token MakeEquals() {
            Position posStart = pos.Copy();
            Advance();

            TokenType tokType = TokenType.Equals;

            if (currentChar == '=') {
                tokType = TokenType.EqualsEquals;
                Advance();
            }

            return new Token(tokType, posStart: posStart, posEnd: pos);
        }

        public Token MakeLessThan() {
            Position posStart = pos.Copy();
            Advance();

            TokenType tokType = TokenType.LessThan;

            if (currentChar == '=') {
                tokType = TokenType.LessThanOrEquals;
            }

            return new Token(tokType, posStart: posStart, posEnd: pos);
        }

        public Token MakeGreaterThan() {
            Position posStart = pos.Copy();
            Advance();

            TokenType tokType = TokenType.GreaterThan;

            if (currentChar == '=') {
                tokType = TokenType.GreaterThanOrEquals;
            }

            return new Token(tokType, posStart: posStart, posEnd: pos);
        }
    }

    internal enum TokenType {
        Int,
        Float,
        Plus,
        Minus,
        Mul,
        Div,
        Pow,
        Mod,
        Index,
        String,
        LeftParen,
        RightParen,
        LeftBraces,
        RightBraces,
        EndOfFile,
        Identifier,
        Keyword,
        Equals,
        EqualsEquals,
        NotEquals,
        LessThan,
        GreaterThan,
        LessThanOrEquals,
        GreaterThanOrEquals,
        Comma,
        Arrow,
        LeftSqBracket,
        RightSqBracket,
        NewLine,
        Dot,
        Unknown
}

    internal class Token {
        public const string DIGITS = "0123456789";
        public const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string SEPERATORS = ";\n";
        public const string LETTERS_DIGITS = LETTERS + DIGITS;
        public const string DIGITS_WITH_SPECIAL = DIGITS + ".";

        public static List<string> KEYWORDS = new() {
            "var", "and", "or", "not", "if", "then", "elseif", "else" , "for", "until", "also", "while", "step", "func", "end", "return", "continue", "break"
        };

        public static Dictionary<char, string> ESCAPE_CHARS = new() {
            { 'n', "\n" }, {'t', "\t"}
        };

        public static Dictionary<TokenType, string> TOKEN_REPRESENTATIONS = new()
        {
            { TokenType.GreaterThanOrEquals, ">=" },
            { TokenType.LessThanOrEquals, "<=" },
            { TokenType.Plus, "+" },
            { TokenType.Minus, "-" },
            { TokenType.Mul, "*" },
            { TokenType.Div, "/" },
            { TokenType.Pow, "^" },
            { TokenType.Mod, "%" },
            { TokenType.Index, "#" },
            { TokenType.LeftParen, "(" },
            { TokenType.RightParen, ")" },
            { TokenType.LeftBraces, "{" },
            { TokenType.RightBraces, "}" },
            { TokenType.LeftSqBracket, "[" },
            { TokenType.RightSqBracket, "]" },
            { TokenType.Equals, "=" },
            { TokenType.EqualsEquals, "==" },
            { TokenType.LessThan, "<" },
            { TokenType.GreaterThan, ">" },
            { TokenType.Comma, "," },
            { TokenType.Arrow, "->" }
        };

        public static (string, bool) GetRepresentation(Token tok)
        {
            if(TOKEN_REPRESENTATIONS.ContainsKey(tok.type)) return (TOKEN_REPRESENTATIONS[tok.type], false);
            if (tok.type == TokenType.String) return ($"\"{tok.value}\"", true);
            return (tok?.value?.ToString(), true);
        }

        public TokenType type;
        public object? value;
        public Position posStart, posEnd;

        public Token(TokenType type, object? value = null, Position posStart = null, Position posEnd = null) {
            this.type = type;
            this.value = value;

            if(posStart != null) {
                this.posStart = posStart.Copy();
                this.posEnd = posStart.Copy();
                this.posEnd.Advance();
            }

            if (posEnd != null) {
                this.posEnd = posEnd.Copy();
            }
        }

        public Token SetPosition(Position posStart, Position posEnd)
        {
            this.posStart = posStart.Copy();
            this.posEnd = posEnd.Copy();

            return this;
        }

        public static implicit operator TokenType(Token t) => t.type;

        public bool Matches(TokenType type, string value) {
            return (this.type == type && (string)this.value == value);
        }

        public override string ToString() {
            if (value != null) return $"{type}:{value}";
            return $"{type}";
        }
    }

    internal class Position {
        public int idx;
        public int ln;
        public int col;
        public string fileName, fileText;

        public Position(int idx, int ln, int col, string fileName, string fileText) {
            this.idx = idx;
            this.ln = ln;
            this.col = col;
            this.fileName = fileName;
            this.fileText = fileText;
        }

        public void Advance(char currentChar = '\0') {
            idx++;
            col++;

            if (currentChar == '\n') {
                ln++;
                col = 0;
            }
        }

        public Position Copy() {
            return new Position(idx, ln, col, fileName, fileText);
        }
    }
}
