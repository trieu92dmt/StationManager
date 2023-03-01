using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationNS.Application.Commands.NNVLGCs
{
    public class NNVLGCIntegrationCommand : IRequest<List<NNVLGCResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Vendor
        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
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
        public string Status { get; set; }
    }

    public class NNVLGCIntegrationCommandHandler : IRequestHandler<NNVLGCIntegrationCommand, List<NNVLGCResponse>>
    {
        private readonly IRepository<VendorModel> _vendorRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ComponentImportModel> _nnvlgcRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;

        public NNVLGCIntegrationCommandHandler(IRepository<VendorModel> vendorRepo, IRepository<ProductModel> prodRepo, IRepository<PlantModel> plantRepo,
                         IRepository<ComponentImportModel> nnvlgcRepo, IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
        {
            _vendorRepo = vendorRepo;
            _prodRepo = prodRepo;
            _plantRepo = plantRepo;
            _nnvlgcRepo = nnvlgcRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
        }
        public async Task<List<NNVLGCResponse>> Handle(NNVLGCIntegrationCommand request, CancellationToken cancellationToken)
        {
            #region Format Day
            //Ngày cân
            if (request.WeightDateFrom.HasValue)
            {
                request.WeightDateFrom = request.WeightDateFrom.Value.Date;
            }
            if (request.WeightDateTo.HasValue)
            {
                request.WeightDateTo = request.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var query = _nnvlgcRepo.GetQuery().AsNoTracking();


            //Lọc điều kiện theo plant
            if (!string.IsNullOrEmpty(request.Plant))
            {
                query = query.Where(x => x.PlantCode == request.Plant);
            }
            //Theo vendor
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                query = query.Where(x => !string.IsNullOrEmpty(x.VendorCode) ? x.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                                                                x.VendorCode.CompareTo(request.VendorTo) <= 0 : false);
            }
            //Theo material
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                query = query.Where(x => !string.IsNullOrEmpty(x.MaterialCode) ? x.MaterialCodeInt >= long.Parse(request.MaterialFrom) &&
                                                                                 x.MaterialCodeInt <= long.Parse(request.MaterialTo) : false);
            }


            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(request.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= request.WeightDateFrom &&
                                         x.CreateTime <= request.WeightDateTo);
            }

            //Check số phiếu cân
            if (request.WeightVotes != null && request.WeightVotes.Any())
            {
                query = query.Where(x => request.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (request.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == request.CreateBy);
            }
            //Search Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(x => x.Status == request.Status && x.ReverseDocument == null);
            }

            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();


            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new NNVLGCResponse
            {
                //Id
                NNVLId = x.ComponentImportId,
                //Plant
                Plant = x.PlantCode,
                //Plant name
                PlantName = x.PlantName,
                //Material
                Material = x.MaterialCodeInt.HasValue ? x.MaterialCodeInt.ToString() : "",
                //Material desc
                MaterialDesc = x.MaterialName ?? "",
                //Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = x.SlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //VendorName
                Vendor = x.VendorCode ?? "",
                VendorName = x.VendorName ?? "",
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Unit
                Unit = x.UOM,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
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
                ChangeOn = x.LastEditTime ?? null,
                //34 Material Doc
                MaterialDoc = x.MaterialDocument ?? null,
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false

            }).ToListAsync();

            return data;
        }
    }
}
