using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types.Responses;

public class UpdateResponse
{
	[JsonPropertyName("version")]
	public required string Version { get; set; }

	[JsonPropertyName("url")]
	public required string Url { get; set; }
}
