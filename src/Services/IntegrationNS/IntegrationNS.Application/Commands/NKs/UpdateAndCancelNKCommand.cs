﻿using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IntegrationNS.Application.Commands.NKs
{
    public class UpdateAndCancelNKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNK> NKs { get; set; } = new List<UpdateAndCancelNK>();
    }

    public class UpdateAndCancelNK
    {
        public Guid NkId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKCommandHandler : IRequestHandler<UpdateAndCancelNKCommand, bool>
    {
        private readonly IRepository<OtherImportModel> _nkRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;

        public UpdateAndCancelNKCommandHandler(IRepository<OtherImportModel> nkRepo, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo)
        {
            _nkRepo = nkRepo;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNKCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {

                if (!request.NKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NK");

                foreach (var item in request.NKs)
                {
                    //Phiếu nhập khác
                    var nk = await _nkRepo.FindOneAsync(x => x.OtherImportId == item.NkId);

                    //Check
                    if (nk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập khác");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nk.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nk.MaterialDocument))// && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                    //    nkpppp.Status = "NOT";
                    nk.LastEditTime = DateTime.Now;


                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nk);
                    var nkNew = JsonConvert.DeserializeObject<OtherImportModel>(serialized);

                    nkNew.OtherImportId = Guid.NewGuid();
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    nkNew.LastEditBy = null;
                    nkNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    nkNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    nkNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    nkNew.Status = "NOT";
                    nkNew.MaterialDocument = null;
                    nkNew.ReverseDocument = null;


                    _nkRepo.Add(nkNew);
                }
            }
            else
            {

                if (!request.NKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NK");

                foreach (var item in request.NKs)
                {
                    //Phiếu nhập khác
                    var nk = await _nkRepo.FindOneAsync(x => x.OtherImportId == item.NkId);

                    //Check
                    if (nk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập khác");

                    //Cập nhật Batch và MaterialDocument
                    nk.Batch = item.Batch;
                    nk.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nk.MaterialDocument))// && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                    //    nkpppp.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
