using ISD.API.EntityModels.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace ISD.API.Extensions
{
    public static class SqlProcHelper
    {
        public static List<T> RawSqlQuery<T>(EntityDataContext context, string query)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var lstResultEntities = new List<T>();

                    var dataTable = new DataTable();
                    dataTable.Load(result);

                    if (dataTable.Rows.Count > 0)
                    {
                        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                        lstResultEntities = (List<T>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<T>));
                    }

                    context.Database.CloseConnection();

                    return lstResultEntities;
                }
            }
        }

        public static List<T> RawSqlQuery<T>(EntityDataContext context, string query, SqlParameter[] parameters)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                command.Parameters.AddRange(parameters);

                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var lstResultEntities = new List<T>();

                    var dataTable = new DataTable();
                    dataTable.Load(result);

                    if (dataTable.Rows.Count > 0)
                    {
                        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                        lstResultEntities = (List<T>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<T>));
                    }

                    context.Database.CloseConnection();

                    return lstResultEntities;
                }
            }
        }
        public static List<T> RawSqlQuery<T>(EntityDataContext context, string query, List<SqlParameter> parameters)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                command.Parameters.AddRange(parameters.ToArray());

                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var lstResultEntities = new List<T>();

                    var dataTable = new DataTable();
                    dataTable.Load(result);

                    if (dataTable.Rows.Count > 0)
                    {
                        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                        lstResultEntities = (List<T>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<T>));
                    }

                    context.Database.CloseConnection();

                    return lstResultEntities;
                }
            }
        }

        public static List<T> RawSqlQuery<T>(EntityDataContext context, string query, SqlParameter sqlParameter)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                command.Parameters.Add(sqlParameter);

                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var lstResultEntities = new List<T>();

                    var dataTable = new DataTable();
                    dataTable.Load(result);

                    if (dataTable.Rows.Count > 0)
                    {
                        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                        lstResultEntities = (List<T>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<T>));
                    }

                    context.Database.CloseConnection();

                    return lstResultEntities;
                }
            }
        }

        // Dùng riêng cho page permission
        public static DataSet GetWebPermissionByAccountId(EntityDataContext context, string storeProc, SqlParameter sqlParameter)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlDataAdapter sda = new SqlDataAdapter(storeProc, context.Database.GetConnectionString()))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.Add(sqlParameter);

                    sda.Fill(ds);

                    ds.Tables[0].TableName = "PageModel";
                    ds.Tables[1].TableName = "MenuModel";
                    ds.Tables[2].TableName = "PagePermissionModel";
                    ds.Tables[3].TableName = "ModuleModel";
                }

                context.Database.CloseConnection();
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
