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

namespace IntegrationNS.Application.Commands.NCKs
{
    public class UpdateAndCancelNCKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNCK> NCKs { get; set; } = new List<UpdateAndCancelNCK>();
    }

    public class UpdateAndCancelNCK
    {
        public Guid NckId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNCKCommandHandler : IRequestHandler<UpdateAndCancelNCKCommand, bool>
    {
        private readonly IRepository<WarehouseImportTransferModel> _nckRep;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelNCKCommandHandler(IRepository<WarehouseImportTransferModel> nckRep, IUnitOfWork unitOfWork)
        {
            _nckRep = nckRep;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNCKCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu nhập chuyển kho");

                foreach (var item in request.NCKs)
                {
                    //Phiếu xuất chuyển kho
                    var nck = await _nckRep.FindOneAsync(x => x.WarehouseImportTransferId == item.NckId);

                    //Check
                    if (nck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập chuyển kho");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nck.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nck.MaterialDocument))// && string.IsNullOrEmpty(nck.ReverseDocument))
                        nck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nck.ReverseDocument))
                    //    nck.Status = "NOT";
                    nck.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nck);
                    var nckNew = JsonConvert.DeserializeObject<WarehouseImportTransferModel>(serialized);

                    nckNew.WarehouseImportTransferId = Guid.NewGuid();
                    nckNew.TotalQuantity = null;
                    nckNew.DeliveryQuantity = null;
                    nckNew.OpenQuantity = null;
                    nckNew.Status = "NOT";
                    nckNew.MaterialDocument = null;
                    nckNew.ReverseDocument = null;

                    _nckRep.Add(nckNew);
                }
            }
            else
            {

                if (!request.NCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu nck");

                foreach (var item in request.NCKs)
                {
                    //Phiếu nhập chuyển kho
                    var nck = await _nckRep.FindOneAsync(x => x.WarehouseImportTransferId == item.NckId);

                    //Check
                    if (nck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập chuyển kho");

                    //Cập nhật Batch và MaterialDocument
                    nck.Batch = item.Batch;
                    nck.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nck.MaterialDocument))// && string.IsNullOrEmpty(nck.ReverseDocument))
                        nck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nck.ReverseDocument))
                    //    nck.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
