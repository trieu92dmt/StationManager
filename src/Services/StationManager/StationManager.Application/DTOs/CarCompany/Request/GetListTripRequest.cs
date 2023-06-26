using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListTripRequest
    {
        public Guid AccountId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime? StartDate { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }
        public string Driver { get; set; }
    }
}
