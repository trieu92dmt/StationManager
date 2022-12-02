using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace MasterData.Controllers
{
    public class WorkShop2Controller : BaseController
    {
        // GET: WorkShop
        #region Index
        //[ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        //Tìm kiếm phân trang
        public ActionResult _Search(WorkShopSearchViewModel searchViewModel)
        {
            Session["frmSearchDistrict"] = searchViewModel;

            return ExecuteSearch(() =>
            {
                var workShops = (from p in _context.WorkShopModel
                                 join pr in _context.CompanyModel on p.CompanyId equals pr.CompanyId
                                 where
                                 //search by WorkShopName
                                 (searchViewModel.WorkShopName == null || p.WorkShopName == searchViewModel.WorkShopName)
                                 //search by Actived
                                 && (searchViewModel.Actived == null || p.Actived == searchViewModel.Actived)
                                 select new WorkShopViewModel()
                                 {
                                     WorkShopId = p.WorkShopId,
                                     CompanyName = pr.CompanyName,
                                     WorkShopCode = p.WorkShopCode,
                                     WorkShopName = p.WorkShopName,
                                     OrderIndex = p.OrderIndex,
                                     Actived = p.Actived
                                 })
                                    .Take(200)
                                    .ToList();

                return PartialView(workShops);
            });
        }
        #endregion END Index

        //GET: /WorkShop/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {

            var listCompany = (from p in _context.CompanyModel
                               where p.Actived == true
                               select new { 
                               p.CompanyId,
                               p.CompanyName
                               }).ToList();
            ViewBag.CompanyId = new SelectList(listCompany, "CompanyId", "CompanyName");
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(WorkShopModel model)
        {
            return ExecuteContainer(() =>
            {
                model.WorkShopId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_WorkShop.ToLower())
                });
            });
        }
        #endregion

        //GET: /District/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var listCompany = (from p in _context.CompanyModel
                               where p.Actived == true
                               select new
                               {
                                   p.CompanyId,
                                   p.CompanyName
                               }).ToList();
            
            var workShop = (from p in _context.WorkShopModel
                            join pr in _context.CompanyModel on p.CompanyId equals pr.CompanyId
                            where p.WorkShopId == id
                            select new WorkShopViewModel()
                            {
                                WorkShopId = p.WorkShopId,
                                CompanyId = pr.CompanyId,
                                WorkShopCode = p.WorkShopCode,
                                WorkShopName = p.WorkShopName,
                                OrderIndex = p.OrderIndex,
                                CompanyName = pr.CompanyName,
                                Actived = p.Actived
                            })
                         .FirstOrDefault();
            if (workShop == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_District.ToLower()) });
            }
            ViewBag.CompanyId = new SelectList(listCompany, "CompanyId", "CompanyName", workShop.CompanyId);
            return View(workShop);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(WorkShopModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_WorkShop.ToLower())
                });
            });
        }
        #endregion

    }
}