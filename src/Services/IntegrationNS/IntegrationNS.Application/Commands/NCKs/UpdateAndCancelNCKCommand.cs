using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<AccountModel> _userRepo;

        public UpdateAndCancelNCKCommandHandler(IRepository<WarehouseImportTransferModel> nckRep, IUnitOfWork unitOfWork, IRepository<MaterialDocumentModel> matDocRepo, 
                                                IRepository<AccountModel> userRepo            )
        {
            _nckRep = nckRep;
            _unitOfWork = unitOfWork;
            _matDocRepo = matDocRepo;
            _userRepo = userRepo;   
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
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            //Tạo query material doc với MVT = 313
            var matDoc313Query = _matDocRepo.GetQuery(x => x.MovementType == "313").AsNoTracking();

            //Tạo query material doc với MVT = 315
            var matDoc315Query = _matDocRepo.GetQuery(x => x.MovementType == "315").AsNoTracking();

            if (request.IsCancel == true)
            {
                //Get query chứng từ
                var documentQuery = _matDocRepo.GetQuery().AsNoTracking();

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

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.MaterialDocId == nck.MaterialDocId);

                    nckNew.WarehouseImportTransferId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    nckNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    nckNew.LastEditBy = null;
                    nckNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    nckNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    nckNew.CreateTime = DateTime.Now;
                    //_______//
                    nckNew.TotalQuantity = 0;
                    //nckNew.DeliveryQuantity = 0;
                    //nckNew.OpenQuantity = 0;
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
                    var nck = await _nckRep.GetQuery().Include(x => x.MaterialDoc).FirstOrDefaultAsync(x => x.WarehouseImportTransferId == item.NckId);

                    //Check
                    if (nck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập chuyển kho");

                    //Cập nhật Batch và MaterialDocument
                    nck.Batch = item.Batch;
                    nck.MaterialDocument = item.MaterialDocument;
                    nck.TotalQuantity = nck.MaterialDocId.HasValue ? matDoc313Query.Where(x => x.MaterialDocCode == nck.MaterialDoc.MaterialDocCode && 
                                                                                               x.ItemAutoCreated == "X").Sum(x => x.Quantity) : 0;  
                    //nck.DeliveryQuantity = nck.MaterialDocId.HasValue ? 
                    //                       matDoc313Query.Where(x => x.MaterialDocCode == nck.MaterialDoc.MaterialDocCode).Sum(x => x.Quantity)
                    //                       - matDoc315Query.Where(x => x.MaterialDocCode == nck.MaterialDoc.MaterialDocCode).Sum(x => x.Quantity) : 0;
                    //nck.OpenQuantity = nck.TotalQuantity - nck.DeliveryQuantity;
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
