using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Common
{
    public class PagingResponse
    {
        public int FilterResultsCount { get; set; }
        public int TotalResultsCount { get; set; }
        public int TotalPagesCount { get; set; }
    }
}
