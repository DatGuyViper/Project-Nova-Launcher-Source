using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types.IPC;

internal class UpdateStatus : Message
{
	[JsonPropertyName("type")]
	public override string Type { get; set; } = "update-status";


	[JsonPropertyName("progress")]
	public int Progress { get; set; }

	[JsonPropertyName("error")]
	public string Error { get; set; }

	public UpdateStatus(int percentile, string? error = null)
	{
		Progress = percentile;
		Error = error;
		 
	}
}
