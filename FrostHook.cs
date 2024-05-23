using System.Runtime.InteropServices;

namespace frosthook;

public static class FrostHook
{
    private static readonly IntPtr NetworkProtocolServerR11 = new(0x1753911);

    private static readonly ThreadStart HookThread = new(Initialize);
    private static nint _threadId = 0;

    public static void Enable()
    {
        Kernel32.CreateThread(IntPtr.Zero, 0, HookThread, IntPtr.Zero, 0, out _threadId);
        Console.WriteLine($"threadId created = 0x{_threadId:X0}");
    }

    private unsafe static void Initialize()
    {
        var versionString = Marshal.PtrToStringAnsi(NetworkProtocolServerR11);
        Console.WriteLine($"Executable Version: {versionString}");

        Kernel32.CloseHandle(_threadId);
    }
}