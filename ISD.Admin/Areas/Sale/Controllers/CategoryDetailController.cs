using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels.Sale;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sale.Controllers
{
    public class CategoryDetailController : BaseController
    {
        // GET: CategoryDetail

        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _Search(CategorySearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                var ChiTietVatTu = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.CHITIETVATTU).FirstOrDefault();

                var categorydetail = (from p in _context.CategoryModel
                                      join c in _context.CategoryModel on p.ParentCategoryId equals c.CategoryId into caTmp
                                      from ppa in caTmp.DefaultIfEmpty()
                                      join br in _context.CategoryModel on ppa.ParentCategoryId equals br.CategoryId into cttmp
                                      from pca in cttmp.DefaultIfEmpty()
                                      where
                                //(p.ParentCategoryId != null)
                                p.CategoryTypeId == ChiTietVatTu.CategoryTypeId
                                //search by ParentCategoryId
                                && (searchViewModel.ParentCategoryId == null || p.ParentCategoryId == searchViewModel.ParentCategoryId)
                                //search by CategoryName
                                && (searchViewModel.CategoryName == null || p.CategoryName.Contains(searchViewModel.CategoryName))
                                //search by Actived
                                && (searchViewModel.Actived == null || p.Actived == searchViewModel.Actived)
                                      select new CategoryDetailViewModel()
                                      {
                                          CategoryId = p.CategoryId,
                                          CategoryCode = p.CategoryCode,
                                          CategoryName = p.CategoryName,
                                          ParentCategoryId = p.ParentCategoryId,
                                          //ParentCategoryName = p.CategoryName,
                                          OrderIndex = p.OrderIndex,
                                          Actived = p.Actived,
                                          ImageUrl = p.ImageUrl,
                                          BrandCode = pca.CategoryCode,
                                          BrandName = pca.CategoryName,
                                          MaterialGroupCode = ppa.CategoryCode,
                                          MaterialGroupName = ppa.CategoryName,
                                          ADN = p.ADN
                                      })
                              .OrderBy(x => x.OrderIndex).ToList();
                return PartialView(categorydetail);
            });
        }

        #endregion
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CategoryDetailViewModel viewModel = new CategoryDetailViewModel();

            //Get list nhóm sản phẩm
            var materialGroup = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.NHOMVATTU).FirstOrDefault();
            var materialGroupList = (from c in _context.CategoryModel
                     join b in _context.CategoryModel on c.ParentCategoryId equals b.CategoryId
                     orderby c.OrderIndex
                     where c.CategoryTypeId == materialGroup.CategoryTypeId
                     && c.Actived == true
                     select new
                     {
                         CategoryId = c.CategoryId,
                         CategoryName = b.CategoryCode + " | " + b.CategoryName + " - " + c.CategoryCode + " | " + c.CategoryName,
                         OrderIndex = c.OrderIndex
                     }).ToList();
                    
            //var materialGroupList = _context.CategoryModel.Where(p => p.CategoryTypeId == materialGroup.CategoryTypeId && p.Actived == true)
            //                                      .Select (p=>new 
            //                                      {
            //                                          CategoryId = p.CategoryId,
            //                                          CategoryName = p.CategoryCode + " | " + p.CategoryName,
            //                                          OrderIndex = p.OrderIndex
            //                                      })
            //                                .OrderBy(p => p.OrderIndex).ToList();
            ViewBag.ParentCategoryId = new SelectList(materialGroupList, "CategoryId", "CategoryName");
            //ProductType
            var producttypelist = _context.ProductTypeModel.ToList();
            ViewBag.ProductTypeId = new SelectList(producttypelist, "ProductTypeId", "ProductTypeName");
            //viewModel.IsTrackTrend = false;
            return View(viewModel);
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(CategoryModel model, HttpPostedFileBase ImageUrl)
        {
            return ExecuteContainer(() =>
            {
                var ChiTietVatTu = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.CHITIETVATTU).FirstOrDefault();

                model.CategoryId = Guid.NewGuid();
                model.CategoryTypeId = ChiTietVatTu.CategoryTypeId;
                // nhóm sản phẩm
                var materialGroup = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.NHOMVATTU).FirstOrDefault();
                var material = _context.CategoryModel.Where(p => p.CategoryTypeId == materialGroup.CategoryTypeId && p.Actived == true && p.CategoryId == model.ParentCategoryId)
                                                      .Select(p => p.ADN).FirstOrDefault().ToString();
                model.ADN = material + "." + model.CategoryCode;

                //if (ImageUrl != null)
                //{
                //    model.ImageUrl = Upload(ImageUrl, "Category");
                //}
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Sale_CategoryDetail.ToLower())
                });
            });
        }
        #endregion
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var categorydetail = (from p in _context.CategoryModel
                            where p.CategoryId == id
                            select new CategoryDetailViewModel()
                            {
                                CategoryId = p.CategoryId,
                                CategoryCode = p.CategoryCode,
                                CategoryName = p.CategoryName,
                                ParentCategoryId = p.ParentCategoryId,
                                CategoryTypeId = p.CategoryTypeId,
                                OrderIndex = p.OrderIndex,
                                Actived = p.Actived,
                                ImageUrl = p.ImageUrl,
                                ProductTypeId = p.ProductTypeId,
                                IsTrackTrend = p.IsTrackTrend ?? false,
                            })
                           .FirstOrDefault();
            if (categorydetail == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Sale_Category.ToLower()) });
            }
            var materialGroup = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.NHOMVATTU).FirstOrDefault();
            var materialGroupList = (from c in _context.CategoryModel
                                     join b in _context.CategoryModel on c.ParentCategoryId equals b.CategoryId
                                     orderby c.OrderIndex
                                     where c.CategoryTypeId == materialGroup.CategoryTypeId
                                     && c.Actived == true
                                     select new
                                     {
                                         CategoryId = c.CategoryId,
                                         CategoryName = b.CategoryCode + " | " + b.CategoryName + " - " + c.CategoryCode + " | " + c.CategoryName,
                                         OrderIndex = c.OrderIndex
                                     }).ToList();
            ViewBag.ParentCategoryId = new SelectList(materialGroupList, "CategoryId", "CategoryName",categorydetail.ParentCategoryId);
            //var producttypelist = _context.ProductTypeModel.ToList();
            //ViewBag.ProductTypeId = new SelectList(producttypelist, "ProductTypeId", "ProductTypeName", category.ProductTypeId);

            return View(categorydetail);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(CategoryDetailViewModel viewModel, HttpPostedFileBase ImageUrl)
        {
            return ExecuteContainer(() =>
            {
                var ChiTietVatTu = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.CHITIETVATTU).FirstOrDefault();

                var modelUpdate = _context.CategoryModel.Find(viewModel.CategoryId);
                if (modelUpdate == null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotFound,
                        Success = false,
                        Data = LanguageResource.Mobile_NotFound
                    });
                }

                //if (ImageUrl != null)
                //{
                //    modelUpdate.ImageUrl = Upload(ImageUrl, "Category");
                //}
                modelUpdate.CategoryName = viewModel.CategoryName;
                modelUpdate.OrderIndex = viewModel.OrderIndex;
                modelUpdate.Actived = viewModel.Actived.Value;
                modelUpdate.ParentCategoryId = viewModel.ParentCategoryId;
                //Get list nhóm sản phẩm
                var materialGroup = _context.CategoryTypeModel.Where(p => p.CategoryTypeCode == ConstCategoryCode.NHOMVATTU).FirstOrDefault();
                var material = _context.CategoryModel.Where(p => p.CategoryTypeId == materialGroup.CategoryTypeId && p.Actived == true && p.CategoryId == modelUpdate.ParentCategoryId)
                                                      .Select(p => p.ADN).FirstOrDefault().ToString();
                modelUpdate.ADN = material + "." + modelUpdate.CategoryCode;

                _context.Entry(modelUpdate).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Sale_CategoryDetail.ToLower())
                });
            });
        }
        #endregion
        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var categoryDetail = _context.CategoryModel.FirstOrDefault(p => p.CategoryId == id);
                if (categoryDetail != null)
                {
                    _context.Entry(categoryDetail).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Sale_CategoryDetail.ToLower())
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
        #region Remote Validation
        private bool IsExists(string CategoryCode, Guid? ParentCategoryId)
        {
            return (_context.CategoryModel.FirstOrDefault(p => p.CategoryCode == CategoryCode && p.ParentCategoryId == ParentCategoryId) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingCategoryCode(string CategoryCode, string CategoryCodeValid, Guid? ParentCategoryId)
        {
            try
            {
                if (CategoryCodeValid != CategoryCode)
                {
                    return Json(!IsExists(CategoryCode, ParentCategoryId));
                }
                else
                {
                    return Json(true);
                }
            }
            catch //(Exception ex)
            {
                return Json(false);
            }
        }
        #endregion
    }
}