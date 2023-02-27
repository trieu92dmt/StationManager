using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NNVL
{
    public class GetInputDataResponse
    {
        //Plant
        public string Plant { get; set; }
        //Vendor
        public string Vendor { get; set; }
        //Vendor name
        public string VendorName { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //UOM
        public string Uint { get; set; }
    }
    public class SearchNNVLResponse
    {
    }
}
