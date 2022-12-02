using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class DateClosedController : BaseController
    {
        // GET: DateClosed
        #region Index
        public ActionResult Index()
        {
            var dateClosed = _context.DateClosedModel.First().DateClosed;
            ViewBag.DateClosedInput = dateClosed;
            return View();
        }

        public ActionResult _Search()
        {
            return ExecuteSearch(() =>
            {
                var list = (from d in _context.DateClosedHistoryModel
                            join a in _context.AccountModel on d.ModifiedUser equals a.AccountId
                            orderby d.ModifiedTime descending
                            select new DateClosedViewModel
                            {
                                ModifiedUserName = !string.IsNullOrEmpty(a.FullName) ? a.FullName : a.UserName,
                                DateClosed = d.DateClosed,
                                ModifiedTime = d.ModifiedTime,
                            }).ToList();

                return PartialView(list);
            });
        }
        #endregion

        #region Lưu thông tin khóa sổ ngày
        public ActionResult SaveDateClosed(DateTime? DateClosedInput)
        {
            return ExecuteContainer(() =>
            {
                #region Xử lý các trường hợp không hợp lệ
                if (!DateClosedInput.HasValue)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Required, LanguageResource.DateClosed_DateClosedInput.ToLower())
                    });
                }
                #endregion

                #region Xử lý lưu dữ liệu
                //Lưu thông tin ngày mở khóa => Update ngày khóa sổ
                var existsModel = _context.DateClosedModel.FirstOrDefault();
                if (existsModel == null)
                {
                    var modelNew = new DateClosedModel
                    {
                        DateClosedId = Guid.NewGuid(),
                        DateClosed = DateClosedInput.Value,
                    };
                    _context.Entry(modelNew).State = EntityState.Added;
                }
                else
                {
                    existsModel.DateClosed = DateClosedInput.Value;
                    _context.Entry(existsModel).State = EntityState.Modified;
                }

                //Lưu thông tin lịch sử cập nhật
                var modelHistory = new DateClosedHistoryModel
                {
                    DateClosedId = Guid.NewGuid(),
                    DateClosed = DateClosedInput.Value,
                    ModifiedUser = CurrentUser.AccountId,
                    ModifiedTime = DateTime.Now,
                };

                _context.Entry(modelHistory).State = EntityState.Added;
                _context.SaveChanges();
                #endregion

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.DateClosed_DateClosedUpdate.ToLower())
                });

            });
        }
        #endregion
    }
}