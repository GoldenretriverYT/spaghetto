using CSSpaghettoLibBase;
using spaghetto;

public class LibMain : CSSpagLib
{
    public override void Initiliaze(SpaghettoBridge bridge)
    {
        bridge.Register("Form", FormClass.@class);
    }
}