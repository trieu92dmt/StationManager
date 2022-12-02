using ISD.Core;
using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marketing.Controllers
{
    public class CampaignController : BaseController
    {
        // GET: Campaign
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(Guid Id)
        {
            var saleOrg = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.SaleOrgCode).ToList().Select(s => new StoreModel
            {
                SaleOrgCode = s.SaleOrgCode,
                StoreName = s.SaleOrgCode + " | " + s.StoreName
            });
            ViewBag.SaleOrg = new SelectList(saleOrg, "SaleOrgCode", "StoreName");
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult Create()
        {
            var saleOrg = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.SaleOrgCode).ToList().Select(s => new StoreModel
            {
                SaleOrgCode = s.SaleOrgCode,
                StoreName = s.SaleOrgCode + " | " + s.StoreName
            });
            ViewBag.SaleOrg = new SelectList(saleOrg, "SaleOrgCode", "StoreName");
            return View();
        }

        public ActionResult ReportById()
        {      
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult Unsubscribe(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}