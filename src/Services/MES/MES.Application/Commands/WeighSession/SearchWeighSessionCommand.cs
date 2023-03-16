using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.WeighSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.WeighSession
{
    public class SearchWeighSessionCommand : IRequest<List<WeighSessionResponse>>
    {
        //Đầu cân
        public string ScaleCode { get; set; }
        //Ngày thực hiện
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        //Id đợt cân
        public string WeighSessionId { get; set; }
    }

    public class SearchWeighSessionCommandHandler : IRequestHandler<SearchWeighSessionCommand, List<WeighSessionResponse>>
    {
        //Nhập kho mua hàng
        private readonly IRepository<GoodsReceiptModel> _nkmhRepo;
        //Nhập kho hàng trả
        //Xuất kho theo lệnh xuất hàng
        //Nhập kho thành phẩm sản xuất
        //Nhập kho phụ phẩm phế phẩm
        //Xuất tiêu hao theo lệnh sản xuất
        //Xuất nguyên vật liệu gia công
        //Nhập nguyên vật liệu gia công thừa
        //Nhập hàng loại T
        //Nhập chuyển kho
        //Xuất chuyển kho
        //Nhập kho điều chuyển nội bộ
        //Nhập khác
        //Xuất khác
        public SearchWeighSessionCommandHandler()
        {
        }

        public Task<List<WeighSessionResponse>> Handle(SearchWeighSessionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
