using Core.Commons;
using Core.Extensions;
using Core.Identity;
using Core.Interfaces.Databases;
using Core.Properties;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Shared.Identity;
using Shared.Models;

namespace Authentication.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _service;
        private readonly ICommonService _commonService;
        private readonly EntityDataContext _context;

        public AuthController(ITokenService service, ICommonService commonService, EntityDataContext context)
        {
            _service = service;
            _commonService = commonService;
            _context = context;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            //Get token
            var response = await _service.GetToken(request);

            return Ok(new ApiSuccessResponse<TokenResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Đăng nhập")
            });
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            //Check email
            var emailCheck = await _context.UserModel.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (emailCheck != null) 
            {
                return Ok(new ApiSuccessResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Email đã được đăng ký"
                });
            }

            //Check user name
            var usernameCheck = await _context.AccountModel.FirstOrDefaultAsync(x => x.UserName == request.Username);
            if (usernameCheck != null)
            {
                return Ok(new ApiSuccessResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Username đã được đăng ký"
                });
            }

            //Get list role
            var roles = await _context.RolesModel.ToListAsync();

            //Create new user
            var user = new UserModel
            {
                UserId = Guid.NewGuid(),
                Fullname = request.Fullname,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                CreateTime = DateTime.Now,
                Actived = true
            };

            //Create new account
            var acccount = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                FullName = request.Fullname,
                UserName = request.Username,
                Password = _commonService.GetMd5Sum(request.Password),
                Roles = roles.Where(x => x.RolesCode == "NormalUser").ToList(),
                Actived = true
            };

            user.Account = acccount;

            _context.UserModel.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = true,
                Message = string.Format(CommonResource.Msg_Success, "Đăng ký thành công")
            });
        }

        /// <summary>
        /// Đăng nhập bằng FB
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("loginFB")]
        public async Task<IActionResult> LoginWithFB([FromBody] TokenRequest request)
        {
            //Get token
            var response = await _service.GetToken(request);

            return Ok(new ApiSuccessResponse<TokenResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Đăng nhập")
            });
        }
    }
}
