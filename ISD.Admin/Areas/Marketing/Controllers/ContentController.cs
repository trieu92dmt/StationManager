using ISD.Core;
using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marketing.Controllers
{
    public class ContentController : BaseController
    {
        // GET: Content
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            var companyList = _context.CompanyModel.Where(p => p.Actived == true).OrderBy(p => p.CompanyCode).ToList().Select(s => new CompanyModel
            {
                CompanyCode = s.CompanyCode,
                CompanyName = s.CompanyCode + " | " + s.CompanyName
            });
            ViewBag.CompanyCode = new SelectList(companyList, "CompanyCode", "CompanyName");
            var saleOrg = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.SaleOrgCode).ToList().Select(s => new StoreModel
            {
                SaleOrgCode = s.SaleOrgCode,
                StoreName = s.SaleOrgCode + " | " + s.StoreName
            });
            ViewBag.SaleOrg = new SelectList(saleOrg, "SaleOrgCode", "StoreName");
            return View();
        }

        public ActionResult Edit(Guid Id)
        {
            ViewBag.Id = Id;
            var companyList = _context.CompanyModel.Where(p => p.Actived == true).OrderBy(p => p.CompanyCode).ToList().Select(s => new CompanyModel
            {
                CompanyCode = s.CompanyCode,
                CompanyName = s.CompanyCode + " | " + s.CompanyName
            });
            ViewBag.CompanyCode = new SelectList(companyList, "CompanyCode", "CompanyName");
            var saleOrg = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.SaleOrgCode).ToList().Select(s => new StoreModel
            {
                SaleOrgCode = s.SaleOrgCode,
                StoreName = s.SaleOrgCode + " | " + s.StoreName
            });
            ViewBag.SaleOrg = new SelectList(saleOrg, "SaleOrgCode", "StoreName");
            return View();
        }
    }
}