using System;
using System.Diagnostics;
using System.Text;

namespace Windows.Win32.Foundation;

[DebuggerDisplay("{DebuggerDisplay}")]
internal readonly struct PCSTR : IEquatable<PCSTR>
{
	internal unsafe readonly byte* Value;

	internal unsafe int Length
	{
		get
		{
			byte* ptr = Value;
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

	internal unsafe PCSTR(byte* value)
	{
		Value = value;
	}

	public unsafe static implicit operator byte*(PCSTR value)
	{
		return value.Value;
	}

	public unsafe static explicit operator PCSTR(byte* value)
	{
		return new PCSTR(value);
	}

	public unsafe bool Equals(PCSTR other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		if (obj is PCSTR other)
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
			return new string((sbyte*)Value, 0, Length, Encoding.Default);
		}
		return null;
	}

	internal unsafe ReadOnlySpan<byte> AsSpan()
	{
		if (Value != null)
		{
			return new ReadOnlySpan<byte>(Value, Length);
		}
		return default(ReadOnlySpan<byte>);
	}
}
