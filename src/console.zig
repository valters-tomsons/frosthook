const std = @import("std");
const win = std.os.windows;
const winapi = @import("winapi.zig");

var initialized: bool = false;
var console_window: ?win.HWND = null;
var stdout_handle: ?win.HANDLE = null;

pub fn initialize(title: []const u8) void {
    if (initialized) return;

    if (winapi.AllocConsole() == 0) return;

    // Convert title to UTF16
    var title_buf: [256]u16 = undefined;
    const title_len = std.unicode.utf8ToUtf16Le(&title_buf, title) catch 0;
    if (title_len > 0) {
        title_buf[title_len] = 0;
        _ = winapi.SetConsoleTitleW(@ptrCast(&title_buf));
    }

    stdout_handle = winapi.GetStdHandle(win.STD_OUTPUT_HANDLE);
    console_window = winapi.GetConsoleWindow();

    if (console_window) |window| {
        _ = winapi.SetWindowPos(
            window,
            null,
            50,
            50,
            0,
            0,
            winapi.WindowFlags.SWP_NOSIZE | winapi.WindowFlags.SWP_NOZORDER,
        );
        _ = winapi.ShowWindow(window, 5);
    }

    initialized = true;
}

fn writeWithColor(message: []const u8, color: winapi.ConsoleColor) void {
    if (!initialized) return;
    if (stdout_handle) |handle| {
        var time_buf: [32]u8 = undefined;
        var sys_time: winapi.SYSTEMTIME = undefined;
        winapi.GetLocalTime(&sys_time);

        const time_str = std.fmt.bufPrint(
            &time_buf,
            "[{:0>2}:{:0>2}:{:0>2}.{:0>3}] ",
            .{
                sys_time.wHour,
                sys_time.wMinute,
                sys_time.wSecond,
                sys_time.wMilliseconds,
            },
        ) catch return;

        // Set color
        _ = winapi.SetConsoleTextAttribute(handle, @intFromEnum(color));

        // Convert to UTF16 and write
        var msg_buf: [512]u8 = undefined;
        var utf16_buf: [1024]u16 = undefined;
        const full_msg = std.fmt.bufPrint(&msg_buf, "[frosthook] {s}{s}\n", .{ time_str, message }) catch return;
        const len = std.unicode.utf8ToUtf16Le(&utf16_buf, full_msg) catch return;

        var written: win.DWORD = undefined;
        _ = winapi.WriteConsoleW(handle, &utf16_buf, @intCast(len), &written, null);

        // Reset color
        _ = winapi.SetConsoleTextAttribute(handle, @intFromEnum(winapi.ConsoleColor.White));
    }
}

pub fn log(message: []const u8) void {
    writeWithColor(message, .White);
}

pub fn logError(message: []const u8) void {
    writeWithColor(message, .Red);
}

pub fn logNotice(message: []const u8) void {
    writeWithColor(message, .Cyan);
}

pub fn pause(message: ?[]const u8) void {
    if (!initialized) return;

    if (message) |msg| {
        writeWithColor(msg, .White);
    }

    if (stdout_handle) |_| {
        const stdin_handle = winapi.GetStdHandle(win.STD_INPUT_HANDLE) orelse return;

        var buffer: [2]u16 = undefined;
        var chars_read: win.DWORD = undefined;

        _ = winapi.ReadConsoleW(
            stdin_handle,
            &buffer,
            2,
            &chars_read,
            null,
        );
    }
}

fn writeWithColorFmt(comptime fmt: []const u8, args: anytype, color: winapi.ConsoleColor) void {
    if (!initialized) return;
    if (stdout_handle) |handle| {
        var time_buf: [32]u8 = undefined;
        var sys_time: winapi.SYSTEMTIME = undefined;
        winapi.GetLocalTime(&sys_time);

        const time_str = std.fmt.bufPrint(
            &time_buf,
            "[{:0>2}:{:0>2}:{:0>2}.{:0>3}] ",
            .{
                sys_time.wHour,
                sys_time.wMinute,
                sys_time.wSecond,
                sys_time.wMilliseconds,
            },
        ) catch return;

        // Set color
        _ = winapi.SetConsoleTextAttribute(handle, @intFromEnum(color));

        // Convert to UTF16 and write
        var msg_buf: [1024]u8 = undefined;
        var full_msg_buf: [2048]u8 = undefined;
        var utf16_buf: [4096]u16 = undefined;

        const message = std.fmt.bufPrint(&msg_buf, fmt, args) catch return;
        const full_msg = std.fmt.bufPrint(&full_msg_buf, "[frosthook] {s}{s}\n", .{ time_str, message }) catch return;
        const len = std.unicode.utf8ToUtf16Le(&utf16_buf, full_msg) catch return;

        var written: win.DWORD = undefined;
        _ = winapi.WriteConsoleW(handle, &utf16_buf, @intCast(len), &written, null);

        // Reset color
        _ = winapi.SetConsoleTextAttribute(handle, @intFromEnum(winapi.ConsoleColor.White));
    }
}

pub fn logFmt(comptime fmt: []const u8, args: anytype) void {
    writeWithColorFmt(fmt, args, .White);
}

pub fn logErrorFmt(comptime fmt: []const u8, args: anytype) void {
    writeWithColorFmt(fmt, args, .Red);
}

pub fn logNoticeFmt(comptime fmt: []const u8, args: anytype) void {
    writeWithColorFmt(fmt, args, .Cyan);
}
