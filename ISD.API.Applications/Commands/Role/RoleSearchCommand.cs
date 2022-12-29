using ISD.API.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Commands.Role
{
    public class RoleSearchCommand
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        public string RoleName { get; set; }
    }
}