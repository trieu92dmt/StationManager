using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Report
{
   public class WorkShopProductionProgressRepository
    {
        EntityDataContext _context;
        public WorkShopProductionProgressRepository(EntityDataContext context)
        {
            _context = context;
        }
        public List<WorkShopProductionProgressReportViewModel> WorkshopProductionProgress()
        {
           
            var res = _context.Database.SqlQuery<WorkShopProductionProgressReportViewModel>("[ghReports].WorkshopProductionProgress").ToList();
            return res;
        }

    }
}
