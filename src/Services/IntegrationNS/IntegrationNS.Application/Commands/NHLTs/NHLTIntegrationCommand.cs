using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationNS.Application.Commands.NHLTs
{
    public class NHLTIntegrationCommand : IRequest<List<NHLTResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Customer
        public string CustomerFrom { get; set; }
        public string CustomerTo { get; set; }
        //Outbound Delivery
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Document Date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân 
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }
        public string Status { get; set; }
    }

    public class NHLTIntegrationCommandHandler : IRequestHandler<NHLTIntegrationCommand, List<NHLTResponse>>
    {
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _dtOdRepo;
        private readonly IRepository<CustmdSaleModel> _cusRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;

        public NHLTIntegrationCommandHandler(IRepository<GoodsReceiptTypeTModel> nhltRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prodRepo,
                         IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<CustmdSaleModel> cusRepo, IRepository<StorageLocationModel> slocRepo,
                         IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
        {
            _nhltRepo = nhltRepo;
            _plantRepo = plantRepo;
            _prodRepo = prodRepo;
            _dtOdRepo = dtOdRepo;
            _cusRepo = cusRepo;
            _slocRepo = slocRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
        }

        public async Task<List<NHLTResponse>> Handle(NHLTIntegrationCommand command, CancellationToken cancellationToken)
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
            //Search Status
            if (!string.IsNullOrEmpty(command.Status))
            {
                query = query.Where(x => x.Status == command.Status && x.ReverseDocument == null);
            }


            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new NHLTResponse
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
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Số batch
                Batch = x.Batch ?? "",
                //Trọng lượng
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Đơn vị vận tải
                TransportUnit = x.TransportUnit ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Unit
                Unit = x.UOM ?? "",
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
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
                //34 Material Doc
                MatDoc = x.MaterialDocument ?? null,
                //Documentdate
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //35 Reverse Doc
                RevDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
            }).ToListAsync();

            return data;
        }
    }
}
