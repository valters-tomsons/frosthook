// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using frosthook.Frostbite;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Sources;

namespace frosthook.Patches.BC2;

public static class Win32ServerR11
{
    // Network Protocol Patch
    const int NetworkProtocolLength = 12;
    static readonly IntPtr NetworkProtocolOffset = new(0x1753911);
    static readonly byte[] NetworkProtocolPatch = Encoding.ASCII.GetBytes("RETAIL133337");

    static readonly IntPtr DispatchMessageOffset = new(0x0042eeb0);
    static IHook<DispatchMessage>? DispatchMessageHook;

    static readonly IntPtr GetPreemptiveDenyReasonAddress = new(0x012c1e00);
    static IHook<GetPreemptiveDenyReason>? GetPreemptiveDenyReasonHook;

    static IntPtr AllowPlayerEntryInternalAddress = new IntPtr(0x012E88C0);
    static IHook<AllowPlayerEntryInternal>? AllowPlayerEntryInternalHook;


    static IHook<CreateFileA>? CreateFileAHook;

    // Ignore Trimmer warnings for now, they're coming from the hooking library, but seem to work fine at runtime.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "<Pending>")]
    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "<Pending>")]
    public static unsafe void Initialize()
    {
        var version = GetNetworkProtocolVersion();
        FrostHook.LogLine($"Executable Version: {version}");
        OverrideNetworkProtocol();

        version = GetNetworkProtocolVersion();
        FrostHook.LogLine($"Runtime Version: {version}");

        var kernel32Handle = Kernel32.GetModuleHandleW(Kernel32.LibraryName); 
        var fileCreatePointer = Kernel32.GetProcAddress(kernel32Handle, "CreateFileA");
        CreateFileAHook = new Hook<CreateFileA>(CreateFileAImpl, (nuint)fileCreatePointer).Activate();

        // DispatchMessageHook = new Hook<DispatchMessage>(DispatchMessageImpl, (nuint)DispatchMessageOffset).Activate();
        GetPreemptiveDenyReasonHook = new Hook<GetPreemptiveDenyReason>(GetPreemptiveDenyReasonImpl, (nuint)GetPreemptiveDenyReasonAddress).Activate();
        AllowPlayerEntryInternalHook = new Hook<AllowPlayerEntryInternal>(AllowPlayerEntryInternalImpl, (nuint)AllowPlayerEntryInternalAddress).Activate();
    }

    static string GetNetworkProtocolVersion()
    {
        return Marshal.PtrToStringAnsi(NetworkProtocolOffset, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    static unsafe void OverrideNetworkProtocol()
    {
        var offset = (nuint)NetworkProtocolOffset;
        var memory = Memory.Instance;

        var oldPerm = memory.ChangePermission(offset, NetworkProtocolLength, Reloaded.Memory.Kernel32.Kernel32.MEM_PROTECTION.PAGE_READWRITE);
        memory.WriteRaw(offset, NetworkProtocolPatch);
        memory.ChangePermission(offset, NetworkProtocolLength, oldPerm);
    }

    static IntPtr CreateFileAImpl(string filename, FileAccess access, FileShare share, IntPtr securityAttributes, FileMode creationDisposition, FileAttributes flagsAndAttributes, IntPtr templateFile)
    {
        FrostHook.LogLine($"[CFA] Opening File {filename}");
        return CreateFileAHook!.OriginalFunction(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
    }

    static void DispatchMessageImpl(IntPtr @this, IntPtr message)
    {
        FrostHook.LogLine($"msg dispatch, MessageManager: 0x{@this:X0}, Message: 0x{message:X0}");
        DispatchMessageHook!.OriginalFunction(@this, message);
    }

    static int GetPreemptiveDenyReasonImpl(
        IntPtr @this,
        GamePlayerType playerType,
        string playerName,
        int playerRef,
        bool alreadyJoining)
    {
        var denyReason = GetPreemptiveDenyReasonHook!.OriginalFunction(@this, playerType, playerName, playerRef, alreadyJoining);
        FrostHook.LogLine($"GetPreemptiveDenyReason(): {playerType}, {playerName}, {playerRef}, {alreadyJoining}: {denyReason}");
        return denyReason;
    }

    static unsafe void AllowPlayerEntryInternalImpl(IntPtr @this, IntPtr playerPtr, bool allow, int reason)
    {
        var player = (GameManagerPlayerImpl*)playerPtr;

        FrostHook.LogLine($"AllowPlayerEntryInternal(): {allow}, {reason}");
        FrostHook.LogLine($"Player: {player->PlayerType} {player->PlayerState}");
        AllowPlayerEntryInternalHook!.OriginalFunction(@this, playerPtr, allow, reason);
        FrostHook.LogLine($"Player: {player->PlayerType} {player->PlayerState}");
    }
}