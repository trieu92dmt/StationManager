using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.Repositories.MES;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class BC07ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC07ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo xuất hàng
        /// </summary>
        /// <returns>BC07ReportViewModel</returns>
        public List<BC07ReportViewModel> GetData(BC07ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC07Report] @LSX, @DSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant, @WorkShop, @DeliveryFromDate, @DeliveryToDate, @Customer, @isOpen";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSX",
                    Value = searchViewModel.LSX ?? (object)DBNull.Value,
                },
                 new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DSX",
                    Value = searchViewModel.DSX ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "VBELN",
                    Value = searchViewModel.VBELN ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "POSNR",
                    Value = searchViewModel.POSNR ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedFromDate",
                    Value = searchViewModel.CompletedFromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedToDate",
                    Value = searchViewModel.CompletedToDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "TopRow",
                    Value = searchViewModel.TopRow ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Plant",
                    Value = searchViewModel.Plant ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkShop",
                    Value = searchViewModel.WorkShop ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DeliveryFromDate",
                    Value = searchViewModel.DeliveryFromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DeliveryToDate",
                    Value = searchViewModel.DeliveryToDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Customer",
                    Value = searchViewModel.Customer ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "isOpen",
                    Value = searchViewModel.isOpen ?? (object)DBNull.Value,
                },
            };
            #endregion

            List<BC07ReportViewModel> result = _context.Database.SqlQuery<BC07ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo 07
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC07()
        {
            var _loggerRepository = new LoggerRepository();
            try
            {
                EntityDataContext _newContext = new EntityDataContext();
                //Lấy danh sách LSX SAP
                var allLSXSAPList = await (from p in _newContext.BC01Model
                                           join t in _newContext.TaskModel on p.LSXSAPId equals t.TaskId
                                           where t.isDeleted == null || t.isDeleted == false
                                           select new BC07ReportViewModel
                                           {
                                               LSXSAP = p.LSXSAP,
                                               Plant = p.Plant,
                                               DSX = p.DSX,
                                               CreateTime = p.CreateTime
                                           }).OrderByDescending(p => p.CreateTime).ToListAsync();
                IList<BC07ReportViewModel> LSXSAPList = (from p in allLSXSAPList
                                                         group p by new { p.LSXSAP, p.Plant, p.DSX } into g
                                                         select new BC07ReportViewModel
                                                         {
                                                             LSXSAP = g.Key.LSXSAP,
                                                             Plant = g.Key.Plant,
                                                             DSX = g.Key.DSX,
                                                         }).ToList();
                //Lấy ưu tiên những lệnh chưa tồn tại trong BC07
                var BC07Lst = await (from p in _context.BC07Model
                                     group p by p.LSXSAP into g
                                     select new BC07ReportViewModel { LSXSAP = g.Key }
                                    ).ToListAsync();
                var LSXSAPExistsLst = BC07Lst.Select(p => p.LSXSAP).ToList();
                if (BC07Lst != null && BC07Lst.Count > 0)
                {
                    LSXSAPList = LSXSAPList.Where(p => !LSXSAPExistsLst.Contains(p.LSXSAP)).ToList();
                } 

                _loggerRepository.Logging("BC07 LSXSAPList Count: " + LSXSAPList.Count, "INFO");
                if (LSXSAPList != null)
                {

                    foreach (var LSXSAP in LSXSAPList)
                    {
                        try
                        {

                            //Gán thông số chạy stored để cập nhật vào DB
                            BC07ReportViewModel searchViewModel = new BC07ReportViewModel();
                            searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                            searchViewModel.Plant = LSXSAP.Plant;
                            searchViewModel.DSX = LSXSAP.DSX;

                            string sqlQuery = "[Report].[usp_BC07Report_GetData] @LSX, @DSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant, @WorkShop, @Customer";
                            #region Parameters
                            List<SqlParameter> parameters = new List<SqlParameter>()
                        {
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "LSX",
                                Value = searchViewModel.LSX ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "DSX",
                                Value = (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "LSXSAP",
                                Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "VBELN",
                                Value = searchViewModel.VBELN ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "POSNR",
                                Value = searchViewModel.POSNR ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = "CompletedFromDate",
                                Value = searchViewModel.CompletedFromDate ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = "CompletedToDate",
                                Value = searchViewModel.CompletedToDate ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.Int,
                                Direction = ParameterDirection.Input,
                                ParameterName = "TopRow",
                                Value = (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "Plant",
                                Value = searchViewModel.Plant ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "WorkShop",
                                Value = searchViewModel.WorkShop ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "Customer",
                                Value = searchViewModel.Customer ?? (object)DBNull.Value,
                            },
                        };
                            #endregion
                            _newContext.Database.CommandTimeout = 1200;
                            List<BC07ReportViewModel> result = await _newContext.Database.SqlQuery<BC07ReportViewModel>(sqlQuery, parameters.ToArray()).ToListAsync();

                            //Lưu thông tin vào bảng BC07
                            if (result != null && result.Count > 0)
                            {
                                foreach (var item in result)
                                {
                                    int? STT = null;
                                    if (item.STT.HasValue)
                                    {
                                        STT = (int)item.STT;
                                    }
                                    //Check dữ liệu
                                    var existBC07 = await _newContext.BC07Model.Where(p => p.LSXSAPId == item.LSXSAPId).FirstOrDefaultAsync();
                                    //1. Nếu tồn tại => update
                                    if (existBC07 != null)
                                    {
                                        existBC07.STT = STT;
                                        existBC07.Plant = item.Plant;
                                        existBC07.AssignResponsibility = item.AssignResponsibility;
                                        existBC07.Customer = item.Customer;
                                        existBC07.PYCSXDT = item.PYCSXDT;
                                        existBC07.LSXDT = item.LSXDT;
                                        existBC07.DSX = item.DSX;
                                        existBC07.VBELN = item.VBELN;
                                        existBC07.ProductName = item.ProductName;
                                        existBC07.ProductCode = item.ProductCode;
                                        existBC07.LSXSAP = item.LSXSAP;
                                        existBC07.WBS = item.WBS;
                                        existBC07.TinhTrangMT = item.TinhTrangMT;
                                        existBC07.NguyenLieu = item.NguyenLieu;
                                        existBC07.HoanThien = item.HoanThien;
                                        existBC07.HinhAnh = item.HinhAnh;
                                        existBC07.DVT = item.DVT;
                                        existBC07.SLKH = item.SLKH;
                                        existBC07.SLDaNK = item.SLDaNK;
                                        existBC07.SLChuaNK = item.SLChuaNK;
                                        existBC07.SLDaXuat = item.SLDaXuat;
                                        existBC07.SLChuaXuat = item.SLChuaXuat;
                                        existBC07.SLTonKho = item.SLTonKho;
                                        existBC07.SoContKH = item.SoContKH;
                                        existBC07.SoContDaNK = item.SoContDaNK;
                                        existBC07.SoContChuaNK = item.SoContChuaNK;
                                        existBC07.SoContDaXuat = item.SoContDaXuat;
                                        existBC07.SoContChuaXuat = item.SoContChuaXuat;
                                        existBC07.SoContTonKho = item.SoContTonKho;
                                        existBC07.NgayKTDKTheoLSXSAP = item.NgayKTDKTheoLSXSAP;
                                        existBC07.NgayKTDKTheoDSX = item.NgayKTDKTheoDSX;
                                        existBC07.SLSP = item.SLSP;
                                        existBC07.CBM = item.CBM;
                                        existBC07.TongCBMChuaXuat = item.TongCBMChuaXuat;
                                        existBC07.PONoiBo = item.PONoiBo;
                                        existBC07.SoPOKhach = item.SoPOKhach;
                                        existBC07.MaSKU = item.MaSKU;
                                        existBC07.CangDen = item.CangDen;
                                        existBC07.LoaiCont = item.LoaiCont;
                                        existBC07.NgayTauChay = item.NgayTauChay;
                                        existBC07.StartSWDate = item.StartSWDate;
                                        existBC07.EndSWDate = item.EndSWDate;
                                        existBC07.AustinDirect = item.AustinDirect;
                                        existBC07.TuanXuatHang = item.TuanXuatHang;
                                        existBC07.ThangXuatHang = item.ThangXuatHang;
                                        existBC07.NgayYeuCauXuatHangSO = item.NgayYeuCauXuatHangSO;
                                        existBC07.CompletedPercentGo = item.CompletedPercentGo;
                                        existBC07.CompletedPercentVan = item.CompletedPercentVan;
                                        existBC07.CompletedPercentGiaCong = item.CompletedPercentGiaCong;
                                        existBC07.CompletedPercentVatTu = item.CompletedPercentVatTu;
                                        existBC07.CompletedPercentHoaChat = item.CompletedPercentHoaChat;
                                        existBC07.CompletedPercentBBPL = item.CompletedPercentBBPL;
                                        //Phân xưởng Phôi
                                        existBC07.PXP_SoContKH = item.PXP_SoContKH;
                                        existBC07.PXP_SoContHT = item.PXP_SoContHT;
                                        existBC07.PXP_SoContConLai = item.PXP_SoContConLai;
                                        existBC07.PXP_NgayYCHT = item.PXP_NgayYCHT;
                                        existBC07.PXP_TinhTrang = item.PXP_TinhTrang;
                                        //Phân xưởng Mẫu
                                        existBC07.PXM_SoContKH = item.PXM_SoContKH;
                                        existBC07.PXM_SoContHT = item.PXM_SoContHT;
                                        existBC07.PXM_SoContConLai = item.PXM_SoContConLai;
                                        existBC07.PXM_NgayYCHT = item.PXM_NgayYCHT;
                                        existBC07.PXM_TinhTrang = item.PXM_TinhTrang;
                                        //Phân xưởng 4
                                        existBC07.PX4_SoContKH = item.PX4_SoContKH;
                                        existBC07.PX4_SoContHT = item.PX4_SoContHT;
                                        existBC07.PX4_SoContConLai = item.PX4_SoContConLai;
                                        existBC07.PX4_NgayYCHT = item.PX4_NgayYCHT;
                                        existBC07.PX4_TinhTrang = item.PX4_TinhTrang;
                                        //Phân xưởng 5
                                        existBC07.PX5_SoContKH = item.PX5_SoContKH;
                                        existBC07.PX5_SoContHT = item.PX5_SoContHT;
                                        existBC07.PX5_SoContConLai = item.PX5_SoContConLai;
                                        existBC07.PX5_NgayYCHT = item.PX5_NgayYCHT;
                                        existBC07.PX5_TinhTrang = item.PX5_TinhTrang;
                                        //Phân xưởng 3
                                        existBC07.PX3_SoContKH = item.PX3_SoContKH;
                                        existBC07.PX3_SoContHT = item.PX3_SoContHT;
                                        existBC07.PX3_SoContConLai = item.PX3_SoContConLai;
                                        existBC07.PX3_NgayYCHT = item.PX3_NgayYCHT;
                                        existBC07.PX3_TinhTrang = item.PX3_TinhTrang;
                                        //Phân xưởng 6
                                        existBC07.PX6_SoContKH = item.PX6_SoContKH;
                                        existBC07.PX6_SoContHT = item.PX6_SoContHT;
                                        existBC07.PX6_SoContConLai = item.PX6_SoContConLai;
                                        existBC07.PX6_NgayYCHT = item.PX6_NgayYCHT;
                                        existBC07.PX6_TinhTrang = item.PX6_TinhTrang;
                                        //Phân xưởng Cửa
                                        existBC07.PXC_SoContKH = item.PXC_SoContKH;
                                        existBC07.PXC_SoContHT = item.PXC_SoContHT;
                                        existBC07.PXC_SoContConLai = item.PXC_SoContConLai;
                                        existBC07.PXC_NgayYCHT = item.PXC_NgayYCHT;
                                        existBC07.PXC_TinhTrang = item.PXC_TinhTrang;
                                        //Phân xưởng 1
                                        existBC07.PX1_SoContKH = item.PX1_SoContKH;
                                        existBC07.PX1_SoContHT = item.PX1_SoContHT;
                                        existBC07.PX1_SoContConLai = item.PX1_SoContConLai;
                                        existBC07.PX1_NgayYCHT = item.PX1_NgayYCHT;
                                        existBC07.PX1_TinhTrang = item.PX1_TinhTrang;

                                        existBC07.FinishDateChange = item.FinishDateChange;
                                        existBC07.DeliveryDate = item.DeliveryDate;

                                        existBC07.LastEditTime = DateTime.Now;
                                        _newContext.Entry(existBC07).State = EntityState.Modified;
                                    }
                                    //2. Chưa có => insert
                                    else
                                    {
                                        BC07Model newBC07 = new BC07Model();
                                        newBC07.BC07Id = Guid.NewGuid();
                                        newBC07.STT = STT;
                                        newBC07.Plant = item.Plant;
                                        newBC07.AssignResponsibility = item.AssignResponsibility;
                                        newBC07.Customer = item.Customer;
                                        newBC07.PYCSXDT = item.PYCSXDT;
                                        newBC07.LSXDT = item.LSXDT;
                                        newBC07.DSX = item.DSX;
                                        newBC07.VBELN = item.VBELN;
                                        newBC07.ProductName = item.ProductName;
                                        newBC07.ProductCode = item.ProductCode;
                                        newBC07.LSXSAP = item.LSXSAP;
                                        newBC07.WBS = item.WBS;
                                        newBC07.TinhTrangMT = item.TinhTrangMT;
                                        newBC07.NguyenLieu = item.NguyenLieu;
                                        newBC07.HoanThien = item.HoanThien;
                                        newBC07.HinhAnh = item.HinhAnh;
                                        newBC07.DVT = item.DVT;
                                        newBC07.SLKH = item.SLKH;
                                        newBC07.SLDaNK = item.SLDaNK;
                                        newBC07.SLChuaNK = item.SLChuaNK;
                                        newBC07.SLDaXuat = item.SLDaXuat;
                                        newBC07.SLChuaXuat = item.SLChuaXuat;
                                        newBC07.SLTonKho = item.SLTonKho;
                                        newBC07.SoContKH = item.SoContKH;
                                        newBC07.SoContDaNK = item.SoContDaNK;
                                        newBC07.SoContChuaNK = item.SoContChuaNK;
                                        newBC07.SoContDaXuat = item.SoContDaXuat;
                                        newBC07.SoContChuaXuat = item.SoContChuaXuat;
                                        newBC07.SoContTonKho = item.SoContTonKho;
                                        newBC07.NgayKTDKTheoLSXSAP = item.NgayKTDKTheoLSXSAP;
                                        newBC07.NgayKTDKTheoDSX = item.NgayKTDKTheoDSX;
                                        newBC07.SLSP = item.SLSP;
                                        newBC07.CBM = item.CBM;
                                        newBC07.TongCBMChuaXuat = item.TongCBMChuaXuat;
                                        newBC07.PONoiBo = item.PONoiBo;
                                        newBC07.SoPOKhach = item.SoPOKhach;
                                        newBC07.MaSKU = item.MaSKU;
                                        newBC07.CangDen = item.CangDen;
                                        newBC07.LoaiCont = item.LoaiCont;
                                        newBC07.NgayTauChay = item.NgayTauChay;
                                        newBC07.StartSWDate = item.StartSWDate;
                                        newBC07.EndSWDate = item.EndSWDate;
                                        newBC07.AustinDirect = item.AustinDirect;
                                        newBC07.TuanXuatHang = item.TuanXuatHang;
                                        newBC07.ThangXuatHang = item.ThangXuatHang;
                                        newBC07.NgayYeuCauXuatHangSO = item.NgayYeuCauXuatHangSO;
                                        newBC07.CompletedPercentGo = item.CompletedPercentGo;
                                        newBC07.CompletedPercentVan = item.CompletedPercentVan;
                                        newBC07.CompletedPercentGiaCong = item.CompletedPercentGiaCong;
                                        newBC07.CompletedPercentVatTu = item.CompletedPercentVatTu;
                                        newBC07.CompletedPercentHoaChat = item.CompletedPercentHoaChat;
                                        newBC07.CompletedPercentBBPL = item.CompletedPercentBBPL;
                                        //Phân xưởng Phôi
                                        newBC07.PXP_SoContKH = item.PXP_SoContKH;
                                        newBC07.PXP_SoContHT = item.PXP_SoContHT;
                                        newBC07.PXP_SoContConLai = item.PXP_SoContConLai;
                                        newBC07.PXP_NgayYCHT = item.PXP_NgayYCHT;
                                        newBC07.PXP_TinhTrang = item.PXP_TinhTrang;
                                        //Phân xưởng Mẫu
                                        newBC07.PXM_SoContKH = item.PXM_SoContKH;
                                        newBC07.PXM_SoContHT = item.PXM_SoContHT;
                                        newBC07.PXM_SoContConLai = item.PXM_SoContConLai;
                                        newBC07.PXM_NgayYCHT = item.PXM_NgayYCHT;
                                        newBC07.PXM_TinhTrang = item.PXM_TinhTrang;
                                        //Phân xưởng 4
                                        newBC07.PX4_SoContKH = item.PX4_SoContKH;
                                        newBC07.PX4_SoContHT = item.PX4_SoContHT;
                                        newBC07.PX4_SoContConLai = item.PX4_SoContConLai;
                                        newBC07.PX4_NgayYCHT = item.PX4_NgayYCHT;
                                        newBC07.PX4_TinhTrang = item.PX4_TinhTrang;
                                        //Phân xưởng 5
                                        newBC07.PX5_SoContKH = item.PX5_SoContKH;
                                        newBC07.PX5_SoContHT = item.PX5_SoContHT;
                                        newBC07.PX5_SoContConLai = item.PX5_SoContConLai;
                                        newBC07.PX5_NgayYCHT = item.PX5_NgayYCHT;
                                        newBC07.PX5_TinhTrang = item.PX5_TinhTrang;
                                        //Phân xưởng 3
                                        newBC07.PX3_SoContKH = item.PX3_SoContKH;
                                        newBC07.PX3_SoContHT = item.PX3_SoContHT;
                                        newBC07.PX3_SoContConLai = item.PX3_SoContConLai;
                                        newBC07.PX3_NgayYCHT = item.PX3_NgayYCHT;
                                        newBC07.PX3_TinhTrang = item.PX3_TinhTrang;
                                        //Phân xưởng 6
                                        newBC07.PX6_SoContKH = item.PX6_SoContKH;
                                        newBC07.PX6_SoContHT = item.PX6_SoContHT;
                                        newBC07.PX6_SoContConLai = item.PX6_SoContConLai;
                                        newBC07.PX6_NgayYCHT = item.PX6_NgayYCHT;
                                        newBC07.PX6_TinhTrang = item.PX6_TinhTrang;
                                        //Phân xưởng Cửa
                                        newBC07.PXC_SoContKH = item.PXC_SoContKH;
                                        newBC07.PXC_SoContHT = item.PXC_SoContHT;
                                        newBC07.PXC_SoContConLai = item.PXC_SoContConLai;
                                        newBC07.PXC_NgayYCHT = item.PXC_NgayYCHT;
                                        newBC07.PXC_TinhTrang = item.PXC_TinhTrang;
                                        //Phân xưởng 1
                                        newBC07.PX1_SoContKH = item.PX1_SoContKH;
                                        newBC07.PX1_SoContHT = item.PX1_SoContHT;
                                        newBC07.PX1_SoContConLai = item.PX1_SoContConLai;
                                        newBC07.PX1_NgayYCHT = item.PX1_NgayYCHT;
                                        newBC07.PX1_TinhTrang = item.PX1_TinhTrang;
                                        newBC07.LSXSAPId = item.LSXSAPId;

                                        newBC07.FinishDateChange = item.FinishDateChange;
                                        newBC07.DeliveryDate = item.DeliveryDate;

                                        newBC07.CreateTime = DateTime.Now;
                                        _newContext.Entry(newBC07).State = EntityState.Added;
                                    }

                                    await _newContext.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = ex.Message;
                            if (ex.InnerException != null)
                            {
                                message = ex.InnerException.Message;
                                if (ex.InnerException.InnerException != null)
                                {
                                    message = ex.InnerException.InnerException.Message;
                                }
                            }
                            _loggerRepository.Logging("BC07 - LSXSAP: " + LSXSAP.LSXSAP + ", DSX: " + LSXSAP.DSX + " - " + message, "ERROR");
                            continue;
                        }
                    }
                }
                _newContext.Dispose();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message = ex.InnerException.InnerException.Message;
                    }
                }
                _loggerRepository.Logging("BC07:" + message, "ERROR");
            }

        }
    }
}
