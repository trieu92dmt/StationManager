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

namespace IntegrationNS.Application.Commands.XKs
{
    public class UpdateAndCancelXKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXK> XKs { get; set; } = new List<UpdateAndCancelXK>();
    }

    public class UpdateAndCancelXK
    {
        public Guid XkId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXKCommandHandler : IRequestHandler<UpdateAndCancelXKCommand, bool>
    {
        private readonly IRepository<OtherExportModel> _xkRep;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelXKCommandHandler(IRepository<OtherExportModel> xkRep, IUnitOfWork unitOfWork)
        {
            _xkRep = xkRep;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXKCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.XKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất khác");

                foreach (var item in request.XKs)
                {
                    //Phiếu xuất khác
                    var xk = await _xkRep.FindOneAsync(x => x.OtherExportId == item.XkId);

                    //Check
                    if (xk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất khác");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xk.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xk.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                    xk.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xk);
                    var xkNew = JsonConvert.DeserializeObject<OtherExportModel>(serialized);

                    xkNew.OtherExportId = Guid.NewGuid();
                    xkNew.Status = "NOT";
                    xkNew.MaterialDocument = null;
                    xkNew.ReverseDocument = null;

                    _xkRep.Add(xkNew);
                }
            }
            else
            {

                if (!request.XKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XK");

                foreach (var item in request.XKs)
                {
                    //Phiếu xuất khác
                    var xk = await _xkRep.FindOneAsync(x => x.OtherExportId == item.XkId);

                    //Check
                    if (xk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất khác");

                    //Cập nhật Batch và MaterialDocument
                    xk.Batch = item.Batch;
                    xk.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(xk.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
