using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.OutboundDelivery
{
    public class OutboundDeliveryIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<OutboundDeliveryIntegration> OutboundDeliveries { get; set; } = new List<OutboundDeliveryIntegration>();
    }

    public class OutboundDeliveryIntegration
    {
        public string Plant { get; set; }
        public string PurchasingOrganization { get; set; }
        public string PurchasingGroup { get; set; }
        public string Vendor { get; set; }
        public string POType { get; set; }
        public string PurchaseOrder { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string ReleaseIndicator { get; set; }
        public List<PurchaseOrderDetailIntegration> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetailIntegration>();
    }

    public class OutboundDeliveryDetailIntegration
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
        public decimal? QuantityReceived { get; set; }
        public string DeletionInd { get; set; }
        public string Deliver { get; set; }
        public string VehicleOwner { get; set; }
        public string TransportUnit { get; set; }
        public string DeliveryCompleted { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string WeightUnit { get; set; }
    }
}
