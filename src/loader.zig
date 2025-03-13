const minhook = @import("bindings/minhook.zig");
const frosthook = @import("frosthook.zig");

pub fn deploy_mods() !void {
    try minhook.initialize();
    try frosthook.apply();
}
