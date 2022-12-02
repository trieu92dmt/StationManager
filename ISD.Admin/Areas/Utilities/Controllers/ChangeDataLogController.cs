using ISD.ViewModels;
using ISD.Core;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Utilities.Controllers
{
    public class ChangeDataLogController : BaseController
    {
        // GET: ChangeDataLog
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _List(Guid id)
        {
            var log = (from p in _context.ChangeDataLogModel
                       join acc in _context.AccountModel on p.LastEditBy equals acc.AccountId
                       where p.PrimaryKey == id
                       orderby p.LastEditTime descending
                       select new ChangeDataLogViewModel()
                       {
                           LogId = p.LogId,
                           TableName = p.TableName,
                           PrimaryKey = p.PrimaryKey,
                           FieldName = p.FieldName,
                           OldData = p.OldData,
                           NewData = p.NewData,
                           LastEditBy = p.LastEditBy,
                           LastEditUser = acc.UserName,
                           LastEditTime = p.LastEditTime
                       }).ToList();

            return PartialView(log);
        }
    }
}