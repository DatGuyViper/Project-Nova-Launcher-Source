using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NovaLauncher.Core.Utilities;

internal class Native
{
	[StructLayout(LayoutKind.Sequential)]
	public class SecurityAttributes
	{
		public int Length;

		public nint lpSecurityDescriptor = IntPtr.Zero;

		public bool bInheritHandle;

		public SecurityAttributes()
		{
			Length = Marshal.SizeOf(this);
		}
	}

	[Flags]
	public enum LogonFlags
	{
		LOGON_WITH_PROFILE = 1,
		LOGON_NETCREDENTIALS_ONLY = 2
	}

	[StructLayout(LayoutKind.Sequential)]
	public class StartupInfo
	{
		public int cb;

		public nint lpReserved = IntPtr.Zero;

		public nint lpDesktop = IntPtr.Zero;

		public nint lpTitle = IntPtr.Zero;

		public int dwX;

		public int dwY;

		public int dwXSize;

		public int dwYSize;

		public int dwXCountChars;

		public int dwYCountChars;

		public int dwFillAttribute;

		public int dwFlags;

		public short wShowWindow;

		public short cbReserved2;

		public nint lpReserved2 = IntPtr.Zero;

		public nint hStdInput = IntPtr.Zero;

		public nint hStdOutput = IntPtr.Zero;

		public nint hStdError = IntPtr.Zero;

		public StartupInfo()
		{
			cb = Marshal.SizeOf(this);
		}
	}

	public struct ProcessInformation
	{
		public nint hProcess;

		public nint hThread;

		public int dwProcessId;

		public int dwThreadId;
	}

	[Flags]
	public enum CreateProcessFlags : uint
	{
		DEBUG_PROCESS = 1u,
		DEBUG_ONLY_THIS_PROCESS = 2u,
		CREATE_SUSPENDED = 4u,
		DETACHED_PROCESS = 8u,
		CREATE_NEW_CONSOLE = 0x10u,
		NORMAL_PRIORITY_CLASS = 0x20u,
		IDLE_PRIORITY_CLASS = 0x40u,
		HIGH_PRIORITY_CLASS = 0x80u,
		REALTIME_PRIORITY_CLASS = 0x100u,
		CREATE_NEW_PROCESS_GROUP = 0x200u,
		CREATE_UNICODE_ENVIRONMENT = 0x400u,
		CREATE_SEPARATE_WOW_VDM = 0x800u,
		CREATE_SHARED_WOW_VDM = 0x1000u,
		CREATE_FORCEDOS = 0x2000u,
		BELOW_NORMAL_PRIORITY_CLASS = 0x4000u,
		ABOVE_NORMAL_PRIORITY_CLASS = 0x8000u,
		INHERIT_PARENT_AFFINITY = 0x10000u,
		INHERIT_CALLER_PRIORITY = 0x20000u,
		CREATE_PROTECTED_PROCESS = 0x40000u,
		EXTENDED_STARTUPINFO_PRESENT = 0x80000u,
		PROCESS_MODE_BACKGROUND_BEGIN = 0x100000u,
		PROCESS_MODE_BACKGROUND_END = 0x200000u,
		CREATE_BREAKAWAY_FROM_JOB = 0x1000000u,
		CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000u,
		CREATE_DEFAULT_ERROR_MODE = 0x4000000u,
		CREATE_NO_WINDOW = 0x8000000u,
		PROFILE_USER = 0x10000000u,
		PROFILE_KERNEL = 0x20000000u,
		PROFILE_SERVER = 0x40000000u,
		CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000u
	}

	[Flags]
	public enum DuplicateOptions : uint
	{
		DUPLICATE_CLOSE_SOURCE = 1u,
		DUPLICATE_SAME_ACCESS = 2u
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern nint GetEnvironmentStrings();

	[DllImport("kernel32.dll")]
	public static extern uint ResumeThread(nint hThread);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, SecurityAttributes lpProcessAttributes, SecurityAttributes lpThreadAttributes, bool bInheritHandles, CreateProcessFlags dwCreationFlags, nint lpEnvironment, string? lpCurrentDirectory, [In] StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessAsUser(nint hToken, string lpApplicationName, [In] StringBuilder lpCommandLine, SecurityAttributes lpProcessAttributes, SecurityAttributes lpThreadAttributes, bool bInheritHandles, CreateProcessFlags dwCreationFlags, nint lpEnvironment, string lpCurrentDirectory, [In] StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessWithLogon(string lpUserName, string lpDomain, string lpPassword, LogonFlags dwLogonFlags, string lpApplicationName, StringBuilder lpCommandLine, CreateProcessFlags dwCreationFlags, nint lpEnvironment, string lpCurrentDirectory, [In] StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

	[DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern bool CreateProcessWithToken(nint hToken, LogonFlags dwLogonFlags, string lpApplicationName, string lpCommandLine, CreateProcessFlags dwCreationFlags, nint lpEnvironment, string lpCurrentDirectory, [In] StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);
}
