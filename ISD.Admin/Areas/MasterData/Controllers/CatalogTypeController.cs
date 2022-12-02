using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ISD.Core;

namespace MasterData.Controllers
{
    public class CatalogTypeController : BaseController
    {
        // GET: CatalogType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(string CatalogTypeCode = "",string CatalogTypeName = "", bool? Actived = null)
        {
            return ExecuteSearch(() =>
            {
                var catalogTypeList = (from c in _context.CatalogTypeModel
                                       where (CatalogTypeName == "" || c.CatalogTypeName == CatalogTypeName)
                                       && (CatalogTypeCode == "" || c.CatalogTypeCode == CatalogTypeCode)
                                       && (Actived == null || c.Actived == Actived)
                                       select c).ToList();
                return PartialView(catalogTypeList);
            });
        }

        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(CatalogTypeModel model)
        {
            return ExecuteContainer(() =>
            {
                model.Actived = true;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.CatalogTypeCode.ToLower())
                });
            });
        }
        #endregion
    }
}