const win = @import("std").os.windows;

pub const FwdReason = enum(win.DWORD) {
    DLL_PROCESS_ATTACH = 1,
    DLL_PROCESS_DETACH = 0,
    DLL_THREAD_ATTACH = 2,
    DLL_THREAD_DETACH = 3,
};

pub const SYSTEMTIME = extern struct {
    wYear: win.WORD,
    wMonth: win.WORD,
    wDayOfWeek: win.WORD,
    wDay: win.WORD,
    wHour: win.WORD,
    wMinute: win.WORD,
    wSecond: win.WORD,
    wMilliseconds: win.WORD,
};

pub const ConsoleColor = enum(win.WORD) {
    Black = 0,
    Blue = 1,
    Green = 2,
    Cyan = 3,
    Red = 4,
    Magenta = 5,
    Yellow = 6,
    White = 7,
    Gray = 8,
    BrightBlue = 9,
    BrightGreen = 10,
    BrightCyan = 11,
    BrightRed = 12,
    BrightMagenta = 13,
    BrightYellow = 14,
    BrightWhite = 15,
};

pub const WindowFlags = struct {
    pub const SWP_NOSIZE = 0x0001;
    pub const SWP_NOZORDER = 0x0004;
};

pub const ThreadPriority = struct {
    pub const THREAD_PRIORITY_IDLE = -15;
    pub const THREAD_PRIORITY_NORMAL = 0;
    pub const THREAD_PRIORITY_TIME_CRITICAL = 15;
};

pub const ThreadRights = struct {
    pub const THREAD_SUSPEND_RESUME = 0x0002;
};

pub const EXCEPTION_CONTINUE_SEARCH = 0;
pub const EXCEPTION_CONTINUE_EXECUTION = -1;
pub const EXCEPTION_EXECUTE_HANDLER = 1;

pub const EXCEPTION_RECORD = extern struct {
    ExceptionCode: win.DWORD,
    ExceptionFlags: win.DWORD,
    ExceptionRecord: ?*EXCEPTION_RECORD,
    ExceptionAddress: win.LPVOID,
    NumberParameters: win.DWORD,
    ExceptionInformation: [15]win.ULONG_PTR,
};

pub const CONTEXT = extern struct {
    ContextFlags: win.DWORD,
    Dr0: win.DWORD,
    Dr1: win.DWORD,
    Dr2: win.DWORD,
    Dr3: win.DWORD,
    Dr6: win.DWORD,
    Dr7: win.DWORD,
    FloatSave: FLOATING_SAVE_AREA,
    SegGs: win.DWORD,
    SegFs: win.DWORD,
    SegEs: win.DWORD,
    SegDs: win.DWORD,
    Edi: win.DWORD,
    Esi: win.DWORD,
    Ebx: win.DWORD,
    Edx: win.DWORD,
    Ecx: win.DWORD,
    Eax: win.DWORD,
    Ebp: win.DWORD,
    Eip: win.DWORD,
    SegCs: win.DWORD,
    EFlags: win.DWORD,
    Esp: win.DWORD,
    SegSs: win.DWORD,
    ExtendedRegisters: [512]u8,
};

pub const FLOATING_SAVE_AREA = extern struct {
    ControlWord: win.DWORD,
    StatusWord: win.DWORD,
    TagWord: win.DWORD,
    ErrorOffset: win.DWORD,
    ErrorSelector: win.DWORD,
    DataOffset: win.DWORD,
    DataSelector: win.DWORD,
    RegisterArea: [80]u8,
    Cr0NpxState: win.DWORD,
};

pub const EXCEPTION_POINTERS = extern struct {
    ExceptionRecord: *EXCEPTION_RECORD,
    ContextRecord: *CONTEXT,
};

pub const D3DPRESENT_PARAMETERS = extern struct {
    BackBufferWidth: u32,
    BackBufferHeight: u32,
    BackBufferFormat: u32,
    BackBufferCount: u32,
    MultiSampleType: u32,
    MultiSampleQuality: u32,
    SwapEffect: D3DSWAPEFFECT,
    hDeviceWindow: win.HWND,
    Windowed: i32,
    EnableAutoDepthStencil: i32,
    AutoDepthStencilFormat: u32,
    Flags: u32,
    FullScreen_RefreshRateInHz: u32,
    PresentationInterval: u32,
};

pub const D3DSWAPEFFECT = enum(u32) { DISCARD = 1, FLIP = 2, COPY = 3, OVERLAY = 4, FLIPEX = 5, FORCE_DWORD = 0xFFFFFFFF };
pub const D3DPRESENT = enum(u32) { DO_NOT_WAIT = 0x00000001, LINEAR_CONTENT = 0x00000002, DO_NOT_FLIP = 0x00000004, FLIP_RESTART = 0x00000008, VIDEO_RESTRICT_TO_MONITOR = 0x00000010, UPDATE_OVERLAY_ONLY = 0x00000020, HIDEOVERLAY = 0x00000040, UPDATE_COLORKEY = 0x00000080, FORCE_IMMEDIATE = 0x00000100 };

pub extern "ntdll" fn RtlCaptureStackBackTrace(
    FramesToSkip: win.DWORD,
    FramesToCapture: win.DWORD,
    BackTrace: [*]win.PVOID,
    BackTraceHash: ?*win.DWORD,
) callconv(win.WINAPI) win.WORD;

