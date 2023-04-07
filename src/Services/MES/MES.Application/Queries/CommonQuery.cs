using Azure.Core;
using Core.Extensions;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Newtonsoft.Json;
using Shared.Models;
using Shared.WeighSession;
using System.Linq;
using System.Text;

namespace MES.Application.Queries
{
    public interface ICommonQuery
    {
        /// <summary>
        /// Lấy danh sách common date
        /// </summary>
        /// <returns></returns>
        Task<List<CommonResponse>> GetCommonDate();

        /// <summary>
        /// Dropdown mã nhà máy
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPlant(string keyword);

        /// <summary>
        /// Dropdown Sale Org
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownSaleOrg();

        /// <summary>
        /// Dropdown Purchasing Group
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPurchasingGr(string keyword);

        /// <summary>
        /// Dropdown material
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="odFrom">OutboundDelivery</param>
        /// <param name="odTo">OutboundDelivery</param>
        /// <param name="deliveryType">DeliveryType</param>
        /// <param name="woFrom">ProductionOrder</param>
        /// <param name="woTo">ProductionOrder</param>
        /// <param name="orderType">OrderType</param>
        /// <param name="resFrom">Reservation</param>
        /// <param name="resTo">Reservation</param>
        /// <param name="soFrom">SalesOrder</param>
        /// <param name="soTo">SalesOrder</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <param name="shipToPartyFrom">ShipToParty</param>
        /// <param name="shipToPartyTo">ShipToParty</param>
        /// <param name="poType">POType</param>
        /// <param name="type">Tên màn hình</param>
        /// <returns></returns>
        Task<List<DropdownMaterialResponse>> GetDropdownMaterial(string keyword, string plant,
                                                                 string poFrom, string poTo,
                                                                 string odFrom, string odTo, string deliveryType,
                                                                 string woFrom, string woTo, string orderType,
                                                                 string resFrom, string resTo,
                                                                 string soFrom, string soTo,
                                                                 string vendorFrom, string vendorTo,
                                                                 string shipToPartyFrom, string shipToPartyTo,
                                                                 string poType, string type);

        /// <summary>
        /// Dropdown mã nguyên vật liệu
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="woFrom">WorkOrder</param>
        /// <param name="woTo">WorkOrder</param>
        /// <param name="type">Tên nhà máy</param>
        /// <returns></returns>
        Task<List<DropdownMaterialResponse>> GetDropdownComponent(string keyword, string plant,
                                                                  string poFrom, string poTo,
                                                                  string woFrom, string woTo,
                                                                  string soFrom, string soTo,
                                                                  string materialFrom, string materialTo,
                                                                  string orderType,
                                                                  string vendorFrom, string vendorTo,
                                                                  string type);

        /// <summary>
        /// Dropdown component item theo WorkOrderCode
        /// </summary>
        /// <param name="workorder">WorkOrderCode</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownItemComponent(string workorder);

        /// <summary>
        /// Dropdonw Purchasing Organization
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plantCode">Mã nhà máy</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPurchasingOrgByPlant(string keyword, string plantCode);

        /// <summary>
        /// Dropdown vendor
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownVendor(string keyword, string type, string plant);

        /// <summary>
        /// Dropdown PO Type
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPOType(string keyword, string plant, string vendorFrom, string vendorTo);

        /// <summary>
        /// Dropdown PO
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="poType">PO Type</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant, 
                                                 string type, string poType, 
                                                 string vendorFrom, string vendorTo,
                                                 string materialFrom, string materialTo);

        /// <summary>
        /// Dropdown POLine
        /// </summary>
        /// <param name="poCode">PurchaseOrderCode</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPOItem(string poCode);

