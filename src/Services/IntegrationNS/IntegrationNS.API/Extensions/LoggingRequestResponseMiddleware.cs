using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IO;

namespace IntegrationNS.API.Extensions
{
    public class LoggingRequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public LoggingRequestResponseMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequestResponse(context);
        }

        private async Task LogRequestResponse(HttpContext context)
        {
            try
            {
                #region Request handling to HttpContext

                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);
                context.Request.Body.Position = 0;
                var originalBodyStream = context.Response.Body;
                await using var responseBody = _recyclableMemoryStreamManager.GetStream();
                context.Response.Body = responseBody;

                #endregion

                #region Request and response handling to HttpContext

                await _next(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                //Get response BODY (Json)
                var response = await new StreamReader(context.Response.Body).ReadToEndAsync();

                int? statusCode = context.Response.StatusCode;

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                //Get request BODY (Json)
                using var reader = new StreamReader(context.Request.Body);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                var request = await reader.ReadToEndAsync();

                #endregion

                #region Save log to database

                //Get user id
                var userIdentity = context.User?.Identity;
                var userName = userIdentity?.Name;
                Guid? userId = new();
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EntityDataContext>();
                    var user = await dbContext.AccountModel.FirstOrDefaultAsync(x => x.UserName == userName);
                    userId = user?.AccountId;

                    dbContext.LogApiModel.Add(new Infrastructure.Models.LogApiModel
                    {
                        Url = $"{context.Request.Path}{context.Request.QueryString}",
                        Module = "IntegrationNS.API",
                        StatusCode = statusCode,
                        Method = context.Request.Method,
                        Request = request,
                        Response = response,
                        CreateTime = DateTime.UtcNow.AddHours(7),
                        CreateBy = string.IsNullOrEmpty(userId.ToString()) ? "CITEK" : userId.ToString()   
                    });

                    await dbContext.SaveChangesAsync();
                }
                #endregion              
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}
