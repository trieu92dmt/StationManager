using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace MES.Application.Commands.TruckInfo
{
    public class UpdateTruckInfoCommand : IRequest<bool>
    {
        //Id
        public string TruckInfoId { get; set; }
        //Tài xế xe tải
        public string Driver { get; set; }
        //Số cân đâu vào
        public decimal? InputWeight { get; set; }
    }

    public class UpdateTruckInfoCommandHandler : IRequestHandler<UpdateTruckInfoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;

        public UpdateTruckInfoCommandHandler(IUnitOfWork unitOfWork, IRepository<TruckInfoModel> truckInfoRepo)
        {
            _unitOfWork = unitOfWork;
            _truckInfoRepo = truckInfoRepo;
        }

        public async Task<bool> Handle(UpdateTruckInfoCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var truckInfo = await _truckInfoRepo.FindOneAsync(x => x.TruckInfoCode == request.TruckInfoId);
            if (truckInfo == null)
                throw new ISDException(CommonResource.Msg_NotFound, "Thông tin cân xe tải");

            //Mapping
            //Tài xế
            truckInfo.Driver = request.Driver;
            //Số cân đầu vào
            truckInfo.InputWeight = request.InputWeight;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
