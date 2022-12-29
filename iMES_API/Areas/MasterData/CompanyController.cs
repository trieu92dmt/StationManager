using ISD.API.Applications.Commands.Company;
using ISD.API.Applications.DTOs.Company;
using ISD.API.Applications.Queries.MasterData;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLG_API.Areas.MasterData
{
    [Route("api/v{version:apiVersion}/MasterData/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CompanyController : ControllerBaseAPI
    {
        private readonly IMediator _mediator;
        private readonly ICompanyQuery _companyQuery;

        public CompanyController(IMediator mediator, ICompanyQuery companyQuery)
        {
            _mediator = mediator;
            _companyQuery = companyQuery;
        }

        #region Search công ty
        /// <summary>
        /// Tìm kiếm công ty
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("search-company")]
        public async Task<IActionResult> SearchCompany(CompanySearchCommand req)
        {
            var response = await _companyQuery.SearchCompany(req);

            return Ok(new ApiSuccessResponse<IList<CompanySearchResponse>> 
            { 
                Data = response.Data, 
                IsSuccess = true,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }
        #endregion
    }
}
