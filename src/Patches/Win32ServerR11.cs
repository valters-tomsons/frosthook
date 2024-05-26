// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
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

    static readonly IntPtr MakeConndIdOffset = new(0x012c1300);
    static IHook<MakeConnId>? MakeConnIdHook;

    static IHook<CreateFileA>? CreateFileAHook;

    // Ignore Trimmer warnings for now, they're coming from the hooking library, but seem to work fine at runtime.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "<Pending>")]
    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "<Pending>")]
    public static unsafe void Initialize()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Executable Version: {version}");

        var kernel32Handle = Kernel32.GetModuleHandleW(Kernel32.LibraryName); 
        var fileCreatePointer = Kernel32.GetProcAddress(kernel32Handle, "CreateFileA");
        Console.WriteLine($"frosthook Kernel32 : 0x{kernel32Handle:X0}");
        Console.WriteLine($"frosthook Kernel32.CreateFileA : 0x{fileCreatePointer:X0}");

        OverrideNetworkProtocol();

        Console.WriteLine($"frosthook hooking {nameof(CreateFileA)}");
        CreateFileAHook = new Hook<CreateFileA>(CreateFileAImpl, (nuint)fileCreatePointer).Activate();

        Console.WriteLine($"frosthook hooking {nameof(MakeConnId)}");
        MakeConnIdHook = new Hook<MakeConnId>(MakeConnIdImpl, (nuint)MakeConndIdOffset).Activate();
    }

    public static void DoRuntimeStuff()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Runtime Version: {version}");
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

    [Function(CallingConventions.Stdcall)]
    delegate IntPtr CreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

    static IntPtr CreateFileAImpl(string filename, FileAccess access, FileShare share, IntPtr securityAttributes, FileMode creationDisposition, FileAttributes flagsAndAttributes, IntPtr templateFile)
    {
        Console.WriteLine($"[CFA] Opening File {filename}");
        return CreateFileAHook!.OriginalFunction(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
    }

    [Function(CallingConventions.MicrosoftThiscall)]
    delegate void MakeConnId(IntPtr @this, int rf, IntPtr buf, uint bufSize);

    static void MakeConnIdImpl(IntPtr @this, int refValue, IntPtr buf, uint bufSize)
    {
        Console.WriteLine($"Intercepted: {nameof(MakeConnId)}, bufSize:{bufSize}");
        MakeConnIdHook!.OriginalFunction(@this, refValue, buf, bufSize);
    }
}