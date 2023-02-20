﻿using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NCK;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Queries
{
    public interface INCKQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchNCKCommand command);

        /// <summary>
        /// Lấy data xck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNCKResponse>> GetDataXCK(SearchNCKCommand command);

        /// <summary>
        /// Get data by reservation and reservation item
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="reservationItem"></param>
        /// <returns></returns>
        //Task<GetDataByRsvAndRsvItemResponse> GetDataByRsvAndRsvItem(string reservation, string reservationItem);
    }

    public class NCKQuery : INCKQuery
    {
        private readonly IRepository<WarehouseImportTransferModel> _nckRepo;
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ReservationModel> _resRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public NCKQuery(IRepository<WarehouseImportTransferModel> nckRepo, IRepository<MaterialDocumentModel> matDocRepo, IRepository<ProductModel> prodRepo,
                        IRepository<PlantModel> plantRepo, IRepository<ReservationModel> resRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _nckRepo = nckRepo;
            _matDocRepo = matDocRepo;
            _prodRepo = prodRepo;
            _plantRepo = plantRepo;
            _resRepo = resRepo;
            _slocRepo = slocRepo;
        }

        public Task<List<SearchNCKResponse>> GetDataXCK(SearchNCKCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nckRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.WeightVote,
                                            Value = x.WeightVote
                                        }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchNCKCommand command)
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

            //Tạo query
            var query = _matDocRepo.GetQuery()
                                        //Lấy các mat doc có movement type là 313
                                        .Where(x => (x.MovementType == "313") &&
                                                    //Lấy các mat doc đã post xuất kho
                                                    (x.ItemAutoCreated == "X") &&
                                                    //Loại trừ các mat doc có movement type là 315
                                                    (x.MovementType != "315"))
                                        .AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Reservation
            var reservations = _resRepo.GetQuery().Include(x => x.DetailReservationModel).AsNoTracking();

            //Query matDoc
            var matDocs = _matDocRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
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

                query = query.Where(x => x.Reservation.CompareTo(command.ReservationFrom) >= 0 &&
                                         x.Reservation.CompareTo(command.ReservationTo) <= 0);
            }

            //Theo sloc
            if (!string.IsNullOrEmpty(command.SlocFrom))
            {
                //Không có sloc to thì search 1
                if (string.IsNullOrEmpty(command.SlocFrom))
                    command.SlocTo = command.SlocFrom;

                query = query.Where(x => x.StorageLocation.CompareTo(command.SlocFrom) >= 0 &&
                                         x.StorageLocation.CompareTo(command.SlocTo) <= 0);
            }

            //Theo Material Doc
            if (!string.IsNullOrEmpty(command.MaterialDocFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialDocTo))
                    command.MaterialDocTo = command.MaterialDocFrom;

                query = query.Where(x => x.MaterialDocCode.CompareTo(command.MaterialDocFrom) >= 0 &&
                                         x.MaterialDocCode.CompareTo(command.MaterialDocTo) <= 0);
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
                query = query.Where(x => x.PostingDate >= command.DocumentDateFrom &&
                                         x.PostingDate <= command.DocumentDateTo);
            }


            //Data 
            var data = await query.Select(x => new GetInputDataResponse
            {
                //1. Plant
                Plant = x.PlantCode,
                PlantName = plants.FirstOrDefault(p => p.PlantCode == x.PlantCode).PlantName,
                //Material doc
                MaterialDoc = x.MaterialDocCode,
                //Material doc item
                MaterialDocItem = x.MaterialDocItem ?? "",
                //2. Reservation
                Reservation = !string.IsNullOrEmpty(x.Reservation) ? long.Parse(x.Reservation).ToString() : "",
                //4. Material
                Material = x.MaterialCodeInt.HasValue ? x.MaterialCodeInt.ToString() : "",
                //5. Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName : "",
                //7. Stor.Loc
                Sloc = x.StorageLocation ?? "",
                SlocName = string.IsNullOrEmpty(x.StorageLocation) ? "" : slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName,
                SlocFmt = string.IsNullOrEmpty(x.StorageLocation) ? "" : $"{x.StorageLocation} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName}",
                //9. Batch
                Batch = x.Batch,
                //10. Total Quantity
                TotalQty = x.Quantity ?? 0,
                //11. Delivered Quantity
                DeliveredQty = !string.IsNullOrEmpty(x.Reservation) && matDocs.Where(m => m.Reservation == x.Reservation && (x.MovementType == "313" || x.MovementType =="315")).Any() ? 
                                                                       matDocs.Where(m => m.Reservation == x.Reservation && x.MovementType == "313").Sum(m => m.Quantity) 
                                                                       - matDocs.Where(m => m.Reservation == x.Reservation && x.MovementType == "315").Sum(m => m.Quantity) : 0,
                //13. UoM
                Unit = x.BaseUOM,
                //Document Date
                DocumentDate = x.PostingDate

            }).ToListAsync();

            var index = 1;
            //Tính open quantity
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new GetInputDataResponse
                {
                    Plant = command.Plant,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                    Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
                });
            }

            return data;
        }
    }
}