using ISD.API.Applications.Services;
using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;

namespace ISD.API.Applications.Commands.Auth
{
    public class LoginCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, bool>
    {
        private readonly IGeneRepo<AccountModel> _accountRep;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IUtilitiesService _service;

        public LoginCommandHandler(IGeneRepo<AccountModel> accountRep, IISDUnitOfWork unitOfWork, IUtilitiesService service)
        {
            _accountRep = accountRep;
            _unitOfWork = unitOfWork;
            _service = service;
        }
        public async Task<bool> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            request.UserName = request.UserName.Trim();
            request.Password = request.Password.Trim();

            //Check tồn tại
            var user = await _accountRep.FindOneAsync(x => x.UserName == request.UserName && x.Actived == true);
            if(user == null)
                throw new ISDException(LanguageResource.Account_Locked);

            var passwordEncrypt = _service.GetMd5Sum(request.Password);

            throw new NotImplementedException();
        }
    }
}
