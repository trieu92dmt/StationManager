using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class BankDTO
    {
        public string bankcode { get; set; }
        public string name { get; set; }
        public int displayorder { get; set; }
        public int pmcid { get; set; }
    }

    public class BankListResponse
    {
        public string returncode { get; set; }
        public string returnmessage { get; set; }
        public Dictionary<string, List<BankDTO>> banks { get; set; }
    }
}
