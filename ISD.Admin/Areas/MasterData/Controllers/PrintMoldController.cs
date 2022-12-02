using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
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
    public class PrintMoldController : BaseController
    {
        // GET: PrintMoldModel
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(string PrintMoldCode = "", string PrintMoldName = "", string PrintMoldType = "",string Serial = "",string Bin = "",string ProductId = "",string ProfileName = "")
        {
            return ExecuteSearch(() =>
            {
                var printMold = (from p in _context.PrintMoldModel
                                 //join cus in _context.ProfileModel on p.ProfileId equals cus.ProfileId
                                 join pro in _context.ProductModel on p.ProductId equals pro.ProductId
                                 where
                                 //search by Code
                                (PrintMoldCode == "" || p.PrintMoldCode == PrintMoldCode)
                                //Search by name
                                && (PrintMoldName == "" || p.PrintMoldName == (PrintMoldName))
                                //search by type
                                && (PrintMoldType == "" || p.PrintMoldType == PrintMoldType)
                                //search by serial
                                && (Serial == "" || p.PrintMoldType == Serial)
                                //search by bin
                                && (Bin == "" || p.PrintMoldType == Bin)
                                //search by PRoduct
                                && (ProductId == "" || p.ProductId.ToString() == ProductId)
                                 //search by profilename
                                 && (ProfileName == "" || p.ProfileName.Contains(ProfileName))
                                 select new PrintMoldViewModel
                                 {
                                     PrintMoldId = p.PrintMoldId,
                                     PrintMoldIntId = p.PrintMoldIntId,
                                     PrintMoldName = p.PrintMoldName,
                                     PrintMoldType = p.PrintMoldType,
                                     PrintMoldCode = p.PrintMoldCode,
                                     ProfileName = p.ProfileName,
                                     ProductName = pro.ProductName,
                                     Specifications_Length = p.Specifications_Length,
                                     Specifications_Width = p.Specifications_Width,
                                     Specifications_Height = p.Specifications_Height,
                                     Specifications_Overalls = p.Specifications_Overalls,
                                     Specifications_Side = p.Specifications_Side,
                                     ProductPerMold = p.ProductPerMold,
                                     LocationNote = p.LocationNote,
                                     PrintMoldFilm = p.PrintMoldFilm,
                                     PrintMoldDate = p.PrintMoldDate,
                                     LastMaintenanceDate = p.LastMaintenanceDate,
                                     MaintenanceAlert = p.MaintenanceAlert,
                                     StampQuantity = p.StampQuantity,
                                     CurrentStampeQuantity = p.CurrentStampeQuantity,
                                     Description = p.Description,
                                     Serial = p.Serial,
                                     Bin = p.Bin,
                                     Status = p.Status,
                                     StampQuantityAlert = p.StampQuantityAlert,
                                     ProductId = p.ProductId,
                                     ProfileId = p.ProfileId
                                 })
                               .ToList();
                return PartialView(printMold);
            });
        }
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(PrintMoldModel model, HttpPostedFileBase FileUpload)
        {
            return ExecuteContainer(() =>
            {
                model.PrintMoldId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_PrintMold.ToLower())
                });
            });
        }
        #endregion
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var PrintMold = _context.PrintMoldModel.FirstOrDefault(p => p.PrintMoldId == id);
            if (PrintMold == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Master_Department.ToLower()) });
            }
            CreateViewBag();
            return View(PrintMold);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(PrintMoldModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_PrintMold.ToLower())
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
                var PrintMold = _context.PrintMoldModel.FirstOrDefault(p => p.PrintMoldId == id);
                if (PrintMold != null)
                {
                    _context.Entry(PrintMold).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {

                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_PrintMold.ToLower())
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
        public void CreateViewBag()
        {

            //list khách hàng
            var lstProfile = _context.ProfileModel.Select(x => x).ToList();
            ViewBag.ProfileId = new SelectList(lstProfile, "ProfileId", "ProfileName");
            //list sản phẩm
            var lstProduct = _context.ProductModel.Select(x => x).ToList();
            ViewBag.ProductId = new SelectList(lstProduct, "ProductId", "ProductName");
        }
        #endregion

    }
}