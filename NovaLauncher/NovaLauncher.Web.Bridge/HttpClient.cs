using System;
using System.Reflection;
using RestSharp;

namespace NovaLauncher.Web.Bridge;

internal class HttpClient
{
	private const string Protocol = "https";

	private const string Domain = "api.novafn.dev";

	private const string BasePath = "nova";

	public static RestClient Client = new RestClient(new RestClientOptions
	{
		BaseUrl = new Uri("https://api.novafn.dev/nova"),
		UserAgent = "NovaLauncher/" + Assembly.GetExecutingAssembly().GetName().Version.ToString()
	}, (ConfigureHeaders)null, (ConfigureSerialization)null, false);
}
