using ISD.EntityModels;
using ISD.Extensions;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class SalesEmployee2Repository
    {
        EntityDataContext _context;
        public SalesEmployee2Repository(EntityDataContext dataContext)
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


        /// <summary>
        /// Thêm mới SalesEmployee
        /// </summary>
        /// <param name="model">SalesEmployeeViewModel</param>
        /// <returns>SalesEmployeeModel</returns>
        public SalesEmployeeModel Create(SalesEmployeeViewModel model)
        {
            SalesEmployeeModel salesEmployeeModel = new SalesEmployeeModel()
            {
                CreateBy = model.CreateBy,
                CreateTime = DateTime.Now,
                SalesEmployeeName = model.SalesEmployeeName.FirstCharToUpper(),
                AbbreviatedName = model.SalesEmployeeName.ToAbbreviation(),
               
            };
            // Add SalesEmployee
            _context.SalesEmployeeModel.Add(salesEmployeeModel);
            _context.SaveChanges();

            return salesEmployeeModel;
        }

        /// <summary>
        /// Cập nhật SalesEmployee
        /// </summary>
        /// <param name="viewModel">SalesEmployeeViewModel</param>
        public void Update(SalesEmployeeViewModel viewModel)
        {
            //Get SalesEmployee theo SalesEmployeeId
            var SalesEmployee = _context.SalesEmployeeModel.FirstOrDefault(p => p.SalesEmployeeCode == viewModel.SalesEmployeeCode);
            if (SalesEmployee != null)
            {
                SalesEmployee.SalesEmployeeName = viewModel.SalesEmployeeName.FirstCharToUpper();
                SalesEmployee.AbbreviatedName = viewModel.SalesEmployeeName.ToAbbreviation();
                SalesEmployee.LastEditBy = viewModel.LastEditBy;
                SalesEmployee.LastEditTime = DateTime.Now;
                //Tìm trong account nếu có user reference đến thì => update tên luôn
                var acc = _context.AccountModel.Where(p => p.EmployeeCode == viewModel.SalesEmployeeCode).FirstOrDefault();
                if (acc != null)
                {
                    acc.FullName = viewModel.SalesEmployeeName;
                    _context.Entry(acc).State = EntityState.Modified;
                }
                _context.Entry(viewModel).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

    }
}
