using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class SalesEmployeeRepository
    {
        EntityDataContext _context;
        public SalesEmployeeRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Lấy tất cả nhân viên để gắn vào DropdownList
        /// </summary>
        /// <returns>Danh sách nhân viên</returns>
        public List<SalesEmployeeViewModel> GetAllForDropdownlist(bool? isMobile = false)
        {
            var salesEmployeeList = (from p in _context.SalesEmployeeModel
                                     join a in _context.AccountModel on p.SalesEmployeeCode equals a.EmployeeCode
                                     where p.Actived == true
                                     orderby p.SalesEmployeeCode
                                     select new SalesEmployeeViewModel
                                     {
                                         AccountId = a.AccountId,
                                         SalesEmployeeCode = p.SalesEmployeeCode,
                                         SalesEmployeeName = isMobile == true ? p.SalesEmployeeName : p.SalesEmployeeCode + " | " + p.SalesEmployeeName,
                                         RolesName = a.RolesModel.Where(m => m.isEmployeeGroup == true).Select(p => p.RolesName).FirstOrDefault(),
                                     }).ToList();
           
            return salesEmployeeList;
        }
        public List<SalesEmployeeViewModel> GetAllForDropdownlistWithoutAccount(bool? isMobile = false)
        {
            var salesEmployeeList = (from p in _context.SalesEmployeeModel
                                     where p.Actived == true
                                     orderby p.SalesEmployeeCode
                                     select new SalesEmployeeViewModel
                                     {
                                         SalesEmployeeCode = p.SalesEmployeeCode,
                                         SalesEmployeeName = isMobile == true ? p.SalesEmployeeName : p.SalesEmployeeCode + " | " + p.SalesEmployeeName,
                                     }).ToList();

            return salesEmployeeList;
        }
        public List<SalesEmployeeViewModel> GetSalesEmployeeByRoles(string RolesCode)
        {
            var rolesId = _context.RolesModel.Where(p => p.RolesCode == RolesCode).Select(p => p.RolesId).FirstOrDefault();
            var salesEmployeeList = (from p in _context.SalesEmployeeModel
                                     join a in _context.AccountModel on p.SalesEmployeeCode equals a.EmployeeCode
                                     from r in a.RolesModel
                                     where p.Actived == true && r.RolesId == rolesId
                                     orderby p.SalesEmployeeCode
                                     select new SalesEmployeeViewModel
                                     {
                                         AccountId = a.AccountId,
                                         SalesEmployeeCode = p.SalesEmployeeCode,
                                         SalesEmployeeName = p.SalesEmployeeName,
                                         RolesName = a.RolesModel.Where(m => m.isEmployeeGroup == true).Select(p => p.RolesName).FirstOrDefault(),
                                     }).ToList();

            salesEmployeeList = salesEmployeeList.GroupBy(x => new { x.AccountId, x.SalesEmployeeCode, x.SalesEmployeeName,  x.RolesName })
                                                .Select(x => new SalesEmployeeViewModel()
                                                {
                                                    AccountId = x.Key.AccountId,
                                                    SalesEmployeeCode = x.Key.SalesEmployeeCode,
                                                    SalesEmployeeName = x.Key.SalesEmployeeName,
                                                    RolesName = x.Key.RolesName,
                                                }).ToList();

            return salesEmployeeList;
        }

        public string GetSaleEmployeeCodeBy(Guid? AccountId = null)
        {
            var salesEmployee = (from p in _context.AccountModel
                                 where p.AccountId == AccountId
                                 && p.Actived == true
                                 select p.EmployeeCode).FirstOrDefault();
            return salesEmployee;
        }

        public string GetSaleEmployeeNameBy(string SalesEmployeeCode)
        {
            var SalesEmployeeName = (from p in _context.SalesEmployeeModel
                                     where p.SalesEmployeeCode == SalesEmployeeCode
                                     && p.Actived == true
                                     select p.SalesEmployeeName).FirstOrDefault();
            return SalesEmployeeName;
        }
         
        public EmployeeCheckinViewModel GetSaleEmployeeBySerialTag(string SerialTag)
        {
            var employee = (from p in _context.SalesEmployeeModel
                            join de in _context.DepartmentModel on p.DepartmentId equals de.DepartmentId into tmpDep
                            from dp in tmpDep.DefaultIfEmpty()
                            where p.SerialTag == SerialTag
                            select new EmployeeCheckinViewModel
                            {
                                SalesEmployeeCode = p.SalesEmployeeCode,
                                SalesEmployeeName = p.SalesEmployeeName,
                                SerialTag = p.SerialTag,
                                DepartmentName = dp.DepartmentName
                            }).FirstOrDefault();
            return employee;
        }

        public NFCCheckInOutModel SaveHistoryCheckInOut(CheckInHistoryViewModel checkInHistoryViewModel)
        {
            var NFC = new NFCCheckInOutModel
            {
                CheckInId = Guid.NewGuid(),
                CheckInDate = (DateTime)checkInHistoryViewModel.CheckInDate,
                SerialTag = checkInHistoryViewModel.SerialTag,
                WorkingDepartment = checkInHistoryViewModel.WorkingDepartment,
                CheckInOutDepartment = checkInHistoryViewModel.CheckInOutDepartment
            };
            _context.Entry(NFC).State = System.Data.Entity.EntityState.Added;
            _context.SaveChanges();
            return NFC;
        }

        public IEnumerable<CheckInHistoryViewModel> GetHistoryCheckInOut(CheckInHistoryViewModel checkInHistoryViewModel)
        {
            var historyList = (from p in _context.NFCCheckInOutModel
                               //Thẻ NFC
                               join e in _context.SalesEmployeeModel on p.SerialTag equals e.SerialTag
                               //Tổ làm việc
                               join wdTemp in _context.DepartmentModel on p.WorkingDepartment equals wdTemp.DepartmentId into wdList
                               from wd in wdList.DefaultIfEmpty()
                               //Tổ checkInOut
                               join cdTemp in _context.DepartmentModel on p.CheckInOutDepartment equals cdTemp.DepartmentId into cdList
                               from cd in cdList.DefaultIfEmpty()
                               //Phân xưởng làm việc
                               join wwTemp in _context.WorkShopModel on wd.WorkShopId equals wwTemp.WorkShopId into wwList
                               from ww in wwList.DefaultIfEmpty()  
                               //Phân xưởng checkInOut
                               join cwTemp in _context.WorkShopModel on cd.WorkShopId equals cwTemp.WorkShopId into cwList
                               from cw in cwList.DefaultIfEmpty()
                               //Tìm theo thẻ NFC
                               where p.SerialTag == checkInHistoryViewModel.SerialTag
                               //Sắp xếp theo ngày checkIn
                               orderby p.CheckInDate descending
                               select new CheckInHistoryViewModel
                               {
                                   //Thẻ SerialTag
                                   SerialTag = p.SerialTag,
                                   //Ngày checkInOut
                                   CheckInDate = p.CheckInDate,
                                   //Mã nhân viên
                                   SalesEmployeeCode = e.SalesEmployeeCode,
                                   //Tổ làm việc
                                   WorkingDepartment = wd.DepartmentId,
                                   WorkingDepartmentName = wd.DepartmentName,
                                   WorkingDepartmentCode = wd.DepartmentCode,
                                   //Phân xưởng làm việc
                                   WorkShopWorkingId= ww.WorkShopId,
                                   WorkShopWorkingName = ww.WorkShopName,
                                   WorkShopWorkingpCode = ww.WorkShopCode,
                                   //Tổ checkInOut
                                   CheckInOutDepartment = wd.DepartmentId,
                                   CheckInOutDepartmentName = wd.DepartmentName,
                                   CheckInOutDepartmentCode = wd.DepartmentCode,
                                   //Phân xưởng CheckInOut
                                   WorkShopCheckInOutId = cw.WorkShopId,
                                   WorkShopCheckInOutName = cw.WorkShopName,
                                   WorkShopCheckInOutCode = cw.WorkShopCode,
                               });
            return historyList;
        }  


        public IEnumerable<CheckInHistoryViewModel> SearchHistoryCheckInOut(CheckInOutSearchViewModel checkInOutSearchView)
        {
            IQueryable<CheckInHistoryViewModel> historyList = (from p in _context.NFCCheckInOutModel
                               join em in _context.SalesEmployeeModel on p.SerialTag equals em.SerialTag
                               where (checkInOutSearchView.SalesEmployeeCode == null || em.SalesEmployeeCode == checkInOutSearchView.SalesEmployeeCode)
                               && (checkInOutSearchView.SerialTag == null || p.SerialTag == checkInOutSearchView.SerialTag)
                               && (checkInOutSearchView.DurationFromDate == null || checkInOutSearchView.DurationFromDate <= p.CheckInDate)
                               && (checkInOutSearchView.DurationToDate == null || p.CheckInDate <= checkInOutSearchView.DurationToDate)
                               orderby p.CheckInDate descending
                               select new CheckInHistoryViewModel
                               {
                                   CheckInDate = p.CheckInDate,
                                   SerialTag = p.SerialTag,
                                   SalesEmployeeCode = em.SalesEmployeeCode,
                                   SalesEmployeeName = em.SalesEmployeeName
                               });
            return historyList;
        }

        public SalesEmployeeModel Find(Guid? AccountId)
        {
            var emp = (from p in _context.SalesEmployeeModel
                       join acc in _context.AccountModel on p.SalesEmployeeCode equals acc.EmployeeCode
                       where acc.AccountId == AccountId
                       select p).FirstOrDefault();
            return emp;
        }
        public SalesEmployeeModel Find(string SaleEmployee)
        {
            var emp = (from p in _context.SalesEmployeeModel
                       where p.SalesEmployeeCode == SaleEmployee
                       select p).FirstOrDefault();
            return emp;
        }
    }
}
