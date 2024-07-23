using System.Security.Cryptography;

namespace NovaLauncher.Web;

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
			return null;
		}
	}

	public static byte[] Unprotect(byte[] data, byte[] entropy)
	{
		try
		{
			return ProtectedData.Unprotect(data, entropy, DataProtectionScope.CurrentUser);
		}
		catch (CryptographicException)
		{
			return null;
		}
	}
}
