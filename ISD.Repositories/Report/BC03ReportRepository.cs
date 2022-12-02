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
    public class BC03ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC03ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo cân đối năng lực sản xuất - chiều dọc
        /// </summary>
        /// <returns>BC03ReportViewModel</returns>
        public List<BC03ReportViewModel> GetData(BC03ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC03Report] @LSX, @DSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant, @WorkShop, @DeliveryFromDate, @DeliveryToDate, @isOpen";

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
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "isOpen",
                    Value = searchViewModel.isOpen ?? (object)DBNull.Value,
                },
            };
            #endregion

            List<BC03ReportViewModel> result = _context.Database.SqlQuery<BC03ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo 03
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC03()
        {
            var _loggerRepository = new LoggerRepository();
            try
            {
                EntityDataContext _newContext = new EntityDataContext();
                //Lấy danh sách đợt trong BC01
                IList<BC03ReportViewModel> DSXList = await (from p in _newContext.BC01Model
                                                         group p by new { p.DSX } into g
                                                         select new BC03ReportViewModel
                                                         {
                                                             DSX = g.Key.DSX,
                                                         }).ToListAsync();
                if (DSXList != null)
                {

                    foreach (var DSX in DSXList)
                    {
                        try
                        {

                            //Gán thông số chạy stored để cập nhật vào DB
                            BC03ReportViewModel searchViewModel = new BC03ReportViewModel();
                            searchViewModel.DSX = DSX.DSX;

                            string sqlQuery = "[Report].[usp_BC03Report_UpdateData] @LSX, @DSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant, @WorkShop, @DeliveryFromDate, @DeliveryToDate";
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
                            };
                            #endregion
                            _newContext.Database.CommandTimeout = 1200;
                            List<BC03ReportViewModel> result = await _newContext.Database.SqlQuery<BC03ReportViewModel>(sqlQuery, parameters.ToArray()).ToListAsync();

                            //Lưu thông tin vào bảng BC03
                            if (result != null && result.Count > 0)
                            {
                                foreach (var item in result)
                                {
                                    //Nếu có trả về lỗi thì lưu vào bảng log
                                    if (!string.IsNullOrEmpty(item.ErrorNumber))
                                    {
                                        _loggerRepository.Logging("BC03 - DSX: " + DSX.DSX + " - " + item.ErrorNumber + " " + item.ErrorMessage, "ERROR");
                                    }
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
                            _loggerRepository.Logging("BC03 - DSX: " + DSX.DSX + " - " + message, "ERROR");
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
                _loggerRepository.Logging("BC03:" + message, "ERROR");
            }

        }
    }
}
