const win = @import("std").os.windows;

pub const FESL_ENVIRONMENT = enum(win.DWORD) { STEST = 0x0, PROD = 0x1, INTERNAL_SCERT = 0x2 };
