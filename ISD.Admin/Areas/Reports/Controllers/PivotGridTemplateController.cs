using ISD.Core;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class PivotGridTemplateController : BaseController
    {
        public JsonResult Create(string templateName, string pageUrl, string parameter, bool isSystem, bool IsDefault)
        {
            Guid pageId = Guid.Empty;
            if (string.IsNullOrEmpty(parameter))
            {
                pageId = GetPageId(pageUrl);
            }
            else
            {
                pageId = GetPageId(pageUrl, parameter);
            }
            List<FieldSettingModel> settings = (List<FieldSettingModel>)(Session[CurrentUser.AccountId + "Layout"]);
            Session.Remove(CurrentUser.AccountId + "Layout");
            if (settings == null || settings.Count <= 0)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Session time out"
                });
            }
            if (pageId == null || pageId == Guid.Empty)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Page url invalid"
                });
            }
            _unitOfWork.PivotGridTemplateRepository.Create(templateName, IsDefault, isSystem, CurrentUser.AccountId, pageId, settings);
            _context.SaveChanges();
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true
            });

        }
        public JsonResult Edit(Guid templateId, string templateName, bool isDefault)
        {
            List<FieldSettingModel> settings = (List<FieldSettingModel>)(Session[CurrentUser.AccountId + "Layout"]);
            Session.Remove(CurrentUser.AccountId + "Layout");
            if (settings == null || settings.Count <= 0)
            {
                settings = new List<FieldSettingModel>();
            }
            _unitOfWork.PivotGridTemplateRepository.Update(templateId, templateName, isDefault, settings);
            _context.SaveChanges();
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true
            });

        }
        public JsonResult Delete(Guid templateId)
        {
            if (templateId != Guid.Empty)
            {
                _unitOfWork.PivotGridTemplateRepository.Delete(templateId);
                _context.SaveChanges();

            }
            return Json(new
            {
                Code = System.Net.HttpStatusCode.NoContent,
                Success = true
            });
        }
        [ValidateInput(false)]
        #region BEGIN GRANTT CHART
        public JsonResult CreateGrantt(string templateName, string pageUrl, string parameter, bool isSystem, bool IsDefault,List<FieldSettingGranttModel> settings)
        {
            Guid pageId = Guid.Empty;
            if (string.IsNullOrEmpty(parameter))
            {
                pageId = GetPageId(pageUrl);
            }
            else
            {
                pageId = GetPageId(pageUrl, parameter);
            }
            //List<FieldSettingGranttModel> settings = (List<FieldSettingGranttModel>)(Session[CurrentUser.AccountId + "Layout"]);
            //Session.Remove(CurrentUser.AccountId + "Layout");
            if (settings == null || settings.Count <= 0)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Session time out"
                });
            }
            if (pageId == null || pageId == Guid.Empty)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Page url invalid"
                });
            }
            _unitOfWork.PivotGridTemplateRepository.CreateGrantt(templateName, IsDefault, isSystem, CurrentUser.AccountId, pageId, settings);
            _context.SaveChanges();
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true
            });

        }

        [ValidateInput(false)]
        public JsonResult EditGrantt(Guid templateId, string templateName, bool isDefault, List<FieldSettingGranttModel> settings)
        {
            //List<FieldSettingGranttModel> settings = (List<FieldSettingGranttModel>)(Session[CurrentUser.AccountId + "Layout"]);
            //Session.Remove(CurrentUser.AccountId + "Layout");
            if (settings == null || settings.Count <= 0)
            {
                settings = new List<FieldSettingGranttModel>();
            }
            _unitOfWork.PivotGridTemplateRepository.UpdateGrantt(templateId, templateName, isDefault, settings);
            _context.SaveChanges();
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true
            });

        }
        #endregion END GRANTT CHART
    }
}