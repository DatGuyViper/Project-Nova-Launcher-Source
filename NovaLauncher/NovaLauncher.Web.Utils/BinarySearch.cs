namespace NovaLauncher.Web.Utils;

internal class BinarySearch
{
	public static int FindByteSequence(byte[] bytes, byte[] sequence)
	{
		for (int i = 0; i <= bytes.Length - sequence.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < sequence.Length; j++)
			{
				if (bytes[i + j] != sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}
}
