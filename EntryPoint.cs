using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace frosthook;

public partial class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(DllMain), CallConvs = [typeof(CallConvStdcall)])]
    public static bool DllMain(IntPtr hModule, uint ul_reason_for_call, IntPtr lpReserved)
    {
        if ((uint)FwdReason.DLL_PROCESS_ATTACH == ul_reason_for_call)
        {
            Console.WriteLine("frosthook attaching");
        }

        return true;
    }
}