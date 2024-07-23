using NovaLauncher.Core.Epic.OnlineServices.Mcp;

namespace NovaLauncher.Core.Epic.OnlineServices;

public interface IOnlineIdentity
{
	static readonly FOnlineIdentityMcp OnlineIdentity = new FOnlineIdentityMcp();

	FUserOnlineAccount? GetOnlineAccount();

	static FOnlineIdentityMcp Get()
	{
		return OnlineIdentity;
	}
}
