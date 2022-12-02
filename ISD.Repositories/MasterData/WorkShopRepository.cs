using ISD.Constant;
using ISD.EntityModels;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class WorkShopRepository
    {
        EntityDataContext _context;

        /// <summary>
        /// Khởi tạo WorkShop Repository
        /// </summary>
        /// <param name="db">EntityDataContext</param>
        public WorkShopRepository(EntityDataContext db)
        {
            _context = db;
        }

        /// <summary>
        /// Tìm kiếm WorkShop
        /// </summary>
        /// <param name="searchModel">WorkShop Search Model</param>
        /// <returns>List WorkFlowViewModel</returns>
        public List<WorkShopViewModel> Search(WorkShopSearchViewModel searchModel)
        {
            var lst = (
                       //WorkShop
                       from p in _context.WorkShopModel
                       //Company
                       join pr in _context.CompanyModel on p.CompanyId equals pr.CompanyId
                       //Company
                       join sTempt in _context.StoreModel on p.StoreId equals sTempt.StoreId into sList
                       from s in sList.DefaultIfEmpty()
                       where
                       //search by WorkShopCode
                       (searchModel.WorkShopCode == null || p.WorkShopCode.Contains(searchModel.WorkShopCode))
                       //search by WorkShopName
                       && (searchModel.WorkShopName == null || p.WorkShopName.Contains(searchModel.WorkShopName))
                       //search by CompanyId
                       && (searchModel.CompanyId == null || p.CompanyId == searchModel.CompanyId)
                       //search by Actived
                       && (searchModel.Actived == null || p.Actived == searchModel.Actived)
                       //Sắp xếp theo OrderIndex
                       orderby p.OrderIndex
                       select new WorkShopViewModel
                       {
                           WorkShopId = p.WorkShopId,
                           WorkShopCode = p.WorkShopCode,
                           WorkShopName = p.WorkShopName,
                           CompanyName = pr.CompanyName,
                           OrderIndex = p.OrderIndex,
                           Actived = p.Actived,
                           CreatedUser = p.CreatedUser,
                           CreatedTime = p.CreatedTime,
                           StoreId = p.StoreId,
                           StoreName = s.StoreName,
                       }).OrderBy(x=> x.OrderIndex).ToList();
           
            return lst;
        }

        /// <summary>
        /// Get All WorkShop
        /// </summary>
        /// <returns>Danh sách WorkShop</returns>
        public List<WorkShopViewModel> GetAll()
        {
            var lst = (from p in _context.WorkShopModel
                       where p.Actived == true
                       select new WorkShopViewModel
                       {
                           WorkShopId = p.WorkShopId,
                           WorkShopCode = p.WorkShopCode,
                           WorkShopName = p.WorkShopName,
                           CompanyId = p.CompanyId,
                           OrderIndex = p.OrderIndex,
                           Actived = p.Actived,
                           CreatedUser = p.CreatedUser,
                           StoreId = p.StoreId,
                           CreatedTime = p.CreatedTime
                       }).ToList();
            return lst;
        }

        /// <summary>
        /// Lấy WorkShop theo WorkShopId
        /// </summary>
        /// <param name="WorkShopId">Guid WorkShopId</param>
        /// <returns>WorkShopViewModel</returns>
        public WorkShopViewModel GetWorkShop(Guid WorkShopId)
        {
            var workFlow = (from w in _context.WorkShopModel
                            where w.WorkShopId == WorkShopId
                            select new WorkShopViewModel
                            {
                                WorkShopId = w.WorkShopId,
                                WorkShopCode = w.WorkShopCode,
                                WorkShopName = w.WorkShopName,
                                OrderIndex = w.OrderIndex,
                                Actived = w.Actived,
                                CreatedUser = w.CreatedUser,
                                CreatedTime = w.CreatedTime,
                                LastModifiedUser = w.LastModifiedUser,
                                LastModifiedTime = w.LastModifiedTime,
                                StoreId = w.StoreId,
                                CompanyId = w.CompanyId,
                            })
                            .FirstOrDefault();
            return workFlow;
        }

        /// <summary>
        /// Lấy WorkShop theo DepartmentId
        /// </summary>
        /// <param name="DepartmentId">Guid DepartmentId</param>
        /// <returns>WorkShopViewModel</returns>
        public WorkShopViewModel GetWorkShopByDepartment(Guid DepartmentId)
        {
            var workFlow = (from d in _context.DepartmentModel
                                //Department
                            join w in _context.WorkShopModel on d.WorkShopId equals w.WorkShopId
                            where d.DepartmentId == DepartmentId
                            select new WorkShopViewModel
                            {
                                WorkShopId = w.WorkShopId,
                                WorkShopCode = w.WorkShopCode,
                                WorkShopName = w.WorkShopName,
                                OrderIndex = w.OrderIndex,
                                Actived = w.Actived
                            }).FirstOrDefault();
            return workFlow;
        }

        /// <summary>
        /// Lấy WorkShop theo StoreId
        /// </summary>
        /// <param name="StoreId">Guid StoreId</param>
        /// <returns>List<WorkShopViewModel></WorkShopViewModel></returns>
        public IEnumerable<WorkShopViewModel> GetWorkShopByStore(string SaleOrgCode)
        {
            IQueryable<WorkShopViewModel> workFlow = (from w in _context.WorkShopModel
                                                      join d in _context.StoreModel on w.StoreId equals d.StoreId
                                                      //Departmen
                                                      where d.SaleOrgCode == SaleOrgCode
                                                      select new WorkShopViewModel
                                                      {
                                                          WorkShopId = w.WorkShopId,
                                                          WorkShopCode = w.WorkShopCode,
                                                          WorkShopName = w.WorkShopName,
                                                          OrderIndex = w.OrderIndex,
                                                          Actived = w.Actived
                                                      }).OrderBy(x => x.WorkShopCode);
            return workFlow;
        }

        /// <summary>
        /// Lấy WorkShop theo StoreId
        /// </summary>
        /// <param name="StoreId">Guid StoreId</param>
        /// <returns>List<WorkShopViewModel></WorkShopViewModel></returns>
        public IEnumerable<WorkShopViewModel> GetWorkShopByStore(Guid? StoreId)
        {
            IQueryable<WorkShopViewModel> workFlow = (from w in _context.WorkShopModel 
                            join d in _context.StoreModel on w.StoreId equals d.StoreId
                            //Departmen
                            where d.StoreId == StoreId
                            select new WorkShopViewModel
                            {
                                WorkShopId = w.WorkShopId,
                                WorkShopCode = w.WorkShopCode,
                                WorkShopName = w.WorkShopName,
                                OrderIndex = w.OrderIndex,
                                Actived = w.Actived
                            }).OrderBy(x=>x.WorkShopCode);
            return workFlow;
        }


        /// <summary>
        /// Thêm mới WorkShop
        /// </summary>
        /// <param name="model">WorkShopModel</param>
        /// <returns>WorkShopModel</returns>
        public WorkShopModel Create(WorkShopViewModel workShopViewModel)
        {
            WorkShopModel model = new WorkShopModel()
            {
                //Create WorkShopId and CreatedTime
               
                WorkShopCode = workShopViewModel.WorkShopCode,
                CompanyId = workShopViewModel.CompanyId,
                StoreId = workShopViewModel.StoreId,
                WorkShopName = workShopViewModel.WorkShopName,
                OrderIndex = workShopViewModel.OrderIndex,
                Actived = workShopViewModel.Actived,
                WorkShopId = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                CreatedUser = workShopViewModel.CreatedUser
            };
           
            // Add WorkShop
            _context.WorkShopModel.Add(model);
            _context.SaveChanges();
            return model;
        }

        /// <summary>
        /// Cập nhật WorkShop
        /// </summary>
        /// <param name="viewModel">WorkShopViewModel</param>
        public void Update(WorkShopViewModel viewModel)
        {
            //Get workShop theo WorkShopId
            var WorkShop = _context.WorkShopModel.FirstOrDefault(p => p.WorkShopId == viewModel.WorkShopId);
            if (WorkShop != null)
            {
                WorkShop.WorkShopCode = viewModel.WorkShopCode;
                WorkShop.WorkShopName = viewModel.WorkShopName;
                WorkShop.CompanyId = viewModel.CompanyId;
                WorkShop.StoreId = viewModel.StoreId;
                WorkShop.OrderIndex = viewModel.OrderIndex;
                WorkShop.Actived = viewModel.Actived;
                WorkShop.LastModifiedUser = viewModel.LastModifiedUser;
                WorkShop.LastModifiedTime = DateTime.Now;
                _context.Entry(WorkShop).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
