using ISD.Core;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using ISD.ViewModels.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class QuestionBankController : BaseController
    {
        // GET: QuestionBank
        public ActionResult Index()
        {
            ViewBag.Actived = new List<SelectListItem>()
            {
                new SelectListItem(){Text="Đã trả lời",Value="true"},
                new SelectListItem(){Text="Chưa trả lời", Value="false" }
            };
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(Guid id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ExportExcel(QuestionSearchViewModel searchModel)
        {
            List<QuestionExportViewModel> result = new List<QuestionExportViewModel>();
            var categoryId = Guid.NewGuid();
            if(!string.IsNullOrEmpty(searchModel.QuestionCategoryId))
            {
                categoryId = Guid.Parse(searchModel.QuestionCategoryId);
            }   
            result = (from question in _context.QuestionBankModel
                      join category in _context.CatalogModel on question.QuestionCategoryId equals category.CatalogId
                      join department in _context.CatalogModel on question.DepartmentId equals department.CatalogId
                      join account in _context.AccountModel on question.CreateBy equals account.AccountId
                      where (searchModel.QuestionBankCode == null || question.QuestionBankCode == searchModel.QuestionBankCode) &&
                             (searchModel.Question == null || searchModel.Question == "" || question.Question.Contains(searchModel.Question)) &&
                              (searchModel.QuestionCategoryId == null || searchModel.QuestionCategoryId == "" || question.QuestionCategoryId == categoryId) &&
                              (searchModel.Actived == null || question.Actived == searchModel.Actived)
                      orderby question.QuestionBankCode
                      select new QuestionExportViewModel()
                      {
                          QuestionBankCode = question.QuestionBankCode,
                          Question = question.Question,
                          Answer = question.Answer,
                          AnswerC = question.AnswerC,
                          AnswerB = question.AnswerB,
                          QuestionCategoryName = category.CatalogText_vi,
                          CreateBy = account.FullName,
                          DepartmentName = department.CatalogText_vi,
                          CreateTime = question.CreateTime
                      }).ToList();
            var s = ConvertHtmlToText(result);
            return Export(ConvertHtmlToText(result));
        }


        public ActionResult Export(List<QuestionExportViewModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "NGÂN HÀNG CÂU HỎI";

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "QuestionBankCode", isAllowedToEdit = false}); 
            columns.Add(new ExcelTemplate { ColumnName = "Question", isAllowedToEdit = false, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "Answer", isAllowedToEdit = false, isWraptext = true });           
            columns.Add(new ExcelTemplate { ColumnName = "AnswerC", isAllowedToEdit = false, isWraptext = true });         
            columns.Add(new ExcelTemplate { ColumnName = "AnswerB", isAllowedToEdit = false, isWraptext = true });           
            columns.Add(new ExcelTemplate { ColumnName = "QuestionCategoryName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CreateBy", isAllowedToEdit = false });           
            columns.Add(new ExcelTemplate { ColumnName = "CreateTime", isAllowedToEdit = false, isDateTime = true });
           // columns.Add(new ExcelTemplate { ColumnName = "CreateTime", isAllowedToEdit = false });
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        private List<QuestionExportViewModel> ConvertHtmlToText(List<QuestionExportViewModel> list)
        {
            foreach(var item in list)
            {
                if(!string.IsNullOrEmpty(item.Answer))
                {
                    item.Answer = HtmlToText.ConvertHtml(item.Answer);
                }

                if (!string.IsNullOrEmpty(item.AnswerC))
                {
                    item.AnswerC = HtmlToText.ConvertHtml(item.AnswerC);
                }
                if (!string.IsNullOrEmpty(item.AnswerB))
                {
                    item.AnswerB = HtmlToText.ConvertHtml(item.AnswerB);
                }              
            }
            return list;
        }
    }
}