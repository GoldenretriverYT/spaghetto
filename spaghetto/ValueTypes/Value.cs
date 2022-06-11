using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    [Serializable]
    internal abstract class Value {
        public object value;
        public Position posStart = null, posEnd = null;
        public Context context = null;

        public List<BaseFunction> instanceFunctions = new();

        public Value SetPosition(Position posStart = null, Position posEnd = null) {
            this.posStart = posStart;
            this.posEnd = posEnd;

            return this;
        }

        public Value SetContext(Context ctx) {
            this.context = ctx;

            return this;
        }

        public abstract Value Copy();

        public virtual (Value, SpaghettoException) AddedTo(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support AddedTo-Operator", context));
        }

        public virtual (Value, SpaghettoException) SubtractedBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support SubtractedBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) MultipliedBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support MultipliedBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) DividedBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support DividedBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) PoweredBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support POW-Operator", context));
        }
        public virtual (Value, SpaghettoException) Modulo(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support MOD-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsEqualTo(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsEqual-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsNotEqualTo(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsNotEqual-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsGreaterThan(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsGreaterThan-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsGreaterThanOrEquals(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsGreaterThanOrEquals-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsLessThan(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsLessThan-Operator", context));
        }

        public virtual (Value, SpaghettoException) IsLessThanOrEquals(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IsLessThanOrEquals-Operator", context));
        }

        public virtual (Value, SpaghettoException) AndBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support AndBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) OrBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support OrBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) IndexedBy(Value other) {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support IndexedBy-Operator", context));
        }

        public virtual (Value, SpaghettoException) Not() {
            return (null, new RuntimeError(posStart, posEnd, this.GetType().Name + " does not support Not-Operator", context));
        }

        public abstract string Represent();

        public virtual bool IsTrue() {
            return false;
        }

        public virtual RuntimeResult Execute(List<Value> args) {
            return new RuntimeResult().Failure(new RuntimeError(posStart, posEnd, "Unable to execute " + this.GetType().Name, context));
        }
    }
}
