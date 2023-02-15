using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;

namespace IntegrationNS.Application.Commands.ShippingPoint
{
    public class ShippingPointIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<ShippingPoint> ShippingPoints { get; set; } = new List<ShippingPoint>();
    }

    public class ShippingPoint
    {
        public string ShippingCode { get; set; }
        public string ShippingName { get; set; }
    }

    public class ShippingPointIntegrationCommandHandler : IRequestHandler<ShippingPointIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<ShippingPointModel> _shipRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingPointIntegrationCommandHandler(IRepository<ShippingPointModel> shipRepo, IUnitOfWork unitOfWork)
        {
            _shipRepo = shipRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IntegrationNSResponse> Handle(ShippingPointIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.ShippingPoints.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.ShippingPoints.Count();

            var plants = _shipRepo.GetQuery();

            foreach (var shippingPointIntegration in request.ShippingPoints)
            {
                try
                {
                    //Check tồn tại
                    var shippingPoint = await _shipRepo.FindOneAsync(x => x.ShippingPointCode == shippingPointIntegration.ShippingCode);

                    if (shippingPoint is null)
                    {
                        _shipRepo.Add(new ShippingPointModel
                        {
                            ShippingPointId = Guid.NewGuid(),
                            ShippingPointCode = shippingPointIntegration.ShippingCode,
                            ShippingPointName = shippingPointIntegration.ShippingName,
                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        shippingPoint.ShippingPointName = shippingPointIntegration.ShippingName;
                        //Common
                        shippingPoint.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = shippingPointIntegration.ShippingCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
