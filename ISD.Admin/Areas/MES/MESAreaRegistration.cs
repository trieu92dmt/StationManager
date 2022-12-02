using System.Web.Mvc;

namespace ISD.Admin.Areas.MES
{
    public class MESAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MES";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MES_default",
                "MES/{controller}/{action}/{id}",
                new { controller = "MES", action = "Index", id = UrlParameter.Optional },
                new string[] { "MES.Controllers" }
            );

            //context.MapRoute(
            //    "Permission_Account",
            //    "Permission/Account/{action}/{id}",
            //    new { controller = "Account", action = "Index", id = UrlParameter.Optional },
            //    new string[] { "Permission.Controllers" }
            //);
        }
    }
}