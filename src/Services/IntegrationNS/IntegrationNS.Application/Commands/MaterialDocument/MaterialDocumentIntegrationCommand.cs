using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.Graph.TermStore;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Grpc.Core.Metadata;
using ZXing.QrCode.Internal;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.MaterialDocument
{
    public class MaterialDocumentIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<MaterialDocumentIntegration> MaterialDocs { get; set; } = new List<MaterialDocumentIntegration>();
    }
    public class MaterialDocumentIntegration
    {
        public string MaterialDocCode { get; set; }      //Material Document
        public string MaterialDocItem { get; set; }      //Material Doc.Item
        public string Identification { get; set; }      //Identification
        public string ParentLineID { get; set; }      //Parent line ID
        public string HierarchyLevel { get; set; }      //Hierarchy level
        public string OriginalLineItem { get; set; }      //Original Line Item
        public string MovementType { get; set; }      //Movement type
        public string ItemAutoCreated { get; set; }      //Item automatically created
        public string MaterialCode { get; set; }      //Material
        public string PlantCode { get; set; }      //Plant
        public string StorageLocation { get; set; }      //Storage location
        public string Batch { get; set; }      //Batch
        public string StockType { get; set; }      //Stock Type
        public string BatchRestricted { get; set; }      //        Batch Restricted
        public string SpecialStock { get; set; }      //Special Stock
        public string Supplier { get; set; }      //Supplier
        public string Customer { get; set; }      //Customer
        public string SalesOrder { get; set; }      //Sales Order
        public string SalesOrderItem { get; set; }      //Sales order item
        public string SalesOrderSchedule { get; set; }      //Sales order schedule
        public string DebitCredit { get; set; }      //Debit/Credit ind
        public decimal? Quantity { get; set; }      //Quantity
        public string BaseUOM { get; set; }      //Base Unit of Measure
        public decimal? QtyUnitOfEntry { get; set; }      //Qty in unit of entry
        public string UnitOfEntry { get; set; }      //Unit of Entry
        public decimal? QtyOPU { get; set; }      //Qty in OPUn
        public string OPUnit { get; set; }      //Order Price Unit
        public string PurchaseOrder { get; set; }      //Purchase order
        public string Item { get; set; }      //Item
        public string ReferenceDocItem { get; set; }      //Reference Doc.Item
        public string MaterialDocYear { get; set; }      //Material Doc.Year
        public string MaterialDoc2 { get; set; }      //Material Document 2
        public string MaterialDocItem2 { get; set; }      //Material Doc.Item2
        public string DeliveryCompleted { get; set; }      //Delivery Completed
        public string Text { get; set; }      //Text
        public string Reservation { get; set; }      //Reservation
        public string ItemReservation { get; set; }      //Item number of reservation
        public string FinalIssue { get; set; }      //Final issue
        public string ReceivingMaterial { get; set; }      //Receiving Material
        public string ReceivingPlant { get; set; }      //Receiving plant
        public string ReceivingSloc { get; set; }      //Receiving stor.loc.
        public string ReceivingBatch { get; set; }      //Receiving Batch
        public string MovementIndicator { get; set; }      //Movement indicator
        public string ReceiptIndicator { get; set; }      //Receipt Indicator
        public decimal? QtyOrderUnit { get; set; }      //Qty in order unit
        public string OrderUnit { get; set; }      //Order Unit
        public string Supplier2 { get; set; }      //Supplier 2
        public string SalesOrder2 { get; set; }      //Sales order 2
        public string SalesOrderItem2 { get; set; }      //Sales order item 2
        public DateTime? PostingDate { get; set; }      //Posting Date
        public DateTime? EntryDateTime { get; set; }      //Entry Date
        public string Reference { get; set; }      //Reference
        public string TransactionCode { get; set; }      //Transaction Code
        public string Delivery { get; set; }      //Delivery
        public string Item2 { get; set; }      //Item2
        public string CompletedIndicator { get; set; }      //Completed indicator

    }
    public class MaterialDocumentIntegrationCommandHandler : IRequestHandler<MaterialDocumentIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MaterialDocumentModel> _mateDoc;
        private readonly IRepository<ProductModel> _prodRepo;

        public MaterialDocumentIntegrationCommandHandler(IUnitOfWork unitOfWork, IRepository<MaterialDocumentModel> mateDoc, IRepository<ProductModel> prodRepo)
        {
            _unitOfWork = unitOfWork;
            _mateDoc = mateDoc;
            _prodRepo = prodRepo;
        }

        public async Task<IntegrationNSResponse> Handle(MaterialDocumentIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.MaterialDocs.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.MaterialDocs.Count();

            var materials = _mateDoc.GetQuery();

            var prods = _prodRepo.GetQuery().AsNoTracking();

            foreach (var item in request.MaterialDocs)
            {
                try
                {
                    //Check material
                    if (prods.FirstOrDefault(x => x.ProductCode == item.MaterialCode) == null)
                        throw new ISDException(String.Format(CommonResource.Msg_NotFound, "Material"));

                    //Check tồn tại
                    var materialDoc = materials.FirstOrDefault(x => (x.MaterialDocCode == item.MaterialDocCode) && (x.MaterialDocItem == item.MaterialDocItem));

                    if (materialDoc is null)
                    {
                        _mateDoc.Add(new MaterialDocumentModel
                        {
                            MaterialDocId = Guid.NewGuid(),
                            MaterialDocCode = item.MaterialDocCode,
                            MaterialDocItem = item.MaterialDocItem,
                            Identification = item.Identification,
                            ParentLineID = item.ParentLineID,
                            HierarchyLevel = item.HierarchyLevel,
                            OriginalLineItem = item.OriginalLineItem,
                            MovementType = item.MovementType,
                            ItemAutoCreated = item.ItemAutoCreated,
                            MaterialCode = item.MaterialCode,
                            MaterialCodeInt = long.Parse(item.MaterialCode),
                            PlantCode = item.PlantCode,
                            StorageLocation = item.StorageLocation,
                            Batch = item.Batch,
                            StockType = item.StockType,
                            BatchRestricted = item.BatchRestricted,
                            SpecialStock = item.SpecialStock,
                            Supplier = item.Supplier,
                            Customer = item.Customer,
                            SalesOrder = item.SalesOrder,
                            SalesOrderItem = item.SalesOrderItem,
                            SalesOrderSchedule = item.SalesOrderSchedule,
                            DebitCredit = item.DebitCredit,
                            Quantity = item.Quantity,
                            BaseUOM = item.BaseUOM,
                            QtyUnitOfEntry = item.QtyUnitOfEntry,
                            UnitOfEntry = item.UnitOfEntry,
                            QtyOPU = item.QtyOPU,
                            OPUnit = item.OPUnit,
                            PurchaseOrder = item.PurchaseOrder,
                            Item = item.Item,
                            ReferenceDocItem = item.ReferenceDocItem,
                            MaterialDocYear = item.MaterialDocYear,
                            MaterialDoc2 = item.MaterialDoc2,
                            MaterialDocItem2 = item.MaterialDocItem2,
                            DeliveryCompleted = item.DeliveryCompleted,
                            Text = item.Text,
                            Reservation = item.Reservation,
                            ItemReservation = item.ItemReservation,
                            FinalIssue = item.FinalIssue,
                            ReceivingMaterial = item.ReceivingMaterial,
                            ReceivingPlant = item.ReceivingPlant,
                            ReceivingSloc = item.ReceivingSloc,
                            ReceivingBatch = item.ReceivingBatch,
                            MovementIndicator = item.MovementIndicator,
                            ReceiptIndicator = item.ReceiptIndicator,
                            QtyOrderUnit = item.QtyOrderUnit,
                            OrderUnit = item.OrderUnit,
                            Supplier2 = item.Supplier2,
                            SalesOrder2 = item.SalesOrder2,
                            SalesOrderItem2 = item.SalesOrderItem2,
                            PostingDate = item.PostingDate,
                            EntryDateTime = item.EntryDateTime,
                            Reference = item.Reference,
                            TransactionCode = item.TransactionCode,
                            Delivery = item.Delivery,
                            Item2 = item.Item2,
                            CompletedIndicator = item.CompletedIndicator,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        materialDoc.Identification = item.Identification;
                        materialDoc.ParentLineID = item.ParentLineID;
                        materialDoc.HierarchyLevel = item.HierarchyLevel;
                        materialDoc.OriginalLineItem = item.OriginalLineItem;
                        materialDoc.MovementType = item.MovementType;
                        materialDoc.ItemAutoCreated = item.ItemAutoCreated;
                        materialDoc.MaterialCode = item.MaterialCode;
                        materialDoc.MaterialCodeInt = long.Parse(item.MaterialCode);
                        materialDoc.PlantCode = item.PlantCode;
                        materialDoc.StorageLocation = item.StorageLocation;
                        materialDoc.Batch = item.Batch;
                        materialDoc.StockType = item.StockType;
                        materialDoc.BatchRestricted = item.BatchRestricted;
                        materialDoc.SpecialStock = item.SpecialStock;
                        materialDoc.Supplier = item.Supplier;
                        materialDoc.Customer = item.Customer;
                        materialDoc.SalesOrder = item.SalesOrder;
                        materialDoc.SalesOrderItem = item.SalesOrderItem;
                        materialDoc.SalesOrderSchedule = item.SalesOrderSchedule;
                        materialDoc.DebitCredit = item.DebitCredit;
                        materialDoc.Quantity = item.Quantity;
                        materialDoc.BaseUOM = item.BaseUOM;
                        materialDoc.QtyUnitOfEntry = item.QtyUnitOfEntry;
                        materialDoc.UnitOfEntry = item.UnitOfEntry;
                        materialDoc.QtyOPU = item.QtyOPU;
                        materialDoc.OPUnit = item.OPUnit;
                        materialDoc.PurchaseOrder = item.PurchaseOrder;
                        materialDoc.Item = item.Item;
                        materialDoc.ReferenceDocItem = item.ReferenceDocItem;
                        materialDoc.MaterialDocYear = item.MaterialDocYear;
                        materialDoc.MaterialDoc2 = item.MaterialDoc2;
                        materialDoc.MaterialDocItem2 = item.MaterialDocItem2;
                        materialDoc.DeliveryCompleted = item.DeliveryCompleted;
                        materialDoc.Text = item.Text;
                        materialDoc.Reservation = item.Reservation;
                        materialDoc.ItemReservation = item.ItemReservation;
                        materialDoc.FinalIssue = item.FinalIssue;
                        materialDoc.ReceivingMaterial = item.ReceivingMaterial;
                        materialDoc.ReceivingPlant = item.ReceivingPlant;
                        materialDoc.ReceivingSloc = item.ReceivingSloc;
                        materialDoc.ReceivingBatch = item.ReceivingBatch;
                        materialDoc.MovementIndicator = item.MovementIndicator;
                        materialDoc.ReceiptIndicator = item.ReceiptIndicator;
                        materialDoc.QtyOrderUnit = item.QtyOrderUnit;
                        materialDoc.OrderUnit = item.OrderUnit;
                        materialDoc.Supplier2 = item.Supplier2;
                        materialDoc.SalesOrder2 = item.SalesOrder2;
                        materialDoc.SalesOrderItem2 = item.SalesOrderItem2;
                        materialDoc.PostingDate = item.PostingDate;
                        materialDoc.EntryDateTime = item.EntryDateTime;
                        materialDoc.Reference = item.Reference;
                        materialDoc.TransactionCode = item.TransactionCode;
                        materialDoc.Delivery = item.Delivery;
                        materialDoc.Item2 = item.Item2;
                        materialDoc.CompletedIndicator = item.CompletedIndicator;
                        materialDoc.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = item.MaterialDocCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
