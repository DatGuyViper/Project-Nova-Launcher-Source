using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using NovaLauncher.Core.Epic.Extensions;
using NovaLauncher.Core.Epic.OnlineServices;
using NovaLauncher.Core.Epic.OnlineServices.Mcp;
using NovaLauncher.Web.Utils;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;

namespace NovaLauncher.Web.Bridge.Utils;

[ComVisible(true)]
public class UtilsExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	private const string LauncherDataUrl = "/api/launcher/info";

	private const string UserInfoUrl = "/api/launcher/user";

	private const string CheckUsernameUrl = "/api/launcher/onboarding/username";

	private const string FinishOnboardingUrl = "/api/launcher/onboarding/finish";

	private const string ChangeUsernameUrl = "/api/launcher/user/username";

	private const string CdnBaseUrl = "https://cdn.novafn.dev";

	public UtilsExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
		 
	}

	public bool IsPresidioDownloadEnabled()
	{
		return !Environment.GetCommandLineArgs().Any((string x) => x.ToLower() == "-noacdl");
	}

	public bool IsClientDllDownloadEnabled()
	{
		return !Environment.GetCommandLineArgs().Any((string x) => x.ToLower() == "-nodlldl");
	}

	public void OpenUri(string url)
	{
		Process.Start(new ProcessStartInfo(url)
		{
			UseShellExecute = true
		});
	}

	public void SetVisitorId(string fp)
	{
		RestClientExtensions.AddDefaultHeader((IRestClient)(object)HttpClient.Client, "x-user-id", fp);
	}

	public void SetupCrashReporter(string gamePath)
	{
		try
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string text = Path.Combine(folderPath, "CrashReportClient", "Saved", "Config", "Windows");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			StreamWriter streamWriter = File.CreateText(Path.Combine(text, "Engine.ini"));
			streamWriter.Write("[CrashReportClient]\r\nNovaRouterUrl=\"https://sentry.novafn.dev/api/3/unreal/aad683c76fbd0c53eb2e38f908f6d90b/\"");
			streamWriter.Close();
			if (BinarySearch.FindByteSequence(File.ReadAllBytes(Path.Combine(gamePath, "Engine", "Binaries", "Win64", "CrashReportClient.exe")), new byte[25]
			{
				78, 0, 111, 0, 118, 0, 97, 0, 82, 0,
				111, 0, 117, 0, 116, 0, 101, 0, 114, 0,
				85, 0, 114, 0, 108
			}) == -1)
			{
				return;
			}
			using BinaryReplacer binaryReplacer = new BinaryReplacer(File.Open(Path.Combine(gamePath, "Engine", "Binaries", "Win64", "CrashReportClient.exe"), FileMode.Open));
			binaryReplacer.Replace(new byte[25]
			{
				68, 0, 97, 0, 116, 0, 97, 0, 82, 0,
				111, 0, 117, 0, 116, 0, 101, 0, 114, 0,
				85, 0, 114, 0, 108
			}, new byte[25]
			{
				78, 0, 111, 0, 118, 0, 97, 0, 82, 0,
				111, 0, 117, 0, 116, 0, 101, 0, 114, 0,
				85, 0, 114, 0, 108
			});
		}
		catch (Exception)
		{
		}
	}

	public async Task<bool> DownloadAsset(string uri, string relativePath, bool isPresidio)
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher", "Assets");
		if (isPresidio)
		{
			text = Path.Combine(text, "Presidio");
		}
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string downloadToPath = Path.Combine(text, relativePath);
		RestClient val = new RestClient("https://cdn.novafn.dev", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);
		RestRequest val2 = new RestRequest(uri, (Method)0);
		BuildUriExtensions.BuildUri((IRestClient)val, val2).ToString();
		RestResponse val3 = await RestClientExtensions.GetAsync((IRestClient)(object)HttpClient.Client, val2, default(CancellationToken));
		if (!((RestResponseBase)val3).IsSuccessful || ((RestResponseBase)val3).RawBytes.Length == 0)
		{
			return false;
		}
		File.WriteAllBytes(downloadToPath, ((RestResponseBase)val3).RawBytes);
		return true;
	}

    public async Task<string> AreServicesAlive()
    {
        try
        {
            RestRequest val = new RestRequest("/healthcheck", Method.Get);
			val.Timeout = TimeSpan.FromMilliseconds(3000);
            RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
            if (val2.IsSuccessful)
            {
                return val2.Content == "ok" ? "ok" : "maintenance";
            }
        }
        catch
        {
            return "ko";
        }
        return "ko";
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

	public async Task<string> GetLauncherData()
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("GetLauncherData request failed. No logged in user!");
			return "{}";
		}
		RestRequest val = new RestRequest("/api/launcher/info", (Method)0);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		Log.Information<string>("Sending GetLauncherData request. url={url}", url);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return "{}";
		}
		if (!((RestResponseBase)val2).IsSuccessful)
		{
			return "{}";
		}
		Log.Information<string, int>("GetLauncherData request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return ((RestResponseBase)val2).Content;
	}

	public async Task<string> IsUsernameAvailable(string username)
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("IsUsernameAvailable request failed. No logged in user!");
			return "failed";
		}
		RestRequest val = new RestRequest("/api/launcher/onboarding/username", (Method)1);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		RestRequestExtensions.AddJsonBody(val, new
		{
			newDisplayName = username
		}, (ContentType)null);
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		Log.Information<string>("Sending IsUsernameAvailable request. url={url}", url);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		Log.Information<string, int>("IsUsernameAvailable request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return ((RestResponseBase)val2).StatusCode switch
		{
			HttpStatusCode.NoContent => "ok", 
			HttpStatusCode.Conflict => "taken", 
			HttpStatusCode.BadRequest => "invalid", 
			_ => "failed", 
		};
	}

	public async Task<string> ChangeDisplayName(string newDisplayName)
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("ChangeDisplayName request failed. No logged in user!");
			return "failed";
		}
		RestRequest val = new RestRequest("/api/launcher/user/username", (Method)6);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		RestRequestExtensions.AddJsonBody(val, new { newDisplayName }, (ContentType)null);
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		Log.Information<string>("Sending ChangeDisplayName request. url={url}", url);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return "failed";
		}
		Log.Information<string, int>("ChangeDisplayName request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return ((RestResponseBase)val2).StatusCode switch
		{
			HttpStatusCode.NoContent => "ok", 
			HttpStatusCode.Conflict => "taken", 
			HttpStatusCode.BadRequest => "invalid", 
			_ => "failed", 
		};
	}

	public async Task<bool> FinishOnboarding(string jsonData)
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("FinishOnboarding request failed. No logged in user!");
			return false;
		}
		RestRequest val = new RestRequest("/api/launcher/onboarding/finish", (Method)1);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		RestRequestExtensions.AddJsonBody<string>(val, jsonData, (ContentType)null);
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		Log.Information<string>("Sending FinishOnboarding request. url={url}", url);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return false;
		}
		if (!((RestResponseBase)val2).IsSuccessful)
		{
			return false;
		}
		Log.Information<string, int>("FinishOnboarding request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return ((RestResponseBase)val2).StatusCode == HttpStatusCode.OK;
	}

	public async Task<string> GetUserInfo()
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("GetUserInfo request failed. No logged in user!");
			return "{}";
		}
		RestRequest val = new RestRequest("/api/launcher/user", (Method)0);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		Log.Information<string>("Sending GetUserInfo request. url={url}", url);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return "{}";
		}
		if (!((RestResponseBase)val2).IsSuccessful)
		{
			return "{}";
		}
		Log.Information<string, int>("GetUserInfo request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return ((RestResponseBase)val2).Content;
	}

	public Task<string> PromptForDirectory()
	{
		TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
		SynchronizationContext.Current.Post(delegate
		{
			OpenFolderDialog openFolderDialog = new OpenFolderDialog();
			if (openFolderDialog.ShowDialog().GetValueOrDefault())
			{
				tcs.SetResult(openFolderDialog.FolderName);
			}
			else
			{
				tcs.SetResult(string.Empty);
			}
		}, null);
		return tcs.Task;
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("utils", (object)this);
	}
}
