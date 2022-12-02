using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class PivotGridTemplateRepository
    {
        EntityDataContext _context;
       
        /// <summary>
        /// Khởi tạo repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public PivotGridTemplateRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
        }
        public void Create(string  templateName,bool isDefault, bool isSystem, Guid? accountId, Guid pageId, List<FieldSettingModel> settings)
        {
            SearchResultTemplateModel newTemplate = new SearchResultTemplateModel();
            newTemplate.SearchResultTemplateId = Guid.NewGuid();
            newTemplate.TemplateName = templateName;
            newTemplate.PageId = pageId;
   
            if (isSystem)
            {
                newTemplate.isSystem = true;
                if (isDefault)
                {
                    newTemplate.IsDefaultTemplate = true;
                    var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == pageId && s.isSystem == true).ToList();
                    if (existingTempalte.Count > 0)
                    {
                        foreach(var item in existingTempalte)
                        {
                            item.IsDefaultTemplate = false;
                            _context.Entry(item).State = EntityState.Modified;
                        }    
                    }
                }
            }
            else
            {
                newTemplate.isSystem = false;
                newTemplate.AccountId = accountId;
                if (isDefault)
                {
                    newTemplate.IsDefaultTemplate = true;
                    var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == pageId && s.isSystem == false && s.AccountId == accountId).ToList();
                    if (existingTempalte.Count > 0)
                    {
                        foreach (var item in existingTempalte)
                        {
                            item.IsDefaultTemplate = false;
                            _context.Entry(item).State = EntityState.Modified;
                        }
                    }
                }
            }
            _context.SearchResultTemplateModel.Add(newTemplate);
            foreach(var field in settings)
            {
                SearchResultDetailTemplateModel detail = new SearchResultDetailTemplateModel();
                detail.SearchResultTemplateId = newTemplate.SearchResultTemplateId;
                detail.SearchResultDetailTemplateId = Guid.NewGuid();
                detail.PivotArea = field.PivotArea;
                detail.FieldName = field.FieldName;
                detail.CellFormat_FormatType = field.CellFormat_FormatType;
                detail.CellFormat_FormatString = field.CellFormat_FormatString;
                detail.Caption = field.Caption;
                detail.AreaIndex = field.AreaIndex;
                detail.Visible = field.Visible;
                _context.SearchResultDetailTemplateModel.Add(detail);
            }
            
        }
        public void Update(Guid templateId, string templateName,bool isDefault, List<FieldSettingModel> settings)
        {
            SearchResultTemplateModel update = _context.SearchResultTemplateModel.FirstOrDefault(s => s.SearchResultTemplateId == templateId);
            if(update !=null)
            {
                update.TemplateName = templateName;
                
                if (isDefault)
                {                  
                    if (update.isSystem == true)
                    {
                        
                        var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == update.PageId && s.isSystem == true).ToList();
                        if (existingTempalte.Count > 0)
                        {
                            foreach (var item in existingTempalte)
                            {
                                item.IsDefaultTemplate = false;
                                _context.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == update.PageId && s.isSystem == false && s.AccountId == update.AccountId).ToList();
                        if (existingTempalte.Count > 0)
                        {
                            foreach (var item in existingTempalte)
                            {
                                item.IsDefaultTemplate = false;
                                _context.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    update.IsDefaultTemplate = isDefault;
                    _context.Entry(update).State = EntityState.Modified;
                }
                else
                {
                    update.IsDefaultTemplate = isDefault;
                    _context.Entry(update).State = EntityState.Modified;
                }
                if(settings !=null && settings.Count>0)
                {
                    var detail = _context.SearchResultDetailTemplateModel.Where(s => s.SearchResultTemplateId == templateId).ToList();
                    foreach (var item in detail)
                    {
                        foreach (var setting in settings)
                        {
                            if (item.FieldName == setting.FieldName)
                            {
                                item.PivotArea = setting.PivotArea;
                                item.FieldName = setting.FieldName;
                                item.CellFormat_FormatType = setting.CellFormat_FormatType;
                                item.CellFormat_FormatString = setting.CellFormat_FormatString;
                                item.Caption = setting.Caption;
                                item.AreaIndex = setting.AreaIndex;
                                item.Visible = setting.Visible;
                                _context.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                }    
                
            }    
            

        }
        public void Delete(Guid templateId)
        {
            SearchResultTemplateModel delete = _context.SearchResultTemplateModel.FirstOrDefault(s => s.SearchResultTemplateId == templateId);
            if (delete != null)
            {
                _context.Entry(delete).State = EntityState.Deleted;
                var detail = _context.SearchResultDetailTemplateModel.Where(s => s.SearchResultTemplateId == templateId).ToList();
                foreach (var item in detail)
                {  
                  _context.Entry(item).State = EntityState.Deleted;                   
                }
            }
        }
        public List<PivotTemplateViewModel> GetSystemTemplate(Guid pageId)
        {
            var list = _context.SearchResultTemplateModel.Where(s => s.isSystem == true && s.PageId == pageId)
                                                           .OrderBy(n => n.TemplateName)
                                                          .Select(s => new PivotTemplateViewModel
                                                          {
                                                              SearchResultTemplateId = s.SearchResultTemplateId,
                                                              TemplateName = s.TemplateName,
                                                              IsDefault = s.IsDefaultTemplate
                                                          }).ToList();
            return list;
        }
        public List<PivotTemplateViewModel> GetUserTemplate(Guid pageId, Guid accountId)
        {
            var list = _context.SearchResultTemplateModel.Where(s => s.AccountId == accountId && s.PageId == pageId)
                                                           .OrderBy(n => n.TemplateName)
                                                          .Select(s => new PivotTemplateViewModel
                                                          {
                                                              SearchResultTemplateId = s.SearchResultTemplateId,
                                                              TemplateName = s.TemplateName,
                                                              IsDefault = s.IsDefaultTemplate
                                                          }).ToList();
            return list;
        }
        public List<FieldSettingModel> GetSettingByTemplate(Guid templateId)
        {
            var list = _context.SearchResultDetailTemplateModel.Where(s => s.SearchResultTemplateId == templateId)
                                                          .Select(s => new FieldSettingModel
                                                          {
                                                              FieldName = s.FieldName,
                                                              Caption = s.Caption,
                                                              PivotArea = s.PivotArea,
                                                              AreaIndex = s.AreaIndex,
                                                              CellFormat_FormatString = s.CellFormat_FormatString,
                                                              CellFormat_FormatType = s.CellFormat_FormatType,
                                                              Visible = s.Visible
                                                          }).ToList();
            return list;
        }


        public string GetTemplateNameBy(Guid templateId)
        {
            var templateName = _context.SearchResultTemplateModel.Where(x => x.SearchResultTemplateId == templateId).Select(x => x.TemplateName).FirstOrDefault();
            return templateName;
        }

        #region BEGIN GRANTT CHART
        public void CreateGrantt(string templateName, bool isDefault, bool isSystem, Guid? accountId, Guid pageId, List<FieldSettingGranttModel> settings)
        {
            SearchResultTemplateModel newTemplate = new SearchResultTemplateModel();
            newTemplate.SearchResultTemplateId = Guid.NewGuid();
            newTemplate.TemplateName = templateName;
            newTemplate.PageId = pageId;

            if (isSystem)
            {
                newTemplate.isSystem = true;
                if (isDefault)
                {
                    newTemplate.IsDefaultTemplate = true;
                    var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == pageId && s.isSystem == true).ToList();
                    if (existingTempalte.Count > 0)
                    {
                        foreach (var item in existingTempalte)
                        {
                            item.IsDefaultTemplate = false;
                            _context.Entry(item).State = EntityState.Modified;
                        }
                    }
                }
            }
            else
            {
                newTemplate.isSystem = false;
                newTemplate.AccountId = accountId;
                if (isDefault)
                {
                    newTemplate.IsDefaultTemplate = true;
                    var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == pageId && s.isSystem == false && s.AccountId == accountId).ToList();
                    if (existingTempalte.Count > 0)
                    {
                        foreach (var item in existingTempalte)
                        {
                            item.IsDefaultTemplate = false;
                            _context.Entry(item).State = EntityState.Modified;
                        }
                    }
                }
            }
            _context.SearchResultTemplateModel.Add(newTemplate);
            foreach (var field in settings)
            {
                SearchResultDetailTemplateModel detail = new SearchResultDetailTemplateModel();
                detail.SearchResultTemplateId = newTemplate.SearchResultTemplateId;
                detail.SearchResultDetailTemplateId = Guid.NewGuid();
                detail.PivotArea = field.PivotArea;
                detail.FieldName = field.FieldName;
                detail.CellFormat_FormatType = field.CellFormat_FormatType;
                detail.CellFormat_FormatString = field.CellFormat_FormatString;
                detail.Caption = field.Caption;
                detail.AreaIndex = field.AreaIndex;
                detail.Visible = field.Visible;
                detail.Width = field.Width;
                detail.Height = field.Height;
                detail.Resize = field.Resize;
                detail.Tree = field.Tree;
              
                _context.SearchResultDetailTemplateModel.Add(detail);
            }

        }

        public List<FieldSettingGranttModel> GetSettingGranttByTemplate(Guid templateId)
        {
            var list = _context.SearchResultDetailTemplateModel.Where(s => s.SearchResultTemplateId == templateId)
                                                          .Select(s => new FieldSettingGranttModel
                                                          {
                                                              FieldName = s.FieldName,
                                                              Caption = s.Caption,
                                                              PivotArea = s.PivotArea,
                                                              AreaIndex = s.AreaIndex,
                                                              CellFormat_FormatString = s.CellFormat_FormatString,
                                                              CellFormat_FormatType = s.CellFormat_FormatType,
                                                              Visible = s.Visible,
                                                              Height = s.Height,
                                                              Resize = s.Resize,
                                                              Tree = s.Tree,
                                                              Width =s.Width
                                                          }).ToList();
            return list;
        }

        public void UpdateGrantt(Guid templateId, string templateName, bool isDefault, List<FieldSettingGranttModel> settings)
        {
            SearchResultTemplateModel update = _context.SearchResultTemplateModel.FirstOrDefault(s => s.SearchResultTemplateId == templateId);
            if (update != null)
            {
                update.TemplateName = templateName;

                if (isDefault)
                {
                    if (update.isSystem == true)
                    {

                        var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == update.PageId && s.isSystem == true).ToList();
                        if (existingTempalte.Count > 0)
                        {
                            foreach (var item in existingTempalte)
                            {
                                item.IsDefaultTemplate = false;
                                _context.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        var existingTempalte = _context.SearchResultTemplateModel.Where(s => s.PageId == update.PageId && s.isSystem == false && s.AccountId == update.AccountId).ToList();
                        if (existingTempalte.Count > 0)
                        {
                            foreach (var item in existingTempalte)
                            {
                                item.IsDefaultTemplate = false;
                                _context.Entry(item).State = EntityState.Modified;
                            }
                        }
                    }
                    update.IsDefaultTemplate = isDefault;
                    _context.Entry(update).State = EntityState.Modified;
                }
                else
                {
                    update.IsDefaultTemplate = isDefault;
                    _context.Entry(update).State = EntityState.Modified;
                }
                if (settings != null && settings.Count > 0)
                {
                    var details = _context.SearchResultDetailTemplateModel.Where(s => s.SearchResultTemplateId == templateId).ToList();
                    foreach (var item in details)
                    {
                        _context.Entry(item).State = EntityState.Deleted;
                    }

                    foreach (var field in settings)
                    {
                        SearchResultDetailTemplateModel detail = new SearchResultDetailTemplateModel();
                        detail.SearchResultTemplateId =templateId;
                        detail.SearchResultDetailTemplateId = Guid.NewGuid();
                        detail.PivotArea = field.PivotArea;
                        detail.FieldName = field.FieldName;
                        detail.CellFormat_FormatType = field.CellFormat_FormatType;
                        detail.CellFormat_FormatString = field.CellFormat_FormatString;
                        detail.Caption = field.Caption;
                        detail.AreaIndex = field.AreaIndex;
                        detail.Visible = field.Visible;
                        detail.Width = field.Width;
                        detail.Height = field.Height;
                        detail.Resize = field.Resize;
                        detail.Tree = field.Tree;

                        _context.SearchResultDetailTemplateModel.Add(detail);
                    }
                   
                }

            }


        }
        #endregion END GRATT CHART
    }
}
