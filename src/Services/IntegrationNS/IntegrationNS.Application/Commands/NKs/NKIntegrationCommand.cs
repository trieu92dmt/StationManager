using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.NKs
{
    public class NKIntegrationCommand : IRequest<List<NKResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //sloc
        public string SlocFrom { get; set; }
        public string SlocTo { get; set; }
        //Customer
        public string CustomerFrom { get; set; }
        public string CustomerTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }

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

        //status
        public string Status { get; set; }
    }

    public class NKIntegrationCommandHandler : IRequestHandler<NKIntegrationCommand, List<NKResponse>>
    {
        private readonly IRepository<OtherImportModel> _nkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        public NKIntegrationCommandHandler(IRepository<OtherImportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo,
                                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo)
        {
            _nkRepo = nkRepo;
            _plantRepo = plantRepo;
            _prdRepo = prdRepo;
            _custRepo = custRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
        }

        public async Task<List<NKResponse>> Handle(NKIntegrationCommand command, CancellationToken cancellationToken)
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

            //Theo Sloc
            if (!string.IsNullOrEmpty(command.SlocFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SlocTo))
                    command.SlocTo = command.SlocFrom;
                query = query.Where(x => x.SlocCode.CompareTo(command.SlocFrom) >= 0 &&
                                         x.SlocCode.CompareTo(command.SlocTo) <= 0);
            }

            //Theo Customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.CustomerTo))
                    command.CustomerTo = command.CustomerFrom;
                query = query.Where(x => x.Customer.CompareTo(command.CustomerFrom) >= 0 &&
                                         x.Customer.CompareTo(command.CustomerTo) <= 0);
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

            //Query Material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Query Customer
            var customers = _custRepo.GetQuery().AsNoTracking();

            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();

            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();


            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new NKResponse
            {
                //ID NK
                NKID = x.OtherImportId,
                //7 Plant
                Plant = x.PlantCode ?? "",
                //9 Material
                Material = x.MaterialCodeInt.ToString() ?? "",
                //10 Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName : "",
                //Customer
                Customer = x.Customer,
                CustomerFmt = !string.IsNullOrEmpty(x.Customer) ? $"{x.Customer} | {customers.FirstOrDefault(x => x.CustomerNumber == x.CustomerNumber).CustomerName}" : "",
                //Special Stock
                SpecialStock = x.SpecialStock ?? "",
                //Số xe tải
                TruckInfoId = x.TruckInfoId,
                TruckNumber = x.TruckNumber,
                //13 Stor.Loc
                Sloc = x.SlocCode ?? "",
                SlocFmt = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {x.SlocName}",
                //14 Batch
                Batch = x.Batch ?? "",
                //15 SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //16 Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //17 Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //18 Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Vehicle code
                VehicleCode = x.VehicleCode ?? "",
                //19 Confirm Quantity
                ConfirmQuantity = x.ConfirmQty ?? 0,
                //20 SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //21 Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //24 UOM
                Unit = x.UOM ?? "",
                //25 Ghi chú
                Description = x.Description ?? "",
                //26 Hình ảnh
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //27 Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //28 Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //29 Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //30 Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                //31 Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //32 Crete On
                CreateOn = x.CreateTime ?? null,
                //33 Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //34 Material Doc
                MaterialDoc = x.MaterialDocument ?? "",
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? "",
            }).ToListAsync();

            return data;
        }
    }
}
