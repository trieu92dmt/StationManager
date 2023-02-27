using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NNVL
{
    public class SearchNNVLCommand
    {
        //Plant
        [Required(ErrorMessage = "Plant is required")]
        public string Plant { get; set; }
        //Vendor
        [Required(ErrorMessage = "Vendor is required")]
        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
        //Material
        [Required(ErrorMessage = "Material is required")]
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân 
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }
    }
}
