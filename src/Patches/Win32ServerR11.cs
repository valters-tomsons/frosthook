// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

using System.Diagnostics.CodeAnalysis;
using frosthook.Frostbite;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

namespace frosthook.Patches.BC2;

public static class Win32ServerR11
{
    static IHook<CreateFileA>? CreateFileAHook;
    static IHook<FeslTitleParametersInit>? TitleInitHook;

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "<Pending>")]
    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "<Pending>")]
    public static unsafe void Apply()
    {
        var kernel32Handle = Kernel32.GetModuleHandleW(Kernel32.LibraryName);
        var fileCreatePointer = Kernel32.GetProcAddress(kernel32Handle, "CreateFileA");
        CreateFileAHook = new Hook<CreateFileA>(CreateFileAImpl, (nuint)fileCreatePointer).Activate();

        TitleInitHook = new Hook<FeslTitleParametersInit>(TitleInitImpl, 0x01315860).Activate();
    }

    static IntPtr CreateFileAImpl(string filename, FileAccess access, FileShare share, IntPtr securityAttributes, FileMode creationDisposition, FileAttributes flagsAndAttributes, IntPtr templateFile)
    {
        FrostHook.LogLine($"Opening file: {filename}");
        return CreateFileAHook!.OriginalFunction(filename, access, share, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
    }

    static void TitleInitImpl(IntPtr @this, string sku, string clientVersion, string clientString, int feslPort, FeslEnvironment env)
    {
        FrostHook.LogLine($"TitleInit Called: {sku}, {clientVersion}, {clientString}, {feslPort}, {env}");
        TitleInitHook!.OriginalFunction(@this, sku, clientVersion, clientString, feslPort, env);
    }
}