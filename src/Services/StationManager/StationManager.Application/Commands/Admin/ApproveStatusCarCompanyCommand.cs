using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using StationManager.Application.Services;

namespace StationManager.Application.Commands.Admin
{
    public class ApproveStatusCarCompanyCommand : IRequest<bool>
    {
        public Guid CarCompanyId { get; set; }
    }

    public class ApproveStatusCarCompanyCommandHandler : IRequestHandler<ApproveStatusCarCompanyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<AccountModel> _accRepo;
        private readonly IRepository<CarCompany_Package_MappingModel> _mappingRepo;
        private readonly IMailService _mailService;

        public ApproveStatusCarCompanyCommandHandler(IUnitOfWork unitOfWork, IRepository<CarCompanyModel> carCompanyRepo,
                                                     IRepository<AccountModel> accRepo, 
                                                     IRepository<CarCompany_Package_MappingModel> mappingRepo,
                                                     IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _carCompanyRepo = carCompanyRepo;
            _accRepo = accRepo;
            _mappingRepo = mappingRepo;
            _mailService = mailService;
        }

        public async Task<bool> Handle(ApproveStatusCarCompanyCommand request, CancellationToken cancellationToken)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.CarCompanyId == request.CarCompanyId);

            carCompany.Actived = true;
            carCompany.Status = "active";

            var account = await _accRepo.FindOneAsync(x => x.AccountId == carCompany.AccountId);

            account.Actived = true;

            var mapping = await _mappingRepo.FindOneAsync(x => x.CarCompanyId == carCompany.CarCompanyId);

            mapping.Actived = true;

            if (!string.IsNullOrEmpty(carCompany.Email))
            {
                var content = new MailContent
                {
                    To = carCompany.Email,
                    Subject = "Thông báo duyệt đơn đăng ký đối tác nhà xe thành công",
                    Body = "Đơn đăng ký đối tác nhà xe đã được duyệt thành công. " +
                    "\nDưới đây là thông tin tài khoản nhà của quý khách: " +
                    $"\nTên đăng nhập: {account.UserName} " +
                    $"\nMật khẩu: {account.UserName}" +
                    "\nCảm ơn đã sử dụng dịch vụ của chúng tôi!"
                };
                _mailService.SendMail(content);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
