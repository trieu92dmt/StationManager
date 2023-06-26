using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.Permission
{
    public class SearchAccountCommand
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public bool? Active { get; set; }
        public string Role { get; set; }
    }
}
