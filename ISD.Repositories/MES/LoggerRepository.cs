using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{
    public class LoggerRepository
    {
        EntityDataContext _context;
        public LoggerRepository()
        {
            _context = new EntityDataContext();
        }
        public void Logging(string message, string type)
        {
            var newLog = new MesSyncLogModel
            {
                Description = message,
                LogTime = DateTime.Now,
                LogType = type
            };
            _context.Entry(newLog).State = EntityState.Added;
            _context.SaveChanges();
        }
    }
}
