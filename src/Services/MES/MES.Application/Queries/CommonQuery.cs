using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Queries
{
    public interface ICommonQuery
    {
        /// <summary>
        /// Dropdown Plant
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPlant(string keyword);

        /// <summary>
        /// Dropdown Sale Org
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownSaleOrg();

        /// <summary>
        /// Dropdown Purchasing Gr
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPurchasingGr(string keyword);

        /// <summary>
        /// Dropdown Material
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<Common3Response>> GetDropdownMaterial(string keyword, string plant);

        /// <summary>
        /// Dropdown Component by wo
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownComponent(string workorder);

        /// <summary>
        /// Dropdown Purchasing Org by Plant
        /// </summary>
        /// <param name="plantCode"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPurchasingOrgByPlant(string keyword, string plantCode);

        /// <summary>
        /// Dropdown Vendor
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownVendor(string keyword);

        /// <summary>
        /// Dropdown POType
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPOType(string keyword);

        /// <summary>
        /// Dropdown PO
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant);

        /// <summary>
        /// Dropdown PO Item
        /// </summary>
        /// <param name="poCode"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPOItem(string poCode);

        /// <summary>
        /// Dropdown WeightHead
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse<bool>>> GetDropdownWeightHeadByPlant(string keyword, string plantCode);

        /// <summary>
        /// Dropdown Sloc
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<Common3Response>> GetDropdownSloc(string keyword, string plant);
        
        /// <summary>
        /// Dropdown Sale Order
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownSaleOrder(string keyword);

        /// <summary>
        /// Dropdown Outbound Delivery
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownOutboundDelivery(string keyword);

        /// <summary>
        /// Dropdown Outbound Delivery Item
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownOutboundDeliveryItem(string keyword, string odCode);

        /// <summary>
        /// Dropdown Ship To Party
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownShipToParty(string keyword);

        /// <summary>
        /// Dropdown số xe tải
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<Common2Response>> GetDropdownTruckNumber(string keyword, string plant);
        
        /// <summary>
        /// Dropdown Create By
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<Common2Response>> GetDropdownCreateBy(string keyword);
        
        /// <summary>
        /// Get số phiêu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetWeightVote(string keyword);

        /// <summary>
        /// Get OrderType
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetOrderType(string plant, string keyword);

        /// <summary>
        /// Get WorkOrder
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetWorkOrder(string plant, string orderType, string keyword);

        /// <summary>
        /// Get Reservation
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetReservation(string keyword);
    }
    
    public class CommonQuery : ICommonQuery
    {
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<SaleOrgModel> _saleOrgRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<PurchasingOrgModel> _purOrgRepo;
        private readonly IRepository<PurchasingGroupModel> _purGrRepo;
        private readonly IRepository<VendorModel> _vendorRepo;
        private readonly IRepository<PurchaseOrderMasterModel> _poMasterRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<SalesDocumentModel> _saleDocRepo;
        private readonly IRepository<OutboundDeliveryModel> _obDeliveryRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<AccountModel> _accRepo;
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;
        private readonly IRepository<OrderTypeModel> _oTypeRep;
        private readonly IRepository<WorkOrderModel> _workOrderRep;
        private readonly IRepository<ReservationModel> _rsRepo;

        public CommonQuery(IRepository<PlantModel> plantRepo, IRepository<SaleOrgModel> saleOrgRepo, IRepository<ProductModel> prodRepo,
                           IRepository<PurchasingOrgModel> purOrgRepo, IRepository<PurchasingGroupModel> purGrRepo, IRepository<VendorModel> vendorRepo,
                           IRepository<PurchaseOrderMasterModel> poMasterRepo, IRepository<StorageLocationModel> slocRepo, IRepository<ScaleModel> scaleRepo,
                           IRepository<GoodsReceiptModel> nkmhRep, IRepository<SalesDocumentModel> saleDocRepo, IRepository<OutboundDeliveryModel> obDeliveryRepo,
                           IRepository<CustmdSaleModel> custRepo, IRepository<AccountModel> accRepo, IRepository<TruckInfoModel> truckInfoRepo, 
                           IRepository<OrderTypeModel> oTypeRep, IRepository<WorkOrderModel> workOrderRep, IRepository<ReservationModel> rsRepo)
        {
            _plantRepo = plantRepo;
            _saleOrgRepo = saleOrgRepo;
            _prodRepo = prodRepo;
            _purOrgRepo = purOrgRepo;
            _purGrRepo = purGrRepo;
            _vendorRepo = vendorRepo;
            _poMasterRepo = poMasterRepo;
            _slocRepo = slocRepo;
            _scaleRepo = scaleRepo;
            _nkmhRep = nkmhRep;
            _saleDocRepo = saleDocRepo;
            _obDeliveryRepo = obDeliveryRepo;
            _custRepo = custRepo;
            _accRepo = accRepo;
            _truckInfoRepo = truckInfoRepo;
            _oTypeRep = oTypeRep;
            _workOrderRep = workOrderRep;
            _rsRepo = rsRepo;
        }

        #region Get DropdownMaterial
        public Task<List<Common3Response>> GetDropdownMaterial(string keyword, string plant)
        {
            var response = _prodRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ProductName.Contains(keyword) || x.ProductCodeInt.ToString().Contains(keyword) : true) &&
                                                   (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true))
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new Common3Response
                                     {
                                         Key = long.Parse(x.ProductCode).ToString(),
                                         Value = $"{x.ProductCodeInt} | {x.ProductName}",
                                         Name = x.ProductName
                                     }).Take(10).ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Plant
        public async Task<List<CommonResponse>> GetDropdownPlant(string keyword)
        {
            var response = await _plantRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.PlantName.Contains(keyword) || x.PlantCode.Contains(keyword) : true)
                                     .OrderBy(x => x.PlantCode)
                                     .Select(x => new CommonResponse
                                     {
                                         Key = x.PlantCode,
                                         Value = $"{x.PlantCode} | {x.PlantName}" 
                                     }).Take(10).ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Sale Org
        public async Task<List<CommonResponse>> GetDropdownSaleOrg()
        {
            var response = await _saleOrgRepo.GetQuery(x => x.Actived == true)
                .OrderBy(x => x.SaleOrgCode)
                .Select(x => new CommonResponse
                {
                    Key = x.SaleOrgCode,
                    Value = $"{x.SaleOrgCode} | {x.SaleOrgName}"
                }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Purchasing Gr
        public async Task<List<CommonResponse>> GetDropdownPurchasingGr(string keyword)
        {
            var response = await _purGrRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.PurchasingGroupName.Contains(keyword) || x.PurchasingGroupCode.Contains(keyword) : true)
                .OrderBy(x => x.PurchasingGroupCode)
                .Select(x => new CommonResponse
                {
                    Key = x.PurchasingGroupCode,
                    Value = $"{x.PurchasingGroupCode} | {x.PurchasingGroupName}"
                }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Purchasing Org by Plant
        public async Task<List<CommonResponse>> GetDropdownPurchasingOrgByPlant(string keyword, string plantCode)
        {
            var response = await _purOrgRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchasingOrgName.Contains(keyword) || x.PurchasingOrgCode.Contains(keyword) : true))
                .OrderBy(x => x.PurchasingOrgCode)
                .Select(x => new CommonResponse
                {
                    Key = x.PurchasingOrgCode,
                    Value = $"{x.PurchasingOrgCode} | {x.PurchasingOrgName}"
                }).AsNoTracking().ToListAsync();

            return response;
        }

        #endregion

        #region Dropdown Vendor
        public async Task<List<CommonResponse>> GetDropdownVendor(string keyword)
        {
            var response = await _vendorRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.VendorName.Contains(keyword) || x.VendorCode.Contains(keyword) : true)
                .OrderBy(x => x.VendorCode)
                .Select(x => new CommonResponse
                {
                    Key = x.VendorCode,
                    Value = $"{x.VendorCode} | {x.VendorName}"
                }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown po type
        public async Task<List<CommonResponse>> GetDropdownPOType(string keyword)
        {
            var response = await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.POType.Contains(keyword) : true) &&
                                                             (x.POType != null) &&
                                                             (x.POType != ""))
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.POType,
                                            Value = x.POType
                                        }).AsNoTracking().ToListAsync();

            return response.DistinctBy(x => x.Key).ToList();
        }
        #endregion

        #region Dropdown po
        public async Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant)
        {
            var response = await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             (x.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "X")).Include(x => x.PurchaseOrderDetailModel )
                                        .Where(x => x.PurchaseOrderDetailModel.FirstOrDefault(p => p.DeliveryCompleted != "X" && p.DeletionInd != "X") != null)
                                        .OrderBy(x => x.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrderCode,
                                            Value = x.PurchaseOrderCode
                                        }).AsNoTracking().ToListAsync();

            return response;
        }

        public async Task<List<CommonResponse<bool>>> GetDropdownWeightHeadByPlant(string keyword, string plantCode)
        {
            var response = await _scaleRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ScaleName.Contains(keyword) : true) &&
                                                          (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true))
                                    .OrderBy(x => x.ScaleCode)
                                    .Select(x => new CommonResponse<bool>
                                    {
                                        Key = x.ScaleCode,
                                        Value = x.ScaleName,
                                        Data = x.ScaleType.Value == true ? true : false
                                    }).AsNoTracking().ToListAsync();

            return response;
        }

        public async Task<List<Common3Response>> GetDropdownSloc(string keyword, string plant)
        {
            var response = await _slocRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.StorageLocationCode.Contains(keyword) || x.StorageLocationName.Contains(keyword) : true) &&
                                                         x.PlantCode == plant)
                                    .OrderBy(x => x.StorageLocationCode)
                                    .Select(x => new Common3Response
                                    {
                                        Key = x.StorageLocationCode,
                                        Value = $"{x.StorageLocationCode} | {x.StorageLocationName}",
                                        Name = x.StorageLocationName
                                    }).Take(10).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region GetWeightVote
        public async Task<List<CommonResponse>> GetWeightVote(string keyword)
        {
            return await _nkmhRep.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeitghtVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeitghtVote,
                                             Value = x.WeitghtVote
                                         }).Distinct().Take(20).ToListAsync();
        }
        #endregion

        #region Dropdown PoItem
        public async Task<List<CommonResponse>> GetDropdownPOItem(string poCode)
        {
            var po = await _poMasterRepo.GetQuery().Include(x => x.PurchaseOrderDetailModel).FirstOrDefaultAsync(x => x.PurchaseOrderCode == poCode);

            var response = po.PurchaseOrderDetailModel.OrderBy(x => x.PoLinetInt)
                                                      .Select(x => new CommonResponse
                                                      {
                                                          Key = x.POLine,
                                                          Value = x.POLine
                                                      }).ToList();

            return response;
        }
        #endregion

        #region Dropdown Sale Orrder
        public async Task<List<CommonResponse>> GetDropdownSaleOrder(string keyword)
        {
            return await _saleDocRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.SalesDocumentCode.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .OrderBy(x => x.SalesDocumentCode)
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.SalesDocumentCode,
                                             Value = x.SalesDocumentCode
                                         }).Take(10).ToListAsync();
        }
        #endregion

        #region Dropdown Outbound Delivery
        public async Task<List<CommonResponse>> GetDropdownOutboundDelivery(string keyword)
        {
            //Delivery Type lấy ra
            var deliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

            return await _obDeliveryRepo.GetQuery(x => (string.IsNullOrEmpty(keyword) ? true : x.DeliveryCode.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                       (deliveryType.Contains(x.DeliveryType)))
                                         .OrderBy(x => x.DeliveryCode)
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.DeliveryCode,
                                             Value = x.DeliveryCode
                                         }).Take(10).ToListAsync();
        }

        public async Task<List<CommonResponse>> GetDropdownShipToParty(string keyword)
        {
            var response = await _obDeliveryRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.ShiptoParty.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                                                                                x.ShiptoPartyName.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .OrderBy(x => x.ShiptoParty)
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.ShiptoParty,
                                             Value = $"{x.ShiptoParty} | {x.ShiptoPartyName}"
                                         }).ToListAsync();

            return response.DistinctBy(x => x.Key).Take(10).ToList();
        }
        #endregion

        #region GetDropdownCreateBy
        public async Task<List<Common2Response>> GetDropdownCreateBy(string keyword)
        {
            return await _accRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.UserName.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .OrderBy(x => x.UserName)
                                         .Select(x => new Common2Response
                                         {
                                             Key = x.AccountId,
                                             Value = x.UserName
                                         }).Take(10).ToListAsync();
        }
        #endregion

        #region GetDropdownTruckNumber
        public async Task<List<Common2Response>> GetDropdownTruckNumber(string keyword, string plant)
        {
            var response = await _truckInfoRepo.GetQuery(x => (string.IsNullOrEmpty(keyword) ? true : x.TruckNumber.Contains(keyword)) &&
                                                              x.PlantCode == plant &&
                                                              (x.CreateTime.HasValue ? x.CreateTime.Value.Date == DateTime.Now.Date &&
                                                                                       x.CreateTime.Value.Month == DateTime.Now.Month &&
                                                                                       x.CreateTime.Value.Year == DateTime.Now.Year: false))
                                  .OrderBy(x => x.TruckNumber).ThenByDescending(x => x.CreateTime)
                                  .Select(x => new Common2Response
                                  {
                                      Key = x.TruckInfoId,
                                      Value = x.TruckNumber
                                  }).AsNoTracking().ToListAsync();

            return response.DistinctBy(x => x.Key).ToList();
        }
        #endregion

        #region GetDropdownOutboundDeliveryItem
        public async Task<List<CommonResponse>> GetDropdownOutboundDeliveryItem(string keyword, string odCode)
        {
            var od = await _obDeliveryRepo.GetQuery().Include(x => x.DetailOutboundDeliveryModel).FirstOrDefaultAsync(x => x.DeliveryCode == odCode);

            var response = od.DetailOutboundDeliveryModel.OrderBy(x => x.OutboundDeliveryItem)
                                                      .Select(x => new CommonResponse
                                                      {
                                                          Key = x.OutboundDeliveryItem,
                                                          Value = x.OutboundDeliveryItem
                                                      }).ToList();

            return response;
        }
        #endregion

        #region Get OrderType
        /// <summary>
        /// GetOrderType
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetOrderType(string plant, string keyword)
        {
            var result = await _oTypeRep.GetQuery(x => x.Plant == plant && 
                                                    (!string.IsNullOrEmpty(keyword) ? x.OrderTypeCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true))
                                  .OrderBy(x => x.OrderTypeCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.OrderTypeCode,
                                      Value = $"{x.OrderTypeCode} | {x.ShortText}"
                                  }).Take(20).ToListAsync();

            return result;
        }
        #endregion

        #region Get WorkOrder
        /// <summary>
        /// Get WorkOrder
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetWorkOrder(string plant, string orderType, string keyword)
        {
            var result = await _workOrderRep.GetQuery(x => x.Plant == plant &&
                                                           (!x.SystemStatus.StartsWith("REL CNF")) &&
                                                           (!x.SystemStatus.StartsWith("TECO")) &&
                                                           (x.DeletionFlag != "X") &&
                                                           (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           (!string.IsNullOrEmpty(keyword) ? x.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true))
                                  .OrderBy(x => x.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrderCode,
                                      Value = x.WorkOrderCodeInt.ToString()
                                  }).Take(20).ToListAsync();

            return result;
        }

        public async Task<List<CommonResponse>> GetDropdownComponent(string workorder)
        {
            var wo = await _workOrderRep.GetQuery().Include(x => x.DetailWorkOrderModel).FirstOrDefaultAsync(x => x.WorkOrderCodeInt == long.Parse(workorder));

            var productQuery = _prodRepo.GetQuery().AsNoTracking();

            var response = new List<CommonResponse>();

            if (wo != null)
            {
                return wo.DetailWorkOrderModel
                    .Select(x => new CommonResponse
                    {
                        Key = x.ProductCode.ToString(),
                        Value = productQuery.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName
                    }).OrderBy(x => x.Key).DistinctBy(x => x.Key).ToList();
            }
            return response;
        }

        public async Task<List<CommonResponse>> GetReservation(string keyword)
        {
            return await _rsRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ReservationCode.ToLower().Contains(keyword.ToLower().Trim()) : true))
                                .Select(x => new CommonResponse
                                {
                                    Key = x.ReservationCode,
                                    Value = x.ReservationCodeInt.ToString()
                                }).AsNoTracking().ToListAsync();
        }
        #endregion
    }
}
