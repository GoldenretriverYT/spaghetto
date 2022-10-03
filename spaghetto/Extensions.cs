using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public static class Extensions {
        public static string Join<T>(this List<T> list, string sep) {
            string o = "";
            for (int i = 0; i < list.Count; i++) o += (i == list.Count - 1 ? list[i].ToString() : list[i].ToString() + sep);
            return o;
        }
    }
}
