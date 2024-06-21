// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Diagnostics;
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

    static readonly IntPtr DispatchMessageAddress = new(0x0042eeb0);
    static IHook<DispatchMessage>? DispatchMessageHook;

    static readonly IntPtr GetPreemptiveDenyReasonAddress = new(0x012c1e00);
    static IHook<GetPreemptiveDenyReason>? GetPreemptiveDenyReasonHook;

    static readonly IntPtr AllowPlayerEntryInternalAddress = new(0x012e88C0);
    static IHook<AllowPlayerEntryInternal>? AllowPlayerEntryInternalHook;

    static readonly IntPtr HandleEnterGameHostRequestAddress = new(0x012e8fa0);
    static IHook<HandleEnterGameHostRequest>? HandleEnterGameHostRequestHook;


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

        // OverrideNetworkProtocol();
        OverwriteString(NetworkVersionAddress, NetworkProtocolLength, "RETAIL511118");
        OverwriteString(NetworkProtocolIdAddress, NetworkProtocolLength, "RETAIL515757");

        // DispatchMessageHook = new Hook<DispatchMessage>(DispatchMessageImpl, (nuint)DispatchMessageOffset).Activate();
        // GetPreemptiveDenyReasonHook = new Hook<GetPreemptiveDenyReason>(GetPreemptiveDenyReasonImpl, (nuint)GetPreemptiveDenyReasonAddress).Activate();
        // AllowPlayerEntryInternalHook = new Hook<AllowPlayerEntryInternal>(AllowPlayerEntryInternalImpl, (nuint)AllowPlayerEntryInternalAddress).Activate();
        // GetReasonHook = new Hook<GetReason>(GetReasonImpl, (nuint)GetReasonAddress).Activate();
        HandleEnterGameHostRequestHook = new Hook<HandleEnterGameHostRequest>(HandleEnterGameHostRequestImpl, (nuint)HandleEnterGameHostRequestAddress).Activate();
    }

    static string GetNetworkVersion()
    {
        return Marshal.PtrToStringAnsi(NetworkVersionAddress, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    static string GetNetworkProtocolId()
    {
        return Marshal.PtrToStringAnsi(NetworkVersionAddress, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
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
        FrostHook.LogLine($"GetPreemptiveDenyReason() player name={playerName}, type={playerType}, ptr=0x{playerRef:X0}, already={alreadyJoining}: denyReason={denyReason}");
        return denyReason;
    }

    static unsafe void AllowPlayerEntryInternalImpl(IntPtr @this, IntPtr playerPtr, bool allow, int reason)
    {
        var player = (GameManagerPlayer*)playerPtr;

        FrostHook.LogLine($"AllowPlayerEntryInternal() allow={allow}, reason={reason}");
        FrostHook.LogLine($"Pre-Player type={player->PlayerType}, state={player->PlayerState}");
        AllowPlayerEntryInternalHook!.OriginalFunction(@this, playerPtr, allow, reason);
        FrostHook.LogLine($"Post-Player type={player->PlayerType}, state={player->PlayerState}");
    }

    static unsafe void HandleEnterGameHostRequestImpl(IntPtr @this, IntPtr txnPtr)
    {
        var txn = (FeslTransaction*)txnPtr;
        FrostHook.LogLine($"Pre-HandleEnterGameHostRequest() allowedState={txn->mAllowedServiceState}, code={txn->mCode}, errCode={txn->mErrorCode}");

        var st = new StackTrace();
        FrostHook.LogLine("Stack trace:");
        FrostHook.LogLine(st.ToString());

        HandleEnterGameHostRequestHook!.OriginalFunction(@this, txnPtr);
        FrostHook.LogLine($"Post-HandleEnterGameHostRequest() allowedState={txn->mAllowedServiceState}, code={txn->mCode}, errCode={txn->mErrorCode}");
    }
}