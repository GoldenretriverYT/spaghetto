using spaghetto.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.Helpers {
    public static class ListExtensions {
        public static int GetEndingPosition(this IEnumerable<SyntaxNode> list, int fallback) {
            if (list.Count() == 0) return fallback;
            return list.Last().EndPosition;
        }

        public static int GetStartingPosition(this IEnumerable<SyntaxNode> list, int fallback) {
            if (list.Count() == 0) return fallback;
            return list.First().StartPosition;
        }

        // TODO: Maybe use an interface instead
        public static int GetEndingPosition(this IEnumerable<SyntaxToken> list, int fallback) {
            if (list.Count() == 0) return fallback;
            return list.Last().EndPosition;
        }

        public static int GetStartingPosition(this IEnumerable<SyntaxToken> list, int fallback) {
            if (list.Count() == 0) return fallback;
            return list.First().Position;
        }


    }
}
