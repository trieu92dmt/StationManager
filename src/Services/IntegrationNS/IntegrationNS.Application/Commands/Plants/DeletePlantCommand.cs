using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Plants
{
    public class DeletePlantCommand : IRequest<DeleteNSResponse>
    {
        public List<string> Plants { get; set; } = new List<string>();
    }

    public class DeletePlantCommandHandler : IRequestHandler<DeletePlantCommand, DeleteNSResponse>
    {
        private readonly IRepository<PlantModel> _plantRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePlantCommandHandler(IRepository<PlantModel> plantRep, IUnitOfWork unitOfWork)
        {
            _plantRep = plantRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteNSResponse> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.Plants.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.Plants.Count();

            foreach (var plantDelete in request.Plants)
            {
                try
                {
                    //Xóa Disivision
                    var plant = await _plantRep.FindOneAsync(x => x.PlantCode == plantDelete);
                    if (plant is not null)
                    {
                        _plantRep.Remove(plant);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(plantDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(plantDelete);
                }

            }
            return response;
        }
    }
}
