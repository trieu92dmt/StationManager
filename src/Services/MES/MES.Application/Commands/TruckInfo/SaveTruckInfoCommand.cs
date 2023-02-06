using Azure.Core;
using ISD.Core.Extensions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
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
            //Lấy ra chuỗi năm tháng
            var str = DateTime.Now.ToString("yyMM");

            var truckInfoCount = await _truckInfoRepo.GetQuery(x => x.TruckInfoCode.Substring(2,4) == str).CountAsync();

            var truckInfos = new List<TruckInfoModel>();

            int index = 1;

            foreach(var item in request.TruckInfos)
            {
                truckInfos.Add(new TruckInfoModel
                {
                    TruckInfoId = Guid.NewGuid(),
                    TruckInfoCode = $"XT{DateTime.Now.ToString("yyMM")}{1000 + truckInfoCount + index}",
                    PlantCode = item.PlantCode,
                    TruckNumber = item.TruckNumber,
                    Driver = item.Driver,
                    InputWeight = item.InputWeight,
                    Actived = true,
                    CreateTime = DateTime.Now,
                    CreateBy = string.IsNullOrEmpty(TokenExtensions.GetUserId()) ? null : Guid.Parse(TokenExtensions.GetUserId())
                });

                index++;
            }

            _truckInfoRepo.AddRange(truckInfos);
            await _unitOfWork.SaveChangesAsync();


            return true;
        }
    }
}
