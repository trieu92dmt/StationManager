using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.CarManager;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CarManagerController : ControllerBase
    {
        private readonly ICarManagerQuery _query;
        private readonly IMediator _mediator;

        public CarManagerController(ICarManagerQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Get danh sách xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-car")]
        public async Task<IActionResult> GetListCar(GetListCarRequest request)
        {
            var response = await _query.GetListCar(request);

            return Ok(new ApiSuccessResponse<List<CarResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách xe")
            });
        }

        /// <summary>
        /// Thêm loại xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-car-type")]
        public async Task<IActionResult> AddCarType(AddCarTypeCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thêm loại xe")
            });
        }

        /// <summary>
        /// Thêm xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-car")]
        public async Task<IActionResult> AddCar(AddCarCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thêm xe")
            });
        }

        /// <summary>
        /// Cập nhật thông tin xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-car")]
        public async Task<IActionResult> UpdateCar(UpdateCarCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật xe")
            });
        }

        /// <summary>
        /// Thêm xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpDelete("remove-car/{carId}")]
        public async Task<IActionResult> RemoveCar(Guid carId)
        {
            var request = new RemoveCarCommand();
            request.CarId = carId;
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Xóa xe")
            });
        }

        /// <summary>
        /// Get chi tiết xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-car")]
        public async Task<IActionResult> GetDetailCar(Guid carId)
        {
            var response = await _query.GetDetailCar(carId);

            return Ok(new ApiSuccessResponse<DetailCarResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết xe")
            });
        }


        /// <summary>
        /// Get danh sách ghế theo xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-seat-by-car-number")]
        public async Task<IActionResult> GetSeatByCarNumber(string carNumber)
        {
            var response = await _query.GetSeatByCarNumber(carNumber);

            return Ok(new ApiSuccessResponse<SeatDiagramResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách ghế theo xe")
            });
        }

        /// <summary>
        /// Gia hạn nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("package-renew")]
        public async Task<IActionResult> PackageRenew(PackageRenewCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Gia hạn gói nhà xe")
            });
        }
    }
}
