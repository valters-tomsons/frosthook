const minhook = @import("bindings/minhook.zig");

pub fn deploy_mods() !void {
    try minhook.initialize();
}
