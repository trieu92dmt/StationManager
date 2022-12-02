using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class WorkShopController : BaseController
    {
        // GET: WorkShop
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(WorkShopSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                //Gọi function search từ WorkShopRepository
                var workShopList = _unitOfWork.WorkShopRepository.Search(searchViewModel);
                return PartialView(workShopList);
            });
        }
        #endregion
        //GET: /WorkShop/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
       

        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(WorkShopViewModel workShopViewModel)
        {
            return ExecuteContainer(() =>
            {
                //Create WorkShop
                _unitOfWork.WorkShopRepository.Create(workShopViewModel);

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_WorkShop.ToLower())
                });
            });
        }
        #endregion

        //GET: /WorkShop/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var dataInDb = _unitOfWork.WorkShopRepository.GetWorkShop(id);
            if (dataInDb == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_WorkShop.ToLower()) });
            }
            CreateViewBag(dataInDb.CompanyId, dataInDb.StoreId);
            return View(dataInDb);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(WorkShopViewModel viewModel)
        {
            return ExecuteContainer(() =>
            {
                //Edit WorkShop
                _unitOfWork.WorkShopRepository.Update(viewModel);

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_WorkShop.ToLower())
                });
            });
        }
        #endregion

        private void CreateViewBag(Guid? CompanyId = null, Guid? StoreId = null)
        { 
            //Lấy danh sách company (CompanyId, CompanyName)
            var lstCompany = _unitOfWork.CompanyRepository.GetAll().Select(x => new { x.CompanyId, x.CompanyName });
            ViewBag.CompanyId = new SelectList(lstCompany, "CompanyId", "CompanyName", CompanyId);
            //Lấy danh sách Store (CompanyId, CompanyName)
            var lstStore = _unitOfWork.StoreRepository.GetAllStore().Select(x => new { x.StoreId, x.StoreName });
            ViewBag.StoreId = new SelectList(lstStore, "StoreId", "StoreName", StoreId);
        }
    }
}