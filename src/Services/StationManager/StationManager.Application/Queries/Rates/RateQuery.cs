using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Shared.Models;

namespace StationManager.Application.Queries.Rates
{
    public interface IRateQuery
    {
        Task<ApiResponse> CheckRatePermission(Guid accountId, Guid companyId);
    }

    public class RateQuery : IRateQuery
    {
        private readonly IRepository<TicketModel> _ticketRepo;

        public RateQuery(IRepository<TicketModel> ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }

        public async Task<ApiResponse> CheckRatePermission(Guid accountId, Guid companyId)
        {
            var check = await _ticketRepo.FindOneAsync(x => x.CarCompanyId == companyId && x.UserId ==  accountId);
            if (check == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Chỉ người dùng đã trải nghiệm dịch vụ từ nhà xe mới có thể thực hiện chức năng đánh giá"
                };
            }

            return new ApiResponse
            {
                IsSuccess = true,
            };
        }
    }
}
