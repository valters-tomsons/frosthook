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
            Console.WriteLine($"frosthook attached, hModule = 0x{hModule:X0}, 0x{lpReserved:X0}");
            UnprotectProcess();

            FrostHook.OnLoad();
            FrostHook.Start();
        }

        return true;
    }

    private static void UnprotectProcess()
    {
        using var currentProcess = Process.GetCurrentProcess();
        using var module = currentProcess.MainModule;

        if (module == null)
        {
            Console.WriteLine("Failed to get main module, goodbye!");
            throw new Exception();
        }

        Console.WriteLine($"{module.ModuleName}, 0x{module.BaseAddress:X0}");
        var success = Kernel32.VirtualProtect(module.BaseAddress, (uint)module.ModuleMemorySize, MemoryProtection.PAGE_EXECUTE_READWRITE, out var oldProtection);
        Console.WriteLine($"{nameof(UnprotectProcess)} = {success}, {oldProtection}");
    }
}