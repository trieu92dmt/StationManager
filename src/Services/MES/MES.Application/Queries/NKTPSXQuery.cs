using Azure.Core;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.ReceiptFromProduction;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NKTPSX;
using MES.Application.DTOs.MES.OutboundDelivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKTPSXQuery
    {
        /// <summary>
        /// Lấy data lệnh sản xuất
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchWOResponse>> GetWO(SearchNKTPSXCommand command);

        /// <summary>
        /// Lấy data nktpsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKTPSXResponse>> GetNKTPSX(SearchNKTPSXCommand command);

        /// <summary>
        /// Lấy data theo wo
        /// </summary>
        /// <param name="workorder"></param>
        /// <returns></returns>
        Task<GetDataByWoResponse> GetDataByWo(string workorder);

        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);
    }

    public class NKTPSXQuery : INKTPSXQuery
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<ReceiptFromProductionModel> _rfProdRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;
        private readonly IRepository<ReceiptFromProductionModel> _nktpsxRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;

        public NKTPSXQuery(IUnitOfWork unitOfWork, IRepository<WorkOrderModel> woRepo, IRepository<ReceiptFromProductionModel> rfProdRepo,
                           IRepository<ProductModel> prodRepo, IRepository<OrderTypeModel> orderTypeRepo, IRepository<ReceiptFromProductionModel> nktpsxRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
            _rfProdRepo = rfProdRepo;
            _prodRepo = prodRepo;
            _orderTypeRepo = orderTypeRepo;
            _nktpsxRepo = nktpsxRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
        }
        public async Task<List<SearchNKTPSXResponse>> GetNKTPSX(SearchNKTPSXCommand command)
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
            var query = _nktpsxRepo.GetQuery()
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
                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
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


            //Get query data material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchNKTPSXResponse
            {
                //ID NKTPSX
                NKTPSXId = x.RcFromProductiontId,
                //Plant
                Plant = x.PlantCode ?? "",
                //Production Order
                WorkOrder = x.WorkOrderId.HasValue ? x.WorkOrder.WorkOrderCodeInt.ToString() : "",
                //Material
                Material = x.MaterialCode ?? "",
                //Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName : "",
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
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
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

        public async Task<List<SearchWOResponse>> GetWO(SearchNKTPSXCommand command)
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
            #endregion

            //Tạo query
            var query = _woRepo.GetQuery(x => x.SystemStatus.StartsWith("REL") &&
                                              !x.SystemStatus.StartsWith("REL CNF") &&
                                              !x.SystemStatus.StartsWith("REL TECO") &&
                                              x.DeletionFlag != "X")
                               .Include(x => x.DetailWorkOrderModel)
                               .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.Plant == command.Plant);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderType))
            {
                query = query.Where(x => x.OrderTypeCode == command.OrderType);
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkOrderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkOrderTo))
                    command.WorkOrderTo = command.WorkOrderFrom;
                query = query.Where(x => x.WorkOrderCode.CompareTo(command.WorkOrderFrom) >= 0 &&
                                         x.WorkOrderCode.CompareTo(command.WorkOrderTo) <= 0);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SaleOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SaleOrderTo))
                    command.SaleOrderTo = command.SaleOrderFrom;
                query = query.Where(x => x.SalesOrder.CompareTo(command.SaleOrderFrom) >= 0 &&
                                         x.SalesOrder.CompareTo(command.SaleOrderTo) <= 0);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.ScheduledStartDate >= command.ScheduledStartFrom &&
                                         x.ScheduledStartDate <= command.ScheduledStartTo);
            }

            //Get query data material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WorkOrderCode).Select(x => new SearchWOResponse
            {
                //Plant
                Plant = x.Plant ?? "",
                //Production Order
                WorkOrder = long.Parse(x.WorkOrderCode).ToString() ?? "",
                //Material
                Material = long.Parse(x.ProductCode).ToString() ?? "",
                //Material Desc
                MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName ?? "",
                //Storage Location
                Sloc = x.StorageLocation ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //Total Quantity
                TotalQuantity = x.TargetQuantity ?? 0,
                //Delivery Quantity
                DeliveryQuantity = x.DeliveredQuantity ?? 0,
                //UoM
                Unit = x.Unit ?? "",
                //Order Type
                OrderType = !string.IsNullOrEmpty(x.OrderTypeCode) ? 
                            $"{x.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.OrderTypeCode).ShortText}" : "",
                //Sales Order
                SalesOrder =x.SalesOrder ?? "",
                //Sales order item
                SaleOrderItem = x.SalesOrderItem ?? ""
            }).ToListAsync();

            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQuantity = item.TotalQuantity - item.DeliveryQuantity;
            }

            //Thêm dòng trống nếu search theo material
            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new SearchWOResponse
                {
                    Plant = command.Plant,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = materials.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                });
            }

            return data;
        }

        public async Task<GetDataByWoResponse> GetDataByWo(string workorder)
        {
            //Lấy ra wo
            var wo = await _woRepo.GetQuery().FirstOrDefaultAsync(x => x.WorkOrderCode == workorder);

            //Danh sách product
            var prods = _prodRepo.GetQuery().AsNoTracking();

            var totalQuantity = wo.TargetQuantity.HasValue ? wo.TargetQuantity : 0;
            var deliveryQuantity = wo.DeliveredQuantity.HasValue ? wo.DeliveredQuantity : 0;
            var openQuantity = totalQuantity - deliveryQuantity;

            var response = new GetDataByWoResponse
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(wo.ProductCode)).ProductCodeInt.ToString(),
                //Material Desc
                MaterialName = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(wo.ProductCode)).ProductName,
                //Batch
                Batch = wo.Batch,
                //Total Quantity
                TotalQty = totalQuantity,
                //Delivered Quantity
                DeliveryQty = deliveryQuantity,
                //Open Quantity
                OpenQty = openQuantity
            };

            return response;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nktpsxRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }
    }
}
