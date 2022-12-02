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
    public class PlantRoutingController : BaseController
    {
        
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _Search(PlantRoutingConfigViewModel plantRoutingConfigViewModel)
        {
            return ExecuteSearch(() =>
            {
                var dataList = _unitOfWork.PlantRoutingConfigRepository.ListAll(plantRoutingConfigViewModel);
                CreateViewBag(plantRoutingConfigViewModel.PlantRoutingGroup);
                return PartialView(dataList);
            });
        }
        #endregion

        //GET: /Catalog/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            PlantRoutingConfigViewModel model = new PlantRoutingConfigViewModel();
            model.IsPrimaryStep = false;
            model.Actived = true;
            return View(model);
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(PlantRoutingConfigModel model)
        {
            return ExecuteContainer(() =>
            {
                model.CreatedTime = DateTime.Now;
                model.CreatedUser = CurrentUser.UserName;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.PlantRouting.ToLower())
                });
            });
        }
        #endregion

        //GET: /Catalog/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(int id)
        {
            var PlantRouting = _unitOfWork.PlantRoutingConfigRepository.GetBy(id);
            if (PlantRouting == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.PlantRouting.ToLower()) });
            }
            CreateViewBag(PlantRouting.PlantRoutingGroup, PlantRouting.FromData, PlantRouting.Attribute1);
            return View(PlantRouting);
        }
        //POST: Edit
        [HttpPost]
        [ValidateInput(false)]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(PlantRoutingConfigModel model)
        {
            return ExecuteContainer(() =>
            {
                model.LastModifiedTime = DateTime.Now;
                model.LastModifiedUser = CurrentUser.UserName;
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.PlantRouting.ToLower())
                });
            });
        }
        #endregion

        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(int id)
        {
            return ExecuteDelete(() =>
            {
                var model = _context.PlantRoutingConfigModel.FirstOrDefault(p => p.PlantRoutingCode == id);
                if (model != null)
                {
                    _context.Entry(model).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.PlantRouting.ToLower())
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

        #region CreateViewBag, Helper
        public void CreateViewBag(string PlantRoutingGroup = null, string FromData = null, string DepartmentCode = null)
        {
            //Get list PlantRoutingGroup
            var PlantRoutingGroupList = _context.CatalogModel.Where(p => p.Actived == true && p.CatalogTypeCode == "MES_REPORT_CONFIG")
                                                           .OrderBy(p => p.OrderIndex).ToList();

            ViewBag.PlantRoutingGroup = new SelectList(PlantRoutingGroupList, "CatalogCode", "CatalogText_vi", PlantRoutingGroup);

            //Get list FromData
            var FromDataList = _context.CatalogModel.Where(p => p.Actived == true && p.CatalogTypeCode == "MES_FROM_DATA")
                                                           .OrderBy(p => p.OrderIndex).ToList();

            ViewBag.FromData = new SelectList(FromDataList, "CatalogCode", "CatalogText_vi", FromData);

            //Phân xưởng/Phòng ban
            //var departmentList = _context.AllDepartmentModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).Select(p => new ISDSelectStringItem()
            //{
            //    id = p.DepartmentCode,
            //    name = p.DepartmentName,
            //    additional = "Phòng ban"
            //}).ToList();
            var physicsWorkshopList = _context.PhysicsWorkShopModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).Select(p => new ISDSelectStringItem()
            {
                id = p.PhysicsWorkShopCode,
                name = p.PhysicsWorkShopName,
                additional = "Phân xưởng vật lý"
            }).ToList();
            var allDepartmentList = new List<ISDSelectStringItem>();
            //allDepartmentList.AddRange(departmentList);
            allDepartmentList.AddRange(physicsWorkshopList);
            //ViewBag.Attribute1 = new SelectList(allDepartmentList, dataValueField: "id", dataTextField: "name", dataGroupField: "additional", selectedValue: DepartmentCode);
            ViewBag.Attribute1 = new SelectList(allDepartmentList, dataValueField: "id", dataTextField: "name", selectedValue: DepartmentCode);
        }
        #endregion
    }
}