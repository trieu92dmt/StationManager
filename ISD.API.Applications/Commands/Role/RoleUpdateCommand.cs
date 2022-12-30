using ISD.API.Core.Exceptions;
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
    public class RoleUpdateCommand : IRequest<bool>
    {
        //Id
        public Guid Id { get; set; }
        //Tên nhóm
        public string RoleName { get; set; }
        //Thứ tự
        public int OrderIndex { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }

    public class RoleUpdateCommandHandler : IRequestHandler<RoleUpdateCommand, bool>
    {
        private readonly IRepository<RolesModel> _roleRepo;
        private readonly IISDUnitOfWork _unitOfWork;

        public RoleUpdateCommandHandler(IRepository<RolesModel> roleRepo, IISDUnitOfWork unitOfWork)
        {
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RoleUpdateCommand request, CancellationToken cancellationToken)
        {
            //Kiểm tra tồn tại của nhóm người dùng
            var role = await _roleRepo.FindOneAsync(x => x.RolesId == request.Id);

            if (role == null)
                throw new ISDException(CommonResource.Msg_NotFound, "Nhóm người dùng");

            //Thực hiện cập nhật
            //Tên nhóm
            role.RolesName = request.RoleName;
            //Thứ tự
            role.OrderIndex = request.OrderIndex;
            //Trạng thái
            role.Actived = request.Actived;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
