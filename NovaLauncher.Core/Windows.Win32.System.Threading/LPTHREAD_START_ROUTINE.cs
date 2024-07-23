using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Threading;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal unsafe delegate uint LPTHREAD_START_ROUTINE(void* lpThreadParameter);
