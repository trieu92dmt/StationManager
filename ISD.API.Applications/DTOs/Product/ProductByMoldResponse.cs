namespace ISD.API.Applications.DTOs.Product
{
    public class ProductByMoldResponse
    {
        public Guid ProductId { get; set; }
        public string MoldCode { get; set; }
        public string SerialNumber { get; set; }
        public string StatusMold { get; set; }
        public string FilmCheck { get; set; }
        public string MoldName { get; set; }
        public string Stock { get; set; }
        public string Bin { get; set; }
        public DateTime? MainDate { get; set; }
        //Số lần dập
        public int? TimeStamping { get; set; }
        //Số lượng dập cần cảnh báo
        public int? StampQuantityAlert { get; set; }
        //Số lần dập hiện tại
        public int? CurrentStampQuantity { get; set; }
        public string Description { get; set; }
        public bool? Actived { get; set; }
        //Kho khuôn
        public string LocationNote { get; set; }
    }
}
