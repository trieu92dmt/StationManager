using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.Rating
{
    public class CreateRatingCommand : IRequest<bool>
    {
        public Guid SenderId { get; set; }
        public Guid CarCompanyId { get; set; }
        public decimal Rate { get; set; }
        public string Content { get; set; }
    }

    public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RateModel> _rateRepo;
        private readonly IRepository<UserModel> _userRepo;

        public CreateRatingCommandHandler(IUnitOfWork unitOfWork, IRepository<RateModel> rateRepo,
                                          IRepository<UserModel> userRepo)
        {
            _unitOfWork = unitOfWork;
            _rateRepo = rateRepo;
            _userRepo = userRepo;
        }

        public async Task<bool> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.FindOneAsync(x => x.AccountId == request.SenderId);

            var newRate = new RateModel
            {
                RateId = Guid.NewGuid(),
                Actived = true,
                CreatedTime = DateTime.Now,
                CarCompanyId = request.CarCompanyId,
                SenderId = user.UserId,
                Rate = request.Rate,
                Content = request.Content,
            };

            _rateRepo.Add(newRate);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
