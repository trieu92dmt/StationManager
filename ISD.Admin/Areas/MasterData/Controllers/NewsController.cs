using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.Core;
using ISD.ViewModels.MasterData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class NewsController : BaseController
    {
        // GET: News
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(bool? Actived = null, Guid? NewsCategoryId = null)
        {
            return ExecuteSearch(() =>
            {
                var news = (from p in _context.NewsModel
                            join acc in _context.AccountModel on p.CreateBy equals acc.AccountId
                            join c in _context.NewsCategoryModel on p.NewsCategoryId equals c.NewsCategoryId into cg
                            from c1 in cg.DefaultIfEmpty()
                            orderby p.NewsCode descending
                            where
                            //Search by Actived
                            (Actived == null || p.Actived == Actived)
                            && (NewsCategoryId == null || p.NewsCategoryId == NewsCategoryId)
                            select new NewsViewModel()
                            {
                                NewsId = p.NewsId,
                                NewsCategoryName = c1 != null ? c1.NewsCategoryName : null,
                                Title = p.Title,
                                ScheduleTime = p.ScheduleTime,
                                ImageUrl = p.ImageUrl,
                                CreateTime = p.CreateTime,
                                Actived = p.Actived,
                                CreateByName = acc.UserName,
                                Description = p.Description
                            }).ToList();
                return PartialView(news);
            });
        }
        #endregion

        //GET: /News/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            ViewBag.Company = _context.CompanyModel.OrderBy(p => p.OrderIndex).ToList();
            CreateViewBag();
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(NewsViewModel model, HttpPostedFileBase FileUpload, List<ListCompanyViewModel> listCompany)
        {
            return ExecuteContainer(() =>
            {
                NewsModel result = new NewsModel();
                result.NewsId = Guid.NewGuid();
                result.CreateBy = CurrentUser.AccountId;
                result.CreateTime = DateTime.Now;
                if (FileUpload != null)
                {
                    result.ImageUrl = Upload(FileUpload, "News");
                }
                else
                {
                    result.ImageUrl = "noimage.jpg";
                }
                result.Title = model.Title;
                result.Description = model.Description;
                result.ScheduleTime = model.ScheduleTime;
                result.isShowOnMobile = model.isShowOnMobile;
                result.isShowOnWeb = model.isShowOnWeb;
                //result.LastEditBy = model.LastEditBy;
                //result.LastEditTime = model.LastEditTime;
                result.Actived = model.Actived;
                result.NewsCategoryId = model.NewsCategoryId;
                _context.Entry(result).State = EntityState.Added;
                //_context.SaveChanges();

                if (listCompany != null && listCompany.Count > 0)
                {
                    foreach (var item in listCompany)
                    {
                        if (item.isCheckComp == true)
                        {
                            News_Company_Mapping mapping = new News_Company_Mapping();
                            mapping.NewsId = result.NewsId;
                            mapping.CompanyId = item.CompanyId;
                            _context.Entry(mapping).State = EntityState.Added;
                            //_context.SaveChanges();
                        }
                    }
                }
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_News.ToLower())
                });
            });
        }
        #endregion

        public ActionResult Detail(Guid id)
        {
            var model = _context.NewsModel.FirstOrDefault(p => p.NewsId == id);
            return View(model);
        }

        //GET: /News/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            ViewBag.Company = _context.CompanyModel.OrderBy(p => p.OrderIndex).ToList();
            ViewBag.News_Company_Mapping = _context.News_Company_Mapping.Where(p => p.NewsId == id).ToList();
            var news = _context.NewsModel.FirstOrDefault(p => p.NewsId == id);
            if (news == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_Company.ToLower()) });
            }
            NewsViewModel model = new NewsViewModel();
            model.NewsId = news.NewsId;
            model.NewsCode = news.NewsCode;
            model.NewsCategoryId = news.NewsCategoryId;
            model.Title = news.Title;
            model.Description = news.Description;
            model.ScheduleTime = news.ScheduleTime;
            model.ImageUrl = news.ImageUrl;
            model.isShowOnMobile = news.isShowOnMobile;
            model.isShowOnWeb = news.isShowOnWeb;
            model.CreateBy = news.CreateBy;
            model.CreateTime = news.CreateTime;
            model.LastEditBy = news.LastEditBy;
            model.LastEditTime = news.LastEditTime;
            model.Actived = news.Actived;

            CreateViewBag(null, news.NewsCategoryId);
            return View(model);
        }
        //POST: Edit
        [HttpPost]
        [ValidateInput(false)]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(NewsViewModel model, HttpPostedFileBase FileUpload, List<ListCompanyViewModel> listCompany)
        {
            return ExecuteContainer(() =>
            {
                //Lưu bảng tin
                var news = _context.NewsModel.FirstOrDefault(p => p.NewsId == model.NewsId);
                news.Title = model.Title;
                news.Description = model.Description;
                news.ScheduleTime = model.ScheduleTime;
                news.isShowOnMobile = model.isShowOnMobile;
                news.isShowOnWeb = model.isShowOnWeb;
                news.LastEditBy = CurrentUser.AccountId;
                news.LastEditTime = DateTime.Now;
                news.Actived = model.Actived;
                news.NewsCategoryId = model.NewsCategoryId;
                if (FileUpload != null)
                {
                    news.ImageUrl = getFileName(FileUpload);
                }
                _context.Entry(news).State = EntityState.Modified;
                //_context.SaveChanges();

                //Lưu mapping công ty
                if (listCompany != null && listCompany.Count > 0)
                {
                    foreach (var item in listCompany)
                    {
                        var checkmapping = _context.News_Company_Mapping.FirstOrDefault(p => p.NewsId == model.NewsId && p.CompanyId == item.CompanyId);
                        //Chưa có trong csdl => thêm mới
                        if (checkmapping == null)
                        {
                            if (item.isCheckComp == true)
                            {
                                News_Company_Mapping mapping = new News_Company_Mapping();
                                mapping.NewsId = model.NewsId;
                                mapping.CompanyId = item.CompanyId;
                                _context.Entry(mapping).State = EntityState.Added;
                                //_context.SaveChanges();
                            }
                        }
                        //Đã có trong csdk => sửa
                        else
                        {
                            if (item.isCheckComp == false)
                            {
                                _context.Entry(checkmapping).State = EntityState.Deleted;
                                //_context.SaveChanges();
                            }
                        }
                    }
                }
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_News.ToLower())
                });
            });
        }
        #endregion

        //GET: /News/Delete
        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var mapping = _context.News_Company_Mapping.Where(p => p.NewsId == id).ToList();
                if (mapping != null)
                {
                    _context.News_Company_Mapping.RemoveRange(mapping);
                }
                var news = _context.NewsModel.FirstOrDefault(p => p.NewsId == id);
                if (news != null)
                {
                    _context.Entry(news).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_News.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion

        #region CreateViewBag, Helper
        public void CreateViewBag(string CatalogTypeCode = null, Guid? NewsCategoryId = null)
        {
            //Get list CatalogType
            var catalogTypeList = _context.CatalogTypeModel.Where(p => p.Actived == true)
                                                           .OrderBy(p => p.CatalogTypeName).ToList();

            ViewBag.CatalogTypeCode = new SelectList(catalogTypeList, "CatalogTypeCode", "CatalogTypeName", CatalogTypeCode);

            //Get list NewsCategory
            var newscategoryList = _context.NewsCategoryModel.Where(p => p.Actived == true)
                                                           .OrderBy(p => p.OrderIndex).ToList();

            ViewBag.NewsCategoryId = new SelectList(newscategoryList, "NewsCategoryId", "NewsCategoryName", NewsCategoryId);
        }

        public string getFileName(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);

            //Create dynamically folder to save file
            var existPath = Server.MapPath("~/Upload/News");
            Directory.CreateDirectory(existPath);
            var path = Path.Combine(existPath, fileName);

            file.SaveAs(path);

            return fileName;
        }
        #endregion
    }
}