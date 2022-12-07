namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class SaveBarcodeWOResponse
    {
        public Guid BatchId { get; set; }
        public List<Guid> WorkOrderCardIds { get; set; } = new List<Guid>();
    }
}
