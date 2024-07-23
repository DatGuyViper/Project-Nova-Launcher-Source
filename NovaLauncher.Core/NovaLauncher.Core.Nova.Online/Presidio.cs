using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NovaLauncher.Core.Epic.Extensions;
using NovaLauncher.Core.Epic.OnlineServices;
using NovaLauncher.Core.Epic.OnlineServices.Mcp;
using NovaLauncher.Core.Epic.Types.Account;
using NovaLauncher.Core.Epic.Types.Error;
using NovaLauncher.Core.Nova.Types.Presidio;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;

namespace NovaLauncher.Core.Nova.Online;

public class Presidio
{
	private const string Protocol = "https";

	private const string Domain = "api.novafn.dev";

	private const string BasePath = "nova";

	private const string GrantTokenUrl = "/api/presidio/token";

	private static RestClient _client = new RestClient("https://api.novafn.dev/nova", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);

	private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public static async Task<PresidioToken?> GetToken(ExchangeCode exchangeCode)
	{
		FUserOnlineAccountMcp fUserOnlineAccountMcp = (FUserOnlineAccountMcp)IOnlineIdentity.Get().GetOnlineAccount();
		if (fUserOnlineAccountMcp == null)
		{
			Log.Information("GetPresidioToken request failed. No logged in user!");
			return null;
		}
		RestRequest val = new RestRequest("/api/presidio/token", (Method)1);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)_client, val).ToString();
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(fUserOnlineAccountMcp);
		RestRequestExtensions.AddJsonBody(val, new
		{
			exchangeCode = exchangeCode.Code
		}, (ContentType)null);
		Log.Information<string>("Sending GetPresidioToken request. url={url}", url);
		RestResponse val2 = await _client.ExecuteAsync(val, default(CancellationToken));
		if (((RestResponseBase)val2).Content == null)
		{
			return null;
		}
		if (!((RestResponseBase)val2).IsSuccessful)
		{
			EpicError epicError = JsonSerializer.Deserialize<EpicError>(((RestResponseBase)val2).Content, _jsonSerializerOptions);
			string text = "???";
			if (epicError != null)
			{
				text = epicError.ErrorMessage;
			}
			Log.Information<string>("GetPresidioToken request failed. {error}", text);
			return null;
		}
		PresidioToken presidioToken = JsonSerializer.Deserialize<PresidioToken>(((RestResponseBase)val2).Content, _jsonSerializerOptions);
		if (presidioToken == null)
		{
			return null;
		}
		Log.Information<string, int>("GetPresidioToken request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return presidioToken;
	}
}
