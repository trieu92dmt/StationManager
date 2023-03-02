using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.Reservation
{
    public class ReservationIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<ReservationIntegration> ReservationIntegrations { get; set; } = new List<ReservationIntegration>();
    }

    public class ReservationIntegration
    {
        public string ReservationCode { get; set; }         //Mã Reservation
        public string RequiremenType { get; set; }                 //Requirement type
        public string Status { get; set; }                 //Status
        public string FinalIssue { get; set; }                 //Final issue
        public string Plant { get; set; }                 //Plant
        public string Sloc { get; set; }                 //Storage location
        public string ReceivingPlant { get; set; }                 //Receiving Plant
        public string ReceivingSloc { get; set; }                 //Receiving stor.loc.
        public string UnloadingPoint { get; set; }                 //Unloading Point
        public string Supplier { get; set; }                 //Supplier
        public string Customer { get; set; }                       //Customer
        public List<DetailReservationIntegration> DetailReservationIntegrations { get; set; } = new List<DetailReservationIntegration>();
    }

    public class DetailReservationIntegration
    {

        public string ReservationItem { get; set; }          //Item number of reservation
        public string ItemDeleted { get; set; }                  //Item delete
        public string MovementAllowed { get; set; }                  //Movement allowed
        public string MissingPart { get; set; }                  //Missing Part
        public string Material { get; set; }                  //Material
        public string Batch { get; set; }                  //Batch
        public string SpecialStock { get; set; }                  //Special Stock
        public DateTime? RequirementsDate { get; set; }                  //Requirements date
        public decimal? RequirementQty { get; set; }                  //Requirement Quantity
        public string BaseUnit { get; set; }                  //Base Unit of Measure
        public decimal? QtyIsFixed { get; set; }                  //Quantity is fixed
        public decimal? QtyWithdrawn { get; set; }                  //Quantity withdrawn
        public decimal? QtyInUnitOfEntry { get; set; }                  //Qty in unit of entry
        public string UnitOfEntry { get; set; }                  //Unit of Entry
        public string PlannedOrder { get; set; }                  //Planned Order
        public string PurchaseRequisition { get; set; }                  //Purchase Requisition
        public string ItemOfRequisition { get; set; }                  //Item of requisition
        public string Order { get; set; }                  //Order
        public string PeggedRequirement { get; set; }                  //Pegged Requirement
        public string SalesOrder { get; set; }                  //Sales Order
        public string SalesOrderItem { get; set; }                  //Sales order item
        public string MovementType { get; set; }                  //Movement type
        public string PurchasingDoc { get; set; }                  //Purchasing Document
        public string Item { get; set; }                  //Item
        public string MaterialOrigin { get; set; }                  //Material origin
        public string MaterialGr { get; set; }                  //Material Group
        public string ReceivingBatch { get; set; }                          //Receiving Batch

    }
