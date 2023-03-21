using IntegrationNS.Application.Commands.CustmdSales;
using IntegrationNS.Application.Commands.Customers;
using IntegrationNS.Application.Commands.DistributionChannels;
using IntegrationNS.Application.Commands.Divisions;
using IntegrationNS.Application.Commands.MaterialDocument;
using IntegrationNS.Application.Commands.NKHTs;
using IntegrationNS.Application.Commands.NKMHs;
using IntegrationNS.Application.Commands.NKPPPPs;
using IntegrationNS.Application.Commands.NKTPSXs;
using IntegrationNS.Application.Commands.OrderTypes;
using IntegrationNS.Application.Commands.OutboundDelivery;
using IntegrationNS.Application.Commands.Plants;
using IntegrationNS.Application.Commands.ProductGroups;
using IntegrationNS.Application.Commands.Products;
using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.Commands.PurchasingGroups;
using IntegrationNS.Application.Commands.PurchasingOrganizations;
using IntegrationNS.Application.Commands.Reservation;
using IntegrationNS.Application.Commands.SalesDocument;
using IntegrationNS.Application.Commands.SalesOrgs;
using IntegrationNS.Application.Commands.ShippingPoint;
using IntegrationNS.Application.Commands.StorageLocations;
using IntegrationNS.Application.Commands.Vendors;
using IntegrationNS.Application.Commands.WorkOrder;
using IntegrationNS.Application.Commands.XTHLSXs;
using IntegrationNS.Application.DTOs;
using IntegrationNS.Application.Queries;
using DTOs.Models;
using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IntegrationNS.Application.Commands.NKDCNBs;
using IntegrationNS.Application.DTOs.MES.DTOs;
using IntegrationNS.Application.Commands.XCKs;
using IntegrationNS.Application.DTOs.MES.XCK;
using IntegrationNS.Application.Commands.NCKs;
using IntegrationNS.Application.Commands.NKs;
using IntegrationNS.Application.Commands.XKs;
using IntegrationNS.Application.Commands.XKLXH;
using IntegrationNS.Application.Commands.NHLTs;
using IntegrationNS.Application.Commands.NNVLGCs;
using IntegrationNS.Application.Commands.XNVLGCs;

