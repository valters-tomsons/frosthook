const std = @import("std");
const win = std.os.windows;
const winapi = @import("winapi.zig");
const utils = @import("utils.zig");
const console = @import("console.zig");
const loader = @import("loader.zig");

pub fn panic(msg: []const u8, error_return_trace: ?*std.builtin.StackTrace, ret_addr: ?usize) noreturn {
    utils.panicHandler(msg, error_return_trace, ret_addr);
}

pub export fn DirectInput8Create(_: win.HINSTANCE, _: win.DWORD, _: *const win.GUID, _: win.LPVOID, _: win.LPVOID) callconv(.C) win.HRESULT {
    @panic("Stub DirectInput8Create called - this should never happen!");
}

pub export fn DllMain(hModule: win.HINSTANCE, reason: win.DWORD, _: ?win.LPVOID) win.BOOL {
    if (@as(winapi.FwdReason, @enumFromInt(reason)) != winapi.FwdReason.DLL_PROCESS_ATTACH) {
        return win.TRUE;
    }

    _ = winapi.DisableThreadLibraryCalls(hModule);

    winapi.OutputDebugStringA("[dinput8] Loading frosthook...");
    console.initialize("frosthook");

    proxy();

    console.pause("Press ENTER to start...");
    install();

    return win.TRUE;
}

fn proxy() void {
    const originalModule = winapi.LoadLibraryA("C:\\Windows\\System32\\dinput8.dll") orelse {
        @panic("Failed to load original dinput8.dll");
    };

    const originalFunction = winapi.GetProcAddress(originalModule, "DirectInput8Create") orelse {
        @panic("Failed to get DirectInput8Create function address");
    };

    const jump_code = utils.generateJmpTo(@intFromPtr(originalFunction));
    utils.writeMemory(@ptrCast(&DirectInput8Create), &jump_code) catch {
        @panic("Failed to patch DirectInput8Create");
    };

    console.log("DirectInput8Create patched back to original!");
}

fn install() void {
    const main_thread_id = winapi.GetCurrentThreadId();
    const main_thread_handle = winapi.OpenThread(winapi.ThreadRights.THREAD_SUSPEND_RESUME, win.FALSE, main_thread_id).?;

    const payload_thread = std.Thread.spawn(.{}, deploy, .{@as(win.HANDLE, main_thread_handle)}) catch {
        @panic("Failed to spawn payload thread");
    };

    const payload_handle = payload_thread.getHandle();
    _ = winapi.SetThreadPriority(payload_handle, winapi.ThreadPriority.THREAD_PRIORITY_TIME_CRITICAL);
    _ = winapi.SetThreadPriority(main_thread_handle, winapi.ThreadPriority.THREAD_PRIORITY_IDLE);
}

fn deploy(main_thread_handle: win.HANDLE) callconv(.C) void {
    console.log("payload thread spawned");
    defer _ = winapi.CloseHandle(main_thread_handle);
    defer _ = winapi.SetThreadPriority(main_thread_handle, winapi.ThreadPriority.THREAD_PRIORITY_NORMAL);

    loader.deploy_mods() catch unreachable;
}
