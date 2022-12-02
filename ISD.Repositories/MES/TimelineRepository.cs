using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{
    public class TimelineRepository
    {
        EntityDataContext _context;
        public TimelineRepository(EntityDataContext context)
        {
            _context = context;
        }
        public List<TimeLineViewModel> Timeline(TimeLineSearchViewModel timeLineSearchView)
        {

            string sqlQuery = "EXEC [MES].[GanttChart] @StartFromDate,@StartToDate,@EndFromDate,@EndToDate,@StartDCFromDate,@StartDCToDate,@EndDCFromDate,@EndDCToDate,@Summary,@VBELN,@Material,@Summary_Dot, @isDeleted, @CompanyId";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                //Ngày bắt đầu
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StartFromDate",
                    Value = timeLineSearchView.StartFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StartToDate",
                    Value = timeLineSearchView.StartToDate ?? (object)DBNull.Value
                },//Ngày kết thúc
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndFromDate",
                    Value = timeLineSearchView.EndFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndToDate",
                    Value = timeLineSearchView.EndToDate ?? (object)DBNull.Value
                },//Ngày bắt đầu điều chỉnh
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StartDCFromDate",
                    Value = timeLineSearchView.StartDCFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StartDCToDate",
                    Value = timeLineSearchView.StartDCToDate ?? (object)DBNull.Value
                },//Ngày kết thúc điều chỉnh
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndDCFromDate",
                    Value = timeLineSearchView.EndDCFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EndDCToDate",
                    Value = timeLineSearchView.EndDCToDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Summary",
                    Value = timeLineSearchView.Summary ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "VBELN",
                    Value = timeLineSearchView.VBELN ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Material",
                    Value = timeLineSearchView.Material ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Summary_Dot",
                    Value = timeLineSearchView.Summary_Dot ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "isDeleted",
                    Value = timeLineSearchView.isDeleted ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CompanyId",
                    Value = timeLineSearchView.CompanyId,
                }
            };
            #endregion
            var result = _context.Database.SqlQuery<TimeLineViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return result;
        }

        public List<BomDetailViewModel> GetBomDetailWithLSXD(Guid Id)
        {
            object[] SqlParams =
            {
                new SqlParameter("@TaskId",Id),
            };
            var res = _context.Database.SqlQuery<BomDetailViewModel>("[MES].[GetBomDetailWithLSXD] @TaskId", SqlParams).ToList();
            return res;
        }
        // Load danh sách subtask 
        public List<TimeLineViewModel> LoadSubTaskInTimeline(Guid parentTaskId)
        {
            object[] SqlParams =
            {
                new SqlParameter("@TaskId", parentTaskId),
            };
            var res = _context.Database.SqlQuery<TimeLineViewModel>("[MES].[GanttChart_GetByLSXC] @TaskId", SqlParams).ToList();
            return res;
        }


    }
}
