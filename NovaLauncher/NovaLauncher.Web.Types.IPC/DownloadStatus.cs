using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types.IPC;

internal class DownloadStatus : Message
{
	[JsonPropertyName("type")]
	public override string Type { get; set; } = "download-status";


	[JsonPropertyName("progress")]
	public bool Progress { get; set; }

	[JsonPropertyName("completed")]
	public bool Completed { get; set; }

	[JsonPropertyName("percentile")]
	public double Percentile { get; set; }

	[JsonPropertyName("speed")]
	public double Mbps { get; set; }

	[JsonPropertyName("version")]
	public string Version { get; set; }

	[JsonPropertyName("error")]
	public string Error { get; set; }

	public DownloadStatus(bool progress, bool completed, double percentile, double mbps, string version, string? error = null)
	{
		Progress = progress;
		Completed = completed;
		Percentile = percentile;
		Mbps = mbps;
		Version = version;
		Error = error;
		 
	}
}
