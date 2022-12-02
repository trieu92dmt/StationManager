using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class PlantRoutingConfigRepository
    {
        private EntityDataContext _context;
        public PlantRoutingConfigRepository(EntityDataContext context)
        {
            _context = context;
        }

        public PlantRoutingConfigViewModel GetBy(int PlantRoutingCode)
        {
            var data = (from p in _context.PlantRoutingConfigModel
                        where p.PlantRoutingCode == PlantRoutingCode
                        select new PlantRoutingConfigViewModel
                        {
                            //Mã công đoạn
                            PlantRoutingCode = p.PlantRoutingCode,
                            //Tên công đoạn
                            PlantRoutingName = p.PlantRoutingName,
                            //Nhóm công đoạn
                            PlantRoutingGroup = p.PlantRoutingGroup,
                            //Fromdata
                            FromData = p.FromData,
                            //Attribute 1
                            Attribute1 = p.Attribute1,
                            //Attribute 2
                            Attribute2 = p.Attribute2,
                            //Attribute 3
                            Attribute3 = p.Attribute3,
                            //Attribute 4
                            Attribute4 = p.Attribute4,
                            //Attribute 5
                            Attribute5 = p.Attribute5,
                            //Attribute 6
                            Attribute6 = p.Attribute6,
                            //Attribute 7
                            Attribute7 = p.Attribute7,
                            //Attribute 8
                            Attribute8 = p.Attribute8,
                            //Attribute 9
                            Attribute9 = p.Attribute9,
                            //Attribute 8
                            Attribute10 = p.Attribute10,
                            //Loại lead time
                            LeadTimeType = p.LeadTimeType,
                            //Lead time
                            LeadTime = p.LeadTime,
                            //Lead time dạng formula cho công thức
                            LeadTimeFormula = p.LeadTimeFormula,
                            //Ngày bắt đầu
                            FromDate = p.FromDate,
                            //Mô tả 
                            DescriptionFromDate = p.DescriptionFromDate,
                            //Ngày kết thúc
                            ToDate = p.ToDate,
                            //Mô tả
                            DescriptionToDate = p.DescriptionToDate,
                            //Condition
                            Condition = p.Condition,
                            //Mô tả
                            DescriptionCondition = p.DescriptionCondition,
                            //Thứ tự
                            OrderIndex = p.OrderIndex,
                            //Active
                            Actived = p.Actived,
                            //Là công đoạn chính
                            IsPrimaryStep = p.IsPrimaryStep,
                        }).FirstOrDefault();
            return data;
        }

        public IEnumerable<PlantRoutingConfigViewModel> ListAll(PlantRoutingConfigViewModel plantRoutingConfigViewModel)
        {
            IQueryable<PlantRoutingConfigViewModel> data = (from p in _context.PlantRoutingConfigModel
                                                                //Phân xưởng/Phòng ban
                                                            join d in _context.AllDepartmentModel on p.Attribute1 equals d.DepartmentCode into dTemp
                                                            from dpt in dTemp.DefaultIfEmpty()
                                                            join e in _context.PhysicsWorkShopModel on p.Attribute1 equals e.PhysicsWorkShopCode into eTemp
                                                            from pws in eTemp.DefaultIfEmpty()
                                                            where
                                                            //Tìm theo mã công đoạn
                                                            (plantRoutingConfigViewModel.PlantRoutingCode == null || plantRoutingConfigViewModel.PlantRoutingCode == p.PlantRoutingCode)
                                                            //Tìm theo tên công đoạn
                                                            && (plantRoutingConfigViewModel.PlantRoutingName == null || plantRoutingConfigViewModel.PlantRoutingName == p.PlantRoutingName)
                                                            //Tìm theo group
                                                            && (plantRoutingConfigViewModel.PlantRoutingGroup == null || plantRoutingConfigViewModel.PlantRoutingGroup == p.PlantRoutingGroup)
                                                            select new PlantRoutingConfigViewModel
                                                            {
                                                                //Mã công đoạn
                                                                PlantRoutingCode = p.PlantRoutingCode,
                                                                //Tên công đoạn
                                                                PlantRoutingName = p.PlantRoutingName,
                                                                //Nhóm công đoạn
                                                                PlantRoutingGroup = p.PlantRoutingGroup,
                                                                //Fromdata
                                                                FromData =  p.FromData,
                                                                //Attribute 1
                                                                //Attribute1 = p.Attribute1,
                                                                Attribute1 = string.IsNullOrEmpty(dpt.DepartmentName) ? pws.PhysicsWorkShopName : dpt.DepartmentName,
                                                                //Attribute 2
                                                                Attribute2 = p.Attribute2,
                                                                //Loại lead time
                                                                LeadTimeType = p.LeadTimeType,
                                                                //Lead time
                                                                LeadTime = p.LeadTime,
                                                                //Lead time formula
                                                                LeadTimeFormula = p.LeadTimeFormula,
                                                                //Ngày bắt đầu
                                                                FromDate= p.FromDate,
                                                                //Ngày kết thúc
                                                                ToDate = p.ToDate,
                                                                //Condition
                                                                Condition = p.Condition,
                                                                //Thứ tự
                                                                OrderIndex = p.OrderIndex,
                                                                //Active
                                                                Actived = p.Actived,
                                                                //Account
                                                                CreatedUser = p.CreatedUser,
                                                                CreatedTime = p.CreatedTime,
                                                                LastModifiedUser = p.LastModifiedUser,
                                                                LastModifiedTime = p.LastModifiedTime
                                                            });
            return data;
        }
    }
}
