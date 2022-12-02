using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Work.Controllers
{
    public class CheckInOutController : BaseController
    {
        // GET: CheckInOut
        [ISDAuthorization]
        public ActionResult Index()
        {
            #region CommonDate
            var SelectedCommonDate = "Today";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            //Common Date 2
            var commonDateList2 = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate2 = new SelectList(commonDateList2, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion
            return View();
        }
        public ActionResult _Search(string SalesEmployeeCode, string SerialTag, string DurationCommonDate, DateTime? DurationFromDate, DateTime? DurationToDate)
        {
            return ExecuteSearch(() =>
            {
                if (DurationCommonDate != "Custom")
                {
                    _unitOfWork.CommonDateRepository.GetDateBy(DurationCommonDate, out DurationFromDate, out DurationToDate);

                }

                var historyList = (from p in _context.NFCCheckInOutModel
                                   join em in _context.SalesEmployeeModel on p.SerialTag equals em.SerialTag
                                   where (SalesEmployeeCode == "" || em.SalesEmployeeCode == SalesEmployeeCode)
                                   && (SerialTag == "" || p.SerialTag == SerialTag)
                                   && (DurationFromDate == null || DurationFromDate <= p.CheckInDate)
                                   && (DurationToDate == null || p.CheckInDate <= DurationToDate)
                                   orderby p.CheckInDate descending
                                   select new CheckInHistoryViewModel
                                   {
                                       CheckInDate = p.CheckInDate,
                                       SerialTag = p.SerialTag,
                                       SalesEmployeeCode = em.SalesEmployeeCode,
                                       SalesEmployeeName = em.SalesEmployeeName
                                   }).ToList();
                return PartialView(historyList);
            });
        }

        public JsonResult _SearchServerSide(DatatableViewModel model, string SalesEmployeeCode, string SerialTag, string DurationCommonDate, DateTime? DurationFromDate, DateTime? DurationToDate)
        {
            if (DurationCommonDate != "Custom")
            {
                _unitOfWork.CommonDateRepository.GetDateBy(DurationCommonDate, out DurationFromDate, out DurationToDate);

            }
            var query = (from p in _context.NFCCheckInOutModel
                         join em in _context.SalesEmployeeModel on p.SerialTag equals em.SerialTag
                         where (SalesEmployeeCode == "" || em.SalesEmployeeCode == SalesEmployeeCode)
                         && (SerialTag == "" || p.SerialTag == SerialTag)
                         && (DurationFromDate == null || DurationFromDate <= p.CheckInDate)
                         && (DurationToDate == null || p.CheckInDate <= DurationToDate)
                         orderby p.CheckInDate descending
                         select new CheckInHistoryViewModel
                         {
                             CheckInDate = p.CheckInDate,
                             SerialTag = p.SerialTag,
                             SalesEmployeeCode = em.SalesEmployeeCode,
                             SalesEmployeeName = em.SalesEmployeeName
                         });

            int filteredResultsCount;
            int totalResultsCount;
            var result = CustomSearchRepository.CustomSearchFunc<CheckInHistoryViewModel>(model, out filteredResultsCount, out totalResultsCount, query, "STT");
            if (result != null && result.Count > 0)
            {
                int i = model.start;
                foreach (var item in result)
                {
                    item.STT = ++i;
                }
            }
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
            });
        }

        [AllowAnonymous]
        public ActionResult CheckInOut()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult _CheckInOut(string SerialTag)
        {
            return ExecuteSearch(() =>
            {

                var Checkin = new NFCCheckInOutModel
                {
                    CheckInId = Guid.NewGuid(),
                    CheckInDate = DateTime.Now,
                    SerialTag = SerialTag
                };
                _context.Entry(Checkin).State = EntityState.Added;
                _context.SaveChanges();

                var historyList = (from p in _context.NFCCheckInOutModel
                                   join e in _context.SalesEmployeeModel on p.SerialTag equals e.SerialTag
                                   where p.SerialTag == SerialTag
                                   orderby p.CheckInDate descending
                                   select new CheckInHistoryViewModel
                                   {
                                       SerialTag = p.SerialTag,
                                       CheckInDate = p.CheckInDate,
                                       SalesEmployeeCode = e.SalesEmployeeCode
                                   }).Take(15).ToList();
                return PartialView(historyList);

            });
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetEmployeeBySerial(string serialTag)
        {
            return ExecuteContainer(() =>
            {
                var employee = (from p in _context.SalesEmployeeModel
                                join de in _context.DepartmentModel on p.DepartmentId equals de.DepartmentId into tmpDep
                                from dp in tmpDep.DefaultIfEmpty()
                                where p.SerialTag == serialTag
                                select new EmployeeCheckinViewModel
                                {
                                    SalesEmployeeCode = p.SalesEmployeeCode,
                                    SalesEmployeeName = p.SalesEmployeeName,
                                    SerialTag = p.SerialTag,
                                    DepartmentName = dp.DepartmentName
                                }).FirstOrDefault();

                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Data = employee
                }, JsonRequestBehavior.AllowGet);
            });
        }

        [AllowAnonymous]
        public JsonResult SaveCheckIn(string SeriaTag)
        {
            return ExecuteContainer(() =>
            {
                var Checkin = new NFCCheckInOutModel
                {
                    CheckInId = Guid.NewGuid(),
                    CheckInDate = DateTime.Now,
                    SerialTag = SeriaTag
                };
                _context.Entry(Checkin).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Message = "Checkin thành công!"
                });
            });
        }

    }
}