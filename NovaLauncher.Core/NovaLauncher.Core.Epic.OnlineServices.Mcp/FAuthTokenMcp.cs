using System;
using NovaLauncher.Core.Epic.Types.Account;

namespace NovaLauncher.Core.Epic.OnlineServices.Mcp;

public class FAuthTokenMcp
{
	private string _token;

	private string _refreshToken;

	private DateTime _expiresAt;

	private DateTime _refreshExpiresAt;

	private Guid _userId;

	public string RefreshToken => _refreshToken;

	public string Token => _token;

	public Guid UserId => _userId;

	public DateTime ExpiresAt => _expiresAt;

	public DateTime RefreshExpiresAt => _refreshExpiresAt;

	public int ExpiresIn => (int)ExpiresAt.Subtract(DateTime.Now).TotalSeconds;

	public int RefreshExpiresIn => (int)RefreshExpiresAt.Subtract(DateTime.Now).TotalSeconds;

	public FAuthTokenMcp(OAuthTokenResponse oAuthResponse)
	{
		_token = oAuthResponse.AccessToken;
		_userId = oAuthResponse.AccountId;
		_expiresAt = oAuthResponse.ExpiresAt;
		_refreshToken = oAuthResponse.RefreshToken;
	}
}
