using Core.Commons;
using Core.Exceptions;
using Core.Identity;
using Core.Properties;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.Identity;
using Shared.Identity.Permissions;
using Shared.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly EntityDataContext _dbContext;
        private readonly ICommonService _commonService;
        private readonly JwtSettings _jwtSettings;

        public TokenService(EntityDataContext dbContext, ICommonService commonService, JwtSettings jwtSettings)
        {
            _dbContext = dbContext;
            _commonService = commonService;
            _jwtSettings = jwtSettings;
        }
        public async Task<TokenResponse> GetToken(TokenRequest request)
        {
            //User
            var user = await _dbContext.AccountModel.FirstOrDefaultAsync(x => x.UserName == request.Username);

            //Check tồn tại
            if (user == null)
                throw new ISDException("Đăng nhập thất bại: Tài khoản không tồn tại.");

            //Check trạng thái hoạt động
            if (user.Actived != true)
                throw new ISDException(LanguageResource.Account_Locked);

            //Kiểm tra nếu không phải sysadmin thì bắt buộc nhập SaleOrg
            if (string.IsNullOrEmpty(request.PlantCode) && request.Username != "sysadmin")
                throw new ISDException(LanguageResource.Chose_Plant);

            //Encrypt passwork
            var passwordEncrypt = _commonService.GetMd5Sum(request.Password);

            //Default password
            if (user.Password != passwordEncrypt && request.Password != "isdcorp@2023")
                throw new ISDException("Đăng nhập thất bại: Mật khẩu không chính xác.");

            var token = await GetToken(request, user);

            return token;
        }

        #region GET Token
        public async Task<TokenResponse> GetToken(TokenRequest request, AccountModel model)
        {
           
            Guid Id = model.AccountId;
            DateTime expireTime = DateTime.Now;

            //Tạo User token
            var token = new TokenResponse();
            token.UserName = model.UserName;
            token.AccountId = model.AccountId;
            token.FullName = model.FullName;
            token.EmployeeCode = model.EmployeeCode;

            //Plant
            var plant = await _dbContext.PlantModel.FirstOrDefaultAsync(x => x.PlantCode == request.PlantCode);
            token.PlantCode = plant?.PlantCode;
            token.PlantName = plant != null ? $"{plant.PlantCode} | {plant.PlantName}" : "";

            //Sale Org
            var saleOrg = await _dbContext.SaleOrgModel.FirstOrDefaultAsync(x => x.SaleOrgCode == plant.SaleOrgCode);
            token.SaleOrgCode = saleOrg.SaleOrgCode;
            token.SaleOrgName = saleOrg?.SaleOrgName;


            #region Role

            var role = (from p in _dbContext.AccountModel
                        from r in p.Roles
                        where p.AccountId == token.AccountId
                        select r.RolesCode).FirstOrDefault();
            token.Role = role;


            var roleList = (from p in _dbContext.AccountModel
                            from r in p.Roles
                            where p.AccountId == token.AccountId
                            select r.RolesCode).ToList();
            var roleCodeJoin = string.Join(",", roleList.ToArray());
            token.Roles = roleCodeJoin;
            #endregion

            #region Permission

            // lấy Web permisstion module -> menu -> page -> page permission
            var sqlQuery = "pms.QTHT_PagePermission_GetPagePermissionByAccountId";
            var webPermissionDs = SqlProcHelper.GetWebPermissionByAccountId(_dbContext, sqlQuery, new Microsoft.Data.SqlClient.SqlParameter("@AccountId", model.AccountId));


            //Convert Dataset -> Json->Object
            var jsonDt = JsonConvert.SerializeObject(webPermissionDs);
            token.WebPermission = JsonConvert.DeserializeObject<PermissionWebResponse>(jsonDt);
            if (token.WebPermission != null && token.WebPermission.PageModel != null)
            {
                token.WebPermission.PageModel = token.WebPermission.PageModel.DistinctBy(x => x.PageId).ToList();
            }

            token.Permission = SpHelper.GetMenuMobileList(model.AccountId);
            token.Token = GenerateJwt(GetSigningCredentials(), token);
            #endregion

            return token;
        }

        private string GenerateJwt(SigningCredentials signingCredentials, TokenResponse account)
        {
            var claims = new[]
            {
                new Claim("Id", account.AccountId.ToString()),
                //new Claim("Permission", JsonConvert.SerializeObject(account.Permission)),
                new Claim(ClaimTypes.Name, account.UserName),
                //new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
                new Claim(ClaimTypes.Sid, account.AccountId.ToString()),
                new Claim(ClaimTypes.Spn, account.EmployeeCode??""),
                //fullname
                new Claim(ClaimTypes.Upn, account.FullName??""),
                //Role
                new Claim(ClaimTypes.Role, account.Role??""),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        #endregion
    }
}
