using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.Core;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ISD.ViewModels;

namespace MasterData.Controllers
{
    public class AllDepartmentController : BaseController
    {
        // GET: AllDepartment
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(AllDepartmentSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (from c in _context.AllDepartmentModel
                                  orderby c.OrderIndex
                                  where
                                  //search by DepartmentName
                                  (searchViewModel.DepartmentName == null || c.DepartmentName.Contains(searchViewModel.DepartmentName))
                                  //search by Actived
                                  && (searchViewModel.Actived == null || c.Actived == searchViewModel.Actived)
                                  select c).ToList();

                return PartialView(result);
            });
        }
        #endregion

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
        public JsonResult Create(AllDepartmentModel model)
        {
            return ExecuteContainer(() =>
            {
                model.AllDepartmentId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_AllDepartment.ToLower())
                });
            });
        }

        #endregion

        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var result = (from c in _context.AllDepartmentModel
                          where c.AllDepartmentId == id
                          select new AllDepartmentViewModel()
                          {
                              AllDepartmentId = c.AllDepartmentId,
                              DepartmentCode = c.DepartmentCode,
                              DepartmentCodeValid = c.DepartmentCode,
                              DepartmentName = c.DepartmentName,
                              Actived = c.Actived,
                              OrderIndex = c.OrderIndex,
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_AllDepartment.ToLower()) });
            }
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(AllDepartmentModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_AllDepartment.ToLower())
                });
            });
        }

        #endregion
        //Check trùng
        #region Remote Validation
        private bool IsExists(string DepartmentCode)
        {
            return (_context.AllDepartmentModel.FirstOrDefault(p => p.DepartmentCode == DepartmentCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingDepartmentCode(string DepartmentCode, string DepartmentCodeValid)
        {
            try
            {
                if (DepartmentCodeValid != DepartmentCode)
                {
                    return Json(!IsExists(DepartmentCode));
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
    }
}