using System.CodeDom.Compiler;

namespace Windows.Win32.Foundation;

[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal enum WAIT_EVENT : uint
{
	WAIT_OBJECT_0 = 0u,
	WAIT_ABANDONED = 128u,
	WAIT_ABANDONED_0 = 128u,
	WAIT_IO_COMPLETION = 192u,
	WAIT_TIMEOUT = 258u,
	WAIT_FAILED = uint.MaxValue
}
