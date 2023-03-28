namespace spaghetto.BuiltinTypes {
    public abstract class SBaseFunction : SValue {
        /// <summary>
        /// If this is true, the first argument should be the instance
        /// </summary>
        public bool IsClassInstanceMethod { get; set; }

        public List<string> ExpectedArgs { get; set; }
    }
}
