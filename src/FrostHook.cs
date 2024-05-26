using System.Diagnostics;

namespace frosthook;

public static class FrostHook
{
    static readonly ThreadStart PayloadStart = new(RunPayloadThread);

    static nint ThreadHandle = 0;
    static nint MainThreadHandle = 0;

    public static void OnAttach(IntPtr hModule)
    {
        Console.WriteLine($"frosthook loading, hModule = 0x{hModule:X0}");

        using (var currentProcess = Process.GetCurrentProcess())
        {
            using var module = currentProcess.MainModule!;
            Console.WriteLine($"frosthook attaching to: {module.ModuleName}, 0x{module.BaseAddress:X0}");
        }

        ThreadHandle = Kernel32.CreateThread(IntPtr.Zero, 0, PayloadStart, IntPtr.Zero, 0, out var threadId);
        Kernel32.SetThreadPriority(ThreadHandle, ThreadPriority.THREAD_PRIORITY_TIME_CRITICAL);

        var mainThreadId = Kernel32.GetCurrentThreadId();
        MainThreadHandle = Kernel32.OpenThread(ThreadRights.THREAD_SUSPEND_RESUME, false, mainThreadId);
        Kernel32.SetThreadPriority(MainThreadHandle, ThreadPriority.THREAD_PRIORITY_IDLE);

        Console.WriteLine($"frosthook thread created = 0x{ThreadHandle:X0}, 0x{threadId:X0}");
    }

    static void RunPayloadThread()
    {
        Console.WriteLine($"frosthook thread executing = 0x{ThreadHandle:X0}");
        Console.WriteLine($"frosthook suspending main thread = 0x{MainThreadHandle:X0}");

        Patches.BC2.Win32ServerR11.Initialize();

        Console.WriteLine($"frosthook resuming main thread = 0x{MainThreadHandle:X0}");
        Kernel32.SetThreadPriority(MainThreadHandle, ThreadPriority.THREAD_PRIORITY_NORMAL);
        
        Console.WriteLine($"frosthook thread closing = 0x{ThreadHandle:X0}");
        Kernel32.CloseHandle(ThreadHandle);
    }
}