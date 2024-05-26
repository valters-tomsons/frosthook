using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace frosthook;

public static class EntryPoint
{
    [UnmanagedCallersOnly(EntryPoint = nameof(DllMain), CallConvs = [typeof(CallConvStdcall)])]
    public static bool DllMain(IntPtr hModule, FwdReason ul_reason_for_call, IntPtr lpReserved)
    {
        if (FwdReason.DLL_PROCESS_ATTACH == ul_reason_for_call)
        {
            Console.WriteLine($"frosthook loading, hModule = 0x{hModule:X0}, 0x{lpReserved:X0}");
            Kernel32.DisableThreadLibraryCalls(hModule);

            using (var currentProcess = Process.GetCurrentProcess())
            {
                using var module = currentProcess.MainModule!;
                Console.WriteLine($"frosthook attaching to: {module.ModuleName}, 0x{module.BaseAddress:X0}");
            }

            FrostHook.OnAttach();
        }

        return true;
    }
}