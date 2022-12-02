using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class BC21ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC21ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo 20
        /// </summary>
        /// <returns>BC21ReportViewModel</returns>
        public BC21ReportViewModel GetData(BC21ReportViewModel searchViewModel, out string error)
        {
            BC21ReportViewModel result = new BC21ReportViewModel();
            //Lấy dữ liệu từ SAP
            error = string.Empty;
            var dataTables = GetSAPData(searchViewModel, out error);
            if (dataTables != null && dataTables.Count > 0)
            {
                try
                {
                    //Xem theo tổ
                    var I_MBX = dataTables[0];
                    if (I_MBX != null && I_MBX.Rows.Count > 0)
                    {
                        result.ChiTiet = new List<BC21ReportDetailViewModel>();
                        result.ChiTiet.Add(new BC21ReportDetailViewModel()
                        {
                            To = "Xưởng Kế hoạch",
                            MaTo = "00000000",
                            TenTo = "Xưởng",
                            SoPhut = 0,
                        });
                        result.ChiTiet.Add(new BC21ReportDetailViewModel()
                        {
                            To = "Xưởng Thực tế",
                            MaTo = "00000001",
                            TenTo = "Xưởng",
                            KeHoach = 0,
                            ThucTe = 0,
                            SoPhut = 0,
                        });
                        foreach (DataRow mbx in I_MBX.Rows)
                        {
                            var ACDATE = mbx["ACDATE"].ToString();
                            var ORGEH = mbx["ORGEH"].ToString();
                            var ZNSNLD = mbx["ZNSNLD"].ToString();
                            var ZNSLQ = mbx["ZNSLQ"].ToString();
                            var SALONROUT = mbx["SALONROUT"].ToString();
                            //Kế hoạch
                            var TenToKeHoach = _context.DepartmentModel.Where(p => p.DepartmentCode == ORGEH).Select(p => p.DepartmentName).FirstOrDefault();
                            result.ChiTiet.Add(new BC21ReportDetailViewModel()
                            {
                                NgayHienTai = DateTime.ParseExact(ACDATE, "yyyyMMdd",
                                                   System.Globalization.CultureInfo.InvariantCulture),
                                MaTo = ORGEH,
                                TenTo = TenToKeHoach,
                                To = TenToKeHoach + " Kế hoạch",
                                KeHoach = !string.IsNullOrEmpty(ZNSNLD) ? decimal.Parse(ZNSNLD) : 0,
                                ThucTe = !string.IsNullOrEmpty(SALONROUT) ? decimal.Parse(SALONROUT) : 0,
                                SoPhut = !string.IsNullOrEmpty(ZNSNLD) ? decimal.Parse(ZNSNLD) : 0
                            });
                            //Thực tế
                            var TenToThucTe = _context.DepartmentModel.Where(p => p.DepartmentCode == ORGEH).Select(p => p.DepartmentName).FirstOrDefault();
                            result.ChiTiet.Add(new BC21ReportDetailViewModel()
                            {
                                NgayHienTai = DateTime.ParseExact(ACDATE, "yyyyMMdd",
                                                   System.Globalization.CultureInfo.InvariantCulture),
                                MaTo = ORGEH,
                                TenTo = TenToThucTe,
                                To = TenToThucTe + " Thực tế",
                                KeHoach = !string.IsNullOrEmpty(ZNSNLD) ? decimal.Parse(ZNSNLD) : 0,
                                ThucTe = !string.IsNullOrEmpty(SALONROUT) ? decimal.Parse(SALONROUT) : 0,
                                SoPhut = !string.IsNullOrEmpty(SALONROUT) ? decimal.Parse(SALONROUT) : 0,
                            });
                        }
                        //Cập nhật số lượng cho Xưởng
                        if (result != null && result.ChiTiet.Count > 0)
                        {
                            result.ChiTiet = result.ChiTiet.Where(p => searchViewModel.Department == null || searchViewModel.Department.Count == 0 || searchViewModel.Department.Contains(p.MaTo)
                                                                        || p.MaTo == "00000000"
                                                                        || p.MaTo == "00000001")
                                                                 .OrderBy(p => p.MaTo).ToList();

                            foreach (var item in result.ChiTiet)
                            {
                                //Xưởng kế hoạch
                                if (item.MaTo == "00000000")
                                {
                                    item.SoPhut = result.ChiTiet.Where(p => p.MaTo != "00000000" && p.MaTo != "000000001" && p.To.Contains("Kế hoạch")).Sum(p => p.SoPhut);
                                }
                                //Xưởng thực tế
                                if (item.MaTo == "00000001")
                                {
                                    item.SoPhut = result.ChiTiet.Where(p => p.MaTo != "00000000" && p.MaTo != "000000001" && p.To.Contains("Thực tế")).Sum(p => p.SoPhut);
                                }

                                //Danh sách tên dòng
                                result.TenDong = new List<string>();
                                result.TenDong = result.ChiTiet.OrderBy(p => p.MaTo).ThenBy(p => p.TenTo).Select(p => p.TenTo).Distinct().ToList();

                                //Danh sách số lượng kế hoạch
                                result.SoPhutKeHoach = new List<decimal?>();
                                result.SoPhutKeHoach = result.ChiTiet.Where(p => p.To.Contains("Kế hoạch")).OrderBy(p => p.MaTo).ThenBy(p => p.TenTo).Select(p => p.SoPhut).ToList();

                                //Danh sách số lượng thực tế
                                result.SoPhutThucTe = new List<decimal?>();
                                result.SoPhutThucTe = result.ChiTiet.Where(p => p.To.Contains("Thực tế")).OrderBy(p => p.MaTo).ThenBy(p => p.TenTo).Select(p => p.SoPhut).ToList();
                            }
                        }
                    }
                    else
                    {
                        error = "No data returned from SAP";
                    }
                    //Xem theo nhân viên
                    var I_PSN = dataTables[1];
                    if (I_PSN != null && I_PSN.Rows.Count > 0)
                    {
                        result.NhanVien = new List<BC21ReportEmployeeDetailViewModel>();
                        foreach (DataRow mbx in I_PSN.Rows)
                        {
                            var ACDATE = mbx["ACDATE"].ToString();
                            var PERNR = mbx["PERNR"].ToString();
                            var ORGEH = mbx["ORGEH"].ToString();
                            var SALONBASE = mbx["SALONBASE"].ToString();
                            var HSTAYN = mbx["HSTAYN"].ToString();
                            var HSHTCV = mbx["HSHTCV"].ToString();
                            var SALONROUT = mbx["SALONROUT"].ToString();
                            var ZPERCENT = mbx["ZPERCENT"].ToString();
                            var ZNSLQ = mbx["ZNSLQ"].ToString();
                            var PER_NAME = mbx["PER_NAME"].ToString();
                            //check nhân viên lọc theo tổ
                            var nhanVienFilter = _context.SalesEmployeeModel.Where(p => p.SalesEmployeeCode == PERNR).FirstOrDefault();
                            var toFilter = _context.DepartmentModel.Where(p => searchViewModel.Department.Contains(p.DepartmentCode)).Select(p => p.DepartmentId).ToList();
                            //if (nhanVienFilter != null && toFilter.Contains(nhanVienFilter.DepartmentId.Value) && searchViewModel.Department.Contains(ORGEH))
                            if (searchViewModel.Department.Contains(ORGEH))
                            {
                                //master data
                                var masterData = (from t in _context.DepartmentModel
                                                  join px in _context.WorkShopModel on t.WorkShopId equals px.WorkShopId
                                                  where t.DepartmentCode == ORGEH
                                                  select new
                                                  {
                                                      t.DepartmentName,
                                                      px.WorkShopName,
                                                      px.WorkShopCode,
                                                  }).FirstOrDefault();
                                if (!string.IsNullOrEmpty(PERNR))
                                {
                                    result.NhanVien.Add(new BC21ReportEmployeeDetailViewModel()
                                    {
                                        ThoiGian = DateTime.ParseExact(ACDATE, "yyyyMMdd",
                                                      System.Globalization.CultureInfo.InvariantCulture),
                                        To = masterData != null ? ORGEH + " | " + masterData.DepartmentName : null,
                                        MaTo = ORGEH,
                                        PhanXuong = masterData != null ? masterData.WorkShopName : null,
                                        MaPhanXuong = masterData != null ? masterData.WorkShopCode : null,
                                        CongNhan = PERNR + " | " + PER_NAME,
                                        //NangSuatDuKienTheoRouting = !string.IsNullOrEmpty(SALONROUT) ? decimal.Parse(SALONROUT) : 0,
                                        //NangSuatTheoSoNLD = !string.IsNullOrEmpty(ZNSNLD) ? decimal.Parse(ZNSNLD) : 0,
                                        //NangSuatLuyKeDenHienTai = !string.IsNullOrEmpty(ZNSLQ) ? decimal.Parse(ZNSLQ) : 0,
                                        TienKH = !string.IsNullOrEmpty(SALONBASE) ? decimal.Parse(SALONBASE) : 0,
                                        HSTN = !string.IsNullOrEmpty(HSTAYN) ? decimal.Parse(HSTAYN) : 0,
                                        HSHTCV = !string.IsNullOrEmpty(HSHTCV) ? decimal.Parse(HSHTCV) : 0,
                                        TienDuKienRouting = !string.IsNullOrEmpty(SALONROUT) ? decimal.Parse(SALONROUT) : 0,
                                        PhanTramHT = !string.IsNullOrEmpty(ZPERCENT) ? decimal.Parse(ZPERCENT) : 0,
                                        TongLuyKe = !string.IsNullOrEmpty(ZNSLQ) ? decimal.Parse(ZNSLQ) : 0,
                                    });
                                }
                            }
                        }
                        if (result.NhanVien != null && result.NhanVien.Count > 0)
                        {
                            result.NhanVien = result.NhanVien.OrderBy(p => p.MaPhanXuong).ThenBy(p => p.MaTo).ThenBy(p => p.CongNhan).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            else
            {
                error = "No data returned from SAP";
            }
            return result;
        }
        private List<DataTable> GetSAPData(BC21ReportViewModel searchViewModel, out string errorMessage)
        {
            errorMessage = string.Empty;
            //SAPRepository _sap = new SAPRepository();
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_REPORT_MBX_CP);

                ////function.SetValue("I_ORGEH", I_ORGEH);
                ////function.SetValue("I_SAL", I_SAL);

                //function.SetValue("I_ACDATE_F", searchViewModel.ReportDate);
                //function.SetValue("I_ACDATE_T", searchViewModel.ReportDate);

                //function.Invoke(destination);

                //var table1 = function.GetTable("I_ORG").ToDataTable("I_ORG");
                //dataTables.Add(table1);
                //var table2 = function.GetTable("I_PSN").ToDataTable("I_PSN");
                //dataTables.Add(table2);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage = ex.InnerException.InnerException.Message;
                    }
                }
            }
            return dataTables;

        }
    }
}
