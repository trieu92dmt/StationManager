﻿using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;

namespace IntegrationNS.Application.Commands.NNVLGCs
{
    public class UpdateAndCancelNNVLGCCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNNVLGC> NNVLGCs { get; set; } = new List<UpdateAndCancelNNVLGC>();
    }

    public class UpdateAndCancelNNVLGC
    {
        public Guid NnvlgcId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNNVLGCCommandHandler : IRequestHandler<UpdateAndCancelNNVLGCCommand, bool>
    {
        private readonly IRepository<ComponentImportModel> _nnvlgcRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;

        public UpdateAndCancelNNVLGCCommandHandler(IRepository<ComponentImportModel> nnvlgcRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _nnvlgcRep = nnvlgcRep;
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
        public async Task<bool> Handle(UpdateAndCancelNNVLGCCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu nhập nguyên vật liệu gia công");

                foreach (var item in request.NNVLGCs)
                {
                    //Phiếu nhập kho dcnb
                    var nnvlgc = await _nnvlgcRep.FindOneAsync(x => x.ComponentImportId == item.NnvlgcId);

                    //Check
                    if (nnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nnvlgc.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nnvlgc.MaterialDocument))//) && string.IsNullOrEmpty(nnvlgc.ReverseDocument))
                        nnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nnvlgc.ReverseDocument))
                    //    nnvlgc.Status = "NOT";
                    nnvlgc.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nnvlgc);
                    var nnvlgcNew = JsonConvert.DeserializeObject<ComponentImportModel>(serialized);

                    nnvlgcNew.ComponentImportId = Guid.NewGuid();
                    nnvlgcNew.Status = "NOT";
                    nnvlgcNew.MaterialDocument = null;
                    nnvlgcNew.ReverseDocument = null;

                    _nnvlgcRep.Add(nnvlgcNew);
                }
            }
            else
            {

                if (!request.NNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NNVLGC");

                foreach (var item in request.NNVLGCs)
                {
                    //Phiếu nhập nguyên vật liệu gia công
                    var nnvlgc = await _nnvlgcRep.FindOneAsync(x => x.ComponentImportId == item.NnvlgcId);

                    //Check
                    if (nnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument
                    nnvlgc.Batch = item.Batch;
                    nnvlgc.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nnvlgc.MaterialDocument))// && string.IsNullOrEmpty(nnvlgc.ReverseDocument))
                        nnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nnvlgc.ReverseDocument))
                    //    nnvlgc.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}