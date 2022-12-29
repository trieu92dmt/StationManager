using ISD.API.Applications.DTOs.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Queries.MES.NKHMH
{
    public interface INKMHQuery
    {
        Task<NKMHMesResponse> GetNKMH();
    }
    public class NKMHQuery : INKMHQuery
    {
    }
}
