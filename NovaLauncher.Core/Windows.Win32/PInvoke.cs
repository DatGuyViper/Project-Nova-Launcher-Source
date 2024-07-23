using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.Memory;
using Windows.Win32.System.Threading;

namespace Windows.Win32;

[GeneratedCode("Microsoft.Windows.CsWin32", "0.3.106+a37a0b4b70")]
internal static class PInvoke
{
	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.0")]
	internal static extern BOOL CloseHandle(HANDLE hObject);

	[SupportedOSPlatform("windows5.1.2600")]
	internal static SafeFileHandle OpenThread_SafeHandle(THREAD_ACCESS_RIGHTS dwDesiredAccess, BOOL bInheritHandle, uint dwThreadId)
	{
		return new SafeFileHandle((nint)OpenThread(dwDesiredAccess, bInheritHandle, dwThreadId), ownsHandle: true);
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern HANDLE OpenThread(THREAD_ACCESS_RIGHTS dwDesiredAccess, BOOL bInheritHandle, uint dwThreadId);

	[SupportedOSPlatform("windows5.1.2600")]
	internal static uint SuspendThread(SafeHandle hThread)
	{
		bool success = false;
		try
		{
			if (hThread != null)
			{
				hThread.DangerousAddRef(ref success);
				HANDLE hThread2 = (HANDLE)hThread.DangerousGetHandle();
				return SuspendThread(hThread2);
			}
			throw new ArgumentNullException("hThread");
		}
		finally
		{
			if (success)
			{
				hThread.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern uint SuspendThread(HANDLE hThread);

	[SupportedOSPlatform("windows5.1.2600")]
	internal static SafeFileHandle OpenProcess_SafeHandle(PROCESS_ACCESS_RIGHTS dwDesiredAccess, BOOL bInheritHandle, uint dwProcessId)
	{
		return new SafeFileHandle((nint)OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId), ownsHandle: true);
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern HANDLE OpenProcess(PROCESS_ACCESS_RIGHTS dwDesiredAccess, BOOL bInheritHandle, uint dwProcessId);

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern BOOL FreeLibrary(HMODULE hLibModule);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static FARPROC GetProcAddress(SafeHandle hModule, string lpProcName)
	{
		bool success = false;
		try
		{
			fixed (byte* value = ((lpProcName != null) ? Encoding.Default.GetBytes(lpProcName) : null))
			{
				if (hModule != null)
				{
					hModule.DangerousAddRef(ref success);
					HMODULE hModule2 = (HMODULE)hModule.DangerousGetHandle();
					return GetProcAddress(hModule2, new PCSTR(value));
				}
				throw new ArgumentNullException("hModule");
			}
		}
		finally
		{
			if (success)
			{
				hModule.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern FARPROC GetProcAddress(HMODULE hModule, PCSTR lpProcName);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static void* VirtualAllocEx(SafeHandle hProcess, void* lpAddress, nuint dwSize, VIRTUAL_ALLOCATION_TYPE flAllocationType, PAGE_PROTECTION_FLAGS flProtect)
	{
		bool success = false;
		try
		{
			if (hProcess != null)
			{
				hProcess.DangerousAddRef(ref success);
				HANDLE hProcess2 = (HANDLE)hProcess.DangerousGetHandle();
				return VirtualAllocEx(hProcess2, lpAddress, dwSize, flAllocationType, flProtect);
			}
			throw new ArgumentNullException("hProcess");
		}
		finally
		{
			if (success)
			{
				hProcess.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static extern void* VirtualAllocEx(HANDLE hProcess, [Optional] void* lpAddress, nuint dwSize, VIRTUAL_ALLOCATION_TYPE flAllocationType, PAGE_PROTECTION_FLAGS flProtect);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static BOOL VirtualFreeEx(SafeHandle hProcess, void* lpAddress, nuint dwSize, VIRTUAL_FREE_TYPE dwFreeType)
	{
		bool success = false;
		try
		{
			if (hProcess != null)
			{
				hProcess.DangerousAddRef(ref success);
				HANDLE hProcess2 = (HANDLE)hProcess.DangerousGetHandle();
				return VirtualFreeEx(hProcess2, lpAddress, dwSize, dwFreeType);
			}
			throw new ArgumentNullException("hProcess");
		}
		finally
		{
			if (success)
			{
				hProcess.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static extern BOOL VirtualFreeEx(HANDLE hProcess, void* lpAddress, nuint dwSize, VIRTUAL_FREE_TYPE dwFreeType);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static BOOL WriteProcessMemory(SafeHandle hProcess, void* lpBaseAddress, void* lpBuffer, nuint nSize, nuint* lpNumberOfBytesWritten)
	{
		bool success = false;
		try
		{
			if (hProcess != null)
			{
				hProcess.DangerousAddRef(ref success);
				HANDLE hProcess2 = (HANDLE)hProcess.DangerousGetHandle();
				return WriteProcessMemory(hProcess2, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesWritten);
			}
			throw new ArgumentNullException("hProcess");
		}
		finally
		{
			if (success)
			{
				hProcess.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static extern BOOL WriteProcessMemory(HANDLE hProcess, void* lpBaseAddress, void* lpBuffer, nuint nSize, [Optional] nuint* lpNumberOfBytesWritten);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static SafeFileHandle CreateRemoteThread(SafeHandle hProcess, SECURITY_ATTRIBUTES? lpThreadAttributes, nuint dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, void* lpParameter, uint dwCreationFlags, uint* lpThreadId)
	{
		bool success = false;
		try
		{
			if (hProcess != null)
			{
				hProcess.DangerousAddRef(ref success);
				HANDLE hProcess2 = (HANDLE)hProcess.DangerousGetHandle();
				SECURITY_ATTRIBUTES valueOrDefault = lpThreadAttributes.GetValueOrDefault();
				return new SafeFileHandle((nint)CreateRemoteThread(hProcess2, lpThreadAttributes.HasValue ? (&valueOrDefault) : null, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId), ownsHandle: true);
			}
			throw new ArgumentNullException("hProcess");
		}
		finally
		{
			if (success)
			{
				hProcess.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static extern HANDLE CreateRemoteThread(HANDLE hProcess, [Optional] SECURITY_ATTRIBUTES* lpThreadAttributes, nuint dwStackSize, LPTHREAD_START_ROUTINE lpStartAddress, [Optional] void* lpParameter, uint dwCreationFlags, [Optional] uint* lpThreadId);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static FreeLibrarySafeHandle GetModuleHandle(string lpModuleName)
	{
		fixed (char* ptr = lpModuleName)
		{
			return new FreeLibrarySafeHandle((nint)GetModuleHandle(ptr), ownsHandle: false);
		}
	}

	[DllImport("KERNEL32.dll", EntryPoint = "GetModuleHandleW", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern HMODULE GetModuleHandle(PCWSTR lpModuleName);

	[SupportedOSPlatform("windows5.1.2600")]
	internal unsafe static FreeLibrarySafeHandle LoadLibrary(string lpLibFileName)
	{
		fixed (char* ptr = lpLibFileName)
		{
			return new FreeLibrarySafeHandle((nint)LoadLibrary(ptr));
		}
	}

	[DllImport("KERNEL32.dll", EntryPoint = "LoadLibraryW", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern HMODULE LoadLibrary(PCWSTR lpLibFileName);

	[SupportedOSPlatform("windows5.1.2600")]
	internal static WAIT_EVENT WaitForSingleObject(SafeHandle hHandle, uint dwMilliseconds)
	{
		bool success = false;
		try
		{
			if (hHandle != null)
			{
				hHandle.DangerousAddRef(ref success);
				HANDLE hHandle2 = (HANDLE)hHandle.DangerousGetHandle();
				return WaitForSingleObject(hHandle2, dwMilliseconds);
			}
			throw new ArgumentNullException("hHandle");
		}
		finally
		{
			if (success)
			{
				hHandle.DangerousRelease();
			}
		}
	}

	[DllImport("KERNEL32.dll", ExactSpelling = true, SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[SupportedOSPlatform("windows5.1.2600")]
	internal static extern WAIT_EVENT WaitForSingleObject(HANDLE hHandle, uint dwMilliseconds);
}
