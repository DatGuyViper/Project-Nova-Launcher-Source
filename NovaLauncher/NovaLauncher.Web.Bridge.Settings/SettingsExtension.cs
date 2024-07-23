using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Core.Nova;

namespace NovaLauncher.Web.Bridge.Settings;

[ComVisible(true)]
public class SettingsExtension : IExtension
{
	[CompilerGenerated]
	private NovaWebView _003CwebView_003EP;

	public SettingsExtension(NovaWebView webView)
	{
		_003CwebView_003EP = webView;
		 
	}

	public async void SetLocale(string locale)
	{
		SettingsManager settingsManager = SettingsManager.Get();
		settingsManager.GetSettings().Locale = locale;
		await settingsManager.Save();
	}

	public async void UpdateMod(string modName)
	{
		SettingsManager settingsManager = SettingsManager.Get();
		ModsStatus mods = settingsManager.GetSettings().Mods;
		switch (modName)
		{
		case "linear":
			mods.Linear = !mods.Linear;
			break;
		case "instant-reset":
			mods.InstantRelease = !mods.InstantRelease;
			break;
		case "edit-on-release":
			mods.EditOnRelease = !mods.EditOnRelease;
			break;
		case "sprint-by-default":
			mods.SprintByDefault = !mods.SprintByDefault;
			break;
		case "no-pre-edit":
			mods.DisablePreEdit = !mods.DisablePreEdit;
			break;
		case "potato":
			mods.Potato = !mods.Potato;
			break;
		}
		await settingsManager.Save();
	}

	public string GetMods()
	{
		return JsonSerializer.Serialize(SettingsManager.Get().GetSettings().Mods);
	}

	public string GetLocale()
	{
		return SettingsManager.Get().GetSettings().Locale;
	}

	public void Register()
	{
		((WebView2)_003CwebView_003EP).CoreWebView2.AddHostObjectToScript("settings", (object)this);
	}
}
