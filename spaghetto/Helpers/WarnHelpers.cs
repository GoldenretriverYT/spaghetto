using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Helpers {
    public class WarnHelpers {
        public readonly static List<string> AlreadyPrintedWarnings = new();

        /// <summary>
        /// Prints the warning once at most. This does not use a warnClass <see cref="PrintOnce(string, string)"></see>
        /// which allows you to print different variations multiple times
        /// </summary>
        /// <param name="warn"></param>
        public static void PrintOnce(string warn) {
            PrintOnce(warn, warn);
        }

        /// <summary>
        /// Prints the warning once at most. warnClass is used to check for duplicates.
        /// </summary>
        /// <param name="warn"></param>
        /// <param name="warnClass"></param>
        public static void PrintOnce(string warn, string warnClass) {
            if (AlreadyPrintedWarnings.Contains(warnClass)) return;
            AlreadyPrintedWarnings.Add(warnClass);

            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warn);
            Console.ForegroundColor = previousColor;
        }
    }
}
