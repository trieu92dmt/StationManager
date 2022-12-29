using ISD.API.Applications.Commands.MES;
using ISD.API.Applications.DTOs.MES;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Queries.MES.NKHMH
{
    public interface INKMHQuery
    {
        Task<NKMHMesResponse> GetNKMH(GetNKMHCommand request);
    }
    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<ProductModel> _prdRep;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep)
        {
            _nkmhRep = nkmhRep;
            _prdRep = prdRep;
        }

        public async Task<NKMHMesResponse> GetNKMH(GetNKMHCommand request)
        {
            #region Format Day

            //Ngày phát lệnh
            if (request.DocumentDateFrom.HasValue)
            {
                request.DocumentDateFrom = request.DocumentDateFrom.Value.Date;
            }
            if (request.DocumentDateTo.HasValue)
            {
                request.DocumentDateTo = request.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Product
            var product = _prdRep.GetQuery().AsNoTracking();

            var query = _nkmhRep.GetQuery()
                                .Include(x => x.PurchaseOrderDetail).ThenInclude(x => x.PurchaseOrder)
                                .AsNoTracking();

            var data = query.Select(x => new NKMHMesResponse
            {
                //Plant
                Plant = x.PurchaseOrderDetail.PurchaseOrder.Plant,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode,
                POItem = x.PurchaseOrderDetail.POLine,
                //Product
                Material = x.PurchaseOrderDetail.ProductCode,


            });

            throw new NotImplementedException();
        }
    }
}
