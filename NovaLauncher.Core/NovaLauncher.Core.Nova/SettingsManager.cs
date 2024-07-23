using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace NovaLauncher.Core.Nova;

public class SettingsManager
{
	private static SettingsManager _settingsManager = new SettingsManager();

	private Settings _settings;

	public SettingsManager()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
		string path = Path.Combine(text, "settings.json");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (File.Exists(path))
		{
			Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(path));
			_settings = ((settings != null) ? settings : new Settings());
		}
		else
		{
			_settings = new Settings();
		}
	}

	public Settings GetSettings()
	{
		return _settings;
	}

	public async Task Save()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
		string path = Path.Combine(text, "settings.json");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		await File.WriteAllTextAsync(path, JsonSerializer.Serialize(_settings));
	}

	public static SettingsManager Get()
	{
		return _settingsManager;
	}
}
