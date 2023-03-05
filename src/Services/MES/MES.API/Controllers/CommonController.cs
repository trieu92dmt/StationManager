﻿using Core.Models;
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
        /// Lấy dropdown plant
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-plant")]
        public async Task<IActionResult> GetListProduct(string keyword)
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
        /// Lấy dropdown material
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-material")]
        public async Task<IActionResult> GetListMaterial(string keyword, string plant,
                                                         string poFrom, string poTo,
                                                         string odFrom, string odTo,
                                                         string woFrom, string woTo,
                                                         string resFrom, string resTo)
        {
            var dropdownList = await _commonQuery.GetDropdownMaterial(keyword, plant, poFrom, poTo, odFrom, odTo, woFrom, woTo, resFrom, resTo);
            return Ok(new ApiSuccessResponse<List<DropdownMaterialResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách material") });
        }
        #endregion

        #region Lấy dropdown Component
        /// <summary>
        /// Lấy dropdown Component
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy dropdown Item Component
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy Purchasing Org theo Plant Code
        /// </summary>
        /// <param name="plantCode"></param>
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
        /// Lấy dropdown purchasing gr
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy dropdown vendor
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-vendor")]
        public async Task<IActionResult> GetListVendor(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownVendor(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách vendor") });
        }
        #endregion

        #region Lấy dropdown POType
        /// <summary>
        /// Lấy dropdown POType
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-potype")]
        public async Task<IActionResult> GetListPOType(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownPOType(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po type") });
        }
        #endregion

        #region Lấy dropdown PO
        /// <summary>
        /// Lấy dropdown PO
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-po")]
        public async Task<IActionResult> GetListPO(string keyword, string plant, string type, string poType, string vendorFrom, string vendorTo)
        {
            var dropdownList = await _commonQuery.GetDropdownPO(keyword, plant, type, poType, vendorFrom, vendorTo);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po") });
        }
        #endregion

        #region Lấy dropdown PO Item
        /// <summary>
        /// Lấy dropdown PO Item
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy dropdown đầu cân theo plant
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-weight-head-by-plant")]
        public async Task<IActionResult> GetListWeightHeadByPlant(string keyword, string plantCode)
        {
            //Lấy danh sách mã đầu cân đã được chọn

            //Query những thằng chưa được chọn
            var dropdownList = await _commonQuery.GetDropdownWeightHeadByPlant(keyword, plantCode);
            return Ok(new ApiSuccessResponse<List<DropdownWeightHeadResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách đầu cân") });
        }
        //[HttpGet("list-dropdown-weight-head-by-plant")]
        //public async Task<IActionResult> GetListWeightHeadByPlant(string keyword, string plantCode, string type)
        //{
        //    //Lấy danh sách mã đầu cân đã được chọn

        //    //Query những thằng chưa được chọn
        //    var dropdownList = await _commonQuery.GetDropdownWeightHeadByPlant(keyword, plantCode, type);
        //    return Ok(new ApiSuccessResponse<List<DropdownWeightHeadResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách đầu cân") });
        //}
        #endregion

        #region Lấy dropdown sloc
        /// <summary>
        /// Lấy dropdown sloc
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy dropdown create by
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Lấy dropdown SaleOrder (SaleDocumentModel)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-sale-order")]
        public async Task<IActionResult> GetListSaleOrder(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownSaleOrder(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách sale order") });
        }
        #endregion

        #region Lấy dropdown Outbound Delivery
        /// <summary>
        /// Lấy dropdown Outbound Delivery
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-outbound-delivery")]
        public async Task<IActionResult> GetListOutboundDelivery(string type, string plant, string materialFrom, string materialTo, string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownOutboundDelivery(type, plant, materialFrom, materialTo, keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách outbound delivery") });
        }
        #endregion

        #region Lấy dropdown Outbound Delivery Item theo od
        /// <summary>
        /// Lấy dropdown Outbound Delivery Item theo od
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
        /// Lấy dropdown Ship to Party
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-ship-to-party")]
        public async Task<IActionResult> GetListShipToParty(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownShipToParty(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách ship to party") });
        }
        #endregion

        #region Lấy dropdown Số xe tải
        /// <summary>
        /// Lấy dropdown số xe tải
        /// </summary>
        /// <param name="keyword"></param>
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
        /// <param name="keyword"></param>
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
        /// Get order type (Delivery Type)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-order-type")]
        public async Task<IActionResult> GetOrderTypeAsync([Required]string plant, string keyword, string type)
        {
            var dropdownList = await _commonQuery.GetOrderType(plant, keyword, type);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown WorkOrder
        /// <summary>
        /// Get Work Order
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-work-order")]
        public async Task<IActionResult> GetWorkOrderAsync(string plant, string orderType, string material, string keyword)
        {
            var dropdownList = await _commonQuery.GetWorkOrder(plant, orderType, material, keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Reservation
        /// <summary>
        /// Get Reservation
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Get dropdown customer
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-customer")]
        public async Task<IActionResult> GetCustomerAsync(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownCustomer(keyword);
            return Ok(new ApiSuccessResponse<List<Common3Response>> { Data = dropdownList });
        }
        #endregion

        #region Dropdown Scale monitor type
        /// <summary>
        /// Get dropdown Scale monitor type
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Get Reservation item by Reservation
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Get Material Doc
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Get Material Doc Item
        /// </summary>
        /// <param name="keyword"></param>
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
        /// Get dropdown scale status
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-scale-status")]
        public async Task<IActionResult> GetScaleStatusAsync(string keyword)
        {
            var dropdownList = await _commonQuery.GetScaleStatus(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion
    }
}
