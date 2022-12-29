﻿using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Commands.Role
{
    public class RoleCreateCommand : IRequest<bool>
    {
        //Mã nhóm
        public string RolesCode { get; set; }
        //Tên nhóm
        public string RolesName { get; set; }
        //Thứ tự
        public int OrderIndex { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }

    public class RoleCreateCommandHandler : IRequestHandler<RoleCreateCommand, bool>
    {
        private readonly IRepository<RolesModel> _roleRepo;
        private readonly IISDUnitOfWork _unitOfWork;

        public RoleCreateCommandHandler(IRepository<RolesModel> roleRepo, IISDUnitOfWork unitOfWork)
        {
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
        {
            //Kiểm tra trùng role code
            var role = await _roleRepo.FindOneAsync(x => x.RolesCode == request.RolesCode);

            if (role == null)
                throw new ISDException(CommonResource.Msg_Existed, "Mã nhóm");

            _roleRepo.Add(new RolesModel
            {
                RolesId = Guid.NewGuid(),
                RolesCode = request.RolesCode,
                RolesName = request.RolesName,
                OrderIndex = request.OrderIndex,
                Actived = request.Actived
            });

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
