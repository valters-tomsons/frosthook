namespace frosthook;

public static class FrostHook
{
    public static void Initialize()
    {
        Console.WriteLine("frosthook thread says hellolize!");
        Thread.Sleep(500);
        Console.WriteLine("frosthook cleaning up!");
        
        Kernel32.CloseHandle(EntryPoint.HookThreadId);
    }
}