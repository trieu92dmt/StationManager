using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Divisions
{
    public class DivisionIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<DivisionIntegration> Divisions { get; set; } = new List<DivisionIntegration>();
    }
    public class DivisionIntegration
    {
        public string Division { get; set; }
        public string DivisionName { get; set; }
        public string SalesOrganization { get; set; }
    }
    public class DivisionIntegrationCommandHandler : IRequestHandler<DivisionIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<DivisionModel> _divisionRep;
        private readonly IUnitOfWork _unitOfWork;

        public DivisionIntegrationCommandHandler(IRepository<DivisionModel> divisionRep, IUnitOfWork unitOfWork)
        {
            _divisionRep = divisionRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(DivisionIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.Divisions.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.Divisions.Count();

            var divisions = _divisionRep.GetQuery();

            foreach (var divisionIntegration in request.Divisions)
            {
                try
                {
                    //Check tồn tại
                    var division = await _divisionRep.FindOneAsync(x => x.DivisionCode == divisionIntegration.Division && x.SaleOrgCode == divisionIntegration.SalesOrganization);

                    if (division is null)
                    {
                        _divisionRep.Add(new DivisionModel
                        {
                            DivisionId = Guid.NewGuid(),
                            DivisionCode = divisionIntegration.Division,
                            DivisionName = divisionIntegration.DivisionName,
                            SaleOrgCode = divisionIntegration.SalesOrganization,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        division.DivisionName = divisionIntegration.DivisionName;   
                        division.SaleOrgCode = divisionIntegration.SalesOrganization;

                        //Common
                        division.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = divisionIntegration.Division,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
