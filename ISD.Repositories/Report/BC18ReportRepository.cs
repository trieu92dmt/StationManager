using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Infrastructure.Extensions;
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
    public class BC18ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC18ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo 18.2: Chi tiết sản lượng đồng bộ tại phân xưởng
        /// </summary>
        /// <returns>ProfileReportResultViewModel</returns>
        public List<BC18_2ReportViewModel> GetData(BC18_2ReportViewModel searchViewModel)
        {


            string sqlQuery = "[Report].[usp_BC18_2Report] @SaleOrgCode, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOrgCode",
                    Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
                },new SqlParameter
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

            List<BC18_2ReportViewModel> result = _context.Database.SqlQuery<BC18_2ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        public async Task UpdateBC18()
        {
            try
            {
                //Lấy ngày cuối cùng chạy
                var Log_CreatedDate = await (from p in _context.Log_BC18Model
                                             orderby p.Running_Time descending
                                             select new
                                             {
                                                 p.Running_Time,
                                             }).FirstOrDefaultAsync();
                //Nếu ngày cuối cùng chạy = ngày hiện tại:
                if (Log_CreatedDate.Running_Time == DateTime.Now.Date)
                {
                    //Lấy từ ngày khóa sổ đến ngày hiện tại mỗi lần chạy
                    var dateClosed = _context.DateClosedModel.FirstOrDefault();
                    //Tìm trong bảng log từ ngày khóa sổ => xóa log từ ngày khóa sổ trở đi => chạy lại
                    if (dateClosed != null)
                    {
                        var exitsLog = (from p in _context.Log_BC18Model
                                              where dateClosed.DateClosed <= p.Running_Time
                                              orderby p.Running_Time descending
                                              select p).ToList();
                        if (exitsLog != null)
                        {
                            _context.Log_BC18Model.RemoveRange(exitsLog);
                            _context.SaveChanges();
                        }
                    }
                }
               
                //Check value
                //1. Nếu đã có value => lấy ngày mới nhất đã lưu
                //2. Nếu chưa có value => lấy ngày hiện tại
                DateTime Running_Time = DateTime.Now.Date;
                if (Log_CreatedDate != null)
                {
                    Running_Time = Log_CreatedDate.Running_Time.Value;
                }
                //Gán ngày hoàn thành từ => đến: ngày đã lưu khi chạy service
                BC18_2ReportViewModel searchViewModel = new BC18_2ReportViewModel();
                searchViewModel.CompletedFromDate = Running_Time;
                searchViewModel.CompletedToDate = Running_Time;
                //Chạy stored lấy danh sách kết quả theo ngày
                string sqlQuery = "[Report].[usp_BC18Report_GetData] @SaleOrgCode, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate";
                #region Parameters
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "SaleOrgCode",
                        Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
                    },new SqlParameter
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
                _context.Database.CommandTimeout = 180;
                List<BC18_2ReportViewModel> result = _context.Database.SqlQuery<BC18_2ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
                //Lưu thông tin vào bảng BC18
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        //Tìm những data không tồn tại trong result và xóa đi do dữ liệu routing được cập nhật từ SAP

                        var existsLst = (from p in _context.BC18Model
                                         where p.SaleOrg == item.SaleOrgCode
                                         && p.WorkShopId == item.WorkShopId
                                         && p.WorkCenterCode == item.WorkCenterCode
                                         && p.LSXSAP == item.LSXSAP
                                         && p.DSX == item.DSX
                                         //&& p.LSXDT == item.LSXDT
                                         && p.ProductId == item.ProductId
                                         && p.CompletedDateTime == item.CompletedDateTime
                                         select p
                                        ).ToList();
                        if (existsLst != null && existsLst.Count > 0)
                        {
                            var removeLst = existsLst.Where(l => result.Any(p =>
                                                             p.SaleOrgCode == l.SaleOrg
                                                             && p.WorkShopId == l.WorkShopId
                                                             && p.WorkCenterCode == l.WorkCenterCode
                                                             && p.LSXSAP == l.LSXSAP
                                                             && p.DSX == l.DSX
                                                             //&& p.LSXDT == l.LSXDT
                                                             && p.ProductId == l.ProductId
                                                             && p.CompletedDateTime == l.CompletedDateTime
                                                             && p.ProductAttributes != l.ProductAttributes 
                                                             && p.IDNRK_MES != l.IDNRK_MES
                                                            )).ToList();

                            if (removeLst != null && removeLst.Count > 0)
                            {
                                _context.BC18Model.RemoveRange(removeLst);
                                _context.SaveChanges();
                            }
                        }
                       
                        //Check dữ liệu
                        //1. Nếu tồn tại => update
                        //2. Chưa có => insert
                        var existsBC18Report = (from p in _context.BC18Model
                                                where p.SaleOrg == item.SaleOrgCode
                                                && p.WorkShopId == item.WorkShopId
                                                && p.WorkCenterCode == item.WorkCenterCode
                                                && p.LSXSAP == item.LSXSAP
                                                && p.DSX == item.DSX
                                                //&& p.LSXDT == item.LSXDT
                                                && p.ProductId == item.ProductId
                                                && p.CompletedDateTime == item.CompletedDateTime
                                                && p.ProductAttributes == item.ProductAttributes
                                                && p.IDNRK_MES == item.IDNRK_MES
                                                select p
                                                ).FirstOrDefault();
                        //update
                        if (existsBC18Report != null)
                        {
                            existsBC18Report.LSXDT = item.LSXDT;
                            existsBC18Report.SLKH = item.SLKH;
                            existsBC18Report.ERPProductCode = item.ERPProductCode;
                            existsBC18Report.ProductName = item.ProductName;
                            existsBC18Report.KTEXT = item.KTEXT;
                            existsBC18Report.MAKTX = item.MAKTX;
                            existsBC18Report.BMEIN = item.BMEIN;
                            existsBC18Report.BMSCH = item.BMSCH;
                            existsBC18Report.P1 = item.P1;
                            existsBC18Report.P2 = item.P2;
                            existsBC18Report.P3 = item.P3;
                            existsBC18Report.CompletedQuantity = item.CompletedQuantity;
                            existsBC18Report.SLCTKH = item.SLCTKH;
                            existsBC18Report.MENGE = item.MENGE;
                            existsBC18Report.LastEditTime = DateTime.Now;
                            _context.Entry(existsBC18Report).State = EntityState.Modified;
                        }
                        //insert
                        else
                        {
                            BC18Model newBC18 = new BC18Model();
                            newBC18.BC18Id = Guid.NewGuid();
                            newBC18.SaleOrg = item.SaleOrgCode;
                            newBC18.WorkShopId = item.WorkShopId;
                            newBC18.WorkShop = item.WorkShop;
                            newBC18.WorkCenterCode = item.WorkCenterCode;
                            newBC18.WorkCenter = item.WorkCenter;
                            newBC18.LSXSAP = item.LSXSAP;
                            newBC18.DSX = item.DSX;
                            newBC18.LSXDT = item.LSXDT;
                            newBC18.ProductId = item.ProductId;
                            newBC18.CompletedDateTime = item.CompletedDateTime;
                            newBC18.SLKH = item.SLKH;
                            newBC18.ERPProductCode = item.ERPProductCode;
                            newBC18.ProductName = item.ProductName;
                            newBC18.ProductAttributes = item.ProductAttributes;
                            newBC18.KTEXT = item.KTEXT;
                            newBC18.IDNRK_MES = item.IDNRK_MES;
                            newBC18.MAKTX = item.MAKTX;
                            newBC18.BMEIN = item.BMEIN;
                            newBC18.BMSCH = item.BMSCH;
                            newBC18.P1 = item.P1;
                            newBC18.P2 = item.P2;
                            newBC18.P3 = item.P3;
                            newBC18.CompletedQuantity = item.CompletedQuantity;
                            newBC18.SLCTKH = item.SLCTKH;
                            newBC18.SLCTKH = item.SLCTKH;
                            newBC18.MENGE = item.MENGE;
                            newBC18.StockRecevingType = item.StockRecevingType;
                            newBC18.IsWorkCenterCompleted = item.IsWorkCenterCompleted;
                            newBC18.WorkCenterConfirmTime = item.WorkCenterConfirmTime;
                            newBC18.CreateTime = DateTime.Now;
                            _context.Entry(newBC18).State = EntityState.Added;
                        }
                    }
                    _context.SaveChanges();
                }

                //Nếu Get được data và save file thì tiến hành ghi Log
                if (Log_CreatedDate == null)
                {
                    Log_BC18Model log = new Log_BC18Model()
                    {
                        Log_BC18ID = Guid.NewGuid(),
                        Last_Running_Time = DateTime.Now,
                        Running_Time = Running_Time,
                        Message = "Run"
                    };
                    _context.Entry(log).State = EntityState.Added;
                    _context.SaveChanges();
                }
                else
                {
                    //Nếu ngày đang tạo nhỏ hơn ngày hiện tại , thì tạo thêm 1 ngày mới để add
                    if (Running_Time.Date < DateTime.Now.Date)
                    {
                        Log_BC18Model log = new Log_BC18Model()
                        {
                            Log_BC18ID = Guid.NewGuid(),
                            Last_Running_Time = DateTime.Now,
                            Running_Time = Running_Time.AddDays(1),
                            Message = "Run"
                        };
                        _context.Entry(log).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                    //Nếu bằng ngày hiện tại thì sửa lại giờ chạy của lần chạy cuối
                    else if (Running_Time == DateTime.Now.Date)
                    {
                        var model = _context.Log_BC18Model
                                    .OrderByDescending(p => p.Running_Time).FirstOrDefault();
                        model.Last_Running_Time = DateTime.Now;
                        _context.Entry(model).State = EntityState.Modified;
                        _context.SaveChanges();
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
                SaveErrorsToDB(error);
            }
        }

        private void SaveErrorsToDB(string message)
        {
            try
            {
                //Lấy ngày cuối cùng chạy
                var Log_CreatedDate = (from p in _context.Log_BC18Model
                                       orderby p.Running_Time descending
                                       select new
                                       {
                                           p.Running_Time,
                                       }).FirstOrDefault();

                Log_BC18Model model = new Log_BC18Model()
                {
                    Log_BC18ID = Guid.NewGuid(),
                    Running_Time = Log_CreatedDate != null ? Log_CreatedDate.Running_Time : DateTime.Now.Date,
                    Last_Running_Time = DateTime.Now,
                    Message = message
                };
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
            }
            catch //(Exception ex)
            {
            }
            finally
            {
            }
        }

        public List<BC18ReportBAKViewModel> GetDataBAK(BC18ReportBAKViewModel searchViewModel)
        {


            string sqlQuery = "[Report].[usp_BC18Report_GetData] @SaleOrgCode, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOrgCode",
                    Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
                },new SqlParameter
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

            List<BC18ReportBAKViewModel> result = _context.Database.SqlQuery<BC18ReportBAKViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Báo cáo 18.1: Tổng hợp sản lượng đồng bộ tại phân xưởng
        /// </summary>
        /// <returns>ProfileReportResultViewModel</returns>
        public List<BC18_1ReportViewModel> GetData_2(BC18_1ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC18_1Report] @SaleOrgCode, @WorkShopId, @WorkCenterCode, @CompletedFromDate, @CompletedToDate";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOrgCode",
                    Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
                },new SqlParameter
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

            List<BC18_1ReportViewModel> result = _context.Database.SqlQuery<BC18_1ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

    }
}
