using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using NovaLauncher.Core.Epic.Types.Account;
using NovaLauncher.Core.Nova.Types.Presidio;
using NovaLauncher.Core.Utilities;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Memory;
using Windows.Win32.System.Threading;

namespace NovaLauncher.Core.Nova;

public class Game
{
	public Process _gameProcess;

	private Process _beProcess;

	private Process? _launcherProcess;

	private Process? _agentProcess;

	public Game(Process gameProcess, Process beProcess, Process? launcherProcess, Process? agentProcess)
	{
		_gameProcess = gameProcess;
		_beProcess = beProcess;
		_launcherProcess = launcherProcess;
		_agentProcess = agentProcess;
		_gameProcess.EnableRaisingEvents = true;
		_gameProcess.Exited += GameExited;
	}

	public void Exit()
	{
		if (!_gameProcess.HasExited)
		{
			_gameProcess.Kill();
		}
		if (!_beProcess.HasExited)
		{
			_beProcess.Kill();
		}
		if (_launcherProcess != null && !_launcherProcess.HasExited)
		{
			_launcherProcess.Kill();
		}
		if (_agentProcess != null && !_agentProcess.HasExited)
		{
			_agentProcess.Kill();
		}
	}

	private void GameExited(object? sender, EventArgs e)
	{
		if (!_beProcess.HasExited)
		{
			_beProcess.Kill();
		}
		if (_launcherProcess != null && !_launcherProcess.HasExited)
		{
			_launcherProcess.Kill();
		}
		if (_agentProcess != null && !_agentProcess.HasExited)
		{
			_agentProcess.Kill();
		}
	}

	internal static Process? CreateProcess(string lpApplicationName, string lpCommandLine = "", bool suspended = false)
	{
		Native.StartupInfo lpStartupInfo = new Native.StartupInfo();
		Native.SecurityAttributes lpProcessAttributes = new Native.SecurityAttributes();
		Native.SecurityAttributes lpThreadAttributes = new Native.SecurityAttributes();
		if (!Native.CreateProcess(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles: true, suspended ? Native.CreateProcessFlags.CREATE_SUSPENDED : ((Native.CreateProcessFlags)0u), 0, null, lpStartupInfo, out var lpProcessInformation))
		{
			return null;
		}
		return Process.GetProcessById(lpProcessInformation.dwProcessId);
	}

	public static void KillOldPresidioProcess()
	{
		Process.GetProcessesByName("PresidioAgent.exe").FirstOrDefault()?.Kill();
	}

	public unsafe static Game? Start(Installation installation, ExchangeCode exchangeCode, out string? error, PresidioToken? presidioToken = null)
	{
		KillOldPresidioProcess();
		error = null;
		if (exchangeCode == null)
		{
			error = "no-exchange-code";
			return null;
		}
		Settings settings = SettingsManager.Get().GetSettings();
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher", "Assets");
		string lpApplicationName = Path.Combine(installation.Path, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe");
		string text = " -epicportal -AUTH_TYPE=exchangecode -AUTH_LOGIN=none -AUTH_PASSWORD=" + exchangeCode.Code + " -epiclocale=en-us -fltoken=none -fromfl=7a848a93a74ba68876c36C1c -noeac -nobe";
		if (settings.Mods.EditOnRelease)
		{
			text += " -eor";
		}
		if (settings.Mods.InstantRelease)
		{
			text += " -instantreset";
		}
		if (settings.Mods.Linear)
		{
			text += " -linear";
		}
		if (settings.Mods.Potato)
		{
			text += " -potato";
		}
		if (settings.Mods.DisablePreEdit)
		{
			text += " -preedit";
		}
		if (settings.Mods.SprintByDefault)
		{
			text += " -sprintbydefault";
		}
		Process process = null;
		if (presidioToken != null && !Environment.GetCommandLineArgs().Any((string x) => x.ToLower() == "-noac"))
		{
			text = text + " -p=" + presidioToken.P;
			process = Process.Start(new ProcessStartInfo
			{
				FileName = Path.Combine(path, "Presidio\\PresidioAgent.exe"),
				Arguments = "-p=" + presidioToken.P,
				UseShellExecute = true,
				CreateNoWindow = true,
				Verb = "runas"
			});
			if (process == null)
			{
				error = "no-presidio-agent";
				return null;
			}
		}
		Process process2 = CreateProcess(lpApplicationName, text, suspended: true);
		if (process2 == null)
		{
			error = "no-game-running";
			return null;
		}
		Process process3 = CreateProcess(Path.Combine(installation.Path, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe"), "", suspended: true);
		if (process3 == null)
		{
			process2.Kill();
			process?.Kill();
			error = "no-be-running";
			return null;
		}
		string text2 = Path.Combine(installation.Path, "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
		Process process4 = CreateProcess(text2, "", suspended: true);
		if (process4 == null && File.Exists(text2))
		{
			process2.Kill();
			process3.Kill();
			process?.Kill();
			error = "no-launcher-running";
			return null;
		}
		string text3 = Path.Combine(path, "NovaLauncher.Client.dll");
		SafeFileHandle hProcess = PInvoke.OpenProcess_SafeHandle(PROCESS_ACCESS_RIGHTS.PROCESS_ALL_ACCESS, false, (uint)process2.Id);
		FARPROC procAddress = PInvoke.GetProcAddress(PInvoke.LoadLibrary("kernel32.dll"), "LoadLibraryW");
		uint num = (uint)((text3.Length + 1) * 2);
		void* ptr = PInvoke.VirtualAllocEx(hProcess, null, num, VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT | VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE, PAGE_PROTECTION_FLAGS.PAGE_READWRITE);
		fixed (byte* lpBuffer = Encoding.Unicode.GetBytes(text3 + "\0"))
		{
			PInvoke.WriteProcessMemory(hProcess, ptr, lpBuffer, num, null);
		}
		PInvoke.WaitForSingleObject(PInvoke.CreateRemoteThread(hProcess, null, 0u, Marshal.GetDelegateForFunctionPointer<LPTHREAD_START_ROUTINE>((nint)procAddress), ptr, 0u, null), 500u);
		PInvoke.VirtualFreeEx(hProcess, ptr, num, VIRTUAL_FREE_TYPE.MEM_RELEASE);
		return new Game(process2, process3, process4, process);
	}
}
