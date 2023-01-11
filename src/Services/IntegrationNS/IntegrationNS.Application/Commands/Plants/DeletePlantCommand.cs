using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Plants
{
    public class DeletePlantCommand : IRequest<bool>
    {
        public string Plant { get; set; }
    }

    public class DeletePlantCommandHandler : IRequestHandler<DeletePlantCommand, bool>
    {
        private readonly IRepository<PlantModel> _plantRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePlantCommandHandler(IRepository<PlantModel> plantRep, IUnitOfWork unitOfWork)
        {
            _plantRep = plantRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
        {
            //Xóa Disivision
            var plant = await _plantRep.FindOneAsync(x => x.PlantCode == request.Plant);
            if (plant is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Plant {request.Plant}");

            _plantRep.Remove(plant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
