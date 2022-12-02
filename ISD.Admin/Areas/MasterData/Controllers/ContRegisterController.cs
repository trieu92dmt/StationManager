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


namespace MasterData.Controllers
{
    public class ContRegisterController : BaseController
    {
        // GET: ContRegister
        #region Index
        [ISDAuthorization]
        public ActionResult Index()
        {
            CreateViewBag(Plant: CurrentUser.SaleOrg);
            return View();
        }
        public ActionResult _Search(ContRegisterSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (from c in _context.ContRegisterModel
                              orderby c.Plant, c.Year, c.Month
                              where
                              //search by Plant
                              (searchViewModel.Plant == null || c.Plant == searchViewModel.Plant)
                              //search by Year
                              && (searchViewModel.Year == null || c.Year == searchViewModel.Year)
                              select new ContRegisterViewModel()
                              {
                                  ContRegisterId = c.ContRegisterId,
                                  Plant = c.Plant,
                                  Month = c.Month,
                                  Year = c.Year,
                                  Cont = c.Cont
                              }).ToList();

                return PartialView(result);
            });
        }
        #endregion

        #region Create
        [ISDAuthorization]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorization]
        public JsonResult Create(ContRegisterModel model)
        {
            return ExecuteContainer(() =>
            {
                //Check xem các key đã tồn tại chưa 
                //Nếu chưa tồn tại thì thêm mới
                //Nếu đã tồn tại thì báo lỗi
                var existsContRegister = _context.ContRegisterModel.Where(p => p.Plant == model.Plant && p.Year == model.Year && p.Month == model.Month).FirstOrDefault();
                if (existsContRegister != null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format("Thông tin đăng ký cont theo Plant {0} - tháng {1} năm {2} đã tồn tại! Vui lòng kiểm tra lại.", model.Plant, model.Month, model.Year)
                    });
                }
                model.ContRegisterId = Guid.NewGuid();
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_ContRegister.ToLower())
                });
            });
        }

        #endregion

        #region Edit
        [ISDAuthorization]
        public ActionResult Edit(Guid id)
        {
            var result = (from c in _context.ContRegisterModel
                          where c.ContRegisterId == id
                          select new ContRegisterViewModel()
                          {
                              ContRegisterId = c.ContRegisterId,
                              Plant = c.Plant,
                              Month = c.Month,
                              Year = c.Year,
                              Cont = c.Cont
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_ContRegister.ToLower()) });
            }
            CreateViewBag(result.Plant, result.Year, result.Month);
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorization]
        public JsonResult Edit(ContRegisterModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_ContRegister.ToLower())
                });
            });
        }

        #endregion

        #region Helper
        public void CreateViewBag(string Plant = null, int? Year = null, int? Month = null)
        {
            //Plant
            var plantList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
            ViewBag.Plant = new SelectList(plantList, "SaleOrgCode", "StoreName", Plant);
            //Năm
            List<ISDSelectIntItem> yearList = new List<ISDSelectIntItem>();
            //1. Lấy dữ liệu từ năm 2021
            int fromYear = 2021;
            //2. Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;
            //3. Lấy năm hiện tại trừ 2021 để lấy các năm từ 2021 đến hiện tại
            int previousYearTotal = currentYear - fromYear;
            //4. Lấy các năm cần hiển thị
            for (int i = 0; i < previousYearTotal; i++)
            {
                yearList.Add(new ISDSelectIntItem()
                {
                    id = fromYear + i,
                    name = (fromYear + i).ToString(),
                });
            }
            for (int i = 0; i < 5; i++)
            {
                yearList.Add(new ISDSelectIntItem()
                {
                    id = currentYear + i,
                    name = (currentYear + i).ToString(),
                });
            }
            ViewBag.Year = new SelectList(yearList, "id", "name", Year);

            //Tháng
            List<ISDSelectIntItem> monthList = new List<ISDSelectIntItem>();
            for (int i = 0; i < 12; i++)
            {
                monthList.Add(new ISDSelectIntItem()
                {
                    id = i + 1,
                    name = (i + 1).ToString(),
                });
            }
            ViewBag.Month = new SelectList(monthList, "id", "name", Month);
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ISDAuthorization]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var Cont = _context.ContRegisterModel.FirstOrDefault(p => p.ContRegisterId == id);
                if (Cont != null)
                {

                    _context.Entry(Cont).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_ContRegister.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.MasterData_ContRegister.ToLower())
                    });
                }
            });
        }
        #endregion
    }
}