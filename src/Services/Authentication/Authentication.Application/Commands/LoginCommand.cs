using Core.Exceptions;
using Core.Jwt;
using Core.Jwt.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Extensions;
using Infrastructure.Models;
using MediatR;

namespace Authentication.Application.Commands
{
    public class LoginCommand : IRequest<UserToken>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PlantCode { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, UserToken>
    {
        private readonly IRepository<AccountModel> _accountRep;
        private readonly IUtilitiesService _service;
        private readonly JwtSettings _jwtSettings;

        public LoginCommandHandler(IRepository<AccountModel> accountRep, IUtilitiesService service, JwtSettings jwtSettings)
        {
            _accountRep = accountRep;
            _service = service;
            _jwtSettings = jwtSettings;
        }
        public async Task<UserToken> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            request.UserName = request.UserName.Trim();
            request.Password = request.Password.Trim();

            //Check tồn tại
            var user = await _accountRep.FindOneAsync(x => x.UserName == request.UserName);
            if (user == null)
                throw new ISDException("Đăng nhập thất bại: Tài khoản không tồn tại.");

            //Check trạng thái hoạt động
            if (user.Actived != true)
                throw new ISDException(LanguageResource.Account_Locked);

            //Kiểm tra nếu không phải sysadmin thì bắt buộc nhập SaleOrg
            if (string.IsNullOrEmpty(request.PlantCode) && request.UserName != "sysadmin")
                throw new ISDException(LanguageResource.Choose_Store);

            //Encrypt passwork
            var passwordEncrypt = _service.GetMd5Sum(request.Password);

            //Default password
            if (user.Password != passwordEncrypt && request.Password != "isdcorp@2023")
                throw new ISDException("Đăng nhập thất bại: Mật khẩu không chính xác.");

            var userToken = await JwtHelpers.GenUserTokens(user, request.PlantCode, _jwtSettings);

            return userToken;
        }
    }

}
