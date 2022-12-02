using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class NewsRepository
    {
        EntityDataContext _context;
        public NewsRepository(EntityDataContext db)
        {
            _context = db;
        }

        public List<NewsMobileViewModel> GetNewsBy(string CompanyCode)
        {
            var ret = new List<NewsMobileViewModel>();
            ret = (from n in _context.NewsModel
                   join m in _context.News_Company_Mapping on n.NewsId equals m.NewsId
                   join c in _context.CompanyModel on m.CompanyId equals c.CompanyId
                   join nc in _context.NewsCategoryModel on n.NewsCategoryId equals nc.NewsCategoryId
                   where c.CompanyCode == CompanyCode
                   && nc.NewsCategoryCode == ConstNewsCategoryCode.BangTin
                   && n.isShowOnMobile == true
                   select new NewsMobileViewModel()
                   {
                       NotificationId = n.NewsId,
                       Title = n.Title,
                       CreatedDate = n.ScheduleTime != null ? n.ScheduleTime : n.CreateTime,
                       //Nếu tin tức không có hình ảnh thì lấy ảnh của loại tin tức
                       ImageUrl = (n.ImageUrl != null && n.ImageUrl != ConstImageUrl.noImage) ? ConstDomain.Domain + "/Upload/News/" + n.ImageUrl : ConstDomain.Domain + "/Upload/NewsCategory/" + nc.ImageUrl
                   }).ToList();
            return ret;
        }

        public NewsMobileViewModel GetNewsDetails(Guid NewsId)
        {
            var ret = new NewsMobileViewModel();
            ret = (from n in _context.NewsModel
                   where n.NewsId == NewsId
                   select new NewsMobileViewModel()
                   {
                       NotificationId = n.NewsId,
                       Title = n.Title,
                       Description = n.Description,
                   }).FirstOrDefault();
            return ret;
        }
    }
}
