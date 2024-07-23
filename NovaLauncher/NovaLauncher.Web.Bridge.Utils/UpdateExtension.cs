using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Web.Types.IPC;
using NovaLauncher.Web.Types.Responses;
using RestSharp;

namespace NovaLauncher.Web.Bridge.Utils;

[ComVisible(true)]
public class UpdateExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	private const string UpdateDataUrl = "/api/launcher/update";

	private UpdateResponse _lastUpdateData;

	public UpdateExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
		 
	}

	public async Task<bool> IsUpdateRequired()
	{
		if (Environment.GetCommandLineArgs().Any((string x) => x.ToLower() == "-noupdate"))
		{
			return false;
		}
		RestRequest val = new RestRequest("/api/launcher/update", (Method)0);
		BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (!((RestResponseBase)val2).IsSuccessful || ((RestResponseBase)val2).Content == null)
		{
			return false;
		}
		_lastUpdateData = JsonSerializer.Deserialize<UpdateResponse>(((RestResponseBase)val2).Content);
		return _lastUpdateData.Version != GetCurrentVersion();
	}

	public async void LaunchUpdate()
	{
		try
		{
			if (_lastUpdateData == null || string.IsNullOrEmpty(_lastUpdateData.Url))
			{
				((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new UpdateStatus(0, "retrieve-failed")));
				return;
			}
			RestRequest val = new RestRequest(_lastUpdateData.Url, (Method)0);
			BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
			RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
			if (!((RestResponseBase)val2).IsSuccessful || ((RestResponseBase)val2).RawBytes == null)
			{
				((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new UpdateStatus(0, "retrieve-failed")));
				return;
			}
			byte[] rawBytes = ((RestResponseBase)val2).RawBytes;
			string tempPath = Path.GetTempPath();
			string path = Path.Combine(tempPath, "Update.msi");
			string text = Path.Combine(tempPath, "Update.bat");
			string text2 = Extensions.GetExecutablePath();
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Assembly.GetExecutingAssembly().Location;
			}
			string text3 = Path.Combine(text2.Substring(0, text2.LastIndexOf("\\")), "NovaLauncher.Web.exe");
			File.WriteAllBytes(path, rawBytes);
			using (StreamWriter streamWriter = new StreamWriter(File.Create(text)))
			{
				streamWriter.WriteLine("@echo off");
				streamWriter.WriteLine("start /wait msiexec.exe /i Update.msi /qb");
				streamWriter.WriteLine("del Update.exe");
				streamWriter.WriteLine("start \"\" \"" + text3 + "\"");
				streamWriter.WriteLine("del %0");
			}
			Process.Start(new ProcessStartInfo(text)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = tempPath
			});
			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			MessageBox.Show("We encountered an error in the application, please retry. Information: " + ex.Message);
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new UpdateStatus(0, "failed")));
		}
	}

	public string GetCurrentVersion()
	{
		return Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("updater", (object)this);
	}
}
