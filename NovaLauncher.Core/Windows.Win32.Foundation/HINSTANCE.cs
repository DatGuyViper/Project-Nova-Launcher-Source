using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{Value}")]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal readonly struct HINSTANCE : IEquatable<HINSTANCE>
{
	internal readonly nint Value;

	internal static HINSTANCE Null => default(HINSTANCE);

	internal bool IsNull => Value == 0;

	internal HINSTANCE(nint value)
	{
		Value = value;
	}

	public static implicit operator nint(HINSTANCE value)
	{
		return value.Value;
	}

	public static explicit operator HINSTANCE(nint value)
	{
		return new HINSTANCE(value);
	}

	public static bool operator ==(HINSTANCE left, HINSTANCE right)
	{
		return left.Value == right.Value;
	}

	public static bool operator !=(HINSTANCE left, HINSTANCE right)
	{
		return !(left == right);
	}

	public bool Equals(HINSTANCE other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is HINSTANCE other)
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

	public static implicit operator HMODULE(HINSTANCE value)
	{
		return new HMODULE(value.Value);
	}
}
