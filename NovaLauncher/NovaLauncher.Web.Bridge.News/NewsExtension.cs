using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Core.Epic.Extensions;
using NovaLauncher.Core.Epic.OnlineServices;
using NovaLauncher.Core.Epic.OnlineServices.Mcp;
using RestSharp;
using RestSharp.Authenticators;

namespace NovaLauncher.Web.Bridge.News;

[ComVisible(true)]
public class NewsExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	private const string NewsUrl = "/api/launcher/news";

	public NewsExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
		 
	}

	public async Task<string> GetNews()
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			return "[]";
		}
		RestRequest val = new RestRequest("/api/launcher/news", (Method)0);
		BuildUriExtensions.BuildUri((IRestClient)(object)HttpClient.Client, val).ToString();
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		RestResponse val2 = await HttpClient.Client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return "[]";
		}
		if (!((RestResponseBase)val2).IsSuccessful)
		{
			return "[]";
		}
		return ((RestResponseBase)val2).Content;
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("news", (object)this);
	}
}
