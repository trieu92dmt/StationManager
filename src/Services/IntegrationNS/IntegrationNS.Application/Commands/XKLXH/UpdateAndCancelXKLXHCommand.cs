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

namespace IntegrationNS.Application.Commands.XKLXH
{
    public class UpdateAndCancelXKLXHCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXKLXH> XKLXHs { get; set; } = new List<UpdateAndCancelXKLXH>();
    }

    public class UpdateAndCancelXKLXH
    {
        public Guid XklxhId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXKLXHCommandHandler : IRequestHandler<UpdateAndCancelXKLXHCommand, bool>
    {
        private readonly IRepository<ExportByCommandModel> _xklxhRep;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelXKLXHCommandHandler(IRepository<ExportByCommandModel> xklxhRep, IUnitOfWork unitOfWork)
        {
            _xklxhRep = xklxhRep;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXKLXHCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.XKLXHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất kho theo lệnh xuất hàng");

                foreach (var item in request.XKLXHs)
                {
                    //Phiếu xuất kho theo lệnh xuất hàng
                    var xklxh = await _xklxhRep.FindOneAsync(x => x.ExportByCommandId == item.XklxhId);

                    //Check
                    if (xklxh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất kho theo lệnh xuất hàng");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xklxh.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xklxh.MaterialDocument))
                        xklxh.Status = "POST";
                    xklxh.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xklxh);
                    var xklxhNew = JsonConvert.DeserializeObject<ExportByCommandModel>(serialized);

                    xklxhNew.ExportByCommandId = Guid.NewGuid();
                    xklxhNew.Status = "NOT";
                    xklxhNew.MaterialDocument = null;
                    xklxhNew.ReverseDocument = null;

                    _xklxhRep.Add(xklxhNew);
                }
            }
            else
            {

                if (!request.XKLXHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XKLXH");

                foreach (var item in request.XKLXHs)
                {
                    //Phiếu xuất kho theo lệnh xuất hàng
                    var xklxh = await _xklxhRep.FindOneAsync(x => x.ExportByCommandId == item.XklxhId);

                    //Check
                    if (xklxh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất kho theo lệnh xuất hàng");

                    //Cập nhật Batch và MaterialDocument
                    xklxh.Batch = item.Batch;
                    xklxh.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(xklxh.MaterialDocument))
                        xklxh.Status = "POST";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
