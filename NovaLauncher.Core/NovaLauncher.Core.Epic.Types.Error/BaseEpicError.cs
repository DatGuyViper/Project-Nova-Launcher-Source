using System;

namespace NovaLauncher.Core.Epic.Types.Error
{
    public class BaseEpicError : Exception
    {
        public EpicError Response { get; }

        public BaseEpicError(string errorCode, string errorMessage, string[]? messageVars, string? errorDescription, int numericErrorCode, string originatingService, string intent, int statusCode)
        {
            Response = new EpicError(errorCode, errorMessage, messageVars, errorDescription, numericErrorCode, originatingService, intent, statusCode);
        }
    }
}
