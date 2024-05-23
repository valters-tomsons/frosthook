namespace frosthook;

public static class FrostHook
{
    private static readonly ThreadStart HookThread = new(Initialize);

    // private static nint _threadId = 0;

    public static void Enable()
    {
        // Kernel32.CreateThread(IntPtr.Zero, 0, HookThread, IntPtr.Zero, 0, out _threadId);
        // Console.WriteLine($"threadId created = 0x{_threadId:X0}");
        // Kernel32.CloseHandle(_threadId);

        Initialize();
    }

    private static void Initialize()
    {
        var version = Patches.BC2.Win32ServerR11.GetExecutableNetworkProtocol();
        Patches.BC2.Win32ServerR11.OverrideNetworkProtocol();
        Console.WriteLine($"Executable Version: {version}");
        Console.WriteLine("frosthook finished");
    }
}