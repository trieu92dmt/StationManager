using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.Permission
{
    public class UpdateAccountCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public bool Active { get; set; }
    }

    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _accRepo;

        public UpdateAccountCommandHandler(IUnitOfWork unitOfWork, IRepository<AccountModel> accRepo)
        {
            _unitOfWork = unitOfWork;
            _accRepo = accRepo;
        }

        public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            account.Actived = request.Active;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
