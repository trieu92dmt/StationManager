using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Divisions
{
    public class DeleteDivisionCommand : IRequest<bool>
    {
        public string Division { get; set; } 
    }

    public class DeleteDistributionChannelCommandHandler : IRequestHandler<DeleteDivisionCommand, bool>
    {
        private readonly IRepository<DivisionModel> _divisionRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDistributionChannelCommandHandler(IRepository<DivisionModel> divisionRep, IUnitOfWork unitOfWork)
        {
            _divisionRep = divisionRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            //Xóa Disivision
            var disivision = await _divisionRep.FindOneAsync(x => x.DivisionCode == request.Division);
            if (disivision is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Disivision {request.Division}");

            _divisionRep.Remove(disivision);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
