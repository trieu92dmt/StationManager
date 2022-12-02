using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ISD.Core;

namespace Warehouse.Controllers
{
    public class StockController : BaseController
    {
        #region Index
        // GET: Stock
        public ActionResult Index()
        {
            ViewBag.Actived = true;
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(string StockCode, string StockName, bool? Actived, string Plant,string StockType)
        {
            var retStockList = (from s in _context.StockModel
                                join a in _context.AccountModel on s.CreateBy equals a.AccountId
                                //search by Mã kho
                                where (StockCode == "" || s.StockCode.Contains(StockCode))
                                //Search by tên kho
                                && (StockName == "" || s.StockName.Contains(StockName))
                                //Search by Plant
                                && (Plant == "" || s.Plant == Plant)
                                //Search by Loại Kho
                                && (StockType== ""||s.StockType.Contains(StockType))
                                //Search by Trạng thái
                                && (Actived == null || s.Actived == Actived)
                                orderby s.StockCode
                                select new StockViewModel
                                {
                                    StockId = s.StockId,
                                    StockCode = s.StockCode,
                                    StockName = s.StockName,
                                    CreateTime = s.CreateTime,
                                    CreateByName = a.UserName,
                                    Actived = s.Actived,
                                    Plant = s.Plant,
                                    StockType = s.StockType,
                                }).ToList();
            return PartialView(retStockList);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            //var storeList = _context.StoreModel.Where(p => p.Actived == true).ToList();
            //var stock = new StockViewModel { StoreList = storeList };
            CreateViewBag();
            return View();
        }

        [HttpPost]
        public JsonResult Create(StockModel stockModel)
        {
            return ExecuteContainer(() =>
            {
                stockModel.StockId = Guid.NewGuid();
                stockModel.CreateBy = CurrentUser.AccountId;
                stockModel.CreateTime = DateTime.Now;
                stockModel.Actived = true;

                _context.Entry(stockModel).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Warehouse_Stock.ToLower()),
                });
            });
        }
        #endregion

        #region Edit
        public ActionResult Edit(Guid id)
        {
           // var storeList = _context.StoreModel.Where(p => p.Actived == true).ToList();
            var stock = _context.StockModel
                .Select(p => new StockViewModel
                {
                    StockId = p.StockId,
                    StockCode = p.StockCode,
                    StockName = p.StockName,
                    Actived = p.Actived,
                    Plant = p.Plant,
                    StockType = p.StockType,
                }).FirstOrDefault(p => p.StockId == id);
            if (stock == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Warehouse_Stock.ToLower()) });
            }
            CreateViewBag(stock.Plant,stock.StockType);
            return View(stock);
        }

        [HttpPost]
        public JsonResult Edit(StockViewModel stockView)
        {
            return ExecuteContainer(() =>
            {
                var stockInDb = _context.StockModel.FirstOrDefault(p => p.StockId == stockView.StockId);
                if (stockInDb == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.BadRequest,
                        Success = false,
                        Data = ""
                    });
                }
                stockInDb.StockName = stockView.StockName;
                stockInDb.StockType = stockView.StockType;
                stockInDb.Actived = stockView.Actived;
                stockInDb.LastEditBy = CurrentUser.AccountId;
                stockInDb.LastEditTime = DateTime.Now;

                _context.Entry(stockInDb).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Warehouse_Stock.ToLower())
                });
            });
        }
        #endregion

        #region Helper
        public void CreateViewBag(string Plant = null,string StockType = null)
        {
            //Plant
            var companyList = _context.CompanyModel.Where(p => p.Actived).Select(p => new CompanyViewModel()
            {
                CompanyCode = p.CompanyCode,
                CompanyName = p.CompanyCode + " | " + p.CompanyName,
                OrderIndex = p.OrderIndex,
            })
            .OrderBy(p => p.OrderIndex).ToList();

            ViewBag.Plant = new SelectList(companyList, "CompanyCode", "CompanyName", Plant);
            //stock type
            var stocktypelst = _context.CatalogModel.Where(x => x.CatalogTypeCode == "StockType").ToList();
            ViewBag.StockType = new SelectList(stocktypelst, "CatalogCode", "CatalogText_vi",StockType);
        }
        public ActionResult StockInStore()
        {
            var listStock = (from p in _context.StockModel
                             where p.Actived == true
                             select new StockViewModel
                             {
                                 StockId = p.StockId,
                                 StockName = p.StockCode + " | " + p.StockName,
                             }).ToList();
            ViewBag.StockId = new SelectList(listStock, "StockId", "StockName");

            var storeList = _unitOfWork.StoreRepository.GetAllStore();
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName");
            return PartialView();
        }


        public JsonResult GetAllForDropdown()
        {
            var listStock = _unitOfWork.StockRepository.GetAllForDropdown();
            var slStock = new SelectList(listStock, "StockId", "StockName");
            return Json(slStock,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockByStore(Guid StoreId)
        {
            var listStock = _unitOfWork.StockRepository.GetStockByStore(StoreId);
            var slStock = new SelectList(listStock, "StockId", "StockName");
            return Json(slStock, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMainStockByStore(Guid StoreId)
        {
            var stock = _unitOfWork.StockRepository.GetStockByStore(StoreId).FirstOrDefault();
            return Json(stock, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStockNameByStockId(Guid StockId)
        {
            var stock = _unitOfWork.StockRepository.GetStockNameByStockId(StockId);
            return Json(stock, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Search by autocomple
        public ActionResult GetStockForAutocomple(string SearchText)
        {
            var stockList = _unitOfWork.StockRepository.GetForAutocomple(SearchText);
            return Json(stockList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetToStockForAutocomple(string SearchText, Guid? FromStockId)
        {
            var stockList = _unitOfWork.StockRepository.GetToStockForAutocomple(SearchText, FromStockId);
            return Json(stockList, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}