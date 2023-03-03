using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NHLT
{
    public class GetDataByOdAndOdItemResponse
    {
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Document date
        public DateTime? DocumentDate { get; set; }
    }
}
