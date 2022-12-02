using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Customer;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.ViewModels;
using ISD.ViewModels.Marketing;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ISD.Repositories
{
    public class ProfileRepository
    {
        EntityDataContext _context;
        public ProfileRepository(EntityDataContext db)
        {
            _context = db;
            _context.Database.CommandTimeout = 1800;
        }
        public Guid SYSTEM = new Guid("FD68F5F8-01F9-480F-ACB7-BA5D74D299C8");

        #region Search
        public List<ProfileSearchResultViewModel> Search(ProfileSearchViewModel searchModel)
        {
            //Không load All chỉ load 100 items tối ưu tốc độ
            var profiles = SearchQuery(searchModel).Take(100).ToList();

            return profiles;
        }

        public IQueryable<ProfileSearchResultViewModel> SearchQuery(ProfileSearchViewModel searchModel)
        {
            var query = (from p in _context.ProfileModel
                             //CustomerType: Bussiness, Individual Customers, Contact
                             //join c in _context.CatalogModel on p.CustomerTypeCode equals c.CatalogCode
                             //Create Account
                         join a in _context.AccountModel on p.CreateBy equals a.AccountId
                         join type in _context.ProfileTypeModel on p.ProfileId equals type.ProfileId
                         //CONTACT
                         join contact in _context.ProfileContactAttributeModel on p.ProfileId equals contact.ProfileId into cGroup
                         from cont in cGroup.DefaultIfEmpty()
                             //company
                         join company in _context.ProfileModel on cont.CompanyId equals company.ProfileId into compGroup
                         from comp in compGroup.DefaultIfEmpty()
                             //company create at
                         join cmp in _context.CompanyModel on p.CreateAtCompany equals cmp.CompanyCode into cmpGroup
                         from cm in cmpGroup.DefaultIfEmpty()
                             //Store
                         join store in _context.StoreModel on p.CreateAtSaleOrg equals store.SaleOrgCode into storeGroup
                         from st in storeGroup.DefaultIfEmpty()
                             //Province
                         join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                         from province in prG.DefaultIfEmpty()
                             //District
                         join d in _context.DistrictModel on p.DistrictId equals d.DistrictId into dG
                         from district in dG.DefaultIfEmpty()

                         orderby p.ProfileCode descending
                         where
                         //ProfileCode
                         (searchModel.ProfileCode == null || p.ProfileCode.ToString().Contains(searchModel.ProfileCode))
                         //ProfileForeignCode
                         && (searchModel.ProfileForeignCode == null || p.ProfileForeignCode.Contains(searchModel.ProfileForeignCode))
                         &&
                         (
                             //Load data by params
                             //Account
                             ((searchModel.Type == null || searchModel.Type == ConstProfileType.Account) && p.CustomerTypeCode != ConstCustomerType.Contact)
                             //Contact
                             || (searchModel.Type == ConstProfileType.Contact && p.CustomerTypeCode == ConstCustomerType.Contact)
                         )
                         //search by ProfileName || AbbreviatedName
                         && (searchModel.ProfileName == null || p.ProfileName.Contains(searchModel.ProfileName) || p.AbbreviatedName.Contains(searchModel.ProfileName))
                         //Search by Phone
                         //&& (searchModel.Phone == null || p.Phone.Contains(searchModel.Phone))
                         //Searh by Province
                         && (searchModel.ProvinceId == null || p.ProvinceId == searchModel.ProvinceId)
                         //Search by District
                         && (searchModel.DistrictId == null || p.DistrictId == searchModel.DistrictId)
                         //Search by Ward
                         && (searchModel.WardId == null || p.WardId == searchModel.WardId)
                         //search by Address
                         //&& (searchModel.Address == null || p.Address.Contains(searchModel.Address))
                         //search by catalogcode
                         && (searchModel.CustomerTypeCode == null || type.CustomerTypeCode == searchModel.CustomerTypeCode)
                         //Search by Actived
                         && (searchModel.Actived == null || p.Actived == searchModel.Actived)
                         //ProfileId
                         /* && (searchModel.ProfileId == null || cont.CompanyId == searchModel.ProfileId)*/
                         //Độ tuổi
                         && (searchModel.Age == null || p.Age == searchModel.Age)
                         && (searchModel.Email == null || p.Email.Contains(searchModel.Email))

                         select new ProfileSearchResultViewModel()
                         {
                             //Thông tin chung Account (Hoặc Contact)
                             ProfileId = p.ProfileId,
                             //Mã int tự tăng
                             ProfileCode = p.ProfileCode,
                             //Mã SAP
                             ProfileForeignCode = p.ProfileForeignCode,
                             CustomerTypeCode = p.CustomerTypeCode,
                             ProfileName = p.ProfileName,
                             Phone = p.Phone,
                             Email = p.Email,
                             Actived = p.Actived,
                             //Đối tượng
                             isForeignCustomer = p.isForeignCustomer,
                             Address = p.Address,
                             CreateTime = p.CreateTime,

                             CreateUser = a.UserName,
                             //Thông tin khách hàng (Show nếu là Contact)
                             CompanyId = comp.ProfileId,
                             CompanyName = comp.ProfileName,
                             StoreId = st.StoreId,
                             AtCompanyId = cm.CompanyId,
                             ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                             DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,

                             //Chi nhánh
                             SaleOrgName = st.StoreName,
                             //Ghi chú
                             Note = p.Note
                         });

            //1. Công ty
            if (searchModel.CompanyId != null)
            {
                query = query.Where(p => searchModel.CompanyId == p.AtCompanyId);
            }
            //2.Chi nhánh
            if (searchModel.StoreId != null && searchModel.StoreId.Count > 0 && searchModel.StoreId.First() != Guid.Empty)
            {
                query = query.Where(p => searchModel.StoreId.Contains(p.StoreId));
            }

            //Không lấy CONTACT
            if (searchModel.hasNoContact == true)
            {
                query = query.Where(p => p.CustomerTypeCode != ConstCustomerType.Contact);
            }

            //Lấy liên hệ theo khách hàng
            if (searchModel.ProfileId != null)
            {
                query = query.Where(p => p.CompanyId == searchModel.ProfileId);
            }

            //Lấy thông tin danh sách số điện thoại nếu khách hàng có nhiều hơn 1 số điện thoại => Filter trong danh sách này
            if (!string.IsNullOrEmpty(searchModel.Phone))
            {
                //Tìm số điện thoại trong bảng phụ
                var query2 = (from phone in _context.ProfilePhoneModel
                              where (phone.PhoneNumber.Contains(searchModel.Phone))
                              group phone by phone.ProfileId into g
                              select g.Key.Value);
                //Tìm số điện thoại trong bảng chính 
                var query3 = (from phone in _context.ProfileModel
                              where (phone.Phone.Contains(searchModel.Phone))
                              group phone by phone.ProfileId into g
                              select g.Key).Union(query2);

                //Join để tìm profile theo số điện thoại
                query = from profile in query
                        join profileId in query3 on profile.ProfileId equals profileId
                        select profile;

            }

            //Tìm kiếm theo địa chỉ
            //Tìm từ khóa “781 lê hồng phong” → ra “781 / c1 / c2 lê hồng phong” hoặc “781 / 25 lê hồng phong…”

            //Cách làm: Nếu Address Khách null hoặc ""
            //Tách từ khoa thành mảng[781, Lê, hồng, phong]
            //=> Tìm 4 điều kiện và and lại với nhau:
            //      Profile.Address.Containt("781") and
            //      Profile.Address.Containt("Lê") and
            //      Profile.Address.Containt("hồng") and
            //      Profile.Address.Containt("phong")

            if (!string.IsNullOrEmpty(searchModel.Address))
            {
                var addressLst = searchModel.Address.Replace(',', ' ').Replace('.', ' ').Replace("  ", " ").Split(' ');

                if (addressLst != null && addressLst.Count() > 0)
                {
                    foreach (var address in addressLst)
                    {
                        query = from profile in query
                                where (profile.Address + profile.ProvinceName + profile.DistrictName).Contains(address)
                                select profile;
                    }
                }
            }
            return query;
        }

        public List<ProfileSearchResultViewModel> SearchQueryProfileEmail(ProfileSearchViewModel searchModel, Guid? CurrentAccountId, string CurrentCompanyCode)
        {
            #region Proc
            ////Parameters for your query
            //#region CreateAtCompany
            //string CreateAtCompany = string.Empty;
            //if (searchModel.CompanyId != null)
            //{
            //    var company = _context.CompanyModel.Where(p => p.CompanyId == searchModel.CompanyId).FirstOrDefault();
            //    if (company != null)
            //    {
            //        CreateAtCompany = company.CompanyCode;
            //    }
            //}
            //#endregion CreateAtCompany

            //#region CreateAtSaleOrg
            ////Build your record
            //var tableCreateAtSaleOrgSchema = new List<SqlMetaData>(1)
            //    {
            //        new SqlMetaData("Code", SqlDbType.NVarChar, 100)
            //    }.ToArray();

            ////And a table as a list of those records
            //var tableCreateAtSaleOrg = new List<SqlDataRecord>();
            //List<string> saleOrgLst = new List<string>();
            //if (searchModel.StoreId != null && searchModel.StoreId.Count > 0)
            //{
            //    foreach (var r in searchModel.StoreId)
            //    {
            //        var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
            //        var store = _context.StoreModel.Where(p => p.StoreId == r.Value).FirstOrDefault();
            //        if (store != null)
            //        {
            //            tableRow.SetString(0, store.SaleOrgCode);
            //            if (!saleOrgLst.Contains(store.SaleOrgCode))
            //            {
            //                saleOrgLst.Add(store.SaleOrgCode);
            //                tableCreateAtSaleOrg.Add(tableRow);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
            //    tableCreateAtSaleOrg.Add(tableRow);
            //}
            //#endregion

            //#region Address
            //string Address = string.Empty;
            //if (!string.IsNullOrEmpty(searchModel.Address))
            //{
            //    Address = searchModel.Address.Replace(',', ' ').Replace('.', ' ').Replace("  ", " ").Replace(" ", "%");
            //}
            //#endregion Address

            //string sqlQuery = "EXEC [Marketing].[SearchProfile_Email] @Type, @ProfileCode, @ProfileForeignCode, @CustomerTypeCode, @CreateAtCompany, @CreateAtSaleOrg, @Age, @ProfileName, @Phone, @ProvinceId, @DistrictId, @WardId, @Address, @Actived, @ProfileId, @CustomerGroupCode, @CustomerCareerCode, @SalesEmployeeCode, @RolesCode, @CreateFromDate, @CreateToDate, @CreateByCode, @CustomerSourceCode, @CurrentCompanyCode, @TaxNo, @AccountId";
            //#region Parameters
            //List<SqlParameter> parameters = new List<SqlParameter>()
            //{
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "Type",
            //        Value = searchModel.Type,
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "ProfileCode",
            //        Value = searchModel.ProfileCode ?? (object)DBNull.Value,
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "ProfileForeignCode",
            //        Value = searchModel.ProfileForeignCode ?? (object)DBNull.Value,
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CustomerTypeCode",
            //        Value = searchModel.CustomerTypeCode ?? (object)DBNull.Value,
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CreateAtCompany",
            //        Value = CreateAtCompany ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.Structured,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CreateAtSaleOrg",
            //        TypeName = "[dbo].[StringList]", //Don't forget this one!
            //        Value = tableCreateAtSaleOrg
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "Age",
            //        Value = searchModel.Age ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "ProfileName",
            //        Value = searchModel.ProfileName ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "Phone",
            //        Value = searchModel.Phone ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.UniqueIdentifier,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "ProvinceId",
            //        Value = searchModel.ProvinceId ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.UniqueIdentifier,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "DistrictId",
            //        Value = searchModel.DistrictId ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.UniqueIdentifier,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "WardId",
            //        Value = searchModel.WardId ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "Address",
            //        Value = Address ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.Bit,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "Actived",
            //        Value = searchModel.Actived ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.UniqueIdentifier,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "ProfileId",
            //        Value = searchModel.ProfileId ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CustomerGroupCode",
            //        Value = searchModel.CustomerGroupCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CustomerCareerCode",
            //        Value = searchModel.CustomerCareerCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "SalesEmployeeCode",
            //        Value = searchModel.SalesEmployeeCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "RolesCode",
            //        Value = searchModel.RolesCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.DateTime,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CreateFromDate",
            //        Value = searchModel.CreateFromDate ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.DateTime,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CreateToDate",
            //        Value = searchModel.CreateToDate ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CreateByCode",
            //        Value = searchModel.CreateByCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CustomerSourceCode",
            //        Value = searchModel.CustomerSourceCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "CurrentCompanyCode",
            //        Value = CurrentCompanyCode ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.NVarChar,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "TaxNo",
            //        Value = searchModel.TaxNo ?? (object)DBNull.Value
            //    },
            //    new SqlParameter
            //    {
            //        SqlDbType = SqlDbType.UniqueIdentifier,
            //        Direction = ParameterDirection.Input,
            //        ParameterName = "AccountId",
            //        Value = CurrentAccountId ?? (object)DBNull.Value
            //    }
            //    //,
            //    //new SqlParameter
            //    //{
            //    //    SqlDbType = SqlDbType.NVarChar,
            //    //    Direction = ParameterDirection.Input,
            //    //    ParameterName = "Email",
            //    //    Value = searchModel.Email ?? (object)DBNull.Value
            //    //}
            //};
            //#endregion

            //var result = _context.Database.SqlQuery<ProfileSearchResultViewModel>(sqlQuery, parameters.ToArray()).ToList();
            //return result;
            #endregion
            searchModel.PageNumber = 1;
            searchModel.PageSize = 500000;
            int count = 0;
            var listAccount = SearchQueryProfile(searchModel, CurrentAccountId, CurrentCompanyCode, out count).Select(s => new ProfileSearchResultViewModel
            {
                ProfileId = s.ProfileId,
                ProfileName = s.ProfileName,
                Email = s.Email
            }).ToList();
            var listContact = (from profile in listAccount
                               join contact in _context.ProfileContactAttributeModel on profile.ProfileId equals contact.CompanyId
                               join p2 in _context.ProfileModel on contact.ProfileId equals p2.ProfileId
                               where p2.Email != null && p2.Email != ""
                               select new ProfileSearchResultViewModel
                               {
                                   ProfileId = p2.ProfileId,
                                   ProfileName = p2.ProfileName,
                                   Email = p2.Email
                               }).ToList();
            listAccount.AddRange(listContact);
            return listAccount.Where(s => s.Email != null && s.Email != "").ToList();
        }

        public List<ProfileSearchResultViewModel> SearchQueryProfile(ProfileSearchViewModel searchModel, Guid? CurrentAccountId, string CurrentCompanyCode, out int filteredResultsCount)
        {
            //Parameters for your query
            #region CreateAtCompany
            string CreateAtCompany = string.Empty;
            if (searchModel.CompanyId != null)
            {
                var company = _context.CompanyModel.Where(p => p.CompanyId == searchModel.CompanyId).FirstOrDefault();
                if (company != null)
                {
                    CreateAtCompany = company.CompanyCode;
                }
            }
            #endregion CreateAtCompany

            #region CreateAtSaleOrg
            //Build your record
            var tableCreateAtSaleOrgSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableCreateAtSaleOrg = new List<SqlDataRecord>();
            List<string> saleOrgLst = new List<string>();
            if (searchModel.StoreId != null && searchModel.StoreId.Count > 0)
            {
                foreach (var r in searchModel.StoreId)
                {
                    var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                    var store = _context.StoreModel.Where(p => p.StoreId == r.Value).FirstOrDefault();
                    if (store != null)
                    {
                        tableRow.SetString(0, store.SaleOrgCode);
                        if (!saleOrgLst.Contains(store.SaleOrgCode))
                        {
                            saleOrgLst.Add(store.SaleOrgCode);
                            tableCreateAtSaleOrg.Add(tableRow);
                        }
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                tableCreateAtSaleOrg.Add(tableRow);
            }
            #endregion

            #region Address
            string Address = string.Empty;
            if (!string.IsNullOrEmpty(searchModel.Address))
            {
                Address = searchModel.Address.Replace(',', ' ').Replace('.', ' ').Replace("  ", " ").Replace(" ", "%");
            }
            #endregion Address

            #region CustomerAccountGroup
            //Build your record
            var tableCustomerAccountGroupSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableCustomerAccountGroup = new List<SqlDataRecord>();
            List<string> customerAccountGroupLst = new List<string>();
            if (searchModel.CustomerAccountGroupCode != null && searchModel.CustomerAccountGroupCode.Count > 0)
            {
                foreach (var r in searchModel.CustomerAccountGroupCode)
                {
                    var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                    tableRow.SetString(0, r);
                    if (!customerAccountGroupLst.Contains(r))
                    {
                        customerAccountGroupLst.Add(r);
                        tableCustomerAccountGroup.Add(tableRow);
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableCustomerAccountGroupSchema);
                tableCustomerAccountGroup.Add(tableRow);
            }
            #endregion

            string sqlQuery = "EXEC [Customer].[usp_SearchProfile_Dynamic] @Type, @ProfileCode, @ProfileForeignCode, @CustomerTypeCode, @CreateAtCompany, @CreateAtSaleOrg, @Age, @ProfileName, @Phone, @ProvinceId, @DistrictId, @WardId, @Address, @Actived, @ProfileId, @CustomerGroupCode, @CustomerCareerCode, @SalesEmployeeCode, @RolesCode, @CreateFromDate, @CreateToDate, @CreateByCode, @CustomerSourceCode, @CurrentCompanyCode, @TaxNo, @AccountId, @PageSize, @PageNumber, @FilteredResultsCount OUT, @Email, @EmailIsNull, @TaxNoIsNull, @SearchProfileForeignCodeIsNull, @AddressIsNull, @CustomerSourceCodeIsNull, @ProvinceIdIsNull, @DistrictIdIsNull, @WardIdIsNull, @CustomerTypeCodeIsNull, @CustomerGroupCodeIsNull, @CustomerCareerCodeIsNull, @SalesEmployeeCodeIsNull, @CustomerAccountGroupCode, @isCreateRequest, @CreateRequestAll, @CreateRequestTimeFrom, @CreateRequestTimeTo, @CustomerAccountGroupAll, @SaleOfficeCode";
            var FilteredResultsCountOutParam = new SqlParameter();
            FilteredResultsCountOutParam.ParameterName = "FilteredResultsCount";
            FilteredResultsCountOutParam.SqlDbType = SqlDbType.Int;
            FilteredResultsCountOutParam.Direction = ParameterDirection.Output;

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Type",
                    Value = searchModel.Type,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileCode",
                    Value = searchModel.ProfileCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileForeignCode",
                    Value = searchModel.ProfileForeignCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerTypeCode",
                    Value = searchModel.CustomerTypeCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateAtCompany",
                    Value = CreateAtCompany ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateAtSaleOrg",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableCreateAtSaleOrg
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Age",
                    Value = searchModel.Age ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileName",
                    Value = searchModel.ProfileName ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Phone",
                    Value = searchModel.Phone ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProvinceId",
                    Value = searchModel.ProvinceId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DistrictId",
                    Value = searchModel.DistrictId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WardId",
                    Value = searchModel.WardId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Address",
                    Value = Address ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Actived",
                    Value = searchModel.Actived ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileId",
                    Value = searchModel.ProfileId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerGroupCode",
                    Value = searchModel.CustomerGroupCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerCareerCode",
                    Value = searchModel.CustomerCareerCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SalesEmployeeCode",
                    Value = searchModel.SalesEmployeeCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "RolesCode",
                    Value = searchModel.RolesCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateFromDate",
                    Value = searchModel.CreateFromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateToDate",
                    Value = searchModel.CreateToDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateByCode",
                    Value = searchModel.CreateByCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerSourceCode",
                    Value = searchModel.CustomerSourceCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentCompanyCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "TaxNo",
                    Value = searchModel.TaxNo ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "AccountId",
                    Value = CurrentAccountId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageSize",
                    Value = searchModel.PageSize ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageNumber",
                    Value = searchModel.PageNumber ?? (object)DBNull.Value,
                },
                FilteredResultsCountOutParam,
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Email",
                    Value = searchModel.Email ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EmailIsNull",
                    Value = searchModel.EmailIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "TaxNoIsNull",
                    Value = searchModel.TaxNoIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SearchProfileForeignCodeIsNull",
                    Value = searchModel.SearchProfileForeignCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "AddressIsNull",
                    Value = searchModel.AddressIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerSourceCodeIsNull",
                    Value = searchModel.CustomerSourceCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProvinceIdIsNull",
                    Value = searchModel.ProvinceIdIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DistrictIdIsNull",
                    Value = searchModel.DistrictIdIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WardIdIsNull",
                    Value = searchModel.WardIdIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerTypeCodeIsNull",
                    Value = searchModel.CustomerTypeCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerGroupCodeIsNull",
                    Value = searchModel.CustomerGroupCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerCareerCodeIsNull",
                    Value = searchModel.CustomerCareerCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SalesEmployeeCodeIsNull",
                    Value = searchModel.SalesEmployeeCodeIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerAccountGroupCode",
                    TypeName = "[dbo].[StringList]",
                    Value = tableCustomerAccountGroup
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "isCreateRequest",
                    Value = searchModel.isCreateRequest ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateRequestAll",
                    Value = searchModel.CreateRequestAll ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateRequestTimeFrom",
                    Value = searchModel.CreateRequestTimeFrom ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreateRequestTimeTo",
                    Value = searchModel.CreateRequestTimeTo ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerAccountGroupAll",
                    Value = searchModel.CustomerAccountGroupAll ?? (object)DBNull.Value
                },
                  new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SaleOfficeCode",
                    Value = searchModel.SaleOfficeCode ?? (object)DBNull.Value
                },
            };
            #endregion

            var result = _context.Database.SqlQuery<ProfileSearchResultViewModel>(sqlQuery, parameters.ToArray()).ToList();
            var filteredResultsCountValue = FilteredResultsCountOutParam.Value;
            if (filteredResultsCountValue != null)
            {
                filteredResultsCount = Convert.ToInt32(filteredResultsCountValue);
            }
            else
            {
                filteredResultsCount = 0;
            }
            return result;
        }

        public List<ProfileSearchResultViewModel> SearchQueryContacDeleted(ProfileSearchViewModel searchModel, Guid? CurrentAccountId, string CurrentCompanyCode, out int filteredResultsCount)
        {

            string sqlQuery = "EXEC Customer.usp_SearchContactDeleted @CurrentCompanyCode, @ProfileCode, @ProfileName, @Phone, @ProfileId, @AccountId, @Email, @EmailIsNull, @PageSize, @PageNumber, @FilteredResultsCount OUTPUT";
            var FilteredResultsCountOutParam = new SqlParameter();
            FilteredResultsCountOutParam.ParameterName = "FilteredResultsCount";
            FilteredResultsCountOutParam.SqlDbType = SqlDbType.Int;
            FilteredResultsCountOutParam.Direction = ParameterDirection.Output;

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {

                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentCompanyCode ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileCode",
                    Value = searchModel.ProfileCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileName",
                    Value = searchModel.ProfileName ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Phone",
                    Value = searchModel.Phone ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ProfileId",
                    Value = searchModel.ProfileId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    ParameterName = "AccountId",
                    Value = CurrentAccountId ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "Email",
                    Value = searchModel.Email ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    ParameterName = "EmailIsNull",
                    Value = searchModel.EmailIsNull ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageSize",
                    Value = searchModel.PageSize,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PageNumber",
                    Value = searchModel.PageNumber,
                },
                FilteredResultsCountOutParam
            };
            #endregion

            var result = _context.Database.SqlQuery<ProfileSearchResultViewModel>(sqlQuery, parameters.ToArray()).ToList();
            var filteredResultsCountValue = FilteredResultsCountOutParam.Value;
            if (filteredResultsCountValue != null)
            {
                filteredResultsCount = Convert.ToInt32(filteredResultsCountValue);
            }
            else
            {
                filteredResultsCount = 0;
            }
            return result;
        }
        #endregion Search

        #region Export profile
        public List<ProfileReportViewModel> SearchQueryProfileExport(ProfileSearchViewModel searchModel, Guid? CurrentAccountId, string CurrentCompanyCode, out int filteredResultsCount)
        {
            var result = new List<ProfileReportViewModel>();
            //Parameters for your query
            #region CreateAtCompany
            string CreateAtCompany = string.Empty;
            if (searchModel.CompanyId != null)
            {
                var company = _context.CompanyModel.Where(p => p.CompanyId == searchModel.CompanyId).FirstOrDefault();
                if (company != null)
                {
                    CreateAtCompany = company.CompanyCode;
                }
            }
            #endregion CreateAtCompany

            #region CreateAtSaleOrg
            //Build your record
            var tableCreateAtSaleOrgSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableCreateAtSaleOrg = new List<SqlDataRecord>();
            List<string> saleOrgLst = new List<string>();
            if (searchModel.StoreId != null && searchModel.StoreId.Count > 0)
            {
                foreach (var r in searchModel.StoreId)
                {
                    var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                    var store = _context.StoreModel.Where(p => p.StoreId == r.Value).FirstOrDefault();
                    if (store != null)
                    {
                        tableRow.SetString(0, store.SaleOrgCode);
                        if (!saleOrgLst.Contains(store.SaleOrgCode))
                        {
                            saleOrgLst.Add(store.SaleOrgCode);
                            tableCreateAtSaleOrg.Add(tableRow);
                        }
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                tableCreateAtSaleOrg.Add(tableRow);
            }
            #endregion

            #region Address
            string Address = string.Empty;
            if (!string.IsNullOrEmpty(searchModel.Address))
            {
                Address = searchModel.Address.Replace(',', ' ').Replace('.', ' ').Replace("  ", " ").Replace(" ", "%");
            }
            #endregion Address

            #region CustomerAccountGroup
            //Build your record
            var tableCustomerAccountGroupSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableCustomerAccountGroup = new List<SqlDataRecord>();
            List<string> customerAccountGroupLst = new List<string>();
            if (searchModel.CustomerAccountGroupCode != null && searchModel.CustomerAccountGroupCode.Count > 0)
            {
                foreach (var r in searchModel.CustomerAccountGroupCode)
                {
                    var tableRow = new SqlDataRecord(tableCreateAtSaleOrgSchema);
                    tableRow.SetString(0, r);
                    if (!customerAccountGroupLst.Contains(r))
                    {
                        customerAccountGroupLst.Add(r);
                        tableCustomerAccountGroup.Add(tableRow);
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableCustomerAccountGroupSchema);
                tableCustomerAccountGroup.Add(tableRow);
            }
            #endregion

            int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Customer].[usp_SearchProfile_Dynamic]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@Type", searchModel.Type);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileCode", searchModel.ProfileCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileForeignCode", searchModel.ProfileForeignCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerTypeCode", searchModel.CustomerTypeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateAtCompany", CreateAtCompany ?? (object)DBNull.Value);
                        var table = sda.SelectCommand.Parameters.AddWithValue("@CreateAtSaleOrg", tableCreateAtSaleOrg);
                        table.SqlDbType = SqlDbType.Structured;
                        table.TypeName = "[dbo].[StringList]";
                        sda.SelectCommand.Parameters.AddWithValue("@Age", searchModel.Age ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileName", searchModel.ProfileName ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Phone", searchModel.Phone ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProvinceId", searchModel.ProvinceId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DistrictId", searchModel.DistrictId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@WardId", searchModel.WardId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Address", Address ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Actived", searchModel.Actived ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.ProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerGroupCode", searchModel.CustomerGroupCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerCareerCode", searchModel.CustomerCareerCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", searchModel.SalesEmployeeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@RolesCode", searchModel.RolesCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateFromDate", searchModel.CreateFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateToDate", searchModel.CreateToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateByCode", searchModel.CreateByCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerSourceCode", searchModel.CustomerSourceCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CurrentCompanyCode", CurrentCompanyCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@TaxNo", searchModel.TaxNo ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@TaxNoIsNull", searchModel.TaxNoIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@AccountId", CurrentAccountId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@PageSize", searchModel.PageSize == 0 || searchModel.PageSize == null ? (object)DBNull.Value : searchModel.PageSize);
                        sda.SelectCommand.Parameters.AddWithValue("@PageNumber", searchModel.PageNumber == 0 || searchModel.PageNumber == null ? (object)DBNull.Value : searchModel.PageNumber);
                        var output = sda.SelectCommand.Parameters.AddWithValue("@FilteredResultsCount", FilteredResultsCount);
                        output.Direction = ParameterDirection.Output;
                        sda.SelectCommand.Parameters.AddWithValue("@Email", searchModel.Email ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EmailIsNull", searchModel.EmailIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SearchProfileForeignCodeIsNull", searchModel.SearchProfileForeignCodeIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@AddressIsNull", searchModel.AddressIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerSourceCodeIsNull", searchModel.CustomerSourceCodeIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProvinceIdIsNull", searchModel.ProvinceIdIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DistrictIdIsNull", searchModel.DistrictIdIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@WardIdIsNull", searchModel.WardIdIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerTypeCodeIsNull", searchModel.CustomerTypeCodeIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerGroupCodeIsNull", searchModel.CustomerGroupCodeIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerCareerCodeIsNull", searchModel.CustomerCareerCodeIsNull ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCodeIsNull", searchModel.SalesEmployeeCodeIsNull ?? (object)DBNull.Value);
                        var table2 = sda.SelectCommand.Parameters.AddWithValue("@CustomerAccountGroupCode", tableCustomerAccountGroup);
                        table2.SqlDbType = SqlDbType.Structured;
                        table2.TypeName = "[dbo].[StringList]";
                        sda.SelectCommand.Parameters.AddWithValue("@isCreateRequest", searchModel.isCreateRequest ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateRequestAll", searchModel.CreateRequestAll ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateRequestTimeFrom", searchModel.CreateRequestTimeFrom ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateRequestTimeTo", searchModel.CreateRequestTimeTo ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CustomerAccountGroupAll", searchModel.CustomerAccountGroupAll ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SaleOfficeCode", searchModel.SaleOfficeCode ?? (object)DBNull.Value);
                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                ProfileReportViewModel model = new ProfileReportViewModel();
                                #region Convert to list
                                if (searchModel.Type == ConstProfileType.Account)
                                {
                                    if (!string.IsNullOrEmpty(item["ProfileId"].ToString()))
                                    {
                                        model.ProfileId = Guid.Parse(item["ProfileId"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(item["ProfileCode"].ToString()))
                                    {
                                        model.ProfileCode = Convert.ToInt32(item["ProfileCode"].ToString());
                                    }
                                    model.ProfileForeignCode = item["ProfileForeignCode"].ToString();
                                    model.ProfileName = item["ProfileName"].ToString();
                                    model.ForeignCustomer = item["ForeignCustomer"].ToString();
                                    model.Address = item["Address"].ToString();
                                    model.Phone = item["Phone"].ToString();
                                    model.Email = item["Email"].ToString();
                                    model.CustomerSourceName = item["CustomerSourceName"].ToString();
                                    model.SaleOrgName = item["SaleOrgName"].ToString();
                                    model.CustomerTypeName = item["CustomerTypeName"].ToString();
                                    model.CustomerGroupName = item["CustomerGroupName"].ToString();
                                    model.CustomerCareerName = item["CustomerCareerName"].ToString();
                                    model.WardName = item["WardName"].ToString();
                                    model.DistrictName = item["DistrictName"].ToString();
                                    model.ProvinceName = item["ProvinceName"].ToString();
                                    model.SaleOfficeName = item["SaleOfficeName"].ToString();
                                    model.TaxNo = item["TaxNo"].ToString();
                                    model.Age = item["Age"].ToString();
                                    model.Note = item["Note"].ToString();
                                    model.ContactCode = item["ContactCode"].ToString();
                                    model.ContactName = item["ContactName"].ToString();
                                    model.ContactPhone = item["ContactPhone"].ToString();
                                    model.ContactEmail = item["ContactEmail"].ToString();
                                    model.ContactPositionName = item["ContactPositionName"].ToString();
                                    model.ContactDepartmentName = item["ContactDepartmentName"].ToString();
                                    model.CreateTime = Convert.ToDateTime(item["CreateTime"].ToString());

                                    UtilitiesRepository _utilitiesRepository = new UtilitiesRepository();
                                    if (!string.IsNullOrEmpty(item["Actived"].ToString()))
                                    {
                                        model.Actived = _utilitiesRepository.ParseBool(item["Actived"].ToString());
                                    }

                                }
                                else if (searchModel.Type == ConstProfileType.Contact)
                                {
                                    if (!string.IsNullOrEmpty(item["CompanyProfileCode"].ToString()))
                                    {
                                        model.ProfileCode = Convert.ToInt32(item["CompanyProfileCode"].ToString());
                                    }
                                    model.ProfileForeignCode = item["CompanyProfileForeignCode"].ToString();
                                    model.ProfileName = item["CompanyName"].ToString();
                                    model.ForeignCustomer = item["CompanyForeignCustomer"].ToString();
                                    model.ContactCode = item["ProfileCode"].ToString();
                                    model.ContactName = item["ProfileName"].ToString();
                                    model.ContactPhone = item["Phone"].ToString();
                                    model.ContactEmail = item["Email"].ToString();
                                    model.PositionName = item["PositionName"].ToString();
                                    model.DepartmentName = item["DepartmentName"].ToString();
                                }
                                else if (searchModel.Type == ConstProfileType.Opportunity)
                                {
                                    model.ProfileId = Guid.Parse(item["ProfileId"].ToString());
                                    model.ProfileName = item["ProfileName"].ToString();
                                    model.Address = item["Address"].ToString();
                                    model.SaleOfficeName = item["SaleOfficeName"].ToString();
                                }
                                model.PersonInCharge = item["PersonInCharge"].ToString();
                                model.RoleInCharge = item["RoleInCharge"].ToString();
                                #endregion
                                result.Add(model);
                            }
                        }
                    }
                }
            }
            filteredResultsCount = FilteredResultsCount;
            return result;
        }
        #endregion

        public List<ProfileViewModel> GetProfiles()
        {
            var profileList = _context.ProfileModel.Where(p => p.Actived == true && p.CustomerTypeCode != ConstCustomerType.Contact)
                .Select(p => new ProfileViewModel
                {
                    ProfileId = p.ProfileId,
                    ProfileCode = p.ProfileCode,
                    ProfileForeignCode = p.ProfileForeignCode,
                    CustomerTypeCode = p.CustomerTypeCode,
                    Title = p.Title,
                    ProfileName = p.ProfileName,
                    ProfileShortName = p.ProfileShortName,
                    Phone = p.Phone,
                    Age = p.Age,
                    Email = p.Email,
                    SaleOfficeCode = p.SaleOfficeCode,
                    Address = p.Address,
                    ProvinceId = p.ProvinceId,
                    DistrictId = p.DistrictId,
                    WardId = p.WardId,
                    Note = p.Note,
                    VisitDate = p.VisitDate,
                    ImageUrl = p.ImageUrl,
                    CreateByEmployee = p.CreateByEmployee,
                    CreateAtCompany = p.CreateAtCompany,
                    CreateAtSaleOrg = p.CreateAtSaleOrg,
                    CreateBy = p.CreateBy,
                    CreateTime = p.CreateTime,
                    LastEditBy = p.LastEditBy,
                    LastEditTime = p.LastEditTime,
                    AbbreviatedName = p.AbbreviatedName,
                    MonthOfBirth = p.MonthOfBirth,
                    YearOfBirth = p.YearOfBirth,
                    isForeignCustomer = p.isForeignCustomer,
                }).ToList();
            return profileList;
        }

        public List<ProfileViewModel> GetContacts()
        {
            var contactLst = _context.ProfileModel.Where(p => p.Actived == true && p.CustomerTypeCode == ConstCustomerType.Contact)
                .Select(p => new ProfileViewModel
                {
                    ProfileId = p.ProfileId,
                    ProfileCode = p.ProfileCode,
                    ProfileForeignCode = p.ProfileForeignCode,
                    CustomerTypeCode = p.CustomerTypeCode,
                    Title = p.Title,
                    ProfileName = p.ProfileName,
                    ProfileShortName = p.ProfileShortName,
                    Phone = p.Phone,
                    Age = p.Age,
                    Email = p.Email,
                    SaleOfficeCode = p.SaleOfficeCode,
                    Address = p.Address,
                    ProvinceId = p.ProvinceId,
                    DistrictId = p.DistrictId,
                    WardId = p.WardId,
                    Note = p.Note,
                    VisitDate = p.VisitDate,
                    ImageUrl = p.ImageUrl,
                    CreateByEmployee = p.CreateByEmployee,
                    CreateAtCompany = p.CreateAtCompany,
                    CreateAtSaleOrg = p.CreateAtSaleOrg,
                    CreateBy = p.CreateBy,
                    CreateTime = p.CreateTime,
                    LastEditBy = p.LastEditBy,
                    LastEditTime = p.LastEditTime,
                    AbbreviatedName = p.AbbreviatedName,
                    MonthOfBirth = p.MonthOfBirth,
                    YearOfBirth = p.YearOfBirth,
                    isForeignCustomer = p.isForeignCustomer,
                }).ToList();
            return contactLst;
        }

        public ProfileViewModel GetById(Guid profileid)
        {
            var ret = (from p in _context.ProfileModel
                       join p1 in _context.ProvinceModel on p.ProvinceId equals p1.ProvinceId into TmpList1
                       from prov in TmpList1.DefaultIfEmpty()
                       join p2 in _context.DistrictModel on p.DistrictId equals p2.DistrictId into TmpList2
                       from dist in TmpList2.DefaultIfEmpty()
                       join p3 in _context.WardModel on p.WardId equals p3.WardId into TmpList3
                       from ward in TmpList3.DefaultIfEmpty()
                       where p.ProfileId == profileid
                       select new ProfileViewModel()
                       {
                           ProfileId = p.ProfileId,
                           ProfileCode = p.ProfileCode,
                           ProfileForeignCode = p.ProfileForeignCode,
                           CustomerTypeCode = p.CustomerTypeCode,
                           Title = p.Title,
                           ProfileName = p.ProfileName,
                           ProfileShortName = p.ProfileShortName,
                           Phone = p.Phone,
                           Age = p.Age,
                           Email = p.Email,
                           SaleOfficeCode = p.SaleOfficeCode,
                           Address = p.Address,
                           ProvinceId = p.ProvinceId,
                           ProvinceName = prov.ProvinceName,
                           DistrictId = p.DistrictId,
                           DistrictName = dist.Appellation + " " + dist.DistrictName,
                           WardId = p.WardId,
                           WardName = ward.Appellation + " " + ward.WardName,
                           Note = p.Note,
                           VisitDate = p.VisitDate,
                           ImageUrl = p.ImageUrl,
                           CreateByEmployee = p.CreateByEmployee,
                           CreateAtCompany = p.CreateAtCompany,
                           CreateAtSaleOrg = p.CreateAtSaleOrg,
                           CreateBy = p.CreateBy,
                           CreateTime = p.CreateTime,
                           LastEditBy = p.LastEditBy,
                           LastEditTime = p.LastEditTime,
                           AbbreviatedName = p.AbbreviatedName,
                           MonthOfBirth = p.MonthOfBirth,
                           YearOfBirth = p.YearOfBirth,
                           isForeignCustomer = p.isForeignCustomer,
                       }).FirstOrDefault();
            return ret;
        }

        public string GetProfileNameBy(Guid? ProfileId)
        {
            return _context.ProfileModel.Where(p => p.ProfileId == ProfileId).Select(p => p.ProfileName).FirstOrDefault();
        }

        /// <summary>
        /// Lấy tên ngắn khách hàng
        /// Nếu không có tên ngắn lấy tên đầy đủ
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public string GetProfileShortNameBy(Guid? ProfileId)
        {
            var name = _context.ProfileModel.Where(p => p.ProfileId == ProfileId).Select(p => p.ProfileShortName).FirstOrDefault();
            if (string.IsNullOrEmpty(name))
            {
                name = _context.ProfileModel.Where(p => p.ProfileId == ProfileId).Select(p => p.ProfileName).FirstOrDefault();
            }
            return name;
        }

        public List<ProfileContactViewModel> GetContactListOfProfile(Guid? ProfileId)
        {
            var contactList = (from a in _context.ProfileModel
                               join v in _context.ProfileContactAttributeModel on a.ProfileId equals v.ProfileId
                               join p in _context.ProfileModel on v.CompanyId equals p.ProfileId
                               //Position
                               join po in _context.CatalogModel on new { Position = v.Position, Type = ConstCatalogType.Position } equals new { Position = po.CatalogCode, Type = po.CatalogTypeCode } into poTemp
                               from position in poTemp.DefaultIfEmpty()
                                   //Department
                               join d in _context.CatalogModel on new { Department = v.DepartmentCode, Type = ConstCatalogType.Department } equals new { Department = d.CatalogCode, Type = d.CatalogTypeCode } into dTemp
                               from department in dTemp.DefaultIfEmpty()
                               where v.CompanyId == ProfileId && p.Actived == true
                               orderby v.IsMain descending, a.CreateTime descending
                               select new ProfileContactViewModel
                               {
                                   ProfileContactId = a.ProfileId,
                                   ProfileContactName = a.ProfileCode + " | " + a.ProfileName + (position != null ? " | " + position.CatalogText_vi : ""),
                                   ProfileContactPhone = a.Phone,
                                   ProfileContactEmail = a.Email,
                                   ProfileContactAddress = a.Address,
                                   ProfileContactPosition = v.Position,
                                   ProfileContactDepartment = v.DepartmentCode,
                                   ProfileContactPositionName = position.CatalogText_vi,
                                   ProfileContactDepartmentName = department.CatalogText_vi,
                                   IsMain = v.IsMain,
                                   ProfileContactFullName = a.ProfileName,
                                   ProfileContactCompanyName = p.ProfileName
                               }).ToList();
            return contactList;
        }

        public ProfileModel Create(ProfileViewModel profileViewModel)
        {
            var profileNew = new ProfileModel();

            profileNew.MapProfile(profileViewModel);
            profileNew.CreateBy = profileViewModel.CreateBy;
            profileNew.CreateTime = profileViewModel.CreateTime;

            _context.Entry(profileNew).State = EntityState.Added;
            return profileNew;
        }
        public bool Update(ProfileViewModel profileViewModel)
        {
            var profileInDb = _context.ProfileModel.FirstOrDefault(p => p.ProfileId == profileViewModel.ProfileId);
            if (profileInDb != null)
            {
                profileInDb.MapProfile(profileViewModel);
                profileInDb.LastEditBy = profileViewModel.LastEditBy;
                profileInDb.LastEditTime = profileViewModel.LastEditTime;
                _context.Entry(profileInDb).State = EntityState.Modified;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateProfileB(ProfileViewModel profileViewModel)
        {
            var bAttAdd = new ProfileBAttributeModel();
            //1. GUID
            bAttAdd.ProfileId = profileViewModel.ProfileId;
            //2. Mã số thuế
            bAttAdd.TaxNo = profileViewModel.TaxNo;
            //3. Người liên hệ
            bAttAdd.ContactName = profileViewModel.ContactName;
            //4. Vị trí
            bAttAdd.Position = profileViewModel.PositionB;
            //5. Ngành nghề
            bAttAdd.CustomerCareerCode = profileViewModel.CustomerCareerCode;
            //6. Website
            bAttAdd.Website = profileViewModel.Website;
            //7. Số điện thoại công ty
            bAttAdd.CompanyNumber = profileViewModel.CompanyNumber;

            _context.Entry(bAttAdd).State = EntityState.Added;
        }

        #region Search mobile
        public List<MobileProfileSearchResultViewModel> MobileSearch(MobileProfileSearchViewModel searchModel)
        {
            //Không load All chỉ load 50 items tối ưu tốc độ
            var profiles = MobileSearchQuery(searchModel).Take(50).ToList();
            return profiles;
        }

        public List<MobileProfileSearchResultViewModel> MobileSearchQuery(MobileProfileSearchViewModel searchModel)
        {
            var profiles = (from p in _context.ProfileModel
                                //Create Account
                            join a in _context.AccountModel on p.CreateBy equals a.AccountId
                            //Province
                            join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                            from province in prG.DefaultIfEmpty()
                                //District
                            join d in _context.DistrictModel on p.DistrictId equals d.DistrictId into dG
                            from district in dG.DefaultIfEmpty()
                                //Ward
                            join w in _context.WardModel on p.WardId equals w.WardId into wG
                            from ward in wG.DefaultIfEmpty()
                                //Create By Employee
                            join s in _context.SalesEmployeeModel on p.CreateByEmployee equals s.SalesEmployeeCode into sG
                            from employee in sG.DefaultIfEmpty()
                                //Profile Group
                            let profileGroup = _context.ProfileGroupModel.Where(p2 => p2.ProfileId == p.ProfileId && p2.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                            //Profile Career
                            let profileCareer = _context.ProfileCareerModel.Where(p3 => p3.ProfileId == p.ProfileId && p3.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                            //Profile Type
                            let profileType = _context.ProfileTypeModel.Where(p3 => p3.ProfileId == p.ProfileId && p3.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                            //CustomerType: Bussiness, Individual Customers, Contact
                            join catalog in _context.CatalogModel on new { CustomerType = profileType.CustomerTypeCode, Type = ConstCatalogType.CustomerType } equals new { CustomerType = catalog.CatalogCode, Type = catalog.CatalogTypeCode } into cTemp
                            from c in cTemp.DefaultIfEmpty()
                                //Main contact
                            let mainContact = (from profileContactAttribute in _context.ProfileContactAttributeModel
                                               join profileCT in _context.ProfileModel on profileContactAttribute.ProfileId equals profileCT.ProfileId
                                               where profileContactAttribute.CompanyId == p.ProfileId && (profileType != null && profileType.CustomerTypeCode == ConstCustomerType.Bussiness)
                                               orderby profileContactAttribute.IsMain == true descending
                                               select new ProfileSearchResultViewModel()
                                               {
                                                   ProfileContactId = profileCT.ProfileId,
                                                   ContactName = profileCT.ProfileName,
                                                   ContactPhone = profileCT.Phone,
                                                   ContactEmail = profileCT.Email,
                                                   ContactPosition = profileContactAttribute.Position,
                                                   ContactDepartment = profileContactAttribute.DepartmentCode,
                                               }).FirstOrDefault()

                            //SalesSupervisor
                            //NV kinh doanh + Phòng ban
                            let SaleSupervisor = (from pic in _context.PersonInChargeModel
                                                  join s in _context.SalesEmployeeModel on pic.SalesEmployeeCode equals s.SalesEmployeeCode
                                                  join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                                  from r in acc.RolesModel
                                                  where pic.ProfileId == p.ProfileId
                                                  select new SalesSupervisorViewModel()
                                                  {
                                                      SalesSupervisorCode = pic.SalesEmployeeCode,
                                                      SalesSupervisorName = s.SalesEmployeeName,
                                                      DepartmentName = r.isEmployeeGroup == true ? r.RolesName : ""
                                                  }).FirstOrDefault()
                            //Order by
                            orderby p.CreateTime descending
                            where
                            //search by ProfileName || AbbreviatedName
                            (searchModel.ProfileName == null || p.ProfileName.Contains(searchModel.ProfileName) || p.AbbreviatedName.Contains(searchModel.ProfileName))
                            //Search by Phone
                            && (searchModel.ProfilePhone == null || p.Phone == (searchModel.ProfilePhone))
                            //Search by TaxNo
                            //B or C
                            && (searchModel.TaxNo == null || p.TaxNo == searchModel.TaxNo)
                            && (p.CustomerTypeCode == ConstCustomerType.Account)
                            //Search by ProfileForeignCode
                            && (searchModel.ProfileForeignCode == null || p.ProfileForeignCode.Contains(searchModel.ProfileForeignCode))
                            select new MobileProfileSearchResultViewModel()
                            {
                                //Thông tin chung Account (B or C)
                                ProfileId = p.ProfileId,
                                //CustomerTypeCode = p.CustomerTypeCode,
                                CustomerTypeCode = profileType != null ? profileType.CustomerTypeCode : null,
                                CustomerTypeName = c.CatalogText_vi,
                                ProfileName = p.ProfileName,
                                Phone = p.Phone,
                                Address = p.Address,
                                ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                                DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                                WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                                TaxNo = p.TaxNo,
                                CustomerSourceCode = p.CustomerSourceCode,
                                //SaleOfficeCode = p.SaleOfficeCode,
                                SaleOfficeCode = province.Area.ToString(),
                                ProvinceId = p.ProvinceId,
                                DistrictId = p.DistrictId,
                                WardId = p.WardId,
                                Email = p.Email,
                                Title = p.Title,
                                ProfileShortName = p.ProfileShortName,
                                CustomerCareerCode = profileCareer != null ? profileCareer.ProfileCareerCode : p.CustomerCareerCode,
                                //B
                                Website = p.Website,
                                // ContactName = p.ContactName,
                                //ContactNumber = proB.CompanyNumber,
                                CustomerGroupCode = profileGroup == null ? null : profileGroup.ProfileGroupCode,
                                AddressTypeCode = p.AddressTypeCode,
                                ProfileForeignCode = p.ProfileForeignCode,
                                SalesEmployeeName = employee.SalesEmployeeName,
                                SalesSupervisorCode = SaleSupervisor.SalesSupervisorCode,
                                SalesSupervisorName = SaleSupervisor.SalesSupervisorName,
                                DepartmentName = SaleSupervisor.DepartmentName,

                                //Contact
                                ProfileContactId = mainContact.ProfileContactId,
                                ContactName = mainContact.ContactName,
                                ContactPhone = mainContact.ContactPhone,
                                ContactEmail = mainContact.ContactEmail,
                                ContactPosition = mainContact.ContactPosition,
                                ContactDepartment = mainContact.ContactDepartment,
                            }).ToList();

            var profileContact = (from p in _context.ProfileModel
                                  join contact in _context.ProfileContactAttributeModel on p.ProfileId equals contact.ProfileId
                                  join comp in _context.ProfileModel on contact.CompanyId equals comp.ProfileId
                                  //Create Account
                                  join a in _context.AccountModel on p.CreateBy equals a.AccountId
                                  //Province
                                  join pr in _context.ProvinceModel on comp.ProvinceId equals pr.ProvinceId into prG
                                  from province in prG.DefaultIfEmpty()
                                      //District
                                  join d in _context.DistrictModel on comp.DistrictId equals d.DistrictId into dG
                                  from district in dG.DefaultIfEmpty()
                                      //Ward
                                  join w in _context.WardModel on comp.WardId equals w.WardId into wG
                                  from ward in wG.DefaultIfEmpty()
                                      //Create By Employee
                                  join s in _context.SalesEmployeeModel on comp.CreateByEmployee equals s.SalesEmployeeCode into sG
                                  from employee in sG.DefaultIfEmpty()
                                      //Profile Group
                                  let profileGroup = _context.ProfileGroupModel.Where(p2 => p2.ProfileId == comp.ProfileId && p2.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                                  //Profile Career
                                  let profileCareer = _context.ProfileCareerModel.Where(p3 => p3.ProfileId == comp.ProfileId && p3.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                                  //Profile Type
                                  let profileType = _context.ProfileTypeModel.Where(p3 => p3.ProfileId == comp.ProfileId && p3.CompanyCode == searchModel.CompanyCode).FirstOrDefault()
                                  //CustomerType: Bussiness, Individual Customers, Contact
                                  join catalog in _context.CatalogModel on new { CustomerType = profileType.CustomerTypeCode, Type = ConstCatalogType.CustomerType } equals new { CustomerType = catalog.CatalogCode, Type = catalog.CatalogTypeCode } into cTemp
                                  from c in cTemp.DefaultIfEmpty()
                                      //SalesSupervisor
                                      //NV kinh doanh + Phòng ban
                                  let SaleSupervisor = (from pic in _context.PersonInChargeModel
                                                        join s in _context.SalesEmployeeModel on pic.SalesEmployeeCode equals s.SalesEmployeeCode
                                                        join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                                        from r in acc.RolesModel
                                                        where pic.ProfileId == comp.ProfileId
                                                        select new SalesSupervisorViewModel()
                                                        {
                                                            SalesSupervisorCode = pic.SalesEmployeeCode,
                                                            SalesSupervisorName = s.SalesEmployeeName,
                                                            DepartmentName = r.isEmployeeGroup == true ? r.RolesName : ""
                                                        }).FirstOrDefault()
                                  //Order by
                                  orderby p.CreateTime descending
                                  where
                                  //search by ProfileName || AbbreviatedName
                                  (searchModel.ProfileName == null || p.ProfileName.Contains(searchModel.ProfileName) || p.AbbreviatedName.Contains(searchModel.ProfileName))
                                  //Search by Phone
                                  && (searchModel.ProfilePhone == null || p.Phone == (searchModel.ProfilePhone))
                                  //Search by TaxNo
                                  //B or C
                                  && (searchModel.TaxNo == null || comp.TaxNo == searchModel.TaxNo)
                                  //Search by ProfileForeignCode
                                  && (searchModel.ProfileForeignCode == null || comp.ProfileForeignCode.Contains(searchModel.ProfileForeignCode))
                                  && p.CustomerTypeCode == ConstCustomerType.Contact
                                  select new MobileProfileSearchResultViewModel()
                                  {
                                      //Thông tin chung Account (B or C)
                                      ProfileId = comp.ProfileId,
                                      //CustomerTypeCode = comp.CustomerTypeCode,
                                      CustomerTypeCode = profileType != null ? profileType.CustomerTypeCode : null,
                                      CustomerTypeName = c.CatalogText_vi,
                                      ProfileName = comp.ProfileName,
                                      Phone = comp.Phone,
                                      Address = comp.Address,
                                      ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                                      DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                                      WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                                      CustomerSourceCode = comp.CustomerSourceCode,
                                      //SaleOfficeCode = comp.SaleOfficeCode,
                                      SaleOfficeCode = province.Area.ToString(),
                                      ProvinceId = comp.ProvinceId,
                                      DistrictId = comp.DistrictId,
                                      WardId = comp.WardId,
                                      Email = comp.Email,
                                      CustomerGroupCode = profileGroup == null ? null : profileGroup.ProfileGroupCode,
                                      Title = comp.Title,
                                      ProfileShortName = comp.ProfileShortName,
                                      TaxNo = comp.TaxNo,
                                      Website = comp.Website,
                                      CustomerCareerCode = profileCareer != null ? profileCareer.ProfileCareerCode : comp.CustomerCareerCode,
                                      AddressTypeCode = comp.AddressTypeCode,
                                      ProfileForeignCode = comp.ProfileForeignCode,
                                      SalesEmployeeName = employee.SalesEmployeeName,
                                      SalesSupervisorCode = SaleSupervisor.SalesSupervisorCode,
                                      SalesSupervisorName = SaleSupervisor.SalesSupervisorName,
                                      DepartmentName = SaleSupervisor.DepartmentName,

                                      //Contact
                                      ProfileContactId = p.ProfileId,
                                      ContactName = p.ProfileName,
                                      ContactPhone = p.Phone,
                                      ContactEmail = p.Email,
                                      ContactPosition = contact.Position,
                                      ContactDepartment = contact.DepartmentCode,
                                  }).ToList();

            if (profileContact == null)
            {
                profileContact = new List<MobileProfileSearchResultViewModel>();
            }
            if (profiles == null)
            {
                profiles = new List<MobileProfileSearchResultViewModel>();
            }
            profileContact.AddRange(profiles);

            //Tìm kiếm theo địa chỉ
            //Tìm từ khóa “781 lê hồng phong” → ra “781 / c1 / c2 lê hồng phong” hoặc “781 / 25 lê hồng phong…”

            //Cách làm: Nếu Address Khách null hoặc ""
            //Tách từ khoa thành mảng[781, Lê, hồng, phong]
            //=> Tìm 4 điều kiện và and lại với nhau:
            //      Profile.Address.Containt("781") and
            //      Profile.Address.Containt("Lê") and
            //      Profile.Address.Containt("hồng") and
            //      Profile.Address.Containt("phong")

            if (!string.IsNullOrEmpty(searchModel.Address))
            {
                var addressLst = searchModel.Address.Replace(',', ' ').Replace('.', ' ').Replace("  ", " ").Split(' ');

                if (addressLst != null && addressLst.Count() > 0)
                {
                    foreach (var address in addressLst)
                    {
                        profileContact = (from profile in profileContact
                                          where (profile.Address + profile.ProvinceName + profile.DistrictName).Contains(address)
                                          select profile).ToList();
                    }
                }
            }

            return profileContact;
        }
        #endregion Search mobile

        public Guid GetProfileIdBy(int ProfileCode)
        {
            return _context.ProfileModel.FirstOrDefault(p => p.ProfileCode == ProfileCode).ProfileId;
        }

        public Nullable<Guid> GetProfileByContact(Guid? ContactId)
        {
            var profileId = _context.ProfileContactAttributeModel.Where(p => p.ProfileId == ContactId)
                                    .Select(p => p.CompanyId).FirstOrDefault();
            return profileId;
        }
        /// <summary>
        /// Lấy thông tin hiển thị overview
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
        public ProfileOverviewModel GetViewBy(Guid profileid, string CurrentCompanyCode)
        {
            ProfileOverviewModel model = new ProfileOverviewModel();
            model = (from p in _context.ProfileModel
                     join p1 in _context.ProvinceModel on p.ProvinceId equals p1.ProvinceId into TmpList1
                     from prov in TmpList1.DefaultIfEmpty()
                     join p2 in _context.DistrictModel on p.DistrictId equals p2.DistrictId into TmpList2
                     from dist in TmpList2.DefaultIfEmpty()
                     join p3 in _context.WardModel on p.WardId equals p3.WardId into TmpList3
                     from ward in TmpList3.DefaultIfEmpty()
                         //Độ tuổi
                     join p4 in _context.CatalogModel on p.Age equals p4.CatalogCode into TmpAge
                     from age in TmpAge.DefaultIfEmpty()
                         //Khu vực
                     join p5 in _context.CatalogModel on p.SaleOfficeCode equals p5.CatalogCode into tmpSaleOffice
                     from saleOff in tmpSaleOffice.DefaultIfEmpty()
                         //Chi nhánh
                     join p6 in _context.StoreModel on p.CreateAtSaleOrg equals p6.SaleOrgCode into tmpStore
                     from store in tmpStore.DefaultIfEmpty()
                         //Phân loại KH
                     join type in _context.ProfileTypeModel on new { p.ProfileId, CompanyCode = CurrentCompanyCode }
                                                            equals new { ProfileId = (Guid)type.ProfileId, type.CompanyCode } into tg
                     from t1 in tg.DefaultIfEmpty()
                     join p7 in _context.CatalogModel on new { t1.CustomerTypeCode, CatalogTypeCode = ConstCatalogType.CustomerType } equals new { CustomerTypeCode = p7.CatalogCode, CatalogTypeCode = p7.CatalogTypeCode } into tmpCusType
                     from cusType in tmpCusType.DefaultIfEmpty()
                         //Nguồn khách hàng
                     join p8 in _context.CatalogModel on new { p.CustomerSourceCode, CatalogTypeCode = ConstCatalogType.CustomerSource } equals new { CustomerSourceCode = p8.CatalogCode, CatalogTypeCode = p8.CatalogTypeCode } into tmpCusSource
                     from CusSource in tmpCusSource.DefaultIfEmpty()
                         //Ngành nghề
                     join career in _context.ProfileCareerModel on new { p.ProfileId, CompanyCode = CurrentCompanyCode }
                                                            equals new { ProfileId = (Guid)career.ProfileId, career.CompanyCode } into cg
                     from ca in cg.DefaultIfEmpty()
                     join p9 in _context.CatalogModel on new { ca.ProfileCareerCode, CatalogTypeCode = ConstCatalogType.CustomerCareer } equals new { ProfileCareerCode = p9.CatalogCode, CatalogTypeCode = p9.CatalogTypeCode } into tmpCusCareer
                     from CusCareer in tmpCusCareer.DefaultIfEmpty()
                         //Loại địa chỉ
                     join p10 in _context.CatalogModel on new { p.AddressTypeCode, CatalogTypeCode = ConstCatalogType.AddressType } equals new { AddressTypeCode = p10.CatalogCode, CatalogTypeCode = p10.CatalogTypeCode } into tmpAddressType
                     from addressType in tmpAddressType.DefaultIfEmpty()
                         //Title
                     join title in _context.CatalogModel on new { p.Title, CatalogTypeCode = ConstCatalogType.Title } equals new { Title = title.CatalogCode, title.CatalogTypeCode } into titleGroup
                     from t in titleGroup.DefaultIfEmpty()
                         //join phone in _context.ProfilePhoneModel on p.ProfileId equals phone.ProfileId

                     where p.ProfileId == profileid
                     select new ProfileOverviewModel()
                     {
                         ProfileId = p.ProfileId,
                         ProfileCode = p.ProfileCode,
                         ProfileForeignCode = p.ProfileForeignCode,
                         //Phân loại KH
                         CustomerTypeCode = p.CustomerTypeCode,
                         CustomerTypeName = cusType.CatalogText_vi,
                         ProfileName = p.ProfileName,
                         ProfileShortName = p.ProfileShortName,
                         CompanyNumber = p.CompanyNumber,
                         Phone = p.Phone,
                         //Độ tuổi
                         Age = age.CatalogText_vi,
                         Email = p.Email,
                         //Khu vực
                         SaleOfficeName = saleOff.CatalogText_vi,
                         //Nguồn KH
                         CustomerSourceName = CusSource.CatalogText_vi,
                         Address = p.CustomerTypeCode == ConstProfileType.Opportunity ? p.ProjectLocation : p.Address,
                         ProvinceId = p.ProvinceId,
                         ProvinceName = prov.ProvinceName == null ? "" : ", " + prov.ProvinceName,
                         DistrictId = p.DistrictId,
                         DistrictName = dist.DistrictName == null ? "" : ", " + dist.Appellation + " " + dist.DistrictName,
                         WardId = p.WardId,
                         WardName = ward.WardName == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                         Note = p.Note,
                         VisitDate = p.VisitDate,
                         //Chi nhánh
                         CreateAtSaleOrgName = store.StoreName,
                         AbbreviatedName = p.AbbreviatedName,
                         isForeignName = p.isForeignCustomer == true ? "Ngoài nước" : "Trong nước",
                         ActivedName = p.Actived == false ? "Ngừng sử dụng" : "Đang sử dụng",
                         //MST
                         TaxNo = p.TaxNo,
                         Website = p.Website,
                         //ngành nghề
                         CustomerCareerName = CusCareer.CatalogText_vi,
                         AddressTypeName = addressType.CatalogText_vi
                     }).FirstOrDefault();
            var phoneList = _context.ProfilePhoneModel.Where(s => s.ProfileId == model.ProfileId).ToList();
            if (phoneList.Count != 0)
            {
                foreach (var phone in phoneList)
                {
                    model.Phone = model.Phone + ", " + phone.PhoneNumber;
                }
            }
            var emailList = _context.ProfileEmailModel.Where(s => s.ProfileId == model.ProfileId).Select(p => p.Email).ToList();
            if (emailList.Count != 0)
            {
                model.Email = string.Join("; ", emailList);
            }
            if (model != null)
            {
                var type = _context.ProfileModel.Where(p => p.ProfileId == profileid).Select(p => p.CustomerTypeCode).FirstOrDefault();
                if (type == ConstProfileType.Account)
                {
                    var CustomerType = _context.ProfileTypeModel.Where(p => p.ProfileId == profileid
                                                                         && p.CompanyCode == CurrentCompanyCode)
                                               .Select(p => p.CustomerTypeCode).FirstOrDefault();
                    model.CustomerTypeCode = CustomerType;
                    if (CustomerType == ConstCustomerType.Bussiness)
                    {
                        model.CustomerTypeName = "Doanh nghiệp";
                    }
                    else
                    {
                        model.CustomerTypeName = "Tiêu dùng";
                    }
                }
                model.Address = model.Address + model.WardName + model.DistrictName + model.ProvinceName;
                if (model.Address.StartsWith(","))
                {
                    model.Address = model.Address.TrimStart(',').Trim();
                }
            }
            return model;
        }

        public List<OpportunityPartnerViewModel> GetOpportunityPartner(Guid ProfileId, int PartnerType)
        {
            var result = (from p in _context.ProfileModel
                          join a in _context.Profile_Opportunity_PartnerModel on p.ProfileId equals a.PartnerId
                          join acc in _context.AccountModel on a.CreateBy equals acc.AccountId into ag
                          from ac in ag.DefaultIfEmpty()
                          join s in _context.SalesEmployeeModel on ac.EmployeeCode equals s.SalesEmployeeCode into sg
                          from emp in sg.DefaultIfEmpty()
                              //Province
                          join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                          from province in prG.DefaultIfEmpty()
                              //District
                          join dt in _context.DistrictModel on p.DistrictId equals dt.DistrictId into dG
                          from district in dG.DefaultIfEmpty()
                              //Ward
                          join w in _context.WardModel on p.WardId equals w.WardId into wG
                          from ward in wG.DefaultIfEmpty()

                          where a.ProfileId == ProfileId && a.PartnerType == PartnerType
                          orderby a.IsMain descending, a.CreateTime descending
                          select new OpportunityPartnerViewModel()
                          {
                              OpportunityPartnerId = a.OpportunityPartnerId,
                              ProfileId = p.ProfileId,
                              ProfileCode = p.ProfileCode,
                              ProfileName = p.ProfileName,
                              Address = p.Address,
                              ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                              DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                              WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                              CreateBy = a.CreateBy,
                              CreateTime = a.CreateTime,
                              IsMain = a.IsMain,
                              CreateUser = emp.SalesEmployeeName,
                          }).ToList();

            if (result != null && result.Count() > 0)
            {
                foreach (var item in result)
                {
                    item.Address = string.Format("{0}{1}{2}{3}", item.Address, item.WardName, item.DistrictName, item.ProvinceName);
                }
            }
            return result;
        }

        public List<OpportunityPartnerViewModel> GetOpportunityInternal(Guid ProfileId)
        {
            var result = (from p in _context.ProfileModel
                          join a in _context.Profile_Opportunity_InternalModel on p.ProfileId equals a.InternalId
                          join acc in _context.AccountModel on a.CreateBy equals acc.AccountId into ag
                          from ac in ag.DefaultIfEmpty()
                          join s in _context.SalesEmployeeModel on ac.EmployeeCode equals s.SalesEmployeeCode into sg
                          from emp in sg.DefaultIfEmpty()
                              //Province
                          join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                          from province in prG.DefaultIfEmpty()
                              //District
                          join dt in _context.DistrictModel on p.DistrictId equals dt.DistrictId into dG
                          from district in dG.DefaultIfEmpty()
                              //Ward
                          join w in _context.WardModel on p.WardId equals w.WardId into wG
                          from ward in wG.DefaultIfEmpty()

                          where a.ProfileId == ProfileId
                          orderby a.IsMain descending, a.CreateTime descending
                          select new OpportunityPartnerViewModel()
                          {
                              OpportunityPartnerId = a.OpportunityInternalId,
                              ProfileId = p.ProfileId,
                              ProfileCode = p.ProfileCode,
                              ProfileName = p.ProfileName,
                              Address = p.Address,
                              ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                              DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                              WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                              CreateBy = a.CreateBy,
                              CreateTime = a.CreateTime,
                              IsMain = a.IsMain,
                              CreateUser = emp.SalesEmployeeName,
                          }).ToList();

            if (result != null && result.Count() > 0)
            {
                foreach (var item in result)
                {
                    item.Address = string.Format("{0}{1}{2}{3}", item.Address, item.WardName, item.DistrictName, item.ProvinceName);
                }
            }
            return result;
        }

        public List<OpportunityPartnerViewModel> GetOpportunityCompetitor(Guid ProfileId)
        {
            var result = (from p in _context.ProfileModel
                          join a in _context.Profile_Opportunity_CompetitorModel on p.ProfileId equals a.CompetitorId
                          join acc in _context.AccountModel on a.CreateBy equals acc.AccountId into ag
                          from ac in ag.DefaultIfEmpty()
                          join s in _context.SalesEmployeeModel on ac.EmployeeCode equals s.SalesEmployeeCode into sg
                          from emp in sg.DefaultIfEmpty()
                              //Province
                          join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                          from province in prG.DefaultIfEmpty()
                              //District
                          join dt in _context.DistrictModel on p.DistrictId equals dt.DistrictId into dG
                          from district in dG.DefaultIfEmpty()
                              //Ward
                          join w in _context.WardModel on p.WardId equals w.WardId into wG
                          from ward in wG.DefaultIfEmpty()

                          where a.ProfileId == ProfileId
                          orderby a.IsMain descending, a.CreateTime descending
                          select new OpportunityPartnerViewModel()
                          {
                              OpportunityPartnerId = a.OpportunityCompetitorId,
                              ProfileId = p.ProfileId,
                              ProfileCode = p.ProfileCode,
                              ProfileName = p.ProfileName,
                              Address = p.Address,
                              ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                              DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                              WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                              CreateBy = a.CreateBy,
                              CreateTime = a.CreateTime,
                              IsMain = a.IsMain,
                              CreateUser = emp.SalesEmployeeName,
                          }).ToList();

            if (result != null && result.Count() > 0)
            {
                foreach (var item in result)
                {
                    item.Address = string.Format("{0}{1}{2}{3}", item.Address, item.WardName, item.DistrictName, item.ProvinceName);
                }
            }
            return result;
        }

        public List<string> GetOpportunityMaterial(Guid ProfileId, int? MaterialType = null)
        {
            var result = (from a in _context.Profile_Opportunity_MaterialModel
                              //Product
                          join c in _context.CatalogModel on new { CatalogTypeCode = ConstCatalogType.HandoverFurniture, CatalogCode = a.MaterialCode } equals new { c.CatalogTypeCode, c.CatalogCode } into cG
                          from cat in cG.DefaultIfEmpty()
                          where a.ProfileId == ProfileId && (MaterialType == null || a.MaterialType == MaterialType)
                          orderby a.CreateTime descending
                          select cat.CatalogText_vi).ToList();
            return result;
        }

        public List<string> GetRequiredProfileField(string Type)
        {
            var list = _context.ProfileConfigModel.Where(p => p.ProfileCategoryCode == Type && p.IsRequired == true)
                               .Select(p => p.FieldCode).ToList();
            if (list == null)
            {
                list = new List<string>();
            }
            return list;
        }

        public void UpdateProfileType(Guid ProfileId, string CustomerTypeCode, string CompanyCode, Guid? CurrentUserId)
        {
            if (!string.IsNullOrEmpty(CustomerTypeCode))
            {
                var exist = _context.ProfileTypeModel.FirstOrDefault(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode);
                if (exist != null)
                {
                    exist.CustomerTypeCode = CustomerTypeCode;
                    exist.LastEditBy = CurrentUserId;
                    exist.LastEditTime = DateTime.Now;
                }
                else
                {
                    ProfileTypeModel model = new ProfileTypeModel();
                    model.ProfileTypeId = Guid.NewGuid();
                    model.ProfileId = ProfileId;
                    model.CustomerTypeCode = CustomerTypeCode;
                    model.CompanyCode = CompanyCode;
                    model.CreateBy = CurrentUserId;
                    model.CreateTime = DateTime.Now;
                    _context.Entry(model).State = EntityState.Added;
                }
            }
        }

        /// <summary>
        /// Lấy property name dựa vào tên ViewBag (= tên parameter cấu hình)
        /// </summary>
        /// <param name="ProfileCategoryCode"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        public string GetPropertyNameByParameter(string ProfileCategoryCode, string Parameter)
        {
            var code = _context.ProfileConfigModel.Where(p => p.ProfileCategoryCode == ProfileCategoryCode && p.Parameters == Parameter)
                               .Select(p => p.FieldCode).FirstOrDefault();
            return code;
        }

        private static string GetPropertyName<T, TValue>(Expression<Func<T, TValue>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        /// <param name="profileVM"></param>
        /// <param name="modelAdd"></param>
        /// <param name="Phone"></param>
        /// <param name="personInChargeList"></param>
        /// <param name="profileGroupList"></param>
        /// <param name="CurrentUserId"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="CurrentEmployeeCode"></param>
        /// <param name="errorList"></param>
        public void CreateProfile(ProfileViewModel profileVM, ProfileModel modelAdd, List<string> Phone, List<string> Email, List<string> HandoverFurnitureList, List<PersonInChargeViewModel> personInChargeList, List<PersonInChargeViewModel> personInCharge2List, List<ProfileGroupCreateViewModel> profileGroupList, Guid? CurrentUserId, string CompanyCode, string CurrentEmployeeCode, out List<string> errorList)
        {
            errorList = new List<string>();

            ProfileRepository profileRepository = new ProfileRepository(_context);
            ProfileCareerRepository careerRepository = new ProfileCareerRepository(_context);
            PersonInChargeRepository personRepository = new PersonInChargeRepository(_context);
            ProfilePhoneRepository phoneRepository = new ProfilePhoneRepository(_context);
            ProfileEmailRepository emailRepository = new ProfileEmailRepository(_context);
            ProfileGroupRepository groupRepository = new ProfileGroupRepository(_context);

            //Profile Config
            var requiredList = GetRequiredProfileField(profileVM.ProfileTypeCode);

            //Thông tin chung
            #region ProfileModel
            //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            if (profileVM.isForeignCustomer == null)
            {
                ProvinceRepository _provinceRepository = new ProvinceRepository(_context);
                //Nếu không có Đối tượng thì lấy theo Area của ProvinceId
                var Area = _provinceRepository.GetAreaByProvince(profileVM.ProvinceId);
                if (!string.IsNullOrEmpty(Area))
                {
                    profileVM.isForeignCustomer = false;
                }
            }
            /*
              Khi chọn đối tượng KH => cập nhật field CustomerAccountGroupCode (Phân nhóm khách hàng).
                    Z002	KH trong nước
                    Z003	KH nước ngoài
             */
            modelAdd.isForeignCustomer = profileVM.isForeignCustomer;
            if (modelAdd.isForeignCustomer == true)
            {
                modelAdd.CustomerAccountGroupCode = "Z003";
            }
            else
            {
                modelAdd.CustomerAccountGroupCode = "Z002";
            }
            //Mã SAP (nếu có): đối với KH được tạo từ web tra cứu bảo hành
            modelAdd.ProfileForeignCode = profileVM.ProfileForeignCode;
            //5. Danh xưng
            modelAdd.Title = profileVM.CustomerTitle;
            //6. Loại
            modelAdd.CustomerTypeCode = profileVM.ProfileTypeCode;
            //Phân loại KH
            if (string.IsNullOrEmpty(profileVM.CustomerTypeCode))
            {
                if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.CustomerTypeCode)))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Phân loại KH\"");
                }
            }
            else
            {
                profileRepository.UpdateProfileType(modelAdd.ProfileId, profileVM.CustomerTypeCode, CompanyCode, CurrentUserId);
            }
            //7. Tên
            //Nếu là doanh nghiệp viết hoa các chữ cái đầu tiên
            var toLowerOtherChar = true;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                toLowerOtherChar = false;
            }
            modelAdd.ProfileName = profileVM.ProfileName.FirstCharToUpper(toLowerOtherChar);
            //8. Tên ngắn
            modelAdd.ProfileShortName = profileVM.ProfileShortName;
            //9. Tên viết tắt
            modelAdd.AbbreviatedName = profileVM.ProfileName.ToAbbreviation();
            //10. Ngày sinh
            modelAdd.DayOfBirth = profileVM.DayOfBirth;
            //11. Tháng sinh
            modelAdd.MonthOfBirth = profileVM.MonthOfBirth;
            //12. Năm sinh
            modelAdd.YearOfBirth = profileVM.YearOfBirth;
            //13. Độ tuổi
            modelAdd.Age = profileVM.Age;
            //14. Số điện thoại liên hệ
            modelAdd.Phone = profileVM.Phone?.Trim().Replace(" ", "");
            if (profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(modelAdd.Phone))
                {
                    errorList.Add("Vui lòng nhập thông tin \"SĐT liên hệ\"");
                }
                else
                {
                    //Check SĐT liên hệ nhập sai định dạng: phải bắt đầu bằng số 0, >= 10 số, < 15 số
                    if (!modelAdd.Phone.StartsWith("0") || modelAdd.Phone.Length < 10 || modelAdd.Phone.Length >= 15)
                    {
                        errorList.Add("SĐT liên hệ không đúng định dạng! (phải bắt đầu bằng số 0, >= 10 số, < 15 số)");
                    }
                }
            }
            //SĐT công ty
            //Không bắt buộc
            else if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                modelAdd.Phone = profileVM.Phone;
                if (!string.IsNullOrEmpty(modelAdd.Phone))
                {
                    //Check SĐT liên hệ nhập sai định dạng: phải bắt đầu bằng số 0, >= 10 số, < 15 số
                    if (!modelAdd.Phone.StartsWith("0") || modelAdd.Phone.Length < 10 || modelAdd.Phone.Length >= 15)
                    {
                        errorList.Add("SĐT công ty không đúng định dạng! (phải bắt đầu bằng số 0, >= 10 số, < 15 số)");
                    }
                }
            }
            //15. Email
            modelAdd.Email = profileVM.Email;
            //16. Khu vực
            modelAdd.SaleOfficeCode = profileVM.SaleOfficeCode;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(modelAdd.SaleOfficeCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Khu vực\"");
                }
            }
            //17. Địa chỉ
            modelAdd.Address = profileVM.Address.FirstCharToUpper();
            //if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            //{
            //    if (string.IsNullOrEmpty(modelAdd.Address))
            //    {
            //        errorList.Add("Vui lòng nhập thông tin \"Địa chỉ\"");
            //    }
            //}
            //18. Tỉnh/Thành phố
            modelAdd.ProvinceId = profileVM.ProvinceId;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (modelAdd.ProvinceId == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Tỉnh/Thành phố\"");
                }
            }
            //19. Quận/Huyện
            modelAdd.DistrictId = profileVM.DistrictId;
            //20. Phường/Xã
            modelAdd.WardId = profileVM.WardId;
            //21. Ghi chú
            modelAdd.Note = profileVM.Note;
            //22. Ngày ghé thăm
            modelAdd.VisitDate = profileVM.VisitDate;
            //23. Trạng thái
            modelAdd.Actived = true;
            //25. Nhân viên tạo
            modelAdd.CreateByEmployee = CurrentEmployeeCode;
            //26. Tạo tại công ty
            modelAdd.CreateAtCompany = CompanyCode;
            //27. Tạo tại chi nhánh
            modelAdd.CreateAtSaleOrg = profileVM.CreateAtSaleOrg;
            if (string.IsNullOrEmpty(profileVM.CreateAtSaleOrg))
            {
                if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.CreateAtSaleOrg)))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Chi nhánh\"");
                }
                //Nếu là Contact thì lưu SaleOrg theo khách hàng
                if (profileVM.ReferenceProfileId != null && profileVM.ProfileTypeCode == ConstProfileType.Contact)
                {
                    var existSaleOrg = _context.ProfileModel.Where(p => p.ReferenceProfileId == profileVM.ReferenceProfileId)
                                               .Select(p => p.CreateAtSaleOrg).FirstOrDefault();
                    modelAdd.CreateAtSaleOrg = existSaleOrg;
                }
            }
            //28. CreateBy
            modelAdd.CreateBy = CurrentUserId;
            //29. Thời gian tạo
            modelAdd.CreateTime = DateTime.Now;
            //32. Nguồn khách hàng
            modelAdd.CustomerSourceCode = profileVM.CustomerSourceCode;
            //Ngành nghề
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                if (string.IsNullOrEmpty(profileVM.CustomerCareerCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngành nghề\"");
                }
                else
                {
                    careerRepository.CreateOrUpdate(modelAdd.ProfileId, CompanyCode, profileVM.CustomerCareerCode, CurrentUserId);
                }
            }
            //Mã số thuế
            modelAdd.TaxNo = profileVM.TaxNo;
            modelAdd.Website = profileVM.Website;
            //Loại địa chỉ
            modelAdd.AddressTypeCode = profileVM.AddressTypeCode;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(modelAdd.AddressTypeCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Loại địa chỉ\"");
                }
            }
            //Khách hàng/Chủ đầu tư
            modelAdd.ReferenceProfileId = profileVM.ReferenceProfileId;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, Guid?>(p => p.ReferenceProfileId)))
            {
                if (profileVM.ReferenceProfileId == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Khách hàng\"");
                }
            }
            //Tư vẫn và thiết kế
            modelAdd.ReferenceProfileId2 = profileVM.ReferenceProfileId2;
            //Trạng thái dự án
            modelAdd.ProjectStatusCode = profileVM.ProjectStatusCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectStatusCode)))
            {
                if (profileVM.ProjectStatusCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Trạng thái\"");
                }
            }
            //Mức độ xác định
            modelAdd.QualificationLevelCode = profileVM.QualificationLevelCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.QualificationLevelCode)))
            {
                if (profileVM.QualificationLevelCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Mức độ xác định\"");
                }
            }
            //Nguồn thông tin
            modelAdd.ProjectSourceCode = profileVM.ProjectSourceCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectSourceCode)))
            {
                if (profileVM.ProjectSourceCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nguồn thông tin\"");
                }
            }
            //Công ty
            modelAdd.Text1 = profileVM.Text1;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.Text1)))
            {
                if (profileVM.Text1 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Công ty\"");
                }
            }
            modelAdd.Text2 = profileVM.Text2;
            modelAdd.Text3 = profileVM.Text3;
            modelAdd.Text4 = profileVM.Text4;
            modelAdd.Text5 = profileVM.Text5;
            //Danh mục dự án
            modelAdd.Dropdownlist1 = profileVM.LeadCategory;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.LeadCategory)))
            {
                if (profileVM.LeadCategory == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Danh mục\"");
                }
            }
            //Danh mục KH tiềm năng
            modelAdd.Dropdownlist2 = profileVM.OpportunityCategory;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityCategory)))
            {
                if (profileVM.OpportunityCategory == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Danh mục\"");
                }
            }
            //Xác suất
            modelAdd.Dropdownlist3 = profileVM.OpportunityPercentage;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityPercentage)))
            {
                if (profileVM.OpportunityPercentage == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Xác suất\"");
                }
            }
            //Loại hình
            modelAdd.Dropdownlist4 = profileVM.OpportunityType;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityType)))
            {
                if (profileVM.OpportunityType == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Loại hình\"");
                }
            }
            //Quy mô
            modelAdd.Dropdownlist5 = profileVM.ProjectScale;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectScale)))
            {
                if (profileVM.ProjectScale == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Quy mô\"");
                }
            }
            //Trạng thái KH tiềm năng
            modelAdd.Dropdownlist6 = profileVM.OpportunityStatus;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityStatus)))
            {
                if (profileVM.OpportunityStatus == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Trạng thái\"");
                }
            }
            modelAdd.Dropdownlist7 = profileVM.Dropdownlist7;
            modelAdd.Dropdownlist8 = profileVM.Dropdownlist8;
            modelAdd.Dropdownlist9 = profileVM.Dropdownlist9;
            modelAdd.Dropdownlist10 = profileVM.Dropdownlist10;
            //Tổng giá trị hợp đồng dự kiến
            modelAdd.ContractValue = profileVM.ContractValue;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, decimal?>(p => p.ContractValue)))
            {
                if (profileVM.ContractValue == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Tổng giá trị hợp đồng dự kiến\"");
                }
            }
            //Ngày bắt đầu
            modelAdd.Date1 = profileVM.Date1;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, DateTime?>(p => p.Date1)))
            {
                if (profileVM.Date1 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngày bắt đầu\"");
                }
            }
            modelAdd.Date2 = profileVM.Date2;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, DateTime?>(p => p.Date2)))
            {
                if (profileVM.Date2 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngày bắt đầu\"");
                }
            }
            modelAdd.Date3 = profileVM.Date3;
            modelAdd.Date4 = profileVM.Date4;
            modelAdd.Date5 = profileVM.Date5;
            //Opportunity
            modelAdd.ProjectLocation = profileVM.ProjectLocation;
            modelAdd.IsAnCuongAccessory = profileVM.IsAnCuongAccessory;
            modelAdd.Laminate = profileVM.Laminate;
            modelAdd.MFC = profileVM.MFC;
            modelAdd.Veneer = profileVM.Veneer;
            modelAdd.Flooring = profileVM.Flooring;
            modelAdd.Accessories = profileVM.Accessories;
            modelAdd.KitchenEquipment = profileVM.KitchenEquipment;
            modelAdd.OtherBrand = profileVM.OtherBrand;
            modelAdd.HandoverFurniture = profileVM.HandoverFurniture;

            //Yêu cầu tạo khách ở ECC
            modelAdd.isCreateRequest = profileVM.isCreateRequest;
            if (modelAdd.isCreateRequest.HasValue && modelAdd.isCreateRequest == true)
            {
                modelAdd.CreateRequestTime = modelAdd.CreateTime;
            }

            _context.Entry(modelAdd).State = EntityState.Added;
            #endregion

            //NV phụ trách (NV kinh doanh)
            #region Add personInCharge
            if (profileVM.Type == ConstProfileType.Opportunity)
            {
                if (personInChargeList != null && personInChargeList.Count > 0)
                {
                    var valueLst = personInChargeList.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                    else
                    {
                        foreach (var item in personInChargeList)
                        {
                            item.ProfileId = modelAdd.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                            item.SalesEmployeeType = 1;
                        }
                        personRepository.CreateOrUpdate(personInChargeList, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                }
                //NV sales admin
                if (personInCharge2List != null && personInCharge2List.Count > 0)
                {
                    var valueLst = personInCharge2List.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV Sales Admin\"!");
                    }
                    else
                    {
                        foreach (var item in personInCharge2List)
                        {
                            item.ProfileId = modelAdd.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                            item.SalesEmployeeType = 2;
                        }
                        personRepository.CreateOrUpdate(personInCharge2List, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV Sales Admin\"!");
                    }
                }
            }
            else
            {
                if (personInChargeList != null && personInChargeList.Count > 0)
                {
                    var valueLst = personInChargeList.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                    else
                    {
                        foreach (var item in personInChargeList)
                        {
                            item.ProfileId = modelAdd.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                        }
                        personRepository.CreateOrUpdate(personInChargeList, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                }
            }

            #endregion

            //Số điện thoại
            #region More phone number
            if (Phone != null && Phone.Count > 1)
            {
                string errMess = string.Empty;
                Phone.Remove(profileVM.Phone);
                var result = phoneRepository.UpdatePhone(Phone, modelAdd.ProfileId, out errMess);

                if (result == false)
                {
                    errorList.Add(errMess);
                }
            }
            #endregion

            //Email
            #region More email
            if (Email != null && Email.Count > 0)
            {
                string errMess = string.Empty;
                //Email.Remove(profileVM.Email);
                var result = emailRepository.UpdateEmail(Email, modelAdd.ProfileId, out errMess);

                if (result == false)
                {
                    errorList.Add(errMess);
                }
            }
            #endregion

            #region Nội thất bàn giao
            if (HandoverFurnitureList != null && HandoverFurnitureList.Count > 0)
            {
                string errMess = string.Empty;
                try
                {
                    foreach (var handoverFurniture in HandoverFurnitureList)
                    {
                        Profile_Opportunity_MaterialModel mat = new Profile_Opportunity_MaterialModel();
                        mat.OpportunityMaterialId = Guid.NewGuid();
                        mat.ProfileId = modelAdd.ProfileId;
                        mat.MaterialCode = handoverFurniture;
                        mat.MaterialType = 1;
                        mat.CreateBy = CurrentUserId;
                        mat.CreateTime = DateTime.Now;

                        _context.Entry(mat).State = EntityState.Added;
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                }
            }
            #endregion

            //Nếu là loại contact
            #region Profile Contact Attribute
            if (profileVM.ProfileTypeCode == ConstProfileType.Contact)
            {
                var contactAttAdd = new ProfileContactAttributeModel();
                //1. GUID
                contactAttAdd.ProfileId = modelAdd.ProfileId;
                //2. Công ty
                contactAttAdd.CompanyId = profileVM.CompanyId;
                //3. Chức vụ
                contactAttAdd.Position = profileVM.PositionB;
                //4. Liên hệ chính
                contactAttAdd.IsMain = profileVM.IsMain;
                //5. Phòng ban
                contactAttAdd.DepartmentCode = profileVM.DepartmentCode;

                _context.Entry(contactAttAdd).State = EntityState.Added;
            }
            #endregion

            //Thông tin liên hệ
            #region Contact
            if (!string.IsNullOrEmpty(profileVM.ContactName))
            {
                var profileContactId = Guid.NewGuid();

                ProfileModel profileContact = new ProfileModel();
                profileContact.ProfileId = profileContactId;
                profileContact.ProfileName = profileVM.ContactName?.FirstCharToUpper();
                profileContact.AbbreviatedName = profileVM.ProfileName.ToAbbreviation();
                profileContact.CustomerTypeCode = ConstProfileType.Contact;
                profileContact.isForeignCustomer = false;
                profileContact.Phone = profileVM.PhoneBusiness;
                if (string.IsNullOrEmpty(profileContact.Phone))
                {
                    errorList.Add("Vui lòng nhập thông tin \"SĐT liên hệ\"");
                }
                else
                {
                    //Check SĐT liên hệ nhập sai định dạng: phải bắt đầu bằng số 0, >= 10 số, < 15 số
                    if (!profileContact.Phone.StartsWith("0") || profileContact.Phone.Length < 10 || profileContact.Phone.Length >= 15)
                    {
                        errorList.Add("SĐT liên hệ không đúng định dạng! (phải bắt đầu bằng số 0, >= 10 số, < 15 số)");
                    }
                }
                profileContact.Email = profileVM.EmailBusiness;
                profileContact.Address = profileVM.Address;
                profileContact.ProvinceId = profileVM.ProvinceId;
                profileContact.DistrictId = profileVM.DistrictId;
                profileContact.WardId = profileVM.WardId;
                profileContact.Actived = true;
                profileContact.CreateByEmployee = CurrentEmployeeCode;
                profileContact.CreateAtCompany = CompanyCode;
                profileContact.CreateAtSaleOrg = profileVM.CreateAtSaleOrg;

                profileContact.CreateBy = CurrentUserId;
                profileContact.CreateTime = DateTime.Now;
                _context.Entry(profileContact).State = EntityState.Added;

                //ProfileContact
                #region ProfileContact
                ProfileContactAttributeModel contactAttAdd = new ProfileContactAttributeModel();
                //1. GUID
                contactAttAdd.ProfileId = profileContactId;
                //2. Công ty
                contactAttAdd.CompanyId = modelAdd.ProfileId;
                //3. Chức vụ
                contactAttAdd.Position = profileVM.PositionB;
                //4. Liên hệ chính
                contactAttAdd.IsMain = true;
                //5. Phòng ban
                contactAttAdd.DepartmentCode = profileVM.DepartmentCode;

                _context.Entry(contactAttAdd).State = EntityState.Added;
                #endregion

                //NV phụ trách
                #region PersonInCharge
                //if (personInChargeList != null && personInChargeList.Count > 0)
                //{
                //    foreach (var item in personInChargeList)
                //    {
                //        item.ProfileId = profileContactId;
                //        item.CreateBy = CurrentUserId;
                //        item.CompanyCode = CompanyCode;
                //    }
                //    personRepository.CreateOrUpdate(personInChargeList, CompanyCode);
                //}
                #endregion
            }
            else
            {
                if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Liên hệ\"");
                }
            }
            #endregion

            //Nhóm khách hàng doanh nghiệp
            #region Profile Group
            //Nếu là khách hàng tiêu dùng (C) thì lấy mặc định CustomerGroupCode là 23
            if (profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                profileGroupList.Add(new ProfileGroupCreateViewModel()
                {
                    ProfileGroupCode = ConstCustomerGroupCode.Customer
                });
            }

            if (profileGroupList != null && profileGroupList.Count > 0)
            {
                var valueLst = profileGroupList.Where(p => p.ProfileGroupCode != null).ToList();
                if (valueLst.Count == 0 && profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nhóm khách hàng\"!");
                }
                else
                {
                    foreach (var item in profileGroupList)
                    {
                        item.ProfileId = modelAdd.ProfileId;
                        item.CompanyCode = CompanyCode;
                        item.CreateBy = CurrentUserId;
                    }
                    groupRepository.CreateOrUpdate(profileGroupList);
                }
            }
            else
            {
                //Nếu là KH doanh nghiệp
                if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nhóm khách hàng\"!");
                }
            }
            #endregion

            //Dự án - Thi công
            //An Cường
            #region Profile_Opportunity_InternalModel
            if (profileVM.internalList != null && profileVM.internalList.Count > 0)
            {
                foreach (var internalItem in profileVM.internalList)
                {
                    if (internalItem.ConstructionId.HasValue)
                    {
                        Profile_Opportunity_InternalModel opp_internal = new Profile_Opportunity_InternalModel();
                        opp_internal.OpportunityInternalId = Guid.NewGuid();
                        opp_internal.ProfileId = modelAdd.ProfileId;
                        opp_internal.InternalId = internalItem.ConstructionId;
                        opp_internal.CreateBy = CurrentUserId;
                        opp_internal.CreateTime = modelAdd.CreateTime;

                        _context.Entry(opp_internal).State = EntityState.Added;
                    }
                }
            }
            #endregion
            //Đối thủ
            #region Profile_Opportunity_CompetitorModel
            if (profileVM.competitorList != null && profileVM.competitorList.Count > 0)
            {
                foreach (var competitorItem in profileVM.competitorList)
                {
                    if (competitorItem.ConstructionId.HasValue)
                    {
                        Profile_Opportunity_CompetitorModel opp_competitor = new Profile_Opportunity_CompetitorModel();
                        opp_competitor.OpportunityCompetitorId = Guid.NewGuid();
                        opp_competitor.ProfileId = modelAdd.ProfileId;
                        opp_competitor.CompetitorId = competitorItem.ConstructionId;
                        opp_competitor.CreateBy = CurrentUserId;
                        opp_competitor.CreateTime = modelAdd.CreateTime;

                        _context.Entry(opp_competitor).State = EntityState.Added;
                    }
                }
            }
            #endregion

            //Dự án
            if (profileVM.ProfileTypeCode == ConstProfileType.Opportunity)
            {
                //ĐVT
                modelAdd.Dropdownlist7 = profileVM.OpportunityUnit;

                //Hoạt động
                TaskModel task = new TaskModel();
                task.TaskId = Guid.NewGuid();
                task.Summary = string.Format("Dự án - {0}", modelAdd.ProfileName);
                task.PriorityCode = ConstPriotityCode.NORMAL;
                task.ProfileId = modelAdd.ProfileId;
                task.WorkFlowId = _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.PROJECT).Select(p => p.WorkFlowId).FirstOrDefault();
                string TaskStatusCode = string.Empty;
                //Nếu user chưa chọn xác suất thì lưu empty
                if (!string.IsNullOrEmpty(modelAdd.Dropdownlist3))
                {
                    TaskStatusCode = modelAdd.Dropdownlist3.Split('_')[1];
                    task.TaskStatusId = _context.TaskStatusModel.Where(p => p.WorkFlowId == task.WorkFlowId && p.TaskStatusCode == TaskStatusCode)
                                                            .Select(p => p.TaskStatusId).FirstOrDefault();
                }
                else
                {
                    task.TaskStatusId = Guid.Empty;
                }
                var _companyRepo = new CompanyRepository(_context);
                task.CompanyId = _companyRepo.GetCompanyIdBy(modelAdd.CreateAtCompany);
                var _storeRepo = new StoreRepository(_context);
                task.StoreId = _storeRepo.GetStoreIdBySaleOrgCode(modelAdd.CreateAtSaleOrg);
                task.CreateBy = CurrentUserId;
                task.CreateTime = DateTime.Now;

                _context.Entry(task).State = EntityState.Added;
            }
        }

        /// <summary>
        /// Lấy chi tiết khách hàng
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="viewMode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ProfileViewModel GetProfileDetails(Guid ProfileId, string CompanyCode, string viewMode, out bool error)
        {
            error = false;
            ProfileViewModel profileView = new ProfileViewModel();
            ProfileGroupRepository groupRepository = new ProfileGroupRepository(_context);
            ProfileCareerRepository careerRepository = new ProfileCareerRepository(_context);

            var profile = _context.ProfileModel.FirstOrDefault(p => p.ProfileId == ProfileId);
            if (profile != null)
            {
                #region Profile
                profileView = new ProfileViewModel
                {
                    //1. GUID
                    ProfileId = profile.ProfileId,
                    //2. Loại
                    ProfileTypeCode = profile.CustomerTypeCode,
                    //CustomerTypeName = CustomerType.CatalogText_vi,
                    //3. Ho va Tên|Tên công ty
                    ProfileName = profile.ProfileName,
                    //4. Số điện thoại
                    Phone = profile.Phone,
                    //PhoneBusiness = profile.Phone,
                    //5. Email
                    Email = profile.Email,
                    //6. Địa chỉ
                    Address = profile.Address,
                    //7. Quận/Huyện
                    ProvinceId = profile.ProvinceId,
                    //8. Tỉnh/Thành phố
                    DistrictId = profile.DistrictId,
                    //9. Ghi chú
                    Note = profile.Note,
                    //10. Hình ảnh
                    ImageUrl = profile.ImageUrl,
                    //11. Nhân viên tạo
                    CreateByEmployee = profile.CreateByEmployee,
                    //12. Tạo tại công ty
                    CreateAtCompany = profile.CreateAtCompany,
                    //13. Tạo tại cửa hàng
                    CreateAtSaleOrg = profile.CreateAtSaleOrg,
                    //14. CreateBy
                    CreateBy = profile.CreateBy,
                    //15. Thời gian tạo
                    CreateTime = profile.CreateTime,
                    //16. CreateBy
                    LastEditBy = profile.LastEditBy,
                    //17. Thời gian tạo
                    LastEditTime = profile.LastEditTime,
                    //18. Actived
                    Actived = profile.Actived == null ? false : profile.Actived,
                    //19. Tên viết tắt
                    AbbreviatedName = profile.ProfileName,
                    //20. Ngày sinh
                    DayOfBirth = profile.DayOfBirth,
                    //21. Tháng sinh
                    MonthOfBirth = profile.MonthOfBirth,
                    //22. Năm sinh
                    YearOfBirth = profile.YearOfBirth,
                    //23. Đối tượng (Trong nước: false| Nước ngoài: true)
                    isForeignCustomer = profile.isForeignCustomer,
                    Age = profile.Age,
                    SaleOfficeCode = profile.SaleOfficeCode,
                    ProfileShortName = profile.ProfileShortName,
                    VisitDate = profile.VisitDate,
                    WardId = profile.WardId,
                    Title = profile.Title,
                    ProfileCode = profile.ProfileCode,
                    ProfileForeignCode = profile.ProfileForeignCode,
                    CustomerSourceCode = profile.CustomerSourceCode,
                    //Nhóm KH
                    CustomerGroupCode = profile.CustomerGroupCode,
                    //SĐT công ty
                    CompanyNumber = profile.CompanyNumber,
                    //Mã số thuế
                    TaxNo = profile.TaxNo,
                    //Website
                    Website = profile.Website,
                    //Ngành nghề
                    CustomerCareerCode = profile.CustomerCareerCode,
                    //Loại địa chỉ
                    AddressTypeCode = profile.AddressTypeCode,
                    ProjectCode = profile.ProjectCode,
                    ProjectStatusCode = profile.ProjectStatusCode,
                    QualificationLevelCode = profile.QualificationLevelCode,
                    ProjectSourceCode = profile.ProjectSourceCode,
                    ReferenceProfileId = profile.ReferenceProfileId,
                    ReferenceProfileId2 = profile.ReferenceProfileId2,
                    ContractValue = profile.ContractValue,
                    Text1 = profile.Text1,
                    Text2 = profile.Text2,
                    Text3 = profile.Text3,
                    Text4 = profile.Text4,
                    Text5 = profile.Text5,
                    Dropdownlist1 = profile.Dropdownlist1,
                    Dropdownlist2 = profile.Dropdownlist2,
                    Dropdownlist3 = profile.Dropdownlist3,
                    Dropdownlist4 = profile.Dropdownlist4,
                    Dropdownlist5 = profile.Dropdownlist5,
                    Dropdownlist6 = profile.Dropdownlist6,
                    Dropdownlist7 = profile.Dropdownlist7,
                    Dropdownlist8 = profile.Dropdownlist8,
                    Dropdownlist9 = profile.Dropdownlist9,
                    Dropdownlist10 = profile.Dropdownlist10,
                    Date1 = profile.Date1,
                    Date2 = profile.Date2,
                    Date3 = profile.Date3,
                    Date4 = profile.Date4,
                    Date5 = profile.Date5,
                    //Opportunity
                    ProjectLocation = profile.ProjectLocation,
                    IsAnCuongAccessory = profile.IsAnCuongAccessory == null ? false : profile.IsAnCuongAccessory,
                    Laminate = profile.Laminate,
                    MFC = profile.MFC,
                    Veneer = profile.Veneer,
                    Flooring = profile.Flooring,
                    Accessories = profile.Accessories,
                    KitchenEquipment = profile.KitchenEquipment,
                    OtherBrand = profile.OtherBrand,
                    HandoverFurniture = profile.HandoverFurniture,
                    CompleteYear = profile.Text4,
                    CompleteQuarter = profile.Text5,
                    //Yêu cầu tạo khách ở ECC
                    isCreateRequest = profile.isCreateRequest,
                    PaymentMethodCode = profile.PaymentMethodCode,
                    PartnerFunctionCode = profile.PartnerFunctionCode,
                    CurrencyCode = profile.CurrencyCode,
                    TaxClassificationCode = profile.TaxClassificationCode,
                    Manager = profile.Manager,
                    DebsEmployee = profile.DebsEmployee,
                    PaymentTermCode= profile.PaymentTermCode,
                    ReconcileAccountCode=profile.ReconcileAccountCode,
                    CustomerAccountAssignmentGroupCode = profile.CustomerAccountAssignmentGroupCode
                };
                //Nếu Khu vực NULL => load Khu vực theo Tỉnh/Thành phố
                if (string.IsNullOrEmpty(profileView.SaleOfficeCode))
                {
                    ProvinceRepository provinceRepository = new ProvinceRepository(_context);
                    profileView.SaleOfficeCode = provinceRepository.GetAreaByProvince(profileView.ProvinceId);
                }
                //Nếu Đối tượng NULL => load theo Khu vực
                if (profileView.isForeignCustomer == null)
                {
                    ProvinceRepository _provinceRepository = new ProvinceRepository(_context);
                    //Nếu không có Đối tượng thì lấy theo Area của ProvinceId
                    var Area = _provinceRepository.GetAreaByProvince(profileView.ProvinceId);
                    if (!string.IsNullOrEmpty(Area))
                    {
                        profileView.isForeignCustomer = false;
                    }
                }
                #endregion
            }
            else
            {
                error = true;
                return profileView;
            }
            #region Details
            //Phân loại KH
            profileView.CustomerTypeCode = _context.ProfileTypeModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode)
                                                   .Select(p => p.CustomerTypeCode).FirstOrDefault();
            //Bussiness
            //Nhóm KH
            var profileGroupList = groupRepository.GetListProfileGroupBy(ProfileId, CompanyCode);
            profileView.profileGroupList = profileGroupList;

            //Hiển thị danh sách Nhóm KH ở cty khác nếu ViewExtens đang ở Mode_2
            if (viewMode == "Mode_2")
            {
                profileView.profileGroupOtherCompanyList = groupRepository.GetListProfileGroupOtherCompanyBy(ProfileId, CompanyCode);
            }

            //Ngành nghề
            var profileCareer = careerRepository.GetProfileCareerBy(ProfileId, CompanyCode);
            if (profileCareer != null)
            {
                profileView.CustomerCareerCode = profileCareer.ProfileCareerCode;
            }
            else
            {
                profileView.CustomerCareerCode = null;
            }

            if (viewMode == "Mode_2")
            {
                profileView.profileCareerOtherCompanyList = careerRepository.GetProfileCareerOtherCompanyBy(ProfileId, CompanyCode);
            }
            //Contact
            if (profileView.ProfileTypeCode == ConstProfileType.Contact)
            {
                var proContact = _context.ProfileContactAttributeModel.FirstOrDefault(p => p.ProfileId == ProfileId);
                if (proContact != null)
                {
                    //2. Công ty
                    profileView.CompanyId = proContact.CompanyId.Value;
                    profileView.ReferenceProfileId = proContact.CompanyId;
                    profileView.CompanyName = _context.ProfileModel.Where(p => p.ProfileId == proContact.CompanyId)
                                                      .Select(p => p.ProfileName).FirstOrDefault();
                    //3.Chức vụ
                    profileView.ProfileContactPosition = proContact.Position;
                    //4. Liên hệ chính
                    profileView.IsMain = proContact.IsMain;
                    //5. Phòng ban
                    profileView.DepartmentCode = proContact.DepartmentCode;
                }
            }
            //ReferenceProfileId
            if (profileView.ReferenceProfileId != null)
            {
                var company = _context.ProfileModel.Where(p => p.ProfileId == profileView.ReferenceProfileId)
                                      .Select(p => p.ProfileName).FirstOrDefault();
                profileView.CompanyName = company;
                profileView.CompanyId = profileView.ReferenceProfileId.Value;
            }
            //ReferenceProfileId2
            if (profileView.ReferenceProfileId2 != null)
            {
                var consultingAndDesign = _context.ProfileModel.Where(p => p.ProfileId == profileView.ReferenceProfileId2)
                                      .Select(p => p.ProfileName).FirstOrDefault();
                profileView.ConsultingAndDesign = consultingAndDesign;
            }
            #endregion
            return profileView;
        }

        /// <summary>
        /// Cập nhật khách hàng
        /// </summary>
        /// <param name="profileVM"></param>
        /// <param name="profile"></param>
        /// <param name="Phone"></param>
        /// <param name="personInChargeList"></param>
        /// <param name="profileGroupList"></param>
        /// <param name="CurrentUserId"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="CurrentEmployeeCode"></param>
        /// <param name="errorList"></param>
        public void UpdateProfile(ProfileViewModel profileVM, ProfileModel profile, List<string> Phone, List<string> Email, List<string> HandoverFurnitureList, List<PersonInChargeViewModel> personInChargeList, List<PersonInChargeViewModel> personInCharge2List, List<ProfileGroupCreateViewModel> profileGroupList, Guid? CurrentUserId, string CompanyCode, string CurrentEmployeeCode, out List<string> errorList)
        {
            errorList = new List<string>();

            ProfileRepository profileRepository = new ProfileRepository(_context);
            ProfileCareerRepository careerRepository = new ProfileCareerRepository(_context);
            PersonInChargeRepository personRepository = new PersonInChargeRepository(_context);
            ProfilePhoneRepository phoneRepository = new ProfilePhoneRepository(_context);
            ProfileEmailRepository emailRepository = new ProfileEmailRepository(_context);
            ProfileGroupRepository groupRepository = new ProfileGroupRepository(_context);

            //Profile Config
            var requiredList = GetRequiredProfileField(profileVM.ProfileTypeCode);

            //Thông tin chung
            #region ProfileModel
            //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            if (profileVM.isForeignCustomer == null)
            {
                ProvinceRepository _provinceRepository = new ProvinceRepository(_context);
                //Nếu không có Đối tượng thì lấy theo Area của ProvinceId
                var Area = _provinceRepository.GetAreaByProvince(profileVM.ProvinceId);
                if (!string.IsNullOrEmpty(Area))
                {
                    profileVM.isForeignCustomer = false;
                }
            }
            profile.isForeignCustomer = profileVM.isForeignCustomer;

            /*
             Khi chọn đối tượng KH => cập nhật field CustomerAccountGroupCode (Phân nhóm khách hàng).
                   Z002	KH trong nước
                   Z003	KH nước ngoài
            */
            if (profile.isForeignCustomer == true)
            {
                profile.CustomerAccountGroupCode = "Z003";
            }
            else
            {
                profile.CustomerAccountGroupCode = "Z002";
            }

            //5. Danh xưng
            profile.Title = profileVM.CustomerTitle;
            //6. Phân loại KH
            var existCustomerTypeCode = _context.ProfileTypeModel
                                                .Where(p => p.ProfileId == profile.ProfileId &&
                                                            p.CompanyCode == CompanyCode).FirstOrDefault();
            //Nếu chưa xét cho cty hiện tại thì cập nhật
            if (existCustomerTypeCode == null)
            {
                //Nếu không có dữ liệu input và config bắt buộc thì báo lỗi
                if (string.IsNullOrEmpty(profileVM.CustomerTypeCode))
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.CustomerTypeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"Phân loại KH\"");
                    }
                }
                else
                {
                    profileRepository.UpdateProfileType(profile.ProfileId, profileVM.CustomerTypeCode, CompanyCode, CurrentUserId);
                }
            }
            //Update cho phép sửa phân loại KH
            else
            {
                profileRepository.UpdateProfileType(profile.ProfileId, profileVM.CustomerTypeCode, CompanyCode, CurrentUserId);
            }
            //7. Tên
            //Nếu là doanh nghiệp viết hoa các chữ cái đầu tiên
            var toLowerOtherChar = true;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                toLowerOtherChar = false;
            }
            profile.ProfileName = profileVM.ProfileName.FirstCharToUpper(toLowerOtherChar);
            //8. Tên ngắn
            profile.ProfileShortName = profileVM.ProfileShortName;
            //9. Tên viết tắt
            profile.AbbreviatedName = profileVM.ProfileName.ToAbbreviation();
            //10. Ngày sinh
            profile.DayOfBirth = profileVM.DayOfBirth;
            //11. Tháng sinh
            profile.MonthOfBirth = profileVM.MonthOfBirth;
            //12. Năm sinh
            profile.YearOfBirth = profileVM.YearOfBirth;
            //13. Độ tuổi
            profile.Age = profileVM.Age;
            //14. Số điện thoại liên hệ
            profile.Phone = profileVM.Phone?.Trim().Replace(" ", "");
            if (profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(profile.Phone))
                {
                    errorList.Add("Vui lòng nhập thông tin \"SĐT liên hệ\"");
                }
                else
                {
                    //Check SĐT liên hệ nhập sai định dạng: phải bắt đầu bằng số 0, >= 10 số, < 15 số
                    if (!profile.Phone.StartsWith("0") || profile.Phone.Length < 10 || profile.Phone.Length >= 15)
                    {
                        errorList.Add("SĐT liên hệ không đúng định dạng! (phải bắt đầu bằng số 0, >= 10 số, < 15 số)");
                    }
                }
            }
            //SĐT công ty
            //Không bắt buộc
            else if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                profile.Phone = profileVM.Phone;
                if (!string.IsNullOrEmpty(profile.Phone))
                {
                    //Check SĐT liên hệ nhập sai định dạng: phải bắt đầu bằng số 0, >= 10 số, < 15 số
                    if (!profile.Phone.StartsWith("0") || profile.Phone.Length < 10 || profile.Phone.Length >= 15)
                    {
                        errorList.Add("SĐT công ty không đúng định dạng! (phải bắt đầu bằng số 0, >= 10 số, < 15 số)");
                    }
                }
            }
            //15. Email
            profile.Email = profileVM.Email;
            //16. Khu vực
            profile.SaleOfficeCode = profileVM.SaleOfficeCode;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(profile.SaleOfficeCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Khu vực\"");
                }
            }
            //17. Địa chỉ
            profile.Address = profileVM.Address.FirstCharToUpper();
            //if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            //{
            //    if (string.IsNullOrEmpty(profile.Address))
            //    {
            //        errorList.Add("Vui lòng nhập thông tin \"Địa chỉ\"");
            //    }
            //}
            //18. Tỉnh/Thành phố
            profile.ProvinceId = profileVM.ProvinceId;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (profile.ProvinceId == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Tỉnh/Thành phố\"");
                }
            }
            //19. Quận/Huyện
            profile.DistrictId = profileVM.DistrictId;
            //20. Phường/Xã
            profile.WardId = profileVM.WardId;
            //21. Ghi chú
            profile.Note = profileVM.Note;
            //22. Ngày ghé thăm
            profile.VisitDate = profileVM.VisitDate;
            //23. Trạng thái
            profile.Actived = profileVM.Actived;
            //27. Tạo tại chi nhánh
            if (string.IsNullOrEmpty(profileVM.CreateAtSaleOrg))
            {
                if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.CreateAtSaleOrg)))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Chi nhánh\"");
                }
            }
            else
            {
                //Cập nhật chi nhánh cho cả liên hệ nếu có thay đổi chi nhánh khách hàng
                if (profileVM.CreateAtSaleOrg != profile.CreateAtSaleOrg && profileVM.ProfileTypeCode == ConstProfileType.Contact)
                {
                    var contactList = _context.ProfileContactAttributeModel.Where(p => p.CompanyId == profile.ProfileId).ToList();
                    if (contactList != null && contactList.Count > 0)
                    {
                        foreach (var item in contactList)
                        {
                            var contact = _context.ProfileModel.FirstOrDefault(p => p.ProfileId == item.ProfileId);
                            if (contact != null)
                            {
                                contact.CreateAtSaleOrg = profileVM.CreateAtSaleOrg;
                            }
                        }
                    }
                }
            }
            profile.CreateAtSaleOrg = profileVM.CreateAtSaleOrg;
            //28. LastEditBy
            profile.LastEditBy = CurrentUserId;
            //29. Thời gian cập nhật
            profile.LastEditTime = DateTime.Now;
            //32. Nguồn khách hàng
            profile.CustomerSourceCode = profileVM.CustomerSourceCode;
            //Ngành nghề
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
            {
                if (string.IsNullOrEmpty(profileVM.CustomerCareerCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngành nghề\"");
                }
                else
                {
                    careerRepository.CreateOrUpdate(profile.ProfileId, CompanyCode, profileVM.CustomerCareerCode, CurrentUserId);
                }
            }
            else if (profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                //Check trường hợp: nếu user đổi từ "doanh nghiệp" sang "tiêu dùng" => xóa extend "Ngành nghề" 
                var existsCareer = _context.ProfileCareerModel.Where(p => p.ProfileId == profileVM.ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
                if (existsCareer != null)
                {
                    _context.Entry(existsCareer).State = EntityState.Deleted;
                }
            }
            //Mã số thuế
            profile.TaxNo = profileVM.TaxNo;
            profile.Website = profileVM.Website;
            //Loại địa chỉ
            profile.AddressTypeCode = profileVM.AddressTypeCode;
            if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness || profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                if (string.IsNullOrEmpty(profile.AddressTypeCode))
                {
                    errorList.Add("Vui lòng nhập thông tin \"Loại địa chỉ\"");
                }
            }
            //Khách hàng/Chủ đầu tư
            profile.ReferenceProfileId = profileVM.ReferenceProfileId;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, Guid?>(p => p.ReferenceProfileId)))
            {
                if (profileVM.ReferenceProfileId == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Khách hàng\"");
                }
            }
            //Tư vấn và thiết kế
            profile.ReferenceProfileId2 = profileVM.ReferenceProfileId2;
            //Trạng thái dự án
            profile.ProjectStatusCode = profileVM.ProjectStatusCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectStatusCode)))
            {
                if (profileVM.ProjectStatusCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Trạng thái\"");
                }
            }
            //Mức độ xác định
            profile.QualificationLevelCode = profileVM.QualificationLevelCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.QualificationLevelCode)))
            {
                if (profileVM.QualificationLevelCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Mức độ xác định\"");
                }
            }
            //Nguồn thông tin
            profile.ProjectSourceCode = profileVM.ProjectSourceCode;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectSourceCode)))
            {
                if (profileVM.ProjectSourceCode == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nguồn thông tin\"");
                }
            }
            //Công ty
            profile.Text1 = profileVM.Text1;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.Text1)))
            {
                if (profileVM.Text1 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Công ty\"");
                }
            }
            profile.Text2 = profileVM.Text2;
            profile.Text3 = profileVM.Text3;
            profile.Text4 = profileVM.Text4;
            profile.Text5 = profileVM.Text5;
            //Danh mục dự án
            profile.Dropdownlist1 = profileVM.LeadCategory;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.LeadCategory)))
            {
                if (profileVM.LeadCategory == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Danh mục\"");
                }
            }
            //Danh mục KH tiềm năng
            profile.Dropdownlist2 = profileVM.OpportunityCategory;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityCategory)))
            {
                if (profileVM.OpportunityCategory == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Danh mục\"");
                }
            }
            //Xác suất
            profile.Dropdownlist3 = profileVM.OpportunityPercentage;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityPercentage)))
            {
                if (profileVM.OpportunityPercentage == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Xác suất\"");
                }
            }
            //Loại hình
            profile.Dropdownlist4 = profileVM.OpportunityType;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityType)))
            {
                if (profileVM.OpportunityType == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Loại hình\"");
                }
            }
            //Quy mô
            profile.Dropdownlist5 = profileVM.ProjectScale;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.ProjectScale)))
            {
                if (profileVM.ProjectScale == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Quy mô\"");
                }
            }
            //Trạng thái KH tiềm năng
            profile.Dropdownlist6 = profileVM.OpportunityStatus;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.OpportunityStatus)))
            {
                if (profileVM.OpportunityStatus == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Trạng thái\"");
                }
            }
            profile.Dropdownlist7 = profileVM.Dropdownlist7;
            profile.Dropdownlist8 = profileVM.Dropdownlist8;
            profile.Dropdownlist9 = profileVM.Dropdownlist9;
            profile.Dropdownlist10 = profileVM.Dropdownlist10;
            //Tổng giá trị hợp đồng dự kiến
            profile.ContractValue = profileVM.ContractValue;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, decimal?>(p => p.ContractValue)))
            {
                if (profileVM.ContractValue == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Tổng giá trị hợp đồng dự kiến\"");
                }
            }
            //Ngày bắt đầu
            profile.Date1 = profileVM.Date1;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, DateTime?>(p => p.Date1)))
            {
                if (profileVM.Date1 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngày bắt đầu\"");
                }
            }
            profile.Date2 = profileVM.Date2;
            if (requiredList.Contains(GetPropertyName<ProfileViewModel, DateTime?>(p => p.Date2)))
            {
                if (profileVM.Date2 == null)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Ngày bắt đầu\"");
                }
            }
            profile.Date3 = profileVM.Date3;
            profile.Date4 = profileVM.Date4;
            profile.Date5 = profileVM.Date5;
            //Opportunity
            profile.ProjectLocation = profileVM.ProjectLocation;
            profile.IsAnCuongAccessory = profileVM.IsAnCuongAccessory;
            profile.Laminate = profileVM.Laminate;
            profile.MFC = profileVM.MFC;
            profile.Veneer = profileVM.Veneer;
            profile.Flooring = profileVM.Flooring;
            profile.Accessories = profileVM.Accessories;
            profile.KitchenEquipment = profileVM.KitchenEquipment;
            profile.OtherBrand = profileVM.OtherBrand;
            profile.HandoverFurniture = profileVM.HandoverFurniture;

            //Yêu cầu tạo khách ở ECC
            if (profileVM.isCreateRequest.HasValue)
            {
                if (profile.isCreateRequest == null && profileVM.isCreateRequest == true)
                {
                    profile.CreateRequestTime = profile.LastEditTime;
                }
            }
            else
            {
                profile.CreateRequestTime = null;
            }
            profile.isCreateRequest = profileVM.isCreateRequest;

            profile.PartnerFunctionCode = profileVM.PartnerFunctionCode;
            profile.PaymentMethodCode = profileVM.PaymentMethodCode;
            profile.TaxClassificationCode = profileVM.TaxClassificationCode;
            profile.CurrencyCode = profileVM.CurrencyCode;
            profile.DebsEmployee = profileVM.DebsEmployee;
            profile.Manager = profileVM.Manager;
            profile.CustomerAccountAssignmentGroupCode = profileVM.CustomerAccountAssignmentGroupCode;
            profile.PaymentTermCode = profileVM.PaymentTermCode;
            profile.ReconcileAccountCode = profileVM.ReconcileAccountCode;

            #endregion

            //NV phụ trách (NV kinh doanh)
            #region Add personInCharge
            if (profileVM.Type == ConstProfileType.Opportunity)
            {
                if (personInChargeList != null && personInChargeList.Count > 0)
                {
                    var valueLst = personInChargeList.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                    else
                    {
                        foreach (var item in personInChargeList)
                        {
                            item.ProfileId = profileVM.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                            item.SalesEmployeeType = 1;
                        }
                        personRepository.CreateOrUpdate(personInChargeList, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                }

                if (personInCharge2List != null && personInCharge2List.Count > 0)
                {
                    var valueLst = personInCharge2List.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV Sales Admin\"!");
                    }
                    else
                    {
                        foreach (var item in personInCharge2List)
                        {
                            item.ProfileId = profileVM.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                            item.SalesEmployeeType = 2;
                        }
                        personRepository.CreateOrUpdate(personInCharge2List, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV Sales Admin\"!");
                    }
                }
            }
            else
            {
                if (personInChargeList != null && personInChargeList.Count > 0)
                {
                    var valueLst = personInChargeList.Select(p => p.SalesEmployeeCode != null).ToList();
                    if (valueLst.Count == 0 && requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                    else
                    {
                        foreach (var item in personInChargeList)
                        {
                            item.ProfileId = profileVM.ProfileId;
                            item.CreateBy = CurrentUserId;
                            item.CompanyCode = CompanyCode;
                        }
                        personRepository.CreateOrUpdate(personInChargeList, CompanyCode);
                    }
                }
                else
                {
                    if (requiredList.Contains(GetPropertyName<ProfileViewModel, string>(p => p.SalesEmployeeCode)))
                    {
                        errorList.Add("Vui lòng nhập thông tin \"NV kinh doanh\"!");
                    }
                }
            }
            #endregion

            //Số điện thoại
            #region More phone number
            if (Phone != null && Phone.Count > 1)
            {
                string errMess = string.Empty;
                Phone.Remove(profileVM.Phone);
                var result = phoneRepository.UpdatePhone(Phone, profileVM.ProfileId, out errMess);

                if (result == false)
                {
                    errorList.Add(errMess);
                }
            }
            else
            {
                var morePhone = _context.ProfilePhoneModel.Where(p => p.ProfileId == profileVM.ProfileId).ToList();
                if (morePhone != null && morePhone.Count > 0)
                {
                    //Xoá hết
                    for (int i = morePhone.Count - 1; i >= 0; i--)
                    {
                        _context.Entry(morePhone[i]).State = EntityState.Deleted;
                    }
                }
            }
            #endregion

            //Email
            #region More email
            if (Email != null && Email.Count > 0)
            {
                string errMess = string.Empty;
                //Email.Remove(profileVM.Email);
                var result = emailRepository.UpdateEmail(Email, profileVM.ProfileId, out errMess);

                if (result == false)
                {
                    errorList.Add(errMess);
                }
            }
            else
            {
                var moreEmail = _context.ProfileEmailModel.Where(p => p.ProfileId == profileVM.ProfileId).ToList();
                if (moreEmail != null && moreEmail.Count > 0)
                {
                    //Xoá hết
                    for (int i = moreEmail.Count - 1; i >= 0; i--)
                    {
                        _context.Entry(moreEmail[i]).State = EntityState.Deleted;
                    }
                }
            }
            #endregion

            #region Nội thất bàn giao
            if (HandoverFurnitureList != null && HandoverFurnitureList.Count > 0)
            {
                //Xóa dữ liệu cũ
                var existsHandoverFurniture = _context.Profile_Opportunity_MaterialModel.Where(p => p.ProfileId == profile.ProfileId && p.MaterialType == 1).ToList();
                if (existsHandoverFurniture != null && existsHandoverFurniture.Count > 0)
                {
                    _context.Profile_Opportunity_MaterialModel.RemoveRange(existsHandoverFurniture);
                }
                string errMess = string.Empty;
                try
                {
                    foreach (var handoverFurniture in HandoverFurnitureList)
                    {
                        Profile_Opportunity_MaterialModel mat = new Profile_Opportunity_MaterialModel();
                        mat.OpportunityMaterialId = Guid.NewGuid();
                        mat.ProfileId = profile.ProfileId;
                        mat.MaterialCode = handoverFurniture;
                        mat.MaterialType = 1;
                        mat.CreateBy = CurrentUserId;
                        mat.CreateTime = DateTime.Now;

                        _context.Entry(mat).State = EntityState.Added;
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                }
            }
            #endregion

            //Nếu là loại contact
            #region Profile Contact Attribute
            if (profileVM.ProfileTypeCode == ConstProfileType.Contact)
            {
                var attr = _context.ProfileContactAttributeModel.FirstOrDefault(p => p.ProfileId == profile.ProfileId);
                if (attr != null)
                {
                    //2. Công ty
                    attr.CompanyId = profileVM.CompanyId;
                    //3. Chức vụ
                    attr.Position = profileVM.PositionB;
                    //4. Liên hệ chính
                    attr.IsMain = profileVM.IsMain;
                    //5. Phòng ban
                    attr.DepartmentCode = profileVM.DepartmentCode;
                }
            }
            #endregion

            //Nhóm khách hàng doanh nghiệp
            #region Profile Group
            //Nếu là khách hàng tiêu dùng (C) thì lấy mặc định CustomerGroupCode là 23
            if (profileVM.CustomerTypeCode == ConstCustomerType.Customer)
            {
                var existProfileGroup = _context.ProfileGroupModel.Where(p => p.ProfileId == profileVM.ProfileId &&
                                                                              p.CompanyCode == CompanyCode).FirstOrDefault();
                if (existProfileGroup == null)
                {
                    profileGroupList.Add(new ProfileGroupCreateViewModel()
                    {
                        ProfileGroupCode = ConstCustomerGroupCode.Customer
                    });
                }
                else
                {
                    //Check trường hợp: nếu user đổi từ "doanh nghiệp" sang "tiêu dùng" => xóa extend "Nhóm khách hàng" 
                    if (existProfileGroup.ProfileGroupCode != ConstCustomerGroupCode.Customer)
                    {
                        _context.Entry(existProfileGroup).State = EntityState.Deleted;

                        profileGroupList.RemoveAt(0);

                        profileGroupList.Add(new ProfileGroupCreateViewModel()
                        {
                            ProfileGroupCode = ConstCustomerGroupCode.Customer
                        });
                    }
                }
            }
            if (profileGroupList != null && profileGroupList.Count > 0)
            {
                var valueLst = profileGroupList.Where(p => p.ProfileGroupCode != null).ToList();
                if (valueLst.Count == 0 && profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nhóm khách hàng\"!");
                }
                else
                {
                    foreach (var item in profileGroupList)
                    {
                        item.ProfileId = profileVM.ProfileId;
                        item.CompanyCode = CompanyCode;
                        item.CreateBy = CurrentUserId;
                    }
                    groupRepository.CreateOrUpdate(profileGroupList);
                }
            }
            else
            {
                //Nếu là KH doanh nghiệp
                if (profileVM.CustomerTypeCode == ConstCustomerType.Bussiness)
                {
                    errorList.Add("Vui lòng nhập thông tin \"Nhóm khách hàng\"!");
                }
            }
            #endregion

            //Dự án - Thi công
            //An Cường
            #region Profile_Opportunity_InternalModel
            //Xóa hết data cũ add lại mới
            var existsInternal = _context.Profile_Opportunity_InternalModel.Where(p => p.ProfileId == profile.ProfileId).ToList();
            if (existsInternal != null && existsInternal.Count > 0)
            {
                _context.Profile_Opportunity_InternalModel.RemoveRange(existsInternal);
            }
            if (profileVM.internalList != null && profileVM.internalList.Count > 0)
            {
                foreach (var internalItem in profileVM.internalList)
                {
                    if (internalItem.ConstructionId.HasValue)
                    {
                        Profile_Opportunity_InternalModel opp_internal = new Profile_Opportunity_InternalModel();
                        opp_internal.OpportunityInternalId = Guid.NewGuid();
                        opp_internal.ProfileId = profile.ProfileId;
                        opp_internal.InternalId = internalItem.ConstructionId;
                        opp_internal.CreateBy = CurrentUserId;
                        opp_internal.CreateTime = profile.CreateTime;

                        _context.Entry(opp_internal).State = EntityState.Added;
                    }
                }
            }
            #endregion
            //Đối thủ
            #region Profile_Opportunity_CompetitorModel
            //Xóa hết data cũ add lại mới
            var existsCompetitor = _context.Profile_Opportunity_CompetitorModel.Where(p => p.ProfileId == profile.ProfileId).ToList();
            if (existsCompetitor != null && existsCompetitor.Count > 0)
            {
                _context.Profile_Opportunity_CompetitorModel.RemoveRange(existsCompetitor);
            }
            if (profileVM.competitorList != null && profileVM.competitorList.Count > 0)
            {
                foreach (var competitorItem in profileVM.competitorList)
                {
                    if (competitorItem.ConstructionId.HasValue)
                    {
                        Profile_Opportunity_CompetitorModel opp_competitor = new Profile_Opportunity_CompetitorModel();
                        opp_competitor.OpportunityCompetitorId = Guid.NewGuid();
                        opp_competitor.ProfileId = profile.ProfileId;
                        opp_competitor.CompetitorId = competitorItem.ConstructionId;
                        opp_competitor.CreateBy = CurrentUserId;
                        opp_competitor.CreateTime = profile.CreateTime;

                        _context.Entry(opp_competitor).State = EntityState.Added;
                    }
                }
            }
            #endregion
            if (profile.CustomerTypeCode == ConstProfileType.Opportunity)
            {
                profile.Dropdownlist7 = profileVM.OpportunityUnit;
            }
        }

        #region Sync Profile from SAP by ForeignCode
        public bool SyncProfile(string SyncProfileForeignCode, out string error)
        {
            error = string.Empty;

            #region Get datatable
            ////Khởi tạo thư viện và kết nối
            //var _sap = new SAPRepository();
            //var destination = _sap.GetRfcWithConfig();
            ////Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.YAC_FM_CRM_GET_DATALIST);
            //var newdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //var date = new DateTime(2019, 01, 01);

            //var dt = new DataTable();
            //var companyList = _context.CompanyModel.Select(p => p.CompanyCode).ToList();
            //if (companyList != null && companyList.Count > 0)
            //{
            //    foreach (var item in companyList)
            //    {
            //        function.SetValue("IM_TYPE", 1);
            //        function.SetValue("IM_FRDATE", date.ToString("yyyyMMdd"));
            //        function.SetValue("IM_TODATE", DateTime.Now.ToString("yyyyMMdd"));
            //        function.SetValue("IM_BUKRS", item);
            //        function.SetValue("IM_VKORG", 1000);
            //        function.SetValue("IM_KUNNR", SyncProfileForeignCode.Length < 10 ? "00" + SyncProfileForeignCode : SyncProfileForeignCode);

            //        function.Invoke(destination);
            //        var datatable = function.GetTable("DATA_T").ToDataTable("DATA_T");
            //        dt.Merge(datatable);
            //    }
            //}
            #endregion

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    //Tỉnh thành
            //    var provinceLst = _context.ProvinceModel.Where(p => p.Actived == true).AsNoTracking().ToList();
            //    //Quận huyện khác 999999
            //    var districtLst = _context.DistrictModel.Where(p => p.Actived == true && p.DistrictCode != "999999").AsNoTracking().ToList();
            //    //Phường xã
            //    var wardLst = _context.WardModel.Where(p => p.DistrictId != null && p.WardName != null).AsNoTracking().ToList();
            //    //Nhân viên (SalesEmployee)
            //    var employeeList = _context.SalesEmployeeModel.Select(p => p.SalesEmployeeCode).AsNoTracking().ToList();

            //    try
            //    {
            //        foreach (DataRow item in dt.Rows)
            //        {
            //            try
            //            {
            //                var ProfileId = Guid.NewGuid();
            //                var ProfileForeignCode = item["KUNNR"].ToString();
            //                //Nếu trên SAP trả về mã giống với mã user nhập vào thì tiến hành đồng bộ
            //                if (ProfileForeignCode == SyncProfileForeignCode)
            //                {
            //                    var ProfileCode_Str = item["LOCCO"].ToString();
            //                    int ProfileCode = 0;
            //                    if (!string.IsNullOrEmpty(ProfileCode_Str))
            //                    {
            //                        ProfileCode = Convert.ToInt32(item["LOCCO"].ToString());
            //                    }
            //                    var CompanyCode = item["BUKRS"].ToString();
            //                    bool isAddressBook = false;
            //                    if (ProfileForeignCode.Length > 8)
            //                    {
            //                        isAddressBook = true;
            //                    }
            //                    //DateTime
            //                    UtilitiesRepository _utilities = new UtilitiesRepository();
            //                    var CreateTime = _utilities.ConvertDateTime(item["ERDAT"].ToString());
            //                    var Phone = item["TEL_NUMBER"].ToString();
            //                    var TaxNo = item["STCEG"].ToString();
            //                    //Trong nước | Nước ngoài: 
            //                    //1. Nếu LAND1 == "VN" => trong nước
            //                    //2. Nếu LAND1 != "VN" => nước ngoài
            //                    bool isForeignCustomer = false;
            //                    string CountryCode = item["LAND1"].ToString();
            //                    if (!string.IsNullOrEmpty(CountryCode) && CountryCode != "VN")
            //                    {
            //                        isForeignCustomer = true;
            //                    }
            //                    //Tỉnh thành
            //                    var ProvinceCode = item["VKGRP"].ToString();
            //                    Guid? ProvinceId = null;
            //                    //Nếu là khách nước ngoài => lưu province là quốc gia
            //                    if (isForeignCustomer == true && !string.IsNullOrEmpty(CountryCode))
            //                    {
            //                        ProvinceId = provinceLst.Where(p => p.ProvinceCode == CountryCode)
            //                                        .Select(p => p.ProvinceId).FirstOrDefault();
            //                    }
            //                    else
            //                    {
            //                        if (!string.IsNullOrEmpty(ProvinceCode))
            //                        {
            //                            ProvinceId = provinceLst.Where(p => p.ProvinceCode == ProvinceCode)
            //                                             .Select(p => p.ProvinceId).FirstOrDefault();
            //                        }
            //                        else
            //                        {
            //                            //Nếu không có Tỉnh thành => Lưu "Khác"
            //                            ProvinceId = Guid.Parse("33b02d3c-c3dc-41ca-a909-09c951b9ad22");
            //                        }
            //                    }

            //                    //Quận huyện
            //                    var DistrictCode = item["BZIRK"].ToString();
            //                    Guid? DistrictId = null;
            //                    if (!string.IsNullOrEmpty(DistrictCode) && DistrictCode != "999999")
            //                    {
            //                        DistrictId = districtLst.Where(p => p.DistrictCode == DistrictCode)
            //                                                .Select(p => p.DistrictId).FirstOrDefault();
            //                    }
            //                    //Phường xã
            //                    var WardName = item["BAHNS_BAHNE"].ToString();
            //                    Guid? WardId = null;
            //                    if (!string.IsNullOrEmpty(WardName) && DistrictId.HasValue)
            //                    {
            //                        var ward = wardLst.Where(p => WardName.Contains(p.WardName) && p.DistrictId == DistrictId).FirstOrDefault();
            //                        if (ward != null)
            //                        {
            //                            WardId = ward.WardId;
            //                        }
            //                    }
            //                    //Phân loại khách hàng: 1.KH Tiêu dùng. 2.KH Doanh nghiệp
            //                    var PhanLoaiKH = item["NHOMKH"].ToString();
            //                    var ProfileTypeCode = string.Empty;
            //                    if (!string.IsNullOrEmpty(PhanLoaiKH))
            //                    {
            //                        if (PhanLoaiKH == "1")
            //                        {
            //                            ProfileTypeCode = ConstCustomerType.Customer;
            //                        }
            //                        else if (PhanLoaiKH == "2")
            //                        {
            //                            ProfileTypeCode = ConstCustomerType.Bussiness;
            //                        }
            //                    }

            //                    //Email
            //                    string Email = item["SMTP_ADDR"].ToString();

            //                    #region Code cũ
            //                    /*
            //                    #region Profile
            //                    var exist = _context.ProfileModel.FirstOrDefault(p => p.ProfileForeignCode == ProfileForeignCode);
            //                    if (exist == null)
            //                    {
            //                        ProfileModel model = new ProfileModel();
            //                        //1. GUID
            //                        model.ProfileId = ProfileId;
            //                        //2. ProfileCode
            //                        if (ProfileCode != 0)
            //                        {
            //                            model.ProfileCode = ProfileCode;
            //                        }
            //                        //3. ProfileForeignCode
            //                        model.ProfileForeignCode = ProfileForeignCode;
            //                        //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            //                        model.isForeignCustomer = false;
            //                        //5. Danh xưng
            //                        var Title = item["ANRED"].ToString();
            //                        if (Title.Length > 10)
            //                        {
            //                            Title = null;
            //                        }
            //                        model.Title = Title;
            //                        //6. Loại
            //                        if (isAddressBook == true)
            //                        {
            //                            model.CustomerTypeCode = ConstCustomerType.Contact;
            //                        }
            //                        else
            //                        {
            //                            model.CustomerTypeCode = ConstCustomerType.Account;

            //                            //Phân loại KH
            //                            ProfileTypeModel profileType = new ProfileTypeModel();
            //                            profileType.ProfileTypeId = Guid.NewGuid();
            //                            profileType.ProfileId = ProfileId;
            //                            if (model.Title == "Company")
            //                            {
            //                                profileType.CustomerTypeCode = ConstCustomerType.Bussiness;
            //                            }
            //                            else
            //                            {
            //                                profileType.CustomerTypeCode = ConstCustomerType.Customer;
            //                            }
            //                            profileType.CompanyCode = item["BUKRS"].ToString();
            //                            profileType.CreateBy = SYSTEM;
            //                            profileType.CreateTime = CreateTime;
            //                            _context.Entry(profileType).State = EntityState.Added;
            //                        }
            //                        //7. Họ va Tên|Tên công ty
            //                        model.ProfileName = item["FULLNAME"].ToString();
            //                        //8. Tên ngắn
            //                        model.ProfileShortName = item["NAME1"].ToString();
            //                        //9. Tên viết tắt
            //                        model.AbbreviatedName = model.ProfileName.ToAbbreviation();
            //                        //10. Ngày sinh
            //                        //11. Tháng sinh
            //                        //12. Năm sinh
            //                        //13. Độ tuổi
            //                        //14. Số điện thoại
            //                        try
            //                        {
            //                            if (!string.IsNullOrEmpty(Phone))
            //                            {
            //                                if (Phone.Contains("-"))
            //                                {
            //                                    var arr = Phone.Split('-').ToList();
            //                                    var PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(arr[0]);
            //                                    var phoneArray = arr.Where(p => !p.Contains(arr[0])).ToList();
            //                                    foreach (var phoneItem in phoneArray)
            //                                    {
            //                                        ProfilePhoneModel phoneModel = new ProfilePhoneModel();
            //                                        phoneModel.PhoneId = Guid.NewGuid();
            //                                        phoneModel.ProfileId = ProfileId;
            //                                        phoneModel.PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(phoneItem).Trim();
            //                                        if (PhoneNumber != phoneModel.PhoneNumber && !string.IsNullOrEmpty(phoneModel.PhoneNumber))
            //                                        {
            //                                            _context.Entry(phoneModel).State = EntityState.Added;
            //                                            //phoneList.Add(phoneModel);
            //                                        }
            //                                    }
            //                                    model.Phone = PhoneNumber.Trim();
            //                                }
            //                                else
            //                                {
            //                                    model.Phone = RepositoryLibrary.ConvertToNoSpecialCharacters(Phone).Trim();
            //                                }
            //                            }
            //                        }
            //                        catch (Exception)
            //                        {
            //                            // ghi log
            //                            continue;
            //                        }
            //                        //15. Email
            //                        //16. Khu vực
            //                        model.SaleOfficeCode = item["VKBUR"].ToString();
            //                        //17. Địa chỉ
            //                        model.Address = item["ADDRESS"].ToString();
            //                        //18. Tỉnh/Thành phố
            //                        model.ProvinceId = ProvinceId;
            //                        //19. Quận/Huyện
            //                        model.DistrictId = DistrictId;
            //                        //20. Phường/Xã
            //                        //21. Ghi chú
            //                        //22. Ngày ghé thăm
            //                        //23. Trạng thái
            //                        model.Actived = true;
            //                        //24. Hình ảnh
            //                        //25. Nhân viên tạo
            //                        //26. Tạo tại công ty
            //                        model.CreateAtCompany = item["BUKRS"].ToString();
            //                        //27. Tạo tại cửa hàng
            //                        model.CreateAtSaleOrg = item["VKORG"].ToString();
            //                        //28. CreateBy
            //                        model.CreateBy = SYSTEM;
            //                        //29. Thời gian tạo
            //                        model.CreateTime = CreateTime;
            //                        //30. LastEditBy
            //                        //31. Thời gian tạo
            //                        //32. Phân nhóm KH/ Customer Account Group
            //                        model.CustomerAccountGroupCode = item["KTOKD"].ToString();
            //                        //33.Mã nhóm  KH/ Customer Group
            //                        model.CustomerGroupCode = item["KDGRP"].ToString();
            //                        //34. Mã Điều khoản thanh toán/ Payment Term
            //                        model.PaymentTermCode = item["ZTERM"].ToString();
            //                        //35.Mã phân loại doanh thu/ Customer Account Assignment Group
            //                        model.CustomerAccountAssignmentGroupCode = item["KTGRD"].ToString();
            //                        //36. Mã phân nhóm dòng tiền/ Cash mgmt Group
            //                        model.CashMgmtGroupCode = item["FDGRV"].ToString();
            //                        //37. Mã tài khoản công nợ/ Reconcile Account
            //                        model.ReconcileAccountCode = item["AKONT"].ToString();
            //                        //38. Số điện thoại (SAP)
            //                        model.SAPPhone = Phone;
            //                        //39. Mã số thuế TaxNo
            //                        if (!string.IsNullOrEmpty(TaxNo) && !TaxNo.Contains("X"))
            //                        {
            //                            model.TaxNo = TaxNo;
            //                        }
            //                        _context.Entry(model).State = EntityState.Added;
            //                    }
            //                    else
            //                    {
            //                        //Phân loại KH
            //                        var existProfileType = _context.ProfileTypeModel.FirstOrDefault(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode);
            //                        if (existProfileType == null)
            //                        {
            //                            ProfileTypeModel profileType = new ProfileTypeModel();
            //                            profileType.ProfileTypeId = Guid.NewGuid();
            //                            profileType.ProfileId = ProfileId;
            //                            if (exist.Title == "Company")
            //                            {
            //                                profileType.CustomerTypeCode = ConstCustomerType.Bussiness;
            //                            }
            //                            else
            //                            {
            //                                profileType.CustomerTypeCode = ConstCustomerType.Customer;
            //                            }
            //                            profileType.CompanyCode = item["BUKRS"].ToString();
            //                            profileType.CreateBy = SYSTEM;
            //                            profileType.CreateTime = CreateTime;
            //                            _context.Entry(profileType).State = EntityState.Added;
            //                        }
            //                    }
            //                    #endregion

            //                    #region PersonInChargeModel
            //                    var existPerson = _context.PersonInChargeModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).ToList();
            //                    if (existPerson == null || existPerson.Count == 0)
            //                    {
            //                        //Sale Employee
            //                        var EmployeeCode = item["EMPLOYEENO"].ToString();
            //                        if (!string.IsNullOrEmpty(EmployeeCode) && EmployeeCode != "00000000" && employeeList.Contains(EmployeeCode))
            //                        {
            //                            PersonInChargeModel person = new PersonInChargeModel();
            //                            person.PersonInChargeId = Guid.NewGuid();
            //                            person.ProfileId = ProfileId;
            //                            person.SalesEmployeeCode = EmployeeCode;
            //                            person.RoleCode = ConstRoleCode.SALE_EMPLOYEE;
            //                            person.CompanyCode = CompanyCode;
            //                            person.CreateBy = SYSTEM;
            //                            person.CreateTime = CreateTime;
            //                            _context.Entry(person).State = EntityState.Added;
            //                        }

            //                        //Sale Employee 2
            //                        var EmployeeCode2 = item["SE_PERNR"].ToString();
            //                        if (!string.IsNullOrEmpty(EmployeeCode2) && EmployeeCode2 != "00000000" && EmployeeCode2 != EmployeeCode && employeeList.Contains(EmployeeCode2))
            //                        {
            //                            PersonInChargeModel person = new PersonInChargeModel();
            //                            person.PersonInChargeId = Guid.NewGuid();
            //                            person.ProfileId = ProfileId;
            //                            person.SalesEmployeeCode = EmployeeCode2;
            //                            person.RoleCode = ConstRoleCode.SALE_EMPLOYEE2;
            //                            person.CompanyCode = CompanyCode;
            //                            person.CreateBy = SYSTEM;
            //                            person.CreateTime = CreateTime;
            //                            _context.Entry(person).State = EntityState.Added;
            //                        }
            //                    }
            //                    #endregion

            //                    #region ProfileGroupModel
            //                    var existProfileGroup = _context.ProfileGroupModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            //                    if (!string.IsNullOrEmpty(item["KDGRP"].ToString()) && existProfileGroup == null)
            //                    {
            //                        ProfileGroupModel profileGroup = new ProfileGroupModel();
            //                        profileGroup.ProfileGroupId = Guid.NewGuid();
            //                        profileGroup.ProfileId = ProfileId;
            //                        profileGroup.ProfileGroupCode = item["KDGRP"].ToString();
            //                        profileGroup.CompanyCode = CompanyCode;
            //                        profileGroup.CreateBy = SYSTEM;
            //                        profileGroup.CreateTime = DateTime.Now;
            //                        _context.Entry(profileGroup).State = EntityState.Added;
            //                    }
            //                    #endregion
            //                    */
            //                    #endregion

            //                    #region Profile
            //                    //Check profile exists có mã CRM
            //                    var existProfileWithCRMCode = _context.ProfileModel.Where(p => p.CustomerTypeCode == ConstCustomerType.Account && p.ProfileCode == ProfileCode).FirstOrDefault();
            //                    //Nếu đã có profile trong DB và chưa có Mã SAP => cập nhật
            //                    if (existProfileWithCRMCode != null
            //                        //Chưa có mã SAP thì cập nhật
            //                        //&& string.IsNullOrEmpty(existProfileWithCRMCode.ProfileForeignCode)
            //                        )
            //                    {
            //                        if (isAddressBook == false)
            //                        {
            //                            //Nếu mã SAP đã tồn tại => check mã CRM
            //                            // => Trùng mã CRM thì cập nhật
            //                            // => Khác mã CRM thì báo cho user
            //                            var existsSAPProfile = _context.ProfileModel.Where(p => p.ProfileForeignCode == ProfileForeignCode).Select(p => new
            //                            {
            //                                p.ProfileCode,
            //                                p.ProfileForeignCode
            //                            }).FirstOrDefault();
            //                            //CRM chưa có mã SAP hoặc trùng mã CRM thì mới cập nhật
            //                            if (existsSAPProfile == null || existsSAPProfile.ProfileCode == ProfileCode)
            //                            {
            //                                //1. GUID
            //                                ProfileId = existProfileWithCRMCode.ProfileId;
            //                                //2. ProfileCode => cannot update identity column
            //                                //if (ProfileCode != 0)
            //                                //{
            //                                //    existProfileWithCRMCode.ProfileCode = ProfileCode;
            //                                //}
            //                                //3. ProfileForeignCode
            //                                existProfileWithCRMCode.ProfileForeignCode = ProfileForeignCode;
            //                                //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            //                                existProfileWithCRMCode.isForeignCustomer = isForeignCustomer;
            //                                existProfileWithCRMCode.CountryCode = CountryCode;
            //                                //5. Danh xưng
            //                                var Title = item["ANRED"].ToString();
            //                                if (Title.Length > 10)
            //                                {
            //                                    Title = null;
            //                                }
            //                                existProfileWithCRMCode.Title = Title;
            //                                ////6. Loại
            //                                existProfileWithCRMCode.CustomerTypeCode = ConstCustomerType.Account;

            //                                //Phân loại KH
            //                                var existProfileType = _context.ProfileTypeModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            //                                if (existProfileType == null)
            //                                {
            //                                    ProfileTypeModel profileType = new ProfileTypeModel();
            //                                    profileType.ProfileTypeId = Guid.NewGuid();
            //                                    profileType.ProfileId = ProfileId;
            //                                    //if (existProfileWithCRMCode.Title == "Company")
            //                                    //{
            //                                    //    profileType.CustomerTypeCode = ConstCustomerType.Bussiness;
            //                                    //}
            //                                    //else
            //                                    //{
            //                                    //    profileType.CustomerTypeCode = ConstCustomerType.Customer;
            //                                    //}
            //                                    profileType.CustomerTypeCode = ProfileTypeCode;
            //                                    profileType.CompanyCode = item["BUKRS"].ToString();
            //                                    profileType.CreateBy = SYSTEM;
            //                                    profileType.CreateTime = CreateTime;
            //                                    _context.Entry(profileType).State = EntityState.Added;
            //                                    //typeList.Add(profileType);
            //                                }
            //                                else
            //                                {
            //                                    existProfileType.CustomerTypeCode = ProfileTypeCode;
            //                                    _context.Entry(existProfileType).State = EntityState.Modified;
            //                                }
            //                                //7. Họ va Tên|Tên công ty
            //                                existProfileWithCRMCode.ProfileName = item["FULLNAME"].ToString();
            //                                //8. Tên ngắn
            //                                existProfileWithCRMCode.ProfileShortName = item["NAME1"].ToString();
            //                                //9. Tên viết tắt
            //                                existProfileWithCRMCode.AbbreviatedName = existProfileWithCRMCode.ProfileName?.ToAbbreviation();
            //                                //10. Ngày sinh
            //                                //11. Tháng sinh
            //                                //12. Năm sinh
            //                                //13. Độ tuổi
            //                                //14. Số điện thoại
            //                                try
            //                                {
            //                                    //Delete all
            //                                    var phoneExistLst = _context.ProfilePhoneModel.Where(p => p.ProfileId == ProfileId).ToList();
            //                                    if (phoneExistLst != null && phoneExistLst.Count > 0)
            //                                    {
            //                                        _context.ProfilePhoneModel.RemoveRange(phoneExistLst);
            //                                    }
            //                                    //Add again
            //                                    if (!string.IsNullOrEmpty(Phone))
            //                                    {
            //                                        if (Phone.Contains("-"))
            //                                        {
            //                                            var arr = Phone.Split('-').ToList();
            //                                            var PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(arr[0]);

            //                                            var phoneArray = arr.Where(p => !p.Contains(arr[0])).ToList();
            //                                            foreach (var phoneItem in phoneArray)
            //                                            {
            //                                                ProfilePhoneModel phoneModel = new ProfilePhoneModel();
            //                                                phoneModel.PhoneId = Guid.NewGuid();
            //                                                phoneModel.ProfileId = ProfileId;
            //                                                phoneModel.PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(phoneItem).Trim();
            //                                                if (PhoneNumber != phoneModel.PhoneNumber && !string.IsNullOrEmpty(phoneModel.PhoneNumber))
            //                                                {
            //                                                    _context.Entry(phoneModel).State = EntityState.Added;
            //                                                    //phoneList.Add(phoneModel);
            //                                                }
            //                                            }
            //                                            existProfileWithCRMCode.Phone = PhoneNumber.Trim();
            //                                        }
            //                                        else
            //                                        {
            //                                            existProfileWithCRMCode.Phone = RepositoryLibrary.ConvertToNoSpecialCharacters(Phone);
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        existProfileWithCRMCode.Phone = null;
            //                                    }
            //                                }
            //                                catch (Exception ex)
            //                                {
            //                                    // ghi log
            //                                    error = ex.Message;
            //                                    if (ex.InnerException != null)
            //                                    {
            //                                        error = ex.InnerException.Message;
            //                                        if (ex.InnerException.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.InnerException.Message;
            //                                        }
            //                                    }
            //                                    continue;
            //                                }
            //                                //15. Email
            //                                //existProfileWithCRMCode.Email = item["SMTP_ADDR"].ToString();
            //                                //if (IsValidEmail(existProfileWithCRMCode.Email) == false)
            //                                //{
            //                                //    existProfileWithCRMCode.Note = string.Format("(Email: {0})", existProfileWithCRMCode.Email);
            //                                //    existProfileWithCRMCode.Email = null;
            //                                //}
            //                                //else
            //                                //{
            //                                //    if (!string.IsNullOrEmpty(existProfileWithCRMCode.Note) && existProfileWithCRMCode.Note.Contains("Email:"))
            //                                //    {
            //                                //        existProfileWithCRMCode.Note = null;
            //                                //    }
            //                                //}
            //                                if (!string.IsNullOrEmpty(Email))
            //                                {
            //                                    try
            //                                    {
            //                                        //Delete all
            //                                        var emailExistLst = _context.ProfileEmailModel.Where(p => p.ProfileId == ProfileId).ToList();
            //                                        if (emailExistLst != null && emailExistLst.Count > 0)
            //                                        {
            //                                            _context.ProfileEmailModel.RemoveRange(emailExistLst);
            //                                        }
            //                                        //Add again
            //                                        if (Email.Contains(";"))
            //                                        {
            //                                            var emailArray = Email.Split(';').ToList();
            //                                            foreach (var emailItem in emailArray)
            //                                            {
            //                                                if (IsValidEmail(emailItem) == true)
            //                                                {
            //                                                    ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                    emailModel.EmailId = Guid.NewGuid();
            //                                                    emailModel.ProfileId = ProfileId;
            //                                                    emailModel.Email = emailItem.Trim();
            //                                                    _context.Entry(emailModel).State = EntityState.Added;
            //                                                }
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            if (IsValidEmail(Email) == true)
            //                                            {
            //                                                ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                emailModel.EmailId = Guid.NewGuid();
            //                                                emailModel.ProfileId = ProfileId;
            //                                                emailModel.Email = Email.Trim();
            //                                                _context.Entry(emailModel).State = EntityState.Added;
            //                                            }
            //                                        }
            //                                    }
            //                                    catch (Exception ex)
            //                                    {
            //                                        // ghi log
            //                                        error = ex.Message;
            //                                        if (ex.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.Message;
            //                                            if (ex.InnerException.InnerException != null)
            //                                            {
            //                                                error = ex.InnerException.InnerException.Message;
            //                                            }
            //                                        }
            //                                        continue;
            //                                    }
            //                                }
            //                                //16. Khu vực
            //                                //existProfileWithCRMCode.SaleOfficeCode = item["VKBUR"].ToString();
            //                                //=> Khi đồng bộ từ SAP về dựa vào tỉnh thành mình set luôn khu vực chứ không đồng bộ data khu vực từ SAP về tránh SAP sai
            //                                var SaleOfficeCode = provinceLst.Where(p => p.ProvinceId == ProvinceId).Select(p => p.Area).FirstOrDefault();
            //                                if (!string.IsNullOrEmpty(SaleOfficeCode))
            //                                {
            //                                    existProfileWithCRMCode.SaleOfficeCode = SaleOfficeCode;
            //                                }
            //                                //17. Địa chỉ
            //                                //existProfileWithCRMCode.Address = item["ADDRESS"].ToString();
            //                                existProfileWithCRMCode.Address = !string.IsNullOrEmpty(item["STCD5"].ToString()) ? item["STCD5"].ToString().FirstCharToUpper() : item["ADDRESS"].ToString().FirstCharToUpper();
            //                                //18. Tỉnh/Thành phố
            //                                existProfileWithCRMCode.ProvinceId = ProvinceId;
            //                                //19. Quận/Huyện
            //                                existProfileWithCRMCode.DistrictId = DistrictId;
            //                                //20. Phường/Xã
            //                                existProfileWithCRMCode.WardId = WardId;
            //                                //21. Ghi chú
            //                                //22. Ngày ghé thăm
            //                                //23. Trạng thái (not update this field)
            //                                //Trạng thái hoạt động: X: ngưng hoạt động
            //                                string TrangThaiHoatDong = item["LOEVM"].ToString();
            //                                bool isActived = true;
            //                                if (!string.IsNullOrEmpty(TrangThaiHoatDong) && TrangThaiHoatDong.ToUpper() == "X")
            //                                {
            //                                    isActived = false;
            //                                }
            //                                if (existProfileWithCRMCode.Actived != isActived)
            //                                {
            //                                    existProfileWithCRMCode.Actived = isActived;
            //                                }
            //                                //24. Hình ảnh
            //                                //25. Nhân viên tạo
            //                                //26. Tạo tại công ty 
            //                                existProfileWithCRMCode.CreateAtCompany = item["BUKRS"].ToString();
            //                                //27. Tạo tại cửa hàng
            //                                existProfileWithCRMCode.CreateAtSaleOrg = item["VKORG"].ToString();
            //                                //28. CreateBy (not update this field)
            //                                //29. Thời gian tạo
            //                                //exist.CreateTime = CreateTime;
            //                                //30. LastEditBy
            //                                existProfileWithCRMCode.LastEditBy = SYSTEM;
            //                                //31. Thời gian sửa
            //                                existProfileWithCRMCode.LastEditTime = DateTime.Now;
            //                                //32. Phân nhóm KH/ Customer Account Group
            //                                existProfileWithCRMCode.CustomerAccountGroupCode = item["KTOKD"].ToString();
            //                                //33.Mã nhóm  KH/ Customer Group
            //                                existProfileWithCRMCode.CustomerGroupCode = item["KDGRP"].ToString();
            //                                //34. Mã Điều khoản thanh toán/ Payment Term
            //                                existProfileWithCRMCode.PaymentTermCode = item["ZTERM"].ToString();
            //                                //35.Mã phân loại doanh thu/ Customer Account Assignment Group
            //                                existProfileWithCRMCode.CustomerAccountAssignmentGroupCode = item["KTGRD"].ToString();
            //                                //36. Mã phân nhóm dòng tiền/ Cash mgmt Group
            //                                existProfileWithCRMCode.CashMgmtGroupCode = item["FDGRV"].ToString();
            //                                //37. Mã tài khoản công nợ/ Reconcile Account
            //                                existProfileWithCRMCode.ReconcileAccountCode = item["AKONT"].ToString();
            //                                //38. Số điện thoại (SAP)
            //                                existProfileWithCRMCode.SAPPhone = Phone;
            //                                //39. Mã số thuế TaxNo
            //                                if (!string.IsNullOrEmpty(TaxNo) && !TaxNo.Contains("X"))
            //                                {
            //                                    existProfileWithCRMCode.TaxNo = TaxNo;
            //                                }
            //                                existProfileWithCRMCode.CustomerSourceCode = "SHOWROOM";
            //                                //Loại địa chỉ
            //                                string LoaiDiaChi = item["ADDRTYPE"].ToString();
            //                                if (!string.IsNullOrEmpty(LoaiDiaChi))
            //                                {
            //                                    existProfileWithCRMCode.AddressTypeCode = LoaiDiaChi;
            //                                }
            //                                //Ngành nghề
            //                                string NganhNghe = item["BRSCH"].ToString();
            //                                if (!string.IsNullOrEmpty(NganhNghe))
            //                                {
            //                                    //NganhNghe = NganhNghe.PadLeft(4, '0');

            //                                    var existProfileCareer = _context.ProfileCareerModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            //                                    if (existProfileCareer == null)
            //                                    {
            //                                        ProfileCareerModel profileCareer = new ProfileCareerModel();
            //                                        profileCareer.ProfileCareerId = Guid.NewGuid();
            //                                        profileCareer.ProfileId = ProfileId;
            //                                        profileCareer.ProfileCareerCode = NganhNghe;
            //                                        profileCareer.CompanyCode = item["BUKRS"].ToString();
            //                                        profileCareer.CreateBy = SYSTEM;
            //                                        profileCareer.CreateTime = CreateTime;
            //                                        _context.Entry(profileCareer).State = EntityState.Added;
            //                                    }
            //                                    else
            //                                    {
            //                                        existProfileCareer.ProfileCareerCode = NganhNghe;
            //                                        _context.Entry(existProfileCareer).State = EntityState.Modified;
            //                                    }
            //                                }
            //                                //Yêu cầu tạo khách ở ECC
            //                                if (existProfileWithCRMCode.isCreateRequest == null)
            //                                {
            //                                    existProfileWithCRMCode.isCreateRequest = false;
            //                                    existProfileWithCRMCode.CreateRequestTime = DateTime.Now;
            //                                }

            //                                _context.Entry(existProfileWithCRMCode).State = EntityState.Modified;

            //                                //thêm nhóm KH
            //                                AddProfileGroup(item, CompanyCode, ProfileId);

            //                                //thêm NVKD
            //                                AddPersonInCharge(employeeList, item, CompanyCode, ProfileId, CreateTime);
            //                            }
            //                            else
            //                            {
            //                                error = "Sync Profile error: Update CRM: " + ProfileCode + " - Voi SAP: " + ProfileForeignCode + ". Nhung da Ton tai:" + existsSAPProfile.ProfileCode;
            //                            }
            //                            //profileCodeList.Add(ProfileForeignCode);
            //                        }
            //                        //cập nhật address book
            //                        else
            //                        {

            //                        }
            //                    }


            //                    //Nếu không phải là yêu cầu cập nhật => tìm theo mã SAP
            //                    else if (existProfileWithCRMCode == null)
            //                    {
            //                        //Tìm theo mã SAP
            //                        var existProfileWithSAPCode = _context.ProfileModel.Where(p => p.ProfileForeignCode == ProfileForeignCode).FirstOrDefault();
            //                        if (existProfileWithSAPCode == null)
            //                        {
            //                            if (isAddressBook == false)
            //                            {
            //                                ProfileModel model = new ProfileModel();
            //                                //1. GUID
            //                                model.ProfileId = ProfileId;
            //                                //2. ProfileCode
            //                                if (ProfileCode != 0)
            //                                {
            //                                    model.ProfileCode = ProfileCode;
            //                                }
            //                                //3. ProfileForeignCode
            //                                model.ProfileForeignCode = ProfileForeignCode;
            //                                //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            //                                model.isForeignCustomer = isForeignCustomer;
            //                                model.CountryCode = CountryCode;
            //                                //5. Danh xưng
            //                                var Title = item["ANRED"].ToString();
            //                                if (Title.Length > 10)
            //                                {
            //                                    Title = null;
            //                                }
            //                                model.Title = Title;
            //                                //6. Loại
            //                                model.CustomerTypeCode = ConstCustomerType.Account;

            //                                #region  //Phân loại KH
            //                                ProfileTypeModel profileType = new ProfileTypeModel();
            //                                profileType.ProfileTypeId = Guid.NewGuid();
            //                                profileType.ProfileId = ProfileId;
            //                                //if (model.Title == "Company")
            //                                //{
            //                                //    profileType.CustomerTypeCode = ConstCustomerType.Bussiness;
            //                                //}
            //                                //else
            //                                //{
            //                                //    profileType.CustomerTypeCode = ConstCustomerType.Customer;
            //                                //}
            //                                profileType.CustomerTypeCode = ProfileTypeCode;
            //                                profileType.CompanyCode = item["BUKRS"].ToString();
            //                                profileType.CreateBy = SYSTEM;
            //                                profileType.CreateTime = CreateTime;
            //                                _context.Entry(profileType).State = EntityState.Added;
            //                                #endregion
            //                                //typeList.Add(profileType);
            //                                //7. Họ va Tên|Tên công ty
            //                                model.ProfileName = item["FULLNAME"].ToString();
            //                                //8. Tên ngắn
            //                                model.ProfileShortName = item["NAME1"].ToString();
            //                                //9. Tên viết tắt
            //                                model.AbbreviatedName = model.ProfileName.ToAbbreviation();
            //                                //10. Ngày sinh
            //                                //11. Tháng sinh
            //                                //12. Năm sinh
            //                                //13. Độ tuổi
            //                                //14. Số điện thoại
            //                                try
            //                                {
            //                                    if (!string.IsNullOrEmpty(Phone))
            //                                    {
            //                                        if (Phone.Contains("-"))
            //                                        {
            //                                            var arr = Phone.Split('-').ToList();
            //                                            var PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(arr[0]);
            //                                            var phoneArray = arr.Where(p => !p.Contains(arr[0])).ToList();
            //                                            foreach (var phoneItem in phoneArray)
            //                                            {
            //                                                ProfilePhoneModel phoneModel = new ProfilePhoneModel();
            //                                                phoneModel.PhoneId = Guid.NewGuid();
            //                                                phoneModel.ProfileId = ProfileId;
            //                                                phoneModel.PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(phoneItem).Trim();
            //                                                if (PhoneNumber != phoneModel.PhoneNumber && !string.IsNullOrEmpty(phoneModel.PhoneNumber))
            //                                                {
            //                                                    _context.Entry(phoneModel).State = EntityState.Added;
            //                                                    //phoneList.Add(phoneModel);
            //                                                }
            //                                            }
            //                                            model.Phone = PhoneNumber.Trim();
            //                                        }
            //                                        else
            //                                        {
            //                                            model.Phone = RepositoryLibrary.ConvertToNoSpecialCharacters(Phone).Trim();
            //                                        }
            //                                    }
            //                                }
            //                                catch (Exception ex)
            //                                {
            //                                    // ghi log
            //                                    error = ex.Message;
            //                                    if (ex.InnerException != null)
            //                                    {
            //                                        error = ex.InnerException.Message;
            //                                        if (ex.InnerException.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.InnerException.Message;
            //                                        }
            //                                    }
            //                                    continue;
            //                                }
            //                                //15. Email
            //                                //model.Email = item["SMTP_ADDR"].ToString();
            //                                //if (!string.IsNullOrEmpty(model.Email) && IsValidEmail(model.Email) == false)
            //                                //{
            //                                //    model.Note = string.Format("(Email: {0})", model.Email);
            //                                //    model.Email = null;
            //                                //}
            //                                //else
            //                                //{
            //                                //    model.Email = null;
            //                                //}
            //                                if (!string.IsNullOrEmpty(Email))
            //                                {
            //                                    try
            //                                    {
            //                                        //Delete all
            //                                        var emailExistLst = _context.ProfileEmailModel.Where(p => p.ProfileId == ProfileId).ToList();
            //                                        if (emailExistLst != null && emailExistLst.Count > 0)
            //                                        {
            //                                            _context.ProfileEmailModel.RemoveRange(emailExistLst);
            //                                        }
            //                                        //Add again
            //                                        if (Email.Contains(";"))
            //                                        {
            //                                            var emailArray = Email.Split(';').ToList();
            //                                            foreach (var emailItem in emailArray)
            //                                            {
            //                                                if (IsValidEmail(emailItem) == true)
            //                                                {
            //                                                    ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                    emailModel.EmailId = Guid.NewGuid();
            //                                                    emailModel.ProfileId = ProfileId;
            //                                                    emailModel.Email = emailItem.Trim();
            //                                                    _context.Entry(emailModel).State = EntityState.Added;
            //                                                }
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            if (IsValidEmail(Email) == true)
            //                                            {
            //                                                ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                emailModel.EmailId = Guid.NewGuid();
            //                                                emailModel.ProfileId = ProfileId;
            //                                                emailModel.Email = Email.Trim();
            //                                                _context.Entry(emailModel).State = EntityState.Added;
            //                                            }
            //                                        }
            //                                    }
            //                                    catch (Exception ex)
            //                                    {
            //                                        // ghi log
            //                                        error = ex.Message;
            //                                        if (ex.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.Message;
            //                                            if (ex.InnerException.InnerException != null)
            //                                            {
            //                                                error = ex.InnerException.InnerException.Message;
            //                                            }
            //                                        }
            //                                        continue;
            //                                    }
            //                                }
            //                                //16. Khu vực
            //                                //model.SaleOfficeCode = item["VKBUR"].ToString();
            //                                //=> Khi đồng bộ từ SAP về dựa vào tỉnh thành mình set luôn khu vực chứ không đồng bộ data khu vực từ SAP về tránh SAP sai
            //                                var SaleOfficeCode = provinceLst.Where(p => p.ProvinceId == ProvinceId).Select(p => p.Area).FirstOrDefault();
            //                                if (!string.IsNullOrEmpty(SaleOfficeCode))
            //                                {
            //                                    model.SaleOfficeCode = SaleOfficeCode;
            //                                }
            //                                //17. Địa chỉ
            //                                //model.Address = item["ADDRESS"].ToString();
            //                                model.Address = !string.IsNullOrEmpty(item["STCD5"].ToString()) ? item["STCD5"].ToString().FirstCharToUpper() : item["ADDRESS"].ToString().FirstCharToUpper();
            //                                //18. Tỉnh/Thành phố
            //                                model.ProvinceId = ProvinceId;
            //                                //19. Quận/Huyện
            //                                model.DistrictId = DistrictId;
            //                                //20. Phường/Xã
            //                                model.WardId = WardId;
            //                                //21. Ghi chú
            //                                //22. Ngày ghé thăm
            //                                //23. Trạng thái
            //                                //model.Actived = true;
            //                                //Trạng thái hoạt động: X: ngưng hoạt động
            //                                string TrangThaiHoatDong = item["LOEVM"].ToString();
            //                                bool isActived = true;
            //                                if (!string.IsNullOrEmpty(TrangThaiHoatDong) && TrangThaiHoatDong.ToUpper() == "X")
            //                                {
            //                                    isActived = false;
            //                                }
            //                                model.Actived = isActived;
            //                                //24. Hình ảnh
            //                                //25. Nhân viên tạo
            //                                //26. Tạo tại công ty
            //                                model.CreateAtCompany = item["BUKRS"].ToString();
            //                                //27. Tạo tại cửa hàng
            //                                model.CreateAtSaleOrg = item["VKORG"].ToString();
            //                                //28. CreateBy
            //                                model.CreateBy = SYSTEM;
            //                                //29. Thời gian tạo
            //                                model.CreateTime = CreateTime;
            //                                //30. LastEditBy
            //                                //31. Thời gian tạo
            //                                //32. Phân nhóm KH/ Customer Account Group
            //                                model.CustomerAccountGroupCode = item["KTOKD"].ToString();
            //                                //33.Mã nhóm  KH/ Customer Group
            //                                model.CustomerGroupCode = item["KDGRP"].ToString();
            //                                //34. Mã Điều khoản thanh toán/ Payment Term
            //                                model.PaymentTermCode = item["ZTERM"].ToString();
            //                                //35.Mã phân loại doanh thu/ Customer Account Assignment Group
            //                                model.CustomerAccountAssignmentGroupCode = item["KTGRD"].ToString();
            //                                //36. Mã phân nhóm dòng tiền/ Cash mgmt Group
            //                                model.CashMgmtGroupCode = item["FDGRV"].ToString();
            //                                //37. Mã tài khoản công nợ/ Reconcile Account
            //                                model.ReconcileAccountCode = item["AKONT"].ToString();
            //                                //38. Số điện thoại (SAP)
            //                                model.SAPPhone = Phone;
            //                                //39. Mã số thuế TaxNo
            //                                if (!string.IsNullOrEmpty(TaxNo) && !TaxNo.Contains("X"))
            //                                {
            //                                    model.TaxNo = TaxNo;
            //                                }
            //                                model.CustomerSourceCode = "SHOWROOM";
            //                                //Loại địa chỉ
            //                                string LoaiDiaChi = item["ADDRTYPE"].ToString();
            //                                if (!string.IsNullOrEmpty(LoaiDiaChi))
            //                                {
            //                                    model.AddressTypeCode = LoaiDiaChi;
            //                                }
            //                                //Ngành nghề
            //                                string NganhNghe = item["BRSCH"].ToString();
            //                                if (!string.IsNullOrEmpty(NganhNghe))
            //                                {
            //                                    //NganhNghe = NganhNghe.PadLeft(4, '0');

            //                                    ProfileCareerModel profileCareer = new ProfileCareerModel();
            //                                    profileCareer.ProfileCareerId = Guid.NewGuid();
            //                                    profileCareer.ProfileId = ProfileId;
            //                                    profileCareer.ProfileCareerCode = NganhNghe;
            //                                    profileCareer.CompanyCode = item["BUKRS"].ToString();
            //                                    profileCareer.CreateBy = SYSTEM;
            //                                    profileCareer.CreateTime = CreateTime;
            //                                    _context.Entry(profileCareer).State = EntityState.Added;

            //                                }
            //                                //Yêu cầu tạo khách ở ECC
            //                                model.isCreateRequest = false;
            //                                model.CreateRequestTime = DateTime.Now;

            //                                _context.Entry(model).State = EntityState.Added;

            //                                //thêm nhóm KH
            //                                AddProfileGroup(item, CompanyCode, ProfileId);

            //                                //thêm NVKD
            //                                AddPersonInCharge(employeeList, item, CompanyCode, ProfileId, CreateTime);

            //                                //profileCodeList.Add(ProfileForeignCode);
            //                                //if (profileCodeList.Contains(ProfileForeignCode))
            //                                //{
            //                                //    profileList.Add(model);
            //                                //}
            //                            }
            //                            //thêm address book
            //                            else
            //                            {

            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (isAddressBook == false)
            //                            {
            //                                //1. GUID
            //                                ProfileId = existProfileWithSAPCode.ProfileId;
            //                                //2. ProfileCode => cannot update ProfileCode => identity
            //                                //if (ProfileCode != 0)
            //                                //{
            //                                //    existProfileWithSAPCode.ProfileCode = ProfileCode;
            //                                //}
            //                                //3. ProfileForeignCode
            //                                existProfileWithSAPCode.ProfileForeignCode = ProfileForeignCode;
            //                                //4. Đối tượng (Trong nước: false| Nước ngoài: true)
            //                                existProfileWithSAPCode.isForeignCustomer = isForeignCustomer;
            //                                existProfileWithSAPCode.CountryCode = CountryCode;
            //                                //5. Danh xưng
            //                                var Title = item["ANRED"].ToString();
            //                                if (Title.Length > 10)
            //                                {
            //                                    Title = null;
            //                                }
            //                                existProfileWithSAPCode.Title = Title;
            //                                ////6. Loại
            //                                existProfileWithSAPCode.CustomerTypeCode = ConstCustomerType.Account;

            //                                //Phân loại KH
            //                                var existProfileType = _context.ProfileTypeModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            //                                if (existProfileType == null)
            //                                {
            //                                    ProfileTypeModel profileType = new ProfileTypeModel();
            //                                    profileType.ProfileTypeId = Guid.NewGuid();
            //                                    profileType.ProfileId = ProfileId;
            //                                    //if (existProfileWithSAPCode.Title == "Company")
            //                                    //{
            //                                    //    profileType.CustomerTypeCode = ConstCustomerType.Bussiness;
            //                                    //}
            //                                    //else
            //                                    //{
            //                                    //    profileType.CustomerTypeCode = ConstCustomerType.Customer;
            //                                    //}
            //                                    profileType.CustomerTypeCode = ProfileTypeCode;
            //                                    profileType.CompanyCode = item["BUKRS"].ToString();
            //                                    profileType.CreateBy = SYSTEM;
            //                                    profileType.CreateTime = CreateTime;
            //                                    _context.Entry(profileType).State = EntityState.Added;
            //                                    //typeList.Add(profileType);
            //                                }
            //                                else
            //                                {
            //                                    existProfileType.CustomerTypeCode = ProfileTypeCode;
            //                                    _context.Entry(existProfileType).State = EntityState.Modified;
            //                                }
            //                                //7. Họ va Tên|Tên công ty
            //                                existProfileWithSAPCode.ProfileName = item["FULLNAME"].ToString();
            //                                //8. Tên ngắn
            //                                existProfileWithSAPCode.ProfileShortName = item["NAME1"].ToString();
            //                                //9. Tên viết tắt
            //                                existProfileWithSAPCode.AbbreviatedName = existProfileWithSAPCode.ProfileName.ToAbbreviation();
            //                                //10. Ngày sinh
            //                                //11. Tháng sinh
            //                                //12. Năm sinh
            //                                //13. Độ tuổi
            //                                //14. Số điện thoại
            //                                try
            //                                {
            //                                    //Delete all
            //                                    var phoneExistLst = _context.ProfilePhoneModel.Where(p => p.ProfileId == ProfileId).ToList();
            //                                    if (phoneExistLst != null && phoneExistLst.Count > 0)
            //                                    {
            //                                        _context.ProfilePhoneModel.RemoveRange(phoneExistLst);
            //                                    }
            //                                    //Add again
            //                                    if (!string.IsNullOrEmpty(Phone))
            //                                    {
            //                                        if (Phone.Contains("-"))
            //                                        {
            //                                            var arr = Phone.Split('-').ToList();
            //                                            var PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(arr[0]);

            //                                            var phoneArray = arr.Where(p => !p.Contains(arr[0])).ToList();
            //                                            foreach (var phoneItem in phoneArray)
            //                                            {
            //                                                ProfilePhoneModel phoneModel = new ProfilePhoneModel();
            //                                                phoneModel.PhoneId = Guid.NewGuid();
            //                                                phoneModel.ProfileId = ProfileId;
            //                                                phoneModel.PhoneNumber = RepositoryLibrary.ConvertToNoSpecialCharacters(phoneItem).Trim();
            //                                                if (PhoneNumber != phoneModel.PhoneNumber && !string.IsNullOrEmpty(phoneModel.PhoneNumber))
            //                                                {
            //                                                    _context.Entry(phoneModel).State = EntityState.Added;
            //                                                    //phoneList.Add(phoneModel);
            //                                                }
            //                                            }
            //                                            existProfileWithSAPCode.Phone = PhoneNumber.Trim();
            //                                        }
            //                                        else
            //                                        {
            //                                            existProfileWithSAPCode.Phone = RepositoryLibrary.ConvertToNoSpecialCharacters(Phone);
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        existProfileWithSAPCode.Phone = null;
            //                                    }
            //                                }
            //                                catch (Exception ex)
            //                                {
            //                                    error = ex.Message;
            //                                    if (ex.InnerException != null)
            //                                    {
            //                                        error = ex.InnerException.Message;
            //                                        if (ex.InnerException.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.InnerException.Message;
            //                                        }
            //                                    }
            //                                    continue;
            //                                }
            //                                //15. Email
            //                                //existProfileWithSAPCode.Email = item["SMTP_ADDR"].ToString();
            //                                //if (!string.IsNullOrEmpty(existProfileWithSAPCode.Email))
            //                                //{
            //                                //    if (IsValidEmail(existProfileWithSAPCode.Email) == false)
            //                                //    {
            //                                //        existProfileWithSAPCode.Note = string.Format("(Email: {0})", existProfileWithSAPCode.Email);
            //                                //        existProfileWithSAPCode.Email = null;
            //                                //    }
            //                                //    else
            //                                //    {
            //                                //        if (!string.IsNullOrEmpty(existProfileWithSAPCode.Note) && existProfileWithSAPCode.Note.Contains("Email:"))
            //                                //        {
            //                                //            existProfileWithSAPCode.Note = null;
            //                                //        }
            //                                //    }
            //                                //}
            //                                //else
            //                                //{
            //                                //    existProfileWithSAPCode.Email = null;
            //                                //}
            //                                if (!string.IsNullOrEmpty(Email))
            //                                {
            //                                    try
            //                                    {
            //                                        //Delete all
            //                                        var emailExistLst = _context.ProfileEmailModel.Where(p => p.ProfileId == ProfileId).ToList();
            //                                        if (emailExistLst != null && emailExistLst.Count > 0)
            //                                        {
            //                                            _context.ProfileEmailModel.RemoveRange(emailExistLst);
            //                                        }
            //                                        //Add again
            //                                        if (Email.Contains(";"))
            //                                        {
            //                                            var emailArray = Email.Split(';').ToList();
            //                                            foreach (var emailItem in emailArray)
            //                                            {
            //                                                if (IsValidEmail(emailItem) == true)
            //                                                {
            //                                                    ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                    emailModel.EmailId = Guid.NewGuid();
            //                                                    emailModel.ProfileId = ProfileId;
            //                                                    emailModel.Email = emailItem.Trim();
            //                                                    _context.Entry(emailModel).State = EntityState.Added;
            //                                                }
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            if (IsValidEmail(Email) == true)
            //                                            {
            //                                                ProfileEmailModel emailModel = new ProfileEmailModel();
            //                                                emailModel.EmailId = Guid.NewGuid();
            //                                                emailModel.ProfileId = ProfileId;
            //                                                emailModel.Email = Email.Trim();
            //                                                _context.Entry(emailModel).State = EntityState.Added;
            //                                            }
            //                                        }
            //                                    }
            //                                    catch (Exception ex)
            //                                    {
            //                                        // ghi log
            //                                        error = ex.Message;
            //                                        if (ex.InnerException != null)
            //                                        {
            //                                            error = ex.InnerException.Message;
            //                                            if (ex.InnerException.InnerException != null)
            //                                            {
            //                                                error = ex.InnerException.InnerException.Message;
            //                                            }
            //                                        }
            //                                        continue;
            //                                    }
            //                                }
            //                                //16. Khu vực
            //                                //existProfileWithSAPCode.SaleOfficeCode = item["VKBUR"].ToString();
            //                                //=> Khi đồng bộ từ SAP về dựa vào tỉnh thành mình set luôn khu vực chứ không đồng bộ data khu vực từ SAP về tránh SAP sai
            //                                var SaleOfficeCode = provinceLst.Where(p => p.ProvinceId == ProvinceId).Select(p => p.Area).FirstOrDefault();
            //                                if (!string.IsNullOrEmpty(SaleOfficeCode))
            //                                {
            //                                    existProfileWithSAPCode.SaleOfficeCode = SaleOfficeCode;
            //                                }
            //                                //17. Địa chỉ
            //                                //existProfileWithSAPCode.Address = item["ADDRESS"].ToString();
            //                                existProfileWithSAPCode.Address = !string.IsNullOrEmpty(item["STCD5"].ToString()) ? item["STCD5"].ToString().FirstCharToUpper() : item["ADDRESS"].ToString().FirstCharToUpper();
            //                                //18. Tỉnh/Thành phố
            //                                existProfileWithSAPCode.ProvinceId = ProvinceId;
            //                                //19. Quận/Huyện
            //                                existProfileWithSAPCode.DistrictId = DistrictId;
            //                                //20. Phường/Xã
            //                                existProfileWithSAPCode.WardId = WardId;
            //                                //21. Ghi chú
            //                                //22. Ngày ghé thăm
            //                                //23. Trạng thái (not update this field)
            //                                //Trạng thái hoạt động: X: ngưng hoạt động
            //                                string TrangThaiHoatDong = item["LOEVM"].ToString();
            //                                bool isActived = true;
            //                                if (!string.IsNullOrEmpty(TrangThaiHoatDong) && TrangThaiHoatDong.ToUpper() == "X")
            //                                {
            //                                    isActived = false;
            //                                }
            //                                if (existProfileWithSAPCode.Actived != isActived)
            //                                {
            //                                    existProfileWithSAPCode.Actived = isActived;
            //                                }
            //                                //24. Hình ảnh
            //                                //25. Nhân viên tạo
            //                                //26. Tạo tại công ty 
            //                                existProfileWithSAPCode.CreateAtCompany = item["BUKRS"].ToString();
            //                                //27. Tạo tại cửa hàng
            //                                existProfileWithSAPCode.CreateAtSaleOrg = item["VKORG"].ToString();
            //                                //28. CreateBy (not update this field)
            //                                //29. Thời gian tạo
            //                                //exist.CreateTime = CreateTime;
            //                                //30. LastEditBy
            //                                existProfileWithSAPCode.LastEditBy = SYSTEM;
            //                                //31. Thời gian sửa
            //                                existProfileWithSAPCode.LastEditTime = DateTime.Now;
            //                                //32. Phân nhóm KH/ Customer Account Group
            //                                existProfileWithSAPCode.CustomerAccountGroupCode = item["KTOKD"].ToString();
            //                                //33.Mã nhóm  KH/ Customer Group
            //                                existProfileWithSAPCode.CustomerGroupCode = item["KDGRP"].ToString();
            //                                //34. Mã Điều khoản thanh toán/ Payment Term
            //                                existProfileWithSAPCode.PaymentTermCode = item["ZTERM"].ToString();
            //                                //35.Mã phân loại doanh thu/ Customer Account Assignment Group
            //                                existProfileWithSAPCode.CustomerAccountAssignmentGroupCode = item["KTGRD"].ToString();
            //                                //36. Mã phân nhóm dòng tiền/ Cash mgmt Group
            //                                existProfileWithSAPCode.CashMgmtGroupCode = item["FDGRV"].ToString();
            //                                //37. Mã tài khoản công nợ/ Reconcile Account
            //                                existProfileWithSAPCode.ReconcileAccountCode = item["AKONT"].ToString();
            //                                //38. Số điện thoại (SAP)
            //                                existProfileWithSAPCode.SAPPhone = Phone;
            //                                //39. Mã số thuế TaxNo
            //                                if (!string.IsNullOrEmpty(TaxNo) && !TaxNo.Contains("X"))
            //                                {
            //                                    existProfileWithSAPCode.TaxNo = TaxNo;
            //                                }
            //                                existProfileWithSAPCode.CustomerSourceCode = "SHOWROOM";
            //                                //Loại địa chỉ
            //                                string LoaiDiaChi = item["ADDRTYPE"].ToString();
            //                                if (!string.IsNullOrEmpty(LoaiDiaChi))
            //                                {
            //                                    existProfileWithSAPCode.AddressTypeCode = LoaiDiaChi;
            //                                }
            //                                //Ngành nghề
            //                                string NganhNghe = item["BRSCH"].ToString();
            //                                if (!string.IsNullOrEmpty(NganhNghe))
            //                                {
            //                                    //NganhNghe = NganhNghe.PadLeft(4, '0');

            //                                    var existProfileCareer = _context.ProfileCareerModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            //                                    if (existProfileCareer == null)
            //                                    {
            //                                        ProfileCareerModel profileCareer = new ProfileCareerModel();
            //                                        profileCareer.ProfileCareerId = Guid.NewGuid();
            //                                        profileCareer.ProfileId = ProfileId;
            //                                        profileCareer.ProfileCareerCode = NganhNghe;
            //                                        profileCareer.CompanyCode = item["BUKRS"].ToString();
            //                                        profileCareer.CreateBy = SYSTEM;
            //                                        profileCareer.CreateTime = CreateTime;
            //                                        _context.Entry(profileCareer).State = EntityState.Added;
            //                                    }
            //                                    else
            //                                    {
            //                                        existProfileCareer.ProfileCareerCode = NganhNghe;
            //                                        _context.Entry(existProfileCareer).State = EntityState.Modified;
            //                                    }
            //                                }

            //                                //Yêu cầu tạo khách ở ECC
            //                                if (existProfileWithSAPCode.isCreateRequest == null)
            //                                {
            //                                    existProfileWithSAPCode.isCreateRequest = false;
            //                                    existProfileWithSAPCode.CreateRequestTime = DateTime.Now;
            //                                }

            //                                _context.Entry(existProfileWithSAPCode).State = EntityState.Modified;
            //                                //profileCodeList.Add(ProfileForeignCode);

            //                                //thêm nhóm KH
            //                                AddProfileGroup(item, CompanyCode, ProfileId);

            //                                //thêm NVKD
            //                                AddPersonInCharge(employeeList, item, CompanyCode, ProfileId, CreateTime);

            //                            }
            //                            //cập nhật address book
            //                            else
            //                            {

            //                            }
            //                        }
            //                    }
            //                    #endregion

            //                    _context.SaveChanges();
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                error = ex.Message;
            //                if (ex.InnerException != null)
            //                {
            //                    if (ex.InnerException.InnerException != null)
            //                    {
            //                        error = ex.InnerException.InnerException.Message;
            //                    }
            //                }
            //                return false;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        error = ex.Message;
            //        if (ex.InnerException != null)
            //        {
            //            if (ex.InnerException.InnerException != null)
            //            {
            //                error = ex.InnerException.InnerException.Message;
            //            }
            //        }
            //        return false;
            //    }
            //}
            //else
            //{
            //    error = string.Format("Không tìm thấy dữ liệu trả về từ SAP đối với khách hàng có mã SAP: {0}", SyncProfileForeignCode);
            //    return false;
            //}
            return true;
        }

        private bool IsValidEmail(string email)
        {
            //try
            //{
            //    var addr = new MailAddress(email);
            //    return addr.Address == email;
            //}
            //catch
            //{
            //    return false;
            //}
            string RegexPattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
                                        @"@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            // only return true if there is only 1 '@' character
            // and it is neither the first nor the last character
            return Regex.IsMatch(email, RegexPattern, RegexOptions.IgnoreCase);
        }

        private void AddProfileGroup(DataRow item, string CompanyCode, Guid ProfileId)
        {
            #region ProfileGroupModel
            //Check tồn tại nhóm KH
            var existProfileGroup = _context.ProfileGroupModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).FirstOrDefault();
            if (existProfileGroup == null)
            {
                //Chưa có => Thêm mới
                if (!string.IsNullOrEmpty(item["KDGRP"].ToString()))
                {
                    ProfileGroupModel profileGroup = new ProfileGroupModel();
                    profileGroup.ProfileGroupId = Guid.NewGuid();
                    profileGroup.ProfileId = ProfileId;
                    profileGroup.ProfileGroupCode = item["KDGRP"].ToString();
                    profileGroup.CompanyCode = CompanyCode;
                    profileGroup.CreateBy = SYSTEM;
                    profileGroup.CreateTime = DateTime.Now;
                    _context.Entry(profileGroup).State = EntityState.Added;
                    //groupList.Add(profileGroup);
                }
            }
            else
            {
                //Cập nhật
                existProfileGroup.ProfileGroupCode = item["KDGRP"].ToString();
                existProfileGroup.LastEditBy = SYSTEM;
                existProfileGroup.LastEditTime = DateTime.Now;
                _context.Entry(existProfileGroup).State = EntityState.Modified;
            }
            #endregion
        }

        private void AddPersonInCharge(List<string> employeeList, DataRow item, string CompanyCode, Guid ProfileId, DateTime? CreateTime)
        {
            #region PersonInChargeModel
            var existPerson = _context.PersonInChargeModel.Where(p => p.ProfileId == ProfileId && p.CompanyCode == CompanyCode).ToList();
            //Nếu có tồn tại thì xóa add lại
            if (existPerson != null && existPerson.Count > 0)
            {
                _context.PersonInChargeModel.RemoveRange(existPerson);
                _context.SaveChanges();
            }

            //if (existPerson == null || existPerson.Count == 0)
            //{
            //Sale Employee
            var EmployeeCode = item["EMPLOYEENO"].ToString();
            if (!string.IsNullOrEmpty(EmployeeCode) && EmployeeCode != "00000000" && employeeList.Contains(EmployeeCode))
            {
                PersonInChargeModel person = new PersonInChargeModel();
                person.PersonInChargeId = Guid.NewGuid();
                person.ProfileId = ProfileId;
                person.SalesEmployeeCode = EmployeeCode;
                person.RoleCode = ConstRoleCode.SALE_EMPLOYEE;
                person.CompanyCode = CompanyCode;
                person.CreateBy = SYSTEM;
                person.CreateTime = CreateTime;
                _context.Entry(person).State = EntityState.Added;
                //personList.Add(person);
            }

            //Sale Employee 2
            var EmployeeCode2 = item["SE_PERNR"].ToString();
            if (!string.IsNullOrEmpty(EmployeeCode2) && EmployeeCode2 != "00000000" && EmployeeCode2 != EmployeeCode && employeeList.Contains(EmployeeCode2))
            {
                PersonInChargeModel person = new PersonInChargeModel();
                person.PersonInChargeId = Guid.NewGuid();
                person.ProfileId = ProfileId;
                person.SalesEmployeeCode = EmployeeCode2;
                person.RoleCode = ConstRoleCode.SALE_EMPLOYEE2;
                person.CompanyCode = CompanyCode;
                person.CreateBy = SYSTEM;
                person.CreateTime = CreateTime;
                _context.Entry(person).State = EntityState.Added;
                //personList.Add(person);
            }
            //}
            #endregion
        }
        #endregion

        #region Báo cáo khách hàng theo nhân viên kinh doanh
        public List<ProfileWithPersonInChargeReportViewModel> GetProfileWithPersonInChargeReport(ProfileWithPersonInChargeReportSearchViewModel searchModel, string CurrentCompanyCode)
        {
            #region SalesEmployeeCode
            //Build your record
            var tableSalesEmployeeCodeSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

            //And a table as a list of those records
            var tableSalesEmployeeCode = new List<SqlDataRecord>();
            List<string> salesEmployeeCodeLst = new List<string>();
            if (searchModel.SalesEmployeeCode != null && searchModel.SalesEmployeeCode.Count > 0)
            {
                foreach (var r in searchModel.SalesEmployeeCode)
                {
                    var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                    tableRow.SetString(0, r);
                    if (!salesEmployeeCodeLst.Contains(r))
                    {
                        salesEmployeeCodeLst.Add(r);
                        tableSalesEmployeeCode.Add(tableRow);
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                tableSalesEmployeeCode.Add(tableRow);
            }
            #endregion

            #region DepartmentCode
            //And a table as a list of those records
            var tableDepartmentCode = new List<SqlDataRecord>();
            List<string> departmnetCodeLst = new List<string>();
            if (searchModel.DepartmentCode != null && searchModel.DepartmentCode.Count > 0)
            {
                foreach (var r in searchModel.DepartmentCode)
                {
                    var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                    tableRow.SetString(0, r);
                    if (!departmnetCodeLst.Contains(r))
                    {
                        departmnetCodeLst.Add(r);
                        tableDepartmentCode.Add(tableRow);
                    }
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                tableDepartmentCode.Add(tableRow);
            }
            #endregion

            string sqlQuery = "EXEC [Report].[usp_ProfileWithPersonInChargeReport] @SalesEmployeeCode, @DepartmentCode, @CurrentCompanyCode";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "SalesEmployeeCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableSalesEmployeeCode
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DepartmentCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableDepartmentCode
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentCompanyCode ?? (object)DBNull.Value,
                },
            };
            #endregion

            var result = _context.Database.SqlQuery<ProfileWithPersonInChargeReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return result;
        }
        #endregion Báo cáo khách hàng theo nhân viên kinh doanh

        #region Gộp thông tin khách hàng
        public void MergeProfile(Guid? ProfileId, List<Guid?> ProfileIdDuplicate, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                //-- Khi gom 2 hay nhiều mã CRM về 1 mã:
                //-- 1. Mã CRM cần gộp => đang sử dụng
                //-- 2. Danh sách mã CRM bị trùng => ngưng sử dụng
                //-- 3. Tất cả các hoạt động liên quan tới khách hàng: chuyển về mã đang sử dụng
                //-- 4. Chuyển cả danh sách liên hệ và địa chỉ về mã đang sử dụng

                #region 1. Mã CRM cần gộp => đang sử dụng
                var profile = _context.ProfileModel.Where(p => p.ProfileId == ProfileId.Value).FirstOrDefault();
                if (profile != null)
                {
                    //Attach the instance so that we don't need to load it from the DB
                    _context.ProfileModel.Attach(profile);
                    profile.Actived = true;

                    //Specify the fields that should be updated.
                    _context.Entry(profile).Property(x => x.Actived).IsModified = true;
                }
                #endregion

                #region 2. Danh sách mã CRM bị trùng => ngưng sử dụng
                if (ProfileIdDuplicate != null && ProfileIdDuplicate.Count > 0)
                {
                    foreach (var profileId in ProfileIdDuplicate)
                    {
                        var profileDuplicate = _context.ProfileModel.Where(p => p.ProfileId == profileId.Value).FirstOrDefault();
                        if (profileDuplicate != null)
                        {
                            //Attach the instance so that we don't need to load it from the DB
                            _context.ProfileModel.Attach(profileDuplicate);
                            profileDuplicate.Actived = false;

                            //Specify the fields that should be updated.
                            _context.Entry(profileDuplicate).Property(x => x.Actived).IsModified = true;

                            #region 3. Tất cả các hoạt động liên quan tới khách hàng: chuyển về mã đang sử dụng
                            //3.1. Khách ghé thăm
                            //3.2. Thăm hỏi KH
                            //3.3. Nhiệm vụ
                            //3.4. Xử lý khiếu nại
                            //3.5. Bảo hành
                            //3.6. Điểm trưng bày
                            //  => Update TaskModel
                            var taskLst = _context.TaskModel.Where(p => p.ProfileId == profileId).ToList();
                            if (taskLst != null && taskLst.Count > 0)
                            {
                                foreach (var task in taskLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.TaskModel.Attach(task);
                                    task.ProfileId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(task).Property(x => x.ProfileId).IsModified = true;
                                }
                            }
                            //3.7. Catalogue => Update DeliveryModel
                            var customerCatalogueLst = _context.DeliveryModel.Where(p => p.ProfileId == profileId).ToList();
                            if (customerCatalogueLst != null && customerCatalogueLst.Count > 0)
                            {
                                foreach (var customerCatalogue in customerCatalogueLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.DeliveryModel.Attach(customerCatalogue);
                                    customerCatalogue.ProfileId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(customerCatalogue).Property(x => x.ProfileId).IsModified = true;
                                }
                            }
                            //3.8. Thị hiếu => Update CustomerTastesModel
                            var customerTastesLst = _context.CustomerTastesModel.Where(p => p.ProfileId == profileId).ToList();
                            if (customerTastesLst != null && customerTastesLst.Count > 0)
                            {
                                foreach (var customerTastes in customerTastesLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.CustomerTastesModel.Attach(customerTastes);
                                    customerTastes.ProfileId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(customerTastes).Property(x => x.ProfileId).IsModified = true;
                                }
                            }
                            //3.9. Đăng ký bảo hành => Update ProductWarrantyModel
                            var productWarrantyLst = _context.ProductWarrantyModel.Where(p => p.ProfileId == profileId).ToList();
                            if (productWarrantyLst != null && productWarrantyLst.Count > 0)
                            {
                                foreach (var productWarranty in productWarrantyLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.ProductWarrantyModel.Attach(productWarranty);
                                    productWarranty.ProfileId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(productWarranty).Property(x => x.ProfileId).IsModified = true;
                                }
                            }
                            #endregion

                            #region 4. Chuyển cả danh sách liên hệ và địa chỉ về mã đang sử dụng
                            //Liên hệ
                            var profileContactLst = _context.ProfileContactAttributeModel.Where(p => p.CompanyId == profileId).ToList();
                            if (profileContactLst != null && profileContactLst.Count > 0)
                            {
                                foreach (var profileContact in profileContactLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.ProfileContactAttributeModel.Attach(profileContact);
                                    profileContact.CompanyId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(profileContact).Property(x => x.CompanyId).IsModified = true;
                                }
                            }
                            //Địa chỉ
                            var addressBookLst = _context.AddressBookModel.Where(p => p.ProfileId == profileId).ToList();
                            if (addressBookLst != null && addressBookLst.Count > 0)
                            {
                                foreach (var addressBook in addressBookLst)
                                {
                                    //Attach the instance so that we don't need to load it from the DB
                                    _context.AddressBookModel.Attach(addressBook);
                                    addressBook.ProfileId = ProfileId.Value;

                                    //Specify the fields that should be updated.
                                    _context.Entry(addressBook).Property(x => x.ProfileId).IsModified = true;
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage = ex.InnerException.InnerException.Message;
                    }
                }
            }

        }
        #endregion Gộp thông tin khách hàng
    }
}
