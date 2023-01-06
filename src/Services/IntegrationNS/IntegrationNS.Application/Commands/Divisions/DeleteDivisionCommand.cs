using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Divisions
{
    public class DeleteDivisionCommand : IRequest<DeleteNSResponse>
    {
        public List<string> Divisions { get; set; } = new List<string>();
    }

    public class DeleteDistributionChannelCommandHandler : IRequestHandler<DeleteDivisionCommand, DeleteNSResponse>
    {
        private readonly IRepository<DivisionModel> _divisionRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDistributionChannelCommandHandler(IRepository<DivisionModel> divisionRep, IUnitOfWork unitOfWork)
        {
            _divisionRep = divisionRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.Divisions.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.Divisions.Count();

            foreach (var divisionDelete in request.Divisions)
            {
                try
                {
                    //Xóa Disivision
                    var disivision = await _divisionRep.FindOneAsync(x => x.DivisionCode == divisionDelete);
                    if (disivision is not null)
                    {
                        _divisionRep.Remove(disivision);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(divisionDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(divisionDelete);
                }

            }
            return response;
        }
    }
}
