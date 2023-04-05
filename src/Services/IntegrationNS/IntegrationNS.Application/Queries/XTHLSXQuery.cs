using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.Commands.XTHLSXs;
using IntegrationNS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Queries
{
    public interface IXTHLSXQuery
    {
        /// <summary>
        /// Lấy data xth theo lsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<XTHLSXResponse>> GetXTHLSX(XTHLSXIntegrationCommand command);
    }

    public class XTHLSXQuery : IXTHLSXQuery
    {
        private readonly IRepository<IssueForProductionModel> _xthlsxRepo;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<DetailWorkOrderModel> _detailWoRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XTHLSXQuery(IRepository<IssueForProductionModel> xthlsxRepo, IRepository<WorkOrderModel> woRepo, IRepository<DetailWorkOrderModel> detailWoRepo,
                          IRepository<ProductModel> prdRepo, IRepository<OrderTypeModel> orderTypeRepo, IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo,
                          IRepository<StorageLocationModel> slocRepo)
        {
            _xthlsxRepo = xthlsxRepo;
            _woRepo = woRepo;
            _detailWoRepo = detailWoRepo;
            _prdRepo = prdRepo;
            _orderTypeRepo = orderTypeRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _slocRepo = slocRepo;
        }

        public async Task<List<XTHLSXResponse>> GetXTHLSX(XTHLSXIntegrationCommand command)
        {
            #region Format Day

            //Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                command.ScheduledStartFrom = command.ScheduledStartFrom.Value.Date;
            }
            if (command.ScheduledStartTo.HasValue)
            {
                command.ScheduledStartTo = command.ScheduledStartTo.Value.Date.AddDays(1).AddSeconds(-1);
            }

            //Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Tạo query
            var query = _xthlsxRepo.GetQuery()
                               .Include(x => x.DetailWorkOrder).ThenInclude(x => x.WorkOrder)
                               .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.Material))
            {
                query = query.Where(x => x.DetailWorkOrder.WorkOrder.ProductCodeInt == long.Parse(command.Material));
            }

            //Theo Component
            if (!string.IsNullOrEmpty(command.ComponentFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.ComponentTo))
                    command.ComponentTo = command.ComponentFrom;
                query = query.Where(x => x.ComponentCodeInt >= long.Parse(command.ComponentFrom) &&
                                         x.ComponentCodeInt <= long.Parse(command.ComponentTo));
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkorderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkorderTo))
                    command.WorkorderTo = command.WorkorderFrom;
                query = query.Where(x => x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderFrom) >= 0 &&
                                                                        x.DetailWorkOrder.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderTo) <= 0 &&
                                                                        x.DetailWorkOrder.RequirementQuantiy > 0 : false);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderType))
            {
                query = query.Where(x => x.DetailWorkOrder.WorkOrder.OrderTypeCode == command.OrderType);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;
                query = query.Where(x => x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
                                                                        x.DetailWorkOrder.WorkOrder.SalesOrder.CompareTo(command.SalesOrderTo) <= 0 : false);
            }


            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.ScheduledStartDate >= command.ScheduledStartFrom &&
                                                                        x.DetailWorkOrder.WorkOrder.ScheduledStartDate <= command.ScheduledStartTo : false);
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(command.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == command.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                if (!command.WeightDateTo.HasValue) command.WeightDateTo = command.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= command.WeightDateFrom &&
                                         x.CreateTime <= command.WeightDateTo);
            }

            //Check số phiếu cân
            if (command.WeightVotes != null && command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            //Search Status
            if (!string.IsNullOrEmpty(command.Status))
            {
                query = query.Where(x => x.Status == command.Status && x.ReverseDocument == null);
            }

            //Get query data material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new XTHLSXResponse
            {
                //ID NKPPPP
                XTHLSXId = x.IssForProductiontId,
                //7 Plant
                Plant = x.PlantCode ?? "",
                //8 Production Order
                WorkOrder = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.WorkOrderCodeInt.ToString() : "",
                //9 Material
                Material = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.ProductCodeInt.ToString() : "",
                //10 Material Desc
                MaterialDesc = x.DetailWorkOrderId.HasValue ? materials.FirstOrDefault(m => m.ProductCode == x.DetailWorkOrder.WorkOrder.ProductCode).ProductName : "",
                //11 Component
                Component = x.ComponentCodeInt.ToString() ?? "",
                //12 Component Desc
                ComponentDesc = !string.IsNullOrEmpty(x.ComponentCode) ? materials.FirstOrDefault(m => m.ProductCode == x.ComponentCode).ProductName : "",
                //13 Stor.Loc
                Sloc = x.SlocCode ?? "",
                SlocName = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {x.SlocName}",
                //14 Batch
                Batch = string.IsNullOrEmpty(x.MaterialDocument) ? x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.Batch : "" : x.Batch ?? "",
                //15 SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //16 Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //17 Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //18 Trọng lượng cân
                Weight = x.Weight ?? 0,
                //19 Confirm Quantity
                ConfirmQuantity = x.ConfirmQty ?? 0,
                //20 SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //21 Số lần cân
                QuantityWeight = x.QuantityWeitght ?? 0,
                //22 Số lượng yêu cầu
                RequirementQty = x.DetailWorkOrderId.HasValue ? Math.Abs(x.DetailWorkOrder.RequirementQuantiy) : 0,
                //23 Số lượng đã nhập thu hồi
                WithdrawnQty = x.DetailWorkOrderId.HasValue ? Math.Abs(x.DetailWorkOrder.QuantityWithdrawn) : 0,
                TotalQty = x.DetailWorkOrderId.HasValue ? Math.Abs(x.DetailWorkOrder.WorkOrder.TargetQuantity) : 0,
                //24 UOM
                Unit = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.Unit : "",
                //25 Ghi chú
                Description = x.Description ?? "",
                //26 Hình ảnh
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //27 Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //28 Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //Schedule Start Time
                ScheduleStartTime = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.ScheduledStartDate : null,
                //Schedule Finish Time
                ScheduleFinishTime = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.ScheduledFinishDate : null,
                //29 Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //30 Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                //31 Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //32 Crete On
                CreateOn = x.CreateTime ?? null,
                //33 Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //34 Material Doc
                MaterialDoc = x.MaterialDocument ?? "",
                //Mat doc item
                MaterialDocItem = x.MaterialDocumentItem ?? "",
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? "",
                isDelete = x.Status == "DEL" ? true : false
            }).ToListAsync();

            return data;
        }
    }
}
