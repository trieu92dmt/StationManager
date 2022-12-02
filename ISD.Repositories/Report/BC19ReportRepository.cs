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
    public class BC19ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC19ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo 19
        /// </summary>
        /// <returns>BC19ReportViewModel</returns>
        public List<BC19ReportViewModel> GetData(BC19ReportViewModel searchViewModel)
        {


            string sqlQuery = "[Report].[usp_BC19Report] @Plant, @DSX, @LSXSAP, @EndFromDate, @EndToDate, @WarningSearch";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Plant",
                    Value = searchViewModel.Plant ?? (object)DBNull.Value,
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DSX",
                    Value = searchViewModel.DSX ?? (object)DBNull.Value,
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndFromDate",
                    Value = searchViewModel.EndFromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndToDate",
                    Value = searchViewModel.EndToDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WarningSearch",
                    Value = searchViewModel.WarningSearch ?? (object)DBNull.Value,
                },
            };
            #endregion

            List<BC19ReportViewModel> result = _context.Database.SqlQuery<BC19ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        public async Task UpdateBC19()
        {
            var _loggerRepository = new LoggerRepository();
            //Lấy danh sách các đợt có lệnh sản xuất đang open
            IList<TaskViewModel> DSXGroupList = await (from p in _context.TaskModel
                                                       join t in _context.TaskModel on p.TaskId equals t.ParentTaskId
                                                        where p.ParentTaskId == null && (t.isDeleted == null || t.isDeleted == false)
                                                        group p by new { p.Summary } into g
                                                        orderby g.Max(p => p.TaskCode) descending
                                                        select new TaskViewModel
                                                        {
                                                            Summary = g.Key.Summary,
                                                            TaskCode = g.Max(p => p.TaskCode)
                                                        }).ToListAsync();
            IList<string> DSXList = DSXGroupList.Select(p => p.Summary).ToList();

            //Nếu có dữ liệu theo đợt thì mới chạy 
            if (DSXList != null)
            {
                foreach (var DSX in DSXList)
                {
                    try
                    {
                        EntityDataContext _newContext = new EntityDataContext();
                        //Gán đợt sản xuất để cập nhật theo đợt
                        BC19ReportViewModel searchViewModel = new BC19ReportViewModel();
                        searchViewModel.DSX = DSX;
                        //Chạy stored lấy danh sách kết quả theo đợt
                        string sqlQuery = "[Report].[usp_BC19Report_GetData] @Plant, @DSX, @LSXSAP, @EndFromDate, @EndToDate, @WarningSearch";
                        #region Parameters
                        List<SqlParameter> parameters = new List<SqlParameter>()
                        {
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "Plant",
                                Value = searchViewModel.Plant ?? (object)DBNull.Value,
                            },new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "DSX",
                                Value = searchViewModel.DSX ?? (object)DBNull.Value,
                            },new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "LSXSAP",
                                Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = "EndFromDate",
                                Value = searchViewModel.EndFromDate ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = "EndToDate",
                                Value = searchViewModel.EndToDate ?? (object)DBNull.Value,
                            },
                            new SqlParameter
                            {
                                SqlDbType = SqlDbType.NVarChar,
                                Direction = ParameterDirection.Input,
                                ParameterName = "WarningSearch",
                                Value = searchViewModel.WarningSearch ?? (object)DBNull.Value,
                            },
                        };
                        #endregion
                        _newContext.Database.CommandTimeout = 180;
                        List<BC19ReportViewModel> result = _newContext.Database.SqlQuery<BC19ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
                        //Lưu thông tin vào bảng BC19
                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                //1. Check lại dữ liệu nếu dữ liệu mới update: CDL != null && CDN != null (LSX SAP này đã được ghi nhận)
                                //2. Check theo LSXSAP dữ liệu cũ: CDL == null && CDN == null (LSX SAP này chưa được ghi nhận) => sau khi được ghi nhận thì xóa đi dữ liệu null
                                if (!string.IsNullOrEmpty(item.CDLCode) && !string.IsNullOrEmpty(item.CDLCode))
                                {
                                    var existsNullDataBC19Report = (from p in _newContext.BC19Model
                                                                    where p.Plant == item.Plant
                                                                    && p.DSX == item.DSX
                                                                    && p.LSXSAP == item.LSXSAP
                                                                    && p.ProductCode == item.ProductCode
                                                                    && p.CDLCode == null
                                                                    && p.CDNCode == null
                                                                    select p
                                                                   ).FirstOrDefault();
                                    if (existsNullDataBC19Report != null)
                                    {
                                        _newContext.Entry(existsNullDataBC19Report).State = EntityState.Deleted;
                                    }
                                }

                                //Check dữ liệu
                                //1. Nếu tồn tại => update
                                //2. Chưa có => insert
                                var existsBC19Report = (from p in _newContext.BC19Model
                                                        where p.Plant == item.Plant
                                                        && p.DSX == item.DSX
                                                        && p.LSXSAP == item.LSXSAP
                                                        && p.ProductCode == item.ProductCode
                                                        && p.CDLCode == item.CDLCode
                                                        && p.CDNCode == item.CDNCode
                                                        select p
                                                        ).FirstOrDefault();
                                //update
                                if (existsBC19Report != null)
                                {
                                    existsBC19Report.EndDateDSX = item.EndDateDSX;
                                    existsBC19Report.ProductName = item.ProductName;
                                    existsBC19Report.PlanQuantity = item.PlanQuantity;
                                    existsBC19Report.CompletedQuantity = item.CompletedQuantity;
                                    existsBC19Report.CDL = item.CDL;
                                    existsBC19Report.CDLIndex = item.CDLIndex;
                                    existsBC19Report.CDN = item.CDN;
                                    existsBC19Report.CDNIndex = item.CDNIndex;
                                    existsBC19Report.SLCTKH = item.SLCTKH;
                                    existsBC19Report.SLCTTT = item.SLCTTT;
                                    existsBC19Report.FromTime = item.FromTime;
                                    existsBC19Report.TransferWaitTime = item.TransferWaitTime;
                                    existsBC19Report.Warning = item.Warning;
                                    existsBC19Report.LastEditTime = DateTime.Now;
                                    _newContext.Entry(existsBC19Report).State = EntityState.Modified;
                                }
                                //insert
                                else
                                {
                                    BC19Model newBC19 = new BC19Model();
                                    newBC19.BC19Id = Guid.NewGuid();
                                    newBC19.Plant = item.Plant;
                                    newBC19.EndDateDSX = item.EndDateDSX;
                                    newBC19.DSX = item.DSX;
                                    newBC19.LSXSAP = item.LSXSAP;
                                    newBC19.ProductCode = item.ProductCode;
                                    newBC19.ProductName = item.ProductName;
                                    newBC19.PlanQuantity = item.PlanQuantity;
                                    newBC19.CompletedQuantity = item.CompletedQuantity;
                                    newBC19.CDL = item.CDL;
                                    newBC19.CDLCode = item.CDLCode;
                                    newBC19.CDLIndex = item.CDLIndex;
                                    newBC19.CDN = item.CDN;
                                    newBC19.CDNCode = item.CDNCode;
                                    newBC19.CDNIndex = item.CDNIndex;
                                    newBC19.SLCTKH = item.SLCTKH;
                                    newBC19.SLCTTT = item.SLCTTT;
                                    newBC19.FromTime = item.FromTime;
                                    newBC19.TransferWaitTime = item.TransferWaitTime;
                                    newBC19.Warning = item.Warning;

                                    newBC19.CreateTime = DateTime.Now;
                                    _newContext.Entry(newBC19).State = EntityState.Added;
                                }
                                _newContext.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        if (ex.InnerException != null)
                        {
                            error = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                error = ex.InnerException.InnerException.Message;
                            }
                        }
                        _loggerRepository.Logging("BC19 - DSX: " + DSX + " " + error, "ERROR");
                        continue;
                    }
                }

                //tìm trong BC19Report nếu không tồn tại trong danh sách đợt đã lấy ra (dữ liệu đợt đã được update mới) thì xóa
                var notExistDSX = _context.BC19Model.Where(p => !DSXList.Contains(p.DSX)).ToList();
                if (notExistDSX != null && notExistDSX.Count > 0)
                {
                    _context.BC19Model.RemoveRange(notExistDSX);
                    _context.SaveChanges();
                }
            }
        }
    }
}
