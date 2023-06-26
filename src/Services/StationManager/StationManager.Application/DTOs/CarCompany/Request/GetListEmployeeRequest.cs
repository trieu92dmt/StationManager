using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListEmployeeRequest
    {
        public Guid AccountId { get; set; }
        public int? EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PositionCode { get; set; }
    }
}
