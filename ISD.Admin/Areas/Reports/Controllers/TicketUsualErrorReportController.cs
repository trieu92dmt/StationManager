using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class TicketUsualErrorReportController : BaseController
    {
        // GET: TicketUsualErrorReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            ViewBag.PageId = GetPageId("/Reports/TicketUsualErrorReport");
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            TaskSearchViewModel searchViewModel = new TaskSearchViewModel
            {
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate
            };
            CreateViewBag();
            return View(searchViewModel);
        }
        public void CreateViewBag()
        {
            //Mã màu SP
            var productColorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductColor);
            ViewBag.ProductColorCode = new SelectList(productColorLst, "CatalogCode", "CatalogCode");

            //Nhóm vật tư
            var productSearchCategoryLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductCategory);
            ViewBag.ProductCategoryCode = new SelectList(productSearchCategoryLst, "CatalogCode", "CatalogText_vi");

            //Các lỗi bảo hành thuờng gặp
            var usualErrorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.UsualError);
            ViewBag.UsualErrorCode = new SelectList(usualErrorLst, "CatalogCode", "CatalogText_vi");

            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi");
        }
        #endregion

        #region Export Excel
        public ActionResult ExportExcel(List<string> ProductCategoryCode, List<string> ProductColorCode, List<string> UsualErrorCode, DateTime? FromDate, DateTime? ToDate)
        {
            if (ToDate.HasValue)
            {
                ToDate = ToDate.Value.AddDays(1).AddSeconds(-1);
            }
            #region UsualErrorCodeList
            //Build your record
            var tableErrorSchema = new List<SqlMetaData>(1)
            {
                new SqlMetaData("Code", SqlDbType.NVarChar, 50)
            }.ToArray();

            //And a table as a list of those records
            var tableError = new List<SqlDataRecord>();
            if (UsualErrorCode != null && UsualErrorCode.Count > 0)
            {
                foreach (var r in UsualErrorCode)
                {
                    var tableRow = new SqlDataRecord(tableErrorSchema);
                    tableRow.SetString(0, r);
                    tableError.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableErrorSchema);
                tableError.Add(tableRow);
            }
            #endregion

            #region ProductColorCodeList
            //Build your record
            var tableColorSchema = new List<SqlMetaData>(1)
            {
                new SqlMetaData("Code", SqlDbType.NVarChar, 50)
            }.ToArray();

            //And a table as a list of those records
            var tableColor = new List<SqlDataRecord>();
            if (ProductColorCode != null && ProductColorCode.Count > 0)
            {
                foreach (var r in ProductColorCode)
                {
                    var tableRow = new SqlDataRecord(tableColorSchema);
                    tableRow.SetString(0, r);
                    tableColor.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableColorSchema);
                tableColor.Add(tableRow);
            }
            #endregion

            #region ProductCategoryCode
            //Build your record
            var tableCategorySchema = new List<SqlMetaData>(1)
            {
                new SqlMetaData("Code", SqlDbType.NVarChar, 50)
            }.ToArray();

            //And a table as a list of those records
            var tableCategory = new List<SqlDataRecord>();
            if (ProductCategoryCode != null && ProductCategoryCode.Count > 0)
            {
                foreach (var r in ProductCategoryCode)
                {
                    var tableRow = new SqlDataRecord(tableCategorySchema);
                    tableRow.SetString(0, r);
                    tableCategory.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableCategorySchema);
                tableCategory.Add(tableRow);
            }
            #endregion

            string sqlQuery = "EXEC [Report].[usp_TicketUsualErrorReport] @ProductCategoryCode, @ProductColorCode, @UsualErrorCode, @CurrentCompanyCode,@FromDate,@ToDate";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductCategoryCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableCategory
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProductColorCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableColor
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "UsualErrorCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableError
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentUser.CompanyCode
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FromDate",
                    Value = FromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ToDate",
                    Value = ToDate ?? (object)DBNull.Value
                }
            };
            #endregion

            var result = _context.Database.SqlQuery<TicketUsualErrorViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return Export(result);
        }
        public ActionResult Export(List<TicketUsualErrorViewModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO TỔNG HỢP LỖI SẢN PHẨM TRONG XỬ LÝ KHIẾU NẠI";
            columns.Add(new ExcelTemplate { ColumnName = "ProductLevelName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductCategoryName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductColorCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "UsualErrorName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CountOfTaskProduct", isAllowedToEdit = false });

            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion
    }
}