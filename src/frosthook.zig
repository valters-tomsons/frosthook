// ff7c40f0271f387c52dc0a9190cac6ea549244f3  Frost.Game.Main_Win32_Final.exe
// Bad Company 2 Server R11

const std = @import("std");
const utils = @import("utils.zig");
const console = @import("console.zig");
const sig = @import("zignature-scanner");
const winapi = @import("winapi.zig");
const win = std.os.windows;

pub fn apply() !void {
    const proc = try utils.getModuleInfo();
    console.logFmt("Base address: 0x{X}", .{proc.BaseAddress});
}
