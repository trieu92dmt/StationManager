using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace MasterData.Applications.Commands.Role
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
        private readonly IUnitOfWork _unitOfWork;

        public RoleCreateCommandHandler(IRepository<RolesModel> roleRepo, IUnitOfWork unitOfWork)
        {
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
        {
            //Kiểm tra trùng role code
            var role = await _roleRepo.FindOneAsync(x => x.RolesCode == request.RolesCode);

            if (role != null)
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
