using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class GanttChartRepository
    {
        EntityDataContext _context;
        /// <summary>
        /// Khởi tạo task repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public GanttChartRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
        }

   

    }
}
