using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types;

public class NewsEntry
{
	[JsonPropertyName("imgSrc")]
	public required string ImageSource { get; set; }

	[JsonPropertyName("description")]
	public required string Description { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }
}
