using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto.BuiltinTypes {
    public abstract class SBaseFunction : SValue {
        /// <summary>
        /// If this is true, the first argument should be the instance
        /// </summary>
        public bool IsClassInstanceMethod { get; set; }
    }
}
