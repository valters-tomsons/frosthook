using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X86;

namespace frosthook;

public enum HRESULT : uint
{
    S_OK = 0x0,
    S_FALSE = 0x1,
    E_NOTIMPL = 0x80004001
}

public enum FwdReason : uint
{
    DLL_PROCESS_ATTACH = 0x1
}

public enum MemoryProtection : uint
{
    PAGE_EXECUTE_READWRITE = 0x40
}

public enum ThreadPriority : int
{
    THREAD_PRIORITY_IDLE = -15,
    THREAD_PRIORITY_NORMAL = 0,
    THREAD_PRIORITY_TIME_CRITICAL = 15
}

public enum ThreadRights : uint
{
    THREAD_SUSPEND_RESUME = 0x0002
}

public static partial class Kernel32
{
    public const string LibraryName = "kernel32.dll";

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DisableThreadLibraryCalls(IntPtr hLibModule);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VirtualProtect(IntPtr lpAddress, uint dwSize, MemoryProtection flNewProtect, out uint lpflOldProtect);

    [LibraryImport(LibraryName, SetLastError = true)]
    public static partial IntPtr CreateThread(
        IntPtr lpThreadAttributes,
        uint dwStackSize,
        ThreadStart lpStartAddress,
        IntPtr lpParameter,
        uint dwCreationFlags,
        out nint lpThreadId);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(IntPtr hObject);

    [LibraryImport(LibraryName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr GetModuleHandleW(string lpModuleName);

    [LibraryImport(LibraryName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetThreadPriority(IntPtr hThread, ThreadPriority nPriority);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static partial uint GetCurrentThreadId();

    [LibraryImport(LibraryName, SetLastError = true)]
    public static partial IntPtr OpenThread(ThreadRights dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwThreadId);
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