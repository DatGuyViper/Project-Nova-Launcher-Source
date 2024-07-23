namespace NovaLauncher.Core.Epic.Types.Account;

public class OAuthExchangeResponse
{
	public required int ExpiresInSeconds { get; set; }

	public required string Code { get; set; }

	public required string CreatingClientId { get; set; }
}
