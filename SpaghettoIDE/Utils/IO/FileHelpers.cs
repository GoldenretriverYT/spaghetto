using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaghettoIDE.Utils.IO {
    internal static class FileHelpers {
        public static string ReadAllTextOrDefault(string path, string def) {
            if (!File.Exists(path)) File.WriteAllText(path, def);
            return File.ReadAllText(path);
        }
    }
}
