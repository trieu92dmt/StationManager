using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NNVL;
using MES.Application.DTOs.MES.NNVL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INNVLQuery
    {
        /// <summary>
        /// Lấy dữ liệu đầu vào
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<GetInputDataResponse>> GetInputDatas(SearchNNVLCommand request);
    }

    public class NNVLQuery : INNVLQuery
    {
        private readonly IRepository<VendorModel> _vendorRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<PlantModel> _plantRepo;

        public NNVLQuery(IRepository<VendorModel> vendorRepo, IRepository<ProductModel> prodRepo, IRepository<PlantModel> plantRepo)
        {
            _vendorRepo = vendorRepo;
            _prodRepo = prodRepo;
            _plantRepo = plantRepo;
        }

        public async Task<List<GetInputDataResponse>> GetInputDatas(SearchNNVLCommand request)
        {
            //Get query material
            //Kiểm tra nếu không có to thì search 1
            if (string.IsNullOrEmpty(request.MaterialTo))
            {
                request.MaterialTo = request.MaterialFrom;
            }
            var materials = await _prodRepo.GetQuery(x => x.ProductCodeInt >= long.Parse(request.MaterialFrom) &&
                                                    x.ProductCodeInt <= long.Parse(request.MaterialTo) &&
                                                    x.PlantCode == request.Plant).AsNoTracking().ToListAsync();

            //Get query vendor
            //Kiểm tra nếu không có to thì search 1
            if (string.IsNullOrEmpty(request.VendorTo))
            {
                request.VendorTo = request.VendorFrom;
            }
            var vendors = await _vendorRepo.GetQuery(x => x.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                                    x.VendorCode.CompareTo(request.VendorTo) <= 0).AsNoTracking().ToListAsync();

            var data = new List<GetInputDataResponse>();

            foreach(var v in vendors)
            {
                foreach(var m in materials)
                {
                    data.Add(new GetInputDataResponse
                    {
                        //Plant
                        Plant = m.PlantCode,
                        //Vendor
                        Vendor = v.VendorCode,
                        //VendorName
                        VendorName = v.VendorName,
                        //Material
                        Material = m.ProductCodeInt.ToString(),
                        //MaterialDesc
                        MaterialDesc = m.ProductName,
                        //Unit
                        Uint = m.Unit
                    });
                }
            }

            return data;
        }
    }
}
