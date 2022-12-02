using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Utilities.Controllers
{
    public class ApplicationConfigController : BaseController
    {
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(string ConfigKey, string ConfigValue)
        {
            return ExecuteSearch(() =>
            {
                var ResourceKeyIsNullOrEmpty = string.IsNullOrEmpty(ConfigKey);
                var ResourceValueIsNullOrEmpty = string.IsNullOrEmpty(ConfigValue);
                var resourceList = _context.ApplicationConfig.Where(p => (ResourceKeyIsNullOrEmpty || p.ConfigKey.ToLower().Contains(ConfigKey.ToLower()))
                                                                     && (ResourceValueIsNullOrEmpty || p.ConfigValue.ToLower().Contains(ConfigValue.ToLower())))
                                                            .Select(p => new ApplicationConfigViewModel()
                                                            {
                                                                ConfigKey = p.ConfigKey,
                                                                ConfigValue = p.ConfigValue
                                                            })
                                                            .OrderBy(p => p.ConfigKey)
                                                            .ToList();
                return PartialView(resourceList);
            });
        }

        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Create(ApplicationConfig model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ApplicationConfig.ToLower())
                });
            });
        }

        [ISDAuthorizationAttribute]
        public ActionResult Edit(string id)
        {
            var resource = (from p in _context.ApplicationConfig
                            where p.ConfigKey == id
                            select new ApplicationConfigViewModel()
                            {
                                ConfigKey = p.ConfigKey,
                                ConfigValue = p.ConfigValue,
                            }).FirstOrDefault();
            if (resource == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ApplicationConfig.ToLower()) });
            }

            return View(resource);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(ApplicationConfig model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.ApplicationConfig.ToLower())
                });
            });
        }

    }
}