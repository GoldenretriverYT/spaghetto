namespace spaghetto
{
    public class Scope
    {
        public Dictionary<string, SValue> Table { get; set; } = new();
        public Scope ParentScope { get; set; }

        public ScopeState State { get; set; } = ScopeState.None;
        public SValue ReturnValue { get; set; } = SValue.Null;

        public Scope() { }
        public Scope(Scope parentScope)
        {
            ParentScope = parentScope;
        }

        public SValue Get(string key)
        {
            if (Table.ContainsKey(key)) return Table[key];
            
            if(ParentScope == null) return null;
            return ParentScope.Get(key);
        }

        public void Set(string key, SValue value)
        {
            Table[key] = value;
        }
    }

    public enum ScopeState
    {
        None,
        ShouldBreak,
        ShouldContinue,
        ShouldReturn
    }
}