pub extern "kernel32" fn DisableThreadLibraryCalls(hModule: win.HINSTANCE) callconv(win.WINAPI) win.BOOL;
pub extern "kernel32" fn LoadLibraryA(lpLibFileName: [*:0]const u8) callconv(win.WINAPI) ?win.HINSTANCE;
pub extern "kernel32" fn GetModuleHandleA(lpModuleName: ?[*:0]const u8) callconv(win.WINAPI) win.HMODULE;
pub extern "kernel32" fn GetProcAddress(hModule: ?win.HINSTANCE, lpProcName: ?[*:0]const u8) callconv(win.WINAPI) ?win.FARPROC;
pub extern "kernel32" fn VirtualProtect(lpAddress: ?*anyopaque, dwSize: usize, flNewProtect: win.DWORD, lpflOldProtect: *win.DWORD) callconv(win.WINAPI) win.BOOL;
pub extern "kernel32" fn VirtualAlloc(lpAddress: ?win.LPVOID, dwSize: usize, flAllocationType: win.DWORD, flProtect: win.DWORD) callconv(win.WINAPI) ?win.LPVOID;
pub extern "kernel32" fn SuspendThread(hThread: win.HANDLE) callconv(win.WINAPI) win.DWORD;
pub extern "kernel32" fn ResumeThread(hThread: win.HANDLE) callconv(win.WINAPI) win.DWORD;
pub extern "kernel32" fn ExitProcess(exit_code: c_uint) callconv(win.WINAPI) noreturn;
pub extern "kernel32" fn OutputDebugStringA(lpOutputString: [*:0]const u8) callconv(win.WINAPI) void;
pub extern "kernel32" fn AllocConsole() callconv(win.WINAPI) win.BOOL;
pub extern "kernel32" fn GetConsoleWindow() callconv(win.WINAPI) ?win.HWND;
pub extern "kernel32" fn SetConsoleTitleW(lpConsoleTitle: [*:0]const u16) callconv(win.WINAPI) win.BOOL;
pub extern "kernel32" fn GetStdHandle(nStdHandle: win.DWORD) callconv(win.WINAPI) ?win.HANDLE;
pub extern "kernel32" fn SetConsoleTextAttribute(hConsoleOutput: win.HANDLE, wAttributes: win.WORD) callconv(win.WINAPI) win.BOOL;
pub extern "kernel32" fn GetCommandLineW() callconv(win.WINAPI) win.LPWSTR;
pub extern "kernel32" fn WriteConsoleW(
    hConsoleOutput: win.HANDLE,
    lpBuffer: [*]const u16,
    nNumberOfCharsToWrite: win.DWORD,
    lpNumberOfCharsWritten: ?*win.DWORD,
    lpReserved: ?*anyopaque,
) callconv(win.WINAPI) win.BOOL;

pub extern "kernel32" fn ReadConsoleW(
    hConsoleInput: win.HANDLE,
    lpBuffer: [*]u16,
    nNumberOfCharsToRead: win.DWORD,
    lpNumberOfCharsRead: *win.DWORD,
    pInputControl: ?*anyopaque,
) callconv(win.WINAPI) win.BOOL;

pub extern "kernel32" fn GetLocalTime(lpSystemTime: *SYSTEMTIME) callconv(win.WINAPI) void;

pub extern "kernel32" fn SetThreadPriority(
    hThread: win.HANDLE,
    nPriority: i32,
) callconv(win.WINAPI) win.BOOL;

pub extern "kernel32" fn GetCurrentThreadId() callconv(win.WINAPI) win.DWORD;

pub extern "kernel32" fn OpenThread(
    dwDesiredAccess: win.DWORD,
    bInheritHandle: win.BOOL,
    dwThreadId: win.DWORD,
) callconv(win.WINAPI) ?win.HANDLE;

pub extern "kernel32" fn CloseHandle(
    hObject: win.HANDLE,
) callconv(win.WINAPI) win.BOOL;

pub extern "kernel32" fn AddVectoredExceptionHandler(
    FirstHandler: win.ULONG,
    VectoredHandler: *const fn (ExceptionInfo: *EXCEPTION_POINTERS) callconv(win.WINAPI) win.LONG,
) callconv(win.WINAPI) ?win.LPVOID;

pub extern "kernel32" fn RemoveVectoredExceptionHandler(
    Handler: win.LPVOID,
) callconv(win.WINAPI) win.ULONG;

pub extern "user32" fn SetWindowPos(
    hWnd: win.HWND,
    hWndInsertAfter: ?win.HWND,
    X: i32,
    Y: i32,
    cx: i32,
    cy: i32,
    uFlags: win.UINT,
) callconv(win.WINAPI) win.BOOL;

pub extern "user32" fn ShowWindow(hWnd: win.HWND, nCmdShow: i32) callconv(win.WINAPI) win.BOOL;
pub extern "user32" fn GetWindowLongA(hWnd: win.HWND, nIndex: i32) callconv(win.WINAPI) u32;
pub extern "user32" fn SetWindowLongA(hWnd: win.HWND, nIndex: i32, newStyle: u32) callconv(win.WINAPI) i32;

pub const MODULEINFO = extern struct {
    lpBaseOfDll: win.LPVOID,
    SizeOfImage: win.DWORD,
    EntryPoint: win.LPVOID,
};

pub extern "psapi" fn GetModuleInformation(
    hProcess: win.HANDLE,
    hModule: win.HMODULE,
    lpmodinfo: *MODULEINFO,
    cb: win.DWORD,
) callconv(win.WINAPI) win.BOOL;

pub extern "kernel32" fn GetCurrentProcess() callconv(win.WINAPI) win.HANDLE;
