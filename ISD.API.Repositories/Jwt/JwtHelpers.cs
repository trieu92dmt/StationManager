using ISD.API.EntityModels.Data;
using ISD.API.EntityModels.Models;
using ISD.API.Extensions;
using ISD.API.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ISD.API.Repositories.Jwt
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this UserTokens account, Guid Id)
        {
            IEnumerable<Claim> claims = new Claim[] {
                new Claim("Id", account.AccountId.ToString()),
                new Claim("Permission", JsonConvert.SerializeObject(account.Permission)),
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
                new Claim(ClaimTypes.Sid, account.AccountId.ToString()),
                new Claim(ClaimTypes.Spn, account.EmployeeCode??""),
                //fullname
                new Claim(ClaimTypes.Upn, account.FullName??""),
                //Role
                new Claim(ClaimTypes.Role, account.Role??""),
                new Claim(ClaimTypes.HomePhone, account.Roles??""),
                new Claim("CompanyId", account.CompanyId.ToString()),
                //Settings
                //new Claim(ClaimTypes.Webpage, (account.isShowChoseModule == true).ToString()),
                //new Claim(ClaimTypes.WindowsAccountName, (account.isShowDashboard == true).ToString()),
                //Quyền xem dữ liệu
                //new Claim(ClaimTypes.UserData, (account.isViewByStore == true).ToString()),
                //Quyền truy cập chức năng khóa sổ
                //new Claim(ClaimTypes.X500DistinguishedName, (isHasPermissionDateClosed == true).ToString()),
            };
            return claims;
        }
        /// <summary>
        /// Here GetClaims() Method is used to create return claims list from user token details.
        /// </summary>
        /// <param name="userAccounts"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
        public static UserTokens GenUserTokens(AccountModel model,JwtSettings jwtSettings)
        {
            try
            {
                if (model == null) throw new ArgumentException(nameof(model));
                using (var _context = new EntityDataContext())
                {
                    Guid Id = Guid.Empty;
                    DateTime expireTime = DateTime.Now;
                    //Tạo Usertoken
                    var UserToken = new UserTokens();
                    UserToken.UserName = model.UserName;
                    UserToken.AccountId = model.AccountId;
                    UserToken.FullName = model.FullName;
                    UserToken.EmployeeCode = model.EmployeeCode;

                    //UserToken.EmailId = model.emp;
                    #region Role
                    var role = (from p in _context.AccountModel
                               from r in p.Roles
                               where p.AccountId == UserToken.AccountId select r).FirstOrDefault();

                    UserToken.Role = role.RolesCode;
                    UserToken.RoleName = role.RolesName;

                    var roleList = (from p in _context.AccountModel
                                    from r in p.Roles
                                    where p.AccountId == UserToken.AccountId
                                    select r.RolesCode).ToList();
                    var roleCodeJoin = string.Join(",", roleList.ToArray());
                    UserToken.Roles = roleCodeJoin;
                    #endregion

                    // lấy Web permisstion module -> menu -> page -> page permission
                    var sqlQuery = "pms.QTHT_PagePermission_GetPagePermissionByAccountId";
                    var webPermissionDs = SqlProcHelper.GetWebPermissionByAccountId(_context, sqlQuery, new SqlParameter("@AccountId", UserToken.AccountId));

                    // Convert Dataset -> Json -> Object
                    var jsonDt = JsonConvert.SerializeObject(webPermissionDs);
                    UserToken.WebPermission = JsonConvert.DeserializeObject<PermissionViewModel>(jsonDt);
                    if (UserToken.WebPermission != null && UserToken.WebPermission.PageModel != null)
                    {
                        UserToken.WebPermission.PageModel = UserToken.WebPermission.PageModel.DistinctBy(x => x.PageId).ToList();
                    }
                    //UserToken.Permission = new AccountRepository(_context).GetMenuMobileList(model.AccountId);
                    //Lấy token
                    var jWtToken = GenJwtToken(UserToken, jwtSettings, out expireTime, out Id);
                    UserToken.Validaty = expireTime.TimeOfDay;
                    UserToken.ExpiredTime = expireTime;
                    UserToken.Token = jWtToken;
                     
                    return UserToken;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GenJwtToken(UserTokens user, JwtSettings jwtSettings, out DateTime expireTime, out Guid GuidId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Guid Id = Guid.Empty;
            expireTime = DateTime.Now.AddDays(1);
            var key = Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
            // Tao Token
            var JWToken = new JwtSecurityToken(issuer: jwtSettings.ValidIssuer, audience: jwtSettings.ValidAudience, claims: GetClaims(user, out Id), notBefore: new DateTimeOffset(DateTime.Now).DateTime, expires: new DateTimeOffset(expireTime).DateTime, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            GuidId = Id;
            var token = tokenHandler.WriteToken(JWToken);
            return token;
        }

        public static RefreshTokenViewModel GenRefreshToken(string UserName)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshTokenViewModel
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.Now.AddMonths(1),
                    Created = DateTime.Now,
                    CreatedByUserName = UserName
                };
            }
        }    
    }

}
