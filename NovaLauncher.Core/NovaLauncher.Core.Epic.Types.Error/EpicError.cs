using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Epic.Types.Error;

public class EpicError
{
	public string ErrorCode { get; set; }

	public string ErrorMessage { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string[]? MessageVars { get; set; }

	public string? ErrorDescription { get; set; }

	public int NumericErrorCode { get; set; }

	public string OriginatingService { get; set; }

	public string Intent { get; set; }

	public int StatusCode { get; set; }

	public EpicError(string errorCode, string errorMessage, string[]? messageVars, string? errorDescription, int numericErrorCode, string originatingService, string intent, int statusCode)
	{
		ErrorCode = errorCode;
		ErrorMessage = errorMessage;
		MessageVars = messageVars;
		ErrorDescription = errorDescription;
		NumericErrorCode = numericErrorCode;
		OriginatingService = originatingService;
		Intent = intent;
		StatusCode = statusCode;
	}
}
