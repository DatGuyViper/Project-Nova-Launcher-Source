using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Nova;

public class Settings
{
	[JsonPropertyName("locale")]
	public string Locale { get; set; } = "en";


	[JsonPropertyName("mods")]
	public ModsStatus Mods { get; set; } = new ModsStatus();

}