;
    public class ReservationIntegrationNSCommandHandler : IRequestHandler<ReservationIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ReservationModel> _reservationRepo;
        private readonly IRepository<DetailReservationModel> _detailReservationRepo;

        public ReservationIntegrationNSCommandHandler(IUnitOfWork unitOfWork, IRepository<ReservationModel> reservationRepo,
                                                      IRepository<DetailReservationModel> detailReservationRepo)
        {
            _unitOfWork = unitOfWork;
            _reservationRepo = reservationRepo;
            _detailReservationRepo = detailReservationRepo;
        }

        public async Task<IntegrationNSResponse> Handle(ReservationIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.ReservationIntegrations.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.ReservationIntegrations.Count();


            foreach (var item in request.ReservationIntegrations)
            {
                try
                {
                    //Reservation
                    var reservation = await _reservationRepo.GetQuery(x => x.ReservationCode == item.ReservationCode)
                                                    .Include(x => x.DetailReservationModel)
                                                    .FirstOrDefaultAsync();

                    //Không có tạo mới
                    if (reservation is null)
                    {
                        //Hearder 
                        reservation = new ReservationModel();
                        reservation.ReservationId = Guid.NewGuid();
                        reservation.ReservationCode = item.ReservationCode;
                        reservation.ReservationCodeInt = long.Parse(item.ReservationCode);
                        reservation.RequiremenType = item.RequiremenType;
                        reservation.Status = item.Status;
                        reservation.FinalIssue = item.FinalIssue;
                        reservation.Plant = item.Plant;
                        reservation.Sloc = item.Sloc;
                        reservation.ReceivingPlant = item.ReceivingPlant;
                        reservation.ReceivingSloc = item.ReceivingSloc;
                        reservation.UnloadingPoint = item.UnloadingPoint;
                        reservation.Supplier = item.Supplier;
                        reservation.Customer = item.Customer;


                        reservation.CreateTime = DateTime.Now;
                        reservation.Actived = true;

                        //Detail
                        var detailReservations = new List<DetailReservationModel>();
                        foreach (var detail in item.DetailReservationIntegrations)
                        {

                            detailReservations.Add(new DetailReservationModel
                            {

                                DetailReservationId = Guid.NewGuid(),
                                ReservationId = reservation.ReservationId,
                                ReservationItem = detail.ReservationItem,
                                ItemDeleted = detail.ItemDeleted,
                                MovementAllowed = detail.MovementAllowed,
                                MissingPart = detail.MissingPart,
                                Material = detail.Material,
                                MaterialCodeInt = long.Parse(detail.Material),
                                Batch = detail.Batch,
                                SpecialStock = detail.SpecialStock,
                                RequirementsDate = detail.RequirementsDate,
                                RequirementQty = detail.RequirementQty ?? 0,
                                BaseUnit = detail.BaseUnit,
                                QtyIsFixed = detail.QtyIsFixed,
                                QtyWithdrawn = detail.QtyWithdrawn ?? 0,
                                QtyInUnitOfEntry = detail.QtyInUnitOfEntry,
                                UnitOfEntry = detail.UnitOfEntry,
                                PlannedOrder = detail.PlannedOrder,
                                PurchaseRequisition = detail.PurchaseRequisition,
                                ItemOfRequisition = detail.ItemOfRequisition,
                                Order = detail.Order,
                                PeggedRequirement = detail.PeggedRequirement,
                                SalesOrder = detail.SalesOrder,
                                SalesOrderItem = detail.SalesOrderItem,
                                MovementType = detail.MovementType,
                                PurchasingDoc = detail.PurchasingDoc,
                                Item = detail.Item,
                                MaterialOrigin = detail.MaterialOrigin,
                                MaterialGr = detail.MaterialGr,
                                ReceivingBatch = detail.ReceivingBatch,



                                CreateTime = DateTime.Now,
                                Actived = true
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

                        reservation.DetailReservationModel = detailReservations;
                        _reservationRepo.Add(reservation);
                    }
                    else
                    {
                        #region Master
                        //Cập nhật master
                        reservation.RequiremenType = item.RequiremenType;
                        reservation.Status = item.Status;
                        reservation.FinalIssue = item.FinalIssue;
                        reservation.Plant = item.Plant;
                        reservation.Sloc = item.Sloc;
                        reservation.ReceivingPlant = item.ReceivingPlant;
                        reservation.ReceivingSloc = item.ReceivingSloc;
                        reservation.UnloadingPoint = item.UnloadingPoint;
                        reservation.Supplier = item.Supplier;
                        reservation.Customer = item.Customer;
                        reservation.LastEditTime = DateTime.Now;

                        #endregion

                        #region Detail
                        //Cập nhật detail
                        foreach (var detail in item.DetailReservationIntegrations)
                        {
                            //Check tồn tại
                            var detailReservation = await _detailReservationRepo.FindOneAsync(x => x.ReservationId == reservation.ReservationId && x.ReservationItem == detail.ReservationItem);
                            if (detailReservation == null)
                            {

                                _detailReservationRepo.Add(new DetailReservationModel
                                {
                                    DetailReservationId = Guid.NewGuid(),
                                    ReservationId = reservation.ReservationId,
                                    ReservationItem = detail.ReservationItem,
                                    ItemDeleted = detail.ItemDeleted,
                                    MovementAllowed = detail.MovementAllowed,
                                    MissingPart = detail.MissingPart,
                                    Material = detail.Material,
                                    MaterialCodeInt = long.Parse(detail.Material),
                                    Batch = detail.Batch,
                                    SpecialStock = detail.SpecialStock,
                                    RequirementsDate = detail.RequirementsDate,
                                    RequirementQty = detail.RequirementQty ?? 0,
                                    BaseUnit = detail.BaseUnit,
                                    QtyIsFixed = detail.QtyIsFixed,
                                    QtyWithdrawn = detail.QtyWithdrawn ?? 0,
                                    QtyInUnitOfEntry = detail.QtyInUnitOfEntry,
                                    UnitOfEntry = detail.UnitOfEntry,
                                    PlannedOrder = detail.PlannedOrder,
                                    PurchaseRequisition = detail.PurchaseRequisition,
                                    ItemOfRequisition = detail.ItemOfRequisition,
                                    Order = detail.Order,
                                    PeggedRequirement = detail.PeggedRequirement,
                                    SalesOrder = detail.SalesOrder,
                                    SalesOrderItem = detail.SalesOrderItem,
                                    MovementType = detail.MovementType,
                                    PurchasingDoc = detail.PurchasingDoc,
                                    Item = detail.Item,
                                    MaterialOrigin = detail.MaterialOrigin,
                                    MaterialGr = detail.MaterialGr,
                                    ReceivingBatch = detail.ReceivingBatch,
                                    CreateTime = DateTime.Now,
                                    Actived = true
                                });
                            }
                            else
                            {
                                detailReservation.ItemDeleted = detail.ItemDeleted;
                                detailReservation.MovementAllowed = detail.MovementAllowed;
                                detailReservation.MissingPart = detail.MissingPart;
                                detailReservation.Material = detail.Material;
                                detailReservation.MaterialCodeInt = long.Parse(detail.Material);
                                detailReservation.Batch = detail.Batch;
                                detailReservation.SpecialStock = detail.SpecialStock;
                                detailReservation.RequirementsDate = detail.RequirementsDate;
                                detailReservation.RequirementQty = detail.RequirementQty ?? 0;
                                detailReservation.BaseUnit = detail.BaseUnit;
                                detailReservation.QtyIsFixed = detail.QtyIsFixed;
                                detailReservation.QtyWithdrawn = detail.QtyWithdrawn ?? 0;
                                detailReservation.QtyInUnitOfEntry = detail.QtyInUnitOfEntry;
                                detailReservation.UnitOfEntry = detail.UnitOfEntry;
                                detailReservation.PlannedOrder = detail.PlannedOrder;
                                detailReservation.PurchaseRequisition = detail.PurchaseRequisition;
                                detailReservation.ItemOfRequisition = detail.ItemOfRequisition;
                                detailReservation.Order = detail.Order;
                                detailReservation.PeggedRequirement = detail.PeggedRequirement;
                                detailReservation.SalesOrder = detail.SalesOrder;
                                detailReservation.SalesOrderItem = detail.SalesOrderItem;
                                detailReservation.MovementType = detail.MovementType;
                                detailReservation.PurchasingDoc = detail.PurchasingDoc;
                                detailReservation.Item = detail.Item;
                                detailReservation.MaterialOrigin = detail.MaterialOrigin;
                                detailReservation.MaterialGr = detail.MaterialGr;
                                detailReservation.ReceivingBatch = detail.ReceivingBatch;
                                detailReservation.LastEditTime = DateTime.Now;
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
                        RecordFail = item.ReservationCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
