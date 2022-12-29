using ISD.API.Applications.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Company
{
    public class CompanySearchResponse
    {
        public int STT { get; set; }
        //Id
        public Guid CompanyId { get; set; }
        //Mã công ty
        public string CompanyCode { get; set; }
        //Tên công ty
        public string CompanyName { get; set; }
        //Logo
        public string Logo { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }
}
