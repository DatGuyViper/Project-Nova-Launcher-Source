using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NovaLauncher.Core.Epic.Extensions;
using NovaLauncher.Core.Epic.Types.Account;
using NovaLauncher.Core.Epic.Types.Error;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;

namespace NovaLauncher.Core.Epic.OnlineServices.Mcp;

public class FOnlineIdentityMcp : IOnlineIdentity
{
	private const string Protocol = "https";

	private const string BasePath = "account";

	private const string Domain = "api.novafn.dev";

	private const string GrantTokenUrl = "/api/oauth/token";

	private const string ExchangeTokenUrl = "/api/oauth/exchange";

	private const string KillTokenUrl = "/api/oauth/sessions/kill/{0}";

	private RestClient _client = new RestClient("https://api.novafn.dev/account", (ConfigureRestClient)null, (ConfigureHeaders)null, (ConfigureSerialization)null);

	private FUserOnlineAccountMcp? _onlineAccount;

	private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public async Task<FUserOnlineAccountMcp?> Login(FOnlineAccountCredentials accountCredentials)
	{
		RestRequest val = new RestRequest("/api/oauth/token", (Method)1);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)_client, val).ToString();
		IOnlineSubsystem onlineSubsystem = IOnlineSubsystem.Get();
		val.Authenticator = (IAuthenticator)new HttpBasicAuthenticator(onlineSubsystem.GetClientId(), onlineSubsystem.GetClientSecret());
		if (accountCredentials.Type == "epic")
		{
			RestRequestExtensions.AddParameter(val, "grant_type", "password", true);
			RestRequestExtensions.AddParameter(val, "username", accountCredentials.Id, true);
			RestRequestExtensions.AddParameter(val, "password", accountCredentials.Token, true);
		}
		else if (accountCredentials.Type == "exchangecode")
		{
			RestRequestExtensions.AddParameter(val, "grant_type", "exchange_code", true);
			RestRequestExtensions.AddParameter(val, "exchange_code", accountCredentials.Token, true);
		}
		else if (accountCredentials.Type == "refresh_token")
		{
			RestRequestExtensions.AddParameter(val, "grant_type", "refresh_token", true);
			RestRequestExtensions.AddParameter(val, "refresh_token", accountCredentials.Token, true);
		}
		else if (accountCredentials.Type == "authorization_code")
		{
			RestRequestExtensions.AddParameter(val, "grant_type", "authorization_code", true);
			RestRequestExtensions.AddParameter(val, "authorization_code", accountCredentials.Token, true);
		}
		Log.Information<string, string, string>("Sending Login request. url={url}, type={type}, id={id}", url, accountCredentials.Type, accountCredentials.Id);
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
			Log.Information<string>("Login request failed. {error}", text);
			throw new Exception(text);
		}
		OAuthTokenResponse oAuthTokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(((RestResponseBase)val2).Content);
		if (oAuthTokenResponse == null)
		{
			return null;
		}
		Log.Information<string, int>("Login request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		_onlineAccount = new FUserOnlineAccountMcp(oAuthTokenResponse);
		return _onlineAccount;
	}

	public async Task Logout()
	{
		if (_onlineAccount != null)
		{
			RestRequest val = new RestRequest($"/api/oauth/sessions/kill/{_onlineAccount.GetAccessToken()}", (Method)3);
			BuildUriExtensions.BuildUri((IRestClient)(object)_client, val).ToString();
			IOnlineSubsystem.Get();
			await _client.ExecuteAsync(val, default(CancellationToken));
			_onlineAccount = null;
		}
	}

	public async Task<ExchangeCode?> GenerateExchangeCode()
	{
		if (_onlineAccount == null)
		{
			Log.Information("GenerateExchangeCode request failed. No logged in user!");
			return null;
		}
		RestRequest val = new RestRequest("/api/oauth/exchange", (Method)0);
		string url = BuildUriExtensions.BuildUri((IRestClient)(object)_client, val).ToString();
		val.Authenticator = (IAuthenticator)(object)new HttpMcpAuthenticator(_onlineAccount);
		Log.Information<string>("Sending GenerateExchangeCode request. url={url}", url);
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
			Log.Information<string>("GenerateExchangeCode request failed. {error}", text);
			return null;
		}
		OAuthExchangeResponse oAuthExchangeResponse = JsonSerializer.Deserialize<OAuthExchangeResponse>(((RestResponseBase)val2).Content, _jsonSerializerOptions);
		if (oAuthExchangeResponse == null)
		{
			return null;
		}
		Log.Information<string, int>("GenerateExchangeCode request complete. url={url} code={code}", url, (int)((RestResponseBase)val2).StatusCode);
		return new ExchangeCode(oAuthExchangeResponse);
	}

	public FUserOnlineAccount? GetOnlineAccount()
	{
		return _onlineAccount;
	}
}
