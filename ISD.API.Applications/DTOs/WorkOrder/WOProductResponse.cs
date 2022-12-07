using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class WOProductResponse
    {
        //Mã NVL
        public string Component { get; set; }
        public string Description { get; set; }
        //Số lượng
        public decimal? Quantity { get; set; }
        //ĐVT
        public string Unit { get; set; }
        //Mã Marquette
        public string MarquetteCode { get; set; }
        //Khổ nguyên (mm)
        public decimal? Size { get; set; }
        //Khổ in (mm)
        public decimal? SizePrint { get; set; }
        //Số lượng xuất kho
        public decimal? QuantityExport { get; set; }
        //Số lượng tờ in (tờ cắt ) (được xả từ tờ nguyên)
        public decimal? QuantityPaperCut { get; set; }
        //Số lượng tờ in ( tờ cắt) cần đạt tối thiểu
        public decimal? QuantityPaperCutMin { get; set; }
        //Thông số màu
        public string ColorParam { get; set; }
        //Mã màu pha
        public string ColorCode { get; set; }
        //Quy cách in
        public string PrintSpecifications { get; set; }
        //khổ kẽm
        public string ZincSize { get; set; }
        //Số kẽm
        public decimal? ZincNumber { get; set; }
        //Ghi chú
        public string Note { get; set; }
        //Gia công ngoài
        public string OutResource { get; set; }

    }
}
