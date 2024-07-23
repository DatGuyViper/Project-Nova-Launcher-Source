using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace NovaLauncher.Web;

public class NovaWebView : WebView2
{
	private static readonly CoreWebView2EnvironmentOptions Options = new CoreWebView2EnvironmentOptions((string)null, (string)null, (string)null, false, (List<CoreWebView2CustomSchemeRegistration>)null)
	{
		AdditionalBrowserArguments = "--enable-features=\"msWebView2EnableDraggableRegions\""
	};

	public event EventHandler? InitializedView;

	public NovaWebView()
	{
		((WebView2)this).DefaultBackgroundColor = Color.Transparent;
	}

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e); // Call the base class method using 'base'
        InitializeAsync(); // Call your custom async initialization method
    }


    private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
	{
		new Uri(args.Request.Uri);
	}

	private async Task InitializeAsync()
	{
		try
		{
			CoreWebView2Environment.GetAvailableBrowserVersionString((string)null, (CoreWebView2EnvironmentOptions)null);
		}
		catch
		{
			MessageBox.Show("In order to use Nova Launcher you must install Edge WebView2. You can find the download on our Discord server or on Google.");
			Environment.Exit(0);
			return;
		}
		await ((WebView2)this).EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync((string)null, Directory.CreateTempSubdirectory().FullName, Options));
		((WebView2)this).CoreWebView2.Settings.IsStatusBarEnabled = false;
		((WebView2)this).CoreWebView2.Settings.IsZoomControlEnabled = false;
		((WebView2)this).CoreWebView2.Settings.IsPinchZoomEnabled = false;
		((WebView2)this).CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
		((WebView2)this).CoreWebView2.Settings.AreDevToolsEnabled = false;
		OnInitializedView();
		((WebView2)this).CoreWebView2.Navigate("https://launcher.novafn.dev/");
		if (Environment.GetCommandLineArgs().Any((string x) => x.ToLower() == "-devtools"))
		{
			((WebView2)this).CoreWebView2.OpenDevToolsWindow();
		}
	}

	protected virtual void OnInitializedView()
	{
		this.InitializedView?.Invoke(this, EventArgs.Empty);
	}
}
