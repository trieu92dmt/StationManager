using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class ProfileReportController : BaseController
    {
        // GET: ProfileReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index(string Type)
        {
            CreateViewBagSearch(ProfileType: Type);
            if (Type == ConstProfileType.Account)
            {
                ViewBag.Title = LanguageResource.Reports_ProfileReport;
            }
            else if (Type == ConstProfileType.Contact)
            {
                ViewBag.Title = LanguageResource.Reports_ContactReport;
            }

            var parameter = "?Type=" + Type;
            ViewBag.PageId = GetPageId("/Reports/ProfileReport", parameter);
            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion

            #region CreateRequestTime
            var currentDate = DateTime.Now;
            ViewBag.CreateRequestTimeFrom = new DateTime(currentDate.Year, currentDate.Month, 1);
            ViewBag.CreateRequestTimeTo = currentDate;
            #endregion
            return View();
        }
        [HttpPost]
        public ActionResult _PaggingServerSide(DatatableViewModel model, ProfileSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                int filteredResultsCount;
                //10
                int totalResultsCount = model.length;
                //Page Size 
                searchViewModel.PageSize = model.length;
                //Page Number
                searchViewModel.PageNumber = model.start / model.length + 1;

                searchViewModel.ProfileForeignCode = searchViewModel.SearchProfileForeignCode;
                #region //Create Date
                if (searchViewModel.CreateCommonDate != "Custom")
                {
                    DateTime? fromDate;
                    DateTime? toDate;
                    _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CreateCommonDate, out fromDate, out toDate);
                    //Tìm kiếm kỳ hiện tại
                    searchViewModel.CreateFromDate = fromDate;
                    searchViewModel.CreateToDate = toDate;
                }
                #endregion

                ProfileRepository repo = new ProfileRepository(_context);

                var profiles = repo.SearchQueryProfile(searchViewModel, CurrentUser.AccountId, CurrentUser.CompanyCode, out filteredResultsCount);
                if (profiles != null && profiles.Count() > 0)
                {
                    int i = 0;
                    foreach (var item in profiles)
                    {
                        i++;
                        item.STT = i;
                        //var ProfileRevenue = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, "");
                        //item.PreRevenue = ProfileRevenue.Where(p => p.YEARMONTH == DateTime.Now.AddYears(-1).Year.ToString()).Select(p => p.DOANHSO).FirstOrDefault();
                        //item.CurrentRevenue = ProfileRevenue.Where(p => p.YEARMONTH == DateTime.Now.Year.ToString()).Select(p => p.DOANHSO).FirstOrDefault();
                        if (item.Address.StartsWith(","))
                        {
                            item.Address = item.Address.Remove(0, 1).Trim();
                        }
                    }
                }
                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = profiles
                });
            });
        }
        #endregion

        #region CreateViewBag
        public void CreateViewBagSearch(string ProfileType = null)
        {
            var _catalogRepository = new CatalogRepository(_context);

            ViewBag.Type = ProfileType;

            #region //Get list CustomerType (Tiêu dùng, Doanh nghiệp || Liên hệ)
            var catalogList = _context.CatalogModel.Where(
                p => p.CatalogTypeCode == ConstCatalogType.CustomerType
                && p.CatalogCode != ConstCustomerType.Contact
                && p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.CustomerTypeCode = new SelectList(catalogList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Company and Store
            var companyList = _unitOfWork.CompanyRepository.GetAll(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName");

            var storeList = _unitOfWork.StoreRepository.GetAllStore(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName");
            #endregion

            #region //Get list Age (Độ tuổi)
            var ageList = _catalogRepository.GetBy(ConstCatalogType.Age);
            ViewBag.Age = new SelectList(ageList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list Province (Tỉnh/Thành phố)
            var _provinceRepository = new ProvinceRepository(_context);
            var provinceList = _provinceRepository.GetAll();
            ViewBag.ProvinceId = new SelectList(provinceList, "ProvinceId", "ProvinceName");
            ViewBag.ProvinceIdSearchList = new SelectList(provinceList, "ProvinceId", "ProvinceName");
            #endregion

            #region //Get list CustomerCareer (Ngành nghề khách hàng doanh nghiệp)
            var customerCareerList = _context.CatalogModel.Where(
                   p => p.CatalogTypeCode == ConstCatalogType.CustomerCareer
                   && p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.CustomerCareerCode = new SelectList(customerCareerList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list CustomerGroup (Nhóm khách hàng doanh nghiệp)
            var customerGroupList = _catalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.CustomerGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list SalesEmployee (NV phụ trách)
            var empList = _unitOfWork.PersonInChargeRepository.GetListEmployee();
            ViewBag.SalesEmployeeCode = new SelectList(empList, "SalesEmployeeCode", "SalesEmployeeName");
            #endregion

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.RolesModel.Where(p => p.Actived == true && p.isEmployeeGroup == true).ToList();
            ViewBag.RolesCode = new SelectList(rolesList, "RolesCode", "RolesName");
            #endregion

            #region //Get list CustomerSource (Nguồn khách hàng)
            //Get AddressType
            var srcLst = _catalogRepository.GetBy(ConstCatalogType.CustomerSource);
            ViewBag.CustomerSourceCode = new SelectList(srcLst, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list CustomerAccountGroup (Phân nhóm khách hàng)
            var customerAccountGroupLst = _catalogRepository.GetCustomerAccountGroup();
            customerAccountGroupLst.Insert(0, new CatalogViewModel()
            {
                CatalogCode = null,
                CatalogText_vi = "Chưa xác định"
            });
            ViewBag.CustomerAccountGroupCode = new SelectList(customerAccountGroupLst, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //isCreateRequest (Yêu cầu tạo khách ở ECC)
            var isCreateRequestLst = new List<ISDSelectBoolItem>();
            isCreateRequestLst.Add(new ISDSelectBoolItem()
            {
                id = null,
                name = "-- Tất cả --",
            });
            isCreateRequestLst.Add(new ISDSelectBoolItem()
            {
                id = null,
                name = "Không tạo",
            });
            isCreateRequestLst.Add(new ISDSelectBoolItem()
            {
                id = true,
                name = "Đang yêu cầu",
            });
            isCreateRequestLst.Add(new ISDSelectBoolItem()
            {
                id = false,
                name = "Đã tạo",
            });
            ViewBag.isCreateRequest = new SelectList(isCreateRequestLst, "id", "name");
            #endregion

            #region Filters
            var filterLst = new List<DropdownlistFilter>();
            //if (ProfileType == ConstProfileType.Account)
            //{
            //    //filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.SearchProfileForeignCode, FilterName = LanguageResource.Profile_ProfileForeignCode });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerTypeCode, FilterName = LanguageResource.Profile_CustomerTypeCode });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CompanyId, FilterName = LanguageResource.Profile_CompanyId });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.StoreId, FilterName = LanguageResource.MasterData_Store });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.TaxNo, FilterName = LanguageResource.Profile_TaxNo });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerAccountGroupCode, FilterName = LanguageResource.Profile_CustomerAccountGroup });
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.isCreateRequest, FilterName = LanguageResource.Profile_isCreateRequest });
            //}
            //else
            //{
            //    filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Age, FilterName = LanguageResource.Profile_Age });
            //}
            if (ProfileType == ConstProfileType.Account)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.StoreId, FilterName = LanguageResource.MasterData_Store });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.isCreateRequest, FilterName = LanguageResource.Profile_isCreateRequest });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.TaxNo, FilterName = LanguageResource.Profile_TaxNo });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerAccountGroupCode, FilterName = LanguageResource.Profile_CustomerAccountGroup });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Create, FilterName = LanguageResource.CommonCreateDate });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Actived, FilterName = LanguageResource.Actived });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Email, FilterName = LanguageResource.Email });
            }
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerTypeCode, FilterName = LanguageResource.Profile_CustomerTypeCode });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CompanyId, FilterName = LanguageResource.Profile_CompanyId });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.ProvinceId, FilterName = LanguageResource.Profile_ProvinceId });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.DistrictId, FilterName = LanguageResource.Profile_DistrictId });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.WardId, FilterName = LanguageResource.WardId });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Address, FilterName = LanguageResource.Profile_Address });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerGroupCode, FilterName = LanguageResource.Profile_CustomerCategoryCode });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerCareerCode, FilterName = LanguageResource.Profile_CustomerCareerCode });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.SalesEmployeeCode, FilterName = LanguageResource.PersonInCharge });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.RolesCode, FilterName = LanguageResource.Profile_Department });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerSourceCode, FilterName = LanguageResource.Profile_CustomerSourceCode });
           
            ViewBag.Filters = filterLst;
            #endregion
        }
        #endregion

        //Export
        #region Export to excel
        const int startIndex = 8;

        public ActionResult ExportExcel(ProfileSearchViewModel searchViewModel)
        {
            var data = new List<ProfileReportViewModel>();

            int filteredResultsCount;

            #region //Create Date
            if (searchViewModel.CreateCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CreateCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.CreateFromDate = fromDate;
                searchViewModel.CreateToDate = toDate;
            }
            #endregion
            searchViewModel.ProfileForeignCode = searchViewModel.SearchProfileForeignCode;
            searchViewModel.PageNumber = null;
            searchViewModel.PageSize = null;

            ProfileRepository repo = new ProfileRepository(_context);
            data = repo.SearchQueryProfileExport(searchViewModel, CurrentUser.AccountId, CurrentUser.CompanyCode, out filteredResultsCount);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    //var ProfileRevenue = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, "");
                    //item.PreRevenue = ProfileRevenue.Where(p => p.YEARMONTH == DateTime.Now.AddYears(-1).Year.ToString()).Select(p => p.DOANHSO).FirstOrDefault();
                    //item.CurrentRevenue = ProfileRevenue.Where(p => p.YEARMONTH == DateTime.Now.Year.ToString()).Select(p => p.DOANHSO).FirstOrDefault();

                    if (!string.IsNullOrEmpty(item.Address) && item.Address.StartsWith(","))
                    {
                        item.Address = item.Address.Remove(0, 1).Trim();
                    }
                }
            }
            return Export(data, searchViewModel.Type);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<ProfileReportViewModel> viewModel, string Type)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;

            if (Type == ConstProfileType.Account)
            {
                fileheader = "BÁO CÁO TỔNG HỢP KHÁCH HÀNG";
                #region Master
                columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileForeignCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Email", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ForeignCustomer", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerSourceName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "SaleOrgName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerTypeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerGroupName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerCareerName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "WardName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "DistrictName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProvinceName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TaxNo", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Age", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Note", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ContactCode", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactPhone", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactEmail", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactPositionName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactDepartmentName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "RoleInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CreateTime", isAllowedToEdit = false, isDateTimeTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "Actived", isAllowedToEdit = false, isBoolean = true });
                //columns.Add(new ExcelTemplate { ColumnName = "PreRevenue", isAllowedToEdit = false, isCurrency = true });
                //columns.Add(new ExcelTemplate { ColumnName = "CurrentRevenue", isAllowedToEdit = false, isCurrency = true });
                #endregion Master
            }
            else
            {
                fileheader = "BÁO CÁO TỔNG HỢP THÔNG TIN LIÊN HỆ";
                #region Master
                columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileForeignCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ForeignCustomer", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ContactCode", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactPhone", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "ContactEmail", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "PositionName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false, isDifferentColorHeader = true, ColorHeader = "#FF6347" });
                columns.Add(new ExcelTemplate { ColumnName = "PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "RoleInCharge", isAllowedToEdit = false });
                #endregion Master
            }
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export to excel
    }
}