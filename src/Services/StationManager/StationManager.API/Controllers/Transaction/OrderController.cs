using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.Transaction;

namespace StationManager.API.Controllers.Transaction
{
    [Route("api/v{version:apiVersion}/Transaction/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IMediator mediator, IRepository<CatalogModel> cataRepo, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _cataRepo = cataRepo;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Tạo đơn hàng
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("create-zalopay-order")]
        public async Task<IActionResult> CreateZaloPayOrderAsync([FromBody] CreateOrderCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<object>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Tạo đơn hàng")
            });
        }

        /// <summary>
        /// Lưu giao dịch
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("save-transaction")]
        public async Task<IActionResult> SaveTransaction([FromBody] SaveTransactionCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Tạo giao dịch")
            });
        }
    }
}
