using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class BC11ReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public BC11ReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo danh sách khách hàng
        /// </summary>
        /// <returns>ProfileReportResultViewModel</returns>
        public List<BC11ReportViewModel> GetData(BC11ReportViewModel searchViewModel)
        {
         

            string sqlQuery = "[Report].[usp_BC11Report] @SaleOrgCode, @DepartmentCode, @WorkCenterCode, @WorkShopId, @FromTime, @ToTime";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOrgCode",
                    Value = searchViewModel.SaleOrgCode ?? (object)DBNull.Value,
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DepartmentCode",
                    Value = searchViewModel.DepartmentCode ?? (object)DBNull.Value,
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkCenterCode",
                    Value = searchViewModel.WorkCenterCode ?? (object)DBNull.Value,
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WorkShopId",
                    Value = searchViewModel.WorkShopId ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FromTime",
                    Value = searchViewModel.FromTime ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ToTime",
                    Value = searchViewModel.ToTime ?? (object)DBNull.Value,
                },
            };
            #endregion

            List<BC11ReportViewModel> result = _context.Database.SqlQuery<BC11ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }
    }
}
