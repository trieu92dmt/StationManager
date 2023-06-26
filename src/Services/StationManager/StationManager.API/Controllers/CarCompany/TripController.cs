using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.TripManager;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITripQuery _query;

        public TripController(IMediator mediator, ITripQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>
        /// Thêm chuyến
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-trip")]
        public async Task<IActionResult> AddTrip(AddTripCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thêm chuyến")
            });
        }

        /// <summary>
        /// Get danh sách chuyến xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-trip")]
        public async Task<IActionResult> GetListTrip(GetListTripRequest request)
        {
            var response = await _query.GetListTrip(request);

            return Ok(new ApiSuccessResponse<List<TripResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh chuyến xe")
            });
        }

        /// <summary>
        /// Get chi tiết chuyến xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-trip-detail")]
        public async Task<IActionResult> GetTripDetail(Guid tripId)
        {
            var response = await _query.GetTripDetail(tripId);

            return Ok(new ApiSuccessResponse<TripDetailResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết chuyến xe")
            });
        }

        /// <summary>
        /// Get chi tiết ghế ngồi và danh sách vé đã bán
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-trip-data")]
        public async Task<IActionResult> GetTripData(Guid tripId)
        {
            var response = await _query.GetTripData(tripId);

            return Ok(new ApiSuccessResponse<TripDataResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy dữ liệu chuyến xe")
            });
        }

        /// <summary>
        /// Chỉnh sửa chuyến
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-trip")]
        public async Task<IActionResult> UpdateTrip(UpdateTripCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Chỉnh sửa thông tin chuyến")
            });
        }

        /// <summary>
        /// Tìm chuyến
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("search-trip")]
        public async Task<IActionResult> SearchTrip(SearchTripRequest request)
        {
            var response = await _query.SearchTrip(request);

            return Ok(new ApiSuccessResponse<IList<TripSearchResponse>>
            {
                Data = response.Data,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }

    }
}
