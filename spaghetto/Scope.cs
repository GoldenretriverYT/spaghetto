namespace spaghetto
{
    public class Scope
    {
        public Dictionary<string, SValue> Table { get; set; } = new();
        public Dictionary<string, SValue> ExportTable { get; set; } = new();
        public Scope ParentScope { get; set; }

        public ScopeState State { get; private set; } = ScopeState.None;
        public SValue ReturnValue { get; set; } = SValue.Null;

        public int CreatedPosition { get; private set; }

        public Scope(int startPos) {
            CreatedPosition = startPos;
        }

        public Scope(Scope parentScope, int startPos)
        {
            ParentScope = parentScope;
            CreatedPosition = startPos;
        }

        public SValue Get(string key)
        {
            if (Table.TryGetValue(key, out SValue val)) return val;
            
            if(ParentScope == null) return null;
            return ParentScope.Get(key);
        }

        public void Set(string key, SValue value)
        {
            if (Table.ContainsKey(key)) throw new Exception("Scope.Set can not be used to overwrite values.");
            Table[key] = value;
        }

        public Exception? Update(string key, SValue value) {
            if (Table.TryGetValue(key, out var origVal)) {
                if (origVal.TypeIsFixed &&
                    origVal.BuiltinName != value.BuiltinName) {
                    if(value.BuiltinName == SBuiltinType.ClassInstance)
                        return new InvalidOperationException("A variables type may not change after initilization (Tried to assign " + value.BuiltinName + " to " + origVal.BuiltinName + ")");
                }
                    

                if (origVal.IsConstant) throw new InvalidOperationException("Tried to assign to constant variable.");

                origVal.CopyMeta(ref value);
                Table[key] = value;
                return null;
            }

            if (ParentScope == null) throw new Exception("Could not update field " + key + ": Not found");
            return ParentScope.Update(key, value);
        }

        public bool Update(string key, SValue value, out Exception ex) {
            var updateEx = Update(key, value);

            if(updateEx == null) {
                ex = new Exception();
                return true;
            }else {
                ex = updateEx;
                return false;
            }
        }

        public Scope GetRoot() {
            if (ParentScope == null) return this;
            return ParentScope.GetRoot();
        }

        public void SetState(ScopeState state) {
            State = state;
            ParentScope?.SetState(state);
        }

        public void SetReturnValue(SValue val) {
            ReturnValue = val;
            ParentScope?.SetReturnValue(val);
        }

        /*public string GetScopeStrace() {
            string o = "";
            var current = this;

            while(true) {
                current += ""
            }
        }*/
    }

    public enum ScopeState
    {
        None,
        ShouldBreak,
        ShouldContinue,
        ShouldReturn
    }
}