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
    public class RoleDeleteCommand : IRequest<bool>
    {
        public Guid RoleId { get; set; }
    }

    public class RoleDeleteCommandHandler : IRequestHandler<RoleDeleteCommand, bool>
    {
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<RolesModel> _roleRepo;

        public RoleDeleteCommandHandler(IISDUnitOfWork unitOfWork, IRepository<RolesModel> roleRepo)
        {
            _unitOfWork = unitOfWork;
            _roleRepo = roleRepo;
        }

        public async Task<bool> Handle(RoleDeleteCommand request, CancellationToken cancellationToken)
        {
            //Kiểm tra tồn tại nhóm người dùng
            var role = await _roleRepo.FindOneAsync(x => x.RolesId == request.RoleId);

            if (role == null)
                throw new ISDException(CommonResource.Msg_NotFound, "Nhóm người dùng");

            _roleRepo.Remove(role);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
