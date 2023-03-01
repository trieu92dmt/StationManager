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

namespace IntegrationNS.Application.Commands.XNVLGCs
{
    public class UpdateAndCancelXNVLGCCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXNVLGC> XNVLGCs { get; set; } = new List<UpdateAndCancelXNVLGC>();
    }

    public class UpdateAndCancelXNVLGC
    {
        public Guid XnvlgcId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXNVLGCCommandHandler : IRequestHandler<UpdateAndCancelXNVLGCCommand, bool>
    {
        private readonly IRepository<ComponentExportModel> _xnvlgcRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;

        public UpdateAndCancelXNVLGCCommandHandler(IRepository<ComponentExportModel> xnvlgcRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _xnvlgcRep = xnvlgcRep;
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
        public async Task<bool> Handle(UpdateAndCancelXNVLGCCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.XNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất nguyên vật liệu gia công");

                foreach (var item in request.XNVLGCs)
                {
                    //Phiếu nhập kho dcnb
                    var xnvlgc = await _xnvlgcRep.FindOneAsync(x => x.ComponentExportId == item.XnvlgcId);

                    //Check
                    if (xnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xnvlgc.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xnvlgc.MaterialDocument))//) && string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                        xnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                    //    xnvlgc.Status = "NOT";
                    xnvlgc.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xnvlgc);
                    var xnvlgcNew = JsonConvert.DeserializeObject<ComponentExportModel>(serialized);

                    xnvlgcNew.ComponentExportId = Guid.NewGuid();
                    xnvlgcNew.Status = "NOT";
                    xnvlgcNew.MaterialDocument = null;
                    xnvlgcNew.ReverseDocument = null;

                    _xnvlgcRep.Add(xnvlgcNew);
                }
            }
            else
            {

                if (!request.XNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XNVLGC");

                foreach (var item in request.XNVLGCs)
                {
                    //Phiếu xuất nguyên vật liệu gia công
                    var xnvlgc = await _xnvlgcRep.FindOneAsync(x => x.ComponentExportId == item.XnvlgcId);

                    //Check
                    if (xnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument
                    xnvlgc.Batch = item.Batch;
                    xnvlgc.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(xnvlgc.MaterialDocument))// && string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                        xnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                    //    xnvlgc.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
