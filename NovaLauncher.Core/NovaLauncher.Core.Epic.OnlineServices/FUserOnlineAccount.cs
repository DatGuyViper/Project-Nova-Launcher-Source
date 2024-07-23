using System;

namespace NovaLauncher.Core.Epic.OnlineServices;

public interface FUserOnlineAccount : FOnlineUser
{
	string GetAccessToken();

	string GetRefreshToken();

	bool HasAccessTokenExpired(DateTime time);

	bool GetAuthAttribute(string attrName, out string outAttrValue);

	bool SetUserAttribute(string attrName, out string attrValue);
}
