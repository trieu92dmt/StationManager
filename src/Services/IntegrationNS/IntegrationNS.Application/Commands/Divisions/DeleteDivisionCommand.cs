using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Divisions
{
    public class DeleteDivisionCommand : IRequest<bool>
    {
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
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
            var disivision = await _divisionRep.FindOneAsync(x => x.DivisionCode == request.Division && x.SaleOrgCode == request.SalesOrganization);
            if (disivision is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Disivision {request.Division}");

            _divisionRep.Remove(disivision);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
