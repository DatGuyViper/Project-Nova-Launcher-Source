using System.Text.Json;
using Serilog;

namespace NovaLauncher.Web.Utils;

internal class JsonExtension
{
	private static T? TryDeserialize<T>(string json) where T : class
	{
		try
		{
			return JsonSerializer.Deserialize<T>(json);
		}
		catch (JsonException ex)
		{
			Log.Information<string>("Failed to deserialize: {0}", ex.Message);
		}
		return null;
	}
}
