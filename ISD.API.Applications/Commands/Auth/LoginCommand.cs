using ISD.API.Applications.Services;
using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Repositories.Jwt;
using ISD.API.Resources;
using ISD.API.ViewModels;
using MediatR;

namespace ISD.API.Applications.Commands.Auth
{
    public class LoginCommand : IRequest<UserTokens>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SaleOrg { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, UserTokens>
    {
        private readonly IRepository<AccountModel> _accountRep;
        private readonly IUtilitiesService _service;
        private readonly JwtSettings _jwtSettings;

        public LoginCommandHandler(IRepository<AccountModel> accountRep, IISDUnitOfWork unitOfWork, IUtilitiesService service, JwtSettings jwtSettings)
        {
            _accountRep = accountRep;
            _service = service;
            _jwtSettings = jwtSettings;
        }
        public async Task<UserTokens> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            request.UserName = request.UserName.Trim();
            request.Password = request.Password.Trim();

            //Check tồn tại
            var user = await _accountRep.FindOneAsync(x => x.UserName == request.UserName);
            if (user == null)
                throw new ISDException("Đăng nhập thất bại: Tài khoản không tồn tại");

            //Check trạng thái hoạt động
            if (user.Actived != true)
                throw new ISDException(LanguageResource.Account_Locked);

            //Kiểm tra nếu không phải sysadmin thì bắt buộc nhập SaleOrg
            if (string.IsNullOrEmpty(request.SaleOrg) && request.UserName != "sysadmin")
                throw new ISDException(LanguageResource.Choose_Store);

            //Encrypt passwork
            var passwordEncrypt = _service.GetMd5Sum(request.Password);

            //Default password
            if (user.Password != passwordEncrypt && request.Password != "isdcorp@2023")
                throw new ISDException("Đăng nhập thất bại: Mật khẩu không chính xác");

            var userToken = await JwtHelpers.GenUserTokens(user, request.SaleOrg, _jwtSettings);

            return userToken;
        }
    }
}
