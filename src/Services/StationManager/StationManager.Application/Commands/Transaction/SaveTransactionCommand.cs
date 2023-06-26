using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.Transaction
{
    public class SaveTransactionCommand : IRequest<bool>
    {
        public string TransactionType { get; set; }
        public decimal Price { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SaveTransactionCommandHandler : IRequestHandler<SaveTransactionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TransactionModel> _transRepo;

        public SaveTransactionCommandHandler(IUnitOfWork unitOfWork, IRepository<TransactionModel> transRepo)
        {
            _unitOfWork = unitOfWork;
            _transRepo = transRepo;
        }

        public async Task<bool> Handle(SaveTransactionCommand request, CancellationToken cancellationToken)
        {
            var newTransaction = new TransactionModel
            {
                TransactionId = Guid.NewGuid(),
                TransactionType = request.TransactionType,
                Price = request.Price,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            _transRepo.Add(newTransaction);

            await _unitOfWork.SaveChangesAsync();

            return true;

        }
    }
}
