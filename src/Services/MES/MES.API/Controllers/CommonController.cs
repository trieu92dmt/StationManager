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
    }
}
