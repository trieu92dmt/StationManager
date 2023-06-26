using Core.Commons;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Twilio;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Api.V2010.Account;
using System.Text.RegularExpressions;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class CarCompanyRegisterCommand : IRequest<bool>
    {
        public string CompanyName { get; set; }
        public string OfficeAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string Description { get; set; }
        public string PackageCode { get; set; }
    }

    public class CarCompanyRegisterCommandHandler : IRequestHandler<CarCompanyRegisterCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CarCompany_Package_MappingModel> _mappingRepo;
        private readonly ICommonService _commonService;
        private readonly IRepository<PackageModel> _packageRepo;
        private readonly IRepository<AccountModel> _accRepo;
        private readonly IRepository<RolesModel> _rolesRepo;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public CarCompanyRegisterCommandHandler(IUnitOfWork unitOfWork, IRepository<CarCompanyModel> carCompanyRepo,
                                                IRepository<CarCompany_Package_MappingModel> mappingRepo, ICommonService commonService,
                                                IRepository<PackageModel> packageRepo, IRepository<AccountModel> accRepo,
                                                IRepository<RolesModel> rolesRepo, IMailService mailService, 
                                                IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _carCompanyRepo = carCompanyRepo;
            _mappingRepo = mappingRepo;
            _commonService = commonService;
            _packageRepo = packageRepo;
            _accRepo = accRepo;
            _rolesRepo = rolesRepo;
            _mailService = mailService;
            _config = config;
        }

        public async Task<bool> Handle(CarCompanyRegisterCommand request, CancellationToken cancellationToken)
        {
            string result = request.CompanyName.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");

            //Create Username
            var username = $"{result.Trim().Replace(" ","")}{DateTime.Now.ToString("ddMMyyyyHHmmss")}";

            //Get Role
            var roles = await _rolesRepo.GetQuery().ToListAsync();

            //Create Account
            var newAccount = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                FullName = request.CompanyName,
                UserName = username,
                Password = _commonService.GetMd5Sum(username),
                Roles = roles.Where(x => x.RolesCode == "CarCompany").ToList(),
                Actived = true
            };

            //Create Car company
            var newCarCompany = new CarCompanyModel
            {
                CarCompanyId = Guid.NewGuid(),
                AccountId = newAccount.AccountId,
                CarCompanyName = request.CompanyName,
                Email = request.Email,
                Hotline = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber2,
                OfficeAddress = request.OfficeAddress,
                Description = request.Description,
                CreateTime = DateTime.Now,
                Status = "unactive",
                Actived = false
            };

            //Get Package
            var package = await _packageRepo.FindOneAsync(x => x.PackageCode == request.PackageCode);

            //Create new Package Company Mapping
            var newMapping = new CarCompany_Package_MappingModel
            {
                CarCompany_Package_MappingId = Guid.NewGuid(),
                CarCompanyId = newCarCompany.CarCompanyId,
                PackageId = package.PackageId,
                CreatedTime = DateTime.Now,
                Actived = false,
                ExpireTime = DateTime.Now.AddMonths(package.Duration ?? 0),
            };

            if (!string.IsNullOrEmpty(request.Email))
            {
                var content = new MailContent
                {
                    To = request.Email,
                    Subject = "Thông báo đăng ký đối tác nhà xe thành công",
                    Body = "Quý khách vừa đăng ký trở thành đối tác nhà xe thành công, vui lòng chờ hệ thống xét duyệt trong vòng 1-2 ngày. \nCảm ơn đã sử dụng dịch vụ của chúng tôi!"
                };
                _mailService.SendMail(content);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber2))
            {
                var body = "Quý khách vừa đăng ký trở thành đối tác nhà xe thành công, vui lòng chờ hệ thống xét duyệt trong vòng 1-2 ngày. \nCảm ơn đã sử dụng dịch vụ của chúng tôi!";
                var to = "+84948513923";
                //_smsService.SendSMS(body, to);

                //Get Account
                var account = _config.GetSection("Twilio")["Account"];
                var authTokent = _config.GetSection("Twilio")["AuthToken"];
                var phoneNumber = _config.GetSection("Twilio")["PhoneNumber"];

                TwilioClient.Init(account, authTokent);

                var message = MessageResource.Create(
                    body: body,
                    from: new Twilio.Types.PhoneNumber(phoneNumber),
                    to: new Twilio.Types.PhoneNumber(to)
                );
            }

            _accRepo.Add(newAccount);
            _carCompanyRepo.Add(newCarCompany);
            _mappingRepo.Add(newMapping);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
