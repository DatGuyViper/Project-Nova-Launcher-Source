using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{Value}")]
[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal readonly struct BOOL : IEquatable<BOOL>
{
	internal readonly int Value;

	internal BOOL(int value)
	{
		Value = value;
	}

	public static implicit operator int(BOOL value)
	{
		return value.Value;
	}

	public static explicit operator BOOL(int value)
	{
		return new BOOL(value);
	}

	public static bool operator ==(BOOL left, BOOL right)
	{
		return left.Value == right.Value;
	}

	public static bool operator !=(BOOL left, BOOL right)
	{
		return !(left == right);
	}

	public bool Equals(BOOL other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is BOOL other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return $"0x{Value:x}";
	}

	internal BOOL(bool value)
	{
		Value = (value ? 1 : 0);
	}

	public static implicit operator bool(BOOL value)
	{
		return value.Value != 0;
	}

	public static implicit operator BOOL(bool value)
	{
		return new BOOL(value);
	}
}
