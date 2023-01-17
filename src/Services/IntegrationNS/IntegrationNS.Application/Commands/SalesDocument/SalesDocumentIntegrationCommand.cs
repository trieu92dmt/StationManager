using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.SalesDocument
{
    public class SalesDocumentIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<SalesDocumentIntegration> SalesDocuments { get; set; } = new List<SalesDocumentIntegration>();
    }

    public class SalesDocumentIntegration
    {
        public string SalesDocumentCode { get; set; }
        public string CustomerReference { get; set; }
        public string CustomerReferenceHeader { get; set; }
        public string SalesDocumentType { get; set; }
        public string SoldtoPartyCode { get; set; }
        public string SoldToPartyName { get; set; }
        public string OverallStatus { get; set; }
        public string DeliveryStatus { get; set; }

        public List<SalesDocumentDetailIntegration> SalesDocumentDetails { get; set; } = new List<SalesDocumentDetailIntegration>();
    }

    public class SalesDocumentDetailIntegration
    {
        public string SalesDocumentItem { get; set; }
        public string Material { get; set; }
        public decimal? OrderQuantity { get; set; }
        public string SalesUnit { get; set; }
        public string DivisionCode { get; set; }
        public string SalesOffice { get; set; }
        public string SalesGroup { get; set; }
        public string SalesOrgCode { get; set; }
        public string DistributionChannelCode { get; set; }
        public string Batch { get; set; }
        public decimal? ConfirmedQuantity { get; set; }
        public string Unit { get; set; }
        public string Returns { get; set; }
        public string ShippingPoint { get; set; }
        public string Plant { get; set; }
        public string OverallStatusItem { get; set; }
        public string DeliveryStatusItem { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? GoodsIssueDate { get; set; }

    }
    public class SalesDocumentIntegrationNSCommandHandler : IRequestHandler<SalesDocumentIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SalesDocumentModel> _salesRepo;
        private readonly IRepository<DetailSalesDocumentModel> _saleDetailRepo;
        private readonly IRepository<ProductModel> _prdRepo;

        public SalesDocumentIntegrationNSCommandHandler(IUnitOfWork unitOfWork, IRepository<SalesDocumentModel> salesRepo, IRepository<DetailSalesDocumentModel> saleDetailRepo,
                                                        IRepository<ProductModel> prdRepo)
        {
            _unitOfWork = unitOfWork;
            _salesRepo = salesRepo;
            _saleDetailRepo = saleDetailRepo;
            _prdRepo = prdRepo;
        }
        public async Task<IntegrationNSResponse> Handle(SalesDocumentIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.SalesDocuments.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.SalesDocuments.Count();

            //Dữ liệu Sales Document
            var saleDocs = _salesRepo.GetQuery();

            //Dữ liệu material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            foreach (var salesDocIntegration in request.SalesDocuments)
            {
                try
                {
                    //Sales Document
                    var saleDoc = await saleDocs.Where(x => x.SalesDocumentCode == salesDocIntegration.SalesDocumentCode)
                                                    .Include(x => x.DetailSalesDocumentModel)
                                                    .FirstOrDefaultAsync();

                    //Không có tạo mới
                    if (saleDoc is null)
                    {
                        //Hearder 
                        saleDoc = new SalesDocumentModel();
                        saleDoc.SalesDocumentId = Guid.NewGuid();
                        saleDoc.SalesDocumentCode = salesDocIntegration.SalesDocumentCode;
                        saleDoc.CustomerReference = salesDocIntegration.CustomerReference;
                        saleDoc.CustomerReferenceHeader = salesDocIntegration.CustomerReferenceHeader;
                        saleDoc.SalesDocumentType = salesDocIntegration.SalesDocumentType;
                        saleDoc.SoldtoPartyCode = salesDocIntegration.SoldtoPartyCode;
                        saleDoc.SoldToPartyName = salesDocIntegration.SoldToPartyName;
                        saleDoc.OverallStatus = salesDocIntegration.OverallStatus;
                        saleDoc.DeliveryStatus = salesDocIntegration.DeliveryStatus;

                        saleDoc.CreateTime = DateTime.Now;
                        saleDoc.Actived = true;

                        //Detail
                        var saleDocDetails = new List<DetailSalesDocumentModel>();
                        foreach (var item in salesDocIntegration.SalesDocumentDetails)
                        {

                            if (materials.FirstOrDefault(x => x.ProductCode == item.Material) == null)
                                throw new ISDException(String.Format(CommonResource.Msg_NotFound, $"Material {item.Material}"));

                            saleDocDetails.Add(new DetailSalesDocumentModel
                            {
                                DetailSalesDocumentId = Guid.NewGuid(),
                                SalesDocumentId = saleDoc.SalesDocumentId,
                                SalesDocumentItem = item.SalesDocumentItem,
                                SalesUnit = item.SalesUnit,
                                OrderQuantity = item.OrderQuantity,
                                DivisionCode = item.DivisionCode,
                                SalesOffice = item.SalesOffice,
                                CreateTime = DateTime.Now,
                                Actived = true,
                                SalesGroup = item.SalesGroup,
                                SalesOrgCode = item.SalesOrgCode,
                                DistributionChannelCode = item.DistributionChannelCode,
                                Batch = item.Batch,
                                ConfirmedQuantity = item.ConfirmedQuantity,
                                Unit = item.Unit,
                                Rturns = item.Returns,
                                ProductCode = item.Material,
                                ShippingPoint = item.ShippingPoint,
                                Plant = item.Plant,
                                OverallStatusItem = item.OverallStatusItem,
                                DeliveryStatusItem = item.DeliveryStatusItem,
                                DeliveryDate = item.DeliveryDate,
                                GoodsIssueDate = item.GoodsIssueDate
                            });

                        }
                        //var detailPOs = poIntegration.PurchaseOrderDetails.Select(x => new PurchaseOrderDetailModel
                        //{
                        //    PurchaseOrderDetailId = Guid.NewGuid(),
                        //    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        //    POLine = x.PurchaseOrderItem,
                        //    ProductCode = x.Material,
                        //    OrderQuantity = x.OrderQuantity,
                        //    OpenQuantity = x.OpenQuantity,
                        //    Unit = x.UoM,

                        //    CreateTime = DateTime.Now,
                        //    Actived = true

                        //}).ToList();

                        saleDoc.DetailSalesDocumentModel = saleDocDetails;
                        _salesRepo.Add(saleDoc);
                    }
                    else
                    {
                        #region Master
                        //Cập nhật master
                        saleDoc.SalesDocumentCode = salesDocIntegration.SalesDocumentCode;
                        saleDoc.CustomerReference = salesDocIntegration.CustomerReference;
                        saleDoc.CustomerReferenceHeader = salesDocIntegration.CustomerReferenceHeader;
                        saleDoc.SalesDocumentType = salesDocIntegration.SalesDocumentType;
                        saleDoc.SoldtoPartyCode = salesDocIntegration.SoldtoPartyCode;
                        saleDoc.SoldToPartyName = salesDocIntegration.SoldToPartyName;
                        saleDoc.OverallStatus = salesDocIntegration.OverallStatus;
                        saleDoc.DeliveryStatus = salesDocIntegration.DeliveryStatus;
                        saleDoc.LastEditTime = DateTime.Now;

                        #endregion

                        #region Detail
                        //Cập nhật detail
                        foreach (var item in salesDocIntegration.SalesDocumentDetails)
                        {
                            var detailSalesDoc = await _saleDetailRepo.FindOneAsync(x => x.SalesDocumentId == saleDoc.SalesDocumentId && x.SalesDocumentItem == item.SalesDocumentItem);
                            if (detailSalesDoc == null)
                            {

                                _saleDetailRepo.Add(new DetailSalesDocumentModel
                                {
                                    DetailSalesDocumentId = Guid.NewGuid(),
                                    SalesDocumentId = saleDoc.SalesDocumentId,
                                    SalesDocumentItem = item.SalesDocumentItem,
                                    OrderQuantity = item.OrderQuantity,
                                    Batch = item.Batch,
                                    SalesUnit = item.SalesUnit,
                                    DivisionCode = item.DivisionCode,

                                    CreateTime = DateTime.Now,
                                    Actived = true,
                                    SalesOffice = item.SalesOffice,
                                    SalesGroup = item.SalesGroup,
                                    SalesOrgCode = item.SalesOrgCode,
                                    DistributionChannelCode = item.DistributionChannelCode,
                                    ConfirmedQuantity = item.ConfirmedQuantity,
                                    Unit = item.Unit,
                                    Rturns = item.Returns,
                                    ShippingPoint = item.ShippingPoint,
                                    ProductCode = item.Material,
                                    Plant = item.Plant,
                                    OverallStatusItem = item.OverallStatusItem,
                                    DeliveryStatusItem = item.DeliveryStatusItem,
                                    DeliveryDate = item.DeliveryDate,
                                    GoodsIssueDate = item.GoodsIssueDate,
                                });
                            }
                            else
                            {
                                detailSalesDoc.ProductCode = item.Material;
                                detailSalesDoc.SalesDocumentItem = item.SalesDocumentItem;
                                detailSalesDoc.OrderQuantity = item.OrderQuantity;
                                detailSalesDoc.Batch = item.Batch;
                                detailSalesDoc.SalesUnit = item.SalesUnit;
                                detailSalesDoc.DivisionCode = item.DivisionCode;
                                detailSalesDoc.SalesOffice = item.SalesOffice;
                                detailSalesDoc.SalesGroup = item.SalesGroup;
                                detailSalesDoc.SalesOrgCode = item.SalesOrgCode;
                                detailSalesDoc.DistributionChannelCode = item.DistributionChannelCode;
                                detailSalesDoc.ConfirmedQuantity = item.ConfirmedQuantity;
                                detailSalesDoc.Unit = item.Unit;
                                detailSalesDoc.Rturns = item.Returns;
                                detailSalesDoc.ShippingPoint = item.ShippingPoint;
                                detailSalesDoc.Plant = item.Plant;
                                detailSalesDoc.OverallStatusItem = item.OverallStatusItem;
                                detailSalesDoc.DeliveryStatusItem = item.DeliveryStatusItem;
                                detailSalesDoc.DeliveryDate = item.DeliveryDate;
                                detailSalesDoc.GoodsIssueDate = item.GoodsIssueDate;
                                detailSalesDoc.LastEditTime = DateTime.Now;
                            }
                        }
                        #endregion
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = salesDocIntegration.SalesDocumentCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
