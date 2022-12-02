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
    public class PhysicsWorkShopController : BaseController
    {
        // GET: PhysicsWorkShop
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(PhysicsWorkShopSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (from c in _context.PhysicsWorkShopModel
                                  orderby c.OrderIndex
                                  where
                                  //search by PhysicsWorkShopName
                                  (searchViewModel.PhysicsWorkShopName == null || c.PhysicsWorkShopName.Contains(searchViewModel.PhysicsWorkShopName))
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
        public JsonResult Create(PhysicsWorkShopModel model)
        {
            return ExecuteContainer(() =>
            {
                model.PhysicsWorkShopId = Guid.NewGuid();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_PhysicsWorkShop.ToLower())
                });
            });
        }

        #endregion

        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var result = (from c in _context.PhysicsWorkShopModel
                          where c.PhysicsWorkShopId == id
                          select new PhysicsWorkShopViewModel()
                          {
                              PhysicsWorkShopId = c.PhysicsWorkShopId,
                              PhysicsWorkShopCode = c.PhysicsWorkShopCode,
                              PhysicsWorkShopCodeValid = c.PhysicsWorkShopCode,
                              PhysicsWorkShopName = c.PhysicsWorkShopName,
                              Actived = c.Actived,
                              OrderIndex = c.OrderIndex,
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_PhysicsWorkShop.ToLower()) });
            }
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(PhysicsWorkShopModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_PhysicsWorkShop.ToLower())
                });
            });
        }

        #endregion
        //Check trùng
        #region Remote Validation
        private bool IsExists(string PhysicsWorkShopCode)
        {
            return (_context.PhysicsWorkShopModel.FirstOrDefault(p => p.PhysicsWorkShopCode == PhysicsWorkShopCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingPhysicsWorkShopCode(string PhysicsWorkShopCode, string PhysicsWorkShopCodeValid)
        {
            try
            {
                if (PhysicsWorkShopCodeValid != PhysicsWorkShopCode)
                {
                    return Json(!IsExists(PhysicsWorkShopCode));
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