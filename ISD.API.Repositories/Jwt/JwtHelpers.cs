using ISD.API.EntityModels.Data;
using ISD.API.EntityModels.Models;
using ISD.API.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<UserTokens> GenUserTokens(AccountModel model, string SaleOrg, JwtSettings jwtSettings)
        {

            using (var _context = new EntityDataContext())
            {
                Guid Id = model.AccountId;
                DateTime expireTime = DateTime.Now;
                //Tạo Usertoken
                var UserToken = new UserTokens();
                UserToken.UserName = model.UserName;
                UserToken.AccountId = model.AccountId;
                UserToken.FullName = model.FullName;
                UserToken.EmployeeCode = model.EmployeeCode;

                #region Sale Org

                var saleOrg = await _context.SaleOrgModel.FirstOrDefaultAsync(x => x.SaleOrgCode == SaleOrg);
                UserToken.SaleOrgCode = SaleOrg;
                UserToken.SaleOrgName = saleOrg?.SaleOrgName;

                #endregion

                #region Role
                var role = (from p in _context.AccountModel
                            from r in p.Roles
                            where p.AccountId == UserToken.AccountId
                            select r.RolesCode).FirstOrDefault();
                UserToken.Role = role;
                var roleList = (from p in _context.AccountModel
                                from r in p.Roles
                                where p.AccountId == UserToken.AccountId
                                select r.RolesCode).ToList();
                var roleCodeJoin = string.Join(",", roleList.ToArray());
                UserToken.Roles = roleCodeJoin;
                #endregion

                #region Permission

                UserToken.Permission = GetMenuMobileList(model.AccountId);
                //Lấy token
                var jWtToken = GenJwtToken(UserToken, jwtSettings, out expireTime, out Id);
                UserToken.Validaty = expireTime.TimeOfDay;
                UserToken.ExpiredTime = expireTime;
                UserToken.Token = jWtToken;

                #endregion 

                return UserToken;
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

        #region GET permission mobile list
        /// <summary>
        /// GET permission mobile list
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public static PermissionMobileViewModel GetMenuMobileList(Guid AccountId)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json")
                               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var permission = new PermissionMobileViewModel();
            //using dataset to get multiple table in store procedure: page, menu, page permission
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "pms.QTHT_PagePermissionMobile_GetPagePermissionByAccountId";
                    cmd.Parameters.AddWithValue("@AccountId", AccountId);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    conn.Close();
                }
            }
            ds.Tables[0].TableName = "PageModel";
            ds.Tables[1].TableName = "MenuModel";
            ds.Tables[2].TableName = "PagePermissionModel";

            //Convert datatable into list
            var pageList = (from p in ds.Tables[0].AsEnumerable()
                            select new MobileScreenViewModel()
                            {
                                MobileScreenId = p.Field<Guid>("MobileScreenId"),
                                ScreenName = p.Field<string>("ScreenName"),
                                ScreenCode = p.Field<string>("ScreenCode"),
                                MenuId = p.Field<Guid>("MenuId"),
                                IconName = p.Field<string>("Icon"),
                                OrderIndex = p.Field<int>("OrderIndex"),
                            }).ToList();

            var menuList = (from p in ds.Tables[1].AsEnumerable()
                            select new MenuViewModel()
                            {
                                MenuId = p.Field<Guid>("MenuId"),
                                MenuName = p.Field<string>("MenuName"),
                                Icon = p.Field<string>("Icon"),
                                OrderIndex = p.Field<int>("OrderIndex"),
                            }).ToList();

            var funcList = (from p in ds.Tables[2].AsEnumerable()
                            select new MobileScreenPermissionViewModel()
                            {
                                RolesId = p.Field<Guid>("RolesId"),
                                MobileScreenId = p.Field<Guid>("MobileScreenId"),
                                FunctionId = p.Field<string>("FunctionId"),
                            }).ToList();


            //add list into model Permission
            permission.MobileScreenModel = pageList;
            permission.MenuModel = menuList;
            permission.MobileScreenPermissionModel = funcList;
            return permission;
        }
        #endregion

        #region Get web permission by accountId
        /// <summary>
        /// Get web permission by accountId
        /// </summary>
        /// <param name="context"></param>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public static PermissionViewModel GetWebPermissionByAccountId(EntityDataContext context, Guid AccountId)
        {
            var response = new PermissionViewModel();

            try
            {
                var sp = "pms.QTHT_PagePermission_GetPagePermissionByAccountId";

                DataSet ds = new DataSet();
                using (SqlDataAdapter sda = new SqlDataAdapter(sp, context.Database.GetConnectionString()))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;

                    sda.SelectCommand.Parameters.Add(AccountId);

                    sda.Fill(ds);

                    ds.Tables[0].TableName = "PageModel";
                    ds.Tables[1].TableName = "MenuModel";
                    ds.Tables[2].TableName = "PagePermissionModel";
                    ds.Tables[3].TableName = "ModuleModel";
                }

                context.Database.CloseConnection();

                var jsonDt = JsonConvert.SerializeObject(ds);
                response = JsonConvert.DeserializeObject<PermissionViewModel>(jsonDt);

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }

}
