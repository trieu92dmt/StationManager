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
    public class CapacityRegisterController : BaseController
    {
        // GET: ErrorList
        #region Index
        [ISDAuthorization]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(CapacityRegisterSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var result = (from c in _context.CapacityRegisterModel
                              //join d in _context.AllDepartmentModel on c.WorkShopCode equals d.DepartmentCode into dTemp
                              //from dpt in dTemp.DefaultIfEmpty()
                              join e in _context.PhysicsWorkShopModel on c.WorkShopCode equals e.PhysicsWorkShopCode into eTemp
                              from pws in eTemp.DefaultIfEmpty()
                              orderby pws.OrderIndex, c.Year, c.Month
                              where
                              //search by WorkShop
                              (searchViewModel.WorkShopCode == null || c.WorkShopCode == searchViewModel.WorkShopCode)
                              //search by Year
                              && (searchViewModel.Year == null || c.Year == searchViewModel.Year)
                              select new CapacityRegisterViewModel()
                              {
                                  CapacityRegisterId = c.CapacityRegisterId,
                                  //WorkShopName = string.IsNullOrEmpty(dpt.DepartmentName) ? pws.PhysicsWorkShopName : dpt.DepartmentName,
                                  WorkShopName = pws.PhysicsWorkShopName,
                                  Year = c.Year,
                                  Month = c.Month,
                                  Capacity = c.Capacity,
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
        public JsonResult Create(CapacityRegisterModel model)
        {
            return ExecuteContainer(() =>
            {
                //Check xem các key đã tồn tại chưa 
                //Nếu chưa tồn tại thì thêm mới
                //Nếu đã tồn tại thì báo lỗi
                var existsCapacityRegister = _context.CapacityRegisterModel.Where(p => p.WorkShopCode == model.WorkShopCode && p.Year == model.Year && p.Month == model.Month).FirstOrDefault();
                if (existsCapacityRegister != null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format("Thông tin đăng ký capacity theo phân xường {0} - năm {1} đã tồn tại! Vui lòng kiểm tra lại.", model.WorkShopCode, model.Year)
                    });
                }
                model.CapacityRegisterId = Guid.NewGuid();
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_CapacityRegister.ToLower())
                });
            });
        }

        #endregion
        
        #region Edit
        [ISDAuthorization]
        public ActionResult Edit(Guid id)
        {
            var result = (from c in _context.CapacityRegisterModel
                          join e in _context.PhysicsWorkShopModel on c.WorkShopCode equals e.PhysicsWorkShopCode into eTemp
                          from pws in eTemp.DefaultIfEmpty()
                          where c.CapacityRegisterId == id
                          select new CapacityRegisterViewModel()
                          {
                              CapacityRegisterId = c.CapacityRegisterId,
                              WorkShopCode = c.WorkShopCode,
                              WorkShopName = pws.PhysicsWorkShopName,
                              Year = c.Year,
                              Month = c.Month,
                              Capacity = c.Capacity,
                          }).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_CapacityRegister.ToLower()) });
            }
            CreateViewBag(result.WorkShopCode, result.Year, result.Month);
            return View(result);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorization]
        public JsonResult Edit(CapacityRegisterModel model)
        {
            return ExecuteContainer(() =>
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_CapacityRegister.ToLower())
                });
            });
        }

        #endregion

        #region Helper
        public void CreateViewBag(string WorkShopCode = null, int? Year = null, int? Month = null)
        {
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
                //additional = "Phân xưởng vật lý"
            }).ToList();
            var allDepartmentList = new List<ISDSelectStringItem>();
            //allDepartmentList.AddRange(departmentList);
            allDepartmentList.AddRange(physicsWorkshopList);
            //ViewBag.WorkShopCode = new SelectList(allDepartmentList, dataValueField: "id", dataTextField: "name", dataGroupField: "additional", selectedValue: WorkShopCode);
            ViewBag.WorkShopCode = new SelectList(allDepartmentList, dataValueField: "id", dataTextField: "name", selectedValue: WorkShopCode);

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
                var capacity = _context.CapacityRegisterModel.FirstOrDefault(p => p.CapacityRegisterId == id);
                if (capacity != null)
                {

                    _context.Entry(capacity).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_CapacityRegister.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.MasterData_CapacityRegister.ToLower())
                    });
                }
            });
        }
        #endregion
    }
}