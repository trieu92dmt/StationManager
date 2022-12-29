using ISD.API.Applications.Commands.IntegrationNS;
using ISD.API.Applications.DTOs.IntegrationNS;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.EntityModels.Models;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITP_MES_API.Areas.IntegrationNS
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MasterDataIntegrationController : ControllerBaseAPI
    {
        private readonly IMediator _mediator;

        public MasterDataIntegrationController(IMediator mediator)
        {
            _mediator = mediator;
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
        ///                 "shortText": "string"
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp OrderType") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchasingOrganization") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchasingGroup") });
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
        ///                  "vendorName": "string",
        ///                  "country": "string"
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
        public async Task<IActionResult> VendorIntegration([FromBody] VendorIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Vendor") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Customer") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp DistributionChannel") });
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
        ///                     "message": "Tích hợp DistributionChannel thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("sale-Org")]
        public async Task<IActionResult> SALESORGANIZATIONIntegration([FromBody] SaleOrgIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp SalesOrrgania") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Division") });
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
        public async Task<IActionResult> PlantIntegration([FromBody] DivisionIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Plant") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp StorageLocation") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Material Group") });
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Material") });
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
        ///                  "saleOrgCode": "string",
        ///                  "salesOffice": "string"
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

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp CUSTMDSALES") });
        }
        #endregion

        #region Tích hợp PurchaseOrder 
        /// <summary>
        /// Tích hợp PurchaseOrder
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("purchase-order")]
        public async Task<IActionResult> PurchaseOrderIntegration([FromBody] PurchaseOrderIntegrationNSCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp PurchaseOrder") });
        }
        #endregion

        #region Tích hợp NKMH
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
        /// <summary>Get data NKMH</summary>
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
        ///
        ///             {
        ///               "isCancel": true,                                        - isCancel = true: Hủy phiếu, isCancel = false: Sửa phiếu
        ///               "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",        - ID NKMH MES
        ///               "batch": "string",
        ///               "materialDocument": "string",
        ///               "reverseDocument": ""                                    
        ///             }
        /// OUT PUT
        /// 
        ///                {
        ///                   "code": 200,
        ///                   "data": true
        ///                 }
        /// </remarks>
        [HttpPut("update-nkmh")]
        public async Task<IActionResult> UpdateOrCancelNKMHAsync([FromBody] UpdateAndCancelNKMHCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Cập nhật phiếu NKMH") });
        }
        #endregion
    }
}
