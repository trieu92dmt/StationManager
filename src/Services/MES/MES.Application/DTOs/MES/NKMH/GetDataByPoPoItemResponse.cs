using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKMH
{
    public class GetDataByPoPoItemResponse
    {
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialName { get; set; }
        //UoM
        public string UOM { get; set; }
        //Batch
        public string Batch { get; set; }
        //Vendor Code
        public string VendorCode { get; set; }
        //Vendor Name
        public string VendorName { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total Quantity
        public decimal? TotalQuantity { get; set; }
        //Delivery Quantity
        public decimal? QuantityReceived { get; set; }
        //Opent Quantity
        public decimal? OpenQuantity { get; set; }

    }
}
