namespace frosthook;

public static class FrostHook
{
    static nint PayloadThreadId = 0;
    static readonly ThreadStart PayloadStart = new(RunPayloadThread);

    public static void OnAttach()
    {
        // ! Loader Lock is active here, be warned!
        Patches.BC2.Win32ServerR11.Initialize();

        Kernel32.CreateThread(IntPtr.Zero, 0, PayloadStart, IntPtr.Zero, 0, out PayloadThreadId);
        Console.WriteLine($"frosthook threadId created = 0x{PayloadThreadId:X0}");
    }

    static void RunPayloadThread()
    {
        Patches.BC2.Win32ServerR11.DoRuntimeStuff();

        Console.WriteLine($"frosthook threadId closing = 0x{PayloadThreadId:X0}");
        Kernel32.CloseHandle(PayloadThreadId);
    }
}