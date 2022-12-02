using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class CheckInOutController : BaseController
    {
        // GET: CheckInOut
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _SaveCheckIn(CheckInHistoryViewModel checkInHistoryViewModel)
        {
            return ExecuteSearch(() =>
            {
                _unitOfWork.SalesEmployeeRepository.SaveHistoryCheckInOut(checkInHistoryViewModel);

                var historyList = _unitOfWork.SalesEmployeeRepository.GetHistoryCheckInOut(checkInHistoryViewModel);
                return PartialView(historyList);

            });
        }


        [HttpGet]
        public JsonResult GetEmployeeBySerial(string serialTag)
        {
            return ExecuteContainer(() =>
            {
                var employee = _unitOfWork.SalesEmployeeRepository.GetSaleEmployeeBySerialTag(serialTag);
                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Data = employee
                }, JsonRequestBehavior.AllowGet);
            });
        }


    }
}