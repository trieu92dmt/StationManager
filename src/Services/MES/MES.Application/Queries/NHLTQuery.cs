using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NHLT;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NHLT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        /// <summary>
        /// Lấy dữ liệu đã lưu nhlt
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNHLTResponse>> GetDataNHLT(SearchNHLTCommand command);  
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

        public Task<List<SearchNHLTResponse>> GetDataNHLT(SearchNHLTCommand command)
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
            //Ngày cân
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
            var query = _nhltRepo.GetQuery()
                                 .Include(x => x.DetailOD).ThenInclude(x => x.DetailOutboundDeliveryId)
                                 .AsNoTracking();

            //Get data theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Get data theo material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                {
                    command.MaterialTo = command.MaterialFrom;
                }

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Get data theo customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.CustomerTo))
                {
                    command.CustomerTo = command.CustomerFrom;
                }

                query = query.Where(x => x.Customer.CompareTo(command.CustomerFrom) >= 0 &&
                                                 x.Customer.CompareTo(command.CustomerTo) <= 0);
            }

            //Get data theo Outbound delivery
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                {
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;
                }

                query = query.Where(x => x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                               x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0);
            }

            //Get data theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                 x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
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
            if (command.WeightVotes != null && command.WeightVotes.Any())
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
            var materials = _prodRepo.GetQuery(x => string.IsNullOrEmpty(command.MaterialFrom) ? false : true).AsNoTracking();

            //Get query detail od
            var detailOds = _dtOdRepo.GetQuery(x => string.IsNullOrEmpty(command.OutboundDeliveryFrom) ? false : true).Include(x => x.OutboundDelivery).AsNoTracking();

            //Get query customer
            var customers = _cusRepo.GetQuery(x => string.IsNullOrEmpty(command.CustomerFrom) ? false : true).AsNoTracking();

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

            //Get data theo customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.CustomerTo))
                {
                    command.CustomerTo = command.CustomerFrom;
                }

                customers = customers.Where(x => x.CustomerNumber.CompareTo(command.CustomerFrom) >= 0 &&
                                                 x.CustomerNumber.CompareTo(command.CustomerTo) <= 0);
            }

            //Get data theo Outbound delivery
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
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
                            Customer = sales != null ? sales.CustomerNumber : "",
                            //Customer name
                            CustomerName = sales != null ? sales.CustomerName : "",
                            //Material
                            Material = mtrs != null ? mtrs.PlantCode : "",
                            //Material desc
                            MaterialDesc = mtrs != null ? mtrs.ProductName : "",
                            //UoM
                            Unit = mtrs != null ? mtrs.Unit : "",
                            //Outbound Delivery
                            OutboundDelivery = dtOds != null ? dtOds.OutboundDelivery.DeliveryCode : "",
                            //Outbound Delivery Item
                            OutboundDeliveryItem = dtOds != null ? dtOds.OutboundDeliveryItem : "",
                            //Batch
                            Batch = dtOds != null ? dtOds.Batch : "",
                            //Số phương tiện
                            VehicleCode = dtOds != null ? dtOds.OutboundDelivery.VehicleCode : "",
                            //Sloc
                            Sloc = dtOds != null ? dtOds.StorageLocation : "",
                            SlocName = !string.IsNullOrEmpty(dtOds.StorageLocation) ? slocs.FirstOrDefault(s => s.StorageLocationCode == dtOds.StorageLocation).StorageLocationName : "",
                            //Document Date
                            DocumentDate = dtOds != null ? dtOds.OutboundDelivery.DocumentDate : null
                        }).AsNoTracking().ToListAsync();


            return query;
        }
    }
}
