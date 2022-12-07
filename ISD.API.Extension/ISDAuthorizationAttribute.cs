
using ISD.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace ISD.API.Extensions
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ISDAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        // Khi request vừa chạm đén API chưa thực thi
        // Kiểm tra có đăng nhập chưa (CurrentUser đã gắn ở Jwt Middleware)
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (AppUserPrincipal)context.HttpContext.Items["CurrentUser"];
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { code = 401 ,message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        // Khi request đến API, kiểm tra quyền 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.RouteValues["action"];
            string controllerName = filterContext.ActionDescriptor.RouteValues["controller"];
            string areaName = "";

            if (filterContext.RouteData.DataTokens["area"] != null)
            {
                areaName = filterContext.RouteData.DataTokens["area"].ToString();
            }
            if (!CheckAccessRight(areaName, actionName, controllerName, filterContext.HttpContext))
            {
                filterContext.Result = new StatusCodeResult(404);
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }

        public static bool CheckAccessRight(string Area, string Action, string Controller, HttpContext context)
        {
            if (context.Items["Permission"] != null)
            {
                string ScreenCode = "";
                //Get PageUrl from user input
                if (!string.IsNullOrEmpty(Area))
                {
                    ScreenCode = string.Format("{0}/{1}", Area, Controller);
                }
                else
                {
                    ScreenCode = string.Format("{0}", Controller);
                }
                //Get PageUrl from Session["Menu"]
                PermissionMobileViewModel permission = (PermissionMobileViewModel)context.Items["Permission"];
                var pageId = permission.MobileScreenModel.Where(p => p.ScreenCode == ScreenCode)
                                                .Select(p => p.MobileScreenId)
                                                .FirstOrDefault();
                //Compare with PageId in PagePermission
                var pagePermission = permission.MobileScreenPermissionModel.Where(p => p.MobileScreenId == pageId && p.FunctionId == "M_"+Action.ToUpper()).FirstOrDefault();
                if (pagePermission != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
