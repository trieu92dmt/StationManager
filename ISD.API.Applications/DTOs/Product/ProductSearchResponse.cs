using ISD.API.Applications.DTOs.Common;

namespace ISD.API.Applications.DTOs.Product
{
    public class ProductSearchResponse
    {
        public int STT { get; set; }
        public Guid ProductId { get; set; }
        //Phân cấp sản phẩm
        public string ParentCategoryName { get; set; }
        //Nhóm sản phẩm
        public string CategoryName { get; set; }
        //Nhóm sản phẩm chi tiết
        public string CategoryDetailName { get; set; }
        //Mã TP/BTP
        public string ProductCode { get; set; }
        //Tên TP/BTP
        public string ProductName { get; set; }
        public string Serial { get; set; }
        //Has Routing
        public bool HasRouting { get; set; }
        //Trạng thái
        public bool? Actived { get; set; }
        public bool IsMold { get; set; }
    }

    public class ProductSearchListResponse
    {
        public List<ProductSearchResponse> Products { get; set; } = new List<ProductSearchResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
