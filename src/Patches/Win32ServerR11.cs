// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;

namespace frosthook.Patches.BC2;

public static class Win32ServerR11
{
    const int NetworkProtocolStringLength = 12;
    static readonly IntPtr NetworkProtocolOffset = new(0x1753911);
    static readonly byte[] ProtocolOverride = Encoding.ASCII.GetBytes("RETAIL133337");

    public static unsafe void Initialize()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Executable Version: {version}");

        OverrideNetworkProtocol();
        Console.WriteLine("frosthook finished");
    }

    public static void DoRuntimeStuff()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Runtime Version: {version}");
    }

    static string GetNetworkProtocolVersion()
    {
        return Marshal.PtrToStringAnsi(NetworkProtocolOffset, NetworkProtocolStringLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    unsafe static void OverrideNetworkProtocol()
    {
        var offset = (nuint)NetworkProtocolOffset;
        var memory = Memory.Instance;

        using var stringProtect = memory.ChangeProtectionDisposable(offset, NetworkProtocolStringLength, Reloaded.Memory.Enums.MemoryProtection.Write);
        memory.WriteRaw(offset, ProtocolOverride);
    }
}