using Azure.Core;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;

namespace StationManager.Application.Queries.CarCompany
{
    public interface IEmployeeQuery
    {
        Task<List<EmployeeResponse>> GetListEmployee(GetListEmployeeRequest request);

        Task<List<CommonResponse>> GetListEmployeeCode(Guid accountId);

        Task<List<CommonResponse>> GetListEmployeeName(Guid accountId);

        Task<DetailEmployeeResponse> GetDetailEmployee(Guid employeeId);
    }

    public class EmployeeQuery : IEmployeeQuery
    {
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<EmployeeModel> _eeRepo;
        private readonly IRepository<CatalogModel> _cataRepo;

        public EmployeeQuery(IRepository<CarCompanyModel> carCompanyRepo, IRepository<EmployeeModel> eeRepo,
                             IRepository<CatalogModel> cataRepo)
        {
            _carCompanyRepo = carCompanyRepo;
            _eeRepo = eeRepo;
            _cataRepo = cataRepo;
        }

        public async Task<DetailEmployeeResponse> GetDetailEmployee(Guid employeeId)
        {
            var ee = await _eeRepo.FindOneAsync(x => x.EmpoyeeId == employeeId);

            //Get catalog position
            var positions = _cataRepo.GetQuery(x => x.CatalogTypeCode == "Position").AsNoTracking();

            return new DetailEmployeeResponse
            {
                EmployeeId = ee.EmpoyeeId,
                EmployeeName = ee.EmployeeName,
                EmployeeCode = ee.EmployeeCode,
                Description = ee.Description,
                Email = ee.Email,
                PhoneNumber = ee.PhoneNumber,   
                PositionCode = ee.Position,
                Position = positions.FirstOrDefault(p => p.CatalogCode == ee.Position).CatalogName
            };
        }

        public async Task<List<EmployeeResponse>> GetListEmployee(GetListEmployeeRequest request)
        {
            //Get Car company
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            //Get catalog position
            var positions = _cataRepo.GetQuery(x => x.CatalogTypeCode == "Position").AsNoTracking();

            //Get List Ee
            var ees = await _eeRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId &&
                      (request.EmployeeCode.HasValue ? x.EmployeeCode == request.EmployeeCode : true) &&
                      (!string.IsNullOrEmpty(request.EmployeeName) ? x.EmployeeName == request.EmployeeName : true) &&
                      (!string.IsNullOrEmpty(request.PositionCode) ? x.Position == request.PositionCode : true))
                      .Select(x => new EmployeeResponse
                      {
                          EmployeeId = x.EmpoyeeId,
                          EmployeeCode = x.EmployeeCode,
                          EmployeeName = x.EmployeeName,
                          Description = x.Description,
                          Email = x.Email,
                          PhoneNumber = x.PhoneNumber,
                          Position = positions.FirstOrDefault(p => p.CatalogCode == x.Position).CatalogName
                      }).ToListAsync();

            return ees;
        }

        public async Task<List<CommonResponse>> GetListEmployeeCode(Guid accountId)
        {
            //Get Car company
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

            return await _eeRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId)
                          .Select(x => new CommonResponse
                          {
                              Key = x.EmployeeCode.ToString(),
                              Value = x.EmployeeCode.ToString(),
                          }).ToListAsync();
        }

        public async Task<List<CommonResponse>> GetListEmployeeName(Guid accountId)
        {
            //Get Car company
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

            return await _eeRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId)
                          .Select(x => new CommonResponse
                          {
                              Key = x.EmployeeName.ToString(),
                              Value = x.EmployeeName.ToString(),
                          }).ToListAsync();
        }
    }
}
