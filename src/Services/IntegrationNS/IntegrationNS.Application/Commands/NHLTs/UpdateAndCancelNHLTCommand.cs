using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.NHLTs
{
    public class UpdateAndCancelNHLTCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNHLT> NHLTs { get; set; } = new List<UpdateAndCancelNHLT>();
    }

    public class UpdateAndCancelNHLT
    {
        public Guid NhltId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNHLTCommandHandler : IRequestHandler<UpdateAndCancelNHLTCommand, bool>
    {
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;

        public UpdateAndCancelNHLTCommandHandler(IRepository<GoodsReceiptTypeTModel> nhltRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _nhltRep = nhltRep;
            _unitOfWork = unitOfWork;
            _obDetailRepo = obDetailRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNHLTCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NHLTs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu nhập hàng loại T");

                foreach (var item in request.NHLTs)
                {
                    //Phiếu nhập kho dcnb
                    var nhlt = await _nhltRep.FindOneAsync(x => x.GoodsReceiptTypeTId == item.NhltId);

                    //Check
                    if (nhlt is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập hàng loại T");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nhlt.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nhlt.MaterialDocument))//) && string.IsNullOrEmpty(nhlt.ReverseDocument))
                        nhlt.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nhlt.ReverseDocument))
                    //    nhlt.Status = "NOT";
                    nhlt.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nhlt);
                    var nhltNew = JsonConvert.DeserializeObject<GoodsReceiptTypeTModel>(serialized);

                    nhltNew.GoodsReceiptTypeTId = Guid.NewGuid();
                    nhltNew.Status = "NOT";
                    nhltNew.MaterialDocument = null;
                    nhltNew.ReverseDocument = null;

                    _nhltRep.Add(nhltNew);
                }
            }
            else
            {

                if (!request.NHLTs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NHLT");

                foreach (var item in request.NHLTs)
                {
                    //Phiếu nhập hàng loại T
                    var nhlt = await _nhltRep.FindOneAsync(x => x.GoodsReceiptTypeTId == item.NhltId);

                    //Check
                    if (nhlt is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập hàng loại T");

                    //Cập nhật Batch và MaterialDocument
                    nhlt.Batch = item.Batch;
                    nhlt.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nhlt.MaterialDocument))// && string.IsNullOrEmpty(nhlt.ReverseDocument))
                        nhlt.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nhlt.ReverseDocument))
                    //    nhlt.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
