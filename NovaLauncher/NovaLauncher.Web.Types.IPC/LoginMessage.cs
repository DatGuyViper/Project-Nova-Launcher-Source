using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types.IPC;

internal class LoginMessage : Message
{
	[JsonPropertyName("type")]
	public override string Type { get; set; } = "login-result";


	[JsonPropertyName("success")]
	public bool Success { get; set; }

	[JsonPropertyName("displayName")]
	public string DisplayName { get; set; }

	[JsonPropertyName("error")]
	public string Error { get; set; }

	public LoginMessage(bool success, string? displayName = null, string? error = null)
	{
		Success = success;
		DisplayName = displayName;
		Error = error;
		 
	}
}
