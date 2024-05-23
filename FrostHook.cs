namespace frosthook;

public static class FrostHook
{
    private static readonly ThreadStart HookThread = new(Initialize);
    private static nint _threadId = 0;

    public static void Enable()
    {
        Kernel32.CreateThread(IntPtr.Zero, 0, HookThread, IntPtr.Zero, 0, out _threadId);
        Console.WriteLine($"threadId created = 0x{_threadId:X0}");
    }

    private static void Initialize()
    {
        Console.WriteLine("frosthook thread says hellolize!");
        Thread.Sleep(500);
        Console.WriteLine("frosthook cleaning up!");
        Kernel32.CloseHandle(_threadId);
    }
}