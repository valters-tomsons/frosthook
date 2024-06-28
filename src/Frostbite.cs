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

[Function(CallingConventions.MicrosoftThiscall)]
public delegate void HandleEnterGameHostRequest(
    IntPtr @this,
    IntPtr txnPtr);

[Function(CallingConventions.MicrosoftThiscall)]
public delegate ClientDisconnectReason GetReason(IntPtr @this);

[Function(CallingConventions.MicrosoftThiscall)]
public delegate string GetGameProtocolVersion(IntPtr @this);

[Function(CallingConventions.MicrosoftThiscall)]
public delegate void FeslTitleParametersInit
(
    IntPtr @this,
    [MarshalAs(UnmanagedType.LPStr)] string sku,
    [MarshalAs(UnmanagedType.LPStr)] string clientVersion,
    [MarshalAs(UnmanagedType.LPStr)] string clientString,
    [MarshalAs(UnmanagedType.I4)] int feslPort,
    [MarshalAs(UnmanagedType.U4)] FeslEnvironment env
);

public enum FeslEnvironment : uint
{
    FESL_ENVIRONMENT_STEST = 0x0,
    FESL_ENVIRONMENT_PROD = 0x1,
    FESL_ENVIRONMENT_INTERNAL_SCERT = 0x2
}

public enum GamePlayerType : uint
{
    GAME_PLAYER_TYPE_PARTICIPANT = 0x0,
    GAME_PLAYER_TYPE_OBSERVER = 0x1,
    GAME_PLAYER_TYPE_COUNT = 0x2
};

public enum ClientDisconnectReason : uint
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

public enum GameManagerJoinMode : uint
{
    GAMEMANAGER_JOIN_MODE_CLOSED = 0x0,
    GAMEMANAGER_JOIN_MODE_OPEN = 0x1,
    GAMEMANAGER_JOIN_MODE_WAIT = 0x2,
    GAMEMANAGER_JOIN_MODE_AUTO = 0x3,
    GAMEMANAGER_JOIN_MODE_CUSTOM = 0x4
};

public enum GameManagerGameState : uint
{
    GAMEMANAGER_GAME_STATE_NULL = 0x0,
    GAMEMANAGER_GAME_STATE_WAITING_FOR_ENTRY_RESPONSE = 0x1,
    GAMEMANAGER_GAME_STATE_WAITING_FOR_ALLOW = 0x2,
    GAMEMANAGER_GAME_STATE_QUEUED = 0x3,
    GAMEMANAGER_GAME_STATE_CONNECTING = 0x4,
    GAMEMANAGER_GAME_STATE_ACTIVE = 0x5,
    GAMEMANAGER_GAME_STATE_CREATING = 0x6,
    GAMEMANAGER_GAME_STATE_MIGRATING_INIT = 0x7,
    GAMEMANAGER_GAME_STATE_MIGRATING = 0x8,
    GAMEMANAGER_GAME_STATE_MIGRATING_RECONNECT = 0x9
};

[StructLayout(LayoutKind.Sequential)]
public struct GameManagerPlayer
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

[StructLayout(LayoutKind.Sequential)]
public struct FeslTransaction
{
    public IntPtr mAddress; // Fesl::InternetAddress*
    public IntPtr mBuffer; // char*
    public uint mSize;
    public uint mUsed;
    public int mKind;
    public int mCode;
    public int mErrorCode;
    public int mId;
    public int mAllowedServiceState;
    public bool mMode;
    public IntPtr mHead; // Transaction::Buffer*
    public IntPtr mTail; // Transaction::Buffer*
    public uint mNumBuffers;
    public bool mOwnBuffers;
}

[StructLayout(LayoutKind.Explicit)]
public struct GameManagerHostedGame
{
    [FieldOffset(0xb0)]
    public GameManagerJoinMode CurrentJoinMode;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct TitleInfoImpl
{
    public IntPtr TitleInfoImplVTable;

    [MarshalAs(UnmanagedType.U4)]
    public uint mXb360TitleId;

    [MarshalAs(UnmanagedType.U4)]
    public FeslPlatformOverride mPlatformOverride;

    [MarshalAs(UnmanagedType.I4, SizeConst = 0xa)]
    public int mPS3ContentMinimumAge;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
    public string mSKU;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x8)]
    public string mPS3SPID;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
    public string mPCTitleId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
    public string mPS3TitleId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
    public string mClientVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xa)]
    public string mPS3CommunicationId;

    [MarshalAs(UnmanagedType.U1)]
    public bool mXb360MultiplayerSessionsCheck;
}

public enum FeslPlatformOverride : uint
{
    FESL_PLATFORM_PC = 0x0,
    FESL_PLATFORM_XB360 = 0x1,
    FESL_PLATFORM_PS3 = 0x2
};