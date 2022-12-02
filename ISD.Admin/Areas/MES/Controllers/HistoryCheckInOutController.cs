using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class HistoryCheckInOutController : BaseController
    {
        // GET: CheckInOut
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            #region CommonDate
            var SelectedCommonDate = "Today";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.DurationCommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion
            return View();
        }

        public ActionResult _Search(CheckInOutSearchViewModel checkInOutSearchView)
        {
            return ExecuteSearch(() =>
            {
                if (checkInOutSearchView.DurationCommonDate != "Custom")
                {
                    DateTime? DurationFromDate;
                    DateTime? DurationToDate;

                    _unitOfWork.CommonDateRepository.GetDateBy( checkInOutSearchView.DurationCommonDate, out DurationFromDate, out DurationToDate);
                    checkInOutSearchView.DurationFromDate = DurationFromDate;
                    checkInOutSearchView.DurationToDate = DurationToDate;
                }

                var historyList = _unitOfWork.SalesEmployeeRepository.SearchHistoryCheckInOut(checkInOutSearchView);
                return PartialView(historyList);
            });
        }
    }
}