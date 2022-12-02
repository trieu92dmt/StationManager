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
    public class ProfileOpportunityReportController : BaseController
    {
        // GET: ProfileOpportunityReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            ViewBag.Title = LanguageResource.Reports_ProfileOpportunityReport;
            var parameter = "?Type=" + ConstProfileType.Opportunity;
            ViewBag.PageId = GetPageId("/Reports/ProfileReport", parameter);

            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion

            CreateViewBagSearch(ProfileType: ConstProfileType.Opportunity);
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
                        if (!string.IsNullOrEmpty(item.Address) && item.Address.StartsWith(","))
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

            #region //Company and Store
            var companyList = _unitOfWork.CompanyRepository.GetAll(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName");

            var storeList = _unitOfWork.StoreRepository.GetAllStore(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName");
            #endregion

            #region //Get list SalesEmployee (NV phụ trách)
            var empList = _unitOfWork.PersonInChargeRepository.GetListEmployee();
            ViewBag.SalesEmployeeCode = new SelectList(empList, "SalesEmployeeCode", "SalesEmployeeName");
            #endregion

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.RolesModel.Where(p => p.Actived == true && p.isEmployeeGroup == true).ToList();
            ViewBag.RolesCode = new SelectList(rolesList, "RolesCode", "RolesName");
            #endregion

            #region //Get list SaleOffice (Khu vực)
            var saleOfficeList = _catalogRepository.GetBy(ConstCatalogType.SaleOffice);
            ViewBag.SaleOfficeCode = new SelectList(saleOfficeList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region Filters
            var filterLst = new List<DropdownlistFilter>();
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Address, FilterName = LanguageResource.ProjectLocation });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.SalesEmployeeCode, FilterName = LanguageResource.PersonInCharge });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.RolesCode, FilterName = LanguageResource.Profile_Department });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Create, FilterName = LanguageResource.CommonEditDate });
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.SaleOfficeCode, FilterName = LanguageResource.Profile_SaleOfficeCode });
            ViewBag.Filters = filterLst;
            #endregion
        }
        #endregion

        //Export
        #region Export to excel
        const int startIndex = 8;

        public ActionResult ExportExcel(ProfileSearchViewModel searchViewModel)
        {
            var data = new List<ProfileOpportunityReportViewModel>();

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
            ProfileRepository repo = new ProfileRepository(_context);
            var result = repo.SearchQueryProfileExport(searchViewModel, CurrentUser.AccountId, CurrentUser.CompanyCode, out filteredResultsCount);
            if (result != null && result.Count() > 0)
            {
                var opportunityTypeLst = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.OpportunityType).ToList();
                foreach (var item in result)
                {
                    var profile = _context.ProfileModel.Where(p => p.ProfileId == item.ProfileId).FirstOrDefault();
                    if (profile != null)
                    {
                        ////An Cường
                        //var internalLst = (from p in _context.Profile_Opportunity_InternalModel
                        //                   join c in _context.ProfileModel on p.InternalId equals c.ProfileId
                        //                   where p.ProfileId == item.ProfileId
                        //                   select c.ProfileName
                        //                   ).ToList();
                        ////Đối thủ
                        //var competitorLst = (from p in _context.Profile_Opportunity_CompetitorModel
                        //                   join c in _context.ProfileModel on p.CompetitorId equals c.ProfileId
                        //                   where p.ProfileId == item.ProfileId
                        //                   select c.ProfileName
                        //                  ).ToList();
                        //NV kinh doanh
                        var PersonInCharge = _unitOfWork.PersonInChargeRepository.List(profile.ProfileId, CurrentUser.CompanyCode, 1).Select(p => p.SalesEmployeeName).ToList();
                        string NVKD = string.Empty;
                        if (PersonInCharge != null && PersonInCharge.Count > 0)
                        {
                            NVKD = string.Join(Environment.NewLine, PersonInCharge);
                        }
                        //NV sales admin
                        var PersonInCharge2 = _unitOfWork.PersonInChargeRepository.List(profile.ProfileId, CurrentUser.CompanyCode, 2).Select(p => p.SalesEmployeeName).ToList();
                        string SalesAdmin = string.Empty;
                        if (PersonInCharge2 != null && PersonInCharge2.Count > 0)
                        {
                            SalesAdmin = string.Join(Environment.NewLine, PersonInCharge2);
                        }
                        //Chủ đầu tư
                        string Investor = string.Empty;
                        var investorLst = _unitOfWork.ProfileRepository.GetOpportunityPartner(profile.ProfileId, 1).Select(p => p.ProfileName).ToList();
                        if (investorLst != null && investorLst.Count > 0)
                        {
                            Investor = string.Join(Environment.NewLine, investorLst);
                        }
                        //Thiết kế
                        string Design = string.Empty;
                        var designLst = _unitOfWork.ProfileRepository.GetOpportunityPartner(profile.ProfileId, 2).Select(p => p.ProfileName).ToList();
                        if (designLst != null && designLst.Count > 0)
                        {
                            Design = string.Join(Environment.NewLine, designLst);
                        }
                        //Tổng thầu
                        string Contractor = string.Empty;
                        var contractorLst = _unitOfWork.ProfileRepository.GetOpportunityPartner(profile.ProfileId, 3).Select(p => p.ProfileName).ToList();
                        if (contractorLst != null && contractorLst.Count > 0)
                        {
                            Contractor = string.Join(Environment.NewLine, contractorLst);
                        }
                        //Căn mẫu
                        string Internal = string.Empty;
                        var internalLst = _unitOfWork.ProfileRepository.GetOpportunityPartner(profile.ProfileId, 4).Select(p => p.ProfileName).ToList();
                        if (internalLst != null && internalLst.Count > 0)
                        {
                            Internal = string.Join(Environment.NewLine, internalLst);
                        }
                        //Đại trà
                        string Competitor = string.Empty;
                        var competitorLst = _unitOfWork.ProfileRepository.GetOpportunityPartner(profile.ProfileId, 5).Select(p => p.ProfileName).ToList();
                        if (competitorLst != null && competitorLst.Count > 0)
                        {
                            Competitor = string.Join(Environment.NewLine, competitorLst);
                        }
                        //Nội thất bàn giao
                        string HandoverFurniture = string.Empty;
                        var hanoverFurnitureLst = _unitOfWork.ProfileRepository.GetOpportunityMaterial(profile.ProfileId, 1).ToList();
                        if (hanoverFurnitureLst != null && hanoverFurnitureLst.Count > 0)
                        {
                            HandoverFurniture = string.Join(Environment.NewLine, hanoverFurnitureLst);
                        }
                        data.Add(new ProfileOpportunityReportViewModel
                        {
                            //NV kinh doanh
                            PersonInCharge = NVKD,
                            //NV sales admin
                            SalesAdmin = SalesAdmin,
                            //Ngày cập nhật
                            LastEditTime = profile.LastEditTime.HasValue ? profile.LastEditTime : profile.CreateTime,
                            //Tên dự án
                            ProfileName = item.ProfileName,
                            //Địa điểm dự án
                            ProjectLocation = item.Address,
                            //Khu vực
                            SaleOfficeName = item.SaleOfficeName,
                            //Loại hình
                            OpportunityType = opportunityTypeLst.Where(p => p.CatalogCode == profile.Dropdownlist4).Select(p => p.CatalogText_vi).FirstOrDefault(),
                            //Quy mô
                            ProjectGabarit = profile.Text2,
                            //ĐVT
                            OpportunityUnit = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.OpportunityUnit && p.CatalogCode == profile.Dropdownlist7).Select(p => p.CatalogText_vi).FirstOrDefault(),
                            //Chủ đầu tư
                            Investor = Investor,
                            //Tư vấn thiết kế 
                            ConsultingAndDesign = Design,
                            //Tổng thầu
                            GeneralContractor = Contractor,
                            //Thi công
                            Internal = Internal,
                            Competitor = Competitor,
                            //Chỉ định vật liệu An Cường
                            Laminate = profile.Laminate,
                            MFC = profile.MFC,
                            Veneer = profile.Veneer,
                            Flooring = profile.Flooring,
                            Accessories = profile.Accessories,
                            KitchenEquipment = profile.KitchenEquipment,
                            OtherBrand = profile.OtherBrand,
                            HandoverFurniture = string.IsNullOrEmpty(HandoverFurniture) ? profile.HandoverFurniture : HandoverFurniture,
                            ProjectStatus = profile.Text3,
                            ProjectComplete = string.Format("Năm: {0} - Quý: {1}", profile.Text4, profile.Text5),
                        });
                    }

                }
            }
            return Export(data.OrderByDescending(p => p.LastEditTime).ToList(), searchViewModel.Type);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<ProfileOpportunityReportViewModel> viewModel, string Type)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;

            fileheader = "DANH SÁCH DỰ ÁN";
            #region Columns
            columns.Add(new ExcelTemplate { ColumnName = "PersonInCharge", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30 });
            columns.Add(new ExcelTemplate { ColumnName = "SalesAdmin", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30 });
            columns.Add(new ExcelTemplate { ColumnName = "LastEditTime", isAllowedToEdit = false, isDateTime = true, });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "ProjectLocation", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "OpportunityType", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "ProjectGabarit", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "OpportunityUnit", isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "Investor", isAllowedToEdit = false, isWraptext = true, CustomWidth = 60 });
            columns.Add(new ExcelTemplate { ColumnName = "ConsultingAndDesign", isAllowedToEdit = false, isWraptext = true, CustomWidth = 60 });
            columns.Add(new ExcelTemplate { ColumnName = "GeneralContractor", isAllowedToEdit = false, isWraptext = true, CustomWidth = 60 });
            columns.Add(new ExcelTemplate { ColumnName = "Internal", isAllowedToEdit = false, isWraptext = true, CustomWidth = 60, MergeHeaderTitle = "Thi công" });
            columns.Add(new ExcelTemplate { ColumnName = "Competitor", isAllowedToEdit = false, isWraptext = true, CustomWidth = 60, MergeHeaderTitle = "Thi công" });
            columns.Add(new ExcelTemplate { ColumnName = "Laminate", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "MFC", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "Veneer", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "Flooring", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "Accessories", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "KitchenEquipment", isAllowedToEdit = false, isWraptext = true, CustomWidth = 30, MergeHeaderTitle = "Chỉ định vật liệu An Cường" });
            columns.Add(new ExcelTemplate { ColumnName = "OtherBrand", isWraptext = true, CustomWidth = 30, isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "HandoverFurniture", isWraptext = true, CustomWidth = 30, isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "ProjectStatus", isWraptext = true, CustomWidth = 30, isAllowedToEdit = false, });
            columns.Add(new ExcelTemplate { ColumnName = "ProjectComplete", isAllowedToEdit = false, });
            #endregion Columns
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, headerRowMergeCount: 1);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export to excel
    }
}