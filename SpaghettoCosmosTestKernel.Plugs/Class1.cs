using Cosmos.HAL.BlockDevice;
using IL2CPU.API.Attribs;
using System.Reflection.Emit;
using XSharp;
using XSharp.Assembler;
using static XSharp.XSRegisters;

namespace SpaghettoCosmosTestKernel.Plugs {
    [Plug(Target = typeof(Trace))]
    public class Plugs {
        [PlugMethod(Assembler = typeof(DoTraceAssembler))]
        public static void DoTrace() {
            throw new NotImplementedException();
        }
    }

    public class DoTraceAssembler : AssemblerMethod {
        public override void AssembleNew(Assembler aAssembler, object aMethodInfo) {
            XS.Push(EAX);
            XS.Push(EBX);
            XS.Call(AsmMarker.Labels[AsmMarker.Type.DebugStub_SendSimpleNumber]);
            XS.Add(ESP, 4);
            XS.Call(AsmMarker.Labels[AsmMarker.Type.DebugStub_SendSimpleNumber]);

            XS.ClearInterruptFlag();
            // don't remove the call. It seems pointless, but we need it to retrieve the EIP value
            XS.Call(xLabel + ".StackCorruptionCheck_GetAddress");
            XS.Label(xLabel + ".StackCorruptionCheck_GetAddress");
            XS.Exchange(BX, BX);
            XS.Pop(EAX);
            XS.Set(AsmMarker.Labels[AsmMarker.Type.DebugStub_CallerEIP], EAX, destinationIsIndirect: true);
            XS.Call(AsmMarker.Labels[AsmMarker.Type.DebugStub_SendStackCorruptedEvent]);
            XS.Halt();
            XS.Label(xLabel + ".StackCorruptionCheck_End");
        }
    }
}