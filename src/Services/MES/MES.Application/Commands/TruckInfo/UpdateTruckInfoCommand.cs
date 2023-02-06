
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.TruckInfo
{
    public class UpdateTruckInfoCommand : IRequest<bool>
    {
        //Id
        public Guid TruckInfoId { get; set; }
        //Số xe tải
        public string TruckNumber { get; set; }
        //Số cân đâu vào
        public decimal? InputWeight { get; set; }
    }

    //public class UpdateTruckInfoCommandHandler : IRequestHandler<UpdateTruckInfoCommand, bool>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IRepository<TruckInfoModel> _truckInfoRepo;

    //    public UpdateTruckInfoCommandHandler(IUnitOfWork unitOfWork, IRepository<TruckInfoModel> truckInfoRepo)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _truckInfoRepo = truckInfoRepo;
    //    }

    //    //public async Task<bool> Handle(UpdateTruckInfoCommand request, CancellationToken cancellationToken)
    //    //{
    //    //    //Check tồn tại
    //    //    var truckInfo = await _truckInfoRepo.FindOneAsync(x => x.TruckInfoId == request.TruckInfoId);
    //    //    if (truckInfo == null)
    //    //        throw new ISDException(CommonResource.Msg_NotFound, "Thông tin cân xe tải");


    //    //}
    //}
}
