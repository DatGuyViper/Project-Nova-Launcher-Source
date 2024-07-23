using System;
using System.Security.Cryptography;
using System.Text;

namespace NovaLauncher.Core;

internal class Security
{
	public static byte[] GenerateEntropy()
	{
		byte[] array = new byte[20];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(array);
		return array;
	}

	public static byte[] Protect(byte[] data, byte[] entropy)
	{
		try
		{
			return ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
		}
		catch (CryptographicException)
		{
			return Array.Empty<byte>();
		}
	}

	public static string Unprotect(byte[] data, byte[] entropy)
	{
		try
		{
			byte[] bytes = ProtectedData.Unprotect(data, entropy, DataProtectionScope.CurrentUser);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (CryptographicException)
		{
			return string.Empty;
		}
	}
}
