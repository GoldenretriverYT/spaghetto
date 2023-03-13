namespace spaghetto
{
    public class Scope
    {
        public Dictionary<string, object> Table { get; set; } = new();
        public Scope ParentScope { get; set; }

        public Scope() { }
        public Scope(Scope parentScope)
        {
            ParentScope = parentScope;
        }

        public object Get(string key)
        {
            if (Table.ContainsKey(key)) return Table[key];
            
            if(ParentScope == null) return null;
            return ParentScope.Get(key);
        }

        public void Set(string key, object value)
        {
            Table[key] = value;
        }
    }
}