using ISD.API.Applications.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Equipment
{
    public class EquipmentResponse
    {
        public int STT { get; set; }
        //1.Id
        public Guid EquipmentId { get; set; }
        //2.Mã máy móc/chuyền
        public string EquipmentCode { get; set; }
        //3.Tên máy móc/chuyền
        public string EquipmentName { get; set; }
        //4.Nhóm máy móc
        public string EquipmentGroup { get; set; }
        //5.Phân xưởng
        public string WorkShop { get; set; }
        //6.Phân loại
        public string EquipmentType { get; set; }
        //7.Mô tả
        public string Description { get; set; }
        //8.Công suất máy/chuyền
        public decimal? EquipmentProduction { get; set; }
        //9.Đơn vị tính công suất
        public string Unit { get; set; }
        //10.Trạng thái máy móc
        public string Status { get; set; }
    }

    public class EquipmentListResponse
    {
        //Danh sách máy móc
        public List<EquipmentResponse> Equipments { get; set; } = new List<EquipmentResponse>();
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
