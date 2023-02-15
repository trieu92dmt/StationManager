﻿using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;

namespace IntegrationNS.Application.Commands.StorageLocations
{
    public class StorageLocationIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<StorageLocationIntegration> StorageLocations { get; set; } = new List<StorageLocationIntegration>();
    }

    public class StorageLocationIntegration
    {
        public string StorageLocation { get; set; }
        public string StorageLocationDescription { get; set; }
        public string Plant { get; set; }
    }
    public class StorageLocationIntegrationCommandHandler : IRequestHandler<StorageLocationIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<StorageLocationModel> _storageLocationRep;
        private readonly IUnitOfWork _unitOfWork;

        public StorageLocationIntegrationCommandHandler(IRepository<StorageLocationModel> storageLocationRep, IUnitOfWork unitOfWork)
        {
            _storageLocationRep = storageLocationRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(StorageLocationIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.StorageLocations.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.StorageLocations.Count();

            foreach (var storageLocationIntegration in request.StorageLocations)
            {
                try
                {
                    //Check tồn tại
                    var storageLocation = await _storageLocationRep.FindOneAsync(x => x.StorageLocationCode == storageLocationIntegration.StorageLocation);

                    if (storageLocation is null)
                    {
                        _storageLocationRep.Add(new StorageLocationModel
                        {
                            StorageLocationId = Guid.NewGuid(),
                            StorageLocationCode = storageLocationIntegration.StorageLocation,
                            StorageLocationName = storageLocationIntegration.StorageLocationDescription,
                            PlantCode = storageLocationIntegration.Plant,
                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        storageLocation.StorageLocationName = storageLocationIntegration.StorageLocationDescription;
                        storageLocation.PlantCode = storageLocationIntegration.Plant;
                        //Common
                        storageLocation.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();
                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = storageLocationIntegration.StorageLocation,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
