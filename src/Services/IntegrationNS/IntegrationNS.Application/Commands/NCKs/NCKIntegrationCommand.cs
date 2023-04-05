﻿using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Commands.NCKs
{
    public class NCKIntegrationCommand : IRequest<List<NCKResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Receving Sloc
        public string SlocFrom { get; set; }
        public string SlocTo { get; set; }
        //Reservation
        public string ReservationFrom { get; set; }
        public string ReservationTo { get; set; }
        //Material Doc
        public string MaterialDocFrom { get; set; }
        public string MaterialDocTo { get; set; }
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
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
        //Status
        public string Status { get; set; }
    }

    public class NCKIntegrationCommandHandler : IRequestHandler<NCKIntegrationCommand, List<NCKResponse>>
    {
        private readonly IRepository<WarehouseImportTransferModel> _nckRepo;
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        public NCKIntegrationCommandHandler(IRepository<WarehouseImportTransferModel> nckRepo, IRepository<MaterialDocumentModel> matDocRepo, IRepository<ProductModel> prodRepo,
                                            IRepository<StorageLocationModel> slocRepo, IRepository<AccountModel> userRepo,
                                            IRepository<CatalogModel> cataRepo)
        {
            _nckRepo = nckRepo;
            _matDocRepo = matDocRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
        }

        public async Task<List<NCKResponse>> Handle(NCKIntegrationCommand command, CancellationToken cancellationToken)
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

            var user = _userRepo.GetQuery().AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Query matDoc
            var matDocs = _matDocRepo.GetQuery().AsNoTracking();

            //Get query nck
            var query = _nckRepo.GetQuery().Include(x => x.MaterialDoc).AsNoTracking();

            //Lọc theo điều kiện
            //Lọc theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo reservation
            if (!string.IsNullOrEmpty(command.ReservationFrom))
            {
                //Không có reservation to thì search 1
                if (string.IsNullOrEmpty(command.ReservationTo))
                    command.ReservationTo = command.ReservationFrom;

                query = query.Where(x => !string.IsNullOrEmpty(x.Reservation) ? x.Reservation.CompareTo(command.ReservationFrom) >= 0 &&
                                                                          x.Reservation.CompareTo(command.ReservationTo) <= 0 : false);
            }

            //Theo sloc
            if (!string.IsNullOrEmpty(command.SlocFrom))
            {
                //Không có sloc to thì search 1
                if (string.IsNullOrEmpty(command.SlocTo))
                    command.SlocTo = command.SlocFrom;

                query = query.Where(x => !string.IsNullOrEmpty(x.SlocCode) ? x.SlocCode.CompareTo(command.SlocFrom) >= 0 &&
                                                                          x.SlocCode.CompareTo(command.SlocTo) <= 0 : false);
            }

            //Theo Material Doc
            if (!string.IsNullOrEmpty(command.MaterialDocFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialDocTo))
                    command.MaterialDocTo = command.MaterialDocFrom;

                query = query.Where(x => x.MaterialDocId.HasValue ? x.MaterialDoc.MaterialDocCode.CompareTo(command.MaterialDocFrom) >= 0 &&
                                                                    x.MaterialDoc.MaterialDocCode.CompareTo(command.MaterialDocTo) <= 0 : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.MaterialDocId.HasValue ? x.MaterialDoc.PostingDate >= command.DocumentDateFrom &&
                                                                    x.MaterialDoc.PostingDate <= command.DocumentDateTo : false);
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

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new NCKResponse
            {
                //Id
                NCKId = x.WarehouseImportTransferId,
                //Plant
                Plant = x.PlantCode,
                //Reservation
                Reservation = x.Reservation ?? "",
                //Material Doc
                MaterialDoc = x.MaterialDocId.HasValue ? x.MaterialDoc.MaterialDocCode : "",
                //Material Doc Item
                MaterialDocItem = x.MaterialDocId.HasValue ? x.MaterialDoc.MaterialDocItem : "",
                //Material
                Material = x.MaterialCodeInt.ToString(),
                //MaterialDesc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName,
                //Stor.Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = !string.IsNullOrEmpty(x.SlocCode) ? x.SlocName : "",
                //Batch
                Batch = string.IsNullOrEmpty(x.MaterialDocument) ? x.MaterialDocId.HasValue ? x.MaterialDoc.Batch : "" : x.Batch ?? "",
                //Sl bao
                BagQuantity = x.BagQuantity.HasValue ? x.BagQuantity : 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight.HasValue ? x.SingleWeight : 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight.HasValue ? x.Weight : 0,
                //Confirm Qty
                ConfirmQty = x.ConfirmQty.HasValue ? x.ConfirmQty : 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging.HasValue ? x.QuantityWithPackaging.Value : 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeitght.HasValue ? x.QuantityWeitght : 0,
                //Total Quantity => nếu có mat doc thì lấy table nghiệp vụ, không có thì lấy theo chứng từ
                TotalQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : (x.MaterialDocId.HasValue ? x.MaterialDoc.Quantity : 0),
                //Delivered Quantity => nếu có mat doc thì lấy table nghiệp vụ, không có thì lấy theo chứng từ
                DeliveredQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveryQuantity : 
                               x.MaterialDocId.HasValue ? matDocs.Where(m => m.MaterialDocCode == x.MaterialDoc.MaterialDocCode && m.MovementType == "313").Sum(m => m.Quantity)
                                                          - matDocs.Where(m => m.MaterialDocCode == x.MaterialDoc.MaterialDocCode && m.MovementType == "315").Sum(m => m.Quantity) : 0,
                //UoM
                Unit = prods.FirstOrDefault(x => x.ProductCode == x.ProductCode).Unit,
                //Document date
                DocumentDate = x.MaterialDocId.HasValue ? x.MaterialDoc.PostingDate : null,
                //Số xe tải
                TruckInfoId = x.TruckInfoId.HasValue ? x.TruckInfoId.Value : null,
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight.HasValue ? x.InputWeight : 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight.HasValue ? x.OutputWeight : 0,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
                //Create by
                CreateById = x.CreateBy,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Create on
                CreateOn = x.CreateTime,
                //Change by
                ChangeById = x.LastEditBy,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //Material doc
                MatDoc = x.MaterialDocument,
                //MatDocItem
                MatDocItem = x.MaterialDocumentItem,
                //Reverse doc
                RevDoc = x.ReverseDocument
            }).ToListAsync();

            return data;
        }
    }
}
