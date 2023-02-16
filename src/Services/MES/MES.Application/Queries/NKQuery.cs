using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NK;
using MES.Application.DTOs.MES.NK;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKQuery
    {
        /// <summary>
        /// Lấy input data
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchNKCommand command);

        /// <summary>
        /// Lấy data nhập khác
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKResponse>> SearchNK(SearchNKCommand command);
    }

    public class NKQuery : INKQuery
    {
        private readonly IRepository<OrderImportModel> _nkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;

        public NKQuery(IRepository<OrderImportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo)
        {
            _nkRepo = nkRepo;
            _plantRepo = plantRepo;
            _prdRepo = prdRepo;
            _custRepo = custRepo;
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchNKCommand command)
        {
            //Kiểm tra nếu material không search to thì search 1
            if (string.IsNullOrEmpty(command.MaterialTo))
            {
                command.MaterialTo = command.MaterialFrom;
            }

            //Get plant
            var plant = await _plantRepo.FindOneAsync(x => x.PlantCode == command.Plant);

            //Get customer
            var customer = await _custRepo.FindOneAsync(x => !string.IsNullOrEmpty(command.Customer) ? x.CustomerNumber == command.Customer : false);

            //Get material
            var materials = await _prdRepo.GetQuery(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                                   x.ProductCodeInt <= long.Parse(command.MaterialTo)).Select(x => new GetInputDataResponse
                                                   {
                                                       Plant = plant.PlantCode,
                                                       Customer = customer != null ? customer.CustomerNumber : "",
                                                       CustomerName = customer != null ? customer.CustomerName : "",
                                                       Material = x.ProductCodeInt.ToString(),
                                                       MaterialDesc = x.ProductName,
                                                       Unit = x.Unit
                                                   }).AsNoTracking().ToListAsync();

            //Tạo key
            var index = 1;
            foreach (var item in materials)
            {
                item.IndexKey = index++; 
            }

            return materials;
        }

        public Task<List<SearchNKResponse>> SearchNK(SearchNKCommand command)
        {
            #region Format Day
            //Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Tạo query
            var query = _nkRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo Customer
            if (!string.IsNullOrEmpty(command.Customer))
            {
                query = query.Where(x => x.Customer == command.Customer);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(command.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == command.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                if (!command.WeightDateTo.HasValue) command.WeightDateTo = command.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= command.WeightDateFrom &&
                                         x.CreateTime <= command.WeightDateTo);
            }

            //Check số phiếu cân
            if (!command.WeightVotes.IsNullOrEmpty() || command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            throw new NotImplementedException();
        }
    }
}
 