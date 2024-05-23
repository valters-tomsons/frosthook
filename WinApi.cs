using System.Runtime.InteropServices;

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

public static partial class Kernel32
{
    private const string LibraryName = "kernel32.dll";

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
        out uint lpThreadId);

    [LibraryImport(LibraryName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(IntPtr hObject);
}
