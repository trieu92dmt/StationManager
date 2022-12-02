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
    public class BC01ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC01ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo kế hoạch sản xuất chi tiết
        /// </summary>
        /// <returns>BC01ReportViewModel</returns>
        public List<BC01ReportViewModel> GetData(BC01ReportViewModel searchViewModel)
        {
            string sqlQuery = "[Report].[usp_BC01Report] @LSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @TopRow, @Plant, @FinishFromDate, @FinishToDate, @isOpen";

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
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FinishFromDate",
                    Value = searchViewModel.FinishFromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FinishToDate",
                    Value = searchViewModel.FinishToDate ?? (object)DBNull.Value,
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

            List<BC01ReportViewModel> result = _context.Database.SqlQuery<BC01ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }

        /// <summary>
        /// Lưu data Báo cáo kế hoạch sản xuất chi tiết
        /// </summary>
        /// <returns></returns>
        public async Task UpdateBC01()
        {
            var _loggerRepository = new LoggerRepository();
            //Lấy danh sách SO Item 
            //IList<BC01ReportViewModel> SOItemList = await (from p in _context.SaleOrderItem100Model
            //                                               group p by new { p.VBELN, p.POSNR_MES } into g
            //                                               orderby g.Max(p => p.CreateTime) descending
            //                                               select new BC01ReportViewModel
            //                                               {
            //                                                   VBELN = g.Key.VBELN,
            //                                                   POSNR = g.Key.POSNR_MES,
            //
            //}).ToListAsync();
            //Lấy danh sách LSXSAP
            var LSXSAPWorkFLowId = _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).Select(p => p.WorkFlowId).FirstOrDefault();
            IList<BC01ReportViewModel> LSXSAPList = await (from p in _context.TaskModel
                                                           join b in _context.TaskModel on p.ParentTaskId equals b.TaskId
                                                           where p.WorkFlowId == LSXSAPWorkFLowId
                                                           && b.Property7 != null
                                                           group p by new { p.TaskId, p.Summary } into g
                                                           orderby g.Max(p => p.CreateTime) descending
                                                           select new BC01ReportViewModel
                                                           {
                                                               LSXSAPId = g.Key.TaskId,
                                                               LSXSAP = g.Key.Summary,
                                                           }).ToListAsync();
            if (LSXSAPList != null)
            {
                foreach (var LSXSAP in LSXSAPList)
                {
                    try
                    {
                        EntityDataContext _newContext = new EntityDataContext();
                        //Gán SO Item để cập nhật vào DB
                        BC01ReportViewModel searchViewModel = new BC01ReportViewModel();
                        //searchViewModel.VBELN = SOItem.VBELN;
                        //searchViewModel.POSNR = SOItem.POSNR;
                        searchViewModel.LSXSAP = LSXSAP.LSXSAP;
                        //Lấy danh sách công đoạn thực hiện: trừ những công đoạn nhập tay
                        //var congDoanList = _newContext.PlantRoutingConfigModel.Where(p => p.Actived == true && p.Condition != "MANUAL").OrderBy(p => p.OrderIndex).ToList();
                        //Update lấy danh sách công đoạn thực hiện theo đợt 
                        var step = (from a in _context.TaskModel
                                    join b in _context.TaskModel on a.ParentTaskId equals b.TaskId
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
                                        string sqlQuery = "[Report].[usp_BC01Report_GetData] @LSX, @LSXSAP, @VBELN, @POSNR, @CompletedFromDate, @CompletedToDate, @StepCode";

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
                                                SqlDbType = SqlDbType.NVarChar,
                                                Direction = ParameterDirection.Input,
                                                ParameterName = "StepCode",
                                                Value = congDoan.PlantRoutingCode,
                                            },
                                        };
                                        #endregion
                                        _newContext.Database.CommandTimeout = 180;
                                        List<BC01ReportViewModel> result = _newContext.Database.SqlQuery<BC01ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

                                        //Lưu thông tin vào bảng BC01
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var item in result)
                                            {
                                                //Check dữ liệu
                                                //1. Nếu tồn tại => update
                                                //2. Chưa có => insert
                                                var existsBC01Report = (from p in _newContext.BC01Model
                                                                        where p.Plant == item.Plant
                                                                        && p.DSX == item.DSX
                                                                        && p.VBELN == item.VBELN
                                                                        && p.POSNR == item.POSNR
                                                                        && p.LSXSAPId == item.LSXSAPId
                                                                        && p.ProductId == item.ProductId
                                                                        && p.StockCode == item.StockCode
                                                                        select p
                                                                        ).FirstOrDefault();
                                                //update
                                                if (existsBC01Report != null)
                                                {
                                                    existsBC01Report.LSX = item.LSX;
                                                    existsBC01Report.Stock = item.Stock;
                                                    existsBC01Report.MType = item.MType;
                                                    existsBC01Report.Quantity = item.Quantity;
                                                    existsBC01Report.Unit = item.Unit;
                                                    existsBC01Report.Volumn = item.Volumn;
                                                    existsBC01Report.QuantityProduct = item.QuantityProduct;
                                                    existsBC01Report.LSXDNK = item.LSXDNK;
                                                    existsBC01Report.PYCSXDT = item.PYCSXDT;
                                                    existsBC01Report.AssignResponsibility = item.AssignResponsibility;
                                                    existsBC01Report.SLKH = item.SLKH;
                                                    existsBC01Report.NK = item.NK;
                                                    existsBC01Report.NumberSP = item.NumberSP;
                                                    existsBC01Report.SLKHB = item.SLKHB;
                                                    existsBC01Report.Remaining = item.Remaining;
                                                    existsBC01Report.ContDate = item.ContDate;
                                                    existsBC01Report.DeliveryDate = item.DeliveryDate;
                                                    existsBC01Report.Stock = item.Stock;
                                                    existsBC01Report.WorkShop = item.WorkShop;
                                                    existsBC01Report.Leadtime = item.Leadtime;
                                                    existsBC01Report.SOCreateOn = item.SOCreateOn;
                                                    existsBC01Report.SchedulelineDeliveryDate = item.SchedulelineDeliveryDate;
                                                    existsBC01Report.PRDeliveryDate = item.PRDeliveryDate;
                                                    //SC
                                                    existsBC01Report.SchedulelineStartDateSC = item.SchedulelineStartDateSC;
                                                    existsBC01Report.StartDateSC = item.StartDateSC;
                                                    existsBC01Report.SchedulelineFinishDateSC = item.SchedulelineFinishDateSC;
                                                    //TC
                                                    existsBC01Report.SchedulelineStartDateTC = item.SchedulelineStartDateTC;
                                                    existsBC01Report.StartDateTC = item.StartDateTC;
                                                    existsBC01Report.SchedulelineFinishDateTC = item.SchedulelineFinishDateTC;
                                                    //LRN
                                                    existsBC01Report.SchedulelineStartDateLR = item.SchedulelineStartDateLR;
                                                    existsBC01Report.StartDateLR = item.StartDateLR;
                                                    existsBC01Report.SchedulelineFinishDateLR = item.SchedulelineFinishDateLR;
                                                    //HTD
                                                    existsBC01Report.SchedulelineStartDateHT = item.SchedulelineStartDateHT;
                                                    existsBC01Report.StartDateHT = item.StartDateHT;
                                                    existsBC01Report.SchedulelineFinishDateHT = item.SchedulelineFinishDateHT;

                                                    existsBC01Report.StartDate = item.StartDate;
                                                    existsBC01Report.FinishDate = item.FinishDate;
                                                    existsBC01Report.StartDateChange = item.StartDateChange;
                                                    existsBC01Report.FinishDateChange = item.FinishDateChange;
                                                    existsBC01Report.NumberDaysDelay = item.NumberDaysDelay;
                                                    existsBC01Report.CompletedPercent = item.CompletedPercent;
                                                    existsBC01Report.Status = item.Status;
                                                    existsBC01Report.Plant = item.Plant;
                                                    existsBC01Report.SLTD = item.SLTD;
                                                    existsBC01Report.SLCARTON = item.SLCARTON;
                                                    existsBC01Report.WorkCenterIndex = item.WorkCenterIndex;
                                                    existsBC01Report.ProductId = item.ProductId;
                                                    existsBC01Report.LSXSAPId = item.LSXSAPId;
                                                    existsBC01Report.WBS = item.WBS;
                                                    existsBC01Report.SchedulelineDeliveryDateUpdate = item.SchedulelineDeliveryDateUpdate;
                                                    existsBC01Report.SLYCBOMScrap = item.SLYCBOMScrap;
                                                    existsBC01Report.PRActualQty = item.PRActualQty;
                                                    existsBC01Report.MIGOActualQty = item.MIGOActualQty;

                                                    existsBC01Report.LastEditTime = DateTime.Now;
                                                    _newContext.Entry(existsBC01Report).State = EntityState.Modified;
                                                }
                                                //insert
                                                else
                                                {
                                                    BC01Model newBC01 = new BC01Model();
                                                    newBC01.BC01Id = Guid.NewGuid();
                                                    newBC01.Plant = item.Plant;
                                                    newBC01.LSX = item.LSX;
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
                                                    newBC01.StartDateChange = item.StartDateChange;
                                                    newBC01.FinishDateChange = item.FinishDateChange;
                                                    newBC01.NumberDaysDelay = item.NumberDaysDelay;
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
                                                _newContext.SaveChanges();
                                            }
                                        }
                                    }
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
                        //_loggerRepository.Logging("BC01 - SO: " + SOItem.VBELN + ", SOLine: " + SOItem.POSNR + " " + message, "ERROR");
                        _loggerRepository.Logging("BC01 - LSXSAPId: " + LSXSAP.LSXSAPId + ", LSXSAP: " + LSXSAP.LSXSAP + " " + message, "ERROR");
                        continue;
                    }
                }
            }
        }

        public void UpdateDataBC01(BC01ReportFormViewModel viewModel, Guid? CurrentAccountId)
        {
            var reportLst = _context.BC01Model.Where(p => p.DSX == viewModel.DSX).Select(p => p.BC01Id).ToList();
            if (reportLst != null && reportLst.Count > 0)
            {
                foreach (var reportItem in reportLst)
                {
                    var report = _context.BC01Model.Where(p => p.BC01Id == reportItem).FirstOrDefault();
                    if (report != null)
                    {
                        //STT (Mức độ ưu tiên): lưu cho tất cả các lệnh cùng LSX
                        report.STT = viewModel.STT;
                        //Các thông tin còn lại: chỉ lưu theo thông tin công đoạn thực hiện
                        if (report.StockCode == viewModel.StockCode)
                        {
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
                            if (report.FinishDate.HasValue && report.FinishDateChange.HasValue)
                            {
                                report.NumberDaysDelay = Convert.ToDecimal((report.FinishDateChange.Value - report.FinishDate.Value).TotalDays);
                            }
                            //if (string.IsNullOrEmpty(config.FromDate) && string.IsNullOrEmpty(config.ToDate) && viewModel.EndDate.HasValue && viewModel.StartDate.HasValue)
                            //{
                            //    report.NumberDaysDelay = Convert.ToDecimal((report.FinishDateChange.Value - report.FinishDate.Value).TotalDays);
                            //}
                            //Nhóm lỗi
                            report.ErrorGroup = viewModel.ErrorGroup;
                            //Mô tả chi tiết lỗi 
                            report.ErrorDetail = viewModel.ErrorDetail;
                        }

                        report.LastEditTime = DateTime.Now;
                        report.LastEditBy = CurrentAccountId;

                        _context.Entry(report).State = EntityState.Modified;

                        _context.SaveChanges();
                    }
                }
            }

        }

        public void CreateDataBC01(BC01ReportFormViewModel viewModel, Guid? CurrentAccountId)
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
            List<BC01ReportViewModel> resultList = _context.Database.SqlQuery<BC01ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
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
