using System;

namespace NovaLauncher.Core.Epic.OnlineServices;

public interface FOnlineUser
{
	Guid GetUserId();

	string GetRealName();

	string GetDisplayName(string platform = "");

	bool GetUserAttribute(string attrName, out string outAttrValue);

	bool SetUserLocalAttribute(string attrName, string inAttrValue);
}
