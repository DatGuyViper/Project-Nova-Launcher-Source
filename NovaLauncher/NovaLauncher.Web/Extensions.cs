using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NovaLauncher.Web;

public static class Extensions
{
	private static readonly int MAX_PATH = 255;

	[DllImport("kernel32.dll")]
	private static extern uint GetModuleFileName(nint hModule, StringBuilder lpFilename, int nSize);

	public static string GetExecutablePath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			StringBuilder stringBuilder = new StringBuilder(MAX_PATH);
			GetModuleFileName(IntPtr.Zero, stringBuilder, MAX_PATH);
			return stringBuilder.ToString();
		}
		return Process.GetCurrentProcess().MainModule.FileName;
	}
}
