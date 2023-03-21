using DTOs.Models;
using Core.Properties;
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
        /// 
        /// 
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

            return Ok(new ApiSuccessResponse<IList<ListNKMHResponse>>
            {
                Data = response.Data,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
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
                return Ok(new ApiSuccessResponse<IList<PuchaseOrderNKMHResponse>>
                {
                    Data = response.Data,
                    RecordsTotal = response.Paging.TotalCount,
                    PagesCount = response.Paging.TotalPages,
                    ResultsCount = response.Paging.PageSize,
                    Message = response.Data.Count() == 1 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
                    IsSuccess = response.Data.Count() == 1 ? false : true
                });
            }
            else
                return Ok(new ApiSuccessResponse<IList<PuchaseOrderNKMHResponse>>
                {
                    Data = response.Data,
                    RecordsTotal = response.Paging.TotalCount,
                    PagesCount = response.Paging.TotalPages,
                    ResultsCount = response.Paging.PageSize,
                    Message = response.Data.Count() == 0 ? "Không có chứng từ SAP!" : string.Format(CommonResource.Msg_Success, "Lấy PO"),
                    IsSuccess = response.Data.Count() == 0 ? false : true
                });
        }

        /// <summary>
        /// Cập nhật nkmh
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/NKMH/update-nkmh
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///         {
        ///           "updateNKMHs": [
        ///             {
        ///               "nkmhId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",         --       nkmhId
        ///               "plant": "string",                                        --       plant
        ///               "purchaseOrderCode": "string",                            --       purchaseOrderCode
        ///               "poItem": "string",                                       --       poItem
        ///               "material": "string",                                     --       material
        ///               "storageLocation": "string",                              --       slocCode
        ///               "batch": "string",                                        --       batch
        ///               "bagQuantity": 0,                                         --       bagQuantity
        ///               "singleWeight": 0,                                        --       singleWeight
        ///               "weightHeadCode": "string",                               --       weightHeadCode
        ///               "weight": 0,                                              --       weight
        ///               "confirmQty": 0,                                          --       confirmQty
        ///               "quantityWithPackaging": 0,                               --       quantityWithPackaging
        ///               "vehicleCode": "string",                                  --       vehicleCode
        ///               "quantityWeight": 0,                                      --       quantityWeight
        ///               "truckQty": 0,                                            --       truckQuantity
        ///               "inputWeight": 0,                                         --       inputWeight
        ///               "outputWeight": 0,                                        --       outputWeight
        ///               "weightVote": "string",                                   --       weightVote
        ///               "documentDate": "2023-02-01T06:34:58.267Z",               --       documentDate
        ///               "startTime": "2023-02-01T06:34:58.267Z",                  --       startTime
        ///               "endTime": "2023-02-01T06:34:58.267Z",                    --       endTime
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",       --       createById
        ///               "createOn": "2023-02-01T06:34:58.267Z",                   --       createOn
        ///               "changeBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",       --       lastEditById
        ///               "description": "string",                                  --       description
        ///               "isDelete": true                                          --       isDelete
        ///             }
        ///           ]
        ///         }
        /// </remarks>
        [HttpPost("update-nkmh")]
        public async Task<IActionResult> UpdateNKMH([FromBody] UpdateNKMHCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response.IsSuccess,
                IsSuccess = response.IsSuccess,
                Message = response.Message
            });
        }

        /// <summary>
        /// Lấy số cân
        /// </summary>
        /// <param name="weightHeadCode"></param>
        /// <returns></returns>
        [HttpGet("get-weight-num")]
        public async Task<IActionResult> GetWeighNum(string weightHeadCode)
        {
            var response = await _query.GetWeighNum(weightHeadCode);

            if (response.isSuccess == false)
                return Ok(new ApiSuccessResponse<GetWeighNumResponse>
                {
                    Data = response,
                    IsSuccess = false,
                    Message = string.Format(CommonResource.Msg_NoData, "cân")
                });

            return Ok(new ApiSuccessResponse<GetWeighNumResponse>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy số cân")
            });
        }

        /// <summary>
        /// Lấy dữ liệu theo po và po item
        /// </summary>
        /// <param name="po"></param>
        /// <param name="poItem"></param>
        /// <returns></returns>
        [HttpGet("get-data-by-po-poitem")]
        public async Task<IActionResult> GetDataByPoAndPoItem(string po, string poItem)
        {
            var response = await _query.GetDataByPoAndPoItem(po, poItem);

            return Ok(new ApiSuccessResponse<GetDataByPoPoItemResponse>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy data")
            });
        }
    }
}
