using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Nova;

public class Installation
{
	[JsonPropertyName("version")]
	public required string Version { get; set; }

	[JsonPropertyName("path")]
	public required string Path { get; set; }

	[JsonPropertyName("selected")]
	public bool Selected { get; set; }
}
