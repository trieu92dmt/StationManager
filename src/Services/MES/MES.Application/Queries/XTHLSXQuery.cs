using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.XTHLSX;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XTHLSX;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface IXTHLSXQuery
    {
        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetDataInputResponse>> GetInputData(SearchXTHLSXCommand command);

        /// <summary>
        /// Lấy data xth theo lsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchXTHLSXResponse>> GetXTHLSX(SearchXTHLSXCommand command);

        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data theo wo và component
        /// </summary>
        /// <param name="workorder"></param>
        /// <returns></returns>
        Task<GetDataByWoAndComponentResponse> GetDataByWoAndComponent(string workorder, string component);
    }

    public class XTHLSXQuery : IXTHLSXQuery
    {
        private readonly IRepository<ScrapFromProductionModel> _xthlsxRepo;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<DetailWorkOrderModel> _detailWoRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XTHLSXQuery(IRepository<ScrapFromProductionModel> xthlsxRepo, IRepository<WorkOrderModel> woRepo, IRepository<DetailWorkOrderModel> detailWoRepo,
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

        public async Task<GetDataByWoAndComponentResponse> GetDataByWoAndComponent(string workorder, string component)
        {
            //Lấy ra wo
            var woDetail = await _detailWoRepo.GetQuery().Include(x => x.WorkOrder)
                                              .FirstOrDefaultAsync(x => x.WorkOrder.WorkOrderCodeInt == long.Parse(workorder) &&
                                                                        x.ProductCodeInt == long.Parse(component));

            //Danh sách product
            var prods = _prdRepo.GetQuery().AsNoTracking();

            var response = new GetDataByWoAndComponentResponse
            {
                //Material
                Material = woDetail.WorkOrder.ProductCodeInt.ToString(),
                //Material Desc
                MaterialName = prods.FirstOrDefault(p => p.ProductCodeInt == woDetail.WorkOrder.ProductCodeInt).ProductName,
                //Batch
                Batch = woDetail.Batch,
                //Số lượng yêu cầu
                RequiremenQty = woDetail.RequirementQuantiy,
                //Số lượng nhập đã thu hồi
                WithdrawnQty = woDetail.QuantityWithdrawn,
                //Scheduled Start Date
                ScheduledStartDate = woDetail.WorkOrder.ScheduledStartDate,
                //Scheduled Finish Date
                ScheduledFinishDate = woDetail.WorkOrder.ScheduledFinishDate
            };

            return response;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _xthlsxRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetDataInputResponse>> GetInputData(SearchXTHLSXCommand command)
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
            var query = _detailWoRepo.GetQuery(x => x.SystemStatus.StartsWith("REL"))
                                     .Include(x => x.WorkOrder)
                                     .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.WorkOrder.Plant == command.Plant);
            }

            //Theo Component
            if (!string.IsNullOrEmpty(command.Component))
            {
                query = query.Where(x => x.ProductCodeInt == long.Parse(command.Component));
            }

            //Theo Component
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.WorkOrder.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.WorkOrder.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkorderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkorderTo))
                    command.WorkorderTo = command.WorkorderFrom;
                query = query.Where(x => x.WorkOrder.WorkOrderCodeInt >= long.Parse(command.WorkorderFrom) &&
                                         x.WorkOrder.WorkOrderCodeInt <= long.Parse(command.WorkorderTo) &&
                                         x.RequirementQuantiy > 0);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderType))
            {
                query = query.Where(x => x.WorkOrder.OrderTypeCode == command.OrderType);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;
                query = query.Where(x => x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderTo) <= 0);
            }


            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.WorkOrder.ScheduledStartDate >= command.ScheduledStartFrom &&
                                         x.WorkOrder.ScheduledStartDate <= command.ScheduledStartTo);
            }

            //Get query data material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.Select(x => new GetDataInputResponse
            {
                //Plant
                Plant = x.WorkOrder.Plant ?? "",
                //Production Order
                WorkOrder = long.Parse(x.WorkOrder.WorkOrderCode).ToString() ?? "",
                //Material
                Material = long.Parse(x.WorkOrder.ProductCode).ToString() ?? "",
                //Material Desc
                MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.WorkOrder.ProductCode).ProductName ?? "",
                //Component
                Component = long.Parse(x.ProductCode).ToString() ?? "",
                //Component Desc
                ComponentDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName ?? "",
                //Order Type
                OrderType = !string.IsNullOrEmpty(x.WorkOrder.OrderTypeCode) ?
                            $"{x.WorkOrder.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.WorkOrder.OrderTypeCode).ShortText}" : "",
                //Sales Order
                SalesOrder = x.WorkOrder.SalesOrder ?? "",
                //Storage Location
                Sloc = x.StorageLocation ?? "",
                SlocName = string.IsNullOrEmpty(x.StorageLocation) ? "" : $"{x.StorageLocation} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName}",
                //Batch
                Batch = x.Batch ?? "",
                //UoM
                Unit = x.WorkOrder.Unit ?? "",
                //Schedule Start Time
                ScheduleStartTime = x.WorkOrder.ScheduledStartDate ?? null,
                //Schedule Finish Time
                ScheduleFinishTime = x.WorkOrder.ScheduledFinishDate ?? null,
                //Requirement Qty
                RequirementQty = x.RequirementQuantiy ?? 0,
                //Withdraw Qty
                WithdrawQty = x.QuantityWithdrawn ?? 0
            }).ToListAsync();

            var index = 1;
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            //Thêm dòng trống nếu search theo component
            if (!string.IsNullOrEmpty(command.Component))
            {
                data.Add(new GetDataInputResponse
                {
                    Plant = command.Plant,
                    Component = long.Parse(command.Component).ToString(),
                    ComponentDesc = materials.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.Component)).ProductName,
                    Unit = materials.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.Component)).Unit
                });
            }

            return data;
        }

        public async Task<List<SearchXTHLSXResponse>> GetXTHLSX(SearchXTHLSXCommand command)
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

            //Theo Component
            if (!string.IsNullOrEmpty(command.Component))
            {
                query = query.Where(x => x.ComponentCodeInt == long.Parse(command.Component));
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.DetailWorkOrder.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.DetailWorkOrder.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkorderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkorderTo))
                    command.WorkorderTo = command.WorkorderFrom;
                query = query.Where(x => x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderFrom) >= 0 &&
                                                                        x.DetailWorkOrder.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderTo) <= 0 &&
                                                                        x.DetailWorkOrder.RequirementQuantiy > 0: false);
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
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchXTHLSXResponse
            {
                //ID NKPPPP
                XTHLSXId = x.ScFromProductiontId,
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
                Batch = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.Batch : "",
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
                RequirementQty = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.RequirementQuantiy : 0,
                //23 Số lượng đã nhập thu hồi
                WithdrawnQty = x.DetailWorkOrderId.HasValue ? x.DetailWorkOrder.QuantityWithdrawn : 0,
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
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? "",
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = x.Status == "DEL" || x.MaterialDocument != null || x.MaterialDocument != "" ? false : true
            }).ToListAsync();

            return data;
        }
    }
}
