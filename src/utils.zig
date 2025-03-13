const std = @import("std");
const win = std.os.windows;
const winapi = @import("winapi.zig");
const console = @import("console.zig");

pub fn writeMemory(address: [*]const u8, data: []const u8) !void {
    var old_protect: win.DWORD = undefined;
    const ptr = @constCast(address);

    if (winapi.VirtualProtect(
        @ptrCast(ptr),
        data.len,
        win.PAGE_EXECUTE_READWRITE,
        &old_protect,
    ) == 0) {
        return error.ProtectionChangeFailed;
    }
    defer _ = winapi.VirtualProtect(
        @ptrCast(ptr),
        data.len,
        old_protect,
        &old_protect,
    );

    @memcpy(@constCast(ptr[0..data.len]), data);
}

pub fn generateJmpTo(target_address: usize) [7]u8 {
    var shellcode = [7]u8{
        0xB8, // mov eax,
        0x00, 0x00, 0x00, 0x00, // addr
        0xFF, 0xE0, // jmp eax
    };

    const addr_bytes = std.mem.asBytes(&target_address);
    @memcpy(shellcode[1..5], addr_bytes);
    return shellcode;
}

fn captureStackTrace() []win.PVOID {
    var trace: [32]win.PVOID = undefined;
    const frames_captured = winapi.RtlCaptureStackBackTrace(
        1, // Skip this function
        32,
        &trace,
        null,
    );
    return trace[0..frames_captured];
}

pub fn vectoredExceptionHandler(exception_info: *winapi.EXCEPTION_POINTERS) callconv(win.WINAPI) win.LONG {
    const exception = exception_info.ExceptionRecord;
    const context = exception_info.ContextRecord;
    const code = exception.ExceptionCode;
    const addr = @intFromPtr(exception.ExceptionAddress);

    console.logNotice("=== PROCESS EXCEPTION CAUGHT ===");
    console.logErrorFmt("Exception code: 0x{X}", .{code});
    console.logErrorFmt("Exception address: 0x{X}", .{addr});

    if (exception.NumberParameters > 0) {
        console.logError("Exception parameters:");
        var i: usize = 0;
        while (i < exception.NumberParameters) : (i += 1) {
            console.logErrorFmt("  param {d}: 0x{X}", .{ i, exception.ExceptionInformation[i] });
        }
    }

    console.logError("CPU Context:");
    console.logErrorFmt("  EAX: 0x{X:0>8}", .{context.Eax});
    console.logErrorFmt("  EBX: 0x{X:0>8}", .{context.Ebx});
    console.logErrorFmt("  ECX: 0x{X:0>8}", .{context.Ecx});
    console.logErrorFmt("  EDX: 0x{X:0>8}", .{context.Edx});
    console.logErrorFmt("  ESI: 0x{X:0>8}", .{context.Esi});
    console.logErrorFmt("  EDI: 0x{X:0>8}", .{context.Edi});
    console.logErrorFmt("  EBP: 0x{X:0>8}", .{context.Ebp});
    console.logErrorFmt("  ESP: 0x{X:0>8}", .{context.Esp});
    console.logErrorFmt("  EIP: 0x{X:0>8}", .{context.Eip});
    console.logErrorFmt("  EFLAGS: 0x{X:0>8}", .{context.EFlags});

    // Capture and log stack trace
    console.logError("Stack trace:");
    const trace = captureStackTrace();
    for (trace, 0..) |frame, i| {
        console.logErrorFmt("  frame {d}: 0x{X}", .{ i, @intFromPtr(frame) });
    }

    console.logError("===================");
    return winapi.EXCEPTION_CONTINUE_SEARCH;
}

pub fn panicHandler(msg: []const u8, _: ?*std.builtin.StackTrace, ret_addr: ?usize) noreturn {
    console.logNotice("=== PANIC ===");
    console.logErrorFmt("Message: {s}", .{msg});

    // Try to extract error information from the message
    if (std.mem.indexOf(u8, msg, "error.")) |start| {
        if (std.mem.indexOfScalar(u8, msg[start..], ' ')) |end| {
            const error_name = msg[start..(start + end)];
            console.logErrorFmt("Error type: {s}", .{error_name});
        }
    }

    // Log the return address
    if (ret_addr) |addr| {
        console.logErrorFmt("Return address: 0x{X}", .{addr});
    } else {
        const addr = @returnAddress();
        console.logErrorFmt("Return address: 0x{X}", .{addr});
    }

    console.logError("Stack trace:");
    const trace = captureStackTrace();
    for (trace, 0..) |frame, i| {
        console.logErrorFmt("  frame {d}: 0x{X}", .{ i, @intFromPtr(frame) });
    }

    console.logError("===================");
    console.pause("Press Enter to terminate...");
    winapi.ExitProcess(1);
}

pub const ModuleInfo = struct { BaseAddress: usize, SizeOfImage: usize };
pub fn getModuleInfo() !ModuleInfo {
    var module_info: winapi.MODULEINFO = undefined;
    const size = @sizeOf(winapi.MODULEINFO);

    if (winapi.GetModuleInformation(winapi.GetCurrentProcess(), winapi.GetModuleHandleA(null), &module_info, @intCast(size)) == 0) {
        return error.FailedToGetModuleInfo;
    }

    const base_address = @intFromPtr(module_info.lpBaseOfDll);
    const module_size = module_info.SizeOfImage;

    if (base_address == 0 or module_size == 0) {
        return error.ModuleInfoInvalid;
    }

    return ModuleInfo{ .BaseAddress = base_address, .SizeOfImage = module_size };
}
