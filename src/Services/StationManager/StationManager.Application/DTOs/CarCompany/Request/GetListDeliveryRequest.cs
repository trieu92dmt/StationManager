using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListDeliveryRequest
    {
        public Guid? RouteId { get; set; }
        public string Receiver { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}
