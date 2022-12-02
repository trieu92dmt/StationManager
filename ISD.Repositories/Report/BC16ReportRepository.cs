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
    public class BC16ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC16ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo đồng bộ sản phẩm
        /// </summary>
        /// <returns>BC16ReportViewModel</returns>
        public List<BC16ReportViewModel> GetData(BC16ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC16Report] @SaleOrgCode, @LSXDT, @DSX, @LSXSAP, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate, @DSXId, @LSXSAPId";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOrgCode",
                    Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
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
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkCenterCode",
                    Value = searchViewModel.WorkCenterCode ?? (object)DBNull.Value,
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
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DSXId",
                    Value = searchViewModel.DSXId ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAPId",
                    Value = searchViewModel.LSXSAPId ?? (object)DBNull.Value,
                },
            };
            #endregion

            List<BC16ReportViewModel> result = _context.Database.SqlQuery<BC16ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo 16
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC16()
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
                                                           //orderby g.Key.CompanyCode, g.Max(p => p.CreateTime ) descending
                                                           orderby g.Key.CompanyCode, g.Max(p => p.CreateTime)
                                                           select new BC05ReportViewModel
                                                           {
                                                               LSXSAP = g.Key.Summary,
                                                               Plant = g.Key.CompanyCode
                                                           }).ToListAsync();
            //lọc những LSX SAP chưa có trong BC16 để lưu trước
            //nếu đã tồn tại tất cả rồi thì mới chạy lại all list
            var LSXSAPExistLst = await _context.BC16Model.Select(p => p.LSXSAP).Distinct().ToListAsync();
            if (LSXSAPExistLst != null && LSXSAPExistLst.Count > 0)
            {
                var LSXSAPNewLst = LSXSAPList.Where(p => !LSXSAPExistLst.Contains(p.LSXSAP)).ToList();
                if (LSXSAPNewLst != null && LSXSAPNewLst.Count > 0)
                {
                    LSXSAPList = LSXSAPNewLst;
                }
            }
            if (LSXSAPList != null)
            {
                foreach (var LSXSAP in LSXSAPList)
                {
                    try
                    {
                        //Gán thông số chạy stored để cập nhật vào DB
                        BC16ReportViewModel searchViewModel = new BC16ReportViewModel();
                        searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                        searchViewModel.SaleOrgCode = LSXSAP.Plant;

                        string sqlQuery = "[Report].[usp_BC16Report_GetData] @SaleOrgCode,  @LSXDT, @DSX, @LSXSAP, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate";
                        #region Parameters
                        List<SqlParameter> parameters = new List<SqlParameter>()
                        {
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "SaleOrgCode",
                                Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
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
                                Value = searchViewModel.WorkShopId ?? (object)DBNull.Value,
                            },new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "WorkCenterCode",
                                Value = searchViewModel.WorkCenterCode ?? (object)DBNull.Value,
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
                        List<BC16ReportViewModel> result = await _newContext.Database.SqlQuery<BC16ReportViewModel>(sqlQuery, parameters.ToArray()).ToListAsync();

                        //Lưu thông tin vào bảng BC16
                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                //Check dữ liệu
                                var existBC16 = await _newContext.BC16Model.Where(p => p.LSXSAPId == item.LSXSAPId && p.ProductAttributes == item.ProductAttributes && p.StepCode == item.StepCode).FirstOrDefaultAsync();
                                //1. Nếu tồn tại => update
                                if (existBC16 != null)
                                {
                                    existBC16.Plant = item.Plant;
                                    //existBC16.WorkShopId = item.WorkShopId;
                                    //existBC16.WorkShopName = item.WorkShopName;
                                    existBC16.NgayHTDC = item.NgayHTDC;
                                    existBC16.LSXDT = item.LSXDT;
                                    existBC16.LSX = item.LSX;
                                    existBC16.ProductName = item.ProductName;
                                    existBC16.ProductAttributesName = item.ProductAttributesName;
                                    existBC16.MaterialCode = item.MaterialCode;
                                    existBC16.MaterialName = item.MaterialName;
                                    existBC16.DVT = item.DVT;
                                    existBC16.SLCTKH = item.SLCTKH;
                                    existBC16.P1 = item.P1;
                                    existBC16.P2 = item.P2;
                                    existBC16.P3 = item.P3;
                                    existBC16.TheTich = item.TheTich;
                                    existBC16.SLKH = item.SLKH;
                                    existBC16.SLCTKHTheoSP = item.SLCTKHTheoSP;
                                    existBC16.StepIndex = item.StepIndex;
                                    existBC16.Step_SLKH = item.Step_SLKH;
                                    existBC16.Step_SLHT = item.Step_SLHT;
                                    existBC16.Step_SLCL = item.Step_SLCL;
                                    existBC16.Step_NCTT = item.Step_NCTT;

                                    existBC16.LastEditTime = DateTime.Now;
                                    _newContext.Entry(existBC16).State = EntityState.Modified;
                                }
                                //2. Chưa có => insert
                                else
                                {
                                    BC16Model newBC16 = new BC16Model();
                                    newBC16.BC16Id = Guid.NewGuid();
                                    newBC16.LSXSAPId = item.LSXSAPId;
                                    newBC16.LSXSAP = item.LSXSAP;
                                    newBC16.ProductCode = item.ProductCode;
                                    newBC16.ProductName = item.ProductName;
                                    newBC16.ProductAttributes = item.ProductAttributes;
                                    newBC16.DSX = item.DSX;
                                    newBC16.Plant = item.Plant;
                                    //newBC16.WorkShopId = item.WorkShopId;
                                    //newBC16.WorkShopName = item.WorkShopName;
                                    newBC16.NgayHTDC = item.NgayHTDC;
                                    newBC16.LSXDT = item.LSXDT;
                                    newBC16.LSX = item.LSX;
                                    newBC16.ProductAttributesName = item.ProductAttributesName;
                                    newBC16.MaterialCode = item.MaterialCode;
                                    newBC16.MaterialName = item.MaterialName;
                                    newBC16.DVT = item.DVT;
                                    newBC16.SLCTKH = item.SLCTKH;
                                    newBC16.P1 = item.P1;
                                    newBC16.P2 = item.P2;
                                    newBC16.P3 = item.P3;
                                    newBC16.TheTich = item.TheTich;
                                    newBC16.SLKH = item.SLKH;
                                    newBC16.SLCTKHTheoSP = item.SLCTKHTheoSP;
                                    newBC16.StepCode = item.StepCode;
                                    newBC16.StepIndex = item.StepIndex;
                                    newBC16.Step_SLKH = item.Step_SLKH;
                                    newBC16.Step_SLHT = item.Step_SLHT;
                                    newBC16.Step_SLCL = item.Step_SLCL;
                                    newBC16.Step_NCTT = item.Step_NCTT;

                                    newBC16.CreateTime = DateTime.Now;
                                    _newContext.Entry(newBC16).State = EntityState.Added;
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
                        _loggerRepository.Logging("BC16 - LSXSAPId: " + LSXSAP.LSXSAPId + ", LSXSAP: " + LSXSAP.LSXSAP + " " + message, "ERROR");
                        continue;
                    }
                }
            }
            _newContext.Dispose();
        }
    }
}
