// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Runtime.InteropServices;
using System.Text;

namespace frosthook.Patches.BC2;

public class Win32ServerR11
{
    private static readonly IntPtr NetworkProtocolServerR11 = new(0x1753911);
    private static readonly byte[] NetworkProtocolOverride = Encoding.ASCII.GetBytes("RETAIL133337");

    public static void OnLoad()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Executable Version: {version}");

        OverrideNetworkProtocol();
        Console.WriteLine("frosthook finished");
    }

    public static void OnHook()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Runtime Version: {version}");
    }

    private static string GetNetworkProtocolVersion()
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