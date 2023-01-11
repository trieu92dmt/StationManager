using ISD.Core.Models;
using ISD.Core.Properties;
using MES.Application.DTOs.Common;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetListMaterial(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownMaterial(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách material") });
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
        public async Task<IActionResult> GetListPO(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownPO(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách po") });
        }
        #endregion

        #region Lấy dropdown đầu cân
        /// <summary>
        /// Lấy dropdown đầu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-weight-head")]
        public async Task<IActionResult> GetListWeightHead(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownWeightHead(keyword);
            return Ok(new ApiSuccessResponse<List<Common2Response>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách đầu cân") });
        }
        #endregion

        #region Lấy dropdown sloc
        /// <summary>
        /// Lấy dropdown sloc
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-dropdown-sloc")]
        public async Task<IActionResult> GetListSloc(string keyword)
        {
            var dropdownList = await _commonQuery.GetDropdownSloc(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList, Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách sloc") });
        }
        #endregion
    }
}
