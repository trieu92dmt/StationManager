using Core.Extensions;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NHLT;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NHLT;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        Task<PagingResultSP<GetInputDataResponse>> GetInputDatas(SearchNHLTCommand command);

        /// <summary>
        /// Lấy dữ liệu đã lưu nhlt
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<PagingResultSP<SearchNHLTResponse>> GetDataNHLT(SearchNHLTCommand command);

        /// <summary>
        /// Lấy data theo od và od item
        /// </summary>
        /// <param name="od">Outbound Delivery</param>
        /// <param name="odItem">Outbound Delivery Item</param>
        /// <returns></returns>
        Task<GetDataByOdAndOdItemResponse> GetDataByOdAndOdItem(string od, string odItem);
    }

    public class NHLTQuery : INHLTQuery
    {
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _dtOdRepo;
        private readonly IRepository<CustmdSaleModel> _cusRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;

        public NHLTQuery(IRepository<GoodsReceiptTypeTModel> nhltRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prodRepo,
                         IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<CustmdSaleModel> cusRepo, IRepository<StorageLocationModel> slocRepo,
                         IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo, IRepository<ScaleModel> scaleRepo)
        {
            _nhltRepo = nhltRepo;
            _plantRepo = plantRepo;
            _prodRepo = prodRepo;
            _dtOdRepo = dtOdRepo;
            _cusRepo = cusRepo;
            _slocRepo = slocRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
            _scaleRepo = scaleRepo;
        }

        public async Task<GetDataByOdAndOdItemResponse> GetDataByOdAndOdItem(string od, string odItem)
        {
            //Lấy ra od
            var odDetails = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                              .FirstOrDefaultAsync(x => x.OutboundDeliveryItem == odItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(od));

            if (odDetails == null)
                return null;

            //Danh sách product
            var prods = _prodRepo.GetQuery().AsNoTracking();

            var response = new GetDataByOdAndOdItemResponse
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductCodeInt.ToString(),
                //Material Desc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductName,
                //Batch
                Batch = odDetails.Batch ?? "",
                //Số phương tiện
                VehicleCode = odDetails.OutboundDelivery.VehicleCode ?? "",
                //DocumentDate
                DocumentDate = odDetails.OutboundDelivery.DocumentDate
            };

            return response;
        }

        public async Task<PagingResultSP<SearchNHLTResponse>> GetDataNHLT(SearchNHLTCommand command)
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

            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();

            //Tạo query
            var query = _nhltRepo.GetQuery()
                                 .Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                 .AsNoTracking();

            //Get query customer
            var customers = _cusRepo.GetQuery().AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

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

            //Scale
            var scale = _scaleRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchNHLTResponse
            {
                //Id
                NHLTId = x.GoodsReceiptTypeTId,
                //plant
                Plant = x.PlantCode,
                //material
                Material = x.MaterialCodeInt.ToString(),
                //material desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? prods.FirstOrDefault(p => p.ProductCodeInt == x.MaterialCodeInt).ProductName : "",
                //customer
                Customer = x.Customer ?? "",
                CustomerName = !string.IsNullOrEmpty(x.Customer) ? customers.FirstOrDefault(c => c.CustomerNumber == x.Customer).CustomerName : "",
                //Sloc
                Sloc = x.SlocCode ?? "",
                //Batch
                SlocName = x.SlocName ?? "",
                //Sl bao
                BagQuantity = x.BagQuantity ?? 0 ,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                ScaleType = !string.IsNullOrEmpty(x.WeightHeadCode) ? scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).isCantai == true ? "CANXETAI" :
                                                                      scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).ScaleType == true ? "TICHHOP" : "KHONGTICHHOP" : "KHONGTICHHOP",
                //Số batch
                Batch = x.Batch ?? "",
                //Trọng lượng
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0 ,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Unit
                Unit = x.UOM ?? "",
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"{new ConfigManager().DomainUploadUrl}{x.Image}" : "",
                //Status
                Status = !string.IsNullOrEmpty(x.Status) ? status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi : "",
                //Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //Cân xe đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Cân xe đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //od
                OutboundDelivery = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode : "",
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                //31 Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //32 Crete On
                CreateOn = x.CreateTime ?? null,
                //33 Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                ChangeOn = x.LastEditTime,
                //34 Material Doc
                MatDoc = x.MaterialDocument ?? null,
                //Documentdate
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //35 Reverse Doc
                RevDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
            });

            #region Phân trang
            //Số dòng dữ liệu
            var totalRecords = data.Count();

            //Sorting
            var dataSorting = PagingSorting.Sorting(command.Paging, data.AsQueryable());
            //Phân trang
            var responsePaginated = PaginatedList<SearchNHLTResponse>.Create(dataSorting, command.Paging.Offset, command.Paging.PageSize);
            var res = new PagingResultSP<SearchNHLTResponse>(responsePaginated, totalRecords, command.Paging.PageIndex, command.Paging.PageSize);

            //Đánh số thứ tự
            if (res.Data.Any())
            {
                int i = command.Paging.Offset;
                foreach (var item in res.Data)
                {
                    i++;
                    item.STT = i;
                }
            }
            #endregion

            return await Task.FromResult(res);
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

        public async Task<PagingResultSP<GetInputDataResponse>> GetInputDatas(SearchNHLTCommand command)
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

            var deliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9" };

            //Get query plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Get query material
            var materialQuery = _prodRepo.GetQuery().AsNoTracking();
            //var materials = _prodRepo.GetQuery(x => string.IsNullOrEmpty(command.MaterialFrom) ? false : true).AsNoTracking();

            //Get query detail od
            var detailOds = _dtOdRepo.GetQuery(x => string.IsNullOrEmpty(command.OutboundDeliveryFrom) ? false : true)
                                     .Include(x => x.OutboundDelivery)
                                     .Where(x => deliveryType.Contains(x.OutboundDelivery.DeliveryType) &&
                                                 x.OutboundDelivery.PODStatus == "A" &&
                                                 x.Plant == command.Plant).AsNoTracking();

            //Get query customer
            //var customers = _cusRepo.GetQuery(x => string.IsNullOrEmpty(command.CustomerFrom) ? false : true).AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get data theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                plants = plants.Where(x => x.PlantCode == command.Plant);
            }    


            //Get data theo customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.CustomerTo)) 
                {
                    command.CustomerTo = command.CustomerFrom;
                }

                detailOds = detailOds.Where(x => x.OutboundDelivery.ShiptoParty.CompareTo(command.CustomerFrom) >= 0 &&
                                                 x.OutboundDelivery.ShiptoParty.CompareTo(command.CustomerTo) <= 0);
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


            //Get data theo material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu ko có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                {
                    command.MaterialTo = command.MaterialFrom;
                }

                detailOds = detailOds.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                                 x.ProductCodeInt <= long.Parse(command.MaterialTo));
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

            var query = detailOds.Select(x => new GetInputDataResponse
                        {
                            //Id = Guid.NewGuid(),
                            //Plant
                            Plant = x.Plant,
                            //Customer
                            Customer = x.OutboundDelivery.ShiptoParty ?? "",
                            //Customer name
                            CustomerName = x.OutboundDelivery.ShiptoPartyName ?? "",
                            //Material
                            Material = x.ProductCodeInt.ToString(),
                            //Material desc
                            MaterialDesc = !string.IsNullOrEmpty(x.ProductCode) ? materialQuery.FirstOrDefault(x => x.ProductCode == x.ProductCode).ProductName : "",
                            //UoM
                            Unit = x.Unit ?? "",
                            //Outbound Delivery
                            OutboundDelivery = x.OutboundDelivery.DeliveryCode ?? "",
                            //Outbound Delivery Item
                            OutboundDeliveryItem = x.OutboundDeliveryItem ?? "",
                            //Batch
                            Batch = x.Batch ?? "",
                            //Số phương tiện
                            VehicleCode = x.OutboundDelivery.VehicleCode ?? "",
                            //Sloc
                            Sloc = x.StorageLocation ?? "",
                            SlocName = !string.IsNullOrEmpty(x.StorageLocation) ? slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName : "",
                            //Document Date
                            DocumentDate = x.OutboundDelivery.DocumentDate
                        });


            var index = 1;
            foreach (var item in query)
            {
                item.IndexKey = index;
                index++;
            }

            #region Phân trang
            //Số dòng dữ liệu
            var totalRecords = query.Count();

            //Sorting
            var dataSorting = PagingSorting.Sorting(command.Paging, query.AsQueryable());
            //Phân trang
            var responsePaginated = PaginatedList<GetInputDataResponse>.Create(dataSorting, command.Paging.Offset, command.Paging.PageSize);
            var res = new PagingResultSP<GetInputDataResponse>(responsePaginated, totalRecords, command.Paging.PageIndex, command.Paging.PageSize);

            //Đánh số thứ tự
            if (res.Data.Any())
            {
                int i = command.Paging.Offset;
                foreach (var item in res.Data)
                {
                    i++;
                    item.STT = i;
                }
            }
            #endregion

            return await Task.FromResult(res);
        }
    }
}
