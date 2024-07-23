using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Core.Epic.OnlineServices;
using NovaLauncher.Core.Epic.OnlineServices.Mcp;
using NovaLauncher.Core.Nova;
using NovaLauncher.Web.Types.IPC;

namespace NovaLauncher.Web.Bridge.Auth;

[ComVisible(true)]
public class AuthExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	public AuthExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
		 
	}

	public bool IsLoggedIn()
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			return false;
		}
		return !fUserOnlineAccountMcp.HasAccessTokenExpired(DateTime.UtcNow);
	}

	public async Task Logout()
	{
		await IOnlineIdentity.Get().Logout();
		AuthFileManager authFileManager = AuthFileManager.Get();
		AuthFile authFile = authFileManager.GetAuthFile();
		if (authFile.AccessToken != null && authFile.RefreshToken != null)
		{
			authFile.AccessToken = null;
			authFile.RefreshToken = null;
			await authFileManager.Save();
		}
	}

	public async Task<bool> AttemptSessionRestore()
	{
		AuthFileManager authManager = AuthFileManager.Get();
		AuthFile authFile = authManager.GetAuthFile();
		if (authFile.AccessToken == null || authFile.RefreshToken == null)
		{
			return false;
		}
		FOnlineIdentityMcp fOnlineIdentityMcp = IOnlineIdentity.Get();
		FUserOnlineAccountMcp onlineAccount;
		try
		{
			onlineAccount = await fOnlineIdentityMcp.Login(new FOnlineAccountCredentials
			{
				Token = authFile.RefreshToken,
				Type = "refresh_token"
			});
			if (onlineAccount != null)
			{
				authFile.AccessToken = onlineAccount.GetAccessToken();
				authFile.RefreshToken = onlineAccount.GetRefreshToken();
			}
		}
		catch
		{
			authFile.AccessToken = null;
			authFile.RefreshToken = null;
			return false;
		}
		await authManager.Save();
		return onlineAccount != null;
	}

	public async Task InitializeCallbackServer()
	{
		HttpListener listener = new HttpListener
		{
			Prefixes = { "http://localhost:54930/" }
		};
		try
		{
			listener.Start();
		}
		catch (Exception)
		{
			string error = "failed-request";
			string responseJson = JsonSerializer.Serialize(new LoginMessage(success: false, null, error));
			((DispatcherObject)(object)_003CwebView_003EP).Dispatcher.Invoke(delegate
			{
				((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(responseJson);
			});
			return;
		}
		HttpListenerContext httpListenerContext = await listener.GetContextAsync();
		HttpListenerRequest request = httpListenerContext.Request;
		HttpListenerResponse response = httpListenerContext.Response;
		try
		{
			if (request.Url.AbsolutePath != "/callback")
			{
				response.StatusCode = 404;
				response.Close();
				return;
			}
			string code = request.QueryString["code"];
			FOnlineIdentityMcp onlineIdentity = IOnlineIdentity.Get();
			if (onlineIdentity.GetOnlineAccount() != null)
			{
				await onlineIdentity.Logout();
			}
			FUserOnlineAccountMcp fUserOnlineAccountMcp = await onlineIdentity.Login(new FOnlineAccountCredentials
			{
				Token = code,
				Type = "authorization_code"
			});
			string responseJson;
			if (fUserOnlineAccountMcp != null)
			{
				responseJson = JsonSerializer.Serialize(new LoginMessage(success: true, fUserOnlineAccountMcp.GetDisplayName()));
				AuthFileManager authFileManager = AuthFileManager.Get();
				authFileManager.GetAuthFile().AccessToken = fUserOnlineAccountMcp.GetAccessToken();
				authFileManager.GetAuthFile().RefreshToken = fUserOnlineAccountMcp.GetRefreshToken();
				await authFileManager.Save();
			}
			else
			{
				responseJson = JsonSerializer.Serialize(new LoginMessage(success: false, null, "failed-request"));
			}
			((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(responseJson);
		}
		catch (Exception)
		{
			string error2 = "failed-request";
			string responseJson = JsonSerializer.Serialize(new LoginMessage(success: false, null, error2));
			((DispatcherObject)(object)_003CwebView_003EP).Dispatcher.Invoke(delegate
			{
				((WebView2)_003CwebView_003EP).CoreWebView2.PostWebMessageAsJson(responseJson);
			});
		}
		response.StatusCode = 200;
		response.ContentType = "text/plain";
		response.OutputStream.Write(Array.Empty<byte>(), 0, 0);
		response.ContentLength64 = 0L;
		response.OutputStream.Close();
		response.Close();
		listener.Stop();
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("auth", (object)this);
	}
}
