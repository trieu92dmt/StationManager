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
    public class ProductionCompletedStagesReportRepository
    {
        EntityDataContext _context;
        RepositoryLibrary _repositoryLibrary;
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public ProductionCompletedStagesReportRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
            _repositoryLibrary = new RepositoryLibrary();
        }

        /// <summary>
        /// Báo cáo danh sách khách hàng
        /// </summary>
        /// <returns>ProfileReportResultViewModel</returns>
        public List<ProductionCompletedStagesReportViewModel> GetData(ProductionCompletedStagesReportViewModel searchViewModel)
        {

          
            List<ProductionCompletedStagesReportViewModel> result = new List<ProductionCompletedStagesReportViewModel>();

            string sqlQuery = "[Report].[ProductionCompletedStagesReport] @Type, @CompletedFromDate, @CompletedToDate";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Type",
                    Value = searchViewModel.Type ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedFromDate",
                    Value = searchViewModel.CompletedFromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompletedToDate",
                    Value = searchViewModel.CompletedToDate ?? (object)DBNull.Value,
                },
            };
            #endregion

            result = _context.Database.SqlQuery<ProductionCompletedStagesReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return result;
        }
    }
}
