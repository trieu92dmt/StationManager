using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class CustomerDataReportController : BaseController
    {
        // GET: CustomerDataReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (CustomerDataReportSearchModel)TempData[CurrentUser.AccountId + "CustomerDataSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "CustomerDataTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "CustomerDataModeSearch"];
            //mode search
            if (modeSearch == null || modeSearch.ToString() == "Default")
            {
                ViewBag.ModeSearch = "Default";
            }
            else
            {
                ViewBag.ModeSearch = "Recently";
            }
            Guid templateId = Guid.Empty;
            if (tempalteIdString != null)
            {
                templateId = Guid.Parse(tempalteIdString.ToString());
            }
            var pageId = GetPageId("/Reports/CustomerDataReport");
            // search data
            if (searchModel == null || searchModel.IsView != true)
            {
                ViewBag.Search = null;
            }
            else
            {
                ViewBag.Search = searchModel;
            }
            //get list template
            var listSystemTemplate = _unitOfWork.PivotGridTemplateRepository.GetSystemTemplate(pageId);
            var listUserTemplate = _unitOfWork.PivotGridTemplateRepository.GetUserTemplate(pageId, CurrentUser.AccountId.Value);
            //get pivot setting
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            //nếu đang có template đang xem
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                //nếu ko có template đang xem thì lấy default của user
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    //nếu user không có template thì lấy default của hệ thống
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(sysDefaultTemplate.SearchResultTemplateId);
                        ViewBag.PivotSetting = pivotSetting;
                        ViewBag.TemplateId = sysDefaultTemplate.SearchResultTemplateId;
                    }
                    else // nếu tất cả đều không có thì render default partial view
                    {
                        ViewBag.PivotSetting = null;
                        ViewBag.TemplateId = templateId;
                    }
                }
            }
            ViewBag.PageId = pageId;
            ViewBag.SystemTemplate = listSystemTemplate;
            ViewBag.UserTemplate = listUserTemplate;
            CreateViewBagForSearch();
            return View();
        }
        private void CreateViewBagForSearch()
        {
            //Chi nhánh
            var storeList = _unitOfWork.StoreRepository.GetAllStore(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.CreateAtSaleOrg = new SelectList(storeList, "StoreId", "StoreName");
            //Phân loại khách hàng
            var catalogList = _context.CatalogModel.Where(
                 p => p.CatalogTypeCode == ConstCatalogType.CustomerType
                 && p.Actived == true && p.CatalogCode != ConstCustomerType.Contact).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.CustomerTypeCode = new SelectList(catalogList, "CatalogCode", "CatalogText_vi");
            //Khu vực
            var SaleOfficeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.SaleOffice);
            ViewBag.SaleOfficeCode = new SelectList(SaleOfficeList, "CatalogCode", "CatalogText_vi");
            //Tỉnh
            var provinceList = _unitOfWork.ProvinceRepository.GetAll();
            ViewBag.ProvinceId = new SelectList(provinceList, "ProvinceId", "ProvinceName");
            //Quận huyện
            var districtList = _unitOfWork.DistrictRepository.GetAll();
            ViewBag.DistrictId = new SelectList(districtList, "DistrictId", "DistrictName");

            //Địa chỉ
            var addressList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.AddressType);
            ViewBag.AddressTypeCode = new SelectList(addressList, "CatalogCode", "CatalogText_vi");
            //Đối tượng
            var ForeignList = new List<SelectListItem>() {
                new SelectListItem() {
                     Text = LanguageResource.Domestic,
                     Value = false.ToString()
                },
                new SelectListItem() {
                     Text = LanguageResource.Foreign,
                     Value = true.ToString()
                }
            };
            ViewBag.IsForeignCustomer = new SelectList(ForeignList, "Value", "Text");
            ViewBag.PageId = GetPageId("/Reports/CustomerDataReport");
        }
        #region export to excel
        public ActionResult ExportExcel(CustomerDataReportSearchModel searchModel)
        {
            var reportData = GetData(searchModel);          
            return Export(reportData);
        }

        private List<CustomerDataReportExcelModel> GetData(CustomerDataReportSearchModel searchModel)
        {
            List<CustomerDataReportExcelModel> result = new List<CustomerDataReportExcelModel>();

            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_CustomerDataReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.FromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.ToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@IsForeignCustomer", searchModel.isForeignCustomer ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerTypeCode", searchModel.CustomerTypeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateAtSaleOrg", searchModel.CreateAtSaleOrg ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@AddressTypeCode", searchModel.AddressTypeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProvinceId", searchModel.ProvinceId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DistrictId", searchModel.DistrictId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SaleOfficeCode", searchModel.SaleOfficeCode ?? (object)DBNull.Value);

                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                #region Convert to list
                                CustomerDataReportExcelModel model = new CustomerDataReportExcelModel();
                                model.ProfileForeignCode = item["ProfileForeignCode"].ToString();
                                model.ProfileName = item["ProfileName"].ToString();
                                model.Address = item["Address"].ToString();
                                model.Phone = item["Phone"].ToString();
                                //model.ProductName = item["ProductName"].ToString();
                                //model.SaleOrderCode = item["SaleOrderCode"].ToString();                               
                                model.SaleOfficeName = item["SaleOfficeName"].ToString();
                                model.ProvinceName = item["ProvinceName"].ToString();
                                model.DistrictName = item["DistrictName"].ToString();

                                //FAIL: dòng 2 trở đi gọi SAP không được
                                //if (!string.IsNullOrEmpty(model.ProfileForeignCode))
                                //{
                                //    var saleOrderList = _unitOfWork.SAPReportRepository.GetSaleOrderList(model.ProfileForeignCode, "2000");
                                //    if (saleOrderList != null && saleOrderList.Count > 0)
                                //    {
                                //        model.ProductName = string.Join(", ", saleOrderList.Select(p => p.ProductName).ToArray());
                                //        model.SaleOrderCode = string.Join(", ", saleOrderList.Select(p => p.SONumber).ToArray());
                                //    }
                                //}

                                result.Add(model);
                                #endregion
                            }
                        }
                    }
                }
            }
            return result;
        }

        public ActionResult Export(List<CustomerDataReportExcelModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "Báo cáo Dữ liệu khách hàng";
          
            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "ProfileForeignCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false, CustomWidth = 60, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false, CustomWidth = 60, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductName", isAllowedToEdit = false, CustomWidth = 60, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "SaleOrderCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProvinceName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DistrictName", isAllowedToEdit = false });

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
        #endregion
        public ActionResult ViewDetail(CustomerDataReportSearchModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerDataSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerDataTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerDataModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, CustomerDataReportSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerDataSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerDataTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerDataModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult CustomerDataPivotGridPartial(Guid? templateId = null, CustomerDataReportSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/CustomerDataReport");
            var listSystemTemplate = _unitOfWork.PivotGridTemplateRepository.GetSystemTemplate(pageId);
            var listUserTemplate = _unitOfWork.PivotGridTemplateRepository.GetUserTemplate(pageId, CurrentUser.AccountId.Value);
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(sysDefaultTemplate.SearchResultTemplateId);
                        ViewBag.PivotSetting = pivotSetting;
                        ViewBag.TemplateId = sysDefaultTemplate.SearchResultTemplateId;
                    }
                    else
                    {
                        ViewBag.PivotSetting = null;
                        ViewBag.TemplateId = templateId;
                    }
                }
            }

            if ((string.IsNullOrEmpty(jsonReq) || jsonReq == "null") && (searchViewModel == null || searchViewModel.IsView != true))
            {
                ViewBag.Search = null;
                return PartialView("_CustomerDataPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<CustomerDataReportSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_CustomerDataPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(CustomerDataReportSearchModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_DU_LIEU_KHACH_HANG";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}