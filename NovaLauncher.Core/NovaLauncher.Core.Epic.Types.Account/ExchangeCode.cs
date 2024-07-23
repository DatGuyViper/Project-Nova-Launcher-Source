using System.ComponentModel.DataAnnotations;

namespace NovaLauncher.Core.Epic.Types.Account;

public class ExchangeCode
{
	[MaxLength(32)]
	public string Code { get; set; }

	[MaxLength(32)]
	public string ClientId { get; set; }

	public ExchangeCode(OAuthExchangeResponse response)
	{
		Code = response.Code;
		ClientId = response.CreatingClientId;
	}
}
