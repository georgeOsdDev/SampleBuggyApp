using Microsoft.AspNetCore.Diagnostics;

namespace ErrorHandling
{

    public class SpecialUnhandleException : Exception
    {
        public SpecialUnhandleException(string message) : base(message)
        {
        }
    }

    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> logger;
        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            this.logger = logger;
        }
        public ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionMessage = exception.Message;
            logger.LogError(
                "Error Message: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);
            // Return false to continue with the default behavior
            // - or - return true to signal that this exception is handled

            // return true if exception is SpecialUnhandleException
            if (exception is SpecialUnhandleException) {
              return ValueTask.FromResult(true);
            }
            else
            {
              return ValueTask.FromResult(false);
            }
        }
    }
}
