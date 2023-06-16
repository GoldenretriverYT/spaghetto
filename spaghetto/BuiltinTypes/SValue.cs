using System.Runtime.CompilerServices;

namespace spaghetto {
    public abstract class SValue
    {
        public static SValue Null => new SNull();
        public static SError Error => new SError();

        public abstract SBuiltinType BuiltinName { get; }

        #region Metadata
        public bool TypeIsFixed { get; set; } = true;
        public bool IsConstant { get; set; } = false;
        #endregion

        public virtual SValue Add(SValue other)
        {
            return NotSupportedOn("Add");
        }

        public virtual SValue Sub(SValue other)
        {
            return NotSupportedOn("Sub");
        }

        public virtual SValue Mul(SValue other)
        {
            return NotSupportedOn("Mul");
        }

        public virtual SValue Div(SValue other)
        {
            return NotSupportedOn("Div");
        }

        public virtual SValue Mod(SValue other)
        {
            return NotSupportedOn("Mod");
        }

        public virtual SValue Idx(SValue other)
        {
            return NotSupportedOn("Idx");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual SValue Dot(SValue other) {
            return NotSupportedOn("Dot");
        }

        public virtual SValue DotAssignment(SValue key, SValue value) {
            return NotSupportedOn("DotAssignment");
        }

        // TODO: Maybe force equals to be implemented?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual SValue Equals(SValue other) {
            return NotSupportedOn("Equals");
        }

        public virtual SValue LessThan(SValue other) {
            return NotSupportedOn("LessThan");
        }

        public virtual SValue LessThanEqu(SValue other) {
            return NotSupportedOn("LessThanEqu");
        }

        public virtual SValue GreaterThan(SValue other) {
            return NotSupportedOn("GreaterThan");
        }

        public virtual SValue GreaterThanEqu(SValue other) {
            return NotSupportedOn("GreaterThanEqu");
        }

        public virtual SValue CastToBuiltin(SBuiltinType other) {
            return NotSupportedOn("CastToBuiltin");
        }

        public virtual SValue Call(Scope scope, List<SValue> args) {
            return NotSupportedOn("Call");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool IsNull() {
            return false;
        }

        public virtual SValue Not()
        {
            return NotSupportedOn("Not");
        }

        public virtual SValue ArithNot()
        {
            return NotSupportedOn("ArithNot");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool IsTruthy();

        public override string ToString()
        {
            return SBuiltinTypeHelper.ToStr(BuiltinName);
        }

        public virtual SString ToSpagString()
        {
            return new SString("<unknown of type " + SBuiltinTypeHelper.ToStr(BuiltinName) + ">");
        }

        protected SValue NotSupportedBetween(SValue other, string type) {
            return Scope.Error(type + " not supported between " + SBuiltinTypeHelper.ToStr(BuiltinName) + " and " + SBuiltinTypeHelper.ToStr(other.BuiltinName));
        }

        protected SValue NotSupportedOn(string type) {
            return Scope.Error(type + " is not supported on " + SBuiltinTypeHelper.ToStr(BuiltinName));
        }

        protected SValue CastInvalid(string type) {
            return Scope.Error(SBuiltinTypeHelper.ToStr(BuiltinName) + " can not be cast to " + type);
        }

        internal void CopyMeta(ref SValue other) {
            other.TypeIsFixed = TypeIsFixed;
            other.IsConstant = IsConstant;
        }

        public string SpagToCsString() {
            return ToSpagString().Value;
        }
    }
}
