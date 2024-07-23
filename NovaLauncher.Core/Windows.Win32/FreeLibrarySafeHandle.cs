using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Windows.Win32;

[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal class FreeLibrarySafeHandle : SafeHandle
{
	private static readonly nint INVALID_HANDLE_VALUE = new IntPtr(0L);

	public override bool IsInvalid => ((IntPtr)handle).ToInt64() == 0;

	internal FreeLibrarySafeHandle()
		: base(INVALID_HANDLE_VALUE, ownsHandle: true)
	{
	}

	internal FreeLibrarySafeHandle(nint preexistingHandle, bool ownsHandle = true)
		: base(INVALID_HANDLE_VALUE, ownsHandle)
	{
		SetHandle(preexistingHandle);
	}

	protected override bool ReleaseHandle()
	{
		return PInvoke.FreeLibrary((HMODULE)handle);
	}
}
