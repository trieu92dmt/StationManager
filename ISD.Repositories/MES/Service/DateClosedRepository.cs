using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class DateClosedRepository
    {
        EntityDataContext _context;
        public DateClosedRepository(EntityDataContext context)
        {
            _context = context;
        }

        public async Task UpdateDateClosed()
        {
            //1.Tạo 1 filed config 1 ngày khóa sổ: lấy ngày hiện tại trừ 2
            DateTime dateClosedConfig = DateTime.Now.Date;
            var config = await _context.ResourceModel.Where(p => p.ResourceKey == "NumberOfDateClosed").FirstOrDefaultAsync();
            if (config != null)
            {
                var numOfDate = int.Parse(config.ResourceValue);
                dateClosedConfig = dateClosedConfig.AddDays(-numOfDate);
            }
            //2.Nếu lần chạy nằm trong khoảng 0 => 6h sáng: tự động khóa sổ
            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 0 && currentHour <= 6)
            {
                //Tìm trong bảng config ngày khóa sổ
                //i.nếu chưa có => thêm mới
                //ii.có rồi => cập nhật
                var configDateClosed = await _context.DateClosedModel.FirstOrDefaultAsync();
                if (configDateClosed == null)
                {
                    DateClosedModel newDateClosed = new DateClosedModel();
                    newDateClosed.DateClosedId = Guid.NewGuid();
                    newDateClosed.DateClosed = dateClosedConfig;
                    _context.Entry(newDateClosed).State = EntityState.Added;
                }
                else
                {
                    configDateClosed.DateClosed = dateClosedConfig;
                    _context.Entry(configDateClosed).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
