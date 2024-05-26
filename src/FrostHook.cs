using System.Diagnostics;

namespace frosthook;

public static class FrostHook
{
    static nint ThreadHandle = 0;
    static readonly ThreadStart PayloadStart = new(RunPayloadThread);

    public static void OnAttach(IntPtr hModule)
    {
        using (var currentProcess = Process.GetCurrentProcess())
        {
            using var module = currentProcess.MainModule!;
            Console.WriteLine($"frosthook attaching to: {module.ModuleName}, 0x{module.BaseAddress:X0}");
        }

        Patches.BC2.Win32ServerR11.Initialize();

        // Start the payload thread after DllMain exits
        ThreadHandle = Kernel32.CreateThread(IntPtr.Zero, 0, PayloadStart, IntPtr.Zero, 0, out var threadId);
        Console.WriteLine($"frosthook thread created = 0x{ThreadHandle:X0}, 0x{threadId:X0}");
    }

    static void RunPayloadThread()
    {
        Patches.BC2.Win32ServerR11.DoRuntimeStuff();

        Console.WriteLine($"frosthook thread closing = 0x{ThreadHandle:X0}");
        Kernel32.CloseHandle(ThreadHandle);
    }
}