        /// <summary>
        /// Dropdown WeightHead
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        //Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode);
        Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode, string type);

        /// <summary>
        /// Dropdown Storage Location
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        Task<List<Common3Response>> GetDropdownSloc(string keyword, string plant);

        /// <summary>
        /// Dropdown Sales Order
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="orderType">OrderType</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownSaleOrder(string keyword, string plant, string type, string orderType);

        /// <summary>
        /// Dropdown Outbound Delivery
        /// </summary>
        /// <param name="type">Tên màn hình</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="deliveryType">DeliveryType</param>
        /// <param name="salesOrderFrom">SalesOrder</param>
        /// <param name="salesOrderTo">SalesOrder</param>
        /// <param name="shipToPartyFrom">ShipToParty</param>
        /// <param name="shipToPartyTo">ShipToParty</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownOutboundDelivery(string type, string plant,
                                                               string deliveryType,
                                                               string salesOrderFrom, string salesOrderTo,
                                                               string shipToPartyFrom, string shipToPartyTo,
                                                               string poFrom, string poTo,
                                                               string materialFrom, string materialTo, string keyword);

        /// <summary>
        /// Dropdown Outbound Delivery Item
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownOutboundDeliveryItem(string keyword, string odCode);

        /// <summary>
        /// Dropdown Ship To Party
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="soFrom">Sales Order</param>
        /// <param name="soTo">Sales Order</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownShipToParty(string keyword, string plant, string type, string soFrom, string soTo, string poFrom, string poTo);

        /// <summary>
        /// Dropdown số xe tải
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        Task<List<Common2Response>> GetDropdownTruckNumber(string keyword, string plant);

        /// <summary>
        /// Dropdown người tạo
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<Common2Response>> GetDropdownCreateBy(string keyword);

        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetWeightVote(string keyword);

        /// <summary>
        /// Dropdown OrderType
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetOrderType(string plant, string keyword, string type,
                                                string poFrom, string poTo);

        /// <summary>
        /// Drodpown work order
        /// </summary>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="orderType">OrderType</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <param name="soFrom">SalesOrder</param>
        /// <param name="soTo">SalesOrder</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetWorkOrder(string plant, string type, 
                                                string orderType,
                                                string materialFrom, string materialTo,
                                                string soFrom, string soTo,
                                                string keyword);

        /// <summary>
        /// Dropdown reservation
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetReservation(string keyword, string plant, string type);

        /// <summary>
        /// Dropdown Customer
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="odFrom">OutboundDelivery</param>
        /// <param name="odTo">OutboundDelivery</param>
        /// <param name="type">Tên nhà máy</param>
        /// <returns></returns>
        Task<List<Common3Response>> GetDropdownCustomer(string keyword, string plant, string odFrom, string odTo, string resFrom, string resTo, string type);

        /// <summary>
        /// Dropdown loại hoạt động cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetScaleMonitorType(string keyword);

        /// <summary>
        /// Dropdown reservation item
        /// </summary>
        /// <param name="reservation">Reservation</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetReservationItem(string reservation, string keyword);

        /// <summary>
        /// Dropdown material document
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetMatDoc(string keyword, string plant);

        /// <summary>
        /// Dropdown MaterialDocumentItem
        /// </summary>
        /// <param name="matdoc">MaterialDocCode</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetMatDocItem(string matdoc, string keyword);

        /// <summary>
        /// Dropdown trạng thái cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetScaleStatus(string keyword);

        /// <summary>
        /// Dropdown màn hình
        /// </summary>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownScreen();

        /// <summary>
        /// Dropdown Id đợt cân
        /// </summary>
        /// <param name="keyword">Mã đầu cân</param>
        /// <param name="ScaleCode">Mã đầu cân</param>
        /// <param name="DateFrom">Từ ngày</param>
        /// <param name="DateTo">Đến ngày</param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownWeighSessionCode(string keyword, string ScaleCode, DateTime? DateFrom, DateTime? DateTo);
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
        private readonly IRepository<ScreenModel> _screenRepo;
        private readonly IRepository<DetailWorkOrderModel> _dtWoRepo;
        private readonly IRepository<WeighSessionModel> _weighSsRepo;
        private readonly IRepository<DimDateModel> _dimdateRepo;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<Screen_Scale_MappingModel> _screenScaleRepo;
        private readonly HttpClient _httpClient;

        public CommonQuery(IRepository<PlantModel> plantRepo, IRepository<SaleOrgModel> saleOrgRepo, IRepository<ProductModel> prodRepo,
                           IRepository<PurchasingOrgModel> purOrgRepo, IRepository<PurchasingGroupModel> purGrRepo, IRepository<VendorModel> vendorRepo,
                           IRepository<PurchaseOrderMasterModel> poMasterRepo, IRepository<StorageLocationModel> slocRepo, IRepository<ScaleModel> scaleRepo,
                           IRepository<GoodsReceiptModel> nkmhRep, IRepository<SalesDocumentModel> saleDocRepo, IRepository<OutboundDeliveryModel> obDeliveryRepo,
                           IRepository<CustmdSaleModel> custRepo, IRepository<AccountModel> accRepo, IRepository<TruckInfoModel> truckInfoRepo, 
                           IRepository<OrderTypeModel> oTypeRep, IRepository<WorkOrderModel> workOrderRep, IRepository<ReservationModel> rsRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<DetailReservationModel> dtRsRepo, IRepository<MaterialDocumentModel> matDocRepo,
                           IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<PurchaseOrderDetailModel> poDetailRepo,
                           IRepository<ScreenModel> screenRepo, IRepository<DetailWorkOrderModel> dtWoRepo, IRepository<WeighSessionModel> weighSsRepo, 
                           IRepository<DimDateModel> dimdateRepo, IHttpClientFactory httpClientFactory, IRepository<ExportByCommandModel> xklxhRepo,
                           IRepository<Screen_Scale_MappingModel> screenScaleRepo)
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
            _screenRepo = screenRepo;
            _dtWoRepo = dtWoRepo;
            _weighSsRepo = weighSsRepo;
            _dimdateRepo = dimdateRepo;
            _xklxhRepo = xklxhRepo;
            _screenScaleRepo = screenScaleRepo;
            _httpClient = httpClientFactory.CreateClient();

        }

        #region Get DropdownMaterial
        /// <summary>
        /// Dropdown material
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="odFrom">OutboundDelivery</param>
        /// <param name="odTo">OutboundDelivery</param>
        /// <param name="deliveryType">DeliveryType</param>
        /// <param name="woFrom">ProductionOrder</param>
        /// <param name="woTo">ProductionOrder</param>
        /// <param name="orderType">OrderType</param>
        /// <param name="resFrom">Reservation</param>
        /// <param name="resTo">Reservation</param>
        /// <param name="soFrom">SalesOrder</param>
        /// <param name="soTo">SalesOrder</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <param name="shipToPartyFrom">ShipToParty</param>
        /// <param name="shipToPartyTo">ShipToParty</param>
        /// <param name="poType">POType</param>
        /// <param name="type">Tên màn hình</param>
        /// <returns></returns>
        public async Task<List<DropdownMaterialResponse>> GetDropdownMaterial(string keyword, string plant,
                                                                        string poFrom, string poTo,
                                                                        string odFrom, string odTo, string deliveryType,
                                                                        string woFrom, string woTo, string orderType,
                                                                        string resFrom, string resTo, 
                                                                        string soFrom, string soTo,
                                                                        string vendorFrom, string vendorTo,
                                                                        string shipToPartyFrom, string shipToPartyTo,
                                                                        string poType, string type)
        {
            //Chỉ search vendorFrom thì search 1
            vendorTo = !string.IsNullOrEmpty(vendorFrom) && string.IsNullOrEmpty(vendorTo) ? vendorFrom : vendorTo;

            //Chỉ search poFrom thì search 1
            poTo = !string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo) ? poFrom : poTo;

            //Chỉ search poFrom thì search 1
            resTo = !string.IsNullOrEmpty(resFrom) && string.IsNullOrEmpty(resTo) ? resFrom : resTo;

            //Chỉ search woFrom thì search 1
            woTo = !string.IsNullOrEmpty(woFrom) && string.IsNullOrEmpty(woTo) ? woFrom : woTo;

            //Chỉ search odFrom thì search 1
            odTo = !string.IsNullOrEmpty(odFrom) && string.IsNullOrEmpty(odTo) ? odFrom : odTo;

            //Chỉ search soFrom thì search 1
            soTo = !string.IsNullOrEmpty(soFrom) && string.IsNullOrEmpty(soTo) ? soFrom : soTo;

            //Chỉ search shipToPartyFrom thì search 1
            shipToPartyTo = !string.IsNullOrEmpty(shipToPartyFrom) && string.IsNullOrEmpty(shipToPartyTo) ? shipToPartyFrom : shipToPartyTo;

            var response = new List<DropdownMaterialResponse>();

            //Get query product
            var products = _prodRepo.GetQuery().AsNoTracking();

            #region NKMH
            //Màn hình nhập kho mua hàng
            //Khi lại màn hình là "NKMH" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            if (type == "NKMH" && (!string.IsNullOrEmpty(poType) || !string.IsNullOrEmpty(vendorFrom) || !string.IsNullOrEmpty(poFrom)))
            {
                response = await _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder)
                                    .Where(x => (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&           //Lọc plant
                                                (!string.IsNullOrEmpty(poFrom) ? x.PurchaseOrder.PurchaseOrderCode.CompareTo(poFrom) >= 0 && 
                                                                                 x.PurchaseOrder.PurchaseOrderCode.CompareTo(poTo) <= 0 : true) &&  //Lọc po from to
                                                (!string.IsNullOrEmpty(poType) ? x.PurchaseOrder.POType == poType : true) && //Lọc theo po type 
                                                (!string.IsNullOrEmpty(vendorFrom) ? x.PurchaseOrder.VendorCode.CompareTo(vendorFrom) >= 0 &&   //Lọc theo vendor from to  
                                                                                     x.PurchaseOrder.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                //Loại các line đã hoàn tất nhập kho
                                                x.DeliveryCompleted != "X" &&
                                                //Loại các line đã đánh dấu xóa
                                                x.DeletionInd != "X" &&
                                                x.PurchaseOrder.DeletionInd != "X" 
                                                /*x.PurchaseOrder.ReleaseIndicator == "R"*/) 
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        //Material code
                                        Key = x.ProductCodeInt.ToString(),
                                        //Material code | material name
                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                        //Material name
                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                        //Đơn vị
                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                    }).ToListAsync();

                //Lọc theo keywork
                return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region NKHT
            //Màn hình nhập kho hàng trả
            //Khi lại màn hình là "NKHT" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "NKHT" && (!string.IsNullOrEmpty(odFrom) || !string.IsNullOrEmpty(shipToPartyFrom) || !string.IsNullOrEmpty(soFrom)))
            {
                //Delivery Type lấy ra
                var NKHTdeliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

                response = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                          .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&  //Lọc plant
                                                                                                                   //Lọc od from to
                                                      (!string.IsNullOrEmpty(odFrom) ? x.OutboundDelivery.DeliveryCode.CompareTo(odFrom) >= 0 &&
                                                                                                                x.OutboundDelivery.DeliveryCode.CompareTo(odTo) <= 0 : true) &&
                                                      //Theo shiptoparty
                                                      (!string.IsNullOrEmpty(shipToPartyFrom) ? (x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                                x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0) : true) &&
                                                      //Theo sales order
                                                      (!string.IsNullOrEmpty(soFrom) ? (x.ReferenceDocument1.CompareTo(soFrom) >= 0 &&
                                                                                       x.ReferenceDocument1.CompareTo(soTo) <= 0) : true) &&
                                                      //Theo delivery type
                                                      (NKHTdeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                                      //Đã hoàn tất giao dịch
                                                      (x.GoodsMovementSts != "C")
                                                      )   
                                     .OrderBy(x => x.ProductCode)
                                     .Select(x => new DropdownMaterialResponse
                                     {
                                         //Material code
                                         Key = x.ProductCodeInt.ToString(),
                                         //Material code | material name
                                         Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                         //Material name
                                         Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                         //Đơn vị
                                         Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                     }).ToListAsync();

                //Lọc theo keyword
                return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region XKLXH
            //Màn hình xuất kho theo lệnh xuất hàng
            //Khi lại màn hình là "XKLXH" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            if (type == "XKLXH" && (!string.IsNullOrEmpty(poFrom) || 
                                    !string.IsNullOrEmpty(soFrom) || 
                                    !string.IsNullOrEmpty(deliveryType) || 
                                    !string.IsNullOrEmpty(shipToPartyFrom) ||
                                    !string.IsNullOrEmpty(odFrom)))
            {
                var deliveryTypeQuery = _oTypeRep.GetQuery().AsNoTracking();

                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo delivery type
                                            (!string.IsNullOrEmpty(deliveryType) ? x.OutboundDelivery.DeliveryType == deliveryType : true) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Theo po
                                            (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                             x.ReferenceDocument1.CompareTo(poTo) <= 0 : true) &&
                                            //Theo so
                                            (!string.IsNullOrEmpty(soFrom) ? x.ReferenceDocument1.CompareTo(soFrom) >= 0 &&
                                                                             x.ReferenceDocument1.CompareTo(soTo) <= 0 : true) &&
                                            //Theo Shiptoparty
                                            (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                      x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                            //Theo od
                                            (!string.IsNullOrEmpty(odFrom) ? x.OutboundDelivery.DeliveryCode.CompareTo(odFrom) >= 0 &&
                                                                             x.OutboundDelivery.DeliveryCode.CompareTo(odTo) <= 0 : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            //Đã hoàn tất giao dịch
                                            (x.OutboundDelivery.GoodsMovementSts != "C"))
                                 .OrderBy(x => x.ProductCodeInt)
                                 .Select(x => new DropdownMaterialResponse
                                 {
                                     //Material code
                                     Key = x.ProductCodeInt.ToString(),
                                     //Material code | material name
                                     Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                     //Material name
                                     Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                     //Đơn vị
                                     Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                 }).ToListAsync();

                return xklxhResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region NKTPSX
            //Màn hình nhập kho thành phẩm sản xuất
            //Khi lại màn hình là "NKTPSX" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "NKTPSX" && (!string.IsNullOrEmpty(orderType) || !string.IsNullOrEmpty(soFrom) || !string.IsNullOrEmpty(woFrom)))
            {
                //Tạo query
                var NKTPSXResponse = await _workOrderRep.GetQuery(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo sale order
                                                  (!string.IsNullOrEmpty(soFrom) ? x.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                   x.SalesOrder.CompareTo(soTo) <= 0 : true) &&
                                                  //Lọc theo workorder
                                                  (!string.IsNullOrEmpty(woFrom) ? x.WorkOrderCode.CompareTo(woFrom) >= 0 &&
                                                                                   x.WorkOrderCode.CompareTo(woTo) <= 0 : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  //Loại System Status bắt đầu bằng "REL CNF" và "REL TECO"
                                                  !x.SystemStatus.StartsWith("REL CNF") &&
                                                  !x.SystemStatus.StartsWith("REL TECO") &&
                                                  //Loại các line có tích DeletionFlag
                                                  x.DeletionFlag != "X")
                                            .OrderBy(x => x.ProductCodeInt)
                                            .Select(x => new DropdownMaterialResponse
                                            {
                                                //Material code
                                                Key = x.ProductCodeInt.ToString(),
                                                //Material code | material name
                                                Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                                //Material name
                                                Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                                //Đơn vị
                                                Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                            }).ToListAsync();

                return NKTPSXResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region NKDCNB
            //Màn hình nhập kho điều chuyển nội bộ
            //Khi lại màn hình là "NKDCNB" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "NKDCNB")
            {
                var NKDCNBResponse = await _dtOdRepo.GetQuery()
                                                    .Include(x => x.OutboundDelivery)
                                                    //Lọc delivery type
                                                    .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                                (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                                //Lấy delivery đã hoàn tất giao dịch
                                                                x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                                x.GoodsMovementSts == "C" &&
                                                                //Lọc theo po
                                                                (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                                                 x.ReferenceDocument1.CompareTo(poTo) <= 0 : true) &&
                                                                //Lọc theo shiptoparty
                                                                (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                                          x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                                                //Lọc theo deliveryType
                                                                (!string.IsNullOrEmpty(deliveryType) ? x.OutboundDelivery.DeliveryType == deliveryType : true) &&
                                                                //Lọc theo od
                                                                (!string.IsNullOrEmpty(odFrom) ? x.OutboundDelivery.DeliveryCode.CompareTo(poFrom) >= 0 &&
                                                                                                 x.OutboundDelivery.DeliveryCode.CompareTo(poTo) <= 0 : true)
                                                                )
                                                    .OrderBy(x => x.OutboundDelivery.DeliveryCodeInt)
                                                    .Select(x => new DropdownMaterialResponse
                                                    {
                                                        //Material code
                                                        Key = x.ProductCodeInt.ToString(),
                                                        //Material code | material name
                                                        Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                                        //Material name
                                                        Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                                        //Đơn vị
                                                        Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                                    }).AsNoTracking().ToListAsync();
                return NKDCNBResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region NKPPPP
            //Màn NKPPPP
            //Khi loại màn hình là "NKPPPP" thì chỉ search theo chứng từ
            else if (type == "NKPPPP")
            {
                //Tạo query
                var NKPPPPResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder).Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo sale order
                                                  (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                   x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true) &&
                                                  //Lọc theo workorder
                                                  (!string.IsNullOrEmpty(woFrom) ? x.WorkOrder.WorkOrderCode.CompareTo(woFrom) >= 0 &&
                                                                                   x.WorkOrder.WorkOrderCode.CompareTo(woTo) <= 0 : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy <= 0)
                                            .OrderBy(x => x.ProductCodeInt)
                                            .Select(x => new DropdownMaterialResponse
                                            {
                                                //Material code
                                                Key = x.WorkOrder.ProductCodeInt.ToString(),
                                                //Material code | material name
                                                Value = $"{x.WorkOrder.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).ProductName}",
                                                //Material name
                                                Name = products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).ProductName,
                                                //Đơn vị
                                                Unit = products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).Unit
                                            }).ToListAsync();

                return NKPPPPResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region XTHLSX
            //Màn XTHLSX
            //Khi lại màn hình là "XTHLSX" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "XTHLSX")
            {
                //Tạo query
                var XTHLSXResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder).Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo sale order
                                                  (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                   x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true) &&
                                                  //Lọc theo workorder
                                                  (!string.IsNullOrEmpty(woFrom) ? x.WorkOrder.WorkOrderCode.CompareTo(woFrom) >= 0 &&
                                                                                   x.WorkOrder.WorkOrderCode.CompareTo(woTo) <= 0 : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy > 0)
                                            .OrderBy(x => x.ProductCodeInt)
                                            .Select(x => new DropdownMaterialResponse
                                            {
                                                //Material code
                                                Key = x.WorkOrder.ProductCodeInt.ToString(),
                                                //Material code | material name
                                                Value = $"{x.WorkOrder.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).ProductName}",
                                                //Material name
                                                Name = products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).ProductName,
                                                //Đơn vị
                                                Unit = products.FirstOrDefault(p => p.ProductCode == x.WorkOrder.ProductCode).Unit
                                            }).ToListAsync();

                return XTHLSXResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region XCK
            //Màn xuất chuyển kho
            //Khi lại màn hình là "XCK" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "XCK" && !string.IsNullOrEmpty(resTo))
            {
                //Tạo query
                var XCKResponse = await _dtRsRepo.GetQuery()
                                           .Include(x => x.Reservation)
                                           .Where(x =>
                                                       //Lọc theo reservation from to
                                                       (!string.IsNullOrEmpty(resFrom) ? x.Reservation.ReservationCode.CompareTo(resFrom) >= 0 &&
                                                                                         x.Reservation.ReservationCode.CompareTo(resTo) <= 0 : true) &&
                                                       //Lấy các reservation có movement type là 311 313
                                                       (x.MovementType == "311" || x.MovementType == "313") &&
                                                       //Loại trừ các reservation đã hoàn tất chuyển kho
                                                       (x.Reservation.FinalIssue != "X") &&
                                                       //Loại trừ các reservation đã đánh dấu xóa
                                                       (x.ItemDeleted != "X"))
                                               .OrderBy(x => x.MaterialCodeInt)
                                               .Select(x => new DropdownMaterialResponse
                                               {
                                                   //Material code
                                                   Key = x.MaterialCodeInt.ToString(),
                                                   //Material code | material name
                                                   Value = $"{x.MaterialCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName}",
                                                   //Material name
                                                   Name = products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName,
                                                   //Đơn vị
                                                   Unit = products.FirstOrDefault(p => p.ProductCode == x.Material).Unit
                                               }).AsNoTracking().ToListAsync();

                return XCKResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region XNVLGC
            //Màn xuất nguyên vật liệu gia công
            //Khi lại màn hình là "XNVLGC" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "XNVLGC" && (!string.IsNullOrEmpty(vendorFrom) || !string.IsNullOrEmpty(poFrom)))
            {
                //Get list po code từ bảng reservation
                var pos = await _dtRsRepo.GetQuery().Select(x => x.PurchasingDoc).AsNoTracking().ToListAsync();

                //Get data
                var XNVLGCResponse = await _poDetailRepo.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x =>
                                                        //Theo plant
                                                        (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&
                                                        //Chỉ lấy những po nằm trong reservation
                                                        pos.Contains(x.PurchaseOrder.PurchaseOrderCode) &&
                                                        //Theo vendor
                                                        (!string.IsNullOrEmpty(vendorFrom) ? x.PurchaseOrder.VendorCode.CompareTo(vendorFrom) >= 0 &&
                                                                                             x.PurchaseOrder.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                        //Theo PuchaseOrder
                                                        (!string.IsNullOrEmpty(poFrom) ? x.PurchaseOrder.PurchaseOrderCodeInt >= long.Parse(poFrom) &&
                                                                                         x.PurchaseOrder.PurchaseOrderCodeInt <= long.Parse(poTo) : true) &&
                                                        //Loại các line đã hoàn tất chuyển kho
                                                        x.DeliveryCompleted != "X" &&
                                                        //Loại các line đã đánh dấu xóa
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X" &&
                                                        //Lấy các line đã được duyệt
                                                        x.PurchaseOrder.ReleaseIndicator == "R")
                                               .OrderBy(x => x.ProductCodeInt)
                                               .Select(x => new DropdownMaterialResponse
                                               {
                                                   //Material code
                                                   Key = x.ProductCodeInt.ToString(),
                                                   //Material code | material name
                                                   Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                                   //Material name
                                                   Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                                   //Đơn vị
                                                   Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                               }).AsNoTracking().ToListAsync();

                return XNVLGCResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region NHLT
            //Màn nhập hàng loại T
            //Khi lại màn hình là "NHLT" có tham số đầu vào liên quan đến chứng từ thì lấy material trong chứng từ
            else if (type == "XCK" && (!string.IsNullOrEmpty(shipToPartyFrom) || !string.IsNullOrEmpty(odFrom)))
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình NHLT
                var NHLTdeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9" };
                var NHLTResponse = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                         .Where(x => x.Plant == plant &&
                                                     x.OutboundDelivery.PODStatus == "A" &&
                                                     //Lọc theo customer
                                                     (string.IsNullOrEmpty(shipToPartyFrom) || (x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                               x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0)) &&
                                                     //Lọc theo outbound delivery
                                                     (string.IsNullOrEmpty(odFrom) || (x.OutboundDelivery.DeliveryCodeInt >= long.Parse(odFrom) &&
                                                                                       x.OutboundDelivery.DeliveryCodeInt <= long.Parse(odTo))) &&
                                                     //Lọc theo delivery type
                                                     NHLTdeliveryType.Contains(x.OutboundDelivery.DeliveryType))
                                         .OrderBy(x => x.ProductCodeInt)
                                         .Select(x => new DropdownMaterialResponse
                                         {
                                             //Material code
                                             Key = x.ProductCodeInt.ToString(),
                                             //Material code | material name
                                             Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                             //Material name
                                             Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                             //Đơn vị
                                             Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                         }).AsNoTracking().ToListAsync();

                return NHLTResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion

            #region XK
            //Màn xuất khác
            else if (type == "XK" && (!string.IsNullOrEmpty(resFrom) || (!string.IsNullOrEmpty(shipToPartyFrom))))
            {
                //Movement type xk
                var movementType = new List<string> { "Z42", "Z44", "Z46", "201" };

                //Tạo query
                var XKResponse = await _dtRsRepo.GetQuery()
                                         .Include(x => x.Reservation)
                                         .Where(x =>
                                                     //Lọc theo plant
                                                     x.Reservation.Plant == plant &&
                                                     //Lọc theo Movement type
                                                     movementType.Contains(x.MovementType) &&
                                                     //Lọc theo reservation
                                                     (!string.IsNullOrEmpty(resFrom) ? x.Reservation.ReservationCode.CompareTo(resFrom) >= 0 &&
                                                                                       x.Reservation.ReservationCode.CompareTo(resTo) <= 0 : true) &&
                                                     //Lọc theo customer
                                                     (!string.IsNullOrEmpty(shipToPartyFrom) ? x.Reservation.Customer.CompareTo(shipToPartyFrom) >=0 &&
                                                                                               x.Reservation.Customer.CompareTo(shipToPartyTo) <= 0 : true) &&
                                                     x.Reservation.FinalIssue != "X" &&
                                                     x.ItemDeleted != "X" &&
                                                     x.Reservation.Customer != null)
                                         .OrderBy(x => x.MaterialCodeInt)
                                         .Select(x => new DropdownMaterialResponse
                                         {
                                             //Material code
                                             Key = x.MaterialCodeInt.ToString(),
                                             //Material code | material name
                                             Value = $"{x.MaterialCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName}",
                                             //Material name
                                             Name = products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName,
                                             //Đơn vị
                                             Unit = products.FirstOrDefault(p => p.ProductCode == x.Material).Unit
                                         }).AsNoTracking().ToListAsync();

                return XKResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
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
        /// <summary>
        /// Dropdown mã nhà máy
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownPlant(string keyword)
        {
            var response = await _plantRepo.GetQuery(x => 
                                                        //Lọc theo từ khóa
                                                        !string.IsNullOrEmpty(keyword) ? x.PlantName.Contains(keyword) || x.PlantCode.Contains(keyword) : true)
                                     .OrderBy(x => x.PlantCode)
                                     .Select(x => new CommonResponse
                                     {
                                         //Mã nhà máy
                                         Key = x.PlantCode,
                                         //Mã nhà máy | tên nhà máy
                                         Value = $"{x.PlantCode} | {x.PlantName}" 
                                     }).Take(10).ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Sale Org
        /// <summary>
        /// Dropdown Sale Org
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Dropdown Purchasing Group
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownPurchasingGr(string keyword)
        {
            var response = await _purGrRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.PurchasingGroupName.Contains(keyword) || x.PurchasingGroupCode.Contains(keyword) : true) //Lọc theo từ khóa
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
        /// <summary>
        /// Dropdonw Purchasing Organization
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plantCode">Mã nhà máy</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownPurchasingOrgByPlant(string keyword, string plantCode)
        {
            var response = await _purOrgRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchasingOrgName.Contains(keyword) || x.PurchasingOrgCode.Contains(keyword) : true)) //Lọc theo từ khóa
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
        /// <summary>
        /// Dropdown vendor
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownVendor(string keyword, string type, string plant)
        {
            //Get query po
            var vendors = _vendorRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.VendorName.Contains(keyword) || x.VendorCode.Contains(keyword) : true).AsNoTracking();

            //Màn nhập kho mua hàng
            if (type == "NKMH")
            {
                //Get query po
                var res = await _poDetailRepo.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x =>
                                                        //Theo plant
                                                        (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&
                                                        //Loại các line đã hoàn tất chuyển kho
                                                        x.DeliveryCompleted != "X" &&
                                                        //Loại các line đã đánh dấu xóa
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X" &&
                                                        //Lấy các line đã được duyệt
                                                        //x.PurchaseOrder.ReleaseIndicator == "R" &&
                                                        //Lấy các line vendor không bị trống
                                                        x.PurchaseOrder.VendorCode != null &&
                                                        x.PurchaseOrder.VendorCode != "")
                                            .Select(x => new CommonResponse
                                            {
                                                //Mã vendor
                                                Key = x.PurchaseOrder.VendorCode,
                                                //Mã vendor | Tên vendor
                                                Value = $"{x.PurchaseOrder.VendorCode} | {vendors.FirstOrDefault(v => v.VendorCode == x.PurchaseOrder.VendorCode).VendorName}"
                                            })
                                            .AsNoTracking().ToListAsync();

                //Lọc theo từ khóa
                return res.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn xuất xnvl gia công
            if (type == "XNVLGC")
            {

                //Get list po code từ bảng reservation
                var pos = await _dtRsRepo.GetQuery().Select(x => x.PurchasingDoc).AsNoTracking().ToListAsync();

                //Get data 
                var res = await _poDetailRepo.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x => 
                                                        //Lọc theo plant
                                                        x.PurchaseOrder.Plant == plant &&
                                                        //Chỉ lấy những po nằm trong reservation
                                                        pos.Contains(x.PurchaseOrder.PurchaseOrderCode) &&
                                                        //Lọc các chứng từ có po type là Z003
                                                        x.PurchaseOrder.POType == "Z003" &&
                                                        //Loại trừ cái chứng từ đã hoàn thành giao dịch
                                                        x.DeliveryCompleted != "X" &&
                                                        //Loại các line đã đánh dấu xóa
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X" &&
                                                        x.PurchaseOrder.ReleaseIndicator == "R")
                                            .Select(x => new CommonResponse
                                            {
                                                //Mã vendor
                                                Key = x.PurchaseOrder.VendorCode,
                                                //Mã vendor | Tên vendor
                                                Value = $"{x.PurchaseOrder.VendorCode} | {vendors.FirstOrDefault(v => v.VendorCode == x.PurchaseOrder.VendorCode).VendorName}"
                                            })
                                            .AsNoTracking().ToListAsync();
                //Lọc theo từ khóa
                return res.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(10).ToList();
            }

            ////Lọc theo từ khóa
            var response = await _vendorRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.VendorName.Contains(keyword) || x.VendorCode.Contains(keyword) : true)
                .OrderBy(x => x.VendorCode)
                .Select(x => new CommonResponse
                {
                    //Mã vendor
                    Key = x.VendorCode,
                    //Mã vendor | Tên vendor
                    Value = $"{x.VendorCode} | {x.VendorName}"
                }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown PO Type
        /// <summary>
        /// Dropdown PO Type
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownPOType(string keyword, string plant, string vendorFrom, string vendorTo)
        {
            //Nếu chỉ có vendorfrom thì search 1
            if (!string.IsNullOrEmpty(vendorFrom)  && string.IsNullOrEmpty(vendorTo))
            {
                vendorTo = vendorFrom;
            }

            //Get query order type
            var orderType = _oTypeRep.GetQuery().AsNoTracking();

            var response = await _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder)
                                              .Where(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrder.POType.Contains(keyword) : true) &&
                                                             //Theo plant
                                                             (x.PurchaseOrder.Plant == plant) && 
                                                             //Theo vendor
                                                             (!string.IsNullOrEmpty(vendorFrom) ? x.PurchaseOrder.VendorCode.CompareTo(vendorFrom) >= 0 && 
                                                                                                  x.PurchaseOrder.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                             //Loại các line đã hoàn tất chuyển kho
                                                             x.DeliveryCompleted != "X" &&
                                                             //Loại các line đã đánh dấu xóa
                                                             x.DeletionInd != "X" &&
                                                             x.PurchaseOrder.DeletionInd != "X"
                                                             //Lấy các line đã duyệt
                                                             //x.PurchaseOrder.ReleaseIndicator == "R"
                                                             )
                                        .OrderBy(x => x.PurchaseOrder.POType)
                                        .Select(x => new CommonResponse
                                        {
                                            //PO Type
                                            Key = x.PurchaseOrder.POType,
                                            //PO Type | PO Type name
                                            Value = $"{x.PurchaseOrder.POType} | {orderType.FirstOrDefault(o => o.OrderTypeCode == x.PurchaseOrder.POType && o.Category == "01").ShortText}"
                                        }).AsNoTracking().ToListAsync();

            return response.DistinctBy(x => x.Key).ToList();
        }
        #endregion

        #region Dropdown PO
        /// <summary>
        /// Dropdown PO
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="poType">PO Type</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownPO(string keyword, string plant, 
                                                              string type, string poType, 
                                                              string vendorFrom, string vendorTo,
                                                              string materialFrom, string materialTo)
        {
            //Nếu chỉ search materialFrom thì search 1
            materialTo = !string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo) ? materialFrom : materialTo;

            //Nếu không search vendorto gán vendor to = vendor from
            if (!string.IsNullOrEmpty(vendorFrom) && string.IsNullOrEmpty(vendorTo))
                vendorTo = vendorFrom;

            //Màn hình nhập kho điều chuyển nội bộ
            if (type == "NKDCNB")
            {
                //Lấy danh sách po code
                var poCodes = await _poMasterRepo.GetQuery().Select(x => x.PurchaseOrderCode).ToListAsync();

                var NKDCNBResponse = await _dtOdRepo.GetQuery()
                                        .Include(x => x.OutboundDelivery)
                                        //Lọc delivery type
                                        .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                    (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                    //Lấy delivery đã hoàn tất giao dịch
                                                    x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                    x.GoodsMovementSts == "C" &&
                                                    x.ReferenceDocument1 != null &&
                                                    poCodes.Contains(x.ReferenceDocument1))
                                        .OrderBy(x => x.ReferenceDocument1)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.ReferenceDocument1,
                                            Value = long.Parse(x.ReferenceDocument1).ToString()
                                        }).AsNoTracking().ToListAsync();

                return NKDCNBResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }   
            //Màn xuất nvl gia công
            else if (type == "XNVLGC")
            {
                //Get list po code từ bảng reservation
                var pos = await _dtRsRepo.GetQuery().Select(x => x.PurchasingDoc).AsNoTracking().ToListAsync();

                var XNVLGCResponse = await _poMasterRepo.GetQuery(x =>     
                                                             //Lọc theo keyword
                                                             (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             //Lọc theo plant
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             //Chỉ lấy những po nằm trong reservation
                                                             pos.Contains(x.PurchaseOrderCode) &&
                                                             //Lọc theo po type
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

                return XNVLGCResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn nhập kho mua hàng
            else if (type == "NKMH")
            {
                var nkmhResponse = await _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder)
                                           .Where(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrder.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&            //Theo plant
                                                             (!string.IsNullOrEmpty(poType) ? x.PurchaseOrder.POType == poType : true) &&         //Theo potype
                                                             (!string.IsNullOrEmpty(vendorFrom) ? x.PurchaseOrder.VendorCode.CompareTo(vendorFrom) >= 0 &&        //Theo vendor
                                                                                                  x.PurchaseOrder.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                             //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                                    //x.ProductCodeInt <= long.Parse(materialTo) : true) && //Theo material
                                                             //(x.PurchaseOrder.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "L") &&
                                                             (x.DeliveryCompleted != "X"))
                                        .OrderBy(x => x.PurchaseOrder.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrder.PurchaseOrderCode,
                                            Value = x.PurchaseOrder.PurchaseOrderCodeInt.ToString()
                                        }).AsNoTracking().ToListAsync();

                return nkmhResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn xuất kho lxh
            else if (type == "XKLXH")
            {
                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                //Query po
                var poQuery = _poMasterRepo.GetQuery().AsNoTracking();

                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            (x.OutboundDelivery.GoodsMovementSts != "C") &&
                                            //Bỏ các line k có reference
                                            (x.ReferenceDocument1 != null) &&
                                            (poQuery.Count(p => p.PurchaseOrderCode == x.ReferenceDocument1) > 0) &&
                                            //Theo Keyword
                                            (!string.IsNullOrEmpty(keyword) ? x.ReferenceDocument1.Contains(keyword) ||
                                                                              x.ReferenceDocument1.Contains(keyword) : true)
                                            )
                                 .OrderBy(x => x.ReferenceDocument1)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.ReferenceDocument1,
                                     Value = long.Parse(x.ReferenceDocument1).ToString()
                                 }).AsNoTracking().ToListAsync();

                return xklxhResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }

            var response = await _poMasterRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.PurchaseOrderCode.Contains(keyword) : true) &&
                                                             (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                             (!string.IsNullOrEmpty(poType) ? x.POType == poType : true) &&
                                                             (!string.IsNullOrEmpty(vendorFrom) ? x.VendorCode.CompareTo(vendorFrom) >= 0 &&
                                                                                                  x.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                             (x.ReleaseIndicator == "R") &&
                                                             (x.DeletionInd != "X")).Include(x => x.PurchaseOrderDetailModel)
                                        .Where(x => x.PurchaseOrderDetailModel.FirstOrDefault(p => p.DeliveryCompleted != "X" && p.DeletionInd != "X") != null)
                                        .OrderBy(x => x.PurchaseOrderCode)
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.PurchaseOrderCode,
                                            Value = x.PurchaseOrderCodeInt.ToString()
                                        }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown đầu cân
        /// <summary>
        /// Dropdown đầu cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plantCode">Mã nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <returns></returns>
        public async Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode, string type)
        {

            //Lấy danh sách cân theo màn hình
            var scaleCodes = await _screenScaleRepo.GetQuery(x => !string.IsNullOrEmpty(type) ? x.ScreenCode == type : false).Select(x => x.ScaleCode).ToListAsync();

            var request = new GetDropdownWeighHeadRequest
            {
                KeyWord = keyword,
                Plant = plantCode,
                ScaleCodes = scaleCodes
            };

            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}get-weight-head";

            //Convert request to json
            var json = JsonConvert.SerializeObject(request);
            //Conver json to dictionary<string, string> => form
            var requestBody = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var scaleData = await _httpClient.PostAsync(url, requestBody);
            var scaleResponse = await scaleData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<ApiSuccessResponse<List<DropdownWeightHeadResponse>>>(scaleResponse, jsonSettings);

            if (!result.IsSuccess)
                return null;

            return result?.Data;
        }

        //public async Task<List<DropdownWeightHeadResponse>> GetDropdownWeightHeadByPlant(string keyword, string plantCode, string type)
        //{
        //    //Lấy danh sách id cân theo màn hình
        //    var scaleCodes = _screenScaleRepo.GetQuery(x => !string.IsNullOrEmpty(type) ? x.ScreenCode == type : false).Select(x => x.ScaleCode);


        //    var response = await _scaleRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.ScaleName.Contains(keyword) : true) &&
        //                                                  (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true) &&
        //                                                  (scaleCodes.Any() ? scaleCodes.Contains(x.ScaleCode) : true))
        //                            .OrderBy(x => x.ScaleCode)
        //                            .Select(x => new DropdownWeightHeadResponse
        //                            {
        //                                Key = x.ScaleCode,
        //                                Value = $"{x.ScaleCode} | {x.ScaleName}",
        //                                Data = x.ScaleType.Value == true ? true : false,
        //                                Type = x.isCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
        //                            }).AsNoTracking().ToListAsync();

        //    return response;
        //}
        #endregion

        #region Dropdown Sloc
        /// <summary>
        /// Dropdown Storage Location
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        public async Task<List<Common3Response>> GetDropdownSloc(string keyword, string plant)
        {
            var response = await _slocRepo.GetQuery(x => 
                                                         //Lọc theo từ khóa
                                                         (!string.IsNullOrEmpty(keyword) ? x.StorageLocationCode.Contains(keyword) || x.StorageLocationName.Contains(keyword) : true) &&
                                                         //Lọc theo plant
                                                         x.PlantCode == plant)
                                    .OrderBy(x => x.StorageLocationCode)
                                    .Select(x => new Common3Response
                                    {
                                        //Storage Location Code
                                        Key = x.StorageLocationCode,
                                        //Storage Location Code | Storage Location Name
                                        Value = $"{x.StorageLocationCode} | {x.StorageLocationName}",
                                        //Storage Location Name
                                        Name = x.StorageLocationName
                                    }).Take(10).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion

        #region GetWeightVote
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetWeightVote(string keyword)
        {
            return await _nkmhRep.GetQuery(x => 
                                                //Lọc theo từ khóa
                                                string.IsNullOrEmpty(keyword) ? true : x.WeitghtVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             //Số phiếu cân
                                             Key = x.WeitghtVote,
                                             Value = x.WeitghtVote
                                         }).Distinct().Take(20).ToListAsync();
        }
        #endregion

        #region Dropdown PoItem
        /// <summary>
        /// Dropdown POLine
        /// </summary>
        /// <param name="poCode">PurchaseOrderCode</param>
        /// <returns></returns>
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

        #region Dropdown Sales Orrder
        /// <summary>
        /// Dropdown Sales Order
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="orderType">OrderType</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownSaleOrder(string keyword, string plant, string type, string orderType)
        {
            //Khai bao mảng chức delivery type
            var deliveryType = new List<string>();

            //Query so
            var soQuery = _saleDocRepo.GetQuery().AsNoTracking();

            //Màn hình nhập kho hàng trả
            if (type == "NKHT")
            {
                //Delivery Type lấy ra
                deliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

                var NKHTResponse =  await _dtOdRepo.GetQuery()
                                       .Include(x => x.OutboundDelivery)
                                       .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) && //Lọc theo Plant
                                                   //Lọc theo từ khóa
                                                   (string.IsNullOrEmpty(keyword) ? true : x.ReferenceDocument1.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                   //Lọc theo delivery type
                                                   (deliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                                   //Lấy các line đã hoàn tất giao dịch
                                                   x.GoodsMovementSts != "C")
                                       .OrderBy(x => x.ReferenceDocument1)
                                       .Select(x => new CommonResponse
                                       {
                                           Key = x.ReferenceDocument1,
                                           Value = x.ReferenceDocument1
                                       }).AsNoTracking().ToListAsync();

                return NKHTResponse.DistinctBy(x => x.Key).Take(10).ToList();

            }
            //Màn xuất kho lxh
            else if (type == "XKLXH")
            {
                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                
                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            (x.OutboundDelivery.GoodsMovementSts != "C") &&
                                            //Bỏ các line k có reference
                                            (x.ReferenceDocument1 != null) &&
                                            (soQuery.FirstOrDefault(p => p.SalesDocumentCode == x.ReferenceDocument1) != null) &&
                                            //Theo Keyword
                                            (!string.IsNullOrEmpty(keyword) ? x.ReferenceDocument1.Contains(keyword) : true)
                                            )
                                 .OrderBy(x => x.ReferenceDocument1)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.ReferenceDocument1,
                                     Value = x.ReferenceDocument1
                                 }).AsNoTracking().ToListAsync();

                return xklxhResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình Nhập kho thành phẩm sản xuất
            else if (type == "NKTPSX")
            {
                //Tạo query
                var NKTPSXResponse = await _workOrderRep.GetQuery(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  //Loại System Status bắt đầu bằng "REL CNF" và "REL TECO"
                                                  !x.SystemStatus.StartsWith("REL CNF") &&
                                                  !x.SystemStatus.StartsWith("REL TECO") &&
                                                  //Loại các line có tích DeletionFlag
                                                  x.DeletionFlag != "X" &&
                                                  //Loại các line sales order trống
                                                  x.SalesOrder != null)
                                    .OrderBy(x => x.SalesOrder)
                                    .Select(x => new CommonResponse
                                    {
                                        Key = x.SalesOrder,
                                        Value = soQuery.FirstOrDefault(p => p.SalesDocumentCode == x.SalesOrder).SalesDocumentCode
                                    }).AsNoTracking().ToListAsync();

                return NKTPSXResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập kho phụ phẩm phế phẩm
            else if (type == "NKPPPP")
            {
                //Tạo query
                var NKTPSXResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder)
                                                  .Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy <= 0 &&
                                                  //Loại các line có tích DeletionFlag
                                                  x.WorkOrder.DeletionFlag != "X" &&
                                                  //Loại các line sales order trống
                                                  x.WorkOrder.SalesOrder != null)
                                    .OrderBy(x => x.WorkOrder.SalesOrder)
                                    .Select(x => new CommonResponse
                                    {
                                        Key = x.WorkOrder.SalesOrder,
                                        Value = soQuery.FirstOrDefault(p => p.SalesDocumentCode == x.WorkOrder.SalesOrder).SalesDocumentCode
                                    }).AsNoTracking().ToListAsync();

                return NKTPSXResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình xuất tiêu hao theo lệnh sản xuất
            else if (type == "XTHLSX")
            {
                //Tạo query
                var NKTPSXResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder)
                                                  .Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy > 0 &&
                                                  //Loại các line có tích DeletionFlag
                                                  x.WorkOrder.DeletionFlag != "X" &&
                                                  //Loại các line sales order trống
                                                  x.WorkOrder.SalesOrder != null)
                                    .OrderBy(x => x.WorkOrder.SalesOrder)
                                    .Select(x => new CommonResponse
                                    {
                                        Key = x.WorkOrder.SalesOrder,
                                        Value = soQuery.FirstOrDefault(p => p.SalesDocumentCode == x.WorkOrder.SalesOrder).SalesDocumentCode
                                    }).AsNoTracking().ToListAsync();

                return NKTPSXResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
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
        /// <summary>
        /// Dropdown Outbound Delivery
        /// </summary>
        /// <param name="type">Tên màn hình</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="deliveryType">DeliveryType</param>
        /// <param name="salesOrderFrom">SalesOrder</param>
        /// <param name="salesOrderTo">SalesOrder</param>
        /// <param name="shipToPartyFrom">ShipToParty</param>
        /// <param name="shipToPartyTo">ShipToParty</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownOutboundDelivery(string type, string plant,
                                                                            string deliveryType,
                                                                            string salesOrderFrom, string salesOrderTo,
                                                                            string shipToPartyFrom, string shipToPartyTo,
                                                                            string poFrom, string poTo,
                                                                            string materialFrom, string materialTo, string keyword)
        {
            var response = new List<CommonResponse>();

            if (!string.IsNullOrEmpty(salesOrderFrom) && string.IsNullOrEmpty(salesOrderTo))
                salesOrderTo = salesOrderFrom;

            //Nếu chỉ search shiptoparty from thì search 1
            shipToPartyTo = !string.IsNullOrEmpty(shipToPartyFrom) && string.IsNullOrEmpty(shipToPartyTo) ? shipToPartyFrom : shipToPartyTo;


            //Nếu chỉ search poFrom thì search 1
            poTo = !string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo) ? poFrom : poTo;

            //Nếu chỉ search shipToPartyFrom thì search 1
            shipToPartyTo = !string.IsNullOrEmpty(shipToPartyFrom) && string.IsNullOrEmpty(shipToPartyTo) ? shipToPartyFrom : shipToPartyTo;

            //Nếu chỉ search material from thì search 1
            materialTo = !string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo) ? materialFrom : materialTo;


            var query = _dtOdRepo.GetQuery()
                                   .Include(x => x.OutboundDelivery)
                                   .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) && //Lọc theo Plant
                                               //Lọc theo từ khóa
                                               (string.IsNullOrEmpty(keyword) ? true : x.OutboundDelivery.DeliveryCode.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                               //Lọc theo sales order
                                               (!string.IsNullOrEmpty(salesOrderFrom) ? x.ReferenceDocument1.CompareTo(salesOrderFrom) >= 0 &&
                                                                                        x.ReferenceDocument1.CompareTo(salesOrderTo) <= 0 : true))
                                   .AsNoTracking();

            //Lọc điều kiện
            //Màn hình nhập kho hàng trả
            if (type == "NKHT")
            {
                //Delivery Type lấy ra
                var NKHTdeliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

                var NKHTresponse = await _dtOdRepo.GetQuery()
                                       .Include(x => x.OutboundDelivery)
                                       .Where(x => (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) && //Plant
                                                   //Lọc theo từ khóa
                                                   (string.IsNullOrEmpty(keyword) ? true : x.OutboundDelivery.DeliveryCode.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                   //Theo sales order
                                                   (!string.IsNullOrEmpty(salesOrderFrom) ? x.ReferenceDocument1.CompareTo(salesOrderFrom) >= 0 &&  
                                                                                            x.ReferenceDocument1.CompareTo(salesOrderTo) <= 0 : true) &&
                                                   //Theo ship to party
                                                   (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&      
                                                                                             x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                                   //Theo material
                                                   //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                          //x.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                   //Theo delivery type
                                                   (NKHTdeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                                   //Loại các line đã hoàn tất giao dịch
                                                   x.GoodsMovementSts != "C")
                                       .OrderBy(x => x.OutboundDelivery.DeliveryCode)
                                       .Select(x => new CommonResponse
                                       {
                                           Key = x.OutboundDelivery.DeliveryCode,
                                           Value = x.OutboundDelivery.DeliveryCodeInt.ToString()
                                       }).AsNoTracking().ToListAsync();
                return NKHTresponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //màn hình xklxh
            else if (type == "XKLXH")
            {
                var deliveryTypeQuery = _oTypeRep.GetQuery().AsNoTracking();

                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo delivery type
                                            (!string.IsNullOrEmpty(deliveryType) ? x.OutboundDelivery.DeliveryType == deliveryType : true) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Theo po
                                            (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                             x.ReferenceDocument1.CompareTo(poTo) <= 0 : true) &&
                                            //Theo so
                                            (!string.IsNullOrEmpty(salesOrderFrom) ? x.ReferenceDocument1.CompareTo(salesOrderFrom) >= 0 &&
                                                                                     x.ReferenceDocument1.CompareTo(salesOrderTo) <= 0 : true) &&
                                            //Theo Shiptoparty
                                            (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                      x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                            //Theo Material
                                            //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                   //x.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            (x.OutboundDelivery.GoodsMovementSts != "C"))
                                 .OrderBy(x => x.OutboundDelivery.DeliveryCode)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.OutboundDelivery.DeliveryCode,
                                     Value = x.OutboundDelivery.DeliveryCodeInt.ToString()
                                 })
                                .AsNoTracking().ToListAsync();

                return xklxhResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Key.Contains(keyword) ||
                                                                                  x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập kho điều chuyển nội bộ
            else if (type == "NKDCNB")
            {
                var NKDCNBResponse = await _dtOdRepo.GetQuery()
                                                    .Include(x => x.OutboundDelivery)
                                                    //Lọc delivery type
                                                    .Where(x =>
                                                                (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                                (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                                //Lấy delivery đã hoàn tất giao dịch
                                                                x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                                x.GoodsMovementSts == "C" &&
                                                                //Lọc theo shiptoparty
                                                                (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                                          x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                                                //Lọc theo deliveryType
                                                                (!string.IsNullOrEmpty(deliveryType) ? x.OutboundDelivery.DeliveryType == deliveryType : true)
                                                                )
                                                    .OrderBy(x => x.OutboundDelivery.DeliveryCodeInt)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.OutboundDelivery.DeliveryCode,
                                                        Value = x.OutboundDelivery.DeliveryCode
                                                    }).AsNoTracking().ToListAsync();
                return NKDCNBResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Key.Contains(keyword) ||
                                                                                  x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập hàng loại T
            else if (type == "NHLT")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình NHLT
                var NHLTdeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9" };
                var NHLTResponse = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                         .Where(x => x.Plant == plant && 
                                                     x.OutboundDelivery.PODStatus == "A" &&
                                                     //Lọc theo customer
                                                     (!string.IsNullOrEmpty(shipToPartyFrom) ? x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyFrom) >= 0 &&
                                                                                               x.OutboundDelivery.ShiptoParty.CompareTo(shipToPartyTo) <= 0 : true) &&
                                                     //Lọc theo delivery type
                                                     NHLTdeliveryType.Contains(x.OutboundDelivery.DeliveryType))
                                         .OrderBy(x => x.OutboundDelivery.DeliveryCodeInt)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.OutboundDelivery.DeliveryCode,
                                                        Value = x.OutboundDelivery.DeliveryCode
                                                    }).AsNoTracking().ToListAsync(); ;

                return NHLTResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Key.Contains(keyword) ||
                                                                                  x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            } 
            //Ở màn hình ghi nhận cân xe tải không lấy dropdown theo master data => lấy theo dữ liệu đã lưu
            else if (type == "GNCXT")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình GNCXT
                var GNCXTdeliveryType = new List<string>(){ "ZLF1","ZLF2","ZLF3","ZLF4","ZLF5","ZLF6","ZLF7","ZLF8","ZLF9","ZLFA","ZNLC","ZNLN","ZXDH"};

                var GNCXTQuery = await _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                 .Where(x => x.PlantCode == plant &&
                                             //Lọc các dòng có deliveryType thuộc biến deliveryType
                                             GNCXTdeliveryType.Contains(x.DetailOD.OutboundDelivery.DeliveryType))
                                 .OrderBy(x => x.DetailOD.OutboundDelivery.DeliveryCodeInt)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.DetailOD.OutboundDelivery.DeliveryCode,
                                     Value = x.DetailOD.OutboundDelivery.DeliveryCodeInt.ToString()
                                 })
                                 .AsNoTracking().ToListAsync();

                return GNCXTQuery.DistinctBy(x => x.Key).Take(10).ToList();
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
        #endregion

        #region Dropdown Ship to party
        /// <summary>
        /// Dropdown Ship To Party
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="soFrom">Sales Order</param>
        /// <param name="soTo">Sales Order</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownShipToParty(string keyword, string plant, string type, string soFrom, string soTo, string poFrom, string poTo)
        {
            //Khai bao mảng chức delivery type
            var deliveryType = new List<string>();

            //Nếu chỉ search soFrom thì search
            soTo = !string.IsNullOrEmpty(soFrom) && string.IsNullOrEmpty(soTo) ? soFrom : soTo;

            //Nếu chỉ search poFrom thì search
            poTo = !string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo) ? poFrom : poTo;

            var response = new List<CommonResponse>();

            if (type == "NKHT")
            {
                //Delivery Type lấy ra
                deliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

                response = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                          .Where(x => (string.IsNullOrEmpty(keyword) ? true : x.OutboundDelivery.ShiptoParty.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                                                                                x.OutboundDelivery.ShiptoPartyName.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                      (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) && // Theo plant
                                                      (!string.IsNullOrEmpty(soFrom) ? x.ReferenceDocument1.CompareTo(soFrom) >= 0 &&
                                                                                       x.ReferenceDocument1.CompareTo(soTo) <= 0 : true) && //Theo SO
                                                      (deliveryType.Contains(x.OutboundDelivery.DeliveryType)) && 
                                                      (x.GoodsMovementSts != "C")
                                                      )
                                         .OrderBy(x => x.OutboundDelivery.ShiptoParty)
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.OutboundDelivery.ShiptoParty,
                                             Value = $"{x.OutboundDelivery.ShiptoParty} | {x.OutboundDelivery.ShiptoPartyName}"
                                         }).ToListAsync();

                return response.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn xuất kho lxh
            else if (type == "XKLXH")
            {
                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                //Query shiptoparty
                var shipToPartys = _vendorRepo.GetQuery().AsNoTracking();

                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            (x.OutboundDelivery.GoodsMovementSts != "C") &&
                                            //Theo po
                                            (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                             x.ReferenceDocument1.CompareTo(poTo) <= 0 : true) &&
                                            //Theo so
                                            (!string.IsNullOrEmpty(soFrom) ? x.ReferenceDocument1.CompareTo(soFrom) >= 0 &&
                                                                             x.ReferenceDocument1.CompareTo(soTo) <= 0 : true) &&
                                            (x.OutboundDelivery.ShiptoParty != null) &&
                                            //Theo Keyword
                                            (!string.IsNullOrEmpty(keyword) ? x.OutboundDelivery.ShiptoPartyName.Contains(keyword) ||
                                                                              x.OutboundDelivery.SoldtoParty.Contains(keyword) : true)
                                            )
                                 .OrderBy(x => x.OutboundDelivery.ShiptoParty)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.OutboundDelivery.ShiptoParty,
                                     Value = $"{x.OutboundDelivery.ShiptoParty} | {x.OutboundDelivery.ShiptoPartyName}"
                                 })
                                .AsNoTracking().ToListAsync();

                return xklxhResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập kho điều chuyển nội bộ
            else if (type == "NKDCNB")
            {
                var NKDCNBResponse = await _dtOdRepo.GetQuery()
                                                    .Include(x => x.OutboundDelivery)
                                                    //Lọc delivery type
                                                    .Where(x =>
                                                                //Theo plant 
                                                                (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                                //Theo keyword
                                                                (string.IsNullOrEmpty(keyword) ? true : x.OutboundDelivery.ShiptoParty.Contains(keyword) ||
                                                                                                x.OutboundDelivery.ShiptoPartyName.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                                                (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                                //Lấy delivery đã hoàn tất giao dịch
                                                                x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                                x.GoodsMovementSts == "C" &&
                                                                //Lọc theo po
                                                                (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                                                 x.ReferenceDocument1.CompareTo(poTo) <= 0 : true)
                                                                )
                                                    .OrderBy(x => x.OutboundDelivery.ShiptoParty)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.OutboundDelivery.ShiptoParty,
                                                        Value = $"{x.OutboundDelivery.ShiptoParty} | {x.OutboundDelivery.ShiptoPartyName}"
                                                    }).AsNoTracking().ToListAsync();
                return NKDCNBResponse.DistinctBy(x => x.Key).Take(10).ToList();
            }    


            response = await _obDeliveryRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.ShiptoParty.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
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
        /// <summary>
        /// Dropdown người tạo
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
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
        /// <summary>
        /// Dropdown số xe tải
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        public async Task<List<Common2Response>> GetDropdownTruckNumber(string keyword, string plant)
        {
            var response = await _truckInfoRepo.GetQuery(x => (string.IsNullOrEmpty(keyword) ? true : x.TruckNumber.Contains(keyword)) &&
                                                              //Lọc theo plant
                                                              x.PlantCode == plant &&
                                                              //Lấy ra các line được tạo trong ngày hiện tại
                                                              (x.CreateTime.HasValue ? x.CreateTime.Value.Date == DateTime.Now.Date &&
                                                                                       x.CreateTime.Value.Month == DateTime.Now.Month &&
                                                                                       x.CreateTime.Value.Year == DateTime.Now.Year: false))
                                  .OrderBy(x => x.TruckNumber).ThenByDescending(x => x.CreateTime)
                                  .AsNoTracking().ToListAsync();
                            //Group by lấy line đầu mỗi group để lấy được line được tạo gần nhất
            return response.GroupBy(x => x.TruckNumber, (k, v) => new { Key = k, Value = v.ToList() })
                                  .Select(x => new Common2Response
                                  {
                                      Key = x.Value.First().TruckInfoId,
                                      Value = x.Key
                                  }).ToList();
        }
        #endregion

        #region GetDropdownOutboundDeliveryItem
        /// <summary>
        /// Dropdown Outbound Delivery Item
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="odCode">OutboundDeliveryCode</param>
        /// <returns></returns>
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
        /// Dropdown OrderType
        /// </summary>
        /// <param name="plant">Nhà máy</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetOrderType(string plant, string keyword, string type,
                                                             string poFrom, string poTo)
        {
            var oTypeQuery = _oTypeRep.GetQuery().AsNoTracking();

            //Nếu chỉ search poFrom thì search 1
            poTo = !string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo) ? poFrom : poTo;

            //Khai bao mảng chức delivery type
            var deliveryType = new List<string>();

            if (type == "XKLXH")
            {
                //Delivery Type lấy ra
                var xklxhDeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

                var xklxhResponse = await _dtOdRepo.GetQuery()
                                .Include(x => x.OutboundDelivery)
                                .Where(x => //Search theo delivery type
                                            (xklxhDeliveryType.Contains(x.OutboundDelivery.DeliveryType)) &&
                                            //Theo plant
                                            (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                            //Điều kiện riêng của màn hình xklxh
                                            //Loại các line đã hoàn tất giao dịch
                                            (x.OutboundDelivery.GoodsMovementSts != "C"))
                                 .OrderBy(x => x.OutboundDelivery.DeliveryType)
                                 .Select(x => new CommonResponse
                                 {
                                     Key = x.OutboundDelivery.DeliveryType,
                                     Value = $"{x.OutboundDelivery.DeliveryType} | {oTypeQuery.FirstOrDefault(d => d.OrderTypeCode == x.OutboundDelivery.DeliveryType).ShortText}" 
                                 })
                                .AsNoTracking().ToListAsync();

                return xklxhResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Key.Contains(keyword) ||
                                                                                  x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }    
            else if (type == "NKTPSX")
            {
                //Tạo query
                var NKTPSXResponse = await _workOrderRep.GetQuery(x => 
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  //Loại System Status bắt đầu bằng "REL CNF" và "REL TECO"
                                                  !x.SystemStatus.StartsWith("REL CNF") &&
                                                  !x.SystemStatus.StartsWith("REL TECO") &&
                                                  //Loại các line có tích DeletionFlag
                                                  x.DeletionFlag != "X")
                                   .OrderBy(x => x.OrderTypeCode)
                                   .Select(x => new CommonResponse
                                   {
                                       Key = x.OrderTypeCode,
                                       Value = $"{x.OrderTypeCode} | {oTypeQuery.FirstOrDefault(d => d.OrderTypeCode == x.OrderTypeCode && d.Category == "02").ShortText}"
                                   })
                                   .AsNoTracking().ToListAsync();

                return NKTPSXResponse.Where(x => //Theo Keyword
                                                (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)
                                          ).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập kho điều chuyển nội bộ
            else if (type == "NKDCNB")
            {
                var NKDCNBResponse = await _dtOdRepo.GetQuery()
                                                    .Include(x => x.OutboundDelivery)
                                                    //Lọc delivery type
                                                    .Where(x =>
                                                                (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&
                                                                (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                                //Lấy delivery đã hoàn tất giao dịch
                                                                x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                                x.GoodsMovementSts == "C" &&
                                                                //Lọc theo po
                                                                (!string.IsNullOrEmpty(poFrom) ? x.ReferenceDocument1.CompareTo(poFrom) >= 0 &&
                                                                                                 x.ReferenceDocument1.CompareTo(poTo) <= 0 : true)
                                                                )
                                                    .OrderBy(x => x.OutboundDelivery.ShiptoParty)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.OutboundDelivery.DeliveryType,
                                                        Value = $"{x.OutboundDelivery.DeliveryType} | {oTypeQuery.FirstOrDefault(d => d.OrderTypeCode == x.OutboundDelivery.DeliveryType).ShortText}"
                                                    }).AsNoTracking().ToListAsync();
                return NKDCNBResponse.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình nhập kho phụ phẩm phế phẩm
            else if (type == "NKPPPP")
            {
                var NKDCNBResponse = await _dtWoRepo.GetQuery(x => (x.SystemStatus.StartsWith("REL") && x.RequirementQuantiy <= 0) &&
                                                                   (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true))
                                                    .Include(x => x.WorkOrder)
                                                    .OrderBy(x => x.WorkOrder.OrderTypeCode)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.WorkOrder.OrderTypeCode,
                                                        Value = $"{x.WorkOrder.OrderTypeCode} | {oTypeQuery.FirstOrDefault(d => d.OrderTypeCode == x.WorkOrder.OrderTypeCode).ShortText}"
                                                    }).AsNoTracking().ToListAsync();
                return NKDCNBResponse.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(10).ToList();
            }
            //Màn hình xuất tiêu hao lệnh sản xuất
            else if (type == "XTHLSX")
            {
                var XTHLSXResponse = await _dtWoRepo.GetQuery(x => (x.SystemStatus.StartsWith("REL") && x.RequirementQuantiy > 0) &&
                                                                   (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true))
                                                    .Include(x => x.WorkOrder)
                                                    .OrderBy(x => x.WorkOrder.OrderTypeCode)
                                                    .Select(x => new CommonResponse
                                                    {
                                                        Key = x.WorkOrder.OrderTypeCode,
                                                        Value = $"{x.WorkOrder.OrderTypeCode} | {oTypeQuery.FirstOrDefault(d => d.OrderTypeCode == x.WorkOrder.OrderTypeCode && d.Category == "02").ShortText}"
                                                    }).AsNoTracking().ToListAsync();
                return XTHLSXResponse.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(10).ToList();
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
        /// Drodpown work order
        /// </summary>
        /// <param name="plant">Nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="orderType">OrderType</param>
        /// <param name="materialFrom">Material</param>
        /// <param name="materialTo">Material</param>
        /// <param name="soFrom">SalesOrder</param>
        /// <param name="soTo">SalesOrder</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetWorkOrder(string plant, string type,
                                                             string orderType,
                                                             string materialFrom, string materialTo,
                                                             string soFrom, string soTo,
                                                             string keyword)
        {

            //Nếu ko có materialTo thì search 1
            materialTo = !string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo) ? materialFrom : materialTo;

            //Nếu ko có soTo thì search 1
            soTo = !string.IsNullOrEmpty(soFrom) && string.IsNullOrEmpty(soTo) ? soFrom : soTo;

            //Màn hình NKTPSX
            if (type == "NKTPSX")
            {
                return await _workOrderRep.GetQuery(x =>
                                                           //Lọc theo plant
                                                           x.Plant == plant &&
                                                           //Điều kiện riêng từng màn hình
                                                           x.SystemStatus.StartsWith("REL") &&
                                                           (!x.SystemStatus.StartsWith("REL CNF")) &&
                                                           (!x.SystemStatus.StartsWith("REL TECO")) &&
                                                           (x.DeletionFlag != "X") &&
                                                           //Theo order type
                                                           (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           //Theo keyword
                                                           (!string.IsNullOrEmpty(keyword) ? x.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                           //Theo material
                                                           //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                                  //x.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                           //Theo sales order
                                                           (!string.IsNullOrEmpty(soFrom) ? x.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                            x.SalesOrder.CompareTo(soTo) <= 0 : true)
                                                           )
                                  .OrderBy(x => x.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrderCode,
                                      Value = x.WorkOrderCodeInt.ToString()
                                  }).Take(20).ToListAsync();
            }
            //Màn hình NKPPPP
            else if (type == "NKPPPP")
            {
                var NKPPPPResponse = await _dtWoRepo.GetQuery().Include(x=> x.WorkOrder)
                                                 .Where(x =>
                                                           //Lọc theo plant
                                                           x.WorkOrder.Plant == plant &&
                                                           //Điều kiện riêng từng màn hình
                                                           x.SystemStatus.StartsWith("REL") && x.RequirementQuantiy <= 0 &&
                                                           //Theo order type
                                                           (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           //Theo keyword
                                                           (!string.IsNullOrEmpty(keyword) ? x.WorkOrder.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                           //Theo material
                                                           //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                           //x.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                           //Theo sales order
                                                           (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                            x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true)
                                                           )
                                  .OrderBy(x => x.WorkOrder.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrder.WorkOrderCode,
                                      Value = x.WorkOrder.WorkOrderCodeInt.ToString()
                                  }).ToListAsync();
                return NKPPPPResponse.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(20).ToList();
            }
            //Màn hình XTHLSX
            else if (type == "XTHLSX")
            {
                var XTHLSXResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder)
                                                 .Where(x =>
                                                           //Lọc theo plant
                                                           x.WorkOrder.Plant == plant &&
                                                           //Điều kiện riêng từng màn hình
                                                           x.SystemStatus.StartsWith("REL") && x.RequirementQuantiy > 0 &&
                                                           //Theo order type
                                                           (!string.IsNullOrEmpty(orderType) ? 
                                                                                x.WorkOrder.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           //Theo keyword
                                                           (!string.IsNullOrEmpty(keyword) ? 
                                                                                x.WorkOrder.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                           //Theo material
                                                           //(!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                           //x.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                           //Theo sales order
                                                           (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                            x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true)
                                                           )
                                  .OrderBy(x => x.WorkOrder.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrder.WorkOrderCode,
                                      Value = x.WorkOrder.WorkOrderCodeInt.ToString()
                                  }).ToListAsync();
                return XTHLSXResponse.Where(x => !string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true).DistinctBy(x => x.Key).Take(20).ToList();
            }
            
            var result = await _workOrderRep.GetQuery(x =>
                                                           //Lọc theo plant
                                                           x.Plant == plant &&
                                                           //Điều kiện riêng từng màn hình
                                                           (!x.SystemStatus.StartsWith("REL CNF")) &&
                                                           (!x.SystemStatus.StartsWith("TECO")) &&
                                                           (x.DeletionFlag != "X") &&
                                                           //Theo order type
                                                           (!string.IsNullOrEmpty(orderType) ? x.OrderTypeCode.Trim().ToUpper().Contains(orderType.Trim().ToUpper()) : true) &&
                                                           //Theo keyword
                                                           (!string.IsNullOrEmpty(keyword) ? x.WorkOrderCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) : true) &&
                                                           //Theo material
                                                           (!string.IsNullOrEmpty(materialFrom) ? x.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                                  x.ProductCodeInt <= long.Parse(materialTo): true))
                                  .OrderBy(x => x.WorkOrderCode)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.WorkOrderCode,
                                      Value = x.WorkOrderCodeInt.ToString()
                                  }).Take(20).ToListAsync();

            return result;
        }
        #endregion

        #region Dropdown Component
        /// <summary>
        /// Dropdown mã nguyên vật liệu
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <param name="woFrom">WorkOrder</param>
        /// <param name="woTo">WorkOrder</param>
        /// <param name="type">Tên nhà máy</param>
        /// <returns></returns>
        public async Task<List<DropdownMaterialResponse>> GetDropdownComponent(string keyword, string plant,
                                                                              string poFrom, string poTo,
                                                                              string woFrom, string woTo,
                                                                              string soFrom, string soTo,
                                                                              string materialFrom, string materialTo,
                                                                              string orderType,
                                                                              string vendorFrom, string vendorTo,
                                                                              string type)
        {
            var response = new List<DropdownMaterialResponse>();

            //Get query product
            var products = _prodRepo.GetQuery().AsNoTracking();

            #region XNVLGC
            //Màn hình xuất nguyên vật liệu gia công
            if (type == "XNVLGC")
            {
                if (!string.IsNullOrEmpty(poFrom) && string.IsNullOrEmpty(poTo))
                {
                    //Check nếu ko search field to thì gán to = from
                    poTo = poFrom;
                }

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo))
                    materialTo = materialFrom;

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(vendorFrom) && string.IsNullOrEmpty(vendorTo))
                    vendorTo = vendorFrom;

                //Get data
                var pos = await _poDetailRepo.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x =>
                                                        //Theo plant
                                                        (!string.IsNullOrEmpty(plant) ? x.PurchaseOrder.Plant == plant : true) &&
                                                        //Theo vendor
                                                        (!string.IsNullOrEmpty(vendorFrom) ? x.PurchaseOrder.VendorCode.CompareTo(vendorFrom) >= 0 &&
                                                                                             x.PurchaseOrder.VendorCode.CompareTo(vendorTo) <= 0 : true) &&
                                                        //Theo PuchaseOrder
                                                        (!string.IsNullOrEmpty(poFrom) ? x.PurchaseOrder.PurchaseOrderCodeInt >= long.Parse(poFrom) &&
                                                                                         x.PurchaseOrder.PurchaseOrderCodeInt <= long.Parse(poTo) : true) &&
                                                        //Loại các line đã hoàn tất chuyển kho
                                                        x.DeliveryCompleted != "X" &&
                                                        //Loại các line đã đánh dấu xóa
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X" &&
                                                        //Lấy các line đã được duyệt
                                                        x.PurchaseOrder.ReleaseIndicator == "R")
                                            .Select(x => x.PurchaseOrder.PurchaseOrderCode).AsNoTracking().ToListAsync();

                var XNVLGCResponse = await _dtRsRepo.GetQuery().Include(x => x.Reservation)
                                          .Where(x => 
                                                      (!string.IsNullOrEmpty(poFrom) ? pos.Contains(x.PurchasingDoc) : true) && //Lọc po from to
                                                      x.PurchasingDoc != null && x.Item != null)   
                                        .OrderBy(x => x.MaterialCodeInt)
                                        .Select(x => new DropdownMaterialResponse
                                        {
                                            //Mã nguyên vật liệu
                                            Key = x.MaterialCodeInt.ToString(),
                                            //Mã nguyên vật liệu | Tên nguyên vật liệu
                                            Value = $"{x.MaterialCodeInt} | {products.FirstOrDefault(x => x.ProductCode == x.ProductCode).ProductName}",
                                            //Tên nguyên vật liệu
                                            Name = products.FirstOrDefault(p => p.ProductCode == x.Material).ProductName,
                                            //Đơn vị
                                            Unit = products.FirstOrDefault(p => p.ProductCode == x.Material).Unit
                                        }).ToListAsync();

                return XNVLGCResponse.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion
            #region XTHLSX                
            //Xuất tiêu theo lệnh xuất hàng
            else if (type == "XTHLSX")
            {
                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo))
                    materialTo = materialFrom;

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(soFrom) && string.IsNullOrEmpty(soTo))
                    soTo = soFrom;

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(woFrom) && string.IsNullOrEmpty(woTo))
                    woTo = woFrom;

                //Tạo query
                var XTHLSXResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder).Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo material
                                                  (!string.IsNullOrEmpty(materialFrom) ? x.WorkOrder.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                         x.WorkOrder.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo sale order
                                                  (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                   x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true) &&
                                                  //Lọc theo workorder
                                                  (!string.IsNullOrEmpty(woFrom) ? x.WorkOrder.WorkOrderCode.CompareTo(woFrom) >= 0 &&
                                                                                   x.WorkOrder.WorkOrderCode.CompareTo(woTo) <= 0 : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy > 0)
                                            .OrderBy(x => x.ProductCodeInt)
                                            .Select(x => new DropdownMaterialResponse
                                            {
                                                //Material code
                                                Key = x.ProductCodeInt.ToString(),
                                                //Material code | material name
                                                Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                                //Material name
                                                Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                                //Đơn vị
                                                Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                            }).ToListAsync();

                return XTHLSXResponse.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion
            #region NKPPPP                
            //Nhập kho phụ phẩm
            else if (type == "NKPPPP")
            {
                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(materialFrom) && string.IsNullOrEmpty(materialTo))
                    materialTo = materialFrom;

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(soFrom) && string.IsNullOrEmpty(soTo))
                    soTo = soFrom;

                //Check nếu ko search field to thì gán to = from
                if (!string.IsNullOrEmpty(woFrom) && string.IsNullOrEmpty(woTo))
                    woTo = woFrom;

                //Tạo query
                var NKPPPPResponse = await _dtWoRepo.GetQuery().Include(x => x.WorkOrder).Where(x =>
                                                  //Lọc theo plant
                                                  (!string.IsNullOrEmpty(plant) ? x.WorkOrder.Plant == plant : true) &&
                                                  //Lọc theo material
                                                  (!string.IsNullOrEmpty(materialFrom) ? x.WorkOrder.ProductCodeInt >= long.Parse(materialFrom) &&
                                                                                         x.WorkOrder.ProductCodeInt <= long.Parse(materialTo) : true) &&
                                                  //Lọc theo order type
                                                  (!string.IsNullOrEmpty(orderType) ? x.WorkOrder.OrderTypeCode == orderType : true) &&
                                                  //Lọc theo sale order
                                                  (!string.IsNullOrEmpty(soFrom) ? x.WorkOrder.SalesOrder.CompareTo(soFrom) >= 0 &&
                                                                                   x.WorkOrder.SalesOrder.CompareTo(soTo) <= 0 : true) &&
                                                  //Lọc theo workorder
                                                  (!string.IsNullOrEmpty(woFrom) ? x.WorkOrder.WorkOrderCode.CompareTo(woFrom) >= 0 &&
                                                                                   x.WorkOrder.WorkOrderCode.CompareTo(woTo) <= 0 : true) &&
                                                  //Lọc theo điều kiện riêng từng màn hình
                                                  //System Status bắt đầu bằng "REL"
                                                  x.SystemStatus.StartsWith("REL") &&
                                                  x.RequirementQuantiy <= 0)
                                            .OrderBy(x => x.ProductCodeInt)
                                            .Select(x => new DropdownMaterialResponse
                                            {
                                                //Material code
                                                Key = x.ProductCodeInt.ToString(),
                                                //Material code | material name
                                                Value = $"{x.ProductCodeInt} | {products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName}",
                                                //Material name
                                                Name = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                                                //Đơn vị
                                                Unit = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Unit
                                            }).ToListAsync();

                return NKPPPPResponse.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
            }
            #endregion
            else
            {
                response = await _prodRepo.GetQuery(x => 
                                                   //Lọc theo plant
                                                   (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true)
                                                   )
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new DropdownMaterialResponse
                                    {
                                        //Mã nguyên vật liệu
                                        Key = x.ProductCodeInt.ToString(),
                                        //Mã nguyên vật liệu | Tên nguyên vật liệu
                                        Value = $"{x.ProductCodeInt} | {x.ProductName}",
                                        //Tên nguyên vật liệu
                                        Name = x.ProductName,
                                        //Đơn vị
                                        Unit = x.Unit
                                    }).ToListAsync();
            }

            return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Value.Contains(keyword) : true)).DistinctBy(x => x.Key).Take(10).ToList();
        }
        #endregion

        #region Dropdown reservation
        /// <summary>
        /// Dropdown reservation
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetReservation(string keyword, string plant, string type)
        {
            //Màn xuất khác
            if (type == "XK")
            {
                //Movement type xk
                var movementType = new List<string> { "Z42", "Z44", "Z46", "201" };

                //Tạo query
                return await _dtRsRepo.GetQuery()
                                         .Include(x => x.Reservation)
                                         .Where(x =>
                                                     //Lọc theo plant
                                                     x.Reservation.Plant == plant &&
                                                     //Lọc theo Movement type
                                                     movementType.Contains(x.MovementType) &&
                                                     x.Reservation.FinalIssue != "X" &&
                                                     x.ItemDeleted != "X")
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.Reservation.ReservationCode,
                                             Value = x.Reservation.ReservationCodeInt.ToString()
                                         }).AsNoTracking().Take(10).ToListAsync();
            }
            return await _rsRepo.GetQuery(x =>
                                               //Lọc theo từ khóa
                                               (!string.IsNullOrEmpty(keyword) ? x.ReservationCode.ToLower().Contains(keyword.ToLower().Trim()) : true) &&
                                               //Lọc theo plant
                                               (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true))
                                .OrderBy(x => x.ReservationCodeInt)
                                .Select(x => new CommonResponse
                                {
                                    Key = x.ReservationCode,
                                    Value = x.ReservationCodeInt.ToString()
                                }).AsNoTracking().Take(10).ToListAsync();

        }

        #endregion

        #region Dropdown component item theo wo
        /// <summary>
        /// Dropdown component item theo WorkOrderCode
        /// </summary>
        /// <param name="workorder">WorkOrderCode</param>
        /// <returns></returns>
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

        #region Dropdown customer
        /// <summary>
        /// Dropdown Customer
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="odFrom">OutboundDelivery</param>
        /// <param name="odTo">OutboundDelivery</param>
        /// <param name="type">Tên nhà máy</param>
        /// <returns></returns>
        public async Task<List<Common3Response>> GetDropdownCustomer(string keyword, string plant, string odFrom, string odTo, string resFrom, string resTo, string type)
        {
            var response = new List<Common3Response>();

            //Nếu ko search to thì search 1
            resTo = !string.IsNullOrEmpty(resFrom) && string.IsNullOrEmpty(resTo) ? resFrom : resTo;

            //Get query customer
            var queryCustomer = _custRepo.GetQuery().AsNoTracking();

            //Màn hình nhập hàng loại T
            if (type == "NHLT")
            {
                //Gán giá trị cho biến deliveryType khi searh màn hình NHLT
                var NHLTdeliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9" };

                response = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                        .Where(x =>
                                                    //Lọc plant
                                                    (!string.IsNullOrEmpty(plant) ? x.Plant == plant : true) &&                                                          
                                                    //Lọc delivery type
                                                    (NHLTdeliveryType.Contains(x.OutboundDelivery.DeliveryType) &&
                                                    (x.OutboundDelivery.PODStatus == "A"))
                                                    )   
                                        .Select(x => new Common3Response
                                        {
                                            //Ship to party
                                            Key = x.OutboundDelivery.ShiptoParty,
                                            //Ship to party | Ship to party Name
                                            Value = $"{x.OutboundDelivery.ShiptoParty} | {x.OutboundDelivery.ShiptoPartyName}",
                                            //Ship to party Name
                                            Name = x.OutboundDelivery.ShiptoPartyName
                                        }).AsNoTracking().ToListAsync();   
            }
            //Màn xuất khác
            else if (type == "XK")
            {
                //Movement type xk
                var movementType = new List<string> { "Z42", "Z44", "Z46", "201" };

                //Tạo query
                var query = _dtRsRepo.GetQuery()
                                         .Include(x => x.Reservation)
                                         .Where(x =>
                                                     //Lọc theo plant
                                                     x.Reservation.Plant == plant &&
                                                     //Lọc theo Movement type
                                                     movementType.Contains(x.MovementType) &&
                                                     //Lọc theo reservation
                                                     (!string.IsNullOrEmpty(resFrom) ? x.Reservation.ReservationCode.CompareTo(resFrom) >= 0 &&
                                                                                       x.Reservation.ReservationCode.CompareTo(resTo) <= 0 : true) &&
                                                     x.Reservation.FinalIssue != "X" &&
                                                     x.ItemDeleted != "X" &&
                                                     x.Reservation.Customer != null)
                                         .Select(x => new Common3Response
                                         {
                                             //Ship to party
                                             Key = x.Reservation.Customer,
                                             //Ship to party | Ship to party Name
                                             Value = $"{x.Reservation.Customer} | {queryCustomer.FirstOrDefault(c => c.CustomerNumber == x.Reservation.Customer).CustomerName}",
                                             //Ship to party Name
                                             Name = queryCustomer.FirstOrDefault(c => c.CustomerNumber == x.Reservation.Customer).CustomerName
                                         }).AsNoTracking().ToListAsync();
            }
            //Không có loại màn hình thì search tất cả của bảng master data
            else
            {
                response = await _custRepo.GetQuery()
                                   .Select(x => new Common3Response
                                   {
                                       //Customer
                                       Key = x.CustomerNumber,
                                       //Customer | Customer name
                                       Value = $"{x.CustomerNumber} | {x.CustomerName}",
                                       //Customer name
                                       Name = x.CustomerName
                                   }).AsNoTracking().ToListAsync();
            }

            return response.Where(x => (!string.IsNullOrEmpty(keyword) ? x.Key.ToLower().Contains(keyword.ToLower().Trim()) : true) ||
                                       (!string.IsNullOrEmpty(keyword) ? x.Name.ToLower().Contains(keyword.ToLower().Trim()) : true)).OrderBy(x => x.Key).DistinctBy(x => x.Key).Take(10).ToList();
        }
        #endregion

        #region Get Scale Monitor Type
        /// <summary>
        /// Dropdown loại hoạt động cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
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
        /// Dropdown reservation item
        /// </summary>
        /// <param name="reservation">Reservation</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetReservationItem(string reservation, string keyword)
        {
            var result = await _dtRsRepo.GetQuery().Include(x => x.Reservation)
                                  .Where(x => 
                                              //Lấy item theo reservation code
                                              !string.IsNullOrEmpty(reservation) ? x.Reservation.ReservationCodeInt == long.Parse(reservation) : false &&
                                              //Lọc theo từ khóa
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
        /// <summary>
        /// Dropdown material document
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetMatDoc(string keyword, string plant)
        {
            var response = await _matDocRepo.GetQuery(x => 
                                                           //Lọc theo plant
                                                           (!string.IsNullOrEmpty(plant) ? x.PlantCode == plant : true) &&
                                                           //Lọc theo từ khóa
                                                           (!string.IsNullOrEmpty(keyword) ? x.MaterialDocCode.ToLower().Contains(keyword.ToLower().Trim()) : true) &&
                                                           //Lấy các line có movement type là "313"
                                                           (x.MovementType == "313") &&
                                                           //Lấy các line có đánh dấu Auto Created
                                                           (x.ItemAutoCreated == "X") &&
                                                           //Loại các line có movement type là "315"
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
        /// <summary>
        /// Dropdown MaterialDocumentItem
        /// </summary>
        /// <param name="matdoc">MaterialDocCode</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetMatDocItem(string matdoc, string keyword)
        {
            return  await _matDocRepo.GetQuery(x =>
                                                   //Lọc theo material documet code
                                                   (!string.IsNullOrEmpty(matdoc) ? x.MaterialDocCode == matdoc : false) &&
                                                   //Lọc theo từ khóa
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
        /// <summary>
        /// Dropdown trạng thái cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetScaleStatus(string keyword)
        {
            var result = await _cataRepo.GetQuery(x => 
                                                       //Lọc theo từ khóa
                                                       (!string.IsNullOrEmpty(keyword) ? x.CatalogCode.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) &&
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
        /// <summary>
        /// Dropdown màn hình
        /// </summary>
        /// <returns></returns>
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

        #region Get common date
        public async Task<List<CommonResponse>> GetCommonDate()
        {
            return await _cataRepo.GetQuery(x => x.CatalogTypeCode == "CommonDate").OrderBy(x => x.OrderIndex)
                                    .Select(x => new CommonResponse()
                                    {
                                        Key = x.CatalogCode,
                                        Value = $"{x.CatalogCode} | {x.CatalogText_vi}"
                                    }).ToListAsync();
        }

        #endregion

        #region Dropdown id đợt cân
        /// <summary>
        /// Dropdown Id đợt cân
        /// </summary>
        /// <param name="ScaleCode">Mã đầu cân</param>
        /// <param name="DateFrom">Từ ngày</param>
        /// <param name="DateTo">Đến ngày</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropdownWeighSessionCode(string keyword, string ScaleCode, DateTime? DateFrom, DateTime? DateTo)
        {
            //Get query đimdate
            var dimdate = _dimdateRepo.GetQuery().AsNoTracking();

            //Không search ngày thì lấy 30 ngày từ ngày hiện tại
            #region Format Day
            if (!DateFrom.HasValue && !DateTo.HasValue)
            {
                DateFrom = DateTime.Now.Date.AddDays(-30);
                DateTo = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }
            else if (DateFrom.HasValue && !DateTo.HasValue)
            {
                DateFrom = DateFrom.Value.Date;
                DateTo = DateFrom.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Chuyển ngày tháng thành datekey
            var dateKeyFrom = dimdate.FirstOrDefault(x => x.Date == DateFrom.Value.Date).DateKey.ToString();
            var dateKeyTo = dimdate.FirstOrDefault(x => x.Date == DateTo.Value.Date).DateKey.ToString();

            //Lấy đợt cân
            return await _weighSsRepo.GetQuery(x => 
                                             //Lọc theo keyword
                                             (!string.IsNullOrEmpty(keyword) ? x.WeighSessionCode.Contains(keyword) : true) &&
                                             //Lọc theo đầu cân
                                             (!string.IsNullOrEmpty(ScaleCode) ? x.ScaleCode == ScaleCode : true) &&
                                             //Theo ngày cân
                                             x.DateKey.CompareTo(dateKeyFrom) >= 0 && 
                                             x.DateKey.CompareTo(dateKeyTo) <=0
                                         ).Select(x => new CommonResponse()
                                         {
                                             Key = x.WeighSessionCode,
                                             Value = x.WeighSessionCode,
                                         }).AsNoTracking().ToListAsync();
        }
        #endregion
    }
}
