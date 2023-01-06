﻿using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.PurchaseOrders
{
    public class PurchaseOrderIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<PurchaseOrderIntegration> PurchaseOrders { get; set; } = new List<PurchaseOrderIntegration>();
    }

    public class PurchaseOrderIntegration
    {
        public string Plant { get; set; }
        public string PurchasingOrganization { get; set; }
        public string PurchasingGroup { get; set; }
        public string Vendor { get; set; }
        public string POType { get; set; }
        public string PurchaseOrder { get; set; }
        public string Material { get; set; }
        public DateTime? DocumentDate { get; set; }
        public List<PurchaseOrderDetailIntegration> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetailIntegration>();
    }

    public class PurchaseOrderDetailIntegration
    {
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderItem { get; set; }
        public string Material { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? OpenQuantity { get; set; }
        public string UoM { get; set; }

    }
    public class PurchaseOrderIntegrationNSCommandHandler : IRequestHandler<PurchaseOrderIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;

        public PurchaseOrderIntegrationNSCommandHandler(IRepository<PurchaseOrderMasterModel> poRep, IUnitOfWork unitOfWork, IRepository<PurchaseOrderDetailModel> poDetailRep)
        {
            _poRep = poRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
        }
        public async Task<IntegrationNSResponse> Handle(PurchaseOrderIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.PurchaseOrders.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.PurchaseOrders.Count();

            foreach (var poIntegration in request.PurchaseOrders)
            {
                try
                {
                    //Purchase Order
                    var purchaseOrder = await _poRep.GetQuery(x => x.PurchaseOrderCode == poIntegration.PurchaseOrder)
                                                    .Include(x => x.PurchaseOrderDetailModel)
                                                    .FirstOrDefaultAsync();

                    //Không có tạo mới
                    if (purchaseOrder is null)
                    {
                        //Hearder 
                        purchaseOrder = new PurchaseOrderMasterModel();
                        purchaseOrder.PurchaseOrderId = Guid.NewGuid();
                        purchaseOrder.PurchaseOrderCode = poIntegration.PurchaseOrder;
                        //purchaseOrder.PurchaseOrderCodeInt = int.Parse(poIntegration.PurchaseOrder);
                        purchaseOrder.POType = poIntegration.POType;
                        purchaseOrder.Plant = poIntegration.Plant;
                        purchaseOrder.PurchasingOrg = poIntegration.PurchasingOrganization;
                        purchaseOrder.PurchasingGroup = poIntegration.PurchasingGroup;
                        purchaseOrder.VendorCode = poIntegration.Vendor;
                        //purchaseOrder.VendorCodeInt = int.Parse(poIntegration.Vendor);
                        purchaseOrder.DocumentDate = poIntegration.DocumentDate;

                        purchaseOrder.CreateTime = DateTime.Now;
                        purchaseOrder.Actived = true;

                        //Detail
                        var detailPOs = poIntegration.PurchaseOrderDetails.Select(x => new PurchaseOrderDetailModel
                        {
                            PurchaseOrderDetailId = Guid.NewGuid(),
                            PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                            POLine = x.PurchaseOrderItem,
                            ProductCode = x.Material,
                            VehicleCode = x.VehicleCode,
                            OrderQuantity = x.OrderQuantity,
                            OpenQuantity = x.OpenQuantity,
                            Unit = x.UoM,

                            CreateTime = DateTime.Now,
                            Actived = true

                        }).ToList();

                        purchaseOrder.PurchaseOrderDetailModel = detailPOs;
                        _poRep.Add(purchaseOrder);
                    }
                    else
                    {
                        #region Master
                        //Cập nhật master
                        purchaseOrder.POType = poIntegration.POType;
                        purchaseOrder.Plant = poIntegration.Plant;
                        purchaseOrder.PurchasingOrg = poIntegration.PurchasingOrganization;
                        purchaseOrder.PurchasingGroup = poIntegration.PurchasingGroup;
                        purchaseOrder.VendorCode = poIntegration.Vendor;
                        purchaseOrder.DocumentDate = poIntegration.DocumentDate;
                        purchaseOrder.LastEditTime = DateTime.Now;

                        #endregion

                        #region Detail
                        //Cập nhật detail
                        foreach (var item in poIntegration.PurchaseOrderDetails)
                        {
                            var detailPO = await _poDetailRep.FindOneAsync(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId && x.POLine == item.PurchaseOrderItem);
                            if (detailPO == null)
                            {

                                _poDetailRep.Add(new PurchaseOrderDetailModel
                                {
                                    PurchaseOrderDetailId = Guid.NewGuid(),
                                    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                                    POLine = item.PurchaseOrderItem,
                                    ProductCode = item.Material,
                                    Batch = item.Batch,
                                    VehicleCode = item.VehicleCode,
                                    OrderQuantity = item.OrderQuantity,
                                    OpenQuantity = item.OpenQuantity,

                                    CreateTime = DateTime.Now,
                                    Actived = true
                                });
                            }
                            else
                            {
                                detailPO.ProductCode = item.Material;
                                detailPO.Batch = item.Batch;
                                detailPO.VehicleCode = item.VehicleCode;
                                detailPO.OrderQuantity = item.OrderQuantity;
                                detailPO.OpenQuantity = item.OpenQuantity;
                                detailPO.LastEditTime = DateTime.Now;
                            }
                        }
                        #endregion
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{poIntegration.PurchaseOrder}");
                }
            }

            return response;
        }
    }
}