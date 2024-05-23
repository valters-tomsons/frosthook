using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace frosthook;

public static class EntryPoint
{
    private static readonly ThreadStart HookThread = new(FrostHook.Initialize);
    private static uint _hookThreadId = 0;

    [UnmanagedCallersOnly(EntryPoint = nameof(DllMain), CallConvs = [typeof(CallConvStdcall)])]
    public static bool DllMain(IntPtr hModule, uint ul_reason_for_call, IntPtr lpReserved)
    {
        if ((uint)FwdReason.DLL_PROCESS_ATTACH == ul_reason_for_call)
        {
            Console.WriteLine($"frosthook attaching, hModule = 0x{hModule:X0}");

            UnprotectCurrentProcess();
            Kernel32.CreateThread(IntPtr.Zero, 0, HookThread, IntPtr.Zero, 0, out _hookThreadId);
            Console.WriteLine($"threadId created = 0x{_hookThreadId:X0}");
        }

        return true;
    }

    private static void UnprotectCurrentProcess()
    {
        using var currentProcess = Process.GetCurrentProcess();
        using var module = currentProcess.MainModule;

        if (module == null)
        {
            Console.WriteLine("Failed to get main module, goodbye!");
            throw new Exception();
        }

        Console.WriteLine($"MainModule = {module.ModuleName}, 0x{module.BaseAddress:X0}");
        var success = Kernel32.VirtualProtect(module.BaseAddress, (uint)module.ModuleMemorySize, MemoryProtection.PAGE_EXECUTE_READWRITE, out uint oldProtection);
        Console.WriteLine($"{nameof(UnprotectCurrentProcess)} = {success}, {oldProtection}");
    }
}