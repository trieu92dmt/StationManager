using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Data;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.WeighSession;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MES.Application.Commands.WeighSession
{
    public class SearchWeighSessionCommand : IRequest<PagingResultSP<WeighSessionResponse>>
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        //Plant
        public string Plant { get; set; }
        //Đầu cân
        public string ScaleCode { get; set; }
        //Ngày thực hiện
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        //Id đợt cân
        public string WeighSessionId { get; set; }
    }

    public class SearchWeighSessionCommandHandler : IRequestHandler<SearchWeighSessionCommand, PagingResultSP<WeighSessionResponse>>
    {
        private readonly EntityDataContext _context;
        private readonly IRepository<AccountModel> _userRepo;

        public SearchWeighSessionCommandHandler(EntityDataContext context, IRepository<AccountModel> userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }

        public async Task<PagingResultSP<WeighSessionResponse>> Handle(SearchWeighSessionCommand request, CancellationToken cancellationToken)
        {
            //User query
            var userQuery = _userRepo.GetQuery().AsNoTracking();

            //Không search ngày thì lấy 30 ngày từ ngày hiện tại
            #region Format Day
            if (!request.DateFrom.HasValue && !request.DateTo.HasValue)
            {
                request.DateFrom = DateTime.Now.Date.AddDays(-30);
                request.DateTo = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }
            else if (request.DateFrom.HasValue && !request.DateTo.HasValue)
            {
                request.DateFrom = request.DateFrom.Value.Date;
                request.DateTo = request.DateFrom.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            try
            {
                var response = new List<WeighSessionResponse>(); 
                string storeProc = "upc_datacollection_UNION_Transaction";

                DataSet ds = new DataSet();
                using (SqlDataAdapter sda = new SqlDataAdapter(storeProc, _context.Database.GetConnectionString()))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Plant", request.Plant ?? ""));
                    sda.SelectCommand.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ScaleCode", request.ScaleCode ?? ""));
                    sda.SelectCommand.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FromDate", request.DateFrom));
                    sda.SelectCommand.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@ToDate", request.DateTo));
                    sda.SelectCommand.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@WeighSessionId", request.WeighSessionId ?? ""));

                    sda.Fill(ds);

                    ds.Tables[0].TableName = "Result";
                }

                _context.Database.CloseConnection();

                //Duyệt kết quả procedure
                for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    response.Add(new WeighSessionResponse
                    {
                        //Id đợt cân
                        WeighSessionId = ds.Tables[0].Rows[i][0].ToString(),
                        //Số lần cân
                        TotalNumberOfWeigh = int.Parse(ds.Tables[0].Rows[i][1].ToString()),
                        //Số phiếu cân
                        WeightVote = ds.Tables[0].Rows[i][3].ToString(),
                        //Mã đầu cân
                        ScaleCode = ds.Tables[0].Rows[i][4].ToString(),
                        //Tên đầu cân
                        ScaleName = ds.Tables[0].Rows[i][5].ToString(),
                        //DateKey
                        DateKey = ds.Tables[0].Rows[i][6].ToString(),
                        //STT đợt cân
                        OrderIndex = ds.Tables[0].Rows[i][2] != DBNull.Value ? int.Parse(ds.Tables[0].Rows[i][2].ToString()) : 0,
                        //Thời gian bắt đầu
                        StartTime = ds.Tables[0].Rows[i][7] != DBNull.Value ? (DateTime)ds.Tables[0].Rows[i][7] : null,
                        //Thời gian kết thúc
                        EndTime = ds.Tables[0].Rows[i][8] != DBNull.Value ? (DateTime)ds.Tables[0].Rows[i][8] : null,
                        //Trọng lượng cân
                        TotalWeight = ds.Tables[0].Rows[i][9] != DBNull.Value ? 
                                      ds.Tables[0].Rows[i][17] != DBNull.Value ? 
                                      decimal.Parse(ds.Tables[0].Rows[i][9].ToString())/decimal.Parse(ds.Tables[0].Rows[i][17].ToString()) : 
                                      decimal.Parse(ds.Tables[0].Rows[i][9].ToString())
                                      : 0,
                        //Confirm quantity
                        ConfirmQuantity = ds.Tables[0].Rows[i][10] != DBNull.Value ? decimal.Parse(ds.Tables[0].Rows[i][10].ToString()) : 0,
                        //Ghi chú
                        Description = ds.Tables[0].Rows[i][11].ToString(),
                        //Hình ảnh
                        //Người tạo
                        CreateById = ds.Tables[0].Rows[i][13] != DBNull.Value ? (Guid)ds.Tables[0].Rows[i][13] : null,
                        CreateBy = ds.Tables[0].Rows[i][14] != DBNull.Value ? ds.Tables[0].Rows[i][14].ToString() : "",
                        //Đánh dấu xóa
                        Status = ds.Tables[0].Rows[i][15] != DBNull.Value ? ds.Tables[0].Rows[i][15].ToString() : "",
                        //Nghiệp vụ
                        TransactionType = ds.Tables[0].Rows[i][16] != DBNull.Value ? ds.Tables[0].Rows[i][16].ToString() : "",
                    });
                }

                #region Phân trang
                var totalRecords = response.Count();

                //Sorting
                var dataSorting = PagingSorting.Sorting(request.Paging, response.AsQueryable());
                //Phân trang
                var responsePaginated = PaginatedList<WeighSessionResponse>.Create(dataSorting, request.Paging.Offset, request.Paging.PageSize);
                var res = new PagingResultSP<WeighSessionResponse>(responsePaginated, totalRecords, request.Paging.PageIndex, request.Paging.PageSize);

                //Đánh số thứ tự
                if (res.Data.Any())
                {
                    int i = request.Paging.Offset;
                    foreach (var item in res.Data)
                    {
                        i++;
                        item.STT = i;
                    }
                }
                #endregion


                return await Task.FromResult(res);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
