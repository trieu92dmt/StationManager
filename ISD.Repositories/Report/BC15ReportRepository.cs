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
    public class BC15ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC15ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo đồng bộ sản phẩm
        /// </summary>
        /// <returns>BC15ReportViewModel</returns>
        public List<BC15ReportViewModel> GetData(BC15ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC15Report] @Plant, @LSXDT, @DSX, @LSXSAP, @WorkShopId, @WorkCenterCode, @PhysicsWorkShopCode, @CompletedFromDate, @CompletedToDate";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
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
                    ParameterName = "LSXDT",
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
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkShopId",
                    Value = searchViewModel.WorkShopId ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkCenterCode",
                    Value = searchViewModel.WorkCenterCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PhysicsWorkShopCode",
                    Value = searchViewModel.PhysicsWorkShopCode ?? (object)DBNull.Value,
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
            };
            #endregion

            List<BC15ReportViewModel> result = _context.Database.SqlQuery<BC15ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo 15
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC15()
        {
            var _loggerRepository = new LoggerRepository();
            EntityDataContext _newContext = new EntityDataContext();
            //Lấy danh sách LSXSAP
            var LSXSAPWorkFLowId = _newContext.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).Select(p => p.WorkFlowId).FirstOrDefault();
            IList<BC05ReportViewModel> LSXSAPList = await (from p in _newContext.TaskModel
                                                           join c in _newContext.CompanyModel on p.CompanyId equals c.CompanyId
                                                           where p.WorkFlowId == LSXSAPWorkFLowId
                                                           && (p.isDeleted == null || p.isDeleted == false)
                                                           group p by new { p.TaskId, p.Summary, c.CompanyCode } into g
                                                           orderby g.Max(p => p.CreateTime) descending
                                                           select new BC05ReportViewModel
                                                           {
                                                               LSXSAP = g.Key.Summary,
                                                               Plant = g.Key.CompanyCode
                                                           }).ToListAsync();
            if (LSXSAPList != null)
            {
                foreach (var LSXSAP in LSXSAPList)
                {
                    try
                    {
                        //Gán thông số chạy stored để cập nhật vào DB
                        BC15ReportViewModel searchViewModel = new BC15ReportViewModel();
                        searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                        searchViewModel.Plant = LSXSAP.Plant;

                        string sqlQuery = "[Report].[usp_BC15Report_GetData] @Plant, @LSXDT, @DSX, @LSXSAP, @WorkShopId, @WorkCenterCode, @PhysicsWorkShopCode, @CompletedFromDate, @CompletedToDate";

                        #region Parameters
                        List<SqlParameter> parameters = new List<SqlParameter>()
                        {
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
                                ParameterName = "LSXDT",
                                Value = (object)DBNull.Value,
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
                                SqlDbType = SqlDbType.UniqueIdentifier,
                                Direction = ParameterDirection.Input,
                                ParameterName = "WorkShopId",
                                Value = (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "WorkCenterCode",
                                Value = (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "PhysicsWorkShopCode",
                                Value = (object)DBNull.Value,
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
                        };
                        #endregion
                        _newContext.Database.CommandTimeout = 180;
                        List<BC15ReportViewModel> result = await _newContext.Database.SqlQuery<BC15ReportViewModel>(sqlQuery, parameters.ToArray()).ToListAsync();

                        //Lưu thông tin vào bảng BC15
                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                //Check dữ liệu
                                var existBC15 = await _newContext.BC15Model.Where(p => p.LSXSAPId == item.LSXSAPId && p.ProductAttributes == item.ProductAttributes).FirstOrDefaultAsync();
                                //1. Nếu tồn tại => update
                                if (existBC15 != null)
                                {
                                    existBC15.Plant = item.Plant;
                                    existBC15.WorkShopId = item.WorkShopId;
                                    existBC15.WorkShopName = item.WorkShopName;
                                    existBC15.NgayHTDC = item.NgayHTDC;
                                    existBC15.LSXDT = item.LSXDT;
                                    existBC15.LSX = item.LSX;
                                    existBC15.ProductName = item.ProductName;
                                    existBC15.ProductAttributesName = item.ProductAttributesName;
                                    existBC15.MaterialCode = item.MaterialCode;
                                    existBC15.MaterialName = item.MaterialName;
                                    existBC15.DVT = item.DVT;
                                    existBC15.SLCTKH = item.SLCTKH;
                                    existBC15.P1 = item.P1;
                                    existBC15.P2 = item.P2;
                                    existBC15.P3 = item.P3;
                                    existBC15.TheTich = item.TheTich;
                                    existBC15.SLKH = item.SLKH;
                                    existBC15.SLCTKHTheoSP = item.SLCTKHTheoSP;
                                    //Phân xưởng phôi
                                    existBC15.PXP_SLHT_SC = item.PXP_SLHT_SC;
                                    existBC15.PXP_SLCL_SC = item.PXP_SLCL_SC;
                                    existBC15.PXP_SoPhutNCTT_SC = item.PXP_SoPhutNCTT_SC;
                                    existBC15.PXP_SoContKH_SC = item.PXP_SoContKH_SC;
                                    existBC15.PXP_SoContConLai_SC = item.PXP_SoContConLai_SC;
                                    existBC15.PXP_NgayYCHT_SC = item.PXP_NgayYCHT_SC;
                                    existBC15.PXP_TinhTrang_SC = item.PXP_TinhTrang_SC;
                                    //Phân xưởng 3
                                    existBC15.PX3_SLHT_TC = item.PX3_SLHT_TC;
                                    existBC15.PX3_SLCL_TC = item.PX3_SLCL_TC;
                                    existBC15.PX3_SLHT_LR = item.PX3_SLHT_LR;
                                    existBC15.PX3_SLCL_LR = item.PX3_SLCL_LR;
                                    existBC15.PX3_SoPhutNCTT_LR = item.PX3_SoPhutNCTT_LR;
                                    existBC15.PX3_SoContKH_LR = item.PX3_SoContKH_LR;
                                    existBC15.PX3_SoContConLai_LR = item.PX3_SoContConLai_LR;
                                    existBC15.PX3_NgayYCHT_LR = item.PX3_NgayYCHT_LR;
                                    existBC15.PX3_TinhTrang_LR = item.PX3_TinhTrang_LR;
                                    //Phân xưởng 6
                                    existBC15.PX6_SLHT_HT = item.PX6_SLHT_HT;
                                    existBC15.PX6_SLCL_HT = item.PX6_SLCL_HT;
                                    existBC15.PX6_SoPhutNCTT_HT = item.PX6_SoPhutNCTT_HT;
                                    existBC15.PX6_SoContKH_HT = item.PX6_SoContKH_HT;
                                    existBC15.PX6_SoContConLai_HT = item.PX6_SoContConLai_HT;
                                    existBC15.PX6_NgayYCHT_HT = item.PX6_NgayYCHT_HT;
                                    existBC15.PX6_TinhTrang_HT = item.PX6_TinhTrang_HT;
                                    //Phân xưởng 4
                                    existBC15.PX4_SLHT_TC = item.PX4_SLHT_TC;
                                    existBC15.PX4_SLCL_TC = item.PX4_SLCL_TC;
                                    existBC15.PX4_SLHT_LR = item.PX4_SLHT_LR;
                                    existBC15.PX4_SLCL_LR = item.PX4_SLCL_LR;
                                    existBC15.PX4_SoPhutNCTT_LR = item.PX4_SoPhutNCTT_LR;
                                    existBC15.PX4_SoContKH_LR = item.PX4_SoContKH_LR;
                                    existBC15.PX4_SoContConLai_LR = item.PX4_SoContConLai_LR;
                                    existBC15.PX4_NgayYCHT_LR = item.PX4_NgayYCHT_LR;
                                    existBC15.PX4_TinhTrang_LR = item.PX4_TinhTrang_LR;
                                    //Phân xưởng 5
                                    existBC15.PX5_SLHT_HT = item.PX5_SLHT_HT;
                                    existBC15.PX5_SLCL_HT = item.PX5_SLCL_HT;
                                    existBC15.PX5_SoPhutNCTT_HT = item.PX5_SoPhutNCTT_HT;
                                    existBC15.PX5_SoContKH_HT = item.PX5_SoContKH_HT;
                                    existBC15.PX5_SoContConLai_HT = item.PX5_SoContConLai_HT;
                                    existBC15.PX5_NgayYCHT_HT = item.PX5_NgayYCHT_HT;
                                    existBC15.PX5_TinhTrang_HT = item.PX5_TinhTrang_HT;
                                    //Phân xưởng cửa
                                    existBC15.PXCua_SLHT_TC = item.PXCua_SLHT_TC;
                                    existBC15.PXCua_SLCL_TC = item.PXCua_SLCL_TC;
                                    existBC15.PXCua_SLHT_LR = item.PXCua_SLHT_LR;
                                    existBC15.PXCua_SLCL_LR = item.PXCua_SLCL_LR;
                                    existBC15.PXCua_SLHT_HT = item.PXCua_SLHT_HT;
                                    existBC15.PXCua_SLCL_HT = item.PXCua_SLCL_HT;
                                    existBC15.PXCua_SoPhutNCTT_HT = item.PXCua_SoPhutNCTT_HT;
                                    existBC15.PXCua_SoContKH_HT = item.PXCua_SoContKH_HT;
                                    existBC15.PXCua_SoContConLai_HT = item.PXCua_SoContConLai_HT;
                                    existBC15.PXCua_NgayYCHT_HT = item.PXCua_NgayYCHT_HT;
                                    existBC15.PXCua_TinhTrang_HT = item.PXCua_TinhTrang_HT;
                                    //Phân xưởng mẫu
                                    existBC15.PXMau_SLHT_SC = item.PXMau_SLHT_SC;
                                    existBC15.PXMau_SLCL_SC = item.PXMau_SLCL_SC;
                                    existBC15.PXMau_SLHT_TC = item.PXMau_SLHT_TC;
                                    existBC15.PXMau_SLCL_TC = item.PXMau_SLCL_TC;
                                    existBC15.PXMau_SLHT_LR = item.PXMau_SLHT_LR;
                                    existBC15.PXMau_SLCL_LR = item.PXMau_SLCL_LR;
                                    existBC15.PXMau_SLHT_HT = item.PXMau_SLHT_HT;
                                    existBC15.PXMau_SLCL_HT = item.PXMau_SLCL_HT;
                                    existBC15.PXMau_SoPhutNCTT_HT = item.PXMau_SoPhutNCTT_HT;
                                    existBC15.PXMau_SoContKH_HT = item.PXMau_SoContKH_HT;
                                    existBC15.PXMau_SoContConLai_HT = item.PXMau_SoContConLai_HT;
                                    existBC15.PXMau_NgayYCHT_HT = item.PXMau_NgayYCHT_HT;
                                    existBC15.PXMau_TinhTrang_HT = item.PXMau_TinhTrang_HT;

                                    existBC15.LastEditTime = DateTime.Now;
                                    _newContext.Entry(existBC15).State = EntityState.Modified;
                                }
                                //2. Chưa có => insert
                                else
                                {
                                    BC15Model newBC15 = new BC15Model();
                                    newBC15.BC15Id = Guid.NewGuid();
                                    newBC15.LSXSAPId = item.LSXSAPId;
                                    newBC15.LSXSAP = item.LSXSAP;
                                    newBC15.ProductCode = item.ProductCode;
                                    newBC15.ProductName = item.ProductName;
                                    newBC15.ProductAttributes = item.ProductAttributes;
                                    newBC15.DSX = item.DSX;
                                    newBC15.Plant = item.Plant;
                                    newBC15.WorkShopId = item.WorkShopId;
                                    newBC15.WorkShopName = item.WorkShopName;
                                    newBC15.NgayHTDC = item.NgayHTDC;
                                    newBC15.LSXDT = item.LSXDT;
                                    newBC15.LSX = item.LSX;
                                    newBC15.ProductAttributesName = item.ProductAttributesName;
                                    newBC15.MaterialCode = item.MaterialCode;
                                    newBC15.MaterialName = item.MaterialName;
                                    newBC15.DVT = item.DVT;
                                    newBC15.SLCTKH = item.SLCTKH;
                                    newBC15.P1 = item.P1;
                                    newBC15.P2 = item.P2;
                                    newBC15.P3 = item.P3;
                                    newBC15.TheTich = item.TheTich;
                                    newBC15.SLKH = item.SLKH;
                                    newBC15.SLCTKHTheoSP = item.SLCTKHTheoSP;
                                    //Phân xưởng phôi
                                    newBC15.PXP_SLHT_SC = item.PXP_SLHT_SC;
                                    newBC15.PXP_SLCL_SC = item.PXP_SLCL_SC;
                                    newBC15.PXP_SoPhutNCTT_SC = item.PXP_SoPhutNCTT_SC;
                                    newBC15.PXP_SoContKH_SC = item.PXP_SoContKH_SC;
                                    newBC15.PXP_SoContConLai_SC = item.PXP_SoContConLai_SC;
                                    newBC15.PXP_NgayYCHT_SC = item.PXP_NgayYCHT_SC;
                                    newBC15.PXP_TinhTrang_SC = item.PXP_TinhTrang_SC;
                                    //Phân xưởng 3
                                    newBC15.PX3_SLHT_TC = item.PX3_SLHT_TC;
                                    newBC15.PX3_SLCL_TC = item.PX3_SLCL_TC;
                                    newBC15.PX3_SLHT_LR = item.PX3_SLHT_LR;
                                    newBC15.PX3_SLCL_LR = item.PX3_SLCL_LR;
                                    newBC15.PX3_SoPhutNCTT_LR = item.PX3_SoPhutNCTT_LR;
                                    newBC15.PX3_SoContKH_LR = item.PX3_SoContKH_LR;
                                    newBC15.PX3_SoContConLai_LR = item.PX3_SoContConLai_LR;
                                    newBC15.PX3_NgayYCHT_LR = item.PX3_NgayYCHT_LR;
                                    newBC15.PX3_TinhTrang_LR = item.PX3_TinhTrang_LR;
                                    //Phân xưởng 6
                                    newBC15.PX6_SLHT_HT = item.PX6_SLHT_HT;
                                    newBC15.PX6_SLCL_HT = item.PX6_SLCL_HT;
                                    newBC15.PX6_SoPhutNCTT_HT = item.PX6_SoPhutNCTT_HT;
                                    newBC15.PX6_SoContKH_HT = item.PX6_SoContKH_HT;
                                    newBC15.PX6_SoContConLai_HT = item.PX6_SoContConLai_HT;
                                    newBC15.PX6_NgayYCHT_HT = item.PX6_NgayYCHT_HT;
                                    newBC15.PX6_TinhTrang_HT = item.PX6_TinhTrang_HT;
                                    //Phân xưởng 4
                                    newBC15.PX4_SLHT_TC = item.PX4_SLHT_TC;
                                    newBC15.PX4_SLCL_TC = item.PX4_SLCL_TC;
                                    newBC15.PX4_SLHT_LR = item.PX4_SLHT_LR;
                                    newBC15.PX4_SLCL_LR = item.PX4_SLCL_LR;
                                    newBC15.PX4_SoPhutNCTT_LR = item.PX4_SoPhutNCTT_LR;
                                    newBC15.PX4_SoContKH_LR = item.PX4_SoContKH_LR;
                                    newBC15.PX4_SoContConLai_LR = item.PX4_SoContConLai_LR;
                                    newBC15.PX4_NgayYCHT_LR = item.PX4_NgayYCHT_LR;
                                    newBC15.PX4_TinhTrang_LR = item.PX4_TinhTrang_LR;
                                    //Phân xưởng 5
                                    newBC15.PX5_SLHT_HT = item.PX5_SLHT_HT;
                                    newBC15.PX5_SLCL_HT = item.PX5_SLCL_HT;
                                    newBC15.PX5_SoPhutNCTT_HT = item.PX5_SoPhutNCTT_HT;
                                    newBC15.PX5_SoContKH_HT = item.PX5_SoContKH_HT;
                                    newBC15.PX5_SoContConLai_HT = item.PX5_SoContConLai_HT;
                                    newBC15.PX5_NgayYCHT_HT = item.PX5_NgayYCHT_HT;
                                    newBC15.PX5_TinhTrang_HT = item.PX5_TinhTrang_HT;
                                    //Phân xưởng cửa
                                    newBC15.PXCua_SLHT_TC = item.PXCua_SLHT_TC;
                                    newBC15.PXCua_SLCL_TC = item.PXCua_SLCL_TC;
                                    newBC15.PXCua_SLHT_LR = item.PXCua_SLHT_LR;
                                    newBC15.PXCua_SLCL_LR = item.PXCua_SLCL_LR;
                                    newBC15.PXCua_SLHT_HT = item.PXCua_SLHT_HT;
                                    newBC15.PXCua_SLCL_HT = item.PXCua_SLCL_HT;
                                    newBC15.PXCua_SoPhutNCTT_HT = item.PXCua_SoPhutNCTT_HT;
                                    newBC15.PXCua_SoContKH_HT = item.PXCua_SoContKH_HT;
                                    newBC15.PXCua_SoContConLai_HT = item.PXCua_SoContConLai_HT;
                                    newBC15.PXCua_NgayYCHT_HT = item.PXCua_NgayYCHT_HT;
                                    newBC15.PXCua_TinhTrang_HT = item.PXCua_TinhTrang_HT;
                                    //Phân xưởng mẫu
                                    newBC15.PXMau_SLHT_SC = item.PXMau_SLHT_SC;
                                    newBC15.PXMau_SLCL_SC = item.PXMau_SLCL_SC;
                                    newBC15.PXMau_SLHT_TC = item.PXMau_SLHT_TC;
                                    newBC15.PXMau_SLCL_TC = item.PXMau_SLCL_TC;
                                    newBC15.PXMau_SLHT_LR = item.PXMau_SLHT_LR;
                                    newBC15.PXMau_SLCL_LR = item.PXMau_SLCL_LR;
                                    newBC15.PXMau_SLHT_HT = item.PXMau_SLHT_HT;
                                    newBC15.PXMau_SLCL_HT = item.PXMau_SLCL_HT;
                                    newBC15.PXMau_SoPhutNCTT_HT = item.PXMau_SoPhutNCTT_HT;
                                    newBC15.PXMau_SoContKH_HT = item.PXMau_SoContKH_HT;
                                    newBC15.PXMau_SoContConLai_HT = item.PXMau_SoContConLai_HT;
                                    newBC15.PXMau_NgayYCHT_HT = item.PXMau_NgayYCHT_HT;
                                    newBC15.PXMau_TinhTrang_HT = item.PXMau_TinhTrang_HT;

                                    newBC15.CreateTime = DateTime.Now;
                                    _newContext.Entry(newBC15).State = EntityState.Added;
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
                        _loggerRepository.Logging("BC15 - LSXSAPId: " + LSXSAP.LSXSAPId + ", LSXSAP: " + LSXSAP.LSXSAP + " " + message, "ERROR");
                        continue;
                    }
                    
                }
            }
            _newContext.Dispose();
        }
    }
}
