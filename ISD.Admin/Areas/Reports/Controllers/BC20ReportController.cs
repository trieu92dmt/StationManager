using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reports.Options;

namespace Reports.Controllers
{
    public class BC20ReportController : BaseController
    {
        // GET: BC20Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var currentDate = DateTime.Now;
            BC20ReportViewModel viewModel = new BC20ReportViewModel();
            viewModel.ReportDate = currentDate;
            CreateViewBag();
            return View(viewModel);
        }

        public ActionResult ViewReport(BC20ReportViewModel viewModel)
        {
            if (viewModel.WorkShop == null || viewModel.WorkShop.Count == 0)
            {
                return RedirectToAction("Index");
            }
            var currentDate = DateTime.Now;
            //Nếu field ngày không nhập giá trị hoặc chọn xem theo ngày hiện tại thì lấy ngày hiện tại
            if (!viewModel.ReportDate.HasValue || viewModel.IsViewAtCurrent == true)
            {
                viewModel.ReportDate = currentDate;
            }
            ViewBag.NgayHienTai = viewModel.ReportDate.Value.ToString("dd/MM/yyyy");
            //Phân xưởng
            var phanXuongLst = _context.WorkShopModel.Where(p => viewModel.WorkShop.Contains(p.WorkShopId)).Select(p => p.WorkShopName).ToList();
            ViewBag.PhanXuong = string.Join(", ", phanXuongLst);
            //return PartialView("_BC20ReportPartial");
            return View(viewModel);
        }

        public ActionResult GetData(BC20ReportViewModel viewModel)
        {
            var currentDate = DateTime.Now;
            //Nếu field ngày không nhập giá trị hoặc chọn xem theo ngày hiện tại thì lấy ngày hiện tại
            if (!viewModel.ReportDate.HasValue || viewModel.IsViewAtCurrent == true)
            {
                viewModel.ReportDate = currentDate;
            }
            //Bắt buộc chọn thông tin phân xưởng
            if (viewModel.WorkShop == null || viewModel.WorkShop.Count == 0)
            {
                return _APIError("Vui lòng chọn thông tin phân xưởng!");
            }
            //Lấy danh sách tổ theo phân xưởng
            else
            {
                if (viewModel.Department == null || viewModel.Department.Count == 0 || viewModel.Department.FirstOrDefault() == string.Empty)
                {
                   viewModel.Department = new List<string>();
                   viewModel.Department = _context.DepartmentModel.Where(p => viewModel.WorkShop.Contains(p.WorkShopId)).Select(p => p.DepartmentCode).ToList();
                }
            }
            string error = string.Empty;
            var result = _unitOfWork.BC20ReportRepository.GetData(viewModel, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return _APIError(error);
            }
            if (result != null && result.ChiTiet != null && result.ChiTiet.Count == 2)
            {
                return _APIError("Không tìm thấy dữ liệu báo cáo. Vui lòng nhập thông tin khác!");
            }

            return _APISuccess(result);
        }

        #region Helper
        public void CreateViewBag()
        {
            //Phân xưởng
            var workshopList = _context.WorkShopModel.Where(p => (bool)p.Actived && p.CompanyId == CurrentUser.CompanyId).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.WorkShop = new SelectList(workshopList, "WorkShopId", "WorkShopName");

            ////Tổ
            //var deparmentList = _context.DepartmentModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            //if (WorkShopId.HasValue)
            //{
            //    deparmentList = deparmentList.Where(p => p.WorkShopId == WorkShopId).OrderBy(p => p.OrderIndex).ToList();
            //}
            //ViewBag.Department = new SelectList(deparmentList, "DepartmentCode", "DepartmentName", DepartmentCode);
            var deparmentList = new List<ISD.EntityModels.DepartmentModel>();
            ViewBag.Department = new SelectList(deparmentList, "DepartmentCode", "DepartmentName");
        }

        public ActionResult GetDepartmentBy(List<Guid?> WorkShop)
        {
            //Tổ
            var deparmentList = _context.DepartmentModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex)
                                                        .Select(p => new DepartmentViewModel()
                                                        {
                                                            WorkShopId = p.WorkShopId,
                                                            DepartmentCode = p.DepartmentCode,
                                                            DepartmentName = p.DepartmentName,
                                                        }).ToList();
            if (WorkShop != null && WorkShop.Count > 0)
            {
                deparmentList = deparmentList.Where(p => WorkShop.Contains(p.WorkShopId)).OrderBy(p => p.OrderIndex)
                                            .Select(p => new DepartmentViewModel() {
                                                DepartmentCode = p.DepartmentCode,
                                                DepartmentName = p.DepartmentName,
                                            }).ToList();
            }
            return Json(deparmentList, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}