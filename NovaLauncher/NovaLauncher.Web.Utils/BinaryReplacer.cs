using System;
using System.IO;

namespace NovaLauncher.Web.Utils;

public sealed class BinaryReplacer : IDisposable
{
	private readonly Stream stream;

	private readonly int bufferSize;

	public BinaryReplacer(Stream stream, int bufferSize = 65535)
	{
		this.stream = stream;
		if (bufferSize < 2)
		{
			throw new ArgumentOutOfRangeException("bufferSize");
		}
		this.bufferSize = bufferSize;
		 
	}

	public void Dispose()
	{
		stream.Dispose();
	}

	public long Replace(byte[] find, byte[] replace)
	{
		if (find.Length != replace.Length)
		{
			throw new ArgumentException("Find and replace hex must be same length");
		}
		if (find.Length > bufferSize)
		{
			throw new ArgumentException("Find size " + find.Length + " is too large for buffer size " + bufferSize);
		}
		long num = 0L;
		long num2 = -1L;
		byte[] array = new byte[bufferSize + find.Length - 1];
		stream.Position = 0L;
		int num3;
		while ((num3 = stream.Read(array, 0, array.Length)) > 0)
		{
			for (int i = 0; i <= num3 - find.Length; i++)
			{
				for (int j = 0; j < find.Length && array[i + j] == find[j]; j++)
				{
					if (j == find.Length - 1)
					{
						stream.Seek(num + i, SeekOrigin.Begin);
						stream.Write(replace, 0, replace.Length);
						if (num2 == -1)
						{
							num2 = num + i;
						}
						break;
					}
				}
			}
			num += num3 - find.Length + 1;
			if (num > stream.Length - find.Length)
			{
				break;
			}
			stream.Seek(num, SeekOrigin.Begin);
		}
		return num2;
	}
}
