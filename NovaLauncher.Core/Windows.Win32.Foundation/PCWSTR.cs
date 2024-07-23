using System;
using System.Diagnostics;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{DebuggerDisplay}")]
internal readonly struct PCWSTR : IEquatable<PCWSTR>
{
	internal unsafe readonly char* Value;

	internal unsafe int Length
	{
		get
		{
			char* ptr = Value;
			if (ptr == null)
			{
				return 0;
			}
			for (; *ptr != 0; ptr++)
			{
			}
			return checked((int)(ptr - Value));
		}
	}

	private string DebuggerDisplay => ToString();

	internal unsafe PCWSTR(char* value)
	{
		Value = value;
	}

	public unsafe static explicit operator char*(PCWSTR value)
	{
		return value.Value;
	}

	public unsafe static implicit operator PCWSTR(char* value)
	{
		return new PCWSTR(value);
	}

	public unsafe bool Equals(PCWSTR other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is PCWSTR other)
		{
			return Equals(other);
		}
		return false;
	}

	public unsafe override int GetHashCode()
	{
		return (int)Value;
	}

	public unsafe override string ToString()
	{
		if (Value != null)
		{
			return new string(Value);
		}
		return null;
	}

	internal unsafe ReadOnlySpan<char> AsSpan()
	{
		if (Value != null)
		{
			return new ReadOnlySpan<char>(Value, Length);
		}
		return default(ReadOnlySpan<char>);
	}
}
