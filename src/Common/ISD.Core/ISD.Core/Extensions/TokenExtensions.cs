using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ISD.Core.Extensions
{
    public static class TokenExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetUserId()
        {
            if (_httpContextAccessor == null) return Guid.Empty.ToString();
            var claims = _httpContextAccessor.HttpContext.GetClaims();
            var userid = claims?.FirstOrDefault(c => c.Type.Contains("Id"))?.Value;
            return userid;
        }

        public static Guid GetAccountId()
        {
            if (_httpContextAccessor == null) return Guid.Empty;
            var claims = _httpContextAccessor.HttpContext.GetClaims();
            var userid = claims?.FirstOrDefault(c => c.Type.Contains("Id"))?.Value;
            return string.IsNullOrEmpty(userid) ? Guid.Empty : Guid.Parse(userid);
        }

        private static string GetToken(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return string.Empty;
            }

            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader == null) return null;
            var bearerToken = authHeader.Substring(7)
                .Trim();
            return bearerToken;
        }

        private static List<Claim> GetClaims(this HttpContext httpContext)
        {
            var token = httpContext.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return new List<Claim>();
            }
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                return jwtSecurityTokenHandler.ReadJwtToken(token).Claims.ToList();
            }
            catch
            {
                return new List<Claim>();
            }
        }
    }
}
