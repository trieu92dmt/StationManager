using ISD.API.Extensions.Jwt;
using System;

namespace ISD.API.Core.Infrastructures
{
    public abstract class BaseHandler
    {
        protected readonly Guid? UserId;
        protected readonly bool IsDevEnv;

        protected BaseHandler()
        {
            UserId = string.IsNullOrEmpty(TokenExtensions.GetUserId()) ? null : Guid.Parse(TokenExtensions.GetUserId());

            IsDevEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "develop";
        }
    }
}
