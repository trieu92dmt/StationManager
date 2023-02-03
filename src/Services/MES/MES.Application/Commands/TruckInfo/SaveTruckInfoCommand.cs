using Azure.Core;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.TruckInfo
{
    public class SaveTruckInfoCommand : IRequest<bool>
    {
        public List<SaveTruckInfo> TruckInfos { get; set; } = new List<SaveTruckInfo>();
    }

    public class SaveTruckInfo
    {
        //Mã plant
        public string PlantCode { get; set; }
        //Số xe tải
        public string TruckNumber { get; set; }
        //Tài xế xe tải
        public string Driver { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
    }

    public class SaveTruckInfoCommandHandler : IRequestHandler<SaveTruckInfoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;

        public SaveTruckInfoCommandHandler(IUnitOfWork unitOfWork, IRepository<TruckInfoModel> truckInfoRepo)
        {
            _unitOfWork = unitOfWork;
            _truckInfoRepo = truckInfoRepo;
        }

        public async Task<bool> Handle(SaveTruckInfoCommand request, CancellationToken cancellationToken)
        {

            var truckInfoCount = await _truckInfoRepo.CountAsync();

            var truckInfos = new List<TruckInfoModel>();

            foreach(var item in request.TruckInfos)
            {
                truckInfos.Add(new TruckInfoModel
                {
                    TruckInfoId = Guid.NewGuid(),
                    TruckInfoCode = $"XT{DateTime.Now.ToString("yyMM")}{1000 + truckInfoCount + 1}",
                    PlantCode = item.PlantCode,
                    TruckNumber = item.TruckNumber,
                    Driver = item.Driver,
                    InputWeight = item.InputWeight
                });
            }

            _truckInfoRepo.AddRange(truckInfos);
            await _unitOfWork.SaveChangesAsync();


            return true;
        }
    }
}
