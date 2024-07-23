using System.CodeDom.Compiler;
using Windows.Win32.Foundation;

namespace Windows.Win32.Security;

[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal struct SECURITY_ATTRIBUTES
{
	internal uint nLength;

	internal unsafe void* lpSecurityDescriptor;

	internal BOOL bInheritHandle;
}
