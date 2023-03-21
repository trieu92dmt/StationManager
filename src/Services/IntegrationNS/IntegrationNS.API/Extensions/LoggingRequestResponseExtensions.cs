namespace IntegrationNS.API.Extensions
{
    public static class LoggingRequestResponseExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingRequestResponseMiddleware>();
        }
    }
}
