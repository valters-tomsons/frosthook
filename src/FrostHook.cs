namespace frosthook;

public static class FrostHook
{
    static nint PayloadThreadId = 0;
    static readonly ThreadStart PayloadStart = new(RunPayload);

    public static void OnAttach()
    {
        // ! Loader Lock is active here, be warned!
        Console.WriteLine("frosthook: OnAttach()");

        Patches.BC2.Win32ServerR11.PatchBinary();

        Kernel32.CreateThread(IntPtr.Zero, 0, PayloadStart, IntPtr.Zero, 0, out PayloadThreadId);
        Console.WriteLine($"threadId created = 0x{PayloadThreadId:X0}");
    }

    static void RunPayload()
    {
        Patches.BC2.Win32ServerR11.DoRuntimeStuff();

        Console.WriteLine($"threadId closing = 0x{PayloadThreadId:X0}");
        Kernel32.CloseHandle(PayloadThreadId);
    }
}