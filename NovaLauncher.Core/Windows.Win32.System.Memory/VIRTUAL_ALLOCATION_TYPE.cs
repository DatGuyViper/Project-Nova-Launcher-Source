using System;
using System.CodeDom.Compiler;

namespace Windows.Win32.System.Memory;

[Flags]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal enum VIRTUAL_ALLOCATION_TYPE : uint
{
	MEM_COMMIT = 0x1000u,
	MEM_RESERVE = 0x2000u,
	MEM_RESET = 0x80000u,
	MEM_RESET_UNDO = 0x1000000u,
	MEM_REPLACE_PLACEHOLDER = 0x4000u,
	MEM_LARGE_PAGES = 0x20000000u,
	MEM_RESERVE_PLACEHOLDER = 0x40000u,
	MEM_FREE = 0x10000u
}
