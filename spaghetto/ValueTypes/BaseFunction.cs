using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    public abstract class BaseFunction : Value {
        public abstract bool IsStatic { get; set; }
        public abstract List<string> ArgNames { get; set; }

        public virtual BaseFunction SetStatic(bool st)
        {
            IsStatic = st;
            return this;
        }
    }
}
