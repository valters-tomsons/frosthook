// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using frosthook.Frostbite;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sources;

namespace frosthook.Patches.BC2;

public static class Win32ServerR11
{
    // Network Protocol Patch
    const int NetworkProtocolLength = 12;
    static readonly IntPtr NetworkVersionAddress = new(0x1753911);
    static readonly IntPtr NetworkProtocolIdAddress = new(0x1753921);

    const int BuildIdLength = 6;
    static readonly IntPtr BuildIdAddress = new(0x1754b90);

    static readonly IntPtr GetGameProtocolVersion = new(0x0132e0f0);
    static IHook<GetGameProtocolVersion>? GetGameProtocolVersionHook;


    // Ignore Trimmer warnings for now, they're coming from the hooking library, but seem to work fine at runtime.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "<Pending>")]
    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "<Pending>")]
    public static unsafe void Apply()
    {
        var netVersion = GetNetworkVersion();
        var protocolId = GetNetworkProtocolId();
        var buildId = GetBuildId();
        FrostHook.LogLine($"Network.Version: {netVersion}");
        FrostHook.LogLine($"networkProtocolId: {protocolId}");
        FrostHook.LogLine($"buildId: {buildId}");

        // OverwriteString(NetworkVersionAddress, NetworkProtocolLength, "RETAIL511118");
        // OverwriteString(NetworkProtocolIdAddress, NetworkProtocolLength, "RETAIL515757");

        GetGameProtocolVersionHook = new Hook<GetGameProtocolVersion>(GetGameProtocolVersionImpl, (nuint)GetGameProtocolVersion).Activate();
    }

    static string GetNetworkVersion()
    {
        return Marshal.PtrToStringAnsi(NetworkVersionAddress, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    static string GetNetworkProtocolId()
    {
        return Marshal.PtrToStringAnsi(NetworkProtocolIdAddress, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    static string GetBuildId()
    {
        return Marshal.PtrToStringAnsi(BuildIdAddress, BuildIdLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    static void OverwriteString(IntPtr address, int length, string value)
    {
        FrostHook.LogLine($"overwriting string at 0x{address:X0} -> {value}");

        var addr = (nuint)address;
        var memory = Memory.Instance;

        var patch = Encoding.ASCII.GetBytes(value);
        if (patch.Length != length)
        {
            FrostHook.LogLine("[WARNING] String patch length mismatch, here be dragons!");
        }

        var oldPerm = memory.ChangePermission(addr, length, Reloaded.Memory.Kernel32.Kernel32.MEM_PROTECTION.PAGE_READWRITE);
        memory.WriteRaw(addr, patch);
        memory.ChangePermission(addr, length, oldPerm);
    }

    static string GetGameProtocolVersionImpl(IntPtr @this)
    {
        var ver = GetGameProtocolVersionHook!.OriginalFunction(@this);
        FrostHook.LogLine($"GetGameProtocolVersion: {ver}");
        return ver;
    }
}