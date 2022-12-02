using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using ISD.ViewModels.Work;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class ProductionManagementRepository
    {
        private EntityDataContext _context;
        public ProductionManagementRepository(EntityDataContext context)
        {
            _context = context;
        }
        #region Tìm kiếm
        public IQueryable<ThucThiLenhSanXuatViewModel> Search(TaskSearchViewModel searchModel)
        {
            IQueryable<ThucThiLenhSanXuatViewModel> listTask = (
                            //TTLSX
                            from t in _context.ThucThiLenhSanXuatModel
                                //Product
                            join p in _context.ProductModel on t.ProductId equals p.ProductId into cTable
                            from pro in cTable.DefaultIfEmpty()
                                //WorkFlow
                                //join w in _context.WorkFlowModel on t.WorkFlowId equals w.WorkFlowId
                                //LSX SAP
                            join tm in _context.TaskModel on t.ParentTaskId equals tm.TaskId
                            //Công đoạn
                            join routing in _context.RoutingModel on t.Property6 equals routing.StepCode into rG
                            from r in rG.DefaultIfEmpty()
                                //Người tạo
                            join createby in _context.AccountModel on t.CreateBy equals createby.AccountId
                            where
                            //(searchModel.PriorityCode == null || t.PriorityCode.Contains(searchModel.PriorityCode))
                            //LSX ĐT
                            (searchModel.ProductionOrder == null || tm.Property5.Contains(searchModel.ProductionOrder))
                            //Tìm theo TTLSX
                            && (searchModel.Summary == null || t.Summary.Contains(searchModel.Summary))
                            //Tìm theo trạng thái
                            && (searchModel.Actived == null || t.Actived == searchModel.Actived)
                            //Tìm theo Product
                            && (searchModel.ProductName == null || pro.ProductName == searchModel.ProductName)
                            //Tìm theo Mã sản phẩm
                            && (searchModel.ProductCode == null || pro.ProductCode == searchModel.ProductCode)
                            //Tìm theo tổ
                            && (searchModel.Team == null || true)
                            //Loại TTLSX
                            //&& w.WorkFlowCode == ConstWorkFlow.TTLSX
                            orderby t.CreateTime descending
                            select new ThucThiLenhSanXuatViewModel
                            {
                                TaskId = t.TaskId,
                                Summary = tm.Summary,
                                ProductId = t.ProductId,
                                ProductName = pro.ProductName,
                                ProductCode = pro.ProductCode,
                                WorkFlowId = t.WorkFlowId,
                                //WorkFlowName = w.WorkFlowName,
                                Qty = t.Qty,
                                Actived = t.Actived,
                                Property1 = tm.Property1,
                                Property2 = tm.Property2,
                                Property5 = tm.Property5,
                                Barcode = t.Barcode,
                                LSXDT_Summary = tm.Property5,
                                StartDate = t.StartDate,
                                CreateBy = t.CreateBy,
                                CreateByName = !string.IsNullOrEmpty(createby.FullName) ? createby.FullName : createby.UserName,
                                CreateTime = t.CreateTime,
                                EstimateEndDate = t.EstimateEndDate,
                                StepCode = t.Property6,
                                StepName = r.StepName,
                            });

            //group new { t, ts, w, pro, tm } by new
            //{
            //    //TaskId TTLSX
            //    t.TaskId,
            //    //LSX SAP
            //    tm.Summary,
            //    //Sản phẩm
            //    t.ProductId,
            //    pro.ProductName,
            //    pro.ProductCode,
            //    //WorkFlow
            //    t.WorkFlowId,
            //    w.WorkFlowName,
            //    //SLKH
            //    t.Qty,
            //    t.Actived,
            //    //SO Number
            //    tm.Property1,
            //    //SO Line
            //    tm.Property2,
            //    //QR Code
            //    t.Barcode,
            //    //LSX ĐT
            //    tm.Property5,
            //    //Thời gian tạo
            //    t.CreateTime,
            //    t.CreateBy,
            //    //Ngày bắt đầu
            //    t.StartDate,
            //    //Ngày KT dự kiến
            //    t.EstimateEndDate
            //} into g
            //select new TaskViewModel
            //{
            //    TaskId = g.Key.TaskId,
            //    Summary = g.Key.Summary,
            //    ProductId = g.Key.ProductId,
            //    ProductName = g.Key.ProductName,
            //    ProductCode = g.Key.ProductCode,
            //    WorkFlowId = g.Key.WorkFlowId,
            //    WorkFlowName = g.Key.WorkFlowName,
            //    Qty = g.Key.Qty,
            //    Actived = g.Key.Actived,
            //    Property1 = g.Key.Property1,
            //    Property2 = g.Key.Property2,
            //    Property5 = g.Key.Property5,
            //    Barcode = g.Key.Barcode,
            //    LSXDT_Summary = g.Key.Property5,
            //    StartDate = g.Key.StartDate,
            //    CreateBy = g.Key.CreateBy,
            //    CreateTime = g.Key.CreateTime,
            //    EstimateEndDate = g.Key.EstimateEndDate
            //});
            return listTask;
        }

        public List<ThucThiLenhSanXuatViewModel> SearchQueryProc(ProductionManagementViewModel searchModel, out int filteredResultsCount)
        {
            string sqlQuery = "EXEC [MES].[usp_SearchTTLSX] @LSXDT, @LSXSAP, @ProductCode, @ProductName, @StepCode, @ToStockCode, @CreateBy, @CreatedFromDate , @CreatedToDate ,  @PageSize, @PageNumber, @FilteredResultsCount OUT, @CompanyId";

            var FilteredResultsCountOutParam = new SqlParameter();
            FilteredResultsCountOutParam.ParameterName = "FilteredResultsCount";
            FilteredResultsCountOutParam.SqlDbType = SqlDbType.Int;
            FilteredResultsCountOutParam.Direction = ParameterDirection.Output;

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXDT",
                    Value = searchModel.LSXDT ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    Value = searchModel.LSXSAP ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductCode",
                    Value = searchModel.ProductCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductName",
                    Value = searchModel.ProductName ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StepCode",
                    Value = searchModel.StepCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ToStockCode",
                    Value = searchModel.ToStockCode ?? (object)DBNull.Value
                }, new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateBy",
                    Value = searchModel.CreateBy ?? (object)DBNull.Value
                },
                 new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreatedFromDate",
                    Value = searchModel.CreatedFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreatedToDate",
                    Value = searchModel.CreatedToDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageSize",
                    Value = searchModel.PageSize ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageNumber",
                    Value = searchModel.PageNumber ?? (object)DBNull.Value
                },
                FilteredResultsCountOutParam,
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompanyId",
                    Value = searchModel.CompanyId ?? (object)DBNull.Value
                },
            };
            #endregion

            var result = _context.Database.SqlQuery<ThucThiLenhSanXuatViewModel>(sqlQuery, parameters.ToArray()).ToList();
            var filteredResultsCountValue = FilteredResultsCountOutParam.Value;
            if (filteredResultsCountValue != null && filteredResultsCountValue != DBNull.Value)
            {
                filteredResultsCount = Convert.ToInt32(filteredResultsCountValue);
            }
            else
            {
                filteredResultsCount = 0;
            }
            return result;
        }

        public List<TaskViewModel> SearchProductionOrderQueryProc(TaskSearchViewModel searchModel, out int filteredResultsCount)
        {
            string sqlQuery = "EXEC [MES].[usp_SearchLSXSAP] @LSXDT, @LSXSAP, @ProductCode, @ProductName, @PageSize, @PageNumber, @FilteredResultsCount OUT, @CompanyId";

            var FilteredResultsCountOutParam = new SqlParameter();
            FilteredResultsCountOutParam.ParameterName = "FilteredResultsCount";
            FilteredResultsCountOutParam.SqlDbType = SqlDbType.Int;
            FilteredResultsCountOutParam.Direction = ParameterDirection.Output;

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXDT",
                    Value = searchModel.PPO_LSXDT ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    Value = searchModel.PPO_LSXSAP ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductCode",
                    Value = searchModel.PPO_ProductCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductName",
                    Value = searchModel.PPO_ProductCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageSize",
                    Value = searchModel.PageSize ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageNumber",
                    Value = searchModel.PageNumber ?? (object)DBNull.Value
                },
                FilteredResultsCountOutParam,
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompanyId",
                    Value = searchModel.CompanyId ?? (object)DBNull.Value,
                },
            };
            #endregion

            var result = _context.Database.SqlQuery<TaskViewModel>(sqlQuery, parameters.ToArray()).ToList();
            var filteredResultsCountValue = FilteredResultsCountOutParam.Value;
            if (filteredResultsCountValue != null && filteredResultsCountValue != DBNull.Value)
            {
                filteredResultsCount = Convert.ToInt32(filteredResultsCountValue);
            }
            else
            {
                filteredResultsCount = 0;
            }
            return result;
        }
        #endregion

        #region Tạo đợt
        public DivisionOfTaskViewModel DevisionOfTask(Guid Id)
        {
            var listTask = (from t in _context.TaskModel
                                //Task Status
                            join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                            join pTemp in _context.ProductModel on t.ProductId equals pTemp.ProductId into pList
                            from p in pList.DefaultIfEmpty()
                            where t.TaskId == Id
                            select new DivisionOfTaskViewModel
                            {
                                TaskId = t.TaskId,
                                Qty = t.Qty,
                                Summary = t.Summary,
                                TaskStatusId = t.TaskStatusId,
                                ProductId = t.ProductId,
                                ProductCode = p.ProductCode,
                                ProductName = p.ProductName,
                                WorkFlowId = t.WorkFlowId,
                                Unit = t.Unit,
                                Property3 = t.Property3,
                                StartDate = t.StartDate,
                                EstimateEndDate = t.EstimateEndDate,
                                TaskStatusName = s.TaskStatusName,
                                Property5 = t.Property5,
                                StartDate_LSXDT = null,
                                EstimateEndDate_LSXDT = null,
                                MaterialsRequired = 0,
                                MaterialsReceived = 0,
                                ActualNumber = 0,
                            }).FirstOrDefault();
            if (listTask != null)
            {
                var LSXDT = (from pro in _context.ProductionOrderModel
                             join t in _context.TaskModel on pro.ProductionOrderId equals t.TaskId
                             where pro.ZZLSX == listTask.Property3
                             select new
                             {
                                 t.StartDate,
                                 t.EstimateEndDate
                             }).ToList();
                listTask.StartDate_LSXDT = LSXDT.Min(x => x.StartDate);
                listTask.EstimateEndDate_LSXDT = LSXDT.Max(x => x.EstimateEndDate);
            }
            return listTask;
        }
        /// <summary>
        /// Lấy danh sách công đoạn
        /// </summary>
        /// <param name="ProductCode">Mã sản phẩm</param>
        /// <param name="ProductAttributes">Mã chi tiết</param>
        /// <returns></returns>
        public IEnumerable<RoutingInventorViewModel> LoadRoutingOf(string ProductCode, string ProductAttributes)
        {
            ConfigRepository config = new ConfigRepository(_context);
            var checkinDistance = config.GetBy("GetFullCongDoanCon");
            if (checkinDistance == "1")
            {
                ProductAttributes = null;
            }
            IQueryable<RoutingInventorViewModel> listStep = (from ri in _context.RoutingInventorModel
                                                             join r in _context.RoutingModel on ri.ARBPL_SUB equals r.StepCode
                                                             where ri.MATNR == ProductCode &&
                                                             (ProductAttributes == null || ri.ITMNO == ProductAttributes)
                                                             orderby r.OrderIndex ascending
                                                             group new { ri } by new { ri.ARBPL_SUB, ri.LTXA1 } into g
                                                             select new RoutingInventorViewModel
                                                             {
                                                                 ARBPL_SUB = g.Key.ARBPL_SUB,
                                                                 LTXA1 = g.Key.LTXA1
                                                             });
            return listStep;
        }

        public DivisionOfTaskViewModel DevisionOfTaskBy(string Summary)
        {
            var listTask = (from t in _context.TaskModel
                                //Task Status
                            join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                            join pTemp in _context.ProductModel on t.ProductId equals pTemp.ProductId into pList
                            from p in pList.DefaultIfEmpty()
                            where t.Property5 == Summary
                            select new DivisionOfTaskViewModel
                            {
                                TaskId = t.TaskId,
                                Qty = t.Qty,
                                Summary = t.Summary,
                                TaskStatusId = t.TaskStatusId,
                                ProductId = t.ProductId,
                                ProductCode = p.ProductCode,
                                ProductName = p.ProductName,
                                WorkFlowId = t.WorkFlowId,
                                Unit = t.Unit,
                                Property3 = t.Property3,
                                Property4 = t.Property4,
                                Property5 = t.Property5,
                                StartDate = t.StartDate,
                                EstimateEndDate = t.EstimateEndDate,
                                TaskStatusName = s.TaskStatusName,
                                StartDate_LSXDT = null,
                                EstimateEndDate_LSXDT = null,
                                MaterialsRequired = 0,
                                MaterialsReceived = 0,
                                ActualNumber = 0,
                            }).FirstOrDefault();
            if (listTask != null)
            {
                var LSXDT = (from pro in _context.ProductionOrderModel
                             join t in _context.TaskModel on pro.ProductionOrderId equals t.TaskId
                             where pro.ZZLSX == listTask.Property3
                             select new
                             {
                                 t.StartDate,
                                 t.EstimateEndDate
                             }).ToList();
                listTask.StartDate_LSXDT = LSXDT.Min(x => x.StartDate);
                listTask.EstimateEndDate_LSXDT = LSXDT.Max(x => x.EstimateEndDate);
            }
            return listTask;
        }
        public List<DivisionOfTaskViewModel> DevisionOfTaskDetail(Guid Id)
        {
            var listTask = (from t in _context.TaskModel
                            join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                            where t.ParentTaskId == Id
                            select new DivisionOfTaskViewModel
                            {
                                TaskId = t.TaskId,
                                Qty = t.Qty,
                                Summary = t.Summary,
                                TaskStatusId = t.TaskStatusId,
                                ProductId = t.ProductId,
                                WorkFlowId = t.WorkFlowId,
                                Unit = t.Unit,
                                StartDate = t.StartDate,
                                EstimateEndDate = t.EstimateEndDate,
                                TaskStatusName = s.TaskStatusName
                            }).ToList();
            return listTask;
        }

        public List<DivisionOfTaskByLSXDTViewModel> GetDevisionOfTaskDetailByLSXDT(string LSXDT)
        {
            //Lấy danh sách đợt theo LSX ĐT
            var listTask = (from t in _context.TaskModel
                            join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                            join w in _context.WorkFlowModel on t.WorkFlowId equals w.WorkFlowId
                            where t.Property5 == LSXDT
                            //Loại là đợt sản xuất
                            && w.WorkFlowCode == ConstWorkFlow.LSXD
                            orderby t.Number1
                            select new DivisionOfTaskByLSXDTViewModel
                            {
                                DotId = t.TaskId,
                                DotName = t.Summary,
                                STTDot = (int)t.Number1,
                                Date1 = t.Date1,
                                Date2 = t.Date2,
                                Date3 = t.Date3,
                                Date4 = t.Date4,
                                Date5 = t.Date5,
                                Date6 = t.Date6,
                                Date7 = t.Date7,
                                Date8 = t.Date8,
                                Date9 = t.Date9,
                                Property3 = t.Property3,
                                Property4 = t.Property4,
                                Property5 = t.Property5,
                                Property6 = t.Property6,
                                Property7 = t.Property7,
                            }).ToList();
            if (listTask != null && listTask.Count > 0)
            {
                foreach (var item in listTask)
                {
                    //Lấy danh sách LSX SAP theo đợt
                    var LSXSAPList = (from t in _context.TaskModel
                                      join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                                      join w in _context.WorkFlowModel on t.WorkFlowId equals w.WorkFlowId
                                      where t.ParentTaskId == item.DotId
                                      //Loại là LSX SAP
                                      && w.WorkFlowCode == ConstWorkFlow.LSXC
                                      select new DivisionOfTaskByLSXDTSubtaskViewModel
                                      {
                                          STTDot = item.STTDot,
                                          LSXSAPId = t.TaskId,
                                          //LSX SAP
                                          LSXSAP = t.Summary,
                                          //BĐ Dự kiến
                                          StartDate = t.StartDate,
                                          //BĐ Điều chỉnh
                                          Date1 = t.Date1,
                                          //KT Dự kiến
                                          EstimateEndDate = t.EstimateEndDate,
                                          //KT Điều chỉnh
                                          ReceiveDate = t.ReceiveDate,
                                          //SL KH
                                          Qty = t.Qty,
                                          //SL ĐC
                                          Number2 = (int)t.Number2,
                                          //Trạng thái
                                          TaskStatusId = t.TaskStatusId,
                                          TaskStatusName = s.TaskStatusName,
                                          Property1 = t.Property1,
                                          Property2 = t.Property2,
                                          Property3 = t.Property3,
                                          Property4 = t.Property4,
                                          Property5 = t.Property5,
                                          ProductId = t.ProductId,
                                          Unit = t.Unit,
                                      }).OrderBy(p => p.LSXSAP).ToList();
                    if (LSXSAPList != null && LSXSAPList.Count > 0)
                    {
                        item.LSXSAPList = new List<DivisionOfTaskByLSXDTSubtaskViewModel>();
                        item.LSXSAPList = LSXSAPList;
                    }
                }
            }
            return listTask;
        }

        public List<DivisionOfTaskByLSXDTViewModel> GetDevisionOfTaskStepByLSXDT(string LSXDT)
        {
            //Lấy danh sách đợt theo LSX ĐT
            var listTask = (from t in _context.TaskModel
                            join s in _context.TaskStatusModel on t.TaskStatusId equals s.TaskStatusId
                            join w in _context.WorkFlowModel on t.WorkFlowId equals w.WorkFlowId
                            where t.Property5 == LSXDT
                            //Loại là đợt sản xuất
                            && w.WorkFlowCode == ConstWorkFlow.LSXD
                            orderby t.Number1
                            select new DivisionOfTaskByLSXDTViewModel
                            {
                                DotId = t.TaskId,
                                DotName = t.Summary,
                                STTDot = (int)t.Number1,
                                //Công đoạn thực hiện
                                Property7 = t.Property7,
                            }).ToList();
            //Lấy danh sách công đoạn thực hiện đã lưu
            if (listTask != null && listTask.Count > 0)
            {
                foreach (var item in listTask)
                {
                    if (!string.IsNullOrEmpty(item.Property7))
                    {
                        item.StepList = new List<string>();
                        item.StepList = item.Property7.Split(',').ToList();
                    }
                }
            }

            return listTask;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskId">Mã thực thi</param>
        /// <param name="Barcode">QR Code</param>
        /// <returns></returns>
        #region Lấy thông tin chi tiết của TTLSX bằng Barcode
        public ProductionManagementViewModel GetExecutionTaskByTaskId(Guid? TaskId)
        {
            var Task = (from t in _context.ThucThiLenhSanXuatModel
                            //HangTag
                        join hTemp in _context.HangTagModel on t.Barcode equals hTemp.HangTagId into hList
                        from h in hList.DefaultIfEmpty()

                            //Sản phẩm
                        join pTemp in _context.ProductModel on t.ProductId equals pTemp.ProductId into pList
                        from p in pList.DefaultIfEmpty()
                            //Công đoạn hoàn tất
                        join sTemp in _context.StockModel on t.StockId equals sTemp.StockId into sList
                        from s in sList.DefaultIfEmpty()
                            //Routing
                        join rTemp in _context.RoutingModel on t.Property6 equals rTemp.StepCode into rList
                        from r in rList.DefaultIfEmpty()
                            //Công đoạn kế tiếp
                        join tsTemp in _context.StockModel on t.ToStockId equals tsTemp.StockId into tsList
                        from ts in tsList.DefaultIfEmpty()
                            //Create By
                        join a in _context.AccountModel on t.CreateBy equals a.AccountId
                        join btemp in _context.AccountModel on t.LastEditBy equals btemp.AccountId into list1
                        from b in list1.DefaultIfEmpty()
                        where t.TaskId == TaskId
                        select new ProductionManagementViewModel
                        {
                            TaskId = t.TaskId,
                            //WorkFlowId = t.WorkFlowId,
                            ParentTaskId = t.ParentTaskId,
                            Unit = t.Unit,
                            //WorkShop = "Phôi",
                            //LoaiGiaoDich = "M02 - Nhập chi tiết sau gia công",
                            StepCode = s.StockCode,
                            StepName = s.StockName,
                            StepId = s.StockId,
                            Barcode = t.Barcode,
                            ProductionOrder_StartDate = t.StartDate,
                            ProductionOrder_EstimateEndDate = t.EstimateEndDate,
                            WorkDate = t.StartDate,
                            Qty = t.Qty,
                            ProductCode = p.ERPProductCode,
                            ProductName = p.ProductName,
                            ProductAttributes = h.ProductAttribute,
                            ProductAttributesQty = t.ProductAttributesQty,
                            ProductId = t.ProductId,
                            ProductionOrder_SAP = t.Summary,
                            CreateByFullName = a.FullName,
                            CreateTime = t.CreateTime,
                            LastEditByFullName = b.FullName,
                            LastEditTime = t.LastEditTime,
                            Actived = t.Actived,
                            ToStockCode = t.ToStockCode,
                            ToStockName = ts.StockName,
                            IsWorkCenterCompleted = t.IsWorkCenterCompleted,
                            WorkCenterConfirmTime = t.WorkCenterConfirmTime,
                            ConfirmWorkCenter = r.WorkCenter,
                            Property1 = t.Property1,
                            Property2 = t.Property2
                        }).FirstOrDefault();

            if (Task != null)
            {
                var data = _context.TaskModel.Where(x => x.TaskId == Task.ParentTaskId).FirstOrDefault();
                if (data != null)
                {
                    Task.ProductionOrder = data.Property5;
                    var Dot = _context.TaskModel.Where(x => x.TaskId == data.ParentTaskId).FirstOrDefault();
                    if (Dot != null)
                    {
                        Task.Summary = Dot.Summary;

                    }
                }
                //RoutingInventer
                var routing = _context.RoutingInventorModel.Where(x => x.ITMNO == Task.ProductAttributes && x.MATNR == Task.ProductCode).FirstOrDefault();

                if (routing != null)
                {
                    Task.ProductAttributesQty = routing.BMSCH * (Task.Qty ?? 0);

                    Task.ProductAttributesName = routing.KTEXT;
                    string sqlQueryView_BOM_Inventor_Rip = "SELECT * FROM MES.View_BOM_Inventor_Rip WHERE MATNR = '" + routing.MATNR + "' AND PART_ID = '" + routing.ITMNO + "' ";
                    var bom = _context.Database.SqlQuery<View_BOM_Inventor_RipViewModel>(sqlQueryView_BOM_Inventor_Rip).FirstOrDefault();
                    if (bom != null)
                    {
                        Task.POT12 = bom.POT12;

                    }
                    Task.ProductAttributesUnit = routing.BMEIN;
                }

                //Lấy thông tin chi tiết đã làm
                //Số lượng confirm: trên 1 pallet lấy số lượng phase lớn nhất
                var QtyD = (from sR in _context.StockReceivingDetailModel
                                //Product
                            join pTemp in _context.ProductModel on sR.ProductId equals pTemp.ProductId into pList
                            from p in pList.DefaultIfEmpty()
                                //Stock
                            join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                            from s in sList.DefaultIfEmpty()
                                //ttlsx
                            join ttlsx in _context.ThucThiLenhSanXuatModel on sR.CustomerReference equals ttlsx.TaskId
                            where
                            //Sản phẩm
                            p.ProductCode == Task.ProductCode
                            //Mã Chi tiết
                            && sR.ProductAttributes == Task.ProductAttributes
                            && s.StockCode == Task.StepCode
                            && sR.StockRecevingType == "D"
                            && ttlsx.ParentTaskId == Task.ParentTaskId
                            && ttlsx.TaskId == Task.TaskId
                            group new { sR } by new { sR.Phase } into g
                            select new
                            {
                                Phase = g.Key.Phase,
                                Quantity = g.Sum(x => x.sR.Quantity)
                            }).OrderByDescending(x => x.Phase).FirstOrDefault();
                Task.ProductAttributesQtyD = QtyD?.Quantity ?? 0;
                Task.Phase = QtyD?.Phase;
            }
            return Task;
        }
        #endregion

        #region Lấy thông tin TTLSX cho chuyển công đoạn
        /// <summary>
        /// Lấy thông tin TTLSX cho chuyển công đoạn
        /// </summary>
        /// <param name="TaskId">ID TTLSX</param>
        /// <returns></returns>
        public SwitchingStagesViewModel GetTTLSXForSwitchingStageByTaskId(Guid? TaskId)
        {
            var Task = (from t in _context.ThucThiLenhSanXuatModel
                            //HangTag
                        join hTemp in _context.HangTagModel on t.Barcode equals hTemp.HangTagId into hList
                        from h in hList.DefaultIfEmpty()
                            //Sản phẩm
                        join pTemp in _context.ProductModel on t.ProductId equals pTemp.ProductId into pList
                        from p in pList.DefaultIfEmpty()

                            //Công đoạn hoàn tất
                        join sTemp in _context.StockModel on t.StockId equals sTemp.StockId into sList
                        from s in sList.DefaultIfEmpty()
                            //Công đoạn kế tiếp
                        join tsTemp in _context.StockModel on t.ToStockId equals tsTemp.StockId into tsList
                        from ts in tsList.DefaultIfEmpty()
                            //Create By
                        join a in _context.AccountModel on t.CreateBy equals a.AccountId
                        join btemp in _context.AccountModel on t.LastEditBy equals btemp.AccountId into list1
                        from b in list1.DefaultIfEmpty()
                        where t.TaskId == TaskId
                        select new SwitchingStagesViewModel
                        {
                            TaskId = t.TaskId,
                            //WorkFlowId = t.WorkFlowId,
                            ParentTaskId = t.ParentTaskId,
                            Unit = t.Unit,
                            //WorkShop = "Phôi",
                            //LoaiGiaoDich = "M02 - Nhập chi tiết sau gia công",
                            FromStepCode = s.StockCode,
                            FromStepName = s.StockName,
                            ProductAttributes = h.ProductAttribute,
                            ProductAttributesQty = t.ProductAttributesQty,
                            //Barcode = t.Barcode,
                            WorkDate = t.StartDate,
                            Qty = t.Qty,
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName,
                            ProductId = t.ProductId,
                            ProductionOrder_SAP = t.Summary,
                            CreateByFullName = a.FullName,
                            CreateTime = t.CreateTime,
                            LastEditByFullName = b.FullName,
                            LastEditTime = t.LastEditTime,
                            Actived = t.Actived,
                            ToStepCode = t.ToStockCode,
                            ToStepName = ts.StockName,
                        }).FirstOrDefault();

            if (Task != null)
            {
                var data = _context.TaskModel.Where(x => x.TaskId == Task.ParentTaskId).FirstOrDefault();
                if (data != null)
                {
                    Task.ProductionOrder = data.Property5;
                    var Dot = _context.TaskModel.Where(x => x.TaskId == data.ParentTaskId).FirstOrDefault();
                    if (Dot != null)
                    {
                        Task.Summary = Dot.Summary;

                    }
                }
                //RoutingInventer
                var routing = _context.RoutingInventorModel.Where(x => x.ITMNO == Task.ProductAttributes && x.MATNR == Task.ProductCode).FirstOrDefault();

                if (routing != null)
                {
                    Task.ProductAttributesQty = routing.BMSCH * (Task.Qty ?? 0);

                    Task.ProductAttributesName = routing.KTEXT;
                    string sqlQueryView_BOM_Inventor_Rip = "SELECT * FROM MES.View_BOM_Inventor_Rip WHERE MATNR = '" + routing.MATNR + "' AND PART_ID = '" + routing.ITMNO + "' ";
                    var bom = _context.Database.SqlQuery<View_BOM_Inventor_RipViewModel>(sqlQueryView_BOM_Inventor_Rip).FirstOrDefault();
                    if (bom != null)
                    {
                        Task.POT12 = bom.POT12;

                    }
                    Task.ProductAttributesUnit = routing.BMEIN;
                }

                //Lấy thông tin chi tiết đã làm
                var QtyD = (from sR in _context.StockReceivingDetailModel
                                //Product
                            join pTemp in _context.ProductModel on sR.ProductId equals pTemp.ProductId into pList
                            from p in pList.DefaultIfEmpty()
                                //Stock
                            join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                            from s in sList.DefaultIfEmpty()
                            where
                            //Sản phẩm
                            p.ProductCode == Task.ProductCode
                            //Mã Chi tiết
                            && sR.ProductAttributes == Task.ProductAttributes
                            && s.StockCode == Task.FromStepCode
                            && sR.StockRecevingType == "D"
                            group new { sR } by new { sR.Phase } into g
                            select new
                            {
                                Phase = g.Key.Phase,
                                Quantity = g.Sum(x => x.sR.Quantity)
                            }).OrderByDescending(x => x.Phase).FirstOrDefault();
                Task.ProductAttributesQtyD = QtyD?.Quantity ?? 0;
            }
            return Task;
        }
        public List<SwitchingStagesViewModel> GetTTLSXForSwitchingStageHistoryByTTLSX(Guid? TaskId)
        {
            ProductionManagementRepository _productionManagementRepo = new ProductionManagementRepository(_context);

            var TTLSX = _productionManagementRepo.GetExecutionTaskByTaskId(TaskId);


            // Tìm thấy TTLSX
            if (TTLSX != null)
            {
                ////Đã chuyển công đoạn kế tiếp
                ////Xem dữ liệu đã chuyển
                //if (!string.IsNullOrEmpty(TTLSX.ToStockCode))
                //{
                //    object[] SqlParams =
                //    {
                //        new SqlParameter("@TaskId",TaskId),
                //    };

                //    var res = _context.Database.SqlQuery<SwitchingStagesViewModel>("[MES].[GetProductionDetailTranfered] @TaskId", SqlParams).ToList();
                //    return res;
                //}
                //// Chưa chuyển công đoạn
                //// Xem dữ liệu ghi nhận
                //else
                //{
                //    object[] SqlParams =
                //    {
                //        new SqlParameter("@TaskId",TaskId),
                //        new SqlParameter("@StockId",TTLSX.StepId),
                //    };

                //    var res = _context.Database.SqlQuery<SwitchingStagesViewModel>("[MES].[GetProductDetailsHistoryForSwitchStageByTTLSX] @TaskId, @StockId", SqlParams).ToList();
                //    return res;
                //}

                //Xem dữ liệu ghi nhận
                object[] SqlParams =
                    {
                        new SqlParameter("@TaskId",TaskId),
                        new SqlParameter("@StockId",TTLSX.StepId),
                    };

                var res = _context.Database.SqlQuery<SwitchingStagesViewModel>("[MES].[GetProductDetailsHistoryForSwitchStageByTTLSX] @TaskId, @StockId", SqlParams).ToList();
                return res;

            }
            return null;

        }

        #endregion

        #region Lấy thông tin chi tiết của thực thi lệnh sản xuất
        //886AC019-BEBD-4685-A8FC-71E8E3C1F006
        public Guid? GetTTLSXByBarcode(Guid? Barcode)
        {
            var Task = (from t in _context.ThucThiLenhSanXuatModel
                        where t.Barcode == Barcode
                        select t.TaskId).FirstOrDefault();
            return Task;
        }
        #endregion

        #region Thêm mới thực thi lệnh sản xuất
        //public ProductionManagementViewModel CreateNewTask(Guid ParentTaskId, Guid? Barcode)
        //{

        //    var Task = new ProductionManagementViewModel();
        //    try
        //    {
        //        var LSXSAP = (from t in _context.TaskModel
        //                      where t.TaskId == ParentTaskId
        //                      select t).FirstOrDefault();
        //        if (LSXSAP != null)
        //        {
        //            TaskModel newTask = new TaskModel();
        //            newTask.TaskId = Guid.NewGuid();
        //            newTask.Summary = LSXSAP.Summary;
        //            newTask.PriorityCode = LSXSAP.PriorityCode;
        //            //Loại là TTLSX
        //            var workFlow = _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.TTLSX).FirstOrDefault();
        //            if (workFlow != null)
        //            {
        //                newTask.WorkFlowId = workFlow.WorkFlowId;
        //                //lấy trạng thái theo loại
        //                var TaskStatusId = _context.TaskStatusModel.Where(p => p.WorkFlowId == workFlow.WorkFlowId).OrderBy(p => p.OrderIndex).Select(p => p.TaskStatusId).FirstOrDefault();
        //                newTask.TaskStatusId = TaskStatusId;
        //            }

        //            newTask.StartDate = LSXSAP.StartDate;
        //            newTask.EstimateEndDate = LSXSAP.EstimateEndDate;
        //            newTask.CompanyId = LSXSAP.CompanyId;
        //            newTask.StoreId = LSXSAP.StoreId;
        //            newTask.CreateBy = LSXSAP.CreateBy;
        //            newTask.CreateTime = DateTime.Now;
        //            newTask.Actived = true;
        //            newTask.Property1 = LSXSAP.Property1;
        //            newTask.Property2 = LSXSAP.Property2;
        //            newTask.Property3 = LSXSAP.Property3;
        //            newTask.Property4 = LSXSAP.Property4;
        //            newTask.Property5 = LSXSAP.Property5;
        //            newTask.Property6 = LSXSAP.Property6;
        //            newTask.ParentTaskId = ParentTaskId;
        //            newTask.ProductId = LSXSAP.ProductId;
        //            newTask.Qty = LSXSAP.Qty;
        //            newTask.Unit = LSXSAP.Unit;
        //            //newTask.Barcode = Barcode;

        //            //Cập nhật TaskCode cho subtask
        //            int index = 0;
        //            var lastSubtask = _context.TaskModel.Where(p => p.ParentTaskId == ParentTaskId).Count();
        //            index = lastSubtask + 1;
        //            string subtaskCode = string.Format("{0}-{1}", LSXSAP.TaskCode, index);
        //            newTask.SubtaskCode = subtaskCode;

        //            _context.Entry(newTask).State = System.Data.Entity.EntityState.Added;
        //            _context.SaveChanges();

        //            Task = GetExecutionTaskByTaskId(newTask.TaskId);
        //        }

        //    }
        //    catch
        //    {

        //    }

        //    return Task;
        //}

        public Guid? CreateNewExecutionTask(Guid ParentTaskId, Guid? Barcode, Guid? CurrentUserId)
        {
            try
            {
                var LSXSAP = (from t in _context.TaskModel
                              where t.TaskId == ParentTaskId
                              select t).FirstOrDefault();
                if (LSXSAP != null)
                {
                    ThucThiLenhSanXuatModel newTask = new ThucThiLenhSanXuatModel();
                    newTask.TaskId = Guid.NewGuid();
                    newTask.Summary = LSXSAP.Summary;
                    //Loại là TTLSX
                    var workFlow = _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.TTLSX).FirstOrDefault();
                    if (workFlow != null)
                    {
                        newTask.WorkFlowId = workFlow.WorkFlowId;
                        //lấy trạng thái theo loại
                        var TaskStatusId = _context.TaskStatusModel.Where(p => p.WorkFlowId == workFlow.WorkFlowId).OrderBy(p => p.OrderIndex).Select(p => p.TaskStatusId).FirstOrDefault();
                        newTask.TaskStatusId = TaskStatusId;
                    }

                    newTask.StartDate = LSXSAP.StartDate;
                    newTask.EstimateEndDate = LSXSAP.EstimateEndDate;
                    newTask.CreateBy = CurrentUserId;
                    newTask.CreateTime = DateTime.Now;
                    newTask.Actived = true;
                    //SO Number
                    newTask.Property1 = LSXSAP.Property1;
                    //SO Line
                    newTask.Property2 = LSXSAP.Property2;

                    //Người theo dõi + LSXĐT
                    newTask.Property3 = LSXSAP.Property3;
                    //Người theo dõi
                    newTask.Property4 = LSXSAP.Property4;
                    //LSX ĐT
                    newTask.Property5 = LSXSAP.Property5;
                    //Công đoạn nhỏ
                    newTask.Property6 = LSXSAP.Property6;
                    newTask.ParentTaskId = ParentTaskId;
                    newTask.ProductId = LSXSAP.ProductId;
                    newTask.Qty = LSXSAP.Qty;
                    if (LSXSAP.Number2 != null)
                    {
                        newTask.Qty = (int)LSXSAP.Number2;
                    }
                    newTask.Unit = LSXSAP.Unit;
                    newTask.Barcode = Barcode;

                    //Cập nhật TaskCode cho subtask
                    int index = 0;
                    var lastSubtask = _context.TaskModel.Where(p => p.ParentTaskId == ParentTaskId).Count();
                    index = lastSubtask + 1;
                    string subtaskCode = string.Format("{0}-{1}", LSXSAP.TaskCode, index);
                    newTask.SubtaskCode = subtaskCode;




                    _context.Entry(newTask).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                    return newTask.TaskId;
                }
                else
                {
                    return null;
                }

            }
            catch// (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region QR Code
        // Cập nhật QR Code
        public void UpdateBarCode(Guid? TaskId, Guid? BarCode)
        {
            //Lấy Task theo TaskId
            var task = _context.ThucThiLenhSanXuatModel.Where(p => p.TaskId == TaskId).FirstOrDefault();

            //Gán BarCode vào Task
            task.Barcode = BarCode;
            _context.Entry(task).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();

        }
        // Check QR Code Exists
        public bool QRCodeExits(Guid? BarCode)
        {
            //Tìm task với barcode
            var task = _context.ThucThiLenhSanXuatModel.Where(p => p.Barcode == BarCode);
            if (task.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BarCode">Mã QR Code</param>
        /// <param name="SAP"> LSX SAP </param>
        /// <returns></returns>
        public bool CheckHangTag(Guid? BarCode, Guid? SAP)
        {
            Guid QRCode = BarCode.Value;
            //Tìm task với barcode
            var task = _context.HangTagModel.Where(p => p.HangTagId == QRCode && p.CustomerReference == SAP);
            if (task.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region Lấy danh sách routing inventer
        /// <summary>
        /// Lấy thông tin công đoạn sản xuất của sản phẩm
        /// </summary>
        /// <param name="productionOrderViewModel.ProductCode"> MÃ sản phẩm </param>
        /// <param name="productionOrderViewModel.StepCode"> Mã Công đoạn Con </param>
        /// <returns>Danh sách ProductionOrderDetailViewModel</returns>
        public List<ProductionOrderDetailViewModel> GetRoutingInventer(string ProductCode, string StepCode = null)
        {
            //Lấy danh sách "Chi tiết" theo "Mã sản phẩm" và "Công đoạn con"
            var data = (from r in _context.RoutingInventorModel
                        where r.MATNR == ProductCode
                        && (string.IsNullOrEmpty(StepCode) || (r.ARBPL_SUB == StepCode || r.ITMNO == "-"))
                        select new ProductionOrderDetailViewModel
                        {
                            ProductAttributes = r.ITMNO,
                            KTEXT = r.KTEXT,
                            //StepCode = r.ARBPL_SUB,
                            //StepName = r.LTXA1
                        }).Distinct().ToList();

            //Nếu có dữ liệu 
            if (data != null)
            {
                //Lấy số lượng Đạt và Không đạt ( StockReceivingModel )
                foreach (var item in data)
                {
                    item.Quantity_DLD = (from sR in _context.StockReceivingDetailModel
                                             //Product
                                         join pTemp in _context.ProductModel on sR.ProductId equals pTemp.ProductId into pList
                                         from p in pList.DefaultIfEmpty()
                                             //Stock
                                         join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                                         from s in sList.DefaultIfEmpty()
                                         where
                                         //Sản phẩm
                                         p.ProductCode == ProductCode
                                         //Mã Chi tiết
                                         && sR.ProductAttributes == item.ProductAttributes
                                         && s.StockCode == item.StepCode
                                         && sR.StockRecevingType == "D"
                                         select sR.Quantity).Sum();
                    item.Quantity_DLKD = (from sR in _context.StockReceivingDetailModel
                                              //Product
                                          join pTemp in _context.ProductModel on sR.ProductId equals pTemp.ProductId into pList
                                          from p in pList.DefaultIfEmpty()
                                              //Stock
                                          join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                                          from s in sList.DefaultIfEmpty()
                                          where
                                          //Sản phẩm
                                          p.ProductCode == ProductCode
                                          //Mã Chi tiết
                                          && sR.ProductAttributes == item.ProductAttributes
                                          && s.StockCode == item.StepCode
                                          && sR.StockRecevingType == "KD"
                                          select sR.Quantity).Sum();

                    //Nếu là chi tiết/cụm
                    if (!string.IsNullOrEmpty(item.ProductAttributes))
                    {
                        //Set quy cách
                        var POT12 = (from p in _context.BOM_Item_InventorModel
                                     where !string.IsNullOrEmpty(p.POT21) &&
                                         p.MATNR == ProductCode &&
                                         p.PART_ID == item.ProductAttributes
                                     select p.POT12).FirstOrDefault();
                        item.POT12 = POT12;
                    }
                }
            }

            return data.OrderBy(x => x.ProductAttributes).ToList();
        }

        /// <summary>
        /// Kiểm tra Routing đã tồn tại hay chưa
        /// </summary>
        /// <param name="productionOrderDetailNew">Routing mới</param>
        /// <param name="productionOrderDetailOld">Routing cũ</param>
        /// <param name="TaskId">Mã Lệnh thực thi</param>
        /// <returns>Danh sách routing</returns>
        public List<ProductionOrderDetailViewModel> GetRoutingChecked(List<ProductionOrderDetailViewModel> productionOrderDetailNew, List<ProductionOrderDetailViewModel> productionOrderDetailOld, Guid TaskId)
        {
            List<ProductionOrderDetailViewModel> list = new List<ProductionOrderDetailViewModel>();
            List<ProductionOrderDetailViewModel> NewList = new List<ProductionOrderDetailViewModel>();
            //var ListProductionOrderDetail_Old = GetProductionOrderDetail(TaskId);

            //Lấy dữ liệu routing mới
            productionOrderDetailNew = productionOrderDetailNew.Where(x => !(x.Check != true)).ToList();

            //So sánh dữ liệu cũ và mới
            if (productionOrderDetailOld != null)
            {
                foreach (var item in productionOrderDetailNew)
                {
                    foreach (var x in productionOrderDetailOld)
                    {
                        //Nếu dữ liệu mới đã có tong dữ liệu cũ thì thêm vào List: "list"
                        if (item.ProductAttributes == x.ProductAttributes)
                        {
                            list.Add(item);

                        }
                    }
                }



                //Loại bỏ dữ liệu trùng giữa dữ liệu cũ và mới
                //add dữ liệu trong trùng vào dữ liệu cũ
                productionOrderDetailOld.AddRange(productionOrderDetailNew.Except(list).ToList());
                NewList = productionOrderDetailOld;
            }
            else
            {
                NewList = productionOrderDetailNew;

            }
            ////Lấy thông tin "Qty" và "Unit"
            //var TTKSX = _context.ThucThiLenhSanXuatModel.Where(x => x.TaskId == TaskId).Select(x => new { x.Qty, x.Unit }).FirstOrDefault();

            //foreach (var item in productionOrderDetailNew)
            //{
            //    item.Qty = TTKSX.Qty;
            //    item.Unit = TTKSX.Unit;
            //}

            return NewList.OrderBy(x => x.ProductAttributes).ToList();
        }

        /// <summary>
        /// XÓa row routing
        /// </summary>
        /// <param name="productionOrderDetailNew">Danh sách routing</param>
        /// <param name="ITMNO">Mã Chi tiết</param>
        /// <returns></returns>
        public List<ProductionOrderDetailViewModel> DelteRowRouting(List<ProductionOrderDetailViewModel> productionOrderDetailOld, string ITMNO)
        {
            productionOrderDetailOld = productionOrderDetailOld.Where(x => x.ProductAttributes.Replace(".", "-") != ITMNO).OrderBy(x => x.ProductAttributes).ToList();


            return productionOrderDetailOld;
        }
        #endregion

        #region Ghi nhận sản lượng
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        public List<ProductionOrderDetailViewModel> GetProductDetailsHistoryByTTLSX(Guid TaskId)
        {
            object[] SqlParams =
            {
                new SqlParameter("@TaskId",TaskId),
            };
            var res = _context.Database.SqlQuery<ProductionOrderDetailViewModel>("[MES].[GetProductDetailsHistoryByTTLSX] @TaskId", SqlParams).OrderBy(x => x.FromTime).ToList();

            return res;
        }

        public List<ProductionOrderDetailViewModel> GetProductDetailsHistoryByTTLSXForTransfer(Guid TaskId)
        {
            object[] SqlParams =
            {
                new SqlParameter("@TaskId",TaskId),
            };
            var res = _context.Database.SqlQuery<ProductionOrderDetailViewModel>("[MES].[GetProductDetailsHistoryByTTLSXForTransfer] @TaskId", SqlParams).ToList();

            return res;
        }
        /// <summary>
        /// Lấy thông tin lịch sử ghi nhận sản lượng
        /// </summary>
        /// <param name="TTLSX">TaskId TTLSX</param>
        /// <param name="fromTime">Thời gian bắt đầu</param>
        /// <param name="toTime">Thời gian kết thúc</param>
        /// <param name="itmno">Chi tiết</param>
        /// <param name="StepCode">Công đoạn</param>
        /// <returns></returns>
        public IEnumerable<ProductionOrderDetailViewModel> GetProductionRecordhistory(Guid TTLSX, DateTime fromTime, DateTime toTime, string itmno, string StepCode)
        {
            IQueryable<ProductionOrderDetailViewModel> data = (from s in _context.StockReceivingDetailModel
                                                                   //Thành phẩm
                                                               join pro in _context.ProductModel on s.ProductId equals pro.ProductId
                                                               //Chi tiết
                                                               join r in _context.RoutingInventorModel on pro.ERPProductCode equals r.MATNR
                                                               //Công đoạn
                                                               join st in _context.StockModel on s.StockId equals st.StockId
                                                               //Người tạo
                                                               join aTemp in _context.AccountModel on s.CreateBy equals aTemp.AccountId into aList
                                                               from a in aList.DefaultIfEmpty()
                                                               where
                                                                  s.ProductAttributes == r.ITMNO
                                                                  && s.ProductAttributes == itmno
                                                                  && s.CustomerReference == TTLSX
                                                                  && s.FromTime == fromTime
                                                                  && s.ToTime == toTime
                                                                  && st.StockCode == StepCode
                                                               group new { r, s, a } by new { r.KTEXT, s.Quantity, s.CreateTime, a.FullName, s.StockRecevingType } into g
                                                               select new ProductionOrderDetailViewModel
                                                               {
                                                                   //Tên chi tiết
                                                                   KTEXT = g.Key.KTEXT,
                                                                   //Số lượng
                                                                   Quantity = g.Key.Quantity,
                                                                   //Thời gian tạo
                                                                   CreateTime = g.Key.CreateTime,
                                                                   //Người tạo
                                                                   CreateByName = g.Key.FullName,
                                                                   //Loại
                                                                   StockRecevingType = g.Key.StockRecevingType
                                                               }).OrderByDescending(x => x.CreateTime);
            return data;

        }

        public DepartmentViewModel GetDepartment(Guid TTLSX, DateTime fromTime, DateTime toTime, string itmno)
        {
            DepartmentViewModel data = (from s in _context.StockReceivingDetailModel
                                            //Thành phẩm
                                        join d in _context.DepartmentModel on s.DepartmentId equals d.DepartmentId
                                        where s.ProductAttributes == itmno
                                        && s.CustomerReference == TTLSX
                                        && s.FromTime == fromTime
                                        && s.ToTime == toTime
                                        select new DepartmentViewModel
                                        {
                                            DepartmentId = d.DepartmentId,
                                            DepartmentName = d.DepartmentName,
                                            DepartmentCode = d.DepartmentCode
                                        }).FirstOrDefault();
            return data;
        }

        public int CountProductionRecordhistory(Guid? TTLSX, DateTime? fromTime, DateTime? toTime, string itmno, string StepCode)
        {
            var stockid = new StockRepository(_context).GetStockIdByStockCode(StepCode);

            var data = _context.StockReceivingDetailModel.Where(x => x.CustomerReference == TTLSX && x.FromTime == fromTime && x.ToTime == toTime && x.ProductAttributes == itmno && x.StockId == stockid).Count();
            return data;

        }

        public List<ProductionOrderDetailViewModel> GetTotalQtyOfProductDetailsByTTLSX(Guid TaskId)
        {
            object[] SqlParams =
            {
                new SqlParameter("@TaskId",TaskId),
            };
            var res = _context.Database.SqlQuery<ProductionOrderDetailViewModel>("[MES].[GetTotalQtyOfProductDetailsByTTLSX] @TaskId", SqlParams).ToList();

            return res;
        }
        #endregion

        #region Lấy thông tin chi tiết cho xác nhận hoàn tất công đoạn
        public List<ConfirmWorkCenterViewModel> GetTTLXSForConfirmWorkCenter(Guid? TaskId, string StepCode)
        {
            ProductionManagementRepository _productionManagementRepo = new ProductionManagementRepository(_context);

            var stockid = new StockRepository(_context).GetStockIdByStockCode(StepCode);

            object[] SqlParams =
                   {
                        new SqlParameter("@TaskId",TaskId),
                        new SqlParameter("@StockId",stockid),
                    };

            var res = _context.Database.SqlQuery<ConfirmWorkCenterViewModel>("[MES].[GetProductionDetailCompleted] @TaskId, @StockId", SqlParams).ToList();
            return res;
        }
        #endregion

        #region Lấy thông tin tồn kho của chi tiết con
        /// <summary>
        /// Lấy thông tin tồn kho của chi tiết
        /// </summary>
        /// <param name="productionOrderDetail">Danh sách chi tiết cần lấy chi tiết con</param>
        /// <param name="Plant">Nhà máy</param>
        /// <param name="radio">Loại công đoạn</param>
        /// <param name="TaskId">Mã TTLSX</param>
        /// <returns></returns>
        public List<UsageQuantityViewModel> UsageQuantity(string Plant, string StepCode, string radio, Guid TaskId)
        {
            Guid? StepId = null;
            //Lấy thông tin TTLSX
            var TTLS = GetExecutionTaskByTaskId(TaskId);
            #region ProductAttributes List
            var tableProductAttributesSchema = new List<SqlMetaData>(1)
            {
                new SqlMetaData("ProductAttributes", SqlDbType.NVarChar,50)
            }.ToArray();
            var tableProductAttributes = new List<SqlDataRecord>();
            #endregion
            string sqlQuery = "[MES].[UsageQuantity] @SO, @ProductionCode, @ProductAttributes, @WERKS, @StockId, @Radio";
            List<UsageQuantityViewModel> res = new List<UsageQuantityViewModel>();
            if (StepCode != null)
            {
                StepId = _context.StockModel.FirstOrDefault(x => x.StockCode == StepCode).StockId;
            }
            if (string.IsNullOrEmpty(TTLS.ProductAttributes))
            {
                var tableRow = new SqlDataRecord(tableProductAttributesSchema);
                tableRow.SetString(0, TTLS.ProductAttributes);
                tableProductAttributes.Add(tableRow);
            }
            else
            {
                var tableRow = new SqlDataRecord(tableProductAttributesSchema);
                tableProductAttributes.Add(tableRow);
            }
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
                {

                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "SO",
                        Value = TTLS.Property1 ?? (object)DBNull.Value,
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "ProductionCode",
                        Value = TTLS.ProductCode ?? (object)DBNull.Value,
                    },
                    new SqlParameter
                    {
                        //SqlDbType = SqlDbType.Structured,
                        //Direction = ParameterDirection.Input,
                        //ParameterName = "ProductAttributes",
                        //TypeName = "[dbo].[StringList]", //Don't forget this one!
                        //Value = tableProductAttributes
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "ProductAttributes",
                        Value = TTLS.ProductAttributes ?? (object)DBNull.Value,
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "WERKS",
                        Value = Plant ?? (object)DBNull.Value,
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input,
                        ParameterName = "StockId",
                        Value = StepId ?? (object)DBNull.Value,
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        ParameterName = "Radio",
                        Value = radio ?? (object)DBNull.Value,
                    },
                };
            #endregion
            var list = _context.Database.SqlQuery<UsageQuantityViewModel>(sqlQuery, parameters.ToArray()).ToList();


            return list;
        }
        #endregion

    }
}
