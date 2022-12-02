using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.Core;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ISD.ViewModels;
using System.Collections.Generic;
using System.Dynamic;
using OfficeOpenXml.Packaging;

namespace MasterData.Controllers
{
    public class ErrorListController : BaseController
    {
        // GET: ErrorList
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(ErrorListSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (  
                                //Danh sách lỗi
                                from e in _context.ErrorListModel
                                //Công đoạn
                                join rm in _context.RoutingModel on e.StepCode equals rm.StepCode into r_jointable
                                from r in r_jointable.DefaultIfEmpty()
                                //Chỉ tiêu kiểm tra
                                join tm in _context.TestTargetModel on e.TargetCode equals tm.TargetCode into t_jointable
                                from t in t_jointable.DefaultIfEmpty()

                                orderby e.OrderIndex

                                where
                                //search by ErrorListName
                                (searchViewModel.ErrorListName == null || e.ErrorListName.Contains(searchViewModel.ErrorListName)
                                //search by Actived
                                && (searchViewModel.Actived == null || e.Actived == searchViewModel.Actived)
                                //search by StepCode
                                  && (searchViewModel.StepCode == null || (r != null && e.StepCode == searchViewModel.StepCode))
                                //search by TargetCode
                                 && (searchViewModel.TargetCode == null || (t != null && e.TargetCode == searchViewModel.TargetCode)))
                                select new ErrorListViewModel()
                                {
                                    //1. ID
                                    ErrorListId = e.ErrorListId,
                                    //2. Mã lỗi
                                    ErrorListCode = e.ErrorListCode,
                                    //3. Tên lỗi
                                    ErrorListName = e.ErrorListName,
                                    //4. Công đoạn
                                    StepCode = e.StepCode,
                                    StepName = r.StepName,
                                    //5. Chỉ tiêu kiểm tra
                                    TargetCode = e.TargetCode,
                                    TestTargetName = t.TargetName,
                                    //6. Thứ tự
                                    OrderIndex = e.OrderIndex,
                                    //7. Trạng thái
                                    Actived = e.Actived
                                }).ToList();
             
                return PartialView(result);
            });
        }
        #endregion

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
        public JsonResult Create(ErrorListModel model)
        {
            return ExecuteContainer(() =>
            {
                model.ErrorListId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_ErrorList.ToLower())
                });
            });
        }

        #endregion

        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var result = (from e in _context.ErrorListModel
                          where e.ErrorListId == id
                          select new ErrorListViewModel()
                          {
                              //1. ID
                              ErrorListId = e.ErrorListId,
                              //2. Mã lỗi
                              ErrorListCode = e.ErrorListCode,
                              //3. Tên lỗi
                              ErrorListName = e.ErrorListName,
                              //4. Công đoạn
                              StepCode = e.StepCode,
                              //5. Chỉ tiêu kiểm tra
                              TargetCode = e.TargetCode,
                              //6. Thứ tự
                              OrderIndex = e.OrderIndex,
                              //7. Trạng thái
                              Actived = e.Actived,
  
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_ErrorList.ToLower()) });
            }
            CreateViewBag(result.StepCode, result.TargetCode);
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(ErrorListModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_ErrorList.ToLower())
                });
            });
        }

        #endregion
        //Check trùng
        #region Remote Validation
        private bool IsExists(string ErrorListCode)
        {
            return (_context.ErrorListModel.FirstOrDefault(p => p.ErrorListCode == ErrorListCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingErrorListCode(string ErrorListCode, string ErrorListCodeValid)
        {
            try
            {
                if (ErrorListCodeValid != ErrorListCode)
                {
                    return Json(!IsExists(ErrorListCode));
                }
                else
                {
                    return Json(true);
                }
            }
            catch //(Exception ex)
            {
                return Json(false);
            }
        }
        #endregion

        #region Helper
        public void CreateViewBag(string StepCode = null, int? TargetCode = null)
        {
            //Danh sách các công đoạn
            var stepList = _context.RoutingModel.OrderBy(p => p.OrderIndex).Select(p => new ISDSelectStringItem()
            {
                id = p.StepCode,
                name = p.StepName
            }).ToList();
            ViewBag.StepCode = new SelectList(stepList, "id", "name", StepCode);
            //Danh sách các chỉ tiêu kiểm tra
            var testTargetList = _context.TestTargetModel.Select(p => new ISDSelectIntItem()
            {
                id = p.TargetCode,
                name = p.TargetName
            }).ToList();
            ViewBag.TargetCode = new SelectList(testTargetList, "id", "name", TargetCode);
        }

       
        #endregion
    }
}