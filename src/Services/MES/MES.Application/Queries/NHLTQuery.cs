using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NHLT;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NHLT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MES.Application.Queries
{
    public interface INHLTQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy dữ liệu đầu vào
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<GetInputDataResponse>> GetInputDatas(SearchNHLTCommand command);
    }

    public class NHLTQuery : INHLTQuery
    {
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _dtOdRepo;
        private readonly IRepository<CustmdSaleModel> _cusRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public NHLTQuery(IRepository<GoodsReceiptTypeTModel> nhltRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prodRepo,
                         IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<CustmdSaleModel> cusRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _nhltRepo = nhltRepo;
            _plantRepo = plantRepo;
            _prodRepo = prodRepo;
            _dtOdRepo = dtOdRepo;
            _cusRepo = cusRepo;
            _slocRepo = slocRepo;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nhltRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.WeightVote,
                                            Value = x.WeightVote
                                        }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetInputDataResponse>> GetInputDatas(SearchNHLTCommand command)
        {
            #region Format Day

            if (command.DocumentDateFrom.HasValue)
            {
                command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
            }
            if (command.DocumentDateTo.HasValue)
            {
                command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion


            //Get query plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Get query material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query detail od
            var detailOds = _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();

            //Get query customer
            var customers = _cusRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get data theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                plants = plants.Where(x => x.PlantCode == command.Plant);
            }    

            //Get data theo material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                {
                    command.MaterialTo = command.MaterialFrom;
                }

                materials = materials.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                                 x.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Get data theo Outbound delivery
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryFrom))
                {
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;
                }

                detailOds = detailOds.Where(x => x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >=0 &&
                                               x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0);
            }

            //Get data theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                detailOds = detailOds.Where(x => x.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                 x.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
            }

            var query = await (from p in plants
                        join m in materials on p.PlantCode equals m.PlantCode into mtr
                        from mtrs in mtr.DefaultIfEmpty()
                        join d in detailOds on p.PlantCode equals d.Plant into dtOd
                        from dtOds in dtOd.DefaultIfEmpty()
                        join c in customers on p.SaleOrgCode equals c.SaleOrgCode into saleOrg
                        from sales in saleOrg.DefaultIfEmpty()
                        select new GetInputDataResponse
                        {
                            //Plant
                            Plant = p.PlantCode,
                            //Customer
                            Customer = sales.CustomerNumber,
                            //Customer name
                            CustomerName = sales.CustomerName,
                            //Material
                            Material = mtrs.PlantCode,
                            //Material desc
                            MaterialDesc = mtrs.ProductName,
                            //UoM
                            Unit = mtrs.Unit,
                            //Outbound Delivery
                            OutboundDelivery = dtOds.OutboundDelivery.DeliveryCode,
                            //Outbound Delivery Item
                            OutboundDeliveryItem = dtOds.OutboundDeliveryItem,
                            //Batch
                            Batch = dtOds.Batch ?? "",
                            //Số phương tiện
                            VehicleCode = dtOds.OutboundDelivery.VehicleCode ?? "",
                            //Sloc
                            Sloc = dtOds.StorageLocation ?? "",
                            SlocName = string.IsNullOrEmpty(dtOds.StorageLocation) ? slocs.FirstOrDefault(s => s.StorageLocationCode == dtOds.StorageLocation).StorageLocationName : "",
                            //Document Date
                            DocumentDate = dtOds.OutboundDelivery.DocumentDate
                        }).AsNoTracking().ToListAsync();


            return query;
        }
    }
}
