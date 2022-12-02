using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Customer;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Customer.Controllers
{
    public class ConstructionController : BaseController
    {
        // GET: Construction
        //Căn mẫu
        public ActionResult _ListInternal(Guid? id, bool? isLoadContent = false)
        {
            return ExecuteSearch(() =>
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

                              where a.ProfileId == id && a.PartnerType == 4 //Căn mẫu
                              orderby a.IsMain descending, a.CreateTime descending
                              select new ProfileViewModel()
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
                if (isLoadContent == true)
                {
                    return PartialView("_ListContent", result);
                }
                return PartialView(result);
            });
        }
        //Đại trà
        public ActionResult _ListCompetitor(Guid? id, bool? isLoadContent = false)
        {
            return ExecuteSearch(() =>
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

                              where a.ProfileId == id && a.PartnerType == 5 //Đại trà
                              orderby a.IsMain descending, a.CreateTime descending
                              select new ProfileViewModel()
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
                if (isLoadContent == true)
                {
                    return PartialView("_ListContent", result);
                }
                return PartialView(result);
            });
        }

        #region Handle data
        [HttpPost]
        public ActionResult SaveInternal(Guid? ProfileId, Guid? PartnerId)
        {
            return ExecuteContainer(() =>
            {
                var existsPartner = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == ProfileId && p.PartnerType == 4).FirstOrDefault();
                #region Create
                Profile_Opportunity_PartnerModel partner = new Profile_Opportunity_PartnerModel();
                partner.OpportunityPartnerId = Guid.NewGuid();
                partner.ProfileId = ProfileId;
                partner.PartnerId = PartnerId;
                partner.PartnerType = 4; //Căn mẫu
                partner.CreateBy = CurrentUser.AccountId;
                partner.CreateTime = DateTime.Now;
                partner.IsMain = existsPartner != null ? false : true;

                _context.Entry(partner).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.TabConstruction.ToLower()),
                    RedirectUrl = "/Customer/Profile/Edit/?id=" + ProfileId,
                });
                #endregion
            });
        }

        [HttpPost]
        public ActionResult SetMainInternal(Guid? OpportunityPartnerId)
        {
            return ExecuteContainer(() =>
            {
                var partner = _context.Profile_Opportunity_PartnerModel.Where(p => p.OpportunityPartnerId == OpportunityPartnerId).FirstOrDefault();
                if (partner != null)
                {
                    partner.IsMain = true;

                    var remainPartner = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == partner.ProfileId && p.OpportunityPartnerId != OpportunityPartnerId && p.PartnerType == 4).ToList();
                    if (remainPartner != null && remainPartner.Count > 0)
                    {
                        foreach (var remain in remainPartner)
                        {
                            remain.IsMain = false;
                            _context.Entry(remain).State = EntityState.Modified;
                        }
                    }

                    _context.Entry(partner).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.TabConstruction.ToLower()),
                        RedirectUrl = "/Customer/Profile/Edit/?id=" + partner.ProfileId,
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Không tìm thấy dữ liệu phù hợp",
                    });
                }
            });
        }

        [HttpPost]
        public ActionResult SetMainCompetitor(Guid? OpportunityPartnerId)
        {
            return ExecuteContainer(() =>
            {
                var partner = _context.Profile_Opportunity_PartnerModel.Where(p => p.OpportunityPartnerId == OpportunityPartnerId).FirstOrDefault();
                if (partner != null)
                {
                    partner.IsMain = true;

                    var remainPartner = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == partner.ProfileId && p.OpportunityPartnerId != OpportunityPartnerId && p.PartnerType == 5).ToList();
                    if (remainPartner != null && remainPartner.Count > 0)
                    {
                        foreach (var remain in remainPartner)
                        {
                            remain.IsMain = false;
                            _context.Entry(remain).State = EntityState.Modified;
                        }
                    }

                    _context.Entry(partner).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.TabConstruction.ToLower()),
                        RedirectUrl = "/Customer/Profile/Edit/?id=" + partner.ProfileId,
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Không tìm thấy dữ liệu phù hợp",
                    });
                }
            });
        }

        [HttpPost]
        public ActionResult SaveCompetitor(Guid? ProfileId, Guid? PartnerId)
        {
            return ExecuteContainer(() =>
            {
                var existsPartner = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == ProfileId && p.PartnerType == 5).FirstOrDefault();
                #region Create
                Profile_Opportunity_PartnerModel partner = new Profile_Opportunity_PartnerModel();
                partner.OpportunityPartnerId = Guid.NewGuid();
                partner.ProfileId = ProfileId;
                partner.PartnerId = PartnerId;
                partner.PartnerType = 5; //Đại trà
                partner.CreateBy = CurrentUser.AccountId;
                partner.CreateTime = DateTime.Now;
                partner.IsMain = existsPartner != null ? false : true;

                _context.Entry(partner).State = EntityState.Added;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.TabConstruction.ToLower()),
                    RedirectUrl = "/Customer/Profile/Edit/?id=" + ProfileId,
                });
                #endregion
            });
        }
        #endregion Handle data

        #region Delete
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                //Căn mẫu
                var internalOp = _context.Profile_Opportunity_PartnerModel.FirstOrDefault(p => p.OpportunityPartnerId == id);
                if (internalOp != null)
                {
                    var ProfileId = internalOp.ProfileId;
                    _context.Entry(internalOp).State = EntityState.Deleted;
                    _context.SaveChanges();

                    var remainInternal = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == ProfileId && p.PartnerType == 4).ToList();
                    if (remainInternal.Count == 1)
                    {
                        foreach (var item in remainInternal)
                        {
                            item.IsMain = true;
                            _context.Entry(item).State = EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.TabConstruction.ToLower())
                    });
                }

                //Đại trà
                var competitorOp = _context.Profile_Opportunity_PartnerModel.FirstOrDefault(p => p.OpportunityPartnerId == id);
                if (competitorOp != null)
                {
                    var ProfileId = competitorOp.ProfileId;
                    _context.Entry(competitorOp).State = EntityState.Deleted;
                    _context.SaveChanges();

                    var remainCompetitor = _context.Profile_Opportunity_PartnerModel.Where(p => p.ProfileId == ProfileId && p.PartnerType == 5).ToList();
                    if (remainCompetitor.Count == 1)
                    {
                        foreach (var item in remainCompetitor)
                        {
                            item.IsMain = true;
                            _context.Entry(item).State = EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.TabConstruction.ToLower())
                    });
                }

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Không tìm thấy dữ liệu phù hợp"
                });
            });
        }
        #endregion
    }
}