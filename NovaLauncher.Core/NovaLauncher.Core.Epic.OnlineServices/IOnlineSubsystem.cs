using NovaLauncher.Core.Epic.OnlineServices.Mcp;

namespace NovaLauncher.Core.Epic.OnlineServices;

public interface IOnlineSubsystem
{
	static readonly IOnlineSubsystem OnlineSubsystem = new FOnlineSubsystemMcp();

	string GetClientSecret();

	string GetClientId();

	static IOnlineSubsystem Get()
	{
		return OnlineSubsystem;
	}
}
