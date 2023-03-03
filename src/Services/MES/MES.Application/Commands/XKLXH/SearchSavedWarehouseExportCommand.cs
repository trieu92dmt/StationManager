using Core.Models;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.XKLXH;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XKLXH
{
    public class SearchSavedWarehouseExportCommand : IRequest<ListSavedWarehouseExportResponse>
    {
        public DatatableViewModel Paging { get; set; }
        //plant
        public string Plant { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
    }

    public class SearchSavedWarehouseExportCommandHandler : IRequestHandler<SearchSavedWarehouseExportCommand, ListSavedWarehouseExportResponse>
    {
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        public SearchSavedWarehouseExportCommandHandler(IRepository<ExportByCommandModel> xklxhRepo, IRepository<ProductModel> prodRepo)
        {
            _xklxhRepo = xklxhRepo;
            _prodRepo = prodRepo;
        }

        public Task<ListSavedWarehouseExportResponse> Handle(SearchSavedWarehouseExportCommand request, CancellationToken cancellationToken)
        {
            //Get query material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Tạo query truy vấn
            var query = _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                  .Where(x => (!string.IsNullOrEmpty(request.Plant) ? x.PlantCode == request.Plant : true) &&
                                              (!string.IsNullOrEmpty(request.OutboundDelivery) ? x.DetailOD.OutboundDelivery.DeliveryCode == request.OutboundDelivery : true) &&
                                              x.RecordTime3 != null)
                                  .Select(x => new SavedWarehouseExportResponse
                                  {
                                      //Id
                                      XKLXHId = x.ExportByCommandId,
                                      //Od
                                      OutboundDelivery = x.DetailOD.OutboundDelivery.DeliveryCodeInt.ToString(),
                                      //Od item
                                      OutboundDeliveryItem = x.DetailOD.OutboundDeliveryItem,
                                      //Số xe tải
                                      TruckNumber = x.TruckNumber,
                                      //Confirm quantity
                                      ConfirmQty = x.ConfirmQty ?? 0,
                                      //SL bao
                                      BagQuantity = x.BagQuantity ?? 0,
                                      //Đơn trọng
                                      SingleWeight = x.SingleWeight ?? 0,
                                      //SL kèm bao bì
                                      QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                                      //Material desc
                                      MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName,
                                      //Material
                                      Material = x.MaterialCodeInt.ToString(),
                                      //Plant
                                      Plant = x.PlantCode,
                                      //Thời gian ghi nhận
                                      RecordTime = x.RecordTime3
                                  }).AsNoTracking();

            int filterResultsCount = 0;
            int totalResultsCount = 0;
            int totalPagesCount = 0;

            var response = new ListSavedWarehouseExportResponse();


            //Lấy danh sách TestTarget có phân trang
            var res = CustomSearch.CustomSearchFunc<SavedWarehouseExportResponse>(request.Paging, out filterResultsCount, out totalResultsCount, out totalPagesCount, query, "STT");

            //Đánh số thứ tự
            if (res != null && res.Count() > 0)
            {
                int i = request.Paging.offset;
                foreach (var item in res)
                {
                    i++;
                    item.STT = i;
                }
            }

            response.SavedWarehouseExports = res;
            response.PagingRep.FilterResultsCount = filterResultsCount;
            response.PagingRep.TotalResultsCount = totalResultsCount;
            response.PagingRep.TotalPagesCount = totalPagesCount;

            return Task.FromResult(response);
        }
    }
}
