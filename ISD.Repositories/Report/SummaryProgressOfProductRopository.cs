using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Report
{
  public class SummaryProgressOfProductRopository
    {
        EntityDataContext _context;
        public SummaryProgressOfProductRopository(EntityDataContext context)
        {
            _context = context;
        }
        public List<SummaryProgressOfProductsViewModel> SummaryProgressOfProduct()
        {

            var res = _context.Database.SqlQuery<SummaryProgressOfProductsViewModel>("[ghReports].SummaryProgressOfProduct").ToList();
            return res;
        }
    }
}
