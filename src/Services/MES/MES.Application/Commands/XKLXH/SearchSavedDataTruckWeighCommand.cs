using Core.Models;
using Core.SeedWork.Repositories;
using Core.SeedWork;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.XKLXH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.XKLXH
{
    public class SearchSavedDataTruckWeighCommand : IRequest<ListSavedDataTruckWeighResponse>
    {
        public DatatableViewModel Paging { get; set; }
        //plant
        public string Plant { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
    }

    public class SearchSavedDataTruckWeighCommandHandler : IRequestHandler<SearchSavedDataTruckWeighCommand, ListSavedDataTruckWeighResponse>
    {
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<ProductModel> _prodRepo;

        public SearchSavedDataTruckWeighCommandHandler(IRepository<ExportByCommandModel> xklxhRepo, IRepository<ProductModel> prodRepo)
        {
            _xklxhRepo = xklxhRepo;
            _prodRepo = prodRepo;
        }

        public Task<ListSavedDataTruckWeighResponse> Handle(SearchSavedDataTruckWeighCommand request, CancellationToken cancellationToken)
        {
            //Get query material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Tạo query truy vấn
            var query = _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                  .Where(x => (!string.IsNullOrEmpty(request.Plant) ? x.PlantCode == request.Plant : true) &&
                                              (!string.IsNullOrEmpty(request.OutboundDelivery) ? x.DetailOD.OutboundDelivery.DeliveryCode == request.OutboundDelivery : true) &&
                                              x.RecordTime2 != null)
                                  .Select(x => new SavedDataTruckWeighReponse
                                  {
                                      //Id
                                      XKLXHId = x.ExportByCommandId,
                                      //Od
                                      OutboundDelivery = x.DetailOD.OutboundDelivery.DeliveryCodeInt.ToString(),
                                      //Od item
                                      OutboundDeliveryItem = x.DetailOD.OutboundDeliveryItem,
                                      //Số xe tải
                                      TruckNumber = x.TruckNumber,
                                      //Số cân đầu ra
                                      OutputWeight = x.OutputWeight ?? 0,
                                      //Trọng lượng hàng hóa
                                      GoodsWeight = x.GoodsWeight ?? 0,
                                      //Confirm quantity
                                      ConfirmQty = x.ConfirmQty ?? 0,
                                      //SL bao
                                      BagQuantity = x.BagQuantity ?? 0,
                                      //Đơn trọng
                                      SingleWeight = x.SingleWeight ?? 0,
                                      //Material desc
                                      MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName,
                                      //Material
                                      Material = x.MaterialCodeInt.ToString(),
                                      //Số cân đầu vào
                                      InputWeight = x.InputWeight ?? 0,
                                      //Plant
                                      Plant = x.PlantCode,
                                      //Thời gian ghi nhận
                                      RecordTime = x.RecordTime2
                                  }).AsNoTracking();

            int filterResultsCount = 0;
            int totalResultsCount = 0;
            int totalPagesCount = 0;

            var response = new ListSavedDataTruckWeighResponse();


            //Lấy danh sách TestTarget có phân trang
            var res = CustomSearch.CustomSearchFunc<SavedDataTruckWeighReponse>(request.Paging, out filterResultsCount, out totalResultsCount, out totalPagesCount, query, "STT");

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

            response.SavedDataTruckWeighs = res;
            response.PagingRep.FilterResultsCount = filterResultsCount;
            response.PagingRep.TotalResultsCount = totalResultsCount;
            response.PagingRep.TotalPagesCount = totalPagesCount;

            return Task.FromResult(response);
        }
    }
}
