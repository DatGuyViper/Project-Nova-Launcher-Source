using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{Value}")]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal readonly struct HMODULE : IEquatable<HMODULE>
{
	internal readonly nint Value;

	internal static HMODULE Null => default(HMODULE);

	internal bool IsNull => Value == 0;

	internal HMODULE(nint value)
	{
		Value = value;
	}

	public static implicit operator nint(HMODULE value)
	{
		return value.Value;
	}

	public static explicit operator HMODULE(nint value)
	{
		return new HMODULE(value);
	}

	public static bool operator ==(HMODULE left, HMODULE right)
	{
		return left.Value == right.Value;
	}

	public static bool operator !=(HMODULE left, HMODULE right)
	{
		return !(left == right);
	}

	public bool Equals(HMODULE other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is HMODULE other)
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

	public static implicit operator HINSTANCE(HMODULE value)
	{
		return new HINSTANCE(value.Value);
	}
}