namespace IntegrationNS.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MasterDataIntegrationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INKMHQuery _nkmhQuery;
        private readonly IOutboundDeliveryQuery _outboundDeliveryQuery;
        private readonly INKTPSXQuery _nKTPSXQuery;
        private readonly INKPPPPQuery _nkppppQuery;
        private readonly IXTHLSXQuery _xthlsxQuery;

        public MasterDataIntegrationController(IMediator mediator, INKMHQuery nkmhQuery, IOutboundDeliveryQuery outboundDeliveryQuery,
                                               INKTPSXQuery nKTPSXQuery, INKPPPPQuery nkppppQuery, IXTHLSXQuery xthlsxQuery)
        {
            _mediator = mediator;
            _nkmhQuery = nkmhQuery;
            _outboundDeliveryQuery = outboundDeliveryQuery;
            _nKTPSXQuery = nKTPSXQuery;
            _nkppppQuery = nkppppQuery;
            _xthlsxQuery = xthlsxQuery;
        }

        #region Tích hợp Order Type

        /// <summary>Tích hợp Order Type</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/OrderType
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///               {
        ///                 "planningPlant": "string",
        ///                 "name": "string",
        ///                 "orderType": "string",
        ///                 "shortText": "string",
        ///                 "category": "string"
        ///               }
        /// OUT PUT
        /// 
        ///               {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Order Type thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("order-type")]
        public async Task<IActionResult> OrderTypeIntegration([FromBody] OrderTypeIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp OrderType") });
        }

        /// <summary>
        /// Xóa Order Type
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-order-type")]
        public async Task<IActionResult> DeleteOrderTypeIntegrationAsync([FromQuery] DeleteOrderTypeCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa OrderType") });
        }
        #endregion

        #region Tích hợp Purchasing Organization

        /// <summary>Tích hợp PurchasingOrganization</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/purchasing-organization
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                     "purchasingOrganization": "string",
        ///                     "purchasingOrganizationDescription": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchasingOrganization thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("purchasing-organization")]
        public async Task<IActionResult> PurchasingOrganizationIntegration([FromBody] PurchasingOrgIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchasingOrganization") });
        }

        /// <summary>
        /// Xóa Purchasing Organization
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-purchasing-organization")]
        public async Task<IActionResult> DeletePurchasingOrganizationIntegrationAsync([FromQuery] DeletePurchasingOrgCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa PurchasingOrganization") });
        }
        #endregion

        #region Tích hợp Purchasing Group

        /// <summary>Tích hợp PurchasingGroup</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/purchasing-group
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                     "purchasingGroup": "string",
        ///                     "purchasingGroupDescription": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchasingGroup thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("purchasing-group")]
        public async Task<IActionResult> PurchasingGroupIntegration([FromBody] PurchasingGroupIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchasingGroup") });
        }

        /// <summary>
        /// Xóa Purchasing Group
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-purchasing-group")]
        public async Task<IActionResult> DeletePurchasingGroupIntegrationAsync([FromQuery] DeletePurchasingGroupCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa PurchasingGroup") });
        }
        #endregion

        #region Tích hợp Vendor

        /// <summary>Tích hợp Vendor</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/vendor
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "vendor": "string",
        ///                  "vendorName": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Vendor thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("vendor")]
        public async Task<IActionResult> VendorIntegrationAsync([FromBody] VendorIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Vendor") });
        }

        /// <summary>
        /// Xóa vendor
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-vendor")]
        public async Task<IActionResult> DeleteVendorIntegrationAsync([FromQuery] DeleteVendorCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xoá Vendor") });
        }
        #endregion

        #region Tích hợp Customer

        /// <summary>Tích hợp Customer</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/customer
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "customer": "string",
        ///                  "name": "string",
        ///                  "country": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Customer thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("customer")]
        public async Task<IActionResult> CustomerIntegration([FromBody] CustomerIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Customer") });
        }

        /// <summary>
        /// Xóa customer
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-customer")]
        public async Task<IActionResult> DeleteCustomerAsync([FromQuery] DeleteCustomerNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Customer") });
        }
        #endregion

        #region Tích hợp DistributionChannel

        /// <summary>Tích hợp DistributionChannel</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/distribution-channel
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "distributionChannel": "string",
        ///                  "name": "string",
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp DistributionChannel thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("distribution-channel")]
        public async Task<IActionResult> DistributionChannelIntegration([FromBody] DistributionChannelIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp DistributionChannel") });
        }

        /// <summary>
        /// Xóa DistributionChannel
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-distribution-channel")]
        public async Task<IActionResult> DeleteDistributionChannelIntegrationAsync([FromQuery] DeleteDistributionChannelCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa DistributionChannel") });
        }
        #endregion

        #region Tích hợp SALES ORGANIZATION

        /// <summary>Tích hợp SALESORGANIZATION</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/sale-Org
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "salesOrganization": "string",
        ///                  "salesOrganizationName": "string",
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp SalesOrganization thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("sale-Org")]
        public async Task<IActionResult> SALESORGANIZATIONIntegration([FromBody] SaleOrgIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp SalesOrganization") });
        }

        /// <summary>
        /// Xóa SalesOrganization
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-sale-Org")]
        public async Task<IActionResult> DeleteSALESORGANIZATIONIntegrationAsync([FromQuery] DeleteSaleOrgCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa SalesOrganization") });
        }
        #endregion

        #region Tích hợp Division

        /// <summary>Tích hợp Division</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/division
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "division": "string",
        ///                  "divisionName": "string".
        ///                  "salesOrganization": "string",
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Division thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("division")]
        public async Task<IActionResult> DivisionIntegration([FromBody] DivisionIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Division") });
        }

        /// <summary>
        /// Xóa division
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-division")]
        public async Task<IActionResult> DeleteDivisionIntegrationAsync([FromQuery] DeleteDivisionCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Division") });
        }
        #endregion

        #region Tích hợp Plant

        /// <summary>Tích hợp Plant</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/plant
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "division": "string",
        ///                  "salesOrganization": "string",
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Plant thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("plant")]
        public async Task<IActionResult> PlantIntegration([FromBody] PlantIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Plant") });
        }

        /// <summary>
        /// Xóa plant
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-plant")]
        public async Task<IActionResult> DeletePlantIntegrationAsync([FromQuery] DeletePlantCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Plant") });
        }
        #endregion

        #region Tích hợp StorageLocation

        /// <summary>Tích hợp StorageLocation</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/storage-location
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "storageLocation": "string",
        ///                  "storageLocationDescription": "string",
        ///                  "plant": "string",
        ///                  "division": "string",
        ///                  "salesOrganization": "string",
        ///                  "distributionChannel": "string",
        ///                  "vendor": "string",
        ///                  "customer": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp StorageLocation thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("storage-location")]
        public async Task<IActionResult> StorageLocationIntegration([FromBody] StorageLocationIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp StorageLocation") });
        }

        /// <summary>
        /// Xóa Storage Location
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-storage-location")]
        public async Task<IActionResult> DeleteStorageLocationIntegrationAsync([FromQuery] DeleteStorageLocationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa StorageLocation") });
        }
        #endregion

        #region Tích hợp Material Group
        /// <summary>Tích hợp Material Group</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/material-group
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                     "materialGroup": "string",
        ///                     "materialGroupName": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp material thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("material-group")]
        public async Task<IActionResult> ProductGroupIntegration([FromBody] ProductGroupIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Material Group") });
        }

        /// <summary>
        /// Xóa Material Group
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-material-group")]
        public async Task<IActionResult> DeleteProductGroupIntegrationAsync([FromQuery] DeleteProductGroupCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Material Group") });
        }
        #endregion

        #region Tích hợp Material
        /// <summary>Tích hợp Material</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/material
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "plant": "string",
        ///                  "plantDescription": "string",
        ///                  "material": "string",
        ///                  "materialDescription": "string",
        ///                  "materialGroup": "string",
        ///                  "materialGroupDesc": "string",
        ///                  "materialType": "string",
        ///                  "materialTypeDescription": "string",
        ///                  "baseUnitofMeasure": "string",
        ///                  "division": "string",
        ///                  "salesOrganization": "string",
        ///                  "distributionChannel": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp material thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("material")]
        public async Task<IActionResult> ProductIntegration([FromBody] ProductIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Material") });
        }

        /// <summary>
        /// Xóa Material
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-material")]
        public async Task<IActionResult> DeleteProductIntegrationAsync([FromQuery] DeleteProductNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Material") });
        }
        #endregion

        #region Tích hợp CUSTMDSALES 
        /// <summary>Tích hợp CUSTMDSALES </summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/custmd-sales 
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "division": "string",
        ///                  "salesOrganization": "string",
        ///                  "distributionChannel": "string",
        ///                  "customerGroup": "string",
        ///                  "salesGroup": "string",
        ///                  "salesOffice": "string",
        ///                  "customerNumber": "string",
        ///                  "customerName": "string",
        ///                  "status": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp CUSTMDSALES thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("custmd-sales")]
        public async Task<IActionResult> CUSTMDSALESIntegration([FromBody] CustmdSaleIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp CUSTMDSALES") });
        }


        /// <summary>
        /// Xóa CUSTMDSALES
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-custmd-sales")]
        public async Task<IActionResult> DeleteCUSTMDSALESIntegrationAsync([FromQuery] DeleteCustmdSaleCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa CUSTMDSALES") });
        }
        #endregion

        #region Tích hợp PurchaseOrder 
        /// <summary>
        /// Tích hợp PurchaseOrder
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/purchase-order
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///             {
        ///               "purchaseOrders": [
        ///                 {
        ///                   "plant": "string",
        ///                   "purchasingOrganization": "string",
        ///                   "purchasingGroup": "string",
        ///                   "vendor": "string",
        ///                   "poType": "string",
        ///                   "purchaseOrder": "string",
        ///                   "documentDate": "2023-01-13T08:21:49.947Z",
        ///                   "releaseIndicator": "string",
        ///                   "purchaseOrderDetails": [
        ///                     {
        ///                       "purchaseOrder": "string",
        ///                       "purchaseOrderItem": "string",
        ///                       "material": "string",
        ///                       "storageLocation": "string",
        ///                       "batch": "string",
        ///                       "vehicleCode": "string",
        ///                       "orderQuantity": 0,
        ///                       "openQuantity": 0,
        ///                       "uoM": "string",
        ///                       "quantityReceived": 0,
        ///                       "deletionInd": "string",
        ///                       "deliver": "string",
        ///                       "vehicleOwner": "string",
        ///                       "transportUnit": "string",
        ///                       "deliveryCompleted": "string",
        ///                       "grossWeight": 0,
        ///                       "netWeight": 0,
        ///                       "weightUnit": "string",
        ///                     }
        ///                   ]
        ///                 }
        ///               ]
        ///             }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchaseOrder thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("purchase-order")]
        public async Task<IActionResult> PurchaseOrderIntegration([FromBody] PurchaseOrderIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchaseOrder") });
        }

        /// <summary>
        /// Xóa PurchaseOrder
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-purchase-order")]
        public async Task<IActionResult> DeletePurchaseOrderIntegrationAsync([FromQuery] DeletePurchaseOrderCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa PurchaseOrder") });
        }
        #endregion

        #region Tích hợp Shipping Point
        /// <summary>Tích hợp Shipping Point</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/shipping-point
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///             {
        ///               "shippingPoints": [
        ///                 {
        ///                   "shippingCode": "string",
        ///                   "shippingName": "string"
        ///                 }
        ///               ]
        ///             }
        ///     OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Division thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("shipping-point")]
        public async Task<IActionResult> ShippingPointIntegration([FromBody] ShippingPointIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Shipping Point") });
        }

        /// <summary>
        /// Xóa Shipping Point
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-shipping-point")]
        public async Task<IActionResult> DeleteShippingPointIntegrationAsync([FromQuery] DeleteShippingPointCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa ShippingPoint") });
        }
        #endregion

        #region Tích hợp WorkOrder
        /// <summary>Tích hợp WorkOrder</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/workorder
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///             {
        ///               "workOrderIntegrations": [
        ///                 {
        ///                   "workOrderCode": "string",
        ///                   "productCode": "string",
        ///                   "orderType": "string",
        ///                   "plant": "string",
        ///                   "storageLocation": "string",
        ///                   "batch": "string",
        ///                   "targetQuantity": 0,
        ///                   "unit": "string",
        ///                   "actualFinishDate": "2023-02-03T09:48:30.184Z",
        ///                   "scheduledFinishDate": "2023-02-03T09:48:30.184Z",
        ///                   "scheduledStartDate": "2023-02-03T09:48:30.184Z",
        ///                   "deliveredQuantity": 0,
        ///                   "salesOrder": "string",
        ///                   "salesOrderItem": "string",
        ///                   "orderCategory": "string",
        ///                   "actualStartDate": "2023-02-03T09:48:30.184Z",
        ///                   "confirmedYieldQuantity": 0,
        ///                   "deletionFlag": "string",
        ///                   "longTextExists": "string",
        ///                   "referenceOrder": "string",
        ///                   "systemStatus": "string",
        ///                   "detallWorkOrderIntegrations": [
        ///                     {
        ///                       "workOrderItem": "string",
        ///                       "productCode": "string",
        ///                       "requirementDate": "2023-02-03T09:48:30.184Z",
        ///                       "requirementQuantiy": 0,
        ///                       "quantityWithdrawn": 0,
        ///                       "baseUnit1": "string",
        ///                       "baseUnit2": "string",
        ///                       "batch": "string",
        ///                       "activity": "string",
        ///                       "reservationItem": "string",
        ///                       "openQuantity": 0,
        ///                       "shortage": 0,
        ///                       "storageLocation": "string",
        ///                       "iconMessType": "string",
        ///                       "systemStatus": "string",
        ///                       "confirmedQty": 0,
        ///                       "quantityFixed1": 0,
        ///                       "purchasingDoc": "string",
        ///                       "purchasingDocItem": "string",
        ///                       "supplier": "string",
        ///                       "movementType": "string",
        ///                       "quantityFixed2": 0,
        ///                       "finalIssue": "string"
        ///                     }
        ///                   ]
        ///                 }
        ///               ]
        ///             }
        ///     OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp WorkOrder thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("workorder")]
        public async Task<IActionResult> WorkorderIntegration([FromBody] WorkOrderIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp WorkOrder") });
        }

        /// <summary>
        /// Xóa WorkOrder
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-workorder")]
        public async Task<IActionResult> DeleteWorkOrderIntegrationAsync([FromQuery] DeleteWorkOrderCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa WorkOrder") });
        }
        #endregion

        #region Tích hợp NKTPSX
        /// <summary>Get data NKTPSX</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nktpsx
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        ///             {
        ///               "plant": "string",
        ///               "orderType": "string",
        ///               "workOrderFrom": "string",
        ///               "workOrderTo": "string",
        ///               "saleOrderFrom": "string",
        ///               "saleOrderTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "scheduledStartFrom": "2023-02-14T06:00:32.314Z",
        ///               "scheduledStartTo": "2023-02-14T06:00:32.315Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-14T06:00:32.315Z",
        ///               "weightDateTo": "2023-02-14T06:00:32.315Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        /// 
        /// 
        ///
        ///
        /// OUT PUT
        /// 
        ///                 {
        ///                   "code": 200,
        ///                   "data": [
        ///                     {
        ///                       "nktpsxId": "03b767c8-3c2a-4af4-86c9-3895854a4c26",
        ///                       "plant": "A200",
        ///                       "workOrder": "001070000020",
        ///                       "material": "000000005310000011",
        ///                       "materialDesc": "",
        ///                       "sloc": "",
        ///                       "batch": null,
        ///                       "bagQuantity": 0,
        ///                       "singleWeight": 0,
        ///                       "weightHeadCode": "NHAP01",
        ///                       "weight": 120.456,
        ///                       "confirmQuantity": 123.456,
        ///                       "quantityWithPackage": 789,
        ///                       "quantityWeight": 0,
        ///                       "totalQuantity": 100000,
        ///                       "deliveryQuantity": 100000,
        ///                       "openQuantity": 0,
        ///                       "unit": "KG",
        ///                       "description": "Ghi chú đến từ Mobile",
        ///                       "image": "",
        ///                       "status": "Chưa tạo giao dịch",
        ///                       "weightVote": "N1000010",
        ///                       "startTime": "2023-01-10T07:00:00",
        ///                       "endTime": "2023-02-14T11:56:00.26",
        ///                       "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///                       "createBy": "admin",
        ///                       "createOn": "2023-02-14T11:56:00.263",
        ///                       "changeById": null,
        ///                       "changeBy": "",
        ///                       "materialDoc": "",
        ///                       "reverseDoc": "",
        ///                       "isDelete": false,
        ///                       "isEdit": false
        ///                     }
        ///                   ],
        ///                   "message": "\"Get data NKTPSX\" thành công.",
        ///                   "isSuccess": true,
        ///                   "resultsCount": null,
        ///                   "recordsTotal": null,
        ///                   "pagesCount": null
        ///                 }
        /// 
        /// </remarks>
        [HttpPost("nktpsx")]
        public async Task<IActionResult> NKTPSXIntegration([FromBody] NKTPSXIntegrationCommand req)
        {
            var response = await _nKTPSXQuery.GetNKTPSX(req);

            return Ok(new ApiSuccessResponse<IList<NKTPSXResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NKTPSX") });
        }
        #endregion

        #region Update phiếu và hủy nhập kho tp sản xuất
        /// <summary>Update, cancel phiếu nhập kho tp sản xuất</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-nktpsx
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nktpsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKTPSX MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nktpsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKTPSX MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nktpsXs": [
        ///                 {
        ///                   "nktpsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NKTPSX MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nktpsXs": [
        ///                 {
        ///                   "nktpsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKTPSX MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nktpsx")]
        public async Task<IActionResult> UpdateOrCancelNKTPSXAsync([FromBody] UpdateAndCancelNKTPSXCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NKTPSX") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKTPSX")
            });
        }
        #endregion

        #region Tích hợp SalesDocument
        /// <summary>Tích hợp SalesDocument</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/workorder
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             {
        ///               "salesDocuments": [
        ///                 {
        ///                   "salesDocumentCode": "string",
        ///                   "customerReference": "string",
        ///                   "customerReferenceHeader": "string",
        ///                   "salesDocumentType": "string",
        ///                   "orderTypeCode": "string",
        ///                   "soldtoPartyCode": "string",
        ///                   "soldToPartyName": "string",
        ///                   "material": "string",
        ///                   "overallStatus": "string",
        ///                   "deliveryStatus": "string",
        ///                   "salesDocumentDetails": [
        ///                     {
        ///                       "salesDocumentItem": "string",
        ///                       "orderQuantity": 0,
        ///                       "salesUnit": "string",
        ///                       "divisionCode": "string",
        ///                       "salesOffice": "string",
        ///                       "salesGroup": "string",
        ///                       "salesOrgCode": "string",
        ///                       "distributionChannelCode": "string",
        ///                       "batch": "string",
        ///                       "confirmedQuantity": 0,
        ///                       "unit": "string",
        ///                       "returns": "string",
        ///                       "shippingPoint": "string",
        ///                       "plant": "string",
        ///                       "overallStatusItem": "string",
        ///                       "deliveryStatusItem": "string",
        ///                       "deliveryDate": "2023-01-13T03:18:15.877Z",
        ///                       "goodsIssueDate": "2023-01-13T03:18:15.877Z"
        ///                     }
        ///                   ]
        ///                 }
        ///               ]
        ///             }
        ///     OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Division thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("sales-document")]
        public async Task<IActionResult> SalesDocumentIntegration([FromBody] SalesDocumentIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Sales Document") });
        }

        /// <summary>
        /// Xóa SalesDocument
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpDelete("delete-sales-document")]
        public async Task<IActionResult> DeleteSalesDocumentIntegrationAsync([FromQuery] DeleteSalesDocumentNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa SalesDocument") });
        }
        #endregion

        #region Tích hợp NKMH MES
        /// <summary>Get data NKMH</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nkmh
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///             {
        ///               "fromTime": "2022-12-01",
        ///               "toTime": "2022-12-15"
        ///             }
        /// OUT PUT
        /// 
        ///                {
        ///                   "code": 200,
        ///                   "data": [
        ///                     {
        ///                       "plant": "A100",                            - Plant
        ///                       "purchaseOrderCode": "4010000204",          - Purchase Order
        ///                       "weightVote": "000001",                     - Số phiếu cân
        ///                       "weightId": "9000122005",                   - ID đợt cân
        ///                       "poType": "",                               - PO Type
        ///                       "purchasingOrg": "A001",                    - Purchasing Organization
        ///                       "purchasingGroup": "101",                   - Purchasing Group
        ///                       "vendorCode": "2100000000",                 - Vendor 
        ///                       "material": null,                           - Material
        ///                       "documentDate": "2022-12-15T00:00:00",      - DocumentDate
        ///                       "poItem": "10",                             - Purchase Order Item
        ///                       "storageLocation": null,                    - Storage Location
        ///                       "batch": null,                              - Batch
        ///                       "vehicleCode": "66C - 3591",                - Số phương tiện
        ///                       "orderQuantity": 0,                         - Order Quantity
        ///                       "openQuantity": 0,                          - Open Quantity
        ///                       "bagQuantity": 0,                           - Số lượng bao
        ///                       "singleWeight": 0,                          - Đơn trọng
        ///                       "weightHeadCode": "DC01",                   - Mã đầu cân
        ///                       "weight": 1000,                             - Trọng lượng cân
        ///                       "confirmQty": 3000,                         - Confirm Qty
        ///                       "quantityWithPackaging": 3005,              - Số lượng kèm bao bì
        ///                       "quantityWeitght": 20,                      - Số lần cân
        ///                       "totalQuantity": 20000,                     - Total Quantity
        ///                       "deliveredQuantity": 0,                     - Delivered Quantity
        ///                       "truckQuantity": 0,                         - Số xe tải
        ///                       "inputWeight": 0,                           - Số cân đầu vào
        ///                       "outputWeight": 0,                          - Số cân đầu ra
        ///                       "description": "Chất lượng A",              - Ghi chú
        ///                       "image": null,                              - Hình ảnh
        ///                       "status": "Chưa tạo giao dịch",             - Trạng thái
        ///                       "startTime": "2022-12-16T08:00:00",         - Thời gian bắt đầu
        ///                       "endTime": "2022-12-16T08:30:00",           - Thời gian kết thúc
        ///                       "createTime": "2022-12-22T10:05:17.03",     - Create On
        ///                       "createBy": long.pt,                        - Create By
        ///                       "lastEditTime": null,                       - Change On
        ///                       "lastEditBy": null                          - Change By
        ///                      }
        ///                     ]
        ///                 }
        /// </remarks>
        [HttpPost("nkmh")]
        public async Task<IActionResult> PurchaseOrderIntegration([FromBody] NKMHIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<NKMHResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NKMH") });
        }
        #endregion

        #region Update phiếu và hủy nhập kho mua hàng
        /// <summary>Update, cancel phiếu nhập kho mua hàng</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-nkmh
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKMH MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKMH MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nkmHs": [
        ///                 {
        ///                   "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NKMH MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nkmHs": [
        ///                 {
        ///                   "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKMH MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nkmh")]
        public async Task<IActionResult> UpdateOrCancelNKMHAsync([FromBody] UpdateAndCancelNKMHCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NKMH") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKMH")
            });
        }
        #endregion

        #region Chi tiết NKMH
        [HttpGet("get-detail-nkmh")]
        public async Task<IActionResult> NKMHDetail(Guid NKMHId)
        {
            var response = await _nkmhQuery.GetNKMHAsync(NKMHId);

            return Ok(new ApiSuccessResponse<NKMHResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get detail NKMH") });
        }
        #endregion

        #region Chi tiết PO
        [HttpGet("get-detail-po")]
        public async Task<IActionResult> PODetail(string poCode)
        {
            var response = await _nkmhQuery.GetPOAsync(poCode);

            return Ok(new ApiSuccessResponse<PuchaseOrderNKMHResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get detail pó") });
        }
        #endregion

        #region Tích hợp OutboundDelivery 
        /// <summary>
        /// Tích hợp OutboundDelivery
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/outbound-delivery
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///             {
        ///               "outboundDeliveries": [
        ///                 {
        ///                   "deliveryCode": "string",                             ---Mã Delivery
        ///                   "shippingPoint": "string",                            ---Shipping Point
        ///                   "saleOrgCode": "string",                              ---Sales Organization
        ///                   "deliveryType": "string",                             ---Delivery Type
        ///                   "deliveryTypeDescription": "string",                  ---Delivery Type Description    
        ///                   "vehicleCode": "string",                              ---Số phương tiện
        ///                   "transportUnit": "string",                            ---Đơn vị vận tải
        ///                   "deliveryDate": "2023-01-30T13:55:05.799Z",           ---Delivery Date
        ///                   "pickingDate": "2023-01-30T13:55:05.799Z",            ---PickingDate
        ///                   "shiptoParty": "string",                              ---Ship-to Party
        ///                   "shiptoPartyName": "string",                          ---Ship-to Party Name
        ///                   "soldtoParty": "string",                              ---Sold-to Party
        ///                   "soldtoPartyName": "string",                          ---Sold-to Party Name
        ///                   "purchasingDocType": "string",                        ---Purchasing Doc. Type
        ///                   "salesDocumentType": "string",                        ---Sales Document Type  
        ///                   "distribChannelCode": "string",                       ---Distrib Channel
        ///                   "divisionCode": "string",                             ---Division
        ///                   "supplierCode": "string",                             ---Supplier
        ///                   "supplierName": "string",                             ---Supplier Name
        ///                   "documentDate": "2023-01-30T13:55:05.799Z",           ---Document Date
        ///                   "actGdsMvmntDate": "2023-01-30T13:55:05.799Z",        ---Act. Gds Mvmnt Date
        ///                   "order": "string",                                    ---Order
        ///                   "receivingPlant": "string",                           ---Receiving Plant (plant)
        ///                   "reference": "string",                                ---Reference
        ///                   "transactionCode": "string",                          ---Transaction Code
        ///                   "storageLocChange": "string",                         ---Storage Loc. Change
        ///                   "overallStatus": "string",                            ---Overall Status
        ///                   "pickConfirmation": "string",                         ---Pick Confirmation
        ///                   "pickingStatus": "string",                            ---Picking Status
        ///                   "overallBlockStatus": "string",                       ---Overall Block Status
        ///                   "overallHeader": "string",                            ---Overall Header
        ///                   "allItems": "string",                                 ---All Items
        ///                   "pickingPutawayHeader": "string",                     ---Picking/Putaway – Header
        ///                   "pickingPtwyAllItems": "string",                      ---Picking/Ptwy – All Items
        ///                   "deliveryHeader": "string",                           ---Delivery – Header
        ///                   "deliveryAllItems": "string",                         ---Delivery – All Items
        ///                   "goodsMvtHeader": "string",                           ---Goods Mvt – Header
        ///                   "goodsMvtAllItems": "string",                         ---Goods Mvt – All Items
        ///                   "goodsMovementSts": "string",                         ---Goods Movement Sts
        ///                   "outboundDeliveryDetails": [
        ///                     {
        ///                       "outboundDeliveryItem": "string",
        ///                       "productCode": "string",
        ///                       "plant": "string",
        ///                       "storageLocation": "string",
        ///                       "batch": "string",
        ///                       "deliveryQuantity": 0,
        ///                       "unit": "string",
        ///                       "pickedQuantityPUoM": 0,
        ///                       "salesUnit": "string",
        ///                       "netWeight": 0,
        ///                       "grossWeight": 0,
        ///                       "weightUnit": "string",
        ///                       "actualDeliveryQty": 0,
        ///                       "itemDescription": "string",
        ///                       "referenceDocument1": "string",
        ///                       "referenceItem": "string",
        ///                       "salesOffice": "string",
        ///                       "salesGroup": "string",
        ///                       "divisionCode": "string",
        ///                       "order": "string",
        ///                       "orderItem": "string",
        ///                       "salesOrder": "string",
        ///                       "salesOrderItem": "string",
        ///                       "referenceDocument2": "string",
        ///                       "referenceDocItem": "string",
        ///                       "goodsMvmntControl": "string",
        ///                       "deliveryCompleted": "string",
        ///                       "originalQuantity": 0,
        ///                       "itemNumberDocument": "string",
        ///                       "overallStatus": "string",
        ///                       "pickingStatus": "string",
        ///                       "item": "string",
        ///                       "pickingPutawayItem": "string",
        ///                       "deliveryItem": "string",
        ///                       "goodsMvtItem": "string",
        ///                       "goodsMovementSts": "string",
        ///                       "distributionChannel": "string"
        ///                     }
        ///                   ]
        ///                 }
        ///               ]
        ///             }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchaseOrder thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("outbound-delivery")]
        public async Task<IActionResult> OutboundDeliveryIntegration([FromBody] OutboundDeliveryIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp OutboundDelivery") });
        }

        /// <summary>
        /// Xóa OutboundDelivery
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-outbound-delivery")]
        public async Task<IActionResult> DeleteOutboundDeliveryIntegrationAsync([FromQuery] DeleteOutboundDeliveryIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa OutboundDelivery") });
        }
        #endregion

        #region Tích hợp NKHT
        /// <summary>Get data NKHT</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nkht
        ///     Params: 
        ///             + version : 1
        ///             {
        ///               "plantCode": "string",
        ///               "salesOrderFrom": "string",
        ///               "salesOrderTo": "string",
        ///               "outboundDeliveryFrom": "string",
        ///               "outboundDeliveryTo": "string",
        ///               "shipToPartyFrom": "string",
        ///               "shipToPartyTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-09T05:41:01.054Z",
        ///               "documentDateTo": "2023-02-09T05:41:01.054Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-09T05:41:01.054Z",
        ///               "weightDateTo": "2023-02-09T05:41:01.054Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///     Body: 
        ///
        ///
        /// OUT PUT
        /// 
        ///     {
        ///       "code": 200,
        ///       "data": [
        ///         {
        ///           "goodsReturnId": "a8506037-18eb-4024-b08e-57709fc3d6e0",
        ///           "plant": "A100",
        ///           "shipToParty": "",
        ///           "shipToPartyName": "",
        ///           "outboundDelivery": "",
        ///           "outboundDeliveryItem": "",
        ///           "material": "000000002200000005",
        ///           "materialDesc": "Gạo xô MN Đài Thơm 5% tấm, C, Đ",
        ///           "sloc": "A123",
        ///           "slocName": "A123 | ÐT.K gạo MN",
        ///           "batch": "",
        ///           "bagQuantity": 0,
        ///           "singleWeight": 0,
        ///           "weightHeadCode": "NHAP01",
        ///           "weight": 0,
        ///           "confirmQty": 0,
        ///           "qtyWithPackage": 0,
        ///           "vehicleCode": "",
        ///           "qtyWeight": 0,
        ///           "totalQty": 0,
        ///           "deliveryQty": 0,
        ///           "openQty": 0,
        ///           "uom": null,
        ///           "description": "",
        ///           "image": "",
        ///           "status": "Chưa tạo giao dịch",
        ///           "weightVote": "N1000002",
        ///           "startTime": "2023-01-10T07:00:00",
        ///           "endTime": "2023-02-09T13:19:20.293",
        ///           "documentDate": null,
        ///           "createById": "00000000-0000-0000-0000-000000000000",
        ///           "createBy": null,
        ///           "createOn": null,
        ///           "changeById": null,
        ///           "changeBy": "",
        ///           "matDoc": null,
        ///           "reverseDoc": null,
        ///           "isDelete": false
        ///         }
        ///       ],
        ///       "message": "\"Get data NKHT\" thành công.",
        ///       "isSuccess": true,
        ///       "resultsCount": null,
        ///       "recordsTotal": null,
        ///       "pagesCount": null
        ///     }
        /// 
        /// </remarks>
        [HttpPost("nkht")]
        public async Task<IActionResult> NKHTIntegration([FromBody] NKHTIntegrationCommand req)
        {
            var response = await _outboundDeliveryQuery.GetGoodsReturn(req);

            return Ok(new ApiSuccessResponse<IList<GoodsReturnResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NKHT") });
        }
        #endregion

        #region Update phiếu và hủy nhập kho hàng trả
        /// <summary>Update, cancel phiếu nhập kho hàng trả</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-nkht
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nkhtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKHT MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nkhtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKHT MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nkhTs": [
        ///                 {
        ///                   "nkhtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NKHT MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nkhTs": [
        ///                 {
        ///                   "nkhtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKHT MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nkht")]
        public async Task<IActionResult> UpdateOrCancelNKHTAsync([FromBody] UpdateAndCancelNKHTCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NKHT") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKHT")
            });
        }
        #endregion

        #region Tích hợp Reservation
        /// <summary>
        /// Tích hợp Reservation
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/reservation
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///         {
        ///           "reservationIntegrations": [
        ///             {
        ///               "reservationCode": "string",
        ///               "requiremenType": "string",
        ///               "status": "string",
        ///               "finalIssue": "string",
        ///               "plant": "string",
        ///               "sloc": "string",
        ///               "receivingPlant": "string",
        ///               "receivingSloc": "string",
        ///               "unloadingPoint": "string",
        ///               "supplier": "string",
        ///               "customer": "string",
        ///               "detailReservationIntegrations": [
        ///                 {
        ///                   "reservationItem": "string",
        ///                   "itemDeleted": "string",
        ///                   "movementAllowed": "string",
        ///                   "missingPart": "string",
        ///                   "material": "string",
        ///                   "batch": "string",
        ///                   "specialStock": "string",
        ///                   "requirementsDate": "2023-02-07T02:05:08.882Z",
        ///                   "requirementQty": 0,
        ///                   "baseUnit": "string",
        ///                   "qtyIsFixed": 0,
        ///                   "qtyWithdrawn": 0,
        ///                   "qtyInUnitOfEntry": 0,
        ///                   "unitOfEntry": "string",
        ///                   "plannedOrder": "string",
        ///                   "purchaseRequisition": "string",
        ///                   "itemOfRequisition": "string",
        ///                   "order": "string",
        ///                   "peggedRequirement": "string",
        ///                   "salesOrder": "string",
        ///                   "salesOrderItem": "string",
        ///                   "movementType": "string",
        ///                   "purchasingDoc": "string",
        ///                   "item": "string",
        ///                   "materialOrigin": "string",
        ///                   "materialGr": "string",
        ///                   "receivingBatch": "string"
        ///                 }
        ///               ]
        ///             }
        ///           ]
        ///         }
        ///
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchaseOrder thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("reservation")]
        public async Task<IActionResult> ReservationIntegration([FromBody] ReservationIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Reservation") });
        }

        /// <summary>
        /// Xóa Reservation
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-reservation")]
        public async Task<IActionResult> DeleteReservationIntegrationAsync([FromQuery] DeleteReservationIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa Reservation") });
        }
        #endregion

        #region Tích hợp Material Doc
        /// <summary>
        /// Tích hợp Material Doc
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/material-document
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///         {
        ///           "materialDocs": [
        ///             {
        ///               "materialDocCode": "string",
        ///               "materialDocItem": "string",
        ///               "identification": "string",
        ///               "parentLineID": "string",
        ///               "hierarchyLevel": "string",
        ///               "originalLineItem": "string",
        ///               "movementType": "string",
        ///               "itemAutoCreated": "string",
        ///               "materialCode": "string",
        ///               "plantCode": "string",
        ///               "storageLocation": "string",
        ///               "batch": "string",
        ///               "stockType": "string",
        ///               "batchRestricted": "string",
        ///               "specialStock": "string",
        ///               "supplier": "string",
        ///               "customer": "string",
        ///               "salesOrder": "string",
        ///               "salesOrderItem": "string",
        ///               "salesOrderSchedule": "string",
        ///               "debitCredit": "string",
        ///               "quantity": 0,
        ///               "baseUOM": "string",
        ///               "qtyUnitOfEntry": 0,
        ///               "unitOfEntry": "string",
        ///               "qtyOPU": 0,
        ///               "opUnit": "string",
        ///               "purchaseOrder": "string",
        ///               "item": "string",
        ///               "referenceDocItem": "string",
        ///               "materialDocYear": "string",
        ///               "materialDoc2": "string",
        ///               "materialDocItem2": "string",
        ///               "deliveryCompleted": "string",
        ///               "text": "string",
        ///               "reservation": "string",
        ///               "itemReservation": "string",
        ///               "finalIssue": "string",
        ///               "receivingMaterial": "string",
        ///               "receivingPlant": "string",
        ///               "receivingSloc": "string",
        ///               "receivingBatch": "string",
        ///               "movementIndicator": "string",
        ///               "receiptIndicator": "string",
        ///               "qtyOrderUnit": 0,
        ///               "orderUnit": "string",
        ///               "supplier2": "string",
        ///               "salesOrder2": "string",
        ///               "salesOrderItem2": "string",
        ///               "postingDate": "2023-02-07T05:03:06.473Z",
        ///               "entryDateTime": "2023-02-07T05:03:06.473Z",
        ///               "reference": "string",
        ///               "transactionCode": "string",
        ///               "delivery": "string",
        ///               "item2": "string",
        ///               "completedIndicator": "string"
        ///             }
        ///           ]
        ///         }
        ///
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp PurchaseOrder thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("material-document")]
        public async Task<IActionResult> MaterialDocIntegration([FromBody] MaterialDocumentIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Material Document") });
        }

        /// <summary>
        /// Xóa material document
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-material-document")]
        public async Task<IActionResult> DeleteMaterialDocIntegrationAsync([FromQuery] DeleteMaterialDocumentIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Xóa material document") });
        }
        #endregion

        #region Tích hợp NKPPPP
        /// <summary>Get data NKPPPP</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nkpppp
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///         {
        ///           "plant": "string",
        ///           "materialFrom": "string",
        ///           "materialTo": "string",
        ///           "component": "string",
        ///           "workorderFrom": "string",
        ///           "workorderTo": "string",
        ///           "salesOrderFrom": "string",
        ///           "salesOrderTo": "string",
        ///           "orderType": "string",
        ///           "scheduledStartFrom": "2023-02-14T15:36:56.394Z",
        ///           "scheduledStartTo": "2023-02-14T15:36:56.394Z",
        ///           "weightHeadCode": "string",
        ///           "weightVotes": [
        ///             "string"
        ///           ],
        ///           "weightDateFrom": "2023-02-14T15:36:56.394Z",
        ///           "weightDateTo": "2023-02-14T15:36:56.394Z",
        ///           "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "status": "string"
        ///         }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "nkppppId": "2e8a253e-fd2d-4205-8d39-73718ccf94e4",
        ///               "plant": "A200",
        ///               "workOrder": "1070000020",
        ///               "material": "5310000011",
        ///               "materialDesc": "Gạo 504 25% tấm đã sortex ĐX22",
        ///               "component": "5510000024",
        ///               "componentDesc": "Cám (KB1) 1",
        ///               "sloc": "",
        ///               "slocName": "",
        ///               "batch": null,
        ///               "bagQuantity": 1,
        ///               "singleWeight": 1,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQuantity": 1,
        ///               "quantityWithPackage": 0,
        ///               "quantityWeight": 0,
        ///               "requirementQty": 540.54,
        ///               "withdrawnQty": 540.54,
        ///               "openQty": 0,
        ///               "unit": "KG",
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "N1000005",
        ///               "startTime": null,
        ///               "endTime": "2023-02-14T17:36:19.973",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-14T17:36:19.973",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "materialDoc": "",
        ///               "reverseDoc": "",
        ///               "scheduleStartTime": null,
        ///               "scheduleFinishTime": null,
        ///               "salesOrder": null,
        ///               "isDelete": false
        ///             }
        ///           ],
        ///           "message": "\"Get data NKPPPP\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nkpppp")]
        public async Task<IActionResult> NKPPPPIntegration([FromBody] NKPPPPIntegrationCommand req)
        {
            var response = await _nkppppQuery.GetNKPPPP(req);

            return Ok(new ApiSuccessResponse<IList<NKPPPPResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NKPPPP") });
        }
        #endregion

        #region Update phiếu và hủy nhập kho phụ phẩm phế phẩm
        /// <summary>Update, cancel phiếu nhập kho phụ phẩm phế phẩm</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-nkpppp
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nkppppId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKPPPP MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nkppppId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKPPPP MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nkpppPs": [
        ///                 {
        ///                   "nkppppId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NKPPPP MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nkpppPs": [
        ///                 {
        ///                   "nkppppId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKPPPP MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nkpppp")]
        public async Task<IActionResult> UpdateOrCancelNKPPPPAsync([FromBody] UpdateAndCancelNKPPPPCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NKPPPP") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKPPPP")
            });
        }
        #endregion

        #region Tích hợp XTHLSX
        /// <summary>Get data XTHLSX</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xthlsx
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///          {
        ///           "plant": "string",
        ///           "materialFrom": "string",
        ///           "materialTo": "string",
        ///           "component": "string",
        ///           "workorderFrom": "string",
        ///           "workorderTo": "string",
        ///           "salesOrderFrom": "string",
        ///           "salesOrderTo": "string",
        ///           "orderType": "string",
        ///           "scheduledStartFrom": "2023-02-14T15:50:07.698Z",
        ///           "scheduledStartTo": "2023-02-14T15:50:07.698Z",
        ///           "weightHeadCode": "string",
        ///           "weightVotes": [
        ///             "string"
        ///           ],
        ///           "weightDateFrom": "2023-02-14T15:50:07.698Z",
        ///           "weightDateTo": "2023-02-14T15:50:07.698Z",
        ///           "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "status": "string"
        ///         }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "xthlsxId": "d39dfc95-4ab4-4aa7-b987-b6ffe6f3a167",
        ///               "plant": "A200",
        ///               "workOrder": "1070000020",
        ///               "material": "5310000011",
        ///               "materialDesc": "Gạo 504 25% tấm đã sortex ĐX22",
        ///               "component": "5210000022",
        ///               "componentDesc": "Gạo lứt 504 25% tấm ĐX22",
        ///               "sloc": "A224",
        ///               "slocName": "A224 | YD.K phụ phẩm MN",
        ///               "batch": null,
        ///               "bagQuantity": 0,
        ///               "singleWeight": 0,
        ///               "weightHeadCode": "NHAP01",
        ///               "weight": 120.456,
        ///               "confirmQuantity": 122.123,
        ///               "quantityWithPackage": 120,
        ///               "quantityWeight": 0,
        ///               "requirementQty": 50000,
        ///               "withdrawnQty": 50000,
        ///               "openQty": 0,
        ///               "unit": "KG",
        ///               "description": "Mobile ghi chú",
        ///               "image": "https://itp-mes.isdcorp.vn/Upload/NKMH/202302/2023-02-07T09-08-452935ed03-21f5-4383-a70d-0851d60b69d1.jpg",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000003",
        ///               "startTime": "2023-01-10T07:00:00",
        ///               "endTime": "2023-02-14T13:53:32.56",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-14T13:53:32.563",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "materialDoc": "",
        ///               "reverseDoc": "",
        ///               "scheduleStartTime": null,
        ///               "scheduleFinishTime": null,
        ///               "salesOrder": null,
        ///               "isDelete": false
        ///             }
        ///           ],
        ///           "message": "\"Get data XTHLSX\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("xthlsx")]
        public async Task<IActionResult> XTHLSXIntegration([FromBody] XTHLSXIntegrationCommand req)
        {
            var response = await _xthlsxQuery.GetXTHLSX(req);

            return Ok(new ApiSuccessResponse<IList<XTHLSXResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data XTHLSX") });
        }
        #endregion

        #region Update phiếu và hủy xuất tiêu hao vào lệnh sản xuất
        /// <summary>Update, cancel phiếu xuất tiêu hao vào lệnh sản xuất</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-xthlsx
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "xthlsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XTHLSX MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "xthlsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XTHLSX MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "xthlsXs": [
        ///                 {
        ///                   "xthlsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID XTHLSX MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "xthlsXs": [
        ///                 {
        ///                   "xthlsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XTHLSX MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-xthlsx")]
        public async Task<IActionResult> UpdateOrCancelXTHLSXAsync([FromBody] UpdateAndCancelXTHLSXCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu XTHlSX") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu XTHLSX")
            });
        }
        #endregion

        #region Tích hợp NKDCNB
        /// <summary>Get data NKDCNB</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xthlsx
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "shippingPoint": "string",
        ///               "purchaseOrderFrom": "string",
        ///               "purchaseOrderTo": "string",
        ///               "outboundDeliveryFrom": "string",
        ///               "outboundDeliveryTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-20T04:20:07.919Z",
        ///               "documentDateTo": "2023-02-20T04:20:07.919Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-20T04:20:07.919Z",
        ///               "weightDateTo": "2023-02-20T04:20:07.919Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "nkdcnbId": "85a6f9ac-77c8-49f6-8998-01356b5cdcaa",
        ///               "plant": "A200",
        ///               "shippingPoint": "A100",
        ///               "outboundDelivery": "7700000161",
        ///               "outboundDeliveryItem": "000010",
        ///               "material": "5400000011",
        ///               "materialDesc": "",
        ///               "sloc": "A221",
        ///               "slocDesc": "YD.K lúa tươi",
        ///               "slocFmt": "A221 | YD.K lúa tươi",
        ///               "batch": "",
        ///               "bagQuantity": 2,
        ///               "singleWeight": 2,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQty": 4,
        ///               "qtyWithPackage": 2,
        ///               "vehicleCode": "cxvcd",
        ///               "qtyWeight": 0,
        ///               "totalQty": 100,
        ///               "deliveryQty": 16,
        ///               "openQty": null,
        ///               "unit": "KG",
        ///               "purchaseOrder": "7700000161",
        ///               "truckNumber": "a262dsf6ds",
        ///               "inputWeight": 20,
        ///               "outputWeight": 3,
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "N1000004",
        ///               "startTime": null,
        ///               "endTime": "2023-02-16T15:22:31.137",
        ///               "documentDate": "2023-02-01T00:00:00",
        ///               "createById": "00000000-0000-0000-0000-000000000000",
        ///               "createBy": null,
        ///               "createOn": "2023-02-16T15:22:31.17",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "matDoc": "",
        ///               "revDoc": "",
        ///               "isDelete": false,
        ///               "isEdit": true
        ///             }
        ///           ],
        ///           "message": "\"Get data NKDCNB\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nkdcnb")]
        public async Task<IActionResult> NKDCNBIntegration([FromBody] SearchNKDCNBCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<SearchNKDCNBResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NKDCNB") });
        }
        #endregion

        #region Update phiếu và hủy nhập kho điều chuyển nội bộ
        /// <summary>Update, cancel phiếu nhập kho điều chuyển nội bộ</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/update-nkdcnb
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nkdcnbId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKDCNB MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nkdcnbId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKDCNB MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nkdcnbId": [
        ///                 {
        ///                   "xthlsxId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NKDCNB MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nkdcnBs": [
        ///                 {
        ///                   "nkdcnbId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKDCNB MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nkdcnb")]
        public async Task<IActionResult> UpdateOrCancelNKDCNBAsync([FromBody] UpdateAndCancelNKDCNBCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NKDCNB") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKDCNB")
            });
        }
        #endregion

        #region Tích hợp XCK
        /// <summary>Get data XCK</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xck
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "slocFrom": "string",
        ///               "slocTo": "string",
        ///               "recevingSlocFrom": "string",
        ///               "recevingSlocTo": "string",
        ///               "reservationFrom": "string",
        ///               "reservationTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-27T04:13:54.366Z",
        ///               "documentDateTo": "2023-02-27T04:13:54.366Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-27T04:13:54.366Z",
        ///               "weightDateTo": "2023-02-27T04:13:54.366Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "xckId": "02e35ef0-f292-48d4-b897-4264060dfb08",
        ///               "plant": "A100",
        ///               "reservation": "1235",
        ///               "reservationItem": "0001",
        ///               "material": "000000002200000003",
        ///               "materialDesc": "Gạo trắng NL Nếp An Giang 10% vụ ĐX19​ C",
        ///               "movementType": "311",
        ///               "sloc": "",
        ///               "slocName": "",
        ///               "receivingSloc": "",
        ///               "receivingSlocName": "",
        ///               "batch": "1000000159",
        ///               "documentDate": "2023-02-03T00:00:00",
        ///               "bagQuantity": 51,
        ///               "singleWeight": 1,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQty": 51,
        ///               "quantityWithPackage": 0,
        ///               "vehicleCode": "",
        ///               "quantityWeight": 0,
        ///               "totalQty": 1001,
        ///               "deliveredQty": 0,
        ///               "openQty": 1001,
        ///               "unit": "KG",
        ///               "truckNumber": "",
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000029",
        ///               "startTime": null,
        ///               "endTime": "2023-02-25T07:56:23.023",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-25T07:56:23.023",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "matDoc": null,
        ///               "revDoc": null,
        ///               "isDelete": false,
        ///               "isEdit": true
        ///             }
        ///           ],
        ///           "message": "\"Get data XCK\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("xck")]
        public async Task<IActionResult> XCKIntegration([FromBody] SearchXCKCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<SearchXCKResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data XCK") });
        }
        #endregion

        #region Update phiếu và hủy xuất chuyển kho
        /// <summary>Update, cancel phiếu xuất chuyển kho</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xck
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "xckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XCK MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "xckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XCK MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "xcKs": [
        ///                 {
        ///                   "xckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID XCK MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "xcKs": [
        ///                 {
        ///                   "xckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XCK MES
        ///                   "batch": "",
        ///                   "materialDocument": "",
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-xck")]
        public async Task<IActionResult> UpdateOrCancelXCKAsync([FromBody] UpdateAndCancelXCKCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu XCK") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu XCK")
            });
        }
        #endregion

        #region Tích hợp NCK
        /// <summary>Get data NCK</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nck
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "slocFrom": "string",
        ///               "slocTo": "string",
        ///               "reservationFrom": "string",
        ///               "reservationTo": "string",
        ///               "materialDocFrom": "string",
        ///               "materialDocTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-22T02:41:50.355Z",
        ///               "documentDateTo": "2023-02-22T02:41:50.355Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-22T02:41:50.355Z",
        ///               "weightDateTo": "2023-02-22T02:41:50.355Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "nckId": "69f0cc4e-21b8-45ff-88ac-74b6d3b0a1b8",
        ///               "plant": "A100",
        ///               "materialDoc": "5000000620",
        ///               "materialDocItem": "0002",
        ///               "reservation": "",
        ///               "material": "3100000013",
        ///               "materialDesc": "Bao nội địa 504, 05kg marka PP",
        ///               "sloc": "A123",
        ///               "slocName": "ÐT.K gạo MN",
        ///               "slocFmt": "A123 | ÐT.K gạo MN",
        ///               "batch": "TAL2212NGX",
        ///               "bagQuantity": 12,
        ///               "singleWeight": 5,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQty": 60,
        ///               "quantityWithPackage": 0,
        ///               "vehicleCode": "XV",
        ///               "quantityWeight": 0,
        ///               "totalQty": 500,
        ///               "deliveredQty": 0,
        ///               "openQty": 500,
        ///               "unit": "KG",
        ///               "documentDate": "2023-02-02T00:00:00",
        ///               "truckInfoId": null,
        ///               "truckNumber": "",
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "description": "",
        ///               "image": "https://itp-mes.isdcorp.vn/Upload/NCK/202302/2023-02-21T14-55-1669f0cc4e-21b8-45ff-88ac-74b6d3b0a1b8.jpg",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000004",
        ///               "startTime": null,
        ///               "endTime": "2023-02-21T14:55:16.843",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-21T14:55:16.843",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "matDoc": null,
        ///               "revDoc": null,
        ///               "isDelete": false,
        ///               "isEdit": false
        ///             }
        ///           ],
        ///           "message": "\"Get data NCK\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nck")]
        public async Task<IActionResult> NCKIntegration([FromBody] NCKIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<NCKResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NCK") });
        }
        #endregion

        #region Update phiếu và hủy nhập chuyển kho
        /// <summary>Update, cancel phiếu nhập chuyển kho</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nck
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NCK MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NCK MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "ncKs": [
        ///                 {
        ///                   "nckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NCK MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "ncKs": [
        ///                 {
        ///                   "nckId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NCK MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nck")]
        public async Task<IActionResult> UpdateOrCancelNCKAsync([FromBody] UpdateAndCancelNCKCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NCK") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NCK")
            });
        }
        #endregion

        #region Tích hợp NK
        /// <summary>Get data NK</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nk
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "slocFrom": "string",
        ///               "slocTo": "string",
        ///               "customerFrom": "string",
        ///               "customerTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-03-03T06:10:17.481Z",
        ///               "weightDateTo": "2023-03-03T06:10:17.481Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "nkid": "1f9f5bcc-f4c4-4986-88f3-7bf437526e38",
        ///               "plant": "A100",
        ///               "material": "2000000003",
        ///               "materialDesc": "Bao xuất khẩu Daily fresh 5kg",
        ///               "customer": "",
        ///               "customerFmt": "",
        ///               "specialStock": "",
        ///               "sloc": "A131",
        ///               "slocFmt": "A131 | ÐT.K vật tư",
        ///               "batch": "",
        ///               "bagQuantity": 12,
        ///               "singleWeight": 2,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQuantity": 24,
        ///               "quantityWithPackage": 0,
        ///               "vehicleCode": null,
        ///               "quantityWeight": 0,
        ///               "unit": "KG",
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "N1000005",
        ///               "startTime": null,
        ///               "endTime": "2023-02-24T10:07:33.273",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-24T10:07:33.273",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "materialDoc": "",
        ///               "reverseDoc": "",
        ///               "truckInfoId": null,
        ///               "truckNumber": null,
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "isDelete": null
        ///             }
        ///           ],
        ///           "message": "\"Get data NK\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nk")]
        public async Task<IActionResult> NKIntegration([FromBody] NKIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<NKResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data NK") });
        }
        #endregion

        #region Update phiếu và hủy nhập khác
        /// <summary>Update, cancel phiếu nhập khác</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nk
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NK MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NK MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nKs": [
        ///                 {
        ///                   "nkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NK MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nKs": [
        ///                 {
        ///                   "nkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NK MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nk")]
        public async Task<IActionResult> UpdateOrCancelNKAsync([FromBody] UpdateAndCancelNKCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NK") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NK")
            });
        }
        #endregion

        #region Tích hợp XK
        /// <summary>Get data XK</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xk
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "reservationFrom": "string",
        ///               "reservationTo": "string",
        ///               "slocFrom": "string",
        ///               "slocTo": "string",
        ///               "customerFrom": "string",
        ///               "customerTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-03-03T03:02:55.710Z",
        ///               "documentDateTo": "2023-03-03T03:02:55.710Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-03-03T03:02:55.710Z",
        ///               "weightDateTo": "2023-03-03T03:02:55.710Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "xkId": "dbcf0a64-4dcd-4bb7-a014-65637698a16d",
        ///               "plant": "A100",
        ///               "reservation": "1235",
        ///               "reservationItem": "0001",
        ///               "material": "2000000003",
        ///               "materialDesc": "Bao xuất khẩu Daily fresh 5kg",
        ///               "movementType": null,
        ///               "sloc": "A124",
        ///               "slocFmt": "A124 | ÐT.K phụ phẩm MN",
        ///               "receivingSloc": "A124",
        ///               "receivingSlocFmt": "A124 | ÐT.K phụ phẩm MN",
        ///               "batch": "",
        ///               "bagQuantity": 3,
        ///               "singleWeight": 2,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "customer": "",
        ///               "customerName": "",
        ///               "specialStock": "",
        ///               "confirmQty": 6,
        ///               "qtyWithPackage": 0,
        ///               "vehicleCode": "",
        ///               "qtyWeight": 0,
        ///               "totalQty": 0,
        ///               "deliveryQty": 0,
        ///               "openQty": 0,
        ///               "unit": "KG",
        ///               "truckInfoId": null,
        ///               "truckNumber": "",
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000006",
        ///               "startTime": null,
        ///               "endTime": "2023-02-18T12:13:22.127",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-18T12:13:22.133",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "matDoc": "",
        ///               "revDoc": "",
        ///               "isDelete": false
        ///             }
        ///           ],
        ///           "message": "\"Get data XK\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("xk")]
        public async Task<IActionResult> XKIntegration([FromBody] XKIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<XKResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data XK") });
        }
        #endregion

        #region Update phiếu và hủy xuất khác
        /// <summary>Update, cancel phiếu xuất khác</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xk
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "xkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XK MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "xkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XK MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "xKs": [
        ///                 {
        ///                   "xkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID XK MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "xKs": [
        ///                 {
        ///                   "xkId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XK MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-xk")]
        public async Task<IActionResult> UpdateOrCancelXKAsync([FromBody] UpdateAndCancelXKCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu XK") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu XK")
            });
        }
        #endregion

        #region Tích hợp XKLXH
        /// <summary>Get data XKLXH</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xklxh
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "deliveryType": "string",
        ///               "purchaseOrderFrom": "string",
        ///               "purchaseOrderTo": "string",
        ///               "salesOrderFrom": "string",
        ///               "salesOrderTo": "string",
        ///               "shipToPartyFrom": "string",
        ///               "shipToPartyTo": "string",
        ///               "outboundDeliveryFrom": "string",
        ///               "outboundDeliveryTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-25T03:11:04.784Z",
        ///               "documentDateTo": "2023-02-25T03:11:04.784Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-25T03:11:04.784Z",
        ///               "weightDateTo": "2023-02-25T03:11:04.784Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "xklxhId": "8b3ac43b-d2d2-446c-80a0-26f72d7aadb0",
        ///               "plant": "A100",
        ///               "shipToPartyName": "",
        ///               "outboundDelivery": "",
        ///               "outboundDeliveryItem": "",
        ///               "material": "2200000005",
        ///               "materialDesc": "Gạo xô MN Đài Thơm 5% tấm, C, Đ",
        ///               "sloc": "",
        ///               "slocName": "",
        ///               "slocFmt": "",
        ///               "batch": "",
        ///               "bagQuantity": 0,
        ///               "singleWeight": 0,
        ///               "weightHeadCode": "NHAP01",
        ///               "weight": 120.456,
        ///               "confirmQty": 120.456,
        ///               "quantityWithPackage": 0,
        ///               "vehicleCode": "",
        ///               "quantityWeight": 3,
        ///               "totalQty": 0,
        ///               "deliveredQty": 0,
        ///               "openQty": 0,
        ///               "unit": "KG",
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000009",
        ///               "startTime": null,
        ///               "endTime": null,
        ///               "documentDate": null,
        ///               "truckInfoId": null,
        ///               "truckNumber": "",
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "goodsWeight": 0,
        ///               "myProperty": 0,
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-24T09:10:55.457",
        ///               "changeById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "changeBy": "admin",
        ///               "matDoc": null,
        ///               "revDoc": null,
        ///               "isDelete": false
        ///             }
        ///           ],
        ///           "message": "\"Get data XKLXH\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("xklxh")]
        public async Task<IActionResult> XKLXHIntegration([FromBody] XKLXHIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<XKLXHResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data XKLXH") });
        }
        #endregion

        #region Update phiếu và hủy xuất kho theo lệnh xuất hàng
        /// <summary>Update, cancel phiếu xuất kho theo lệnh xuất hàng</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xklxh
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "xklxhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XKLXH MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "xklxhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XKLXH MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "xklxHs": [
        ///                 {
        ///                   "xklxhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID XKLXH MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "xklxHs": [
        ///                 {
        ///                   "xklxhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XKLXH MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-xklxh")]
        public async Task<IActionResult> UpdateOrCancelXKLXHAsync([FromBody] UpdateAndCancelXKLXHCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu XKLXH") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu XKLXH")
            });
        }
        #endregion

        #region Tích hợp NHLT
        /// <summary>Get data NHLT</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nhlt
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///             {
        ///               "plant": "string",
        ///               "customerFrom": "string",
        ///               "customerTo": "string",
        ///               "outboundDeliveryFrom": "string",
        ///               "outboundDeliveryTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "documentDateFrom": "2023-02-28T10:31:28.332Z",
        ///               "documentDateTo": "2023-02-28T10:31:28.332Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-28T10:31:28.332Z",
        ///               "weightDateTo": "2023-02-28T10:31:28.332Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///               "status": "string"
        ///             }
        ///
        /// OUTPUT
        /// 
        ///         {   
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "nhltId": "1c7ae705-17e6-4baa-98aa-bbd07cab80f9",
        ///               "plant": "A100",
        ///               "material": "5400000005",
        ///               "materialDesc": "Gạo trắng TP lài thơm 2% tấm",
        ///               "customer": "",
        ///               "customerName": "",
        ///               "sloc": "A123",
        ///               "slocName": "ÐT.K gạo MN",
        ///               "slocFmt": "A123 | ÐT.K gạo MN",
        ///               "batch": "",
        ///               "bagQuantity": 2,
        ///               "singleWeight": 3,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQty": 6,
        ///               "quantityWithPackage": 0,
        ///               "vehicleCode": "",
        ///               "quantityWeight": 0,
        ///               "unit": "",
        ///               "description": "",
        ///               "image": "",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "N1000011",
        ///               "startTime": null,
        ///               "endTime": "2023-02-28T16:35:15.813",
        ///               "createById": "0b581db6-7266-405f-8207-528a53893eec",
        ///               "createBy": "Sysadmin",
        ///               "createOn": "2023-02-28T16:35:15.82",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "matDoc": null,
        ///               "revDoc": null,
        ///               "isDelete": false,
        ///               "isEdit": true,
        ///               "truckInfoId": null,
        ///               "truckNumber": "",
        ///               "inputWeight": 0,
        ///               "outputWeight": 0,
        ///               "outboundDelivery": "6000000103",
        ///               "outboundDeliveryItem": "000010",
        ///               "documentDate": "2023-01-06T00:00:00"
        ///             }
        ///           ],
        ///           "message": "\"nhập hàng loại T\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nhlt")]
        public async Task<IActionResult> NHLTIntegration([FromBody] NHLTIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<NHLTResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data nhập hàng loại T") });
        }
        #endregion

        #region Update phiếu và hủy nhập hàng loại T
        /// <summary>Update, cancel phiếu nhập hàng loại T</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nhlt
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nhltId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NHLT MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nhltId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NHLT MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nhlTs": [
        ///                 {
        ///                   "nhltId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NHLT MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nhlTs": [
        ///                 {
        ///                   "nhltId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NHLT MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nhlt")]
        public async Task<IActionResult> UpdateOrCancelNHLTAsync([FromBody] UpdateAndCancelNHLTCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NHLT") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NHLT")
            });
        }
        #endregion

        #region Tích hợp NNVLGC
        /// <summary>Get data NNVLGC</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nnvlgc
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///
        /// OUTPUT
        /// 
        ///         {   
        ///           "code": 200,
        ///           "data": [
        ///           ],
        ///           "message": "\"nhập NVL GC\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("nnvlgc")]
        public async Task<IActionResult> NNVLGCIntegration([FromBody] NNVLGCIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<NNVLGCResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data nhập NVL GC") });
        }
        #endregion

        #region Update phiếu và hủy nhập NVL GC
        /// <summary>Update, cancel phiếu nhập NVL GC</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/nnvlgc
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "nnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NNVLGC MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "nnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NNVLGC MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "nnvlgCs": [
        ///                 {
        ///                   "nnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID NNVLGC MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "nnvlgCs": [
        ///                 {
        ///                   "nnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NNVLGC MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-nnvlgc")]
        public async Task<IActionResult> UpdateOrCancelNNVLGCAsync([FromBody] UpdateAndCancelNNVLGCCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu NNVLGC") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NNVLGC")
            });
        }
        #endregion

        #region Tích hợp XNVLGC
        /// <summary>Get data XNVLGC</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xnvlgc
        ///     Params: 
        ///             + version : 1
        ///     Body:         
        /// 
        ///         {
        ///           "plant": "string",
        ///           "vendorFrom": "string",
        ///           "vendorTo": "string",
        ///           "poTypeFrom": "string",
        ///           "poTypeTo": "string",
        ///           "purchaseOrderFrom": "string",
        ///           "purchaseOrderTo": "string",
        ///           "materialFrom": "string",
        ///           "materialTo": "string",
        ///           "componentFrom": "string",
        ///           "componentTo": "string",
        ///           "documentDateFrom": "2023-02-28T13:58:29.310Z",
        ///           "documentDateTo": "2023-02-28T13:58:29.310Z",
        ///           "weightHeadCode": "string",
        ///           "weightVotes": [
        ///             "string"
        ///           ],
        ///           "weightDateFrom": "2023-02-28T13:58:29.310Z",
        ///           "weightDateTo": "2023-02-28T13:58:29.310Z",
        ///           "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "status": "string"
        ///         }
        ///
        /// OUTPUT
        /// 
        ///         {   
        ///           "code": 200,
        ///           "data": [
        ///           {
        ///               "xnvlgcId": "35cef998-7263-4163-8afe-497da500c00f",
        ///               "plant": "A100",
        ///               "plantName": "Nhà máy Ðồng Tháp",
        ///               "purchaseOrder": "4030000029",
        ///               "purchaseOrderItem": "00010",
        ///               "material": "5210000007",
        ///               "materialDesc": "Gạo lứt 504 25% tấm ĐX22",
        ///               "component": "2200000003",
        ///               "componentDesc": "Gạo trắng NL Nếp An Giang 10% vụ ĐX19​ C",
        ///               "sloc": "A123",
        ///               "slocName": "ÐT.K gạo MN",
        ///               "slocFmt": "A123 | ÐT.K gạo MN",
        ///               "batch": "TAL2208002",
        ///               "bagQuantity": 12,
        ///               "singleWeight": 68,
        ///               "weightHeadCode": "",
        ///               "weight": 0,
        ///               "confirmQty": 816,
        ///               "quantityWithPackage": 52,
        ///               "vehicleCode": "66X2929",
        ///               "quantityWeight": 0,
        ///               "orderQuantity": 100,
        ///               "orderUnit": "KG",
        ///               "requirementQuantity": 1065,
        ///               "requirementUnit": "KG",
        ///               "vendor": "2100000006",
        ///               "vendorName": "Công ty TNHH Huệ Tâm",
        ///               "truckInfoId": "4e0c42f5-daa7-4168-9a88-a9d6f909e275",
        ///               "truckNumber": "a262dsf6ds",
        ///               "inputWeight": 20,
        ///               "outputWeight": 22,
        ///               "description": "Thêm mới",
        ///               "image": "https://itp-mes.isdcorp.vn/Upload/XNVLGC/202302/2023-02-28T13-28-3135cef998-7263-4163-8afe-497da500c00f.jpg",
        ///               "status": "Chưa tạo giao dịch",
        ///               "weightVote": "X1000002",
        ///               "startTime": null,
        ///               "endTime": "2023-02-28T13:28:32.103",
        ///               "createById": "d3d0cb44-0e76-40d0-8d90-d960dfbdd53a",
        ///               "createBy": "admin",
        ///               "createOn": "2023-02-28T13:28:32.11",
        ///               "changeById": null,
        ///               "changeBy": "",
        ///               "changeOn": null,
        ///               "materialDoc": null,
        ///               "reverseDoc": null,
        ///               "isDelete": false
        ///             }
        ///           ],
        ///           "message": "\"xuất NVL GC\" thành công.",
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        /// 
        /// </remarks>
        [HttpPost("xnvlgc")]
        public async Task<IActionResult> XNVLGCIntegration([FromBody] XNVLGCIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IList<XNVLGCResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data xuất NVL GC") });
        }
        #endregion

        #region Update phiếu và hủy xuất NVL GC
        /// <summary>Update, cancel phiếu xuất NVL GC</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/xnvlgc
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,                                        
        ///               "xnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XNVLGC MES
        ///               "reverseDocument": ""                                    
        ///             }
        ///             -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,                                        
        ///               "xnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XNVLGC MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///             }  
        ///             
        ///             -- Hủy phiếu
        ///             {
        ///               "isCancel": true,
        ///               "xnvlgCs": [
        ///                 {
        ///                   "xnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",     - ID XNVLGC MES
        ///                   "reverseDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///              -- Cập nhật phiếu
        ///             {
        ///               "isCancel": false,
        ///               "xnvlgCs": [
        ///                 {
        ///                   "xnvlgcId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID XNVLGC MES
        ///                   "batch": "",
        ///                   "materialDocument": ""
        ///                 }
        ///               ]
        ///             }
        ///             
        ///     OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": true
        ///             }
        /// </remarks>
        [HttpPut("update-xnvlgc")]
        public async Task<IActionResult> UpdateOrCancelXNVLGCAsync([FromBody] UpdateAndCancelXNVLGCCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = req.IsCancel == true ? string.Format(CommonResource.Msg_Success, "Hủy phiếu XNVLGC") :
                                                                                                       string.Format(CommonResource.Msg_Success, "Cập nhật phiếu XNVLGC")
            });
        }
        #endregion
    }
}
