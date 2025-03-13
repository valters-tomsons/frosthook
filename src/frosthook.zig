// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

const std = @import("std");
const utils = @import("utils.zig");
const console = @import("console.zig");
const sig = @import("zignature-scanner");
const winapi = @import("winapi.zig");
const minhook = @import("bindings/minhook.zig");
const frostbite = @import("frostbite.zig");
const win = std.os.windows;

pub fn apply() !void {
    const proc = try utils.getModuleInfo();
    console.logFmt("Base address: 0x{X}", .{proc.BaseAddress});

    const kernel32_handle = winapi.LoadLibraryA("kernel32") orelse {
        return error.FailedToLoadKernel32;
    };

    const createFile_address = winapi.GetProcAddress(kernel32_handle, "CreateFileA") orelse {
        return error.FailedToGetMethodAddress;
    };

    try minhook.createHook(createFile_address, @ptrCast(&create_file_a_impl), @ptrCast(&create_file_a_ptr));
    try minhook.enableHook(createFile_address);
    console.log("CreateFileA hook installed!");

    const fesl_title_params_init_address = 0x01315860;
    try minhook.createHook(@ptrFromInt(fesl_title_params_init_address), @ptrCast(&fesl_title_params_init_impl), @ptrCast(&fesl_title_params_init_ptr));
    try minhook.enableHook(@ptrFromInt(fesl_title_params_init_address));
    console.log("FeslTitleParametersInit hook installed!");
}

var create_file_a_ptr: *const @TypeOf(create_file_a_impl) = undefined;
fn create_file_a_impl(fileName: win.LPCSTR, desiredAccess: win.DWORD, shareMode: win.DWORD, securityAttributes: ?*anyopaque, disposition: win.DWORD, flags: win.DWORD, templateFile: win.HANDLE) callconv(win.WINAPI) win.HANDLE {
    console.logFmt("CreateFileA hook called: {s}", .{fileName});
    return create_file_a_ptr(fileName, desiredAccess, shareMode, securityAttributes, disposition, flags, templateFile);
}

var fesl_title_params_init_ptr: *const @TypeOf(fesl_title_params_init_impl) = undefined;
fn fesl_title_params_init_impl(this: *anyopaque, sku: win.LPCSTR, clientVersion: win.LPCSTR, clientString: win.LPCSTR, feslPort: win.INT, env: frostbite.FESL_ENVIRONMENT) callconv(.Thiscall) void {
    console.log("FeslTitleParametersInit hook called!");

    console.logFmt("SKU: {s}", .{sku});
    console.logFmt("Client Version: {s}", .{clientVersion});
    console.logFmt("Client String: {s}", .{clientString});
    console.logFmt("Fesl Port: {d}", .{feslPort});
    console.logFmt("Environment: {s}", .{@tagName(env)});

    fesl_title_params_init_ptr(this, sku, clientVersion, clientString, feslPort, env);
}
