namespace spaghetto {
    public class SNativeFunction : SValue {
        public override SBuiltinType BuiltinName => SBuiltinType.NativeFunc;
        public Func<Scope, List<SValue>, SValue> Impl { get; set; }

        public SNativeFunction(Func<Scope, List<SValue>, SValue> impl) {
            Impl = impl;
        }

        /// <summary>
        /// NOTE: The scope in SNativeFunction is the calling scope, but not in SFunction!
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override SValue Call(Scope scope, List<SValue> args) {
            return Impl(scope, args);
        }

        public override bool IsTruthy() {
            return true;
        }
    }
}
