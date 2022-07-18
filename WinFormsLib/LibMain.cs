using CSSpaghettoLibBase;
using spaghetto;

namespace WinFormsLib;

public class LibMain : CSSpagLib
{
    public override void Initiliaze(SpaghettoBridge bridge)
    {
        bridge.Register("Form", FormClass.@class);
    }
}