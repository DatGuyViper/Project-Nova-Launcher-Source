using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Nova;

public class ModsStatus
{
	[JsonPropertyName("eor")]
	public bool EditOnRelease { get; set; }

	[JsonPropertyName("sbd")]
	public bool SprintByDefault { get; set; }

	[JsonPropertyName("irs")]
	public bool InstantRelease { get; set; }

	[JsonPropertyName("lnr")]
	public bool Linear { get; set; }

	[JsonPropertyName("pto")]
	public bool Potato { get; set; }

	[JsonPropertyName("npr")]
	public bool DisablePreEdit { get; set; }
}
