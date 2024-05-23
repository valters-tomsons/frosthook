using System.Runtime.InteropServices;
using System.Text;

namespace frosthook;

public static class FrostHook
{
    private static readonly ThreadStart HookThread = new(Initialize);
    private static readonly IntPtr NetworkProtocolServerR11 = new(0x1753911);
    private static readonly byte[] NetworkProtocolOverride = Encoding.ASCII.GetBytes("RETAIL444442");

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
        var version = GetExecutableNetworkProtocol();
        OverrideNetworkProtocol();
        Console.WriteLine($"Executable Version: {version}");
        Console.WriteLine("frosthook finished");
    }

    private static string GetExecutableNetworkProtocol()
    {
        return Marshal.PtrToStringAnsi(NetworkProtocolServerR11)?.TrimEnd('"') ?? throw new Exception("Failed to read version, goodbye!");
    }

    private unsafe static void OverrideNetworkProtocol()
    {
        byte* ptr = (byte*)NetworkProtocolServerR11.ToPointer();
        for (int i = 0; i < NetworkProtocolOverride.Length; i++)
        {
            ptr[i] = NetworkProtocolOverride[i];
        }
    }
}