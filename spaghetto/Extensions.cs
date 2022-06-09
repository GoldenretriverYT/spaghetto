using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal static class Extensions {
        public static string Join<T>(this List<T> list, string sep) {
            string o = "";
            for (int i = 0; i < list.Count; i++) o += (i == list.Count() - 1 ? list[i] : list[i] + sep);
            return o;
        }
    }
}
