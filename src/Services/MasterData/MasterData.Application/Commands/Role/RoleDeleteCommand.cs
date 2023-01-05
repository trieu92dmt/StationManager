using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace MasterData.Applications.Commands.Role
{
    public class RoleDeleteCommand : IRequest<bool>
    {
        public Guid RoleId { get; set; }
    }

    public class RoleDeleteCommandHandler : IRequestHandler<RoleDeleteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RolesModel> _roleRepo;

        public RoleDeleteCommandHandler(IUnitOfWork unitOfWork, IRepository<RolesModel> roleRepo)
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
