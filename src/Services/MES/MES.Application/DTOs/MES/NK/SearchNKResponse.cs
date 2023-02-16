using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NK
{
    public class GetInputDataResponse
    {
        //Indexkey
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        //Customer
        public string Customer { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //UoM
        public string Unit { get; set; }
    }

    public class SearchNKResponse
    {
        //NK ID
        //Plant
        //Material
        //Material Desc
        //Customer
        //Cusstomer
        //
    }
}
