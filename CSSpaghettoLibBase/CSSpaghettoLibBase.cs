namespace CSSpaghettoLibBase
{
    public abstract class CSSpagLib
    {
        public abstract void Initiliaze(SpaghettoBridge bridge);
    }

    public class SpaghettoBridge
    {
        public void Register(string name, spaghetto.Value value)
        {
            spaghetto.Intepreter.globalSymbolTable.Add(name, value);
        }
    }
}