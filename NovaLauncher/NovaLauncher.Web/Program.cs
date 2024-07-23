using System;
using Sentry;

namespace NovaLauncher.Web;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			Exception ex = e.ExceptionObject as Exception;
			if (ex.StackTrace == null || (!ex.StackTrace.Contains("Microsoft.Toolkit.Uwp") && !ex.StackTrace.Contains("AwaitableSocket") && !ex.StackTrace.Contains("Nova")))
			{
				SentrySdk.CaptureException(ex, (Action<Scope>)delegate(Scope scope)
				{
					scope.SetTag("tracking-id", Guid.NewGuid().ToString());
				});
			}
		};
		SentrySdk.Init((Action<SentryOptions>)delegate(SentryOptions options)
		{
			string environment = "prod";
			options.Dsn = "https://3813e81a803d1a9ebe7c479fddcdf7e3@sentry.novafn.dev/4";
			options.AutoSessionTracking = true;
			options.IsGlobalModeEnabled = true;
			options.Environment = environment;
		});
		new App().Run();
	}
}
