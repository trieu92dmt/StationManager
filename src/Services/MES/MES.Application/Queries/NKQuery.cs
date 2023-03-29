using Core.Extensions;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NK;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Drop down số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy unit theo material và plant
        /// </summary>
        /// <param name="material"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        Task<string> GetUnitByMaterialAndPlant(string material, string plant);
    }

    public class NKQuery : INKQuery
    {
        private readonly IRepository<OtherImportModel> _nkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;

        public NKQuery(IRepository<OtherImportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo,
                       IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<ScaleModel> scaleRepo)
        {
            _nkRepo = nkRepo;
            _plantRepo = plantRepo;
            _prdRepo = prdRepo;
            _custRepo = custRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _scaleRepo = scaleRepo;
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
                                                         x.ProductCodeInt <= long.Parse(command.MaterialTo) &&
                                                         x.PlantCode == command.Plant).Select(x => new GetInputDataResponse
                                                         {
                                                             Id = Guid.NewGuid(),
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

        public async Task<List<SearchNKResponse>> SearchNK(SearchNKCommand command)
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
            if (command.WeightVotes != null && command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            //Query Material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Query Customer
            var customers = _custRepo.GetQuery().AsNoTracking();

            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();

            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            //Scale
            var scale = _scaleRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchNKResponse
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
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
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
                ScaleType = !string.IsNullOrEmpty(x.WeightHeadCode) ? scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).isCantai == true ? "CANXETAI" :
                                                                      scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).ScaleType == true ? "TICHHOP" : "KHONGTICHHOP" : "KHONGTICHHOP",
                //18 Trọng lượng cân
                Weight = x.Weight ?? 0,
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
                Image = !string.IsNullOrEmpty(x.Image) ? $"{new ConfigManager().DomainUploadUrl}{x.Image}" : "",
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
                ChangeOn = x.LastEditTime,
                //34 Material Doc
                MaterialDoc = x.MaterialDocument ?? null,
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
                //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            return data;
        }

        /// <summary>
        /// Lấy dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword">Từ khóa</param>
        /// <returns></returns>
        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nkRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        /// <summary>
        /// Lấy đơn vị của material theo plant
        /// </summary>
        /// <param name="material"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        public async Task<string> GetUnitByMaterialAndPlant(string material, string plant)
        {
            var prd = await _prdRepo.FindOneAsync(x => x.ProductCodeInt == long.Parse(material) && x.PlantCode == plant);

            return prd.Unit;
        }
    }
}
 