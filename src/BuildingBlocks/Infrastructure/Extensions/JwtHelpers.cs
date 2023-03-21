using Core.Jwt;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.Identity;
using Shared.Identity.Permissions;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Directory = System.IO.Directory;

namespace Infrastructure.Extensions
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this TokenResponse account, Guid Id)
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
        public static IEnumerable<Claim> GetClaims(this TokenResponse userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
        public static async Task<TokenResponse> GenUserTokens(AccountModel model, string plantCode, JwtSettings jwtSettings)
        {

            using (var _context = new EntityDataContext())
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
                var plant = await _context.PlantModel.FirstOrDefaultAsync(x => x.PlantCode == plantCode);
                token.PlantCode = plant?.PlantCode;
                token.PlantName = plant != null ? $"{plant.PlantCode} | {plant.PlantName}" : "";

                //Sale Org
                var saleOrg = await _context.SaleOrgModel.FirstOrDefaultAsync(x => x.SaleOrgCode == plant.SaleOrgCode);
                token.SaleOrgCode = saleOrg.SaleOrgCode;
                token.SaleOrgName = saleOrg?.SaleOrgName;


                #region Role

                var role = (from p in _context.AccountModel
                            from r in p.Roles
                            where p.AccountId == token.AccountId
                            select r.RolesCode).FirstOrDefault();
                token.Role = role;


                var roleList = (from p in _context.AccountModel
                                from r in p.Roles
                                where p.AccountId == token.AccountId
                                select r.RolesCode).ToList();
                var roleCodeJoin = string.Join(",", roleList.ToArray());
                token.Roles = roleCodeJoin;
                #endregion

                #region Permission

                // lấy Web permisstion module -> menu -> page -> page permission
                var sqlQuery = "pms.QTHT_PagePermission_GetPagePermissionByAccountId";
                var webPermissionDs = SqlProcHelper.GetWebPermissionByAccountId(_context, sqlQuery, new SqlParameter("@AccountId", model.AccountId));


                //Convert Dataset -> Json->Object
                var jsonDt = JsonConvert.SerializeObject(webPermissionDs);
                token.WebPermission = JsonConvert.DeserializeObject<PermissionWebResponse>(jsonDt);
                if (token.WebPermission != null && token.WebPermission.PageModel != null)
                {
                    token.WebPermission.PageModel = token.WebPermission.PageModel.DistinctBy(x => x.PageId).ToList();
                }

                token.Permission = GetMenuMobileList(model.AccountId);
                //Lấy token
                var jWtToken = GenJwtToken(token, jwtSettings, out expireTime, out Id);
                token.Validaty = expireTime.TimeOfDay;
                token.ExpiredTime = expireTime;
                token.Token = jWtToken;

                #endregion 

                return token;
            }
        }

        public static string GenJwtToken(TokenResponse user, JwtSettings jwtSettings, out DateTime expireTime, out Guid GuidId)
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

        #region GET permission mobile list
        /// <summary>
        /// GET permission mobile list
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public static PermissionMobileResponse GetMenuMobileList(Guid AccountId)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json")
                               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var permission = new PermissionMobileResponse();
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
                            select new MobileScreenResponse
                            {
                                MobileScreenId = p.Field<Guid>("MobileScreenId"),
                                ScreenName = p.Field<string>("ScreenName"),
                                ScreenCode = p.Field<string>("ScreenCode"),
                                MenuId = p.Field<Guid>("MenuId"),
                                IconName = p.Field<string>("Icon"),
                                OrderIndex = p.Field<int>("OrderIndex"),
                            }).ToList();

            var menuList = (from p in ds.Tables[1].AsEnumerable()
                            select new MenuResponse()
                            {
                                MenuId = p.Field<Guid>("MenuId"),
                                MenuName = p.Field<string>("MenuName"),
                                Icon = p.Field<string>("Icon"),
                                OrderIndex = p.Field<int>("OrderIndex"),
                            }).ToList();

            var funcList = (from p in ds.Tables[2].AsEnumerable()
                            select new MobileScreenPermissionResponse()
                            {
                                RolesId = p.Field<Guid>("RolesId"),
                                MobileScreenId = p.Field<Guid>("MobileScreenId"),
                                FunctionId = p.Field<string>("FunctionId"),
                            }).ToList();


            //add list into model Permission
            permission.MobileScreens = pageList;
            permission.Menus = menuList;
            permission.MobileScreenPermissions = funcList;
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
        public static PermissionWebResponse GetWebPermissionByAccountId(EntityDataContext context, Guid AccountId)
        {
            var response = new PermissionWebResponse();

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
                response = JsonConvert.DeserializeObject<PermissionWebResponse>(jsonDt);

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
