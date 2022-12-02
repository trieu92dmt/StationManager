using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{
    public class AssignmentRepository
    {
        EntityDataContext _context;
        public AssignmentRepository(EntityDataContext context)
        {
            _context = context;
        }

        //Tìm kiếm nhân viên
        public List<EmployeeAssignmentSearchResultViewModel> SearchEmployee(EmployeeAssignmentSearchViewModel searchViewModel)
        {
            var result = new List<EmployeeAssignmentSearchResultViewModel>();

            var query = (from a in _context.SalesEmployeeModel
                         join b in _context.DepartmentModel on a.DepartmentId equals b.DepartmentId
                         where a.Actived == true
                         //nhà máy
                         && (searchViewModel.StoreId == null || b.StoreId == searchViewModel.StoreId)
                         //phân xưởng
                         && (searchViewModel.AssignmentWorkShopId == null || b.WorkShopId == searchViewModel.AssignmentWorkShopId)
                         select new { a.DepartmentId, b.DepartmentName, a.SalesEmployeeCode, a.SalesEmployeeName });

            //công đoạn
            if (searchViewModel.RoutingId.HasValue)
            {
                query = (from a in query
                         join b in _context.Department_Routing_Mapping on a.DepartmentId equals b.DepartmentId
                         where b.StepId == searchViewModel.RoutingId
                         select a
                        );
            }

            result = query.Select(a => new EmployeeAssignmentSearchResultViewModel()
            {
                DepartmentId = a.DepartmentId,
                EmployeeCode = a.SalesEmployeeCode,
                EmployeeName = a.SalesEmployeeName,
                Department = a.DepartmentName,
            }).ToList();

            //cập nhật thông tin
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var routingNameList = (from a in _context.Department_Routing_Mapping
                                           join b in _context.RoutingModel on a.StepId equals b.StepId
                                           where a.DepartmentId == item.DepartmentId
                                           select b.StepName).ToList();
                    if (routingNameList != null && routingNameList.Count > 0)
                    {
                        item.EmployeeRouting = string.Join(", ", routingNameList);
                    }
                }
            }

            return result;
        }

        //Lấy danh sách lệnh sản xuất
        public List<AssignmentLenhSanXuatViewModel> GetLenhSanXuatList(AssignmentViewModel searchViewModel)
        {
            var result = new List<AssignmentLenhSanXuatViewModel>();
            if (searchViewModel.WorkingDate.HasValue)
            {
                //Lấy thông tin chi tiết theo sản phẩm
                //var ItemList = _context.Database.SqlQuery<View_Product_DetailViewModel>("SELECT * FROM MES.View_Product_Detail").ToList();
                result = (from a in _context.TaskModel
                              //Đợt sản xuất
                          join c in _context.TaskModel on a.ParentTaskId equals c.TaskId
                          //Loại
                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                          //Sản phẩm
                          join b in _context.ProductModel on a.ProductId equals b.ProductId
                          //Loại: LSX SAP
                          where w.WorkFlowCode == ConstWorkFlow.LSXC
                          && !string.IsNullOrEmpty(a.Property5)
                          //Ngày làm việc
                          && a.StartDate <= searchViewModel.WorkingDate && searchViewModel.WorkingDate <= a.EstimateEndDate
                          group a by new { a.Property5, LSXD = c.Summary, LSXC = a.Summary, b.ERPProductCode, b.ProductName, a.Qty } into g
                          select new AssignmentLenhSanXuatViewModel()
                          {
                              LSXDT = g.Key.Property5,
                              LSXD = g.Key.LSXD,
                              LSXC = g.Key.LSXC,
                              ProductCode = g.Key.ERPProductCode,
                              ProductName = g.Key.ProductName,
                              PlanQty = g.Key.Qty,
                          }).ToList();

                //cập nhật thông tin
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        //tính số lượng chi tiết kế hoạch
                        var bomHeader = _context.BOMHeaderModel.Where(p => p.MATNR == item.ProductCode).FirstOrDefault();
                        if (bomHeader != null)
                        {
                            item.PlanBOMQty = _context.BOMDetailModel.Where(p => p.STLNR == bomHeader.STLNR).Count();
                        }
                    }
                }
            }

            return result;
        }

        //Lấy danh sách LSX ĐT theo ngày làm việc
        public List<string> GetLSXDTByWorkingDate(DateTime? WorkingDate, string SearchText)
        {
            var result = new List<string>();
            if (WorkingDate.HasValue)
            {
                result = (from a in _context.TaskModel
                              //Loại
                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                          //Loại: LSX SAP
                          where w.WorkFlowCode == ConstWorkFlow.LSXC
                          && !string.IsNullOrEmpty(a.Property5)
                          //Ngày làm việc
                          && a.StartDate <= WorkingDate && WorkingDate <= a.EstimateEndDate
                          //Tìm theo nội dung search
                          && (SearchText == null || a.Property5.Contains(SearchText))
                          group a by new { a.Property5 } into g
                          select g.Key.Property5).Take(10).ToList();
            }

            return result;
        }

        //Lấy danh sách đợt sản xuất theo lệnh sản xuất đại trà
        public List<ISDSelectGuidItem> GetLSXDByLSXDT(string LSXDT, string SearchText)
        {
            var result = new List<ISDSelectGuidItem>();
            if (!string.IsNullOrEmpty(LSXDT))
            {
                result = (from a in _context.TaskModel
                              //Loại
                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                          //Loại: Đợt
                          where w.WorkFlowCode == ConstWorkFlow.LSXD
                          //Tìm theo LSX DT
                          && a.Property5 == LSXDT
                          //Tìm theo nội dung search
                          && (SearchText == null || a.Summary.Contains(SearchText))
                          orderby a.Number1
                          select new ISDSelectGuidItem
                          {
                              id = a.TaskId,
                              name = a.Summary,
                          }).Take(10).ToList();
            }

            return result;
        }

        //Lấy danh sách lệnh sản xuất SAP theo đợt sản xuất
        public List<ISDSelectGuidItem> GetLSXCByLSXD(Guid? LSXD, string SearchText)
        {
            var result = new List<ISDSelectGuidItem>();
            if (LSXD.HasValue)
            {
                result = (from a in _context.TaskModel
                              //Loại
                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                          //Loại: LSX SAP
                          where w.WorkFlowCode == ConstWorkFlow.LSXC
                          //Tìm theo đợt
                          && a.ParentTaskId == LSXD
                          //Tìm theo nội dung search
                          && (SearchText == null || a.Summary.Contains(SearchText))
                          orderby a.Summary
                          select new ISDSelectGuidItem
                          {
                              id = a.TaskId,
                              name = a.Summary,
                          }).Take(10).ToList();
            }

            return result;
        }

        //Lấy danh sách chi tiết theo sản phẩm
        public List<ISDSelectStringItem> GetItemByMaterialNumberAndPlant(string WERKS, string MATNR, string SearchText, decimal? SLKH = null)
        {
            var result = new List<ISDSelectStringItem>();
            if (!string.IsNullOrEmpty(WERKS) && !string.IsNullOrEmpty(MATNR))
            {
                string sqlQuery = "SELECT * FROM MES.View_Product_Detail WHERE WERKS = " + WERKS + " AND MATNR = " + MATNR;

                if (!string.IsNullOrEmpty(SearchText))
                {
                    sqlQuery += " AND (ITMNO LIKE N'%" + SearchText + "%' OR KTEXT LIKE N'%" + SearchText + "%')";
                }

                //Nếu số lượng kế hoạch <= 50: chỉ lấy những chi tiết có đơn vị tính là cụm
                string DVT = "cụm";
                if (SLKH.HasValue && SLKH <= 50)
                {
                    sqlQuery += " AND (BMEIN LIKE N'%" + DVT + "%')";
                }

                sqlQuery += " ORDER BY ITMNO ";
                var ItemList = _context.Database.SqlQuery<View_Product_DetailViewModel>(sqlQuery).ToList();
                if (!string.IsNullOrEmpty(SearchText))
                {
                    result = (from a in ItemList
                              select new ISDSelectStringItem
                              {
                                  id = a.ITMNO,
                                  name = a.ITMNO + " | " + a.KTEXT,
                                  additional = a.KTEXT,
                              }).Take(10).ToList();
                }
                else
                {
                    result = (from a in ItemList
                              select new ISDSelectStringItem
                              {
                                  id = a.ITMNO,
                                  name = a.ITMNO + " | " + a.KTEXT,
                                  additional = a.KTEXT,
                              }).ToList();
                }
            }

            return result;
        }

        //Lấy thông tin sản phẩm theo lệnh sản xuất SAP
        public ProductViewModel GetProductByLSXC(Guid? LSXC)
        {
            var result = new ProductViewModel();
            if (LSXC.HasValue)
            {
                result = (from a in _context.TaskModel
                              //Loại
                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                          //Sản phẩm
                          join p in _context.ProductModel on a.ProductId equals p.ProductId
                          //Công ty
                          join c in _context.CompanyModel on a.CompanyId equals c.CompanyId
                          //Loại: LSX SAP
                          where w.WorkFlowCode == ConstWorkFlow.LSXC
                          //Tìm theo đợt
                          && a.TaskId == LSXC
                          select new ProductViewModel
                          {
                              ProductId = p.ProductId,
                              ProductCode = p.ERPProductCode,
                              ProductName = p.ProductName,
                              CompanyCode = c.CompanyCode,
                          }).FirstOrDefault();
            }

            return result;
        }

        //Lấy danh sách nhân viên
        public List<AssignmentNhanVienViewModel> GetNhanVienList(AssignmentViewModel searchViewModel, List<string> EmployeeCodeList = null)
        {
            var result = new List<AssignmentNhanVienViewModel>();
            if (EmployeeCodeList != null && EmployeeCodeList.Count > 0)
            {
                result = (from a in _context.SalesEmployeeModel
                          join b in _context.DepartmentModel on a.DepartmentId equals b.DepartmentId
                          join c in _context.WorkShopModel on b.WorkShopId equals c.WorkShopId
                          where a.Actived == true && EmployeeCodeList.Contains(a.SalesEmployeeCode)
                          select new AssignmentNhanVienViewModel()
                          {
                              WorkShop = c.WorkShopName,
                              Department = b.DepartmentName,
                              EmployeeCode = a.SalesEmployeeCode,
                              EmployeeName = a.SalesEmployeeName,
                          }).ToList();
            }
            else
            {
                if (searchViewModel.DepartmentId.HasValue)
                {
                    result = (from a in _context.SalesEmployeeModel
                              join b in _context.DepartmentModel on a.DepartmentId equals b.DepartmentId
                              join c in _context.WorkShopModel on b.WorkShopId equals c.WorkShopId
                              where b.DepartmentId == searchViewModel.DepartmentId
                              && a.Actived == true
                              select new AssignmentNhanVienViewModel()
                              {
                                  WorkShop = c.WorkShopName,
                                  Department = b.DepartmentName,
                                  EmployeeCode = a.SalesEmployeeCode,
                                  EmployeeName = a.SalesEmployeeName,
                              }).ToList();
                }
            }

            return result;
        }

        //Lấy danh sách công đoạn
        public List<AssignmentCongDoanViewModel> GetCongDoanList(AssignmentViewModel searchViewModel)
        {
            var result = new List<AssignmentCongDoanViewModel>();
            if (searchViewModel.DepartmentId.HasValue && (searchViewModel.LSXC != null && searchViewModel.LSXC.Count > 0))
            {
                //1. Lấy theo tổ
                //2. Lấy theo LSX
                //3. Lấy danh sách công đoạn khi có theo tổ và cả LSX
                //VD: Trong LSX có 16 CĐ, trong tổ có 4 CĐ nhưng có 1 CĐ không tồn tại trong 16 CĐ ở LSX => kết quả trả về 3 CĐ
                //=============================================================================================================
                //1.
                var query1 = (from a in _context.Department_Routing_Mapping
                              join b in _context.RoutingModel on a.StepId equals b.StepId
                              where a.DepartmentId == searchViewModel.DepartmentId
                              select new AssignmentCongDoanViewModel() { StepCode = b.StepCode }).Distinct();
                //2.
                var query2 = (from a in _context.TaskModel
                              join b in _context.ProductModel on a.ProductId equals b.ProductId
                              join c in _context.RoutingInventorModel on b.ERPProductCode equals c.MATNR
                              where searchViewModel.LSXC.Contains(a.Summary)
                              select new AssignmentCongDoanViewModel() { StepCode = c.ARBPL_SUB }).Distinct();

                //3.
                var stepCodeLst = (from a in query1
                                   join b in query2 on a.StepCode equals b.StepCode
                                   select b.StepCode).ToList();

                result = (from c in _context.RoutingModel
                          where stepCodeLst.Contains(c.StepCode)
                          group c by new { c.StepCode, c.StepName } into g
                          select new AssignmentCongDoanViewModel()
                          {
                              StepCode = g.Key.StepCode,
                              StepName = g.Key.StepName,
                          }).ToList();

                //cập nhật thông tin
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        //tính SL nhân viên => dựa vào công đoạn thuộc tổ nào => đếm sl nhân viên thuộc tổ này
                        var departmentLst = (from a in _context.RoutingModel
                                             join b in _context.Department_Routing_Mapping on a.StepId equals b.StepId
                                             join c in _context.DepartmentModel on b.DepartmentId equals c.DepartmentId
                                             where a.StepCode == item.StepCode
                                             select c.DepartmentId
                                             ).Distinct().ToList();
                        item.NumberOfEmployees = _context.SalesEmployeeModel.Where(p => departmentLst.Contains(p.DepartmentId.Value)).Count();

                        //tính SL máy => dựa vào công đoạn
                        item.NumberOfMachines = _context.RoutingModel.Where(p => p.StepCode == item.StepCode).Select(p => p.NumberOfMachines).FirstOrDefault();
                    }
                }
            }

            return result;
        }

        //Lưu thông tin phân công
        public void SaveAssignment(AssignmentViewModel model, List<AssignmentLenhSanXuatViewModel> LSXList, List<AssignmentCongDoanViewModel> CongDoanList, Guid? CurrentAccountId)
        {
            #region Save AssignmentModel
            AssignmentModel newAssignment = new AssignmentModel();
            newAssignment.AssignmentId = Guid.NewGuid();
            newAssignment.WorkShopId = model.WorkShopId;
            newAssignment.DepartmentId = model.DepartmentId;
            newAssignment.WorkingDate = model.WorkingDate;
            newAssignment.DateKey = model.DateKey;
            newAssignment.CompleteFromTime = model.WorkingDate.Value.Add(model.FromTime.Value);
            newAssignment.CompleteToTime = model.WorkingDate.Value.Add(model.ToTime.Value);
            newAssignment.CreateTime = DateTime.Now;
            newAssignment.CreateBy = CurrentAccountId;
            _context.Entry(newAssignment).State = System.Data.Entity.EntityState.Added;
            #endregion

            #region Save Assignment_ProductionOrderModel
            if (LSXList != null && LSXList.Count > 0)
            {
                foreach (var LSX in LSXList)
                {
                    Assignment_ProductionOrderModel newAssignment_ProductionOrder = new Assignment_ProductionOrderModel();
                    newAssignment_ProductionOrder.AssignmentProductionOrderId = Guid.NewGuid();
                    newAssignment_ProductionOrder.AssignmentId = newAssignment.AssignmentId;
                    newAssignment_ProductionOrder.PopularProductionOrder = LSX.LSXDT;
                    newAssignment_ProductionOrder.ProductionOrderBatch = LSX.ProductionOrderBatch;
                    newAssignment_ProductionOrder.ProductionOrder = LSX.ProductionOrder;
                    newAssignment_ProductionOrder.ProductId = LSX.ProductId;
                    newAssignment_ProductionOrder.ItemCode = LSX.ItemCode;
                    newAssignment_ProductionOrder.Qty = LSX.Qty;
                    newAssignment_ProductionOrder.IsCompleteBigStep = LSX.IsCompleteBigStep;
                    _context.Entry(newAssignment_ProductionOrder).State = System.Data.Entity.EntityState.Added;
                }
            }
            #endregion

            #region Save Assignment_StepModel
            if (CongDoanList != null && CongDoanList.Count > 0)
            {
                foreach (var CDTH in CongDoanList)
                {
                    Assignment_StepModel newAssignment_Step = new Assignment_StepModel();
                    newAssignment_Step.AssignmentStepId = Guid.NewGuid();
                    newAssignment_Step.AssignmentId = newAssignment.AssignmentId;
                    newAssignment_Step.WorkShopId = CDTH.WorkShopId;
                    newAssignment_Step.DepartmentId = CDTH.DepartmentId;
                    newAssignment_Step.EmployeeCode = CDTH.EmployeeCode;
                    newAssignment_Step.StepId = CDTH.RoutingId;
                    _context.Entry(newAssignment_Step).State = System.Data.Entity.EntityState.Added;
                }
            }
            #endregion
        }
    }
}
