using ISD.API.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Commands.Company
{
    public class CompanySearchCommand
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        public string CompanyName { get; set; }
        public bool? Actived { get; set; }
    }
}
