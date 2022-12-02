using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class ContConfigController : BaseController
    {
        // GET: ContConfig
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(ContConfigViewModel viewModel)
        {
            return ExecuteSearch(() =>
            {
                var dataList = _unitOfWork.ContConfigRepository.ListAll(viewModel);
                return PartialView(dataList);
            });
        }
        #endregion

        //GET: /Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(ContConfigModel model)
        {
            return ExecuteContainer(() =>
            {
                model.ContConfigId = Guid.NewGuid();
                model.CreateTime = DateTime.Now;
                model.CreateBy = CurrentUser.AccountId;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ContConfig.ToLower())
                });
            });
        }
        #endregion

        //GET: /Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var model = _unitOfWork.ContConfigRepository.GetBy(id);
            if (model == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.ContConfig.ToLower()) });
            }
            return View(model);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(ContConfigModel model)
        {
            return ExecuteContainer(() =>
            {
                model.LastEditTime = DateTime.Now;
                model.LastEditBy = CurrentUser.AccountId;
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.ContConfig.ToLower())
                });
            });
        }
        #endregion

        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var model = _context.ContConfigModel.FirstOrDefault(p => p.ContConfigId == id);
                if (model != null)
                {
                    _context.Entry(model).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.ContConfig.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion
    }
}