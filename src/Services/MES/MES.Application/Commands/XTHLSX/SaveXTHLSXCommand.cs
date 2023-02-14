﻿using ISD.Core.Exceptions;
using ISD.Core.Extensions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using MES.Application.Commands.OutboundDelivery;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XTHLSX
{
    public class SaveXTHLSXCommand: IRequest<bool>
    {
        public List<SaveXTHLSX> SaveXTHLSXs { get; set; } = new List<SaveXTHLSX>();
    }

    public class SaveXTHLSX
    {
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Component
        public string Component { get; set; }
        //Sloc
        public string SlocCode { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confỉm Qty
        public decimal? ConfirmQty { get; set; }
        //Số lần cân
        public int QuantityWeight { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
    }

    public class SaveXTHLSXCommandHandler : IRequestHandler<SaveXTHLSXCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<IssueForProductionModel> _xthlsxRepo;
        private readonly IRepository<DetailWorkOrderModel> _woDetailRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public SaveXTHLSXCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo,
                                             IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService,
                                             IRepository<ProductModel> prodRepo, IRepository<IssueForProductionModel> xthlsxRepo,
                                             IRepository<DetailWorkOrderModel> woDetailRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _unitOfWork = unitOfWork;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _xthlsxRepo = xthlsxRepo;
            _woDetailRepo = woDetailRepo;
            _slocRepo = slocRepo;
        }

        public async Task<bool> Handle(SaveXTHLSXCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho tpsx
            var nkhts = await _xthlsxRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkhts.Count() > 0 ? nkhts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query wo
            var detailWos = _woDetailRepo.GetQuery().Include(x => x.WorkOrder).AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveXTHLSXs)
            {

                //Check điều kiện lưu
                #region Check điều kiện lưu

                if (!item.ConfirmQty.HasValue || item.ConfirmQty <= 0)
                {
                    throw new ISDException("Confirm Quantity phải lớn hơn 0");
                }

                if (string.IsNullOrEmpty(item.WeightHeadCode))
                {
                    if (!item.BagQuantity.HasValue || item.BagQuantity <= 0)
                    {
                        throw new ISDException("Số lượng bao phải lớn hơn 0");
                    }
                    if (!item.SingleWeight.HasValue || item.SingleWeight <= 0)
                    {
                        throw new ISDException("Đơn trọng phải lớn hơn 0");
                    }
                }
                #endregion
                var IssForProductiontId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, IssForProductiontId.ToString(), $"{IssForProductiontId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XTHLSX");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra component
                var detailWo = !string.IsNullOrEmpty(item.WorkOrder) && !string.IsNullOrEmpty(item.Material) && !string.IsNullOrEmpty(item.Component) ?
                                    detailWos.FirstOrDefault(d => d.WorkOrder.WorkOrderCodeInt == long.Parse(item.WorkOrder) &&
                                                             d.WorkOrder.ProductCodeInt == long.Parse(item.Material) &&
                                                             d.ProductCodeInt == long.Parse(item.Component)) : null;

                _xthlsxRepo.Add(new IssueForProductionModel
                {
                    //1 IssForProductiontId
                    IssForProductiontId = IssForProductiontId,
                    //2 DetailWorkOrderId
                    DetailWorkOrderId = detailWo != null ? detailWo.DetailWorkOrderId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    ComponentCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component) && x.PlantCode == item.Plant).ProductCode,
                    ComponentCodeInt = long.Parse(item.Component),
                    //5   WeightId
                    WeightId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.WeighSessionID : null,
                    //6 WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //7   BagQuantity
                    BagQuantity = item.BagQuantity,
                    //8   SingleWeight
                    SingleWeight = item.SingleWeight,
                    //9  WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    //10  Weight
                    Weight = item.Weight,
                    //11  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //12  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackaging,
                    //13  QuantityWeitght
                    QuantityWeitght = item.QuantityWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //21  SlocCode
                    SlocCode = item.SlocCode,
                    //22  SlocName
                    SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.SlocCode)?.StorageLocationName,
                    //24  CreateTime
                    CreateTime = DateTime.Now,
                    //25  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //28  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
