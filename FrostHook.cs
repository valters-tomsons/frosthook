using System.Diagnostics;

namespace frosthook;

public static partial class FrostHook
{
    public static void UnprotectCurrentProcess()
    {
        using var currentProcess = Process.GetCurrentProcess();
        using var module = currentProcess.MainModule;

        if (module == null)
        {
            Console.WriteLine("Failed to get main module, goodbye!");
            throw new NotImplementedException();
        }

        Console.WriteLine($"MainModule = {module.ModuleName}, 0x{module.BaseAddress:X0}");
        var success = Kernel32.VirtualProtect(module.BaseAddress, (uint)module.ModuleMemorySize, MemoryProtection.PAGE_EXECUTE_READWRITE, out uint oldProtection);
        Console.WriteLine($"{nameof(UnprotectCurrentProcess)} = {success}, {oldProtection}");
    }
}
