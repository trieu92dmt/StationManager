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
    public class BC00ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC00ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo kế hoạch sản xuất chi tiết
        /// </summary>
        /// <returns>BC00ReportViewModel</returns>
        public List<BC00ReportViewModel> GetData(BC00ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC00Report] @LSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant";

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
            };
            #endregion

            List<BC00ReportViewModel> result = _context.Database.SqlQuery<BC00ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo kế hoạch sản xuất chi tiết
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC01(Guid? DSXId = null, bool? isGetDataByDate = null)
        {
            var _loggerRepository = new LoggerRepository();
            EntityDataContext _newContext = new EntityDataContext();
            //Lấy danh sách LSXSAP
            var LSXSAPWorkFLowId = _newContext.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).Select(p => p.WorkFlowId).FirstOrDefault();
            IList<BC00ReportViewModel> LSXSAPList = await (from p in _newContext.TaskModel
                                                           join b in _newContext.TaskModel on p.ParentTaskId equals b.TaskId
                                                           where p.WorkFlowId == LSXSAPWorkFLowId
                                                           && b.Property7 != null
                                                           //Lấy theo đợt nếu có truyền tham số DSXId
                                                           && (DSXId == null || p.ParentTaskId == DSXId)
                                                           //Lấy những LSX SAP có số lượng điều chỉnh > 0
                                                           && (p.Number2 > 0)
                                                           group new { p, b } by new { p.TaskId, p.Summary, DSXID = b.TaskId, DSX = b.Summary } into g
                                                           select new BC00ReportViewModel
                                                           {
                                                               LSXSAPId = g.Key.TaskId,
                                                               LSXSAP = g.Key.Summary,
                                                               DSXId = g.Key.DSXID,
                                                               DSX = g.Key.DSX,
                                                           }).ToListAsync();
            if (isGetDataByDate == true)
            {
                //Lấy ngày cuối cùng chạy
                var Log_CreatedDate = await (from p in _context.Log_BC01Model
                                             orderby p.Running_Time descending
                                             select new
                                             {
                                                 p.Running_Time,
                                             }).FirstOrDefaultAsync();
                if (Log_CreatedDate != null)
                {
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
                }

                //Check value
                //1. Nếu đã có value => lấy ngày mới nhất đã lưu
                //2. Nếu chưa có value => lấy ngày hiện tại
                DateTime Running_Time = DateTime.Now.Date;
                if (Log_CreatedDate != null)
                {
                    Running_Time = Log_CreatedDate.Running_Time.Value;
                }
                int dateKey = int.Parse(Running_Time.ToString("yyyyMMdd"));
                //Lấy các lệnh ghi nhận trong ngày
                LSXSAPList = await (from p in _newContext.StockReceivingDetailModel
                                    //Thực thi LSX
                                    join b in _newContext.ThucThiLenhSanXuatModel on p.CustomerReference equals b.TaskId
                                    //LSXSAP
                                    join c in _newContext.TaskModel on b.ParentTaskId equals c.TaskId
                                    //Đợt
                                    join d in _newContext.TaskModel on c.ParentTaskId equals d.TaskId
                                    where p.DateKey == dateKey
                                    group new { c, d } by new { c.TaskId, c.Summary, DSXID = d.TaskId, DSX = d.Summary } into g
                                    select new BC00ReportViewModel
                                    {
                                        LSXSAPId = g.Key.TaskId,
                                        LSXSAP = g.Key.Summary,
                                        DSXId = g.Key.DSXID,
                                        DSX = g.Key.DSX,
                                    }).ToListAsync();
            }
            var DSXList = LSXSAPList.Select(p => p.DSXId).ToList();

            if (LSXSAPList != null && LSXSAPList.Count > 0)
            {
                foreach (var DSXIdItem in DSXList)
                {
                    foreach (var LSXSAP in LSXSAPList.Where(p => p.DSXId == DSXIdItem))
                    {
                        string StepCode = string.Empty;
                        try
                        {
                            //Gán SO Item để cập nhật vào DB
                            BC00ReportViewModel searchViewModel = new BC00ReportViewModel();
                            //searchViewModel.VBELN = SOItem.VBELN;
                            //searchViewModel.POSNR = SOItem.POSNR;
                            searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                            searchViewModel.DSX = LSXSAP.DSX;
                            //Lấy danh sách công đoạn thực hiện: trừ những công đoạn nhập tay
                            //var congDoanList = _newContext.PlantRoutingConfigModel.Where(p => p.Actived == true && p.Condition != "MANUAL").OrderBy(p => p.OrderIndex).ToList();
                            //Update lấy danh sách công đoạn thực hiện theo đợt 
                            var step = (from a in _newContext.TaskModel
                                        join b in _newContext.TaskModel on a.ParentTaskId equals b.TaskId
                                        where a.TaskId == LSXSAP.LSXSAPId
                                        select b.Property7
                                           ).FirstOrDefault();
                            if (!string.IsNullOrEmpty(step))
                            {
                                var stepLst = step.Split(',').Select(int.Parse).ToList();
                                if (stepLst != null && stepLst.Count > 0)
                                {
                                    var congDoanList = await _newContext.PlantRoutingConfigModel.Where(p => p.Actived == true && stepLst.Contains(p.PlantRoutingCode)).OrderBy(p => p.OrderIndex).ToListAsync();
                                    if (congDoanList != null && congDoanList.Count > 0)
                                    {
                                        foreach (var congDoan in congDoanList)
                                        {
                                            string sqlQuery = "[Report].[usp_BC01Report_GetData] @LSX, @LSXSAP, @DSX, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @StepCode";

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
                                                ParameterName = "LSXSAP",
                                                Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
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
                                                SqlDbType = SqlDbType.NVarChar,
                                                Direction = ParameterDirection.Input,
                                                ParameterName = "StepCode",
                                                Value = congDoan.PlantRoutingCode,
                                            },
                                        };
                                            #endregion
                                            _newContext.Database.CommandTimeout = 180;
                                            List<BC00ReportViewModel> result = await _newContext.Database.SqlQuery<BC00ReportViewModel>(sqlQuery, parameters.ToArray()).ToListAsync();

                                            //Lưu thông tin vào bảng BC01
                                            if (result != null && result.Count > 0)
                                            {
                                                foreach (var item in result)
                                                {
                                                    //Nếu có thông tin công đoạn mới lưu
                                                    if (item.StockCode.HasValue)
                                                    {
                                                        //Check dữ liệu
                                                        //1. Nếu tồn tại => update
                                                        //2. Chưa có => insert
                                                        //3. Nếu có trong DB nhưng không có trong danh sách truyền vào => delete

                                                        var existsBC00Report = await (from p in _newContext.BC01Model
                                                                                      where p.Plant == item.Plant
                                                                                      && p.DSX == item.DSX
                                                                                      && p.VBELN == item.VBELN
                                                                                      && p.POSNR == item.POSNR
                                                                                      && p.LSXSAPId == item.LSXSAPId
                                                                                      && p.ProductId == item.ProductId
                                                                                      && p.Stock == item.Stock
                                                                                      && p.StockCode == item.StockCode
                                                                                      select p
                                                                                ).FirstOrDefaultAsync();
                                                        //1. update
                                                        if (existsBC00Report != null)
                                                        {
                                                            existsBC00Report.LSX = item.LSX;
                                                            existsBC00Report.MType = item.MType;
                                                            existsBC00Report.Quantity = item.Quantity;
                                                            existsBC00Report.Unit = item.Unit;
                                                            existsBC00Report.Volumn = item.Volumn;
                                                            existsBC00Report.QuantityProduct = item.QuantityProduct;
                                                            existsBC00Report.LSXDNK = item.LSXDNK;
                                                            existsBC00Report.PYCSXDT = item.PYCSXDT;
                                                            existsBC00Report.AssignResponsibility = item.AssignResponsibility;
                                                            existsBC00Report.SLKH = item.SLKH;
                                                            existsBC00Report.NK = item.NK;
                                                            existsBC00Report.NumberSP = item.NumberSP;
                                                            existsBC00Report.SLKHB = item.SLKHB;
                                                            existsBC00Report.Remaining = item.Remaining;
                                                            //existsBC00Report.ContDate = item.ContDate;
                                                            existsBC00Report.DeliveryDate = item.DeliveryDate;
                                                            existsBC00Report.Stock = item.Stock;
                                                            //existsBC00Report.WorkShop = item.WorkShop;
                                                            //existsBC00Report.Leadtime = item.Leadtime;
                                                            existsBC00Report.SOCreateOn = item.SOCreateOn;
                                                            existsBC00Report.SchedulelineDeliveryDate = item.SchedulelineDeliveryDate;
                                                            existsBC00Report.PRDeliveryDate = item.PRDeliveryDate;
                                                            //SC
                                                            existsBC00Report.SchedulelineStartDateSC = item.SchedulelineStartDateSC;
                                                            existsBC00Report.StartDateSC = item.StartDateSC;
                                                            existsBC00Report.SchedulelineFinishDateSC = item.SchedulelineFinishDateSC;
                                                            //TC
                                                            existsBC00Report.SchedulelineStartDateTC = item.SchedulelineStartDateTC;
                                                            existsBC00Report.StartDateTC = item.StartDateTC;
                                                            existsBC00Report.SchedulelineFinishDateTC = item.SchedulelineFinishDateTC;
                                                            //LRN
                                                            existsBC00Report.SchedulelineStartDateLR = item.SchedulelineStartDateLR;
                                                            existsBC00Report.StartDateLR = item.StartDateLR;
                                                            existsBC00Report.SchedulelineFinishDateLR = item.SchedulelineFinishDateLR;
                                                            //HTD
                                                            existsBC00Report.SchedulelineStartDateHT = item.SchedulelineStartDateHT;
                                                            existsBC00Report.StartDateHT = item.StartDateHT;
                                                            existsBC00Report.SchedulelineFinishDateHT = item.SchedulelineFinishDateHT;

                                                            //existsBC00Report.StartDate = item.StartDate;
                                                            //existsBC00Report.FinishDate = item.FinishDate;
                                                            //existsBC00Report.StartDateChange = item.StartDateChange;
                                                            //existsBC00Report.FinishDateChange = item.FinishDateChange;
                                                            //existsBC00Report.NumberDaysDelay = item.NumberDaysDelay;
                                                            //existsBC00Report.CompletedPercent = item.CompletedPercent;
                                                            //existsBC00Report.Status = item.Status;
                                                            existsBC00Report.Plant = item.Plant;
                                                            existsBC00Report.SLTD = item.SLTD;
                                                            existsBC00Report.SLCARTON = item.SLCARTON;
                                                            existsBC00Report.WorkCenterIndex = item.WorkCenterIndex;
                                                            //existsBC00Report.ProductId = item.ProductId;
                                                            //existsBC00Report.LSXSAPId = item.LSXSAPId;
                                                            existsBC00Report.WBS = item.WBS;
                                                            existsBC00Report.SchedulelineDeliveryDateUpdate = item.SchedulelineDeliveryDateUpdate;
                                                            existsBC00Report.SLYCBOMScrap = item.SLYCBOMScrap;
                                                            existsBC00Report.PRActualQty = item.PRActualQty;
                                                            existsBC00Report.MIGOActualQty = item.MIGOActualQty;

                                                            var config = _newContext.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == existsBC00Report.StockCode).FirstOrDefault();
                                                            //Leadtime
                                                            if (!config.LeadTime.HasValue && string.IsNullOrEmpty(config.LeadTimeFormula))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                existsBC00Report.Leadtime = item.Leadtime;
                                                                existsBC00Report.ContDate = item.ContDate;
                                                            }
                                                            //StartDate
                                                            if (string.IsNullOrEmpty(config.FromDate))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                existsBC00Report.StartDate = item.StartDate;
                                                                //if (!existsBC00Report.StartDateChange.HasValue)
                                                                //{
                                                                //    existsBC00Report.StartDateChange = item.StartDateChange;
                                                                //}

                                                            }
                                                            //EndDate
                                                            if (string.IsNullOrEmpty(config.ToDate))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                existsBC00Report.FinishDate = item.FinishDate;
                                                                //if (!existsBC00Report.FinishDateChange.HasValue)
                                                                //{
                                                                //    existsBC00Report.FinishDateChange = item.FinishDateChange;
                                                                //}
                                                            }
                                                            //CompletedPercent
                                                            if (string.IsNullOrEmpty(config.Attribute8))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                existsBC00Report.CompletedPercent = item.CompletedPercent;
                                                            }
                                                            //WorkShop
                                                            if (string.IsNullOrEmpty(config.Attribute1))
                                                            {

                                                            }
                                                            else
                                                            {
                                                                existsBC00Report.WorkShop = item.WorkShop;
                                                            }
                                                            //if (existsBC00Report.FinishDateChange.HasValue && existsBC00Report.FinishDate.HasValue)
                                                            //{
                                                            //    existsBC00Report.NumberDaysDelay = Convert.ToDecimal((existsBC00Report.FinishDateChange.Value - existsBC00Report.FinishDate.Value).TotalDays);
                                                            //}

                                                            existsBC00Report.LastEditTime = DateTime.Now;
                                                            _newContext.Entry(existsBC00Report).State = EntityState.Modified;
                                                        }

                                                        //2. insert
                                                        else
                                                        {
                                                            BC01Model newBC01 = new BC01Model();
                                                            newBC01.BC01Id = Guid.NewGuid();
                                                            newBC01.Plant = item.Plant;
                                                            newBC01.LSX = item.LSX;
                                                            newBC01.DSX = item.DSX;
                                                            newBC01.VBELN = item.VBELN;
                                                            newBC01.POSNR = item.POSNR;
                                                            newBC01.LSXSAP = item.LSXSAP;
                                                            newBC01.ProductCode = item.ProductCode;
                                                            newBC01.MType = item.MType;
                                                            newBC01.Quantity = item.Quantity;
                                                            newBC01.Unit = item.Unit;
                                                            newBC01.Volumn = item.Volumn;
                                                            newBC01.QuantityProduct = item.QuantityProduct;
                                                            newBC01.LSXDNK = item.LSXDNK;
                                                            newBC01.PYCSXDT = item.PYCSXDT;
                                                            newBC01.AssignResponsibility = item.AssignResponsibility;
                                                            newBC01.SLKH = item.SLKH;
                                                            newBC01.NK = item.NK;
                                                            newBC01.NumberSP = item.NumberSP;
                                                            newBC01.SLKHB = item.SLKHB;
                                                            newBC01.Remaining = item.Remaining;
                                                            newBC01.ContDate = item.ContDate;
                                                            newBC01.DeliveryDate = item.DeliveryDate;
                                                            newBC01.StockCode = item.StockCode;
                                                            newBC01.Stock = item.Stock;
                                                            newBC01.WorkShop = item.WorkShop;
                                                            newBC01.Leadtime = item.Leadtime;
                                                            newBC01.SOCreateOn = item.SOCreateOn;
                                                            newBC01.SchedulelineDeliveryDate = item.SchedulelineDeliveryDate;
                                                            newBC01.PRDeliveryDate = item.PRDeliveryDate;
                                                            //SC
                                                            newBC01.SchedulelineStartDateSC = item.SchedulelineStartDateSC;
                                                            newBC01.StartDateSC = item.StartDateSC;
                                                            newBC01.SchedulelineFinishDateSC = item.SchedulelineFinishDateSC;
                                                            //TC
                                                            newBC01.SchedulelineStartDateTC = item.SchedulelineStartDateTC;
                                                            newBC01.StartDateTC = item.StartDateTC;
                                                            newBC01.SchedulelineFinishDateTC = item.SchedulelineFinishDateTC;
                                                            //LRN
                                                            newBC01.SchedulelineStartDateLR = item.SchedulelineStartDateLR;
                                                            newBC01.StartDateLR = item.StartDateLR;
                                                            newBC01.SchedulelineFinishDateLR = item.SchedulelineFinishDateLR;
                                                            //HTD
                                                            newBC01.SchedulelineStartDateHT = item.SchedulelineStartDateHT;
                                                            newBC01.StartDateHT = item.StartDateHT;
                                                            newBC01.SchedulelineFinishDateHT = item.SchedulelineFinishDateHT;

                                                            newBC01.StartDate = item.StartDate;
                                                            newBC01.FinishDate = item.FinishDate;
                                                            //newBC01.StartDateChange = item.StartDateChange;
                                                            //newBC01.FinishDateChange = item.FinishDateChange;
                                                            //newBC01.NumberDaysDelay = item.NumberDaysDelay;
                                                            newBC01.CompletedPercent = item.CompletedPercent;
                                                            newBC01.Status = item.Status;
                                                            newBC01.Plant = item.Plant;
                                                            newBC01.SLTD = item.SLTD;
                                                            newBC01.SLCARTON = item.SLCARTON;
                                                            newBC01.WorkCenterIndex = item.WorkCenterIndex;
                                                            newBC01.ProductId = item.ProductId;
                                                            newBC01.LSXSAPId = item.LSXSAPId;
                                                            newBC01.WBS = item.WBS;
                                                            newBC01.SchedulelineDeliveryDateUpdate = item.SchedulelineDeliveryDateUpdate;
                                                            newBC01.SLYCBOMScrap = item.SLYCBOMScrap;
                                                            newBC01.PRActualQty = item.PRActualQty;
                                                            newBC01.MIGOActualQty = item.MIGOActualQty;

                                                            newBC01.CreateTime = DateTime.Now;
                                                            _newContext.Entry(newBC01).State = EntityState.Added;
                                                        }
                                                        //3. Xóa những công đoạn không tồn tại trong đợt => user cập nhật lại công đoạn ở màn hình tách đợt
                                                        var deleteBC00Report = await (from p in _newContext.BC01Model
                                                                                      where p.Plant == item.Plant
                                                                                      && p.DSX == item.DSX
                                                                                      && p.VBELN == item.VBELN
                                                                                      && p.POSNR == item.POSNR
                                                                                      && p.LSXSAPId == item.LSXSAPId
                                                                                      && p.ProductId == item.ProductId
                                                                                      && p.StockCode.HasValue
                                                                                      && !stepLst.Contains(p.StockCode.Value)
                                                                                      select p
                                                                                ).ToListAsync();
                                                        if (deleteBC00Report != null && deleteBC00Report.Count > 0)
                                                        {
                                                            _newContext.BC01Model.RemoveRange(deleteBC00Report);
                                                        }
                                                        await _newContext.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                            StepCode = congDoan.PlantRoutingCode.ToString();
                                        }
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
                            //_loggerRepository.Logging("BC01 - SO: " + SOItem.VBELN + ", SOLine: " + SOItem.POSNR + " " + message, "ERROR");
                            _loggerRepository.Logging("BC01 - LSXSAPId: " + LSXSAP.LSXSAPId + ", LSXSAP: " + LSXSAP.LSXSAP + " " + ", StepCode: " + StepCode + " " + message, "ERROR");
                            continue;
                        }
                    }
                }

            }
            _newContext.Dispose();
        }

        public void UpdateBC01FromDSX(Guid? DSXId = null, Guid? LSXSAPId = null)
        {
            var _loggerRepository = new LoggerRepository();
            EntityDataContext _newContext = new EntityDataContext();
            //Lấy danh sách LSXSAP
            var LSXSAPWorkFLowId = _newContext.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).Select(p => p.WorkFlowId).FirstOrDefault();
            IList<BC00ReportViewModel> LSXSAPList = (from p in _newContext.TaskModel
                                                     join b in _newContext.TaskModel on p.ParentTaskId equals b.TaskId
                                                     where p.WorkFlowId == LSXSAPWorkFLowId
                                                     && b.Property7 != null
                                                     //Lấy theo đợt nếu có truyền tham số DSXId
                                                     && (DSXId == null || p.ParentTaskId == DSXId)
                                                     //Lấy theo LSXSAP nếu có truyền tham số DSXId
                                                     && (LSXSAPId == null || p.TaskId == LSXSAPId)
                                                     //Lấy những LSX SAP có số lượng điều chỉnh > 0
                                                     && (p.Number2 > 0)
                                                     group p by new { p.TaskId, LSXSAP = p.Summary, DSX = b.Summary } into g
                                                     orderby g.Max(p => p.CreateTime) descending
                                                     select new BC00ReportViewModel
                                                     {
                                                         LSXSAPId = g.Key.TaskId,
                                                         LSXSAP = g.Key.LSXSAP,
                                                         DSX = g.Key.DSX,
                                                     }).ToList();

            if (LSXSAPList != null)
            {
                foreach (var LSXSAP in LSXSAPList)
                {
                    string StepCode = string.Empty;
                    try
                    {
                        //Gán SO Item để cập nhật vào DB
                        BC00ReportViewModel searchViewModel = new BC00ReportViewModel();
                        //searchViewModel.VBELN = SOItem.VBELN;
                        //searchViewModel.POSNR = SOItem.POSNR;
                        searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                        searchViewModel.DSX = LSXSAP.DSX;
                        //Lấy danh sách công đoạn thực hiện: trừ những công đoạn nhập tay
                        //var congDoanList = _newContext.PlantRoutingConfigModel.Where(p => p.Actived == true && p.Condition != "MANUAL").OrderBy(p => p.OrderIndex).ToList();
                        //Update lấy danh sách công đoạn thực hiện theo đợt 
                        var step = (from a in _newContext.TaskModel
                                    join b in _newContext.TaskModel on a.ParentTaskId equals b.TaskId
                                    where a.TaskId == LSXSAP.LSXSAPId
                                    select b.Property7
                                       ).FirstOrDefault();
                        if (!string.IsNullOrEmpty(step))
                        {
                            var stepLst = step.Split(',').Select(int.Parse).ToList();
                            if (stepLst != null && stepLst.Count > 0)
                            {
                                var congDoanList = _newContext.PlantRoutingConfigModel.Where(p => p.Actived == true && stepLst.Contains(p.PlantRoutingCode)).OrderBy(p => p.OrderIndex).ToList();
                                if (congDoanList != null && congDoanList.Count > 0)
                                {
                                    foreach (var congDoan in congDoanList)
                                    {
                                        string sqlQuery = "[Report].[usp_BC01Report_GetData] @LSX, @LSXSAP, @DSX, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @StepCode";

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
                                                ParameterName = "LSXSAP",
                                                Value = searchViewModel.LSXSAP ?? (object)DBNull.Value,
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
                                                SqlDbType = SqlDbType.NVarChar,
                                                Direction = ParameterDirection.Input,
                                                ParameterName = "StepCode",
                                                Value = congDoan.PlantRoutingCode,
                                            },
                                        };
                                        #endregion
                                        _newContext.Database.CommandTimeout = 180;
                                        List<BC00ReportViewModel> result = _newContext.Database.SqlQuery<BC00ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

                                        //Lưu thông tin vào bảng BC01
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var item in result)
                                            {
                                                //Nếu có thông tin công đoạn mới lưu
                                                if (item.StockCode.HasValue)
                                                {
                                                    //Check dữ liệu
                                                    //1. Nếu tồn tại => update
                                                    //2. Chưa có => insert
                                                    //3. Nếu có trong DB nhưng không có trong danh sách truyền vào => delete

                                                    var existsBC00Report = (from p in _newContext.BC01Model
                                                                            where p.Plant == item.Plant
                                                                            && p.DSX == item.DSX
                                                                            && p.VBELN == item.VBELN
                                                                            && p.POSNR == item.POSNR
                                                                            && p.LSXSAPId == item.LSXSAPId
                                                                            && p.ProductId == item.ProductId
                                                                            && p.Stock == item.Stock
                                                                            && p.StockCode == item.StockCode
                                                                            select p
                                                                            ).FirstOrDefault();
                                                    //1. update
                                                    if (existsBC00Report != null)
                                                    {
                                                        existsBC00Report.LSX = item.LSX;
                                                        existsBC00Report.MType = item.MType;
                                                        existsBC00Report.Quantity = item.Quantity;
                                                        existsBC00Report.Unit = item.Unit;
                                                        existsBC00Report.Volumn = item.Volumn;
                                                        existsBC00Report.QuantityProduct = item.QuantityProduct;
                                                        existsBC00Report.LSXDNK = item.LSXDNK;
                                                        existsBC00Report.PYCSXDT = item.PYCSXDT;
                                                        existsBC00Report.AssignResponsibility = item.AssignResponsibility;
                                                        existsBC00Report.SLKH = item.SLKH;
                                                        existsBC00Report.NK = item.NK;
                                                        existsBC00Report.NumberSP = item.NumberSP;
                                                        existsBC00Report.SLKHB = item.SLKHB;
                                                        existsBC00Report.Remaining = item.Remaining;
                                                        //existsBC00Report.ContDate = item.ContDate;
                                                        existsBC00Report.DeliveryDate = item.DeliveryDate;
                                                        existsBC00Report.Stock = item.Stock;
                                                        //existsBC00Report.WorkShop = item.WorkShop;
                                                        //existsBC00Report.Leadtime = item.Leadtime;
                                                        existsBC00Report.SOCreateOn = item.SOCreateOn;
                                                        existsBC00Report.SchedulelineDeliveryDate = item.SchedulelineDeliveryDate;
                                                        existsBC00Report.PRDeliveryDate = item.PRDeliveryDate;
                                                        //SC
                                                        existsBC00Report.SchedulelineStartDateSC = item.SchedulelineStartDateSC;
                                                        existsBC00Report.StartDateSC = item.StartDateSC;
                                                        existsBC00Report.SchedulelineFinishDateSC = item.SchedulelineFinishDateSC;
                                                        //TC
                                                        existsBC00Report.SchedulelineStartDateTC = item.SchedulelineStartDateTC;
                                                        existsBC00Report.StartDateTC = item.StartDateTC;
                                                        existsBC00Report.SchedulelineFinishDateTC = item.SchedulelineFinishDateTC;
                                                        //LRN
                                                        existsBC00Report.SchedulelineStartDateLR = item.SchedulelineStartDateLR;
                                                        existsBC00Report.StartDateLR = item.StartDateLR;
                                                        existsBC00Report.SchedulelineFinishDateLR = item.SchedulelineFinishDateLR;
                                                        //HTD
                                                        existsBC00Report.SchedulelineStartDateHT = item.SchedulelineStartDateHT;
                                                        existsBC00Report.StartDateHT = item.StartDateHT;
                                                        existsBC00Report.SchedulelineFinishDateHT = item.SchedulelineFinishDateHT;

                                                        //existsBC00Report.StartDate = item.StartDate;
                                                        //existsBC00Report.FinishDate = item.FinishDate;
                                                        //existsBC00Report.StartDateChange = item.StartDateChange;
                                                        //existsBC00Report.FinishDateChange = item.FinishDateChange;
                                                        //existsBC00Report.NumberDaysDelay = item.NumberDaysDelay;
                                                        //existsBC00Report.CompletedPercent = item.CompletedPercent;
                                                        //existsBC00Report.Status = item.Status;
                                                        existsBC00Report.Plant = item.Plant;
                                                        existsBC00Report.SLTD = item.SLTD;
                                                        existsBC00Report.SLCARTON = item.SLCARTON;
                                                        existsBC00Report.WorkCenterIndex = item.WorkCenterIndex;
                                                        //existsBC00Report.ProductId = item.ProductId;
                                                        //existsBC00Report.LSXSAPId = item.LSXSAPId;
                                                        existsBC00Report.WBS = item.WBS;
                                                        existsBC00Report.SchedulelineDeliveryDateUpdate = item.SchedulelineDeliveryDateUpdate;
                                                        existsBC00Report.SLYCBOMScrap = item.SLYCBOMScrap;
                                                        existsBC00Report.PRActualQty = item.PRActualQty;
                                                        existsBC00Report.MIGOActualQty = item.MIGOActualQty;

                                                        var config = _newContext.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == existsBC00Report.StockCode).FirstOrDefault();
                                                        //Leadtime
                                                        if (!config.LeadTime.HasValue && string.IsNullOrEmpty(config.LeadTimeFormula))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            existsBC00Report.Leadtime = item.Leadtime;
                                                            existsBC00Report.ContDate = item.ContDate;
                                                        }
                                                        //StartDate
                                                        if (string.IsNullOrEmpty(config.FromDate))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            existsBC00Report.StartDate = item.StartDate;
                                                            //if (!existsBC00Report.StartDateChange.HasValue)
                                                            //{
                                                            //    existsBC00Report.StartDateChange = item.StartDateChange;
                                                            //}

                                                        }
                                                        //EndDate
                                                        if (string.IsNullOrEmpty(config.ToDate))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            existsBC00Report.FinishDate = item.FinishDate;
                                                            //if (!existsBC00Report.FinishDateChange.HasValue)
                                                            //{
                                                            //    existsBC00Report.FinishDateChange = item.FinishDateChange;
                                                            //}
                                                        }
                                                        //CompletedPercent
                                                        if (string.IsNullOrEmpty(config.Attribute8))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            existsBC00Report.CompletedPercent = item.CompletedPercent;
                                                        }
                                                        //WorkShop
                                                        if (string.IsNullOrEmpty(config.Attribute1))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            existsBC00Report.WorkShop = item.WorkShop;
                                                        }
                                                        //if (existsBC00Report.FinishDateChange.HasValue && existsBC00Report.FinishDate.HasValue)
                                                        //{
                                                        //    existsBC00Report.NumberDaysDelay = Convert.ToDecimal((existsBC00Report.FinishDateChange.Value - existsBC00Report.FinishDate.Value).TotalDays);
                                                        //}

                                                        existsBC00Report.LastEditTime = DateTime.Now;
                                                        _newContext.Entry(existsBC00Report).State = EntityState.Modified;
                                                    }

                                                    //2. insert
                                                    else
                                                    {
                                                        BC01Model newBC01 = new BC01Model();
                                                        newBC01.BC01Id = Guid.NewGuid();
                                                        newBC01.Plant = item.Plant;
                                                        newBC01.LSX = item.LSX;
                                                        newBC01.DSX = item.DSX;
                                                        newBC01.VBELN = item.VBELN;
                                                        newBC01.POSNR = item.POSNR;
                                                        newBC01.LSXSAP = item.LSXSAP;
                                                        newBC01.ProductCode = item.ProductCode;
                                                        newBC01.MType = item.MType;
                                                        newBC01.Quantity = item.Quantity;
                                                        newBC01.Unit = item.Unit;
                                                        newBC01.Volumn = item.Volumn;
                                                        newBC01.QuantityProduct = item.QuantityProduct;
                                                        newBC01.LSXDNK = item.LSXDNK;
                                                        newBC01.PYCSXDT = item.PYCSXDT;
                                                        newBC01.AssignResponsibility = item.AssignResponsibility;
                                                        newBC01.SLKH = item.SLKH;
                                                        newBC01.NK = item.NK;
                                                        newBC01.NumberSP = item.NumberSP;
                                                        newBC01.SLKHB = item.SLKHB;
                                                        newBC01.Remaining = item.Remaining;
                                                        newBC01.ContDate = item.ContDate;
                                                        newBC01.DeliveryDate = item.DeliveryDate;
                                                        newBC01.StockCode = item.StockCode;
                                                        newBC01.Stock = item.Stock;
                                                        newBC01.WorkShop = item.WorkShop;
                                                        newBC01.Leadtime = item.Leadtime;
                                                        newBC01.SOCreateOn = item.SOCreateOn;
                                                        newBC01.SchedulelineDeliveryDate = item.SchedulelineDeliveryDate;
                                                        newBC01.PRDeliveryDate = item.PRDeliveryDate;
                                                        //SC
                                                        newBC01.SchedulelineStartDateSC = item.SchedulelineStartDateSC;
                                                        newBC01.StartDateSC = item.StartDateSC;
                                                        newBC01.SchedulelineFinishDateSC = item.SchedulelineFinishDateSC;
                                                        //TC
                                                        newBC01.SchedulelineStartDateTC = item.SchedulelineStartDateTC;
                                                        newBC01.StartDateTC = item.StartDateTC;
                                                        newBC01.SchedulelineFinishDateTC = item.SchedulelineFinishDateTC;
                                                        //LRN
                                                        newBC01.SchedulelineStartDateLR = item.SchedulelineStartDateLR;
                                                        newBC01.StartDateLR = item.StartDateLR;
                                                        newBC01.SchedulelineFinishDateLR = item.SchedulelineFinishDateLR;
                                                        //HTD
                                                        newBC01.SchedulelineStartDateHT = item.SchedulelineStartDateHT;
                                                        newBC01.StartDateHT = item.StartDateHT;
                                                        newBC01.SchedulelineFinishDateHT = item.SchedulelineFinishDateHT;

                                                        newBC01.StartDate = item.StartDate;
                                                        newBC01.FinishDate = item.FinishDate;
                                                        //newBC01.StartDateChange = item.StartDateChange;
                                                        //newBC01.FinishDateChange = item.FinishDateChange;
                                                        //newBC01.NumberDaysDelay = item.NumberDaysDelay;
                                                        newBC01.CompletedPercent = item.CompletedPercent;
                                                        newBC01.Status = item.Status;
                                                        newBC01.Plant = item.Plant;
                                                        newBC01.SLTD = item.SLTD;
                                                        newBC01.SLCARTON = item.SLCARTON;
                                                        newBC01.WorkCenterIndex = item.WorkCenterIndex;
                                                        newBC01.ProductId = item.ProductId;
                                                        newBC01.LSXSAPId = item.LSXSAPId;
                                                        newBC01.WBS = item.WBS;
                                                        newBC01.SchedulelineDeliveryDateUpdate = item.SchedulelineDeliveryDateUpdate;
                                                        newBC01.SLYCBOMScrap = item.SLYCBOMScrap;
                                                        newBC01.PRActualQty = item.PRActualQty;
                                                        newBC01.MIGOActualQty = item.MIGOActualQty;

                                                        newBC01.CreateTime = DateTime.Now;
                                                        _newContext.Entry(newBC01).State = EntityState.Added;
                                                    }
                                                    //3. Xóa những công đoạn không tồn tại trong đợt => user cập nhật lại công đoạn ở màn hình tách đợt
                                                    var deleteBC00Report = (from p in _newContext.BC01Model
                                                                            where p.Plant == item.Plant
                                                                            && p.DSX == item.DSX
                                                                            && p.VBELN == item.VBELN
                                                                            && p.POSNR == item.POSNR
                                                                            && p.LSXSAPId == item.LSXSAPId
                                                                            && p.ProductId == item.ProductId
                                                                            && p.StockCode.HasValue
                                                                            && !stepLst.Contains(p.StockCode.Value)
                                                                            select p
                                                                            ).ToList();
                                                    if (deleteBC00Report != null && deleteBC00Report.Count > 0)
                                                    {
                                                        _newContext.BC01Model.RemoveRange(deleteBC00Report);
                                                    }
                                                    _newContext.SaveChanges();
                                                }
                                            }
                                        }
                                        StepCode = congDoan.PlantRoutingCode.ToString();
                                    }
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
                        //_loggerRepository.Logging("BC01 - SO: " + SOItem.VBELN + ", SOLine: " + SOItem.POSNR + " " + message, "ERROR");
                        _loggerRepository.Logging("BC01 - LSXSAPId: " + LSXSAP.LSXSAPId + ", LSXSAP: " + LSXSAP.LSXSAP + " " + ", StepCode: " + StepCode + " " + message, "ERROR");
                        continue;
                    }
                }
            }
            _newContext.Dispose();
        }

        public void UpdateDataBC01(BC00ReportFormViewModel viewModel, Guid? CurrentAccountId)
        {
            BC01Model report = _context.BC01Model.Where(p => p.BC01Id == viewModel.BC01Id).FirstOrDefault();
            if (report != null)
            {
                //STT (Mức độ ưu tiên)
                report.STT = viewModel.STT;
                //Phòng ban lỗi
                report.ErrorDepartment = viewModel.ErrorDepartment;
                //Tình trạng
                report.Status = viewModel.Status;

                var config = _context.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == report.StockCode).FirstOrDefault();
                //Leadtime
                if (!config.LeadTime.HasValue && string.IsNullOrEmpty(config.LeadTimeFormula))
                {
                    report.Leadtime = viewModel.Leadtime;

                    //Tính số cont/ ngày = SLKH(Cont) / LeadTime
                    if (report.SLKH.HasValue && report.SLKH > 0 && viewModel.Leadtime.HasValue && viewModel.Leadtime > 0)
                    {
                        report.ContDate = report.SLKH / viewModel.Leadtime;
                    }
                }
                //StartDate
                if (string.IsNullOrEmpty(config.FromDate))
                {
                    //Ngày bắt đầu
                    report.StartDate = viewModel.StartDate;
                    report.StartDateChange = viewModel.StartDate;
                }
                //EndDate
                if (string.IsNullOrEmpty(config.ToDate))
                {
                    //Ngày kết thúc
                    report.FinishDate = viewModel.EndDate;
                    report.FinishDateChange = viewModel.EndDate;
                }
                //CompletedPercent
                if (string.IsNullOrEmpty(config.Attribute8))
                {
                    //% hoàn thành
                    report.CompletedPercent = viewModel.CompletedPercent;
                }
                //WorkShop
                if (string.IsNullOrEmpty(config.Attribute1))
                {
                    //Phân xưởng
                    report.WorkShop = viewModel.WorkShop;
                }
                //StartDateChange
                //Ngày bắt đầu điều chỉnh
                if (viewModel.StartDateChange.HasValue)
                {
                    report.StartDateChange = viewModel.StartDateChange;
                }
                //EndDateChange
                //Ngày kết thúc điều chỉnh
                if (viewModel.EndDateChange.HasValue)
                {
                    report.FinishDateChange = viewModel.EndDateChange;
                }
                //Số ngày trễ
                report.NumberDaysDelay = Convert.ToDecimal((report.FinishDateChange.Value - report.FinishDate.Value).TotalDays);
                //if (string.IsNullOrEmpty(config.FromDate) && string.IsNullOrEmpty(config.ToDate) && viewModel.EndDate.HasValue && viewModel.StartDate.HasValue)
                //{
                //    report.NumberDaysDelay = Convert.ToDecimal((report.FinishDateChange.Value - report.FinishDate.Value).TotalDays);
                //}
                //Nhóm lỗi
                report.ErrorGroup = viewModel.ErrorGroup;
                //Mô tả chi tiết lỗi 
                report.ErrorDetail = viewModel.ErrorDetail;

                report.LastEditTime = DateTime.Now;
                report.LastEditBy = CurrentAccountId;

                _context.Entry(report).State = EntityState.Modified;

                //Cập nhật STT theo LSX SAP
                var LSXDT = _context.BC01Model.Where(p => p.LSXSAP == report.LSXSAP).Select(p => p.LSX).FirstOrDefault();
                var LSXSAP = _context.BC01Model.Where(f => f.LSX == LSXDT && f.BC01Id != report.BC01Id).ToList();
                LSXSAP.ForEach(a => a.STT = viewModel.STT);

                _context.SaveChanges();
            }
        }

        public void CreateDataBC01(BC00ReportFormViewModel viewModel, Guid? CurrentAccountId)
        {
            //Lấy data của báo cáo dựa vào LSXSAP và công đoạn thực hiện
            string sqlQuery = "[Report].[usp_BC01Report_GetData] @LSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @StepCode";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSX",
                    Value = (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    Value = viewModel.LSXSAP ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "VBELN",
                    Value = (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "POSNR",
                    Value = (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedToDate",
                    Value = (object)DBNull.Value,
                },
                 new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedFromDate",
                    Value = (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StepCode",
                    Value = viewModel.StockCode,
                },
            };
            #endregion
            _context.Database.CommandTimeout = 180;
            List<BC00ReportViewModel> resultList = _context.Database.SqlQuery<BC00ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
            if (resultList != null && resultList.Count > 0)
            {
                var result = resultList.FirstOrDefault();
                //Lưu thông tin thêm mới vào báo cáo
                BC01Model newBC01 = new BC01Model();
                newBC01.BC01Id = Guid.NewGuid();
                //STT (Mức độ ưu tiên)
                //Nếu user có nhập STT thì cập nhật lại STT trong báo cáo theo LSX DT
                // Nếu user không nhập STT thì tìm trong báo cáo theo LSX DT nếu có dữ liệu STT thì update vào thêm mới
                if (viewModel.STT.HasValue)
                {
                    newBC01.STT = viewModel.STT;
                    var LSXDT = _context.BC01Model.Where(p => p.LSXSAP == viewModel.LSXSAP).Select(p => p.LSX).FirstOrDefault();
                    var existsBC01 = _context.BC01Model.Where(p => p.LSX == LSXDT).ToList();
                    if (existsBC01 != null && existsBC01.Count > 0)
                    {
                        existsBC01.ForEach(a => a.STT = viewModel.STT);
                    }
                }
                else
                {
                    var existsBC01 = _context.BC01Model.Where(p => p.LSXSAP == viewModel.LSXSAP && p.STT.HasValue).FirstOrDefault();
                    if (existsBC01 != null)
                    {
                        newBC01.STT = existsBC01.STT;
                    }
                }
                newBC01.Plant = result.Plant;
                newBC01.LSX = result.LSX;
                newBC01.VBELN = result.VBELN;
                newBC01.POSNR = result.POSNR;
                newBC01.LSXSAP = result.LSXSAP;
                newBC01.ProductCode = result.ProductCode;
                newBC01.MType = result.MType;
                newBC01.Quantity = result.Quantity;
                newBC01.Unit = result.Unit;
                newBC01.Volumn = result.Volumn;
                newBC01.QuantityProduct = result.QuantityProduct;
                newBC01.LSXDNK = result.LSXDNK;
                newBC01.PYCSXDT = result.PYCSXDT;
                newBC01.AssignResponsibility = result.AssignResponsibility;
                newBC01.SLKH = result.SLKH;
                newBC01.NK = result.NK;
                newBC01.NumberSP = result.NumberSP;
                newBC01.SLKHB = result.SLKHB;
                newBC01.Remaining = result.Remaining;
                //Tính số cont/ ngày = SLKH(Cont) / LeadTime
                if (result.SLKH.HasValue && result.SLKH > 0 && viewModel.Leadtime.HasValue && viewModel.Leadtime > 0)
                {
                    newBC01.ContDate = result.SLKH / viewModel.Leadtime;
                }

                newBC01.DeliveryDate = result.DeliveryDate;
                newBC01.StockCode = viewModel.StockCode;
                if (viewModel.StockCode.HasValue)
                {
                    var stockName = _context.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == viewModel.StockCode).Select(p => p.PlantRoutingName).FirstOrDefault();
                    if (stockName != null)
                    {
                        newBC01.Stock = stockName;
                    }
                }

                newBC01.WorkShop = result.WorkShop;
                newBC01.Leadtime = viewModel.Leadtime;
                newBC01.SOCreateOn = result.SOCreateOn;
                newBC01.SchedulelineDeliveryDate = result.SchedulelineDeliveryDate;
                newBC01.PRDeliveryDate = result.PRDeliveryDate;
                //SC
                newBC01.SchedulelineStartDateSC = result.SchedulelineStartDateSC;
                newBC01.StartDateSC = result.StartDateSC;
                newBC01.SchedulelineFinishDateSC = result.SchedulelineFinishDateSC;
                //TC
                newBC01.SchedulelineStartDateTC = result.SchedulelineStartDateTC;
                newBC01.StartDateTC = result.StartDateTC;
                newBC01.SchedulelineFinishDateTC = result.SchedulelineFinishDateTC;
                //LRN
                newBC01.SchedulelineStartDateLR = result.SchedulelineStartDateLR;
                newBC01.StartDateLR = result.StartDateLR;
                newBC01.SchedulelineFinishDateLR = result.SchedulelineFinishDateLR;
                //HTD
                newBC01.SchedulelineStartDateHT = result.SchedulelineStartDateHT;
                newBC01.StartDateHT = result.StartDateHT;
                newBC01.SchedulelineFinishDateHT = result.SchedulelineFinishDateHT;

                newBC01.StartDate = viewModel.StartDate;
                newBC01.FinishDate = viewModel.EndDate;
                newBC01.StartDateChange = viewModel.StartDate;
                newBC01.FinishDateChange = viewModel.EndDate;
                if (viewModel.EndDate.HasValue && viewModel.StartDate.HasValue)
                {
                    newBC01.NumberDaysDelay = Convert.ToDecimal((viewModel.EndDate.Value - viewModel.StartDate.Value).TotalDays);
                }

                newBC01.CompletedPercent = result.CompletedPercent;
                newBC01.Plant = result.Plant;
                newBC01.SLTD = result.SLTD;
                newBC01.SLCARTON = result.SLCARTON;
                newBC01.WorkCenterIndex = result.WorkCenterIndex;
                newBC01.ProductId = result.ProductId;
                newBC01.LSXSAPId = result.LSXSAPId;

                newBC01.ErrorDepartment = viewModel.ErrorDepartment;
                newBC01.Status = viewModel.Status;

                newBC01.CreateTime = DateTime.Now;
                newBC01.CreateBy = CurrentAccountId;
                _context.Entry(newBC01).State = EntityState.Added;

                _context.SaveChanges();
            }
        }
    }
}
