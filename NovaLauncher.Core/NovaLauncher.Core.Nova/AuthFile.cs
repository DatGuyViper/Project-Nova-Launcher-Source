using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Nova;

public class AuthFile
{
	[JsonPropertyName("at")]
	public string? AccessToken { get; set; }

	[JsonPropertyName("rt")]
	public string? RefreshToken { get; set; }
}
