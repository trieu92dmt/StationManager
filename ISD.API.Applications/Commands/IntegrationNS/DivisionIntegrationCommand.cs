using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class DivisionIntegrationCommand : IRequest<bool>
    {
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
    }
    public class DivisionIntegrationCommandHandler : IRequestHandler<DivisionIntegrationCommand, bool>
    {
        private readonly IGeneRepo<DivisionModel> _divisionRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public DivisionIntegrationCommandHandler(IGeneRepo<DivisionModel> divisionRep, IISDUnitOfWork unitOfWork)
        {
            _divisionRep = divisionRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DivisionIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var division = await _divisionRep.FindOneAsync(x => x.DivisionCode == request.Division);

            if (division is null)
            {
                _divisionRep.Add(new DivisionModel
                {
                    DivisionId = Guid.NewGuid(),
                    DivisionCode = request.Division,
                    SaleOrgCode = request.SalesOrganization,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                division.SaleOrgCode = request.SalesOrganization;

                //Common
                division.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
