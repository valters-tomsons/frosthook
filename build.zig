const std = @import("std");

pub fn build(b: *std.Build) !void {
    const optimize = b.standardOptimizeOption(.{});
    const target = b.standardTargetOptions(.{
        .default_target = .{ .cpu_arch = .x86, .os_tag = .windows, .abi = .gnu },
    });

    if (target.result.os.tag != .windows or target.result.cpu.arch != .x86) {
        return error.UnsupportedTargetPlatform;
    }

    const frosthook_lib = b.addSharedLibrary(.{ .name = "dinput8", .root_source_file = b.path("src/root.zig"), .target = target, .optimize = optimize, .version = .{ .major = 0, .minor = 0, .patch = 1 } });
    frosthook_lib.linkLibC();

    const minhook_lib = b.addStaticLibrary(.{ .name = "minhook", .root_source_file = b.path("src/bindings/minhook.zig"), .target = target, .optimize = optimize });
    minhook_lib.linkLibC();
    minhook_lib.addCSourceFiles(.{
        .root = b.path("third-party/minhook/src/"),
        .files = &.{
            "hook.c",
            "buffer.c",
            "trampoline.c",
            "hde/hde32.c",
        },
    });

    minhook_lib.installHeader(b.path("third-party/minhook/include/MinHook.h"), "MinHook.h");
    frosthook_lib.linkLibrary(minhook_lib);

    const zignature_scanner_dep = b.dependency("zignature-scanner", .{
        .target = target,
        .optimize = optimize,
    });

    frosthook_lib.root_module.addImport("zignature-scanner", zignature_scanner_dep.module("zignature-scanner"));

    b.installArtifact(frosthook_lib);
}
