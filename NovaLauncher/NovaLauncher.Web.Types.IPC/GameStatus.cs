using System.Text.Json.Serialization;

namespace NovaLauncher.Web.Types.IPC;

internal class GameStatus : Message
{
	[JsonPropertyName("type")]
	public override string Type { get; set; } = "game-status";


	[JsonPropertyName("running")]
	public bool Running { get; set; }

	[JsonPropertyName("error")]
	public string Error { get; set; }

	[JsonPropertyName("extra")]
	public string Extra { get; set; }

	public GameStatus(bool running, string? error = null, string? extra = null)
	{
		Running = running;
		Error = error;
		Extra = extra;
		 
	}
}
