using ISD.EntityModels;
using System;
using System.Web;
using System.Web.Mvc;
using ISD.Core;

namespace ISD.Admin.Controllers
{
    public class ModuleController : BaseController
    {
        // GET: Module/ModuleId
        public ActionResult SelectedModule(Guid ModuleId)
        {
            ModuleModel module = _context.ModuleModel.Find(ModuleId);
            if (module == null)
            {
                return HttpNotFound();
            }
            //Save selected module to cookie
            HttpCookie selectedModule = new HttpCookie("selected_module");
            selectedModule["ModuleId"] = ModuleId.ToString();
            selectedModule["ModuleName"] = module.ModuleName;
            selectedModule.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(selectedModule);
            ViewBag.SelectedModuleId = module.ModuleId;
            ViewBag.SelectedModuleName = module.ModuleName;
            return View(module);
        }
    }
}