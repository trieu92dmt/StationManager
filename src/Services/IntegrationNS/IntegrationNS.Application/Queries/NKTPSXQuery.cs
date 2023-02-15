using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.Commands.NKTPSXs;
using IntegrationNS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Queries
{
    public interface INKTPSXQuery
    {
        /// <summary>
        /// Lấy data nktpsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<NKTPSXResponse>> GetNKTPSX(NKTPSXIntegrationCommand command);
    }

    public class NKTPSXQuery : INKTPSXQuery
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<ReceiptFromProductionModel> _rfProdRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;
        private readonly IRepository<ReceiptFromProductionModel> _nkhtRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;

        public NKTPSXQuery(IUnitOfWork unitOfWork, IRepository<WorkOrderModel> woRepo, IRepository<ReceiptFromProductionModel> rfProdRepo,
                           IRepository<ProductModel> prodRepo, IRepository<OrderTypeModel> orderTypeRepo, IRepository<ReceiptFromProductionModel> nkhtRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
            _rfProdRepo = rfProdRepo;
            _prodRepo = prodRepo;
            _orderTypeRepo = orderTypeRepo;
            _nkhtRepo = nkhtRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
        }
        public async Task<List<NKTPSXResponse>> GetNKTPSX(NKTPSXIntegrationCommand command)
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
            var query = _nkhtRepo.GetQuery()
                               .Include(x => x.WorkOrder)
                               .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderType))
            {
                query = query.Where(x => x.WorkOrder.OrderTypeCode == command.OrderType);
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkOrderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkOrderTo))
                    command.WorkOrderTo = command.WorkOrderFrom;
                query = query.Where(x => x.WorkOrderId.HasValue ? x.WorkOrder.WorkOrderCode.CompareTo(command.WorkOrderFrom) >= 0 &&
                                         x.WorkOrder.WorkOrderCode.CompareTo(command.WorkOrderTo) <= 0 : false);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SaleOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SaleOrderTo))
                    command.SaleOrderTo = command.SaleOrderFrom;
                query = query.Where(x => x.WorkOrderId.HasValue ? x.WorkOrder.SalesOrder.CompareTo(command.SaleOrderFrom) >= 0 &&
                                                                  x.WorkOrder.SalesOrder.CompareTo(command.SaleOrderTo) <= 0 : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.MaterialCode.CompareTo(command.MaterialFrom) >= 0 &&
                                         x.MaterialCode.CompareTo(command.MaterialTo) <= 0);
            }

            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.WorkOrderId.HasValue ? x.WorkOrder.ScheduledStartDate >= command.ScheduledStartFrom &&
                                                                  x.WorkOrder.ScheduledStartDate <= command.ScheduledStartTo : false);
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
            if (!command.WeightVotes.IsNullOrEmpty() || command.WeightVotes.Any())
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
                query = query.Where(x => x.Status == command.Status);
            }

            //Get query data material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new NKTPSXResponse
            {
                //ID NKTPSX
                NKTPSXId = x.RcFromProductiontId,
                //Plant
                Plant = x.PlantCode ?? "",
                //Production Order
                WorkOrder = x.WorkOrderId.HasValue ? x.WorkOrder.WorkOrderCode : "",
                //Material
                Material = x.MaterialCode ?? "",
                //Material Desc
                MaterialDesc = string.IsNullOrEmpty(x.MaterialCode) ? materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName : "",
                //Stor.Loc
                Sloc = x.SlocCode ?? "",
                //Batch
                Batch = x.WorkOrderId.HasValue ? x.WorkOrder.Batch : "",
                //SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm Quantity
                ConfirmQuantity = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số lần cân
                QuantityWeight = x.QuantityWeitght ?? 0,
                //Total quantity
                TotalQuantity = x.WorkOrderId.HasValue ? x.WorkOrder.TargetQuantity : 0,
                //Delivery Quantity
                DeliveryQuantity = x.WorkOrderId.HasValue ? x.WorkOrder.DeliveredQuantity : 0,
                //Open Quantity
                OpenQuantity = 0,
                //UOM
                Unit = x.WorkOrderId.HasValue ? x.WorkOrder.Unit : "",
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = x.Image ?? "",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Sales Order
                SalesOrder = x.WorkOrderId.HasValue ? x.WorkOrder.SalesOrder : "",
                //Sales Order Item
                SalesOrderItem = x.WorkOrderId.HasValue ? x.WorkOrder.SalesOrderItem : "",
                //Order Type
                OrderType = x.WorkOrderId.HasValue ? x.WorkOrder.OrderTypeCode : "",
                //Schedule Start Date
                ScheduledStartDate = x.WorkOrderId.HasValue ? x.WorkOrder.ScheduledStartDate : null,
                //Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                //Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Crete On
                CreateOn = x.CreateTime ?? null,
                //Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //Material Doc
                MaterialDoc = x.MaterialDocument ?? "",
                //Reverse Doc
                ReverseDoc = x.ReverseDocument ?? "",
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = x.Status == "DEL" || x.MaterialDocument != null || x.MaterialDocument != "" ? false : true
            }).ToListAsync();

            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQuantity = item.TotalQuantity - item.DeliveryQuantity;
            }

            return data;
        }

    }
}
