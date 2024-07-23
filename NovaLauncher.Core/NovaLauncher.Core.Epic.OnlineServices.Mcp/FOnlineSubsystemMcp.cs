using System;
using System.Text;

namespace NovaLauncher.Core.Epic.OnlineServices.Mcp;

public class FOnlineSubsystemMcp : IOnlineSubsystem
{
	public string GetBasicAuth()
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(GetClientId() + ":" + GetClientSecret()));
	}

	public string GetClientId()
	{
		return "32e1e499d4ba4c49a2002421d6f4a448";
	}

	public string GetClientSecret()
	{
		return "f3620873c4464753b2938363cf3086a4";
	}
}
