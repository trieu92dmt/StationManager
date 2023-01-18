using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using MES.Application.Commands.MES;
using MES.Application.DTOs.MES;
using MES.Application.DTOs.MES.NKMH;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKMHController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INKMHQuery _query;

        public NKMHController(IMediator mediator, INKMHQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        #region Save nhập kho mua hàng
        /// <summary>
        /// Save nhập kho mua hàng
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nkmh")]
        public async Task<IActionResult> SaveNKMHAsync([FromBody] SaveNKMHCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Lưu NKMH") });
        }
        #endregion

        /// <summary>GET nhập kho mua hàng</summary>
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
        ///             {
        ///              "plant": "string",
        ///              "purchasingOrgFrom": 0,
        ///              "purchasingOrgTo": 0,
        ///              "purchasingGroupFrom": 0,
        ///              "purchasingGroupTo": 0,
        ///              "vendorFrom": 0,
        ///              "vendorTo": 0,
        ///              "poType": "string",
        ///              "purchaseOrderFrom": 0,
        ///              "purchaseOrderTo": 0,
        ///              "materialFrom": 0,
        ///              "materialTo": 0,
        ///              "documentDateFrom": "2022-12-30T03:27:58.735Z",
        ///              "documentDateTo": "2022-12-30T03:27:58.735Z"
        ///            }
        ///             
        /// OUT PUT
        ///             {
        ///               "code": 200,
        ///               "data": {
        ///                 "puchaseOrderNKMHs": [
        ///                   {
        ///                     "poDetailId": null,                     - PO Id
        ///                     "plant": null,                          - Plant
        ///                     "purchaseOrderCode": null,              - Purchase Order	
        ///                     "poItem": null,                         - Purchase Order Item	
        ///                     "vendorCode": null,                     - Vendor Code	
        ///                     "vendorName": null,                     - Vendor Name.	
        ///                     "material": "0",                        - Material
        ///                     "materialName": null,                   - Material Desc.	
        ///                     "batch": null,                          - Batch
        ///                     "vehicleCode": null,                    - Số phương tiện	
        ///                     "openQuantity": null,                   - Open Quantity	
        ///                     "unit": null                            - UoM
        ///                     "storageLocation": null                 - Stor. Loc	
        ///                   }
        ///                 ],
        ///                 "listNKMHs": [
        ///                   {
        ///                     "nkmhId": "00000000-0000-0000-0000-000000000000",          - ID NKMH
        ///                     "plant": null,                                             - Plant
        ///                     "purchaseOrderCode": null,                                 - Purchase Order
        ///                     "weightVote": null,                                        - Số phiếu cân	
        ///                     "weightId": null,                                          -
        ///                     "poType": null,                                            -
        ///                     "purchasingOrg": null,                                     -
        ///                     "purchasingGroup": null,                                   -
        ///                     "vendorCode": null,                                        - Vendor Code	
        ///                     "vendorName": null,                                        - Vendor Name	
        ///                     "material": null,                                          - Material
        ///                     "materialName": null,                                      - Material Desc.	
        ///                     "documentDate": "2022-12-28T14:04:41.207",                 -
        ///                     "poItem": null,                                            - Purchase Order Item	
        ///                     "storageLocation": null,                                   -
        ///                     "vehicleCode": null,                                       - Số phương tiện	
        ///                     "orderQuantity": null,                                     -
        ///                     "openQuantity": null,                                      - Open Quantity	
        ///                     "unit": null,                                              - UoM
        ///                     "bagQuantity": null,                                       - SL bao	
        ///                     "singleWeight": null,                                      - Đơn trọng	
        ///                     "weightHeadCode": null,                                    - Đầu cân	
        ///                     "weight": null,                                            - Trọng lượng cân	
        ///                     "confirmQty": null,                                        - Confirm Qty	
        ///                     "quantityWithPackaging": null,                             - SL kèm bao bì	
        ///                     "quantityWeitght": null,                                   - Số lần cân	
        ///                     "totalQuantity": null,                                     - Total Quantity	
        ///                     "deliveredQuantity": null,                                 - Delivered Quantity	
        ///                     "truckQuantity": null,                                     - Số xe tải	
        ///                     "inputWeight": null,                                       - Số cân đầu vào	
        ///                     "outputWeight": null,                                      - Số cân đầu ra	
        ///                     "description": null,                                       - Ghi chú	
        ///                     "image": null,                                             - Hình ảnh	
        ///                     "status": null,                                            - Status
        ///                     "startTime": null,                                         - Thời gian bắt đầu	
        ///                     "endTime": null,                                           - Thời gian kết thúc	
        ///                     "createTime": "2022-12-28T14:04:41.207",                   - Create On	
        ///                     "createBy": null,                                          - Create By	
        ///                     "lastEditBy": null,                                        - Change By	
        ///                     "materialDocument": null,                                  - Material Doc.	
        ///                     "reverseDocument": null,                                   - Reverse Doc.	
        ///                     "batch": null                                              - Batch
        ///                   }
        ///                 ]
        ///               },
        ///               "dataChart": null,
        ///               "message": null,
        ///               "isSuccess": true,
        ///               "resultsCount": null,
        ///               "recordsTotal": null,
        ///               "pagesCount": null
        ///             }
        /// </remarks>
        [HttpPost("get-nkmh")]
        public async Task<IActionResult> GetNKMHAsync([FromBody] GetNKMHCommand command)
        {
            var response = await _query.GetNKMHAsync(command);

            return Ok(new ApiSuccessResponse<List<ListNKMHResponse>>
            {
                Data = response
            });
        }


        /// <summary>
        /// Get PO
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-po")]
        public async Task<IActionResult> GetPOAsync([FromBody] GetNKMHCommand command)
        {
            var response = await _query.GetPOAsync(command);

            if (command.MaterialFrom != null)
            {
                return Ok(new ApiSuccessResponse<List<PuchaseOrderNKMHResponse>>
                {
                    Data = response,
                    Message = response.Count() == 1 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
                    IsSuccess = response.Count() == 1 ? false : true
                });
            }
            else
                return Ok(new ApiSuccessResponse<List<PuchaseOrderNKMHResponse>>
                {
                    Data = response,
                    Message = response.Count() == 0 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
                    IsSuccess = response.Count() == 0 ? false : true
                });
        }

        /// <summary>
        /// Chỉnh sửa NKMH
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[HttpPost("get-po")]
        //public async Task<IActionResult> GetPOAsync([FromBody] GetNKMHCommand command)
        //{
        //    var response = await _query.GetPOAsync(command);

        //    if (command.MaterialFrom != null)
        //    {
        //        return Ok(new ApiSuccessResponse<List<PuchaseOrderNKMHResponse>>
        //        {
        //            Data = response,
        //            Message = response.Count() == 1 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
        //            IsSuccess = response.Count() == 1 ? false : true
        //        });
        //    }
        //    else
        //        return Ok(new ApiSuccessResponse<List<PuchaseOrderNKMHResponse>>
        //        {
        //            Data = response,
        //            Message = response.Count() == 0 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
        //            IsSuccess = response.Count() == 0 ? false : true
        //        });
        //}

        /// <summary>
        /// Lấy số cân
        /// </summary>
        /// <param name="weightHeadCode"></param>
        /// <returns></returns>
        [HttpGet("get-weight-num")]
        public async Task<IActionResult> GetWeighNum(string weightHeadCode)
        {
            var response = await _query.GetWeighNum(weightHeadCode);

            return Ok(new ApiSuccessResponse<GetWeighNumResponse>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy số cân")
            });
        }
    }
}
