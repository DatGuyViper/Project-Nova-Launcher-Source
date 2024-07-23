using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace NovaLauncher.Web;

internal class ManagedStream : Stream
{
	[CompilerGenerated]
	private Stream _003Cs_003EP;

	public override bool CanRead => _003Cs_003EP.CanRead;

	public override bool CanSeek => _003Cs_003EP.CanSeek;

	public override bool CanWrite => _003Cs_003EP.CanWrite;

	public override long Length => _003Cs_003EP.Length;

	public override long Position
	{
		get
		{
			return _003Cs_003EP.Position;
		}
		set
		{
			_003Cs_003EP.Position = value;
		}
	}

	public ManagedStream(Stream s)
	{
		_003Cs_003EP = s;
		 
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _003Cs_003EP.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		try
		{
			num = _003Cs_003EP.Read(buffer, offset, count);
			if (num == 0)
			{
				_003Cs_003EP.Dispose();
			}
		}
		catch
		{
			_003Cs_003EP.Dispose();
			throw;
		}
		return num;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}
}
