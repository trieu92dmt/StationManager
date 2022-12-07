using ISD.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class ISDAuthorizationWebAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        // Khi request vừa chạm đén API chưa thực thi
        // Kiểm tra có đăng nhập chưa (CurrentUser đã gắn ở Jwt Middleware)
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (AppUserPrincipal)context.HttpContext.Items["CurrentUser"];
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { code = 401, message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
