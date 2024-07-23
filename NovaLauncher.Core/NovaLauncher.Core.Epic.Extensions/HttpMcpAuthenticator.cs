using System.Threading.Tasks;
using NovaLauncher.Core.Epic.OnlineServices.Mcp;
using RestSharp;
using RestSharp.Authenticators;

namespace NovaLauncher.Core.Epic.Extensions;

public class HttpMcpAuthenticator : AuthenticatorBase
{
	public HttpMcpAuthenticator(FUserOnlineAccountMcp onlineAccount)
		: base(onlineAccount.GetAccessToken())
	{
	}

	protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		return new ValueTask<Parameter>((Parameter)new HeaderParameter("Authorization", "Bearer " + accessToken));
	}
}
