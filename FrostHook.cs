using frosthook.Patches.BC2;

namespace frosthook;

public static class FrostHook
{
    private static readonly ThreadStart HookThreadInfo = new(OnHook);

    private static nint _threadId = 0;

    public static void OnLoad()
    {
        Console.WriteLine("frosthook -- blocking on load");

        Win32ServerR11.OnLoad();
    }

    public static void Start()
    {
        Kernel32.CreateThread(IntPtr.Zero, 0, HookThreadInfo, IntPtr.Zero, 0, out _threadId);
        Console.WriteLine($"threadId created = 0x{_threadId:X0}");
    }

    private static void OnHook()
    {
        Win32ServerR11.OnHook();

        Console.WriteLine($"threadId closing = 0x{_threadId:X0}");
        Kernel32.CloseHandle(_threadId);
    }
}