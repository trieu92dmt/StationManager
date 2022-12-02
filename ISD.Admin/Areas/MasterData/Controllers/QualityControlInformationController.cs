using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ISD.Core;

namespace MasterData.Controllers
{
    public class QualityControlInformationController : BaseController
    {
        //GET: /QualityControlInformation/Index
        #region Index, _Search
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View(new QualityControlInformationModel());
        }
        public ActionResult _Search(string Name, bool? Actived)
        {
            return ExecuteSearch(() =>
            {
                var NameIsNullOrEmpty = string.IsNullOrEmpty(Name);
                var qualityControlInformationList = _context.QualityControlInformationModel.Where(p => (NameIsNullOrEmpty || p.Name.ToLower().Contains(Name.ToLower())) 
                                                                && (Actived == null || p.Actived == Actived))
                                                  .OrderBy(p => p.OrderIndex)
                                                  .ToList();
                return PartialView(qualityControlInformationList);
            });
        }
        #endregion

        //GET: /QualityControlInformation/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            QualityControlWorkCenterViewModel qualityControlInformation = new QualityControlWorkCenterViewModel()
            {
                Actived = true,
                WorkCenterList = WorkCenterWithOrderBy(),
                //ActivedWorkCenterList = WorkCenterWithOrderBy().ToList()
            };
            CreateViewBag();
            return View(qualityControlInformation);
        }

        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(QualityControlInformationModel model, List<string> WorkCenterCode)
        {
            return ExecuteContainer(() =>
            {
                //Save data in QualityControlInformationModel
                model.Id = Guid.NewGuid();
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                //Save data in QualityControlWorkCenterModel
                if (WorkCenterCode != null)
                {
                    ManyToMany(model, WorkCenterCode);
                }
                //Save
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.QualityControlInformation.ToLower())
                });
            });
        }
        #endregion

        //GET: /QualityControlInformation/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var qualityControlInformation = (from p in _context.QualityControlInformationModel.AsEnumerable()
                        where p.Id == id
                        select new QualityControlWorkCenterViewModel()
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Code = p.Code,
                            Actived = p.Actived,
                            OrderIndex = p.OrderIndex,
                            WorkCenterList = WorkCenterWithOrderBy(),
                            ActivedWorkCenterList = p.WorkCenterModel.ToList(),
                            CreateBy = p.CreateBy,
                            CreateTime = p.CreateTime,
                            LastEditBy = p.LastEditBy,
                            LastEditTime = p.LastEditTime
                        }).FirstOrDefault();
            CreateViewBag();
            return View(qualityControlInformation);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(QualityControlInformationModel model, List<string> WorkCenterCode)
        {
            return ExecuteContainer(() =>
            {
                var qualityControlInformation = _context.QualityControlInformationModel.Where(p => p.Id == model.Id)
                                                   .Include(p => p.WorkCenterModel).FirstOrDefault();
                if (qualityControlInformation != null)
                {
                    //master qualityControlInformation
                    qualityControlInformation.Name = model.Name;
                    qualityControlInformation.OrderIndex = model.OrderIndex;
                    qualityControlInformation.Actived = model.Actived;
                    qualityControlInformation.LastEditBy = CurrentUser.AccountId;
                    qualityControlInformation.LastEditTime = DateTime.Now;
                    //detail function
                    if (WorkCenterCode != null)
                    {
                        ManyToMany(qualityControlInformation, WorkCenterCode);
                    }
                    _context.Entry(qualityControlInformation).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.QualityControlInformation.ToLower())
                });
            });
        }
        #endregion

        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var qualityControlInformation = _context.QualityControlInformationModel.FirstOrDefault(p => p.Id == id);
                if (qualityControlInformation != null)
                {
                    if (qualityControlInformation.WorkCenterModel != null)
                    {
                        qualityControlInformation.WorkCenterModel.Clear();
                    }
                    _context.Entry(qualityControlInformation).State = EntityState.Deleted;

                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.QualityControlInformation.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.QualityControlInformation.ToLower())
                    });
                }
            });
        }
        #endregion

        #region Helper
        private void CreateViewBag(Guid? WorkCenterCode = null)
        {
            // WorkCenterCode
            var WorkCenterList = _context.WorkCenterModel.OrderBy(p => p.WorkCenterCode).ToList();
            ViewBag.WorkCenterCode = new SelectList(WorkCenterList, "WorkCenterCode", "WorkCenterName", WorkCenterCode);
        }

        private void ManyToMany(QualityControlInformationModel model, List<string> WorkCenterList)
        {
            if (model.WorkCenterModel != null)
            {
                model.WorkCenterModel.Clear();
            }
            if (WorkCenterList != null && WorkCenterList.Count > 0)
            {
                foreach (var item in WorkCenterList)
                {
                    var itemAdd = _context.WorkCenterModel.Find(item);
                    model.WorkCenterModel.Add(itemAdd);
                }
            }
        }

        public List<WorkCenterModel> WorkCenterWithOrderBy()
        {
            //get all function
            var funcList = _context.WorkCenterModel
                                .OrderBy(p => p.OrderIndex)
                                .ToList();
            return funcList;
        }
        #endregion
    }
}