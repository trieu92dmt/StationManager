using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.SqlServer.Server;

namespace ISD.Repositories.MES
{
    public class LookUpProductionStageRepository
    {
        EntityDataContext _context;
        public LookUpProductionStageRepository(EntityDataContext context)
        {
            _context = context;
        }

        //Tìm kiếm nhân viên
        public List<ProductRoutingMappingViewModel> SearchRouting(List<string> ProductCodes)
        {
            //Build your record
            var tableProductCodeSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableProductCode = new List<SqlDataRecord>();
            List<string> productCodeLst = new List<string>();
            if (ProductCodes != null && ProductCodes.Count > 0)
            {
                foreach (var r in ProductCodes)
                {
                    var tableRow = new SqlDataRecord(tableProductCodeSchema);
                    tableRow.SetString(0, r);
                    if (!productCodeLst.Contains(r))
                    {
                        productCodeLst.Add(r);
                        tableProductCode.Add(tableRow);
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableProductCodeSchema);
                tableProductCode.Add(tableRow);
            }
            string sqlQuery = "EXEC [MES].[usp_SearchRouting] @ProductCode";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableProductCode
                },
                
            };
            #endregion

            var result = _context.Database.SqlQuery<ProductRoutingMappingViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return result;
        }
    }
}
