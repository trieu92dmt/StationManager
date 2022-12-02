using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class ContConfigRepository
    {
        private EntityDataContext _context;
        public ContConfigRepository(EntityDataContext context)
        {
            _context = context;
        }

        public IEnumerable<ContConfigViewModel> ListAll(ContConfigViewModel viewModel)
        {
            IQueryable<ContConfigViewModel> data = (from p in _context.ContConfigModel
                                                    where
                                                            //Tìm theo mã nhóm sản phẩm
                                                            (viewModel.MaterialType == null || viewModel.MaterialType.Contains(p.MaterialType))
                                                           //Tìm theo tên công đoạn
                                                           && (viewModel.Plant == null || viewModel.Plant.Contains(p.Plant))
                                                           //Actived
                                                           && (viewModel.Actived == null || viewModel.Actived == p.Actived)
                                                    orderby p.OrderIndex
                                                    select new ContConfigViewModel
                                                    {
                                                        ContConfigId = p.ContConfigId,
                                                        //Mã nhóm sản phẩm
                                                        MaterialType = p.MaterialType,
                                                        //Plant
                                                        Plant = p.Plant,
                                                        //Formula
                                                        Formula = p.Formula,
                                                        //Thứ tự
                                                        OrderIndex = p.OrderIndex,
                                                        //Active
                                                        Actived = p.Actived,
                                                    });
            return data;
        }

        public ContConfigViewModel GetBy(Guid ContConfigId)
        {
            var data = (from p in _context.ContConfigModel
                        where p.ContConfigId == ContConfigId
                        select new ContConfigViewModel
                        {
                            ContConfigId = p.ContConfigId,
                            //Mã nhóm sản phẩm
                            MaterialType = p.MaterialType,
                            //Plant
                            Plant = p.Plant,
                            //Formula
                            Formula = p.Formula,
                            //Thứ tự
                            OrderIndex = p.OrderIndex,
                            //Active
                            Actived = p.Actived,
                        }).FirstOrDefault();
            return data;
        }

    }
}
