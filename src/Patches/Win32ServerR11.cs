// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;

namespace frosthook.Patches.BC2;

public static class Win32ServerR11
{
    // Network Protocol Patch
    const int NetworkProtocolLength = 12;
    static readonly IntPtr NetworkProtocolOffset = new(0x1753911);
    static readonly byte[] NetworkProtocolPatch = Encoding.ASCII.GetBytes("RETAIL133337");

    static IHook<CreateFileA> CreateFileAHook;

    // static CreateFileA Kernel32_CreateFileA;
    // [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [Function(CallingConventions.Stdcall)]
    delegate IntPtr CreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

    public static unsafe void Initialize()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Executable Version: {version}");

        var kernel32Handle = Kernel32.GetModuleHandleW(Kernel32.LibraryName); 
        Console.WriteLine($"frosthook Kernel32 : 0x{kernel32Handle:X0}");

        var fileCreatePointer = Kernel32.GetProcAddress(kernel32Handle, "CreateFileA");
        Console.WriteLine($"frosthook Kernel32.CreateFileA : 0x{fileCreatePointer:X0}");

        // Kernel32_CreateFileA = Marshal.GetDelegateForFunctionPointer<CreateFileA>(fileCreatePointer);
        CreateFileAHook = new Hook<CreateFileA>(CreateFileAImpl, (nuint)fileCreatePointer).Activate();

        // GenerateHook(fileCreatePointer);
        OverrideNetworkProtocol();
    }

    public static void DoRuntimeStuff()
    {
        var version = GetNetworkProtocolVersion();
        Console.WriteLine($"Runtime Version: {version}");
    }

    // private static void GenerateHook(IntPtr procAddress)
    // {
    //     var hookAddress = Marshal.GetFunctionPointerForDelegate(new CreateFileA(CreateFileAImpl));
    //     Console.WriteLine($"hookAddress: 0x{hookAddress:X0}");

    //     var assembler = new Assembler(32);
    //     assembler.jmp((ulong)hookAddress.ToInt32());

    //     using var asmStream = new MemoryStream();
    //     var asmResult = assembler.Assemble(new StreamCodeWriter(asmStream), (ulong)procAddress.ToInt32());

    //     Console.WriteLine("ASM generated!");

    //     // Overwrite the original function pointer with the hook bytes
    //     using var stringProtect = Memory.Instance.ChangeProtectionDisposable((nuint)procAddress, NetworkProtocolLength, Reloaded.Memory.Enums.MemoryProtection.Write);
    //     Memory.Instance.WriteRaw((nuint)procAddress, NetworkProtocolPatch);

    //     Console.WriteLine("Hook patched!");
    // }

    static string GetNetworkProtocolVersion()
    {
        return Marshal.PtrToStringAnsi(NetworkProtocolOffset, NetworkProtocolLength) ?? throw new Exception("Failed to read version, goodbye!");
    }

    unsafe static void OverrideNetworkProtocol()
    {
        var offset = (nuint)NetworkProtocolOffset;
        var memory = Memory.Instance;

        using var stringProtect = memory.ChangeProtectionDisposable(offset, NetworkProtocolLength, Reloaded.Memory.Enums.MemoryProtection.Write);
        memory.WriteRaw(offset, NetworkProtocolPatch);
    }

    private static IntPtr CreateFileAImpl(string filename, FileAccess access, FileShare share, IntPtr securityAttributes, FileMode creationDisposition, FileAttributes flagsAndAttributes, IntPtr templateFile)
    {
        Console.WriteLine($"[CFA] Opening File {filename}");
        // return Kernel32_CreateFileA(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
        return CreateFileAHook.OriginalFunction(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
    }
}