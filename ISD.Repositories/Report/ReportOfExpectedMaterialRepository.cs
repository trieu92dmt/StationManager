using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Report
{
   public class ReportOfExpectedMaterialRepository
    {
        EntityDataContext _context;
        public ReportOfExpectedMaterialRepository(EntityDataContext context)
        {
            _context = context;
        }
        public List<ReportOfExpectedMaterialViewModel> ReportOfExpectedMaterial()
        {

            var res = _context.Database.SqlQuery<ReportOfExpectedMaterialViewModel>("[ghReports].ReportOfExpectedMaterial").ToList();
            return res;
        }
    }
}
