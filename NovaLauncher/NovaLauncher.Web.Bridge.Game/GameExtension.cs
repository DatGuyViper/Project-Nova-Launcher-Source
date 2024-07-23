using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Core.Epic.OnlineServices;
using NovaLauncher.Core.Epic.Types.Account;
using NovaLauncher.Core.Nova;
using NovaLauncher.Core.Nova.Online;
using NovaLauncher.Core.Nova.Types.Presidio;
using NovaLauncher.EasyInstaller;
using NovaLauncher.Web.Types.IPC;
using NovaLauncher.Web.Utils;
using Serilog;

namespace NovaLauncher.Web.Bridge.Game;

[ComVisible(true)]
public class GameExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	private CancellationTokenSource _ctsDownload { get; set; }

	private string _currentDownloadVersion { get; set; }

	private string _currentDownloadPath { get; set; }

	private NovaLauncher.Core.Nova.Game? _currentGameInstance { get; set; }

	public GameExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
	}

	private string ComputeMD5Hash(string path)
	{
		if (File.Exists(path))
		{
			using (MD5 mD = MD5.Create())
			{
				using FileStream inputStream = File.OpenRead(path);
				return BitConverter.ToString(mD.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
			}
		}
		return "";
	}

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

	public void PauseDownload()
	{
		Installer.PauseDownload = !Installer.PauseDownload;
	}

	public void CancelDownload()
	{
		_ctsDownload?.Cancel();
	}

	public string GetClientDllHash()
	{
		string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher"), "Assets", "NovaLauncher.Client.dll");
		return ComputeMD5Hash(path);
	}

	public string GetACAgentHash()
	{
		string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher"), "Assets", "Presidio", "PresidioAgent.exe");
		return ComputeMD5Hash(path);
	}

	public string GetACClientHash()
	{
		string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher"), "Assets", "Presidio", "Presidio.dll");
		return ComputeMD5Hash(path);
	}

	public string GetClientHash(string rootGamePath)
	{
		string path = Path.Combine(rootGamePath, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
		return ComputeMD5Hash(path);
	}

	public string CLToBuild(int CL)
	{
		return CL switch
		{
			2870186 => "OT6.5", 
			3532353 => $"Version-{CL}", 
			3541083 => "1.2", 
			3700114 => "1.7.2", 
			3724489 => "1.8", 
			3729133 => "1.8.1", 
			3741772 => "1.8.2", 
			3757339 => "1.9", 
			3775276 => "1.9.1", 
			3790078 => "1.10", 
			3807424 => "1.11", 
			3825894 => "2.1.0", 
			3841827 => "2.2.0", 
			3847564 => "2.3.0", 
			3858292 => "2.4.0", 
			3870737 => "2.4.2", 
			3889387 => "2.5", 
			_ => $"Version-{CL}", 
		};
	}

	public string GetClientVersion(string rootGamePath)
	{
		string path = Path.Combine(rootGamePath, "FortniteGame", "Binaries", "Win64", "FortniteClient-Win64-Shipping.exe");
		if (!File.Exists(path))
		{
			return "";
		}
		try
		{
			byte[] array = File.ReadAllBytes(path);
			byte[] array2 = new byte[38]
			{
				43, 0, 43, 0, 70, 0, 111, 0, 114, 0,
				116, 0, 110, 0, 105, 0, 116, 0, 101, 0,
				43, 0, 82, 0, 101, 0, 108, 0, 101, 0,
				97, 0, 115, 0, 101, 0, 45, 0
			};
			int num = FindByteSequence(array, array2);
			if (num != -1)
			{
				int num2 = num + array2.Length;
				int num3 = num2 + FindByteSequence(array.Skip(num2).ToArray(), Encoding.Unicode.GetBytes(".")) + 2;
				string text = Encoding.Unicode.GetString(array, num2, (num3 - num2) * 2).TrimEnd('-');
				Log.Information<string>("GetClientVersion: {version}", text);
				if (text.ToLowerInvariant().Contains("cert") || text.ToLowerInvariant().Contains("next"))
				{
					byte[] sequence = new byte[48]
					{
						43, 0, 43, 0, 70, 0, 111, 0, 114, 0,
						116, 0, 110, 0, 105, 0, 116, 0, 101, 0,
						43, 0, 82, 0, 101, 0, 108, 0, 101, 0,
						97, 0, 115, 0, 101, 0, 45, 0, 78, 0,
						101, 0, 120, 0, 116, 0, 45, 0
					};
					int num4 = FindByteSequence(array, sequence);
					if (num4 != -1)
					{
						int num5 = num4;
						int num6 = num5 + FindByteSequence(array.Skip(num5).ToArray(), new byte[2]) + 1;
						string text2 = Encoding.Unicode.GetString(array, num5, num6 - num5).TrimEnd();
						Log.Information<string>("GetClientVersion: {version}", text2);
						int cL = int.Parse(text2.Split("-").Last());
						return CLToBuild(cL);
					}
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Log.Information("Error getting client version: " + ex.Message);
			return "";
		}
		return "1.7.2";
	}

	public async Task Launch(string installationName)
	{
		ExchangeCode exchangeCode = await IOnlineIdentity.Get().GenerateExchangeCode();
		if (exchangeCode != null)
		{
			InstallationManager installationManager = InstallationManager.Get();
			Installation installation = installationManager.Find(installationName);
			if (installation == null)
			{
				((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false, "no-installation")));
				return;
			}
			PresidioToken presidioToken = await Presidio.GetToken(exchangeCode);
			if (presidioToken != null)
			{
				try
				{
					Notifications.ShowToast("Wait a few seconds while we load the game.", "Starting " + installationName, (ToastDuration)1);
					string error;
					NovaLauncher.Core.Nova.Game game = NovaLauncher.Core.Nova.Game.Start(installation, exchangeCode, out error, presidioToken);
					if (game != null)
					{
						_currentGameInstance = game;
						game._gameProcess.Exited += HandleGameExited;
						((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: true)));
					}
					else
					{
						((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false, "no-game", error)));
					}
					return;
				}
				catch (Exception ex)
				{
					((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false, ex.Message)));
					return;
				}
			}
			Log.Information("Invalid presidio token!");
		}
		else
		{
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false, "no-auth")));
		}
	}

	private void HandleGameExited(object? sender, EventArgs e)
	{
		((DispatcherObject)(object)_003CwebView_003EP).Dispatcher.Invoke(delegate
		{
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false)));
		});
	}

	public async void ExitGame()
	{
		if (_currentGameInstance != null)
		{
			_currentGameInstance.Exit();
			_currentGameInstance = null;
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new GameStatus(running: false)));
		}
	}

	public async Task ImportBuild(string path, string version)
	{
		InstallationManager installationManager = InstallationManager.Get();
		installationManager.Add(new Installation
		{
			Path = path,
			Version = version,
			Selected = false
		});
		await installationManager.Save();
	}

    public async Task RemoveBuild(string version)
    {
        InstallationManager installationManager = InstallationManager.Get();
        Installation installation = installationManager.GetInstallations()
            .FirstOrDefault(i => i.Version == version);
        if (installation != null)
        {
            installationManager.Remove(installation);
        }
        await installationManager.Save();
    }


    public async Task<bool> VerifyBuild(string path, string version, bool silent, bool fixFiles, bool fromLaunch)
	{
		ManifestFile manifestFile = await Installer.GetManifest(version);
		if (manifestFile == null)
		{
			string text = JsonSerializer.Serialize(new DownloadStatus(progress: false, completed: false, 0.0, 0.0, version, "no-manifest"));
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(text);
			return false;
		}
		Installer.FilesToDownload = manifestFile.Files.Count;
		Installer.DownloadedFiles = 0;
		if (!silent)
		{
			Notifications.ShowToast("Wait a few seconds while we ensure your game has no issues.", "Verifying " + version, (ToastDuration)0);
		}
		bool flag = await Installer.Verify(manifestFile, version, path);
		if (!silent && !fromLaunch)
		{
			if (flag)
			{
				Notifications.ShowToast("You can now launch your game.", "Verification successful!", (ToastDuration)0);
			}
			else
			{
				Notifications.ShowToast($"We are replacing the corrupted files in order to fix the game, this might take a bit. ({Installer.DownloadedFiles} / {Installer.FilesToDownload})", "Verification failed!", (ToastDuration)0);
			}
		}
		if (fixFiles && !flag)
		{
			_ctsDownload = new CancellationTokenSource();
			_currentDownloadVersion = version;
			_currentDownloadPath = path;
			await Installer.DownloadFromList(Installer.InvalidFiles.ToList(), version, path, _ctsDownload);
			Notifications.ShowToast("We repaired the corrupted files and the game has been fixed.", "Game repaired!", (ToastDuration)0);
			return true;
		}
		return flag;
	}

    public async Task InstallBuild(string buildPath, string buildVersion)
    {
        string version = buildVersion;
        string path = buildPath;

        ManifestFile manifest = await Installer.GetManifest(version);
        if (manifest == null)
        {
            string text = JsonSerializer.Serialize(new DownloadStatus(progress: false, completed: false, 0.0, 0.0, version, "no-manifest"));
            ((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(text);
            return;
        }

        Notifications.ShowToast("This will take some time so keep this in your background.", "Downloading " + version, (ToastDuration)0);
        Installer.DownloadProgressChanged += ReportProgress;
        Installer.DownloadCompletedEvent += ReportComplete;
        Installer.PauseDownload = false;
        _ctsDownload = new CancellationTokenSource();
        _currentDownloadVersion = version;
        _currentDownloadPath = path;

        Task.Run(async () =>
        {
            await Installer.Download(manifest, version, path, _ctsDownload);
        });
    }



    private void ReportProgress(object sender, DownloadProgressEventArgs args)
    {
        ((DispatcherObject)(object)_003CwebView_003EP).Dispatcher.Invoke(() =>
        {
            string text = JsonSerializer.Serialize(new DownloadStatus(
                progress: true,
                completed: false,
                args.ProgressPercentage,
                0.0,
                _currentDownloadVersion));
            ((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(text);
        });
    }


    private void ReportComplete(object sender, EventArgs e)
	{
		((DispatcherObject)(object)_003CwebView_003EP).Dispatcher.Invoke((Func<Task>)async delegate
		{
			Installation installation = new Installation
			{
				Path = _currentDownloadPath,
				Version = _currentDownloadVersion
			};
			InstallationManager installationManager = InstallationManager.Get();
			installationManager.Add(installation);
			await installationManager.Select(_currentDownloadVersion);
			await installationManager.Save();
			string text = JsonSerializer.Serialize(new DownloadStatus(progress: true, completed: true, 0.0, 0.0, _currentDownloadVersion));
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(text);
			Notifications.ShowToast("The version " + _currentDownloadVersion + " has been successfully installed.", "Download completed!", (ToastDuration)0);
			Installer.PauseDownload = false;
			_currentDownloadPath = null;
			_currentDownloadVersion = null;
		});
	}

	public void StopDownload()
	{
		if (_currentDownloadVersion != null)
		{
			_ctsDownload.Cancel();
			Notifications.ShowToast("The download for the version " + _currentDownloadVersion + " has been canceled.", "Download canceled!", (ToastDuration)0);
			string text = JsonSerializer.Serialize(new DownloadStatus(progress: false, completed: false, 0.0, 0.0, _currentDownloadVersion));
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(text);
			_currentDownloadPath = null;
			_currentDownloadVersion = null;
		}
	}

	public bool IsGameRunning()
	{
		return Process.GetProcessesByName("FortniteClient-Win64-Shipping.exe").Any();
	}

	public async Task<string> GetDownloadableBuilds()
	{
		return JsonSerializer.Serialize(await Installer.GetVersions());
	}

	public async Task<bool> SetMainBuild(string buildName)
	{
		return await InstallationManager.Get().Select(buildName);
	}

	public string GetInstallations()
	{
		return JsonSerializer.Serialize(InstallationManager.Get().GetInstallations());
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("game", (object)this);
	}
}
