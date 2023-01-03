using ISD.API.Applications.DTOs.Common;
using ISD.API.Applications.Queries.MasterData;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLG_API.Areas.MasterData
{
    [Route("api/v{version:apiVersion}/MasterData/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CommonController : ControllerBaseAPI
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
    }
}
