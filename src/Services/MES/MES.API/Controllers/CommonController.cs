using Core.Models;
using Core.Properties;
using MES.Application.DTOs.Common;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/MasterData/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonQuery _commonQuery;

        public CommonController(ICommonQuery commonQuery)
        {
            _commonQuery = commonQuery;
        }

        #region Lấy dropdown plant
        /// <summary>
        /// Dropdown mã nhà máy
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-plant")]
        public async Task<IActionResult> GetDropdownPlant(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownPlant(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách plant") });
        }
        #endregion

        #region Lấy dropdown sale org
        /// <summary>
        /// Lấy dropdown sale org
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-dropdown-sale-org")]
        public async Task<IActionResult> GetListSaleOrg()
        {
            var dropdownList = await _commonQuery.GetDropdownSaleOrg();
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách sale org") });
        }
        #endregion

        #region Lấy dropdown material
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
        [HttpGet("list-dropdown-material")]
        public async Task<IActionResult> GetListMaterial(string keyword, string plant,
                                                         string poFrom, string poTo,
                                                         string odFrom, string odTo, string deliveryType,
                                                         string woFrom, string woTo, string orderType,
                                                         string resFrom, string resTo,
                                                         string soFrom, string soTo,
                                                         string vendorFrom, string vendorTo,
                                                         string shipToPartyFrom, string shipToPartyTo,
                                                         string poType, string type)
        {
            var dropdownList = await _commonQuery.GetDropdownMaterial(keyword, plant, 
                                                                      poFrom, poTo, 
                                                                      odFrom, odTo, deliveryType,
                                                                      woFrom, woTo, orderType,
                                                                      resFrom, resTo, 
                                                                      soFrom, soTo, 
                                                                      vendorFrom, vendorTo, 
                                                                      shipToPartyFrom, shipToPartyTo, 
                                                                      poType, type);
            return Ok(new ApiSuccessResponse<List<DropdownMaterialResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách material") });
        }
        #endregion

        #region Lấy dropdown Component
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
        [HttpGet("list-dropdown-component")]
        public async Task<IActionResult> GetListComponent(string keyword, string plant,
                                                          string poFrom, string poTo,
                                                          string woFrom, string woTo,
                                                          string type)
        {
            var dropdownList = await _commonQuery.GetDropdownComponent(keyword, plant, poFrom, poTo, woFrom, woTo, type);
            return Ok(new ApiSuccessResponse<List<DropdownMaterialResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách component") });
        }
        #endregion

        #region Lấy dropdown Item Component
        /// <summary>
        /// Dropdown component item theo WorkOrderCode
        /// </summary>
        /// <param name="workorder">WorkOrderCode</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-item-component")]
        public async Task<IActionResult> GetListItemComponent(string wo)
        {
            var dropdownList = await _commonQuery.GetDropdownItemComponent(wo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách item component") });
        }
        #endregion

        #region Lấy Purchasing Org theo Plant Code
        /// <summary>
        /// Dropdonw Purchasing Organization
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <param name="plantCode">Mã nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-purchasingorg-by-plant")]
        public async Task<IActionResult> GetListPurchasingOrgByPlant(string keyword, string plantCode)
        {
            var dropdownList = await _commonQuery.GetDropdownPurchasingOrgByPlant(keyword,plantCode);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách purchasing org") });
        }
        #endregion

        #region Lấy dropdown purchasing gr
        /// <summary>
        /// Dropdown Purchasing Group
        /// </summary>
        /// <param name="keyword">Từ khóa tìm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-purchasing-gr")]
        public async Task<IActionResult> GetListPurchasingGr(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownPurchasingGr(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách purchasing gr") });
        }
        #endregion

        #region Lấy dropdown vendor
        /// <summary>
        /// Dropdown vendor
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-vendor")]
        public async Task<IActionResult> GetListVendor(string keyword, string type, string plant)
        {
            var dropdownList = await _commonQuery.GetDropdownVendor(keyword, type, plant);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách vendor") });
        }
        #endregion

        #region Lấy dropdown POType
        /// <summary>
        /// Dropdown PO Type
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="vendorFrom">Vendor</param>
        /// <param name="vendorTo">Vendor</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-potype")]
        public async Task<IActionResult> GetListPOType(string keyword, string plant, string vendorFrom, string vendorTo)
        {
            var dropdownList = await _commonQuery.GetDropdownPOType(keyword, plant, vendorFrom, vendorTo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po type") });
        }
        #endregion

        #region Lấy dropdown PO
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
        [HttpGet("list-dropdown-po")]
        public async Task<IActionResult> GetListPO(string keyword, string plant, string type, 
                                                   string poType, 
                                                   string vendorFrom, string vendorTo, 
                                                   string materialFrom, string materialTo)
        {
            var dropdownList = await _commonQuery.GetDropdownPO(keyword, plant, type, poType, vendorFrom, vendorTo, materialFrom, materialTo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po") });
        }
        #endregion

        #region Lấy dropdown PO Item
        /// <summary>
        /// Dropdown POLine
        /// </summary>
        /// <param name="poCode">PurchaseOrderCode</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-po-item")]
        public async Task<IActionResult> GetListPOItem(string poCode)
        {
            var dropdownList = await _commonQuery.GetDropdownPOItem(poCode);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po item") });
        }
        #endregion

        #region Lấy dropdown đầu cân theo plant
        /// <summary>
        /// Dropdown WeightHead
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-weight-head-by-plant")]
        public async Task<IActionResult> GetListWeightHeadByPlant(string keyword, string plantCode, string type)
        {
            //Lấy danh sách mã đầu cân đã được chọn

            //Query những thằng chưa được chọn
            var dropdownList = await _commonQuery.GetDropdownWeightHeadByPlant(keyword, plantCode, type);
            return Ok(new ApiSuccessResponse<List<DropdownWeightHeadResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách đầu cân") });
        }
        //[httpget("list-dropdown-weight-head-by-plant")]
        //public async task<iactionresult> getlistweightheadbyplant(string keyword, string plantcode, string type)
        //{
        //    //lấy danh sách mã đầu cân đã được chọn

        //    //query những thằng chưa được chọn
        //    var dropdownlist = await _commonquery.getdropdownweightheadbyplant(keyword, plantcode, type);
        //    return ok(new apisuccessresponse<list<dropdownweightheadresponse>> { data = dropdownlist, message = string.format(commonresource.msg_success, "lấy danh sách đầu cân") });
        //}
        #endregion

        #region Lấy dropdown sloc
        /// <summary>
        /// Dropdown Storage Location
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-sloc")]
        public async Task<IActionResult> GetListSloc(string keyword, string plant)
        {
            var dropdownList = await _commonQuery.GetDropdownSloc(keyword, plant);
            return Ok(new ApiSuccessResponse<List<Common3Response>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách sloc") });
        }
        #endregion

        #region Lấy dropdown create by
        /// <summary>
        /// Dropdown người tạo
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-create-by")]
        public async Task<IActionResult> GetListCreateBy(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownCreateBy(keyword);
            return Ok(new ApiSuccessResponse<List<Common2Response>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách người tạo") });
        }
        #endregion

        #region Lấy dropdown SaleOrder (SaleDocumentModel)
        /// <summary>
        /// Dropdown Sales Order
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Mã nhà máy</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="orderType">OrderType</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-sale-order")]
        public async Task<IActionResult> GetListSaleOrder(string keyword, string plant, string type, string orderType)
        {
            var dropdownList = await _commonQuery.GetDropdownSaleOrder(keyword, plant, type, orderType);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách sale order") });
        }
        #endregion

        #region Lấy dropdown Outbound Delivery
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
        [HttpGet("list-dropdown-outbound-delivery")]
        public async Task<IActionResult> GetListOutboundDelivery(string type, string plant,
                                                               string deliveryType,
                                                               string salesOrderFrom, string salesOrderTo,
                                                               string shipToPartyFrom, string shipToPartyTo,
                                                               string poFrom, string poTo,
                                                               string materialFrom, string materialTo, string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownOutboundDelivery(type, plant,
                                                                              deliveryType,
                                                                              salesOrderFrom, salesOrderTo, 
                                                                              shipToPartyFrom, shipToPartyTo, 
                                                                              poFrom, poTo, 
                                                                              materialFrom, materialTo, 
                                                                              keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách outbound delivery") });
        }
        #endregion

        #region Lấy dropdown Outbound Delivery Item theo od
        /// <summary>
        /// Dropdown Outbound Delivery Item
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-od-item")]
        public async Task<IActionResult> GetListODItem(string keyword, string odCode)
        {
            var dropdownList = await _commonQuery.GetDropdownOutboundDeliveryItem(keyword, odCode);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách outbound delivery item") });
        }
        #endregion

        #region Lấy dropdown Ship to Party
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
        [HttpGet("list-dropdown-ship-to-party")]
        public async Task<IActionResult> GetListShipToParty(string keyword, string plant, string type, string soFrom, string soTo, string poFrom, string poTo)
        {
            var dropdownList = await _commonQuery.GetDropdownShipToParty(keyword, plant, type, soFrom, soTo, poFrom, poTo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách ship to party") });
        }
        #endregion

        #region Lấy dropdown Số xe tải
        /// <summary>
        /// Dropdown số xe tải
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-dropdown-truck-number")]
        public async Task<IActionResult> GetListTruckNumber(string keyword, string plant)
        {
            var dropdownList = await _commonQuery.GetDropdownTruckNumber(keyword, plant);
            return Ok(new ApiSuccessResponse<List<Common2Response>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách truck number") });
        }
        #endregion

        #region Get số phiếu cân
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-weight-vote")]
        public async Task<IActionResult> GetWeightVoteAsync(string keyword)
        {
            var dropdownList = await _commonQuery.GetWeightVote(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown OrderType
        /// <summary>
        /// Dropdown OrderType
        /// </summary>
        /// <param name="plant">Nhà máy</param>
        /// <param name="keyword">Từ khóa</param>
        /// <param name="type">Tên màn hình</param>
        /// <param name="poFrom">PurchaseOrder</param>
        /// <param name="poTo">PurchaseOrder</param>
        /// <returns></returns>
        [HttpGet("list-order-type")]
        public async Task<IActionResult> GetOrderTypeAsync([Required]string plant, string keyword, string type, string poFrom, string poTo)
        {
            var dropdownList = await _commonQuery.GetOrderType(plant, keyword, type, poFrom, poTo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown WorkOrder
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
        [HttpGet("list-work-order")]
        public async Task<IActionResult> GetWorkOrderAsync(string plant, string type,
                                                           string orderType, 
                                                           string materialFrom, string materialTo, 
                                                           string soFrom, string soTo, 
                                                           string keyword)
        {
            var dropdownList = await _commonQuery.GetWorkOrder(plant, type,
                                                               orderType, 
                                                               materialFrom, materialTo, 
                                                               soFrom, soTo, 
                                                               keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Reservation
        /// <summary>
        /// Dropdown reservation
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-reservation")]
        public async Task<IActionResult> GetReservationAsync(string keyword, string plant)
        {
            var dropdownList = await _commonQuery.GetReservation(keyword, plant);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Customer
        /// <summary>
        /// Dropdown Customer
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <param name="odFrom">OutboundDelivery</param>
        /// <param name="odTo">OutboundDelivery</param>
        /// <param name="type">Tên nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-customer")]
        public async Task<IActionResult> GetCustomerAsync(string keyword, string plant, string odFrom, string odTo, string type)
        {
            var dropdownList = await _commonQuery.GetDropdownCustomer(keyword, plant, odFrom, odTo, type);
            return Ok(new ApiSuccessResponse<List<Common3Response>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Scale monitor type
        /// <summary>
        /// Dropdown loại hoạt động cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-scale-monitor-type")]
        public async Task<IActionResult> GetScaleMonitorTypeAsync(string keyword)
        {
            var dropdownList = await _commonQuery.GetScaleMonitorType(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Reservation item by Reservation
        /// <summary>
        /// Dropdown reservation item
        /// </summary>
        /// <param name="reservation">Reservation</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-reservation-item")]
        public async Task<IActionResult> GetReservationItemAsync(string reservation, string keyword)
        {
            var dropdownList = await _commonQuery.GetReservationItem(reservation ,keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Material Doc
        /// <summary>
        /// Dropdown material document
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <param name="plant">Nhà máy</param>
        /// <returns></returns>
        [HttpGet("list-mat-doc")]
        public async Task<IActionResult> GetMatDocAsync(string keyword, string plant)
        {
            var dropdownList = await _commonQuery.GetMatDoc(keyword, plant);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Material Doc Item
        /// <summary>
        /// Dropdown MaterialDocumentItem
        /// </summary>
        /// <param name="matdoc">MaterialDocCode</param>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-mat-doc-item")]
        public async Task<IActionResult> GetMatDocItemAsync(string matdoc, string keyword)
        {
            var dropdownList = await _commonQuery.GetMatDocItem(matdoc, keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Scale Status
        /// <summary>
        /// Dropdown trạng thái cân
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm gần đúng</param>
        /// <returns></returns>
        [HttpGet("list-scale-status")]
        public async Task<IActionResult> GetScaleStatusAsync(string keyword)
        {
            var dropdownList = await _commonQuery.GetScaleStatus(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Screen

        /// <summary>
        /// Dropdown màn hình
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-screen")]
        public async Task<IActionResult> GetDropdownScreenAsync()
        {
            var dropdownList = await _commonQuery.GetDropdownScreen();
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion
    }
}
