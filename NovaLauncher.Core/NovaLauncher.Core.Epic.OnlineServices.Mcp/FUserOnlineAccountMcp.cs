using System;
using NovaLauncher.Core.Epic.Types.Account;

namespace NovaLauncher.Core.Epic.OnlineServices.Mcp;

public class FUserOnlineAccountMcp : FUserOnlineAccount, FOnlineUser
{
	private FAuthTokenMcp _authToken;

	private Guid _userId;

	private string _displayName;

	public FUserOnlineAccountMcp(OAuthTokenResponse oauthResponse)
	{
		_authToken = new FAuthTokenMcp(oauthResponse);
		_userId = oauthResponse.AccountId;
		_displayName = oauthResponse.DisplayName;
	}

	public string GetAccessToken()
	{
		return _authToken.Token;
	}

	public string GetRefreshToken()
	{
		return _authToken.RefreshToken;
	}

	public bool GetAuthAttribute(string attrName, out string outAttrValue)
	{
		throw new NotImplementedException();
	}

	public bool GetUserAttribute(string attrName, out string outAttrValue)
	{
		throw new NotImplementedException();
	}

	public string GetDisplayName(string platform = "")
	{
		return _displayName;
	}

	public string GetRealName()
	{
		return _displayName;
	}

	public Guid GetUserId()
	{
		return _userId;
	}

	public bool HasAccessTokenExpired(DateTime time)
	{
		return _authToken.ExpiresAt < time;
	}

	public bool HasRefreshTokenExpired(DateTime time)
	{
		return _authToken.RefreshExpiresAt < time;
	}

	public bool SetUserAttribute(string attrName, out string attrValue)
	{
		throw new NotImplementedException();
	}

	public bool SetUserLocalAttribute(string attrName, string inAttrValue)
	{
		throw new NotImplementedException();
	}
}
