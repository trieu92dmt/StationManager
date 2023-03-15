using Core.Models;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.Scale;
using MES.Application.DTOs.MES.XKLXH;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XKLXH
{
    public class SearchTruckWeighRecordCommand : IRequest<GetListTruckWeighInfoResponse>
    {
        public DatatableViewModel Paging { get; set; }
        //plant
        public string Plant { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
    }

    public class SearchTruckWeighRecordCommandHandler : IRequestHandler<SearchTruckWeighRecordCommand, GetListTruckWeighInfoResponse>
    {
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<ProductModel> _prodRepo;

        public SearchTruckWeighRecordCommandHandler(IRepository<ExportByCommandModel> xklxhRepo, IRepository<ProductModel> prodRepo)
        {
            _xklxhRepo = xklxhRepo;
            _prodRepo = prodRepo;
        }

        public Task<GetListTruckWeighInfoResponse> Handle(SearchTruckWeighRecordCommand request, CancellationToken cancellationToken)
        {
            //Get query material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Tạo query truy vấn
            var query = _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                  .Where(x => (!string.IsNullOrEmpty(request.Plant) ? x.PlantCode == request.Plant : true) &&
                                              (!string.IsNullOrEmpty(request.OutboundDelivery) ? x.DetailOD.OutboundDelivery.DeliveryCode == request.OutboundDelivery : true) &&
                                              x.TruckInfoId != null && x.RecordTime2 == null)
                                  .Select(x => new GetTruckWeighInfoResponse
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
                                      Plant = x.PlantCode
                                  }).AsNoTracking();

            //Tổng field lọc được
            int filterResultsCount = 0;
            //Tổng số kết quả
            int totalResultsCount = 0;
            //Tổng số trang
            int totalPagesCount = 0;

            var response = new GetListTruckWeighInfoResponse();


            //Lấy danh sách TestTarget có phân trang
            var res = CustomSearch.CustomSearchFunc<GetTruckWeighInfoResponse>(request.Paging, out filterResultsCount, out totalResultsCount, out totalPagesCount, query, "STT");

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

            response.TruckWeightInfos = res;
            //Tổng field lọc được
            response.PagingRep.FilterResultsCount = filterResultsCount;
            //Tổng số kết quả
            response.PagingRep.TotalResultsCount = totalResultsCount;
            //Tổng số trang
            response.PagingRep.TotalPagesCount = totalPagesCount;

            return Task.FromResult(response);
        }
    }
}
