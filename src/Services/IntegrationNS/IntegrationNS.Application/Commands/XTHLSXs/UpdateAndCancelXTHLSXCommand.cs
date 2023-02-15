using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.XTHLSXs
{
    public class UpdateAndCancelXTHLSXCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXTHLSX> XTHLSXs { get; set; } = new List<UpdateAndCancelXTHLSX>();
    }

    public class UpdateAndCancelXTHLSX
    {
        public Guid XthlsxId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXTHLSXCommandHandler : IRequestHandler<UpdateAndCancelXTHLSXCommand, bool>
    {
        private readonly IRepository<IssueForProductionModel> _xthlsxRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelXTHLSXCommandHandler(IRepository<IssueForProductionModel> xthlsxRepo, IUnitOfWork unitOfWork)
        {
            _xthlsxRepo = xthlsxRepo;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXTHLSXCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.XTHLSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                foreach (var item in request.XTHLSXs)
                {
                    //Phiếu nhập kho phụ phâm phế phẩm
                    var xthlsx = await _xthlsxRepo.FindOneAsync(x => x.IssForProductiontId == item.XthlsxId);

                    //Check
                    if (xthlsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xthlsx.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xthlsx.MaterialDocument) && string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "POST";
                    else if (!string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "NOT";
                    xthlsx.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    var xthlsxNew = new IssueForProductionModel
                    {
                        IssForProductiontId = Guid.NewGuid(),
                        PlantCode = xthlsx.PlantCode,
                        DetailWorkOrderId = xthlsx.DetailWorkOrderId,
                        WeightVote = xthlsx.WeightVote,
                        BagQuantity = xthlsx.BagQuantity,
                        SingleWeight = xthlsx.SingleWeight,
                        WeightHeadCode = xthlsx.WeightHeadCode,
                        Weight = xthlsx.Weight,
                        ConfirmQty = xthlsx.ConfirmQty,
                        QuantityWithPackaging = xthlsx.QuantityWithPackaging,
                        QuantityWeitght = xthlsx.QuantityWeitght,
                        SlocCode = xthlsx.SlocCode,
                        Image = xthlsx.Image,
                        Status = xthlsx.Status,
                        StartTime = xthlsx.StartTime,
                        EndTime = xthlsx.EndTime,
                        CreateTime = DateTime.Now,
                        Actived = true,
                        ComponentCode = xthlsx.ComponentCode,
                        ComponentCodeInt = xthlsx.ComponentCodeInt,
                        WeightId = xthlsx.WeightId,
                        SlocName = xthlsx.SlocName 
                    };

                    _xthlsxRepo.Add(xthlsxNew);
                }
            }
            else
            {

                if (!request.XTHLSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                foreach (var item in request.XTHLSXs)
                {
                    //Phiếu xuất tiêu hao lsx
                    var xthlsx = await _xthlsxRepo.FindOneAsync(x => x.IssForProductiontId == item.XthlsxId);

                    //Check
                    if (xthlsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất tiêu hao lệnh sản xuất");

                    //Cập nhật Batch và MaterialDocument
                    xthlsx.Batch = item.Batch;
                    xthlsx.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(xthlsx.MaterialDocument) && string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "POST";
                    else if (!string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
