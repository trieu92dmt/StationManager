using Azure;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Security.Cryptography.Xml;

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
        Task<List<DropdownMaterialResponse>> GetDropdownMaterial(string keyword, string plant, 
                                                                 string poFrom, string poTo, 
                                                                 string odFrom, string odTo, 
                                                                 string woFrom, string woTo, 
                                                                 string resFrom, string resTo);

        /// <summary>
        /// Dropdown Component by wo
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<DropdownMaterialResponse>> GetDropdownComponent(string keyword, string plant,
                                                                 string poFrom, string poTo,
                                                                 string woFrom, string woTo,
                                                                 string type);

        /// <summary>
        /// Dropdown Item Component by wo
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownItemComponent(string workorder);

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
        Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant, string type, string poType, string vendorFrom, string vendorTo);

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
        Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode);
        //Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode, string type);

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
        Task<List<CommonResponse>> GetDropdownOutboundDelivery(string type, string plant, string salesOrderFrom, string salesOrderTo, string materialFrom, string materialTo, string keyword);

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
        Task<List<CommonResponse>> GetOrderType(string plant, string keyword, string type);

        /// <summary>
        /// Get WorkOrder
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetWorkOrder(string plant, string orderType, string material, string keyword);

        /// <summary>
        /// Get Reservation
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetReservation(string keyword, string plant);

        /// <summary>
        /// Get dropdown customer
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<Common3Response>> GetDropdownCustomer(string keyword, string plant, string odFrom, string odTo, string type);

        /// <summary>
        /// Get dropdown scale monitor type
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetScaleMonitorType(string keyword);

        /// <summary>
        /// Get reservation item by reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetReservationItem(string reservation, string keyword);

        /// <summary>
        /// Get mat doc
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetMatDoc(string keyword, string plant);

        /// <summary>
        /// Get mat doc item
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetMatDocItem(string matdoc, string keyword);

        /// <summary>
        /// Get scale status
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetScaleStatus(string keyword);

        /// <summary>
        /// Get screen
        /// </summary>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownScreen();
    }

    #region Response
    //Response Dropdown material
    public class DropdownMaterialResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }

    public class DropdownWeightHeadResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Data { get; set; }
        public string Type { get; set; }

    }
    #endregion
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
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<DetailReservationModel> _dtRsRepo;
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _dtOdRepo;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRepo;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<ScreenModel> _screenRepo;
        private readonly IRepository<DetailWorkOrderModel> _dtWoRepo;

        public CommonQuery(IRepository<PlantModel> plantRepo, IRepository<SaleOrgModel> saleOrgRepo, IRepository<ProductModel> prodRepo,
                           IRepository<PurchasingOrgModel> purOrgRepo, IRepository<PurchasingGroupModel> purGrRepo, IRepository<VendorModel> vendorRepo,
                           IRepository<PurchaseOrderMasterModel> poMasterRepo, IRepository<StorageLocationModel> slocRepo, IRepository<ScaleModel> scaleRepo,
                           IRepository<GoodsReceiptModel> nkmhRep, IRepository<SalesDocumentModel> saleDocRepo, IRepository<OutboundDeliveryModel> obDeliveryRepo,
                           IRepository<CustmdSaleModel> custRepo, IRepository<AccountModel> accRepo, IRepository<TruckInfoModel> truckInfoRepo, 
                           IRepository<OrderTypeModel> oTypeRep, IRepository<WorkOrderModel> workOrderRep, IRepository<ReservationModel> rsRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<DetailReservationModel> dtRsRepo, IRepository<MaterialDocumentModel> matDocRepo,
                           IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<PurchaseOrderDetailModel> poDetailRepo, IRepository<ExportByCommandModel> xklxhRepo,
                           IRepository<ScreenModel> screenRepo, IRepository<DetailWorkOrderModel> dtWoRepo)
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
            _cataRepo = cataRepo;
            _dtRsRepo = dtRsRepo;
            _matDocRepo = matDocRepo;
            _dtOdRepo = dtOdRepo;
            _poDetailRepo = poDetailRepo;
            _xklxhRepo = xklxhRepo;
            _screenRepo = screenRepo;
            _dtWoRepo = dtWoRepo;
        }

        #region Get DropdownMaterial
        public async Task<List<DropdownMaterialResponse>> GetDropdownMaterial(string keyword, string plant,
                                                                        string poFrom, string poTo,
                                                                        string odFrom, string odTo,
                                                                        string woFrom, string woTo,
                                                                        string resFrom, string resTo)
        {
            var response = new List<DropdownMaterialResponse>();

            //Get query product
            var products = _prodRepo.GetQuery().AsNoTracking();

            #region po
            if (!string.IsNullOrEmpty(poFrom))
            {
                //Check nếu ko search field to thì gán to = from
                if (string.IsNullOrEmpty(poTo))
                    poTo = poFrom;

                response = await _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder)
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&           //Lọc plant
                                                    (x.PurchaseOrder.PurchaseOrderCode.CompareTo(poFrom) >= 0 && x.PurchaseOrder.PurchaseOrderCode.CompareTo(poTo) <= 0))   //Lọc po from to
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                    }).ToListAsync();
            }
            #endregion
            #region od
            else if (!string.IsNullOrEmpty(odFrom))
            {
                //Check nếu ko search field to thì gán to = from
                if (string.IsNullOrEmpty(odTo))
                    odTo = odFrom;

                response = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&                                                         //Lọc plant
                                                    (x.OutboundDelivery.DeliveryCode.CompareTo(odFrom) >= 0 && x.OutboundDelivery.DeliveryCode.CompareTo(odTo) <= 0))   //Lọc from to
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(x => x.ProductCode == x.ProductCode).ProductName}",
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                    }).ToListAsync();
            }
            #endregion
            #region wo
            else if (!string.IsNullOrEmpty(woFrom))
            {
                //Check nếu ko search field to thì gán to = from
                if (string.IsNullOrEmpty(woTo))
                    woTo = woFrom;

                response = await _workOrderRep.GetQuery()
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&                         //Lọc plant
                                                    (x.WorkOrderCode.CompareTo(woFrom) >= 0 && x.WorkOrderCode.CompareTo(woTo) <= 0))   //Lọc from to
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                    }).ToListAsync();
            }
            #endregion
            #region reservation
            else if (!string.IsNullOrEmpty(resFrom))
            {
                //Check nếu ko search field to thì gán to = from
                if (string.IsNullOrEmpty(resTo))
                    resTo = resFrom;

                response = await _dtRsRepo.GetQuery().Include(x => x.Reservation)
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.Reservation.Plant == plant : true) &&                                           //Lọc plant
                                                    (x.Reservation.ReservationCode.CompareTo(resFrom) >= 0 && x.Reservation.ReservationCode.CompareTo(resTo) <= 0))   //Lọc from to
                                    .OrderBy(x => x.Material)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.MaterialCodeInt.ToString(),
                                        Value = $"{x.MaterialCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName}",
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName,
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.Material).Unit
                                    }).ToListAsync();
            }
            #endregion
            else
            {
                response = await _prodRepo.GetQuery(x => (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true) &&
                                                   (!string.IsNullOrEmpty(keyword) ? x.ProductCode.Contains(keyword) || x.ProductName.Contains(keyword) : true))
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {x.ProductName}",
                                        Name = x.ProductName,
                                        Unit = x.Unit
                                    }).ToListAsync();
            }

            return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
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
            //Get query order type
            var orderType = _oTypeRep.GetQuery().AsNoTracking();

            var response = await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.POType.Contains(keyword) : true) &&
                                                             (x.POType != null) &&
                                                             (x.POType != ""))
                                        .OrderBy(x => x.POType)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.POType,
                                            Value = $"{x.POType} | {orderType.FirstOrDefault(o => o.OrderTypeCode == x.POType && o.Category == "01").ShortText}"
                                        }).AsNoTracking().ToListAsync();

            return response.DistinctBy(x => x.Key).ToList();
        }
        #endregion

        #region Dropdown po
        public async Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant, string type, string poType, string vendorFrom, string vendorTo)
        {
            //Nếu không search vendorto gán vendor to = vendor from
            if (!string.IsNullOrEmpty(vendorFrom) && string.IsNullOrEmpty(vendorTo))
                vendorTo = vendorFrom;

            if (type == "NKDCNB")
            {
                return await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             (x.POType == "Z007") &&
                                                             (x.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "X")).Include(x => x.PurchaseOrderDetailModel)
                                        .Where(x => x.PurchaseOrderDetailModel.FirstOrDefault(p => p.DeliveryCompleted != "X" && p.DeletionInd != "X") != null)
                                        .OrderBy(x => x.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrderCode,
                                            Value = x.PurchaseOrderCodeInt.ToString()
                                        }).AsNoTracking().ToListAsync();
            }   
            else if (type == "XNVLGC")
            {
                return await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             (x.POType == "Z003") &&
                                                             (x.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "L")).Include(x => x.PurchaseOrderDetailModel)
                                        .Where(x => x.PurchaseOrderDetailModel.FirstOrDefault(p => p.DeliveryCompleted != "X" && p.DeletionInd != "X") != null)
                                        .OrderBy(x => x.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrderCode,
                                            Value = x.PurchaseOrderCodeInt.ToString()
                                        }).AsNoTracking().ToListAsync();
            }

            var response = await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             (!string.IsNullOrEmpty(poType) ? x.POType == poType : true) &&
                                                             (!string.IsNullOrEmpty(vendorFrom) ? x.VendorCode.CompareTo(vendorFrom) >= 0 &&
                                                                                                  x.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                             (x.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "X")).Include(x => x.PurchaseOrderDetailModel )
                                        .Where(x => x.PurchaseOrderDetailModel.FirstOrDefault(p => p.DeliveryCompleted != "X" && p.DeletionInd != "X") != null)
                                        .OrderBy(x => x.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrderCode,
                                            Value = x.PurchaseOrderCodeInt.ToString()
                                        }).AsNoTracking().ToListAsync();

            return response;
        }

        public async Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode)
        {
            var response = await _scaleRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ScaleName.Contains(keyword) : true) &&
                                                          (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true))
                                    .OrderBy(x => x.ScaleCode)
                                    .Select(x => new DropdownWeightHeadResponse
                                    {
                                        Key = x.ScaleCode,
                                        Value = $"{x.ScaleCode} | {x.ScaleName}",
                                        Data = x.ScaleType.Value == true ? true : false,
                                        Type = x.isCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
                                    }).AsNoTracking().ToListAsync();

            return response;
        }

        //public async Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode, string type)
        //{
        //    //Lấy danh sách id cân theo màn hình
        //    var scaleIds = !string.IsNullOrEmpty(type) ? _screenRepo.GetQuery().Include(x => x.Screen_Scale_MappingModel)
        //                                                .FirstOrDefault(x => x.ScreenCode == type).Screen_Scale_MappingModel.Select(x => x.ScaleId) : null;
                                      

        //    var response = await _scaleRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ScaleName.Contains(keyword) : true) &&
        //                                                  (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true) &&
        //                                                  (!string.IsNullOrEmpty(type) ? scaleIds.Contains(x.ScaleId) : true))
        //                            .OrderBy(x => x.ScaleCode)
        //                            .Select(x => new DropdownWeightHeadResponse
        //                            {
        //                                Key = x.ScaleCode,
        //                                Value = x.ScaleName,
        //                                Data = x.ScaleType.Value == true ? true : false,
        //                                Type = x.isCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
        //                            }).AsNoTracking().ToListAsync();

        //    return response;
        //}

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
        public async Task<List<CommonResponse>> GetDropdownOutboundDelivery(string type, string plant, string salesOrderFrom, string salesOrderTo, string materialFrom, string materialTo, string keyword)
        {
            //Khai bao mảng chức delivery type
            var deliveryType = new List<string>();

            if (!string.IsNullOrEmpty(salesOrderFrom) && string.IsNullOrEmpty(salesOrderTo))
                salesOrderTo = salesOrderFrom;

            //Ở màn hình nhập kho hàng trả
            var query = _dtOdRepo.GetQuery()
                                       .Include(x => x.OutboundDelivery)
                                       .Where(x => (string.IsNullOrEmpty(keyword) ? true : x.OutboundDelivery.DeliveryCode.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                   (!string.IsNullOrEmpty(salesOrderFrom) ? x.ReferenceDocument1.CompareTo(salesOrderFrom) >= 0 && x.ReferenceDocument1.CompareTo(salesOrderTo) <= 0 : true))
                                       .AsNoTracking();

            //Lọc điều kiện
            //Màn hình nhập kho điều chuyển nội bộ
            if (type == "NKDCNB")
            {
                query = query.Where(x => x.OutboundDelivery.ReceivingPlant == plant &&
                                                        (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN"));
            }    
            //Màn hình nhập hàng loại T
            else if (type == "NHLT")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình NHLT
                deliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9" };
                query = query.Where(x => x.Plant == plant && 
                                         x.OutboundDelivery.PODStatus == "A" &&
                                         deliveryType.Contains(x.OutboundDelivery.DeliveryType));
            } 
            //Ở màn hình ghi nhận cân xe tải không lấy dropdown theo master data => lấy theo dữ liệu đã lưu
            else if (type == "GNCXT")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình GNCXT
                deliveryType = new List<string>(){ "ZLF1","ZLF2","ZLF3","ZLF4","ZLF5","ZLF6","ZLF7","ZLF8","ZLF9","ZLFA","ZNLC","ZNLN","ZXDH"};

                var xklxhQuery = await _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                 .Where(x => x.PlantCode == plant &&
                                             //Lọc các dòng có deliveryType thuộc biến deliveryType
                                             deliveryType.Contains(x.DetailOD.OutboundDelivery.DeliveryType))
                                 .OrderBy(x => x.DetailOD.OutboundDelivery.DeliveryCodeInt)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.DetailOD.OutboundDelivery.DeliveryCode,
                                     Value = x.DetailOD.OutboundDelivery.DeliveryCodeInt.ToString()
                                 })
                                 .AsNoTracking().ToListAsync();

                return xklxhQuery.DistinctBy(x => x.Key).Take(10).ToList();
            }    
            else query = query.Where(x => x.Plant == plant);


            //Theo material
            if (!string.IsNullOrEmpty(materialFrom))
            {
                //Nếu ko search To thì search 1
                if (string.IsNullOrEmpty(materialTo))
                    materialTo = materialFrom;
                query = query.Where(x => x.ProductCodeInt >= long.Parse(materialFrom) && x.ProductCodeInt <= long.Parse(materialTo));
            }

            var data = await query.OrderBy(x => x.OutboundDelivery.DeliveryCode)
                               .Select(x => new CommonResponse
                               {
                                   Key = x.OutboundDelivery.DeliveryCode,
                                   Value = x.OutboundDelivery.DeliveryCodeInt.ToString()
                               }).ToListAsync();

            return data.DistinctBy(x => x.Key).Take(10).ToList();
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
                                  .AsNoTracking().ToListAsync();

            return response.GroupBy(x => x.TruckNumber, (k, v) => new { Key = k, Value = v.ToList() })
                                  .Select(x => new Common2Response
                                  {
                                      Key = x.Value.First().TruckInfoId,
                                      Value = x.Key
                                  }).ToList();
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
        public async Task<List<CommonResponse>> GetOrderType(string plant, string keyword, string type)
        {
            //Khai bao mảng chức delivery type
            var deliveryType = new List<string>();

            if (type == "XKLXH")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình XKLXH
                deliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                var res = await _obDeliveryRepo.GetQuery(x => x.ShippingPoint == plant &&
                                                    (!string.IsNullOrEmpty(keyword) ? x.DeliveryType.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) &&
                                                                                      x.DeliveryTypeDescription.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                    deliveryType.Contains(x.DeliveryType))
                                  .OrderBy(x => x.DeliveryType)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.DeliveryType,
                                      Value = $"{x.DeliveryType} | {x.DeliveryTypeDescription}"
                                  }).ToListAsync();

                return res.DistinctBy(x => x.Key).Take(10).ToList();
            }    

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
        public async Task<List<CommonResponse>> GetWorkOrder(string plant, string orderType, string material, string keyword)
        {
            var result = await _workOrderRep.GetQuery(x => x.Plant == plant &&
                                                           (!x.SystemStatus.StartsWith("REL CNF")) &&
                                                           (!x.SystemStatus.StartsWith("TECO")) &&
                                                           (x.DeletionFlag != "X") &&
                                                           (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           (!string.IsNullOrEmpty(keyword) ? x.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                           (!string.IsNullOrEmpty(material) ? x.ProductCodeInt == long.Parse(material) : true))
                                  .OrderBy(x => x.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrderCode,
                                      Value = x.WorkOrderCodeInt.ToString()
                                  }).Take(20).ToListAsync();

            return result;
        }

        public async Task<List<DropdownMaterialResponse>> GetDropdownComponent(string keyword, string plant,
                                                                              string poFrom, string poTo,
                                                                              string woFrom, string woTo,
                                                                              string type)
        {
            var response = new List<DropdownMaterialResponse>();

            //Get query product
            var products = _prodRepo.GetQuery().AsNoTracking();

            #region po
            if (type == "XNVLGC")
            {
                if (!string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo))
                {
                    //Check nếu ko search field to thì gán to = from
                    poTo = poFrom;
                }

                response = await _dtRsRepo.GetQuery().Include(x => x.Reservation)
                                          .Where(x => (!string.IsNullOrEmpty(plant) ? x.Reservation.Plant == plant : true) &&                                                    //Lọc plant
                                                      (!string.IsNullOrEmpty(poFrom) ? x.PurchasingDoc.CompareTo(poFrom) >= 0 && x.PurchasingDoc.CompareTo(poTo) <= 0 : true))   //Lọc po from to
                                        .OrderBy(x => x.MaterialCodeInt)
                                        .Select(x => new DropdownMaterialResponse
                                        {
                                            Key = x.MaterialCodeInt.ToString(),
                                            Value = $"{x.MaterialCodeInt} | {products.FirstOrDefault(x => x.ProductCode == x.ProductCode).ProductName}",
                                            Name = products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName,
                                            Unit = products.FirstOrDefault(p => p.ProductCode == x.Material).Unit
                                        }).ToListAsync();
                
            }
            #endregion
            #region wo
            else if (type == "XTHLSX")
            {
                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(woFrom) && string.IsNullOrEmpty(woTo))
                    woTo = woFrom;

                response = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder)
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&                                   //Lọc plant
                                                    (!string.IsNullOrEmpty(woFrom) ? x.WorkOrder.WorkOrderCode.CompareTo(woFrom) >= 0 && x.WorkOrder.WorkOrderCode.CompareTo(woTo) <= 0 : true))   //Lọc from to
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                    }).ToListAsync();
            }
            #endregion
            else
            {
                response = await _prodRepo.GetQuery(x => (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true) &&
                                                   (!string.IsNullOrEmpty(keyword) ? x.ProductCode.Contains(keyword) || x.ProductName.Contains(keyword) : true))
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        Key = x.ProductCodeInt.ToString(),
                                        Value = $"{x.ProductCodeInt} | {x.ProductName}",
                                        Name = x.ProductName,
                                        Unit = x.Unit
                                    }).ToListAsync();
            }

            return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
        }

        public async Task<List<CommonResponse>> GetReservation(string keyword, string plant)
        {
            return await _rsRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ReservationCode.ToLower().Contains(keyword.ToLower().Trim()) : true) &&
                                               (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true))
                                .OrderBy(x => x.ReservationCodeInt)
                                .Select(x => new CommonResponse
                                {
                                    Key = x.ReservationCode,
                                    Value = x.ReservationCodeInt.ToString()
                                }).AsNoTracking().ToListAsync();
        }

        #endregion

        #region Drop down component item theo wo
        public async Task<List<CommonResponse>> GetDropdownItemComponent(string workorder)
        {
            var wo = await _workOrderRep.GetQuery().Include(x => x.DetailWorkOrderModel).FirstOrDefaultAsync(x => x.WorkOrderCodeInt == long.Parse(workorder));

            var response = new List<CommonResponse>();

            if (wo != null)
            {
                return wo.DetailWorkOrderModel
                    .Select(x => new CommonResponse
                    {
                        Key = x.WorkOrderItem,
                        Value = x.WorkOrderItem
                    }).OrderBy(x => x.Key).DistinctBy(x => x.Key).ToList();
            }

            return response;
        }

        #endregion

        public async Task<List<Common3Response>> GetDropdownCustomer(string keyword, string plant, string odFrom, string odTo, string type)
        {
            var response = new List<Common3Response>();

            if (type == "NHLT")
            {
                if (!string.IsNullOrEmpty(odFrom) && string.IsNullOrEmpty(odTo))
                    odTo = odFrom;

                response = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&                                                          //Lọc plant
                                                    //Lọc od from
                                                    (!string.IsNullOrEmpty(odFrom) ? (x.OutboundDelivery.DeliveryCode.CompareTo(odFrom) >= 0 && x.OutboundDelivery.DeliveryCode.CompareTo(odTo) <= 0) : true))   
                                        .Select(x => new Common3Response
                                        {
                                            Key = x.OutboundDelivery.ShiptoParty,
                                            Value = $"{x.OutboundDelivery.ShiptoParty} | {x.OutboundDelivery.ShiptoPartyName}",
                                            Name = x.OutboundDelivery.ShiptoPartyName
                                        }).AsNoTracking().ToListAsync();   
            }
            else
            {
                response = await _custRepo.GetQuery()
                                   .Select(x => new Common3Response
                                   {
                                       Key = x.CustomerNumber,
                                       Value = $"{x.CustomerNumber} | {x.CustomerName}",
                                       Name = x.CustomerName
                                   }).AsNoTracking().ToListAsync();
            }

            return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Key.ToLower().Contains(keyword.ToLower().Trim()) : true) ||
                                       (!string.IsNullOrEmpty(keyword) ? x.Name.ToLower().Contains(keyword.ToLower().Trim()) : true)).OrderBy(x => x.Key).DistinctBy(x => x.Key).Take(10).ToList();
        }

        #region Get Scale Monitor Type
        /// <summary>
        /// Get Scale Monitor Type
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetScaleMonitorType(string keyword)
        {
            var result = await _cataRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.CatalogCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) &&
                                                                                         x.CatalogText_vi.Trim().ToUpper().Contains(keyword.Trim().ToUpper()): true) &&
                                                       x.CatalogTypeCode == "ScaleMonitorType")
                                  .OrderBy(x => x.CatalogText_vi)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.CatalogCode,
                                      Value = x.CatalogText_vi
                                  }).ToListAsync();

            return result;
        }
        #endregion

        #region Get Reservation item
        /// <summary>
        /// Get Reservation item
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetReservationItem(string reservation, string keyword)
        {
            var result = await _dtRsRepo.GetQuery().Include(x => x.Reservation)
                                  .Where(x => !string.IsNullOrEmpty(reservation) ? x.Reservation.ReservationCodeInt == long.Parse(reservation) : false &&
                                              !string.IsNullOrEmpty(keyword) ? x.ReservationItem.Contains(keyword) : true)
                                  .OrderBy(x => x.ReservationItem)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.ReservationItem,
                                      Value = x.ReservationItem
                                  }).ToListAsync();

            return result;
        }
        #endregion

        #region Dropdown mat doc
        public async Task<List<CommonResponse>> GetMatDoc(string keyword, string plant)
        {
            var response = await _matDocRepo.GetQuery(x => (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true) &&
                                                           (!string.IsNullOrEmpty(keyword) ? x.MaterialDocCode.ToLower().Contains(keyword.ToLower().Trim()) : true) &&
                                                           (x.MovementType == "313") &&
                                                           (x.ItemAutoCreated == "X") &&
                                                           (x.MovementType != "315"))
                                .Select(x => new CommonResponse
                                {
                                    Key = x.MaterialDocCode,
                                    Value = x.MaterialDocCode
                                }).AsNoTracking().ToListAsync();

            return response.OrderBy(x => x.Key).DistinctBy(x => x.Key).Take(10).ToList();
        }
        #endregion

        #region Dropdown mat doc item
        public async Task<List<CommonResponse>> GetMatDocItem(string matdoc, string keyword)
        {
            return  await _matDocRepo.GetQuery(x =>(!string.IsNullOrEmpty(matdoc) ? x.MaterialDocCode == matdoc : false) &&
                                                   (!string.IsNullOrEmpty(keyword) ? x.MaterialDocItem.ToLower().Contains(keyword.ToLower().Trim()) : true))
                                .OrderBy(x => x.MaterialDocItem)
                                .Select(x => new CommonResponse
                                {
                                    Key = x.MaterialDocItem,
                                    Value = x.MaterialDocItem
                                }).AsNoTracking().ToListAsync();
        }
        #endregion

        #region Dropdown scale status
        public async Task<List<CommonResponse>> GetScaleStatus(string keyword)
        {
            var result = await _cataRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.CatalogCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) &&
                                                                                         x.CatalogText_vi.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                       x.CatalogTypeCode == "ScaleStatus")
                                  .OrderBy(x => x.CatalogText_vi)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.CatalogCode,
                                      Value = x.CatalogText_vi
                                  }).ToListAsync();

            return result;
        }
        #endregion

        #region Dropdown screen
        public async Task<List<CommonResponse>> GetDropdownScreen()
        {
            return await _screenRepo.GetQuery(x => x.Actived == true)
                                  .OrderBy(x => x.OrderIndex)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.ScreenCode,
                                      Value = x.ScreenName
                                  }).ToListAsync();
        }
        #endregion
    }
}
