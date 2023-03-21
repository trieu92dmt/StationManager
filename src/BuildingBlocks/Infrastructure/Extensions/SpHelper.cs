using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Identity.Permissions;
using System.Data;
using Directory = System.IO.Directory;

namespace Infrastructure.Extensions
{
    public static class SpHelper
    {
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
