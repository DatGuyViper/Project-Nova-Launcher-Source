using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{Value}")]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal readonly struct HANDLE : IEquatable<HANDLE>
{
	internal readonly nint Value;

	internal static HANDLE Null => default(HANDLE);

	internal bool IsNull => Value == 0;

	internal HANDLE(nint value)
	{
		Value = value;
	}

	public static implicit operator nint(HANDLE value)
	{
		return value.Value;
	}

	public static explicit operator HANDLE(nint value)
	{
		return new HANDLE(value);
	}

	public static bool operator ==(HANDLE left, HANDLE right)
	{
		return left.Value == right.Value;
	}

	public static bool operator !=(HANDLE left, HANDLE right)
	{
		return !(left == right);
	}

	public bool Equals(HANDLE other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is HANDLE other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((IntPtr)Value).GetHashCode();
	}

	public override string ToString()
	{
		return $"0x{Value:x}";
	}
}
