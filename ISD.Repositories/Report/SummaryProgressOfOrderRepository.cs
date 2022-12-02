using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Report
{
   public class SummaryProgressOfOrderRepository
    {
        EntityDataContext _context;
        public SummaryProgressOfOrderRepository(EntityDataContext context)
        {
            _context = context;
        }
        public List<SummaryProgressOfOrderViewModel> SummaryProgressOfOrder()
        {

            var res = _context.Database.SqlQuery<SummaryProgressOfOrderViewModel>("[ghReports].SummaryProgressOfOrder").ToList();
            return res;
        }
    }
}
