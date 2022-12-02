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
    public class ContRegisterSO60Controller : BaseController
    {
        // GET: ContRegisterSO60
        #region Index
        [ISDAuthorization]
        public ActionResult Index()
        {
            CreateViewBag(Plant: CurrentUser.SaleOrg);
            return View();
        }
        public ActionResult _Search(ContRegisterSO60SearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (from c in _context.ContRegisterSO60Model
                              orderby c.Plant, c.Year, c.Month
                              where
                              //search by Plant
                              (searchViewModel.Plant == null || c.Plant == searchViewModel.Plant)
                              //search by Year
                              && (searchViewModel.Year == null || c.Year == searchViewModel.Year)
                              select new ContRegisterSO60ViewModel()
                              {
                                  ContRegister60Id = c.ContRegister60Id,
                                  Plant = c.Plant,
                                  Customer = c.Customer,
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
        public JsonResult Create(ContRegisterSO60Model model)
        {
            return ExecuteContainer(() =>
            {
                //Check xem các key đã tồn tại chưa 
                //Nếu chưa tồn tại thì thêm mới
                //Nếu đã tồn tại thì báo lỗi
                var existsContRegisterSO60 = _context.ContRegisterSO60Model.Where(p => p.Plant == model.Plant && p.Year == model.Year && p.Month == model.Month && p.Customer == model.Customer).FirstOrDefault();
                if (existsContRegisterSO60 != null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format("Thông tin đăng ký cont đơn hàng 60% theo Plant {0} - Mã KH: {1} - tháng {2} năm {3} đã tồn tại! Vui lòng kiểm tra lại.", model.Plant, model.Customer, model.Month, model.Year)
                    });
                }
                model.ContRegister60Id = Guid.NewGuid();
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_ContRegisterSO60.ToLower())
                });
            });
        }

        #endregion

        #region Edit
        [ISDAuthorization]
        public ActionResult Edit(Guid id)
        {
            var result = (from c in _context.ContRegisterSO60Model
                          where c.ContRegister60Id == id
                          select new ContRegisterSO60ViewModel()
                          {
                              ContRegister60Id = c.ContRegister60Id,
                              Plant = c.Plant,
                              Customer = c.Customer,
                              Month = c.Month,
                              Year = c.Year,
                              Cont = c.Cont
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_ContRegisterSO60.ToLower()) });
            }
            CreateViewBag(result.Plant, result.Year, result.Month);
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorization]
        public JsonResult Edit(ContRegisterSO60Model model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_ContRegisterSO60.ToLower())
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
                var Cont = _context.ContRegisterSO60Model.FirstOrDefault(p => p.ContRegister60Id == id);
                if (Cont != null)
                {

                    _context.Entry(Cont).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_ContRegisterSO60.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.MasterData_ContRegisterSO60.ToLower())
                    });
                }
            });
        }
        #endregion
    }
}