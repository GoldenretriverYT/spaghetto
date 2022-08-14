using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public class ErrorPointer {
        public static string GenerateErrorPointer(string text, Position posStart, Position posEnd) {
            if (posEnd == null)
            {
                posEnd = new Position(text.Length - 1, posStart.ln, posStart.col, posStart.fileName, posStart.fileName);
                Console.WriteLine("[Warn] posEnd was null");
            }

            string result = "";

            int idxStart = Math.Max(text.Substr(0, posStart.idx).LastIndexOf('\n'), 0);
            int idxEnd = text.IndexOf('\n', Math.Min(idxStart + 1, text.Length));

            if (idxEnd < 0) idxEnd = text.Length;

            int lineCount = posEnd.ln - posStart.ln + 1;
            
            for(int i = 0; i < lineCount; i++) {
                string line = text.Substr(idxStart, idxEnd);
                int colStart = (i == 0 ? posStart.col : 0);
                int colEnd = (i == lineCount - 1 ? posEnd.col : line.Length - 1);

                result += line + "\n";
                result += " ".Repeat(colStart) + "^".Repeat(colEnd - colStart);

                idxStart = idxEnd;
                idxEnd = text.IndexOf("\n", Math.Min(idxStart + 1, text.Length));

                if (idxEnd < 0) idxEnd = text.Length;
            }

            return result.Replace("\t", "");
        }
    }

    public static class StringExtensions {
        public static string Substr(this string str, int start, int end) {
            start = Math.Max(0, start);
            end = Math.Min(str.Length, end);
            return str[start..end];
        }

        public static string SubstrInBounds(this string str, int start, int end)
        {
            return str.Substring(Math.Min(str.Length, Math.Max(0, start)), Math.Max(0, Math.Min(str.Length - start, end - start)));
        }

        public static string Repeat(this string str, int c) {
            string o = "";
            for (int i = 0; i < c; i++) o += str;
            return o;
        }
    }
}
