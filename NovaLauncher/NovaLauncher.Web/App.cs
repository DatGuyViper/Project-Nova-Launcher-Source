using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using NovaLauncher.Web.Bridge;
using NovaLauncher.Web.Bridge.Auth;
using NovaLauncher.Web.Bridge.Game;
using NovaLauncher.Web.Bridge.News;
using NovaLauncher.Web.Bridge.Settings;
using NovaLauncher.Web.Bridge.Utils;
using NovaLauncher.Web.Events;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.File;

namespace NovaLauncher.Web;

public class App : Application
{
	private readonly Window _root = new Window
	{
		Title = "Nova",
		Width = 1280.0,
		Height = 720.0,
		Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
		AllowsTransparency = true,
		WindowStyle = WindowStyle.None,
		ResizeMode = ResizeMode.NoResize
	};

	private readonly Grid _grid = new Grid();

	private readonly Fallback _fallback = new Fallback();

	private readonly NovaWebView _webView = new NovaWebView();

	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public App()
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NovaLauncher");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = Path.Combine(text, "Logs", "NovaLauncher-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".log");
		Log.Logger = (ILogger)(object)FileLoggerConfigurationExtensions.File(new LoggerConfiguration().MinimumLevel.Information().WriteTo, text2, (LogEventLevel)0, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", (IFormatProvider)null, (long?)1073741824L, (LoggingLevelSwitch)null, false, false, (TimeSpan?)null, (RollingInterval)3, false, (int?)31, (Encoding)null, (FileLifecycleHooks)null, (TimeSpan?)null).CreateLogger();
		if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
		{
			Log.Information("Instance already active, exiting.");
			Environment.Exit(0);
		}
		ServiceCollection services = new ServiceCollection();
		services.AddSingleton(_root);
		services.AddSingleton(_webView);
		services.AddSingleton<IExtension, NewsExtension>();
		services.AddSingleton<IExtension, GameExtension>();
		services.AddSingleton<IExtension, SettingsExtension>();
		services.AddSingleton<IExtension, UtilsExtension>();
		services.AddSingleton<IExtension, AuthExtension>();
		services.AddSingleton<IExtension, UpdateExtension>();
		ServiceProvider serviceProvider = services.BuildServiceProvider();
		((WebView2)_webView).WebMessageReceived += OnWebMessageReceived;
		_webView.InitializedView += delegate
		{
			foreach (IExtension service in serviceProvider.GetServices<IExtension>())
			{
				service.Register();
			}
		};
	}

	private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
	{
		BaseEvent baseEvent = JsonSerializer.Deserialize<BaseEvent>(args.TryGetWebMessageAsString(), JsonOptions);
		if (baseEvent != null)
		{
			switch (baseEvent.Name)
			{
			case "window-minimize":
				_root.WindowState = WindowState.Minimized;
				break;
			case "window-close":
				_root.Close();
				break;
			case "loaded":
				_grid.Children.Remove(_fallback);
				break;
			}
		}
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		_grid.Children.Add(_fallback);
		_grid.Children.Add((UIElement)(object)_webView);
		_root.Content = _grid;
		base.MainWindow = _root;
		_root.Show();
	}
}
