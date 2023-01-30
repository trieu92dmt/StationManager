using IntegrationNS.Application.Commands.CustmdSales;
using IntegrationNS.Application.Commands.Customers;
using IntegrationNS.Application.Commands.DistributionChannels;
using IntegrationNS.Application.Commands.Divisions;
using IntegrationNS.Application.Commands.NKMHs;
using IntegrationNS.Application.Commands.OrderTypes;
using IntegrationNS.Application.Commands.OutboundDelivery;
using IntegrationNS.Application.Commands.Plants;
using IntegrationNS.Application.Commands.ProductGroups;
using IntegrationNS.Application.Commands.Products;
using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.Commands.PurchasingGroups;
using IntegrationNS.Application.Commands.PurchasingOrganizations;
using IntegrationNS.Application.Commands.SalesDocument;
using IntegrationNS.Application.Commands.SalesOrgs;
using IntegrationNS.Application.Commands.ShippingPoint;
using IntegrationNS.Application.Commands.StorageLocations;
using IntegrationNS.Application.Commands.Vendors;
using IntegrationNS.Application.Commands.WorkOrder;
using IntegrationNS.Application.DTOs;
using IntegrationNS.Application.Queries;
using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationNS.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MasterDataIntegrationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INKMHQuery _nkmhQuery;

        public MasterDataIntegrationController(IMediator mediator, INKMHQuery nkmhQuery)
        {
            _mediator = mediator;
            _nkmhQuery = nkmhQuery;
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
        ///                   "actualFinishDate": "2023-01-13T02:16:17.868Z",
        ///                   "scheduledFinishDate": "2023-01-13T02:16:17.868Z",
        ///                   "scheduledStartDate": "2023-01-13T02:16:17.868Z",
        ///                   "deliveredQuantity": 0,
        ///                   "salesOrder": "string",
        ///                   "salesOrderItem": "string",
        ///                   "orderCategory": "string",
        ///                   "actualStartDate": "2023-01-13T02:16:17.868Z",
        ///                   "startDate": "2023-01-13T02:16:17.868Z",
        ///                   "confirmedYieldQuantity": 0,
        ///                   "deletionFlag": "string",
        ///                   "longTextExists": "string",
        ///                   "referenceOrder": "string",
        ///                   "systemStatus": "string"
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
        ///                   "deliveryCode": "string",
        ///                   "shippingPoint": "string",
        ///                   "saleOrgCode": "string",
        ///                   "deliveryType": "string",
        ///                   "deliveryTypeDescription": "string",
        ///                   "vehicleCode": "string",
        ///                   "deliveryDate": "2023-01-30T13:55:05.799Z",
        ///                   "pickingDate": "2023-01-30T13:55:05.799Z",
        ///                   "shiptoParty": "string",
        ///                   "shiptoPartyName": "string",
        ///                   "soldtoParty": "string",
        ///                   "soldtoPartyName": "string",
        ///                   "purchasingDocType": "string",
        ///                   "salesDocumentType": "string",
        ///                   "distribChannelCode": "string",
        ///                   "divisionCode": "string",
        ///                   "supplierCode": "string",
        ///                   "supplierName": "string",
        ///                   "documentDate": "2023-01-30T13:55:05.799Z",
        ///                   "actGdsMvmntDate": "2023-01-30T13:55:05.799Z",
        ///                   "order": "string",
        ///                   "receivingPlant": "string",
        ///                   "reference": "string",
        ///                   "transactionCode": "string",
        ///                   "storageLocChange": "string",
        ///                   "overallStatus": "string",
        ///                   "pickConfirmation": "string",
        ///                   "pickingStatus": "string",
        ///                   "overallBlockStatus": "string",
        ///                   "overallHeader": "string",
        ///                   "allItems": "string",
        ///                   "pickingPutawayHeader": "string",
        ///                   "pickingPtwyAllItems": "string",
        ///                   "deliveryHeader": "string",
        ///                   "deliveryAllItems": "string",
        ///                   "goodsMvtHeader": "string",
        ///                   "goodsMvtAllItems": "string",
        ///                   "goodsMovementSts": "string",
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
        ///                       "originalQuantity": "string",
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
    }
}
