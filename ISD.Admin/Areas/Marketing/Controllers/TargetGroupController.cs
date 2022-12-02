using ISD.Core;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

using System.Web.Mvc;


namespace Marketing.Controllers
{
    public class TargetGroupController : BaseController
    {
        // GET: TargetGroup
        public ActionResult Index()
        {
            string s= ConfigurationManager.AppSettings["APINET5DomainUrl"];
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
       
        public ActionResult Edit(Guid Id)
        {
            ViewBag.Id = Id;
            return View();                  
        }
        public ActionResult LoadMember()
        {
            return PartialView();
        }
        #region export to excel
        public ActionResult ExportExcel(Guid? targetGroupId)
        {
            //Guid id = Guid.Parse(targetGroupId);
            var result = (from member in _context.MemberOfTargetGroupModel
                         join profile in _context.ProfileModel on member.ProfileId equals profile.ProfileId
                         where member.TargetGroupId == targetGroupId
                          select new MemberOfTargetGroupExcelModel
                         {
                             ProfileCode = profile.ProfileCode,
                             ProfileName = profile.ProfileName,
                             Email = profile.Email
                         }).ToList();
            return Export(result);
        }

        public ActionResult Export(List<MemberOfTargetGroupExcelModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "Thông tin khách hàng trong nhóm mục tiêu";

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Email", isAllowedToEdit = false });

            #endregion
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = "",//controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false
            });
            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion
    }
}