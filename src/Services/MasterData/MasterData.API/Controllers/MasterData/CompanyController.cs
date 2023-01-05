using ISD.Core.Models;
using MasterData.Application.DTOs;
using MasterData.Applications.Commands.Company;
using MasterData.Applications.Queries.MasterData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CompanyController : ControllerBase
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
