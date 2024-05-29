using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X86;

namespace frosthook.Frostbite;

[Function(CallingConventions.MicrosoftThiscall)]
public delegate void DispatchMessage(IntPtr @this, IntPtr message);

[Function(CallingConventions.MicrosoftThiscall)]
public delegate int GetPreemptiveDenyReason(
    IntPtr @this,
    [MarshalAs(UnmanagedType.I4)] GamePlayerType playerType,
    [MarshalAs(UnmanagedType.LPStr)] string playerName,
    [MarshalAs(UnmanagedType.I4)] int playerRef,
    [MarshalAs(UnmanagedType.U1)] bool playerAlreadyJoining);

[Function(CallingConventions.MicrosoftThiscall)]
public delegate void AllowPlayerEntryInternal(
    IntPtr @this,
    IntPtr playerPtr,
    [MarshalAs(UnmanagedType.U1)] bool allow,
    [MarshalAs(UnmanagedType.I4)] int reason);

public enum GamePlayerType : uint
{
    GAME_PLAYER_TYPE_PARTICIPANT = 0x0,
    GAME_PLAYER_TYPE_OBSERVER = 0x1,
    GAME_PLAYER_TYPE_COUNT = 0x2
};

public enum ClientDisconnectReason
{
    Ok = 0x0,
    WrongProtocolVersion = 0x1,
    WrongTitleVersion = 0x2,
    ServerFull = 0x3,
    KickedOut = 0x4,
    Banned = 0x5,
    GenericError = 0x6,
    WrongPassword = 0x7,
    KickedOutDemoOver = 0x8,
    RankRestricted = 0x9,
    ConfigurationNotAllowed = 0xa,
    ServerReclaimed = 0xb,
    MissingContent = 0xc
};

public enum GameManagerPlayerState : uint
{
    GAMEMANAGER_PLAYER_STATE_REQUESTING_APPROVAL = 0x0,
    GAMEMANAGER_PLAYER_STATE_APPROVED = 0x1,
    GAMEMANAGER_PLAYER_STATE_PENDING_ACTIVE = 0x2,
    GAMEMANAGER_PLAYER_STATE_PREROSTER = 0x3,
    GAMEMANAGER_PLAYER_STATE_ROSTERSENT = 0x4,
    GAMEMANAGER_PLAYER_STATE_JOINING = 0x5,
    GAMEMANAGER_PLAYER_STATE_JOINED = 0x6
};

[StructLayout(LayoutKind.Sequential)]
public struct GameManagerPlayerImpl
{
    public IntPtr VTable;
    public IntPtr EncodableVTable;
    public IntPtr Game;  //GameManagerGameImpl*
    public IntPtr HostedPlayer; // GameManagerHostedPlayerImpl*
    public int PlayerRef;
    public int SlotId;
    public int VoipSlotId;
    public int OldVoipSlotId;
    public IntPtr User; // UserImpl*
    public long PlayGroupRef;
    public IntPtr PeerLink; // GameManagerConnection*
    public GameManagerPlayerState PlayerState;
    public IntPtr PeerAddress; // Address*
    public GamePlayerType PlayerType;
    public bool Connect;
    public bool VoipSendToEnabled;
    public bool VoipMuted;
    public bool VoidHwEnabled;
}