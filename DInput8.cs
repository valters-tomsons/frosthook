using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace frosthook;

public static partial class DInput8
{
    [UnmanagedCallersOnly(EntryPoint = nameof(DirectInput8Create), CallConvs = [typeof(CallConvStdcall)])]
    public static HRESULT DirectInput8Create(IntPtr hinst, uint dwVersion, IntPtr riidltf, IntPtr ppvOut, IntPtr punkOuter)
    {
        Console.WriteLine("DirectInput8Create called, goodbye!");
        return HRESULT.S_FALSE;
    }
}