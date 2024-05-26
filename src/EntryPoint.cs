using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace frosthook;

public static class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(DllMain), CallConvs = [typeof(CallConvStdcall)])]
    public static bool DllMain(IntPtr hModule, FwdReason ul_reason_for_call, IntPtr _)
    {
        if (FwdReason.DLL_PROCESS_ATTACH == ul_reason_for_call)
        {
            Kernel32.DisableThreadLibraryCalls(hModule);
            FrostHook.OnAttach(hModule);
        }

        return true;
    }
}