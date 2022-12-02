using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using ISD.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ISD.ViewModels.Work;
using ISD.ViewModels.MES;
using System.Threading.Tasks;
using System.IO;

namespace MES.Controllers
{
    public class AssignmentController : BaseController
    {
        // GET: Assignment
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View(new AssignmentViewModel());
        }
        #endregion

        #region Helper
        public void CreateViewBag(Guid? StoreId = null, Guid? WorkShopId = null, Guid? DepartmentId = null, Guid? RoutingId = null)
        {
            //Nhà máy
            var factoryList = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.StoreId = new SelectList(factoryList, "StoreId", "StoreName", StoreId);

            //Phân xưởng
            var workshopList = _context.WorkShopModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.WorkShopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkShopId);
            ViewBag.AssignmentWorkShopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkShopId);

            //Tổ
            var deparmentList = _context.DepartmentModel.Include(p => p.StoreModel).Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            if (WorkShopId.HasValue)
            {
                deparmentList = deparmentList.Where(p => p.WorkShopId == WorkShopId).OrderBy(p => p.OrderIndex).ToList();
            }
            ViewBag.DepartmentId = new SelectList(deparmentList, "DepartmentId", "DepartmentName", DepartmentId);
            ViewBag.AssignmentDepartmentId = new SelectList(deparmentList, "DepartmentId", "DepartmentName", DepartmentId);

            //Công đoạn
            var routingList = _context.RoutingModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.RoutingId = new SelectList(routingList, "StepId", "StepName", RoutingId);
            var assignmentRoutingList = (from a in _context.RoutingModel
                                         join b in _context.Department_Routing_Mapping on a.StepId equals b.StepId
                                         where a.Actived == true
                                         && (DepartmentId == null || b.DepartmentId == DepartmentId)
                                         orderby a.OrderIndex
                                         select new RoutingViewModel()
                                         {
                                             StepId = a.StepId,
                                             StepCode = a.StepCode,
                                             StepName = a.StepName,
                                         }).ToList();
            ViewBag.AssignmentRoutingId = assignmentRoutingList;

            //Nhân viên
            var assigneeList = (from a in _context.SalesEmployeeModel
                                where a.Actived == true
                                && (DepartmentId == null || a.DepartmentId == DepartmentId)
                                orderby a.SalesEmployeeCode
                                select new SalesEmployeeViewModel()
                                {
                                    SalesEmployeeCode = a.SalesEmployeeCode,
                                    SalesEmployeeName = a.SalesEmployeeName,
                                }).ToList();
            ViewBag.EmployeeCode = new SelectList(assigneeList, "SalesEmployeeCode", "SalesEmployeeName");
        }

        //Lấy danh sách tổ
        public ActionResult GetDepartmentBy(Guid? WorkShopId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = (from a in _context.DepartmentModel
                              where a.WorkShopId == WorkShopId
                              orderby a.DepartmentCode
                              select new ISDSelectGuidItem()
                              {
                                  id = a.DepartmentId,
                                  name = a.DepartmentName,
                              }).ToList();

                return _APISuccess(result);
            });
        }

        //Lấy danh sách nhân viên
        public ActionResult GetEmployeeBy(Guid? DepartmentId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = (from a in _context.SalesEmployeeModel
                              where a.DepartmentId == DepartmentId
                              orderby a.SalesEmployeeCode
                              select new ISDSelectStringItem()
                              {
                                  id = a.SalesEmployeeCode,
                                  name = a.SalesEmployeeName,
                              }).ToList();

                return _APISuccess(result);
            });
        }

        //Lấy danh sách công đoạn
        public ActionResult GetRoutingBy(Guid? DepartmentId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = (from a in _context.RoutingModel
                              join b in _context.Department_Routing_Mapping on a.StepId equals b.StepId
                              where a.Actived == true
                              && b.DepartmentId == DepartmentId
                              orderby a.OrderIndex
                              select new RoutingViewModel()
                              {
                                  StepId = a.StepId,
                                  StepCode = a.StepCode,
                                  StepName = a.StepName,
                              }).ToList();

                return _APISuccess(result);
            });
        }

        //Tìm kiếm LSX ĐT theo ngày làm việc
        public ActionResult SearchLSXDTByWorkingDate(DateTime? WorkingDate, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.AssignmentRepository.GetLSXDTByWorkingDate(WorkingDate, SearchText);

                return _APISuccess(result);
            });
        }

        //Tìm kiếm đợt SX theo LSX ĐT
        public ActionResult SearchLSXDByLSXDT(string LSXDT, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.AssignmentRepository.GetLSXDByLSXDT(LSXDT, SearchText);

                return _APISuccess(result);
            });
        }

        //Tìm kiếm LSX SAP theo Đợt SX
        public ActionResult SearchLSXCByLSXD(Guid? LSXD, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.AssignmentRepository.GetLSXCByLSXD(LSXD, SearchText);

                return _APISuccess(result);
            });
        }

        //Láy thông tin sản phẩm theo LSX SAP
        public ActionResult GetProductByLSXC(Guid? LSXC)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.AssignmentRepository.GetProductByLSXC(LSXC);

                return _APISuccess(result);
            });
        }

        //Tìm kiếm chi tiết theo sản phẩm
        public ActionResult SearchItemByMaterialNumberAndPlant(string WERKS, string MATNR, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.AssignmentRepository.GetItemByMaterialNumberAndPlant(WERKS, MATNR, SearchText);

                return _APISuccess(result);
            });
        }

        #endregion

        #region Tìm nhân viên ở popup
        public ActionResult _SearchEmployee(EmployeeAssignmentSearchViewModel searchViewModel)
        {
            List<EmployeeAssignmentSearchResultViewModel> model = _unitOfWork.AssignmentRepository.SearchEmployee(searchViewModel);
            return PartialView("_AddEmployeeInner", model);
        }
        #endregion

        #region Box lệnh sản xuất
        public ActionResult _LenhSanXuat(List<AssignmentLenhSanXuatViewModel> LSXList = null)
        {
            //List<AssignmentLenhSanXuatViewModel> model = _unitOfWork.AssignmentRepository.GetLenhSanXuatList(searchViewModel);
            if (LSXList == null)
            {
                LSXList = new List<AssignmentLenhSanXuatViewModel>();
            }

            return PartialView(LSXList);
        }
        public ActionResult _LenhSanXuatInner(List<AssignmentLenhSanXuatViewModel> LSXList = null)
        {
            //Nếu detail == null thì khởi tạo List mới
            if (LSXList == null)
            {
                LSXList = new List<AssignmentLenhSanXuatViewModel>();
            }

            AssignmentLenhSanXuatViewModel item = new AssignmentLenhSanXuatViewModel();
            LSXList.Add(item);

            return PartialView(LSXList);
        }
        //Xóa dòng trong box lệnh sản xuất
        public ActionResult DeleteLSXC(int? DeleteSTT, List<AssignmentLenhSanXuatViewModel> LSXList = null)
        {
            if (LSXList != null && LSXList.Count > 0 && DeleteSTT.HasValue)
            {
                LSXList = LSXList.Where(p => p.STT != DeleteSTT).ToList();
            }

            return PartialView("_LenhSanXuatInner", LSXList);
        }
        #endregion

        #region Box nhân viên
        public ActionResult _NhanVien(AssignmentViewModel searchViewModel, List<string> NVList, List<string> AddEmployeeList)
        {
            List<string> EmployeeList = new List<string>();
            if (NVList != null && NVList.Count > 0)
            {
                EmployeeList.AddRange(NVList);
            }
            if (AddEmployeeList != null && AddEmployeeList.Count > 0)
            {
                EmployeeList.AddRange(AddEmployeeList);
            }
            EmployeeList = EmployeeList.Distinct().ToList();
            List<AssignmentNhanVienViewModel> model = _unitOfWork.AssignmentRepository.GetNhanVienList(searchViewModel, EmployeeList);
            return PartialView(model);
        }
        #endregion

        #region Box công đoạn
        public ActionResult _CongDoan(List<AssignmentCongDoanViewModel> CongDoanList = null)
        {
            if (CongDoanList == null)
            {
                CongDoanList = new List<AssignmentCongDoanViewModel>();
            }

            return PartialView(CongDoanList);
        }
        public ActionResult _CongDoanInner(AssignmentViewModel model, List<AssignmentCongDoanViewModel> CongDoanList = null)
        {
            //Nếu detail == null thì khởi tạo List mới
            if (CongDoanList == null)
            {
                CongDoanList = new List<AssignmentCongDoanViewModel>();
            }

            AssignmentCongDoanViewModel item = new AssignmentCongDoanViewModel();
            CongDoanList.Add(item);

            CreateViewBag(WorkShopId: model.WorkShopId, DepartmentId: model.DepartmentId);

            return PartialView(CongDoanList);
        }
        //Xóa dòng trong box lệnh sản xuất
        public ActionResult DeleteRouting(int? DeleteSTT, AssignmentViewModel model, List<AssignmentCongDoanViewModel> CongDoanList = null)
        {
            if (CongDoanList != null && CongDoanList.Count > 0 && DeleteSTT.HasValue)
            {
                CongDoanList = CongDoanList.Where(p => p.STT != DeleteSTT).ToList();
            }
            CreateViewBag(WorkShopId: model.WorkShopId, DepartmentId: model.DepartmentId);
            return PartialView("_CongDoanInner", CongDoanList);
        }
        //public ActionResult _CongDoan(AssignmentViewModel searchViewModel, List<string> NVList)
        //{
        //    List<AssignmentCongDoanViewModel> model = _unitOfWork.AssignmentRepository.GetCongDoanList(searchViewModel);

        //    //lấy danh sách NV đã chọn ở box nhân viên để cho NV chọn công đoạn
        //    if (NVList != null && NVList.Count > 0)
        //    {
        //        var AssigneeList = (from a in _context.SalesEmployeeModel
        //                            where NVList.Contains(a.SalesEmployeeCode)
        //                            select new ISDSelectStringItem()
        //                            {
        //                                id = a.SalesEmployeeCode,
        //                                name = a.SalesEmployeeName,
        //                            }).ToList();
        //        ViewBag.AssigneeList = AssigneeList;
        //    }

        //    return PartialView(model);
        //}
        #endregion

        #region Save
        public ActionResult SaveAssignment(AssignmentViewModel model, List<AssignmentLenhSanXuatViewModel> LSXList, List<AssignmentCongDoanViewModel> CongDoanList)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                List<string> errorMessage = new List<string>();

                #region Handle null value
                //AssignmentModel
                if (!model.WorkShopId.HasValue)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, LanguageResource.Master_WorkShop));
                }
                if (!model.DepartmentId.HasValue)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, LanguageResource.Master_Department));
                }
                if (!model.WorkingDate.HasValue)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, LanguageResource.Assignment_WorkingDate));
                }
                if (!model.FromTime.HasValue)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, LanguageResource.Assignment_Time + " " + LanguageResource.Assignment_FromTime));
                }
                if (!model.ToTime.HasValue)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, LanguageResource.Assignment_Time + " " + LanguageResource.Assignment_ToTime));
                }

                //LSX: Assignment_ProductionOrderModel
                if (LSXList == null || LSXList.Count == 0)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, "Lệnh sản xuất"));
                }
                else
                {
                    bool LSXListInvalid = false;
                    foreach (var LSX in LSXList)
                    {
                        if (string.IsNullOrEmpty(LSX.LSXDT) || !LSX.ProductionOrderBatch.HasValue || !LSX.ProductionOrder.HasValue || !LSX.ProductId.HasValue || string.IsNullOrEmpty(LSX.ItemCode) || !LSX.Qty.HasValue)
                        {
                            LSXListInvalid = true;
                        }
                    }
                    if (LSXListInvalid == true)
                    {
                        errorMessage.Add(string.Format("Vui lòng nhập đầy đủ thông tin \"Lệnh sản xuất\""));
                    }
                }

                //Công đoạn thực hiện: Assignment_StepModel
                if (CongDoanList == null || CongDoanList.Count == 0)
                {
                    errorMessage.Add(string.Format(LanguageResource.Required, "Công đoạn thực hiện"));
                }
                else
                {
                    bool CongDoanListInvalid = false;
                    foreach (var CDTH in CongDoanList)
                    {
                        if (!CDTH.WorkShopId.HasValue || !CDTH.DepartmentId.HasValue || string.IsNullOrEmpty(CDTH.EmployeeCode) || !CDTH.RoutingId.HasValue)
                        {
                            CongDoanListInvalid = true;
                        }
                    }
                    if (CongDoanListInvalid == true)
                    {
                        errorMessage.Add(string.Format("Vui lòng nhập đầy đủ thông tin \"Công đoạn thực hiện\""));
                    }
                }


                if (errorMessage != null && errorMessage.Count > 0)
                {
                    return _APIError(null, errorMessage);
                }
                #endregion

                #region Save 
                model.DateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(model.WorkingDate);
                _unitOfWork.AssignmentRepository.SaveAssignment(model, LSXList, CongDoanList, CurrentUser.AccountId);
                _context.SaveChanges();
                #endregion

                return _APISuccess(null, "Đã lưu thông tin \"Phân công công việc\" thành công!");
            });
        }
        #endregion
    }
}