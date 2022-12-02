using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.Mvc;
using DevExpress.XtraPivotGrid;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ISD.Extensions
{
    public class PivotGridExportExcel
    {
        //public static PivotGridSettings DataAwarePivotGridSettings(T searchModel)
        //{
        //    if (dataAwarePivotGridSettings == null)
        //        dataAwarePivotGridSettings = CreateDataAwarePivotGridSettings(searchModel);
        //    return dataAwarePivotGridSettings;

        //}
        /// <summary>
        /// Action Export
        /// </summary>
        /// <param name="options">PivotGrid ExportOptions</param>
        /// <param name="data">Data model</param>
        /// <returns>File</returns>
        public static ActionResult GetExportActionResult(string fileName, PivotGridExportOptions options, List<FieldSettingModel> listSetting, object data, string HeaderName = null)
        {
            var _pivotGridHelper = new PivotGridDataOutputHelper();
            _pivotGridHelper.ExportPivotGridSettings = CreatePivotGridSettings(fileName, listSetting);

            return _pivotGridHelper.GetExportActionResult(options, data, HeaderName: HeaderName);
        }

        public static ActionResult GetExportActionResult(string fileName, PivotGridExportOptions options, List<FieldSettingModel> listSetting, object data, List<SearchDisplayModel> searchInfoList, string HeaderName = null)
        {
            var _pivotGridHelper = new PivotGridDataOutputHelper();

            _pivotGridHelper.ExportPivotGridSettings = CreatePivotGridSettings(fileName, listSetting);
            return _pivotGridHelper.GetExportActionResult(options, data, searchInfoList, HeaderName);
        }
        /// <summary>
        /// Setting config Field for pivot
        /// </summary>
        /// <returns></returns>
        private static PivotGridSettings CreatePivotGridSettings(string fileName, List<FieldSettingModel> listSetting)
        {
            PivotGridSettings settings = CreatePivotGridSettingsBase(fileName);

            foreach (var fieldSetting in listSetting)
            {
                settings.Fields.Add(field =>
                {
                    if (fieldSetting.PivotArea == 0)
                    {
                        field.Area = PivotArea.RowArea;
                    }
                    if (fieldSetting.PivotArea == 1)
                    {
                        field.Area = PivotArea.ColumnArea;
                    }
                    if (fieldSetting.PivotArea == 2)
                    {
                        field.Area = PivotArea.FilterArea;
                    }
                    if (fieldSetting.PivotArea == 3)
                    {
                        field.Area = PivotArea.DataArea;
                    }
                    field.FieldName = fieldSetting.FieldName;
                    field.Caption = fieldSetting.Caption;
                    field.AreaIndex = fieldSetting.AreaIndex.Value;
                    field.Visible = fieldSetting.Visible.Value;
                    if (fieldSetting.CellFormat_FormatType == "DateTime")
                    {
                        field.ValueFormat.FormatType = FormatType.DateTime;
                        field.ValueFormat.FormatString = fieldSetting.CellFormat_FormatString;
                        field.CellFormat.FormatType = FormatType.DateTime;
                        field.CellFormat.FormatString = fieldSetting.CellFormat_FormatString;
                        field.ValueStyle.HorizontalAlign = HorizontalAlign.Center;
                        field.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                    if (fieldSetting.CellFormat_FormatType == "Numeric")
                    {
                        field.ValueFormat.FormatType = FormatType.Numeric;
                        field.ValueFormat.FormatString = fieldSetting.CellFormat_FormatString;
                        field.CellFormat.FormatType = FormatType.Numeric;
                        field.CellFormat.FormatString = fieldSetting.CellFormat_FormatString;
                        field.ValueStyle.HorizontalAlign = HorizontalAlign.Right;
                        field.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    }
                    //field.ExportBestFit = true;
                    if (field.FieldName == "TuoiTonWarning")
                    {
                        field.SummaryType = PivotSummaryType.Min;
                    }
                    if (fieldSetting.FieldName == "KTEXT" || fieldSetting.FieldName == "WorkCenterName" || fieldSetting.FieldName == "DepartmentName" || fieldSetting.FieldName == "StockName"
                     || fieldSetting.FieldName == "IDNRK_MES" || fieldSetting.FieldName == "MAKTX" || fieldSetting.FieldName == "BMEIN"
                    || fieldSetting.FieldName == "ProductName" || fieldSetting.FieldName == "ProductAttributes"  || fieldSetting.FieldName == "CDL" || fieldSetting.FieldName == "CDN" 
                    || fieldSetting.FieldName == "Warning" || fieldSetting.FieldName == "PBL01" || fieldSetting.FieldName == "PBL02" || fieldSetting.FieldName == "PBL03" || fieldSetting.FieldName == "PBL04" || fieldSetting.FieldName == "PBL05"
                    || fieldSetting.FieldName == "PBL06" || fieldSetting.FieldName == "PBL07" || fieldSetting.FieldName == "PBL08" || fieldSetting.FieldName == "PBL09" || fieldSetting.FieldName == "PBL10"
                    || fieldSetting.FieldName == "PBL11" || fieldSetting.FieldName == "AssignResponsibility"

              )
                    {
                        field.SummaryType = PivotSummaryType.Min;
                        field.ValueStyle.HorizontalAlign = HorizontalAlign.Left;
                        field.CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    }
                    if (fieldSetting.Caption == "STT")
                    {
                        field.ValueStyle.HorizontalAlign = HorizontalAlign.Center;
                        field.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                    if (fieldSetting.FieldName == "CompletedDateTime" || fieldSetting.FieldName == "NgayHTKH" || fieldSetting.FieldName == "NgayHTTT" || fieldSetting.FieldName == "EndDateDSX")
                    {
                        field.SummaryType = PivotSummaryType.Min;
                        field.ValueStyle.HorizontalAlign = HorizontalAlign.Center;
                        field.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                });
            }

            return settings;
        }

        /// <summary>
        /// Setting base for pivot
        /// </summary>
        /// <returns>PivotGridSettings</returns>
        private static PivotGridSettings CreatePivotGridSettingsBase(string fileName)
        {
            PivotGridSettings settings = new PivotGridSettings();
            settings.Name = fileName;//Pivot name, File export name
            settings.Width = Unit.Percentage(100);
            settings.OptionsPager.RowsPerPage = 20;
            settings.OptionsPager.PageSizeItemSettings.Visible = true;
            settings.OptionsCustomization.AllowDrag = true;
            settings.OptionsCustomization.AllowDragInCustomizationForm = true;
            settings.OptionsCustomization.AllowSort = false;
            //row
            settings.OptionsView.ShowRowTotals = false;// không show total theo row
            settings.OptionsView.ShowRowGrandTotals = false;
            settings.OptionsView.ShowRowGrandTotalHeader = false;
            //column
            settings.OptionsView.ShowColumnGrandTotalHeader = false;
            settings.OptionsView.ShowColumnGrandTotals = false;
            settings.OptionsView.ShowColumnTotals = false;
            //singlevalues
            settings.OptionsView.ShowGrandTotalsForSingleValues = false;

            settings.CustomCellValue = (sender, e) =>
            {
                if (Convert.ToString(e.Value) == "0" || Convert.ToString(e.Value) == "0.0" || Convert.ToString(e.Value) == "0.00" || Convert.ToString(e.Value) == "0.000")
                    e.Value = "";
                else if (e.Value == null)
                    e.Value = "";
                else if (Convert.ToString(e.Value).Contains("ĐỎ"))
                {
                    e.Value = e.Value.ToString().Replace("ĐỎ", "").Trim();
                }
                else if (Convert.ToString(e.Value).Contains("CAM"))
                {
                    e.Value = e.Value.ToString().Replace("CAM", "").Trim();
                }
                else if (Convert.ToString(e.Value).Contains("VÀNG"))
                {
                    e.Value = e.Value.ToString().Replace("VÀNG", "").Trim();
                }
                else if (Convert.ToString(e.Value).Contains("XANH"))
                {
                    e.Value = e.Value.ToString().Replace("XANH", "").Trim();
                }
                else
                    return;
            };

            //custom  row header
            settings.SettingsExport.CustomExportHeader = (sender, e) =>
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#C6EFCE");
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            };
            //custom colunm header
            settings.SettingsExport.CustomExportFieldValue = (sender, e) =>
            {
                if (e.IsColumn)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#C6EFCE");
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    if (e.Field != null)
                    {
                        if (e.Field.ValueStyle.HorizontalAlign == HorizontalAlign.Center)
                        {
                            e.Appearance.TextHorizontalAlignment = HorzAlignment.Center;
                        }
                        if (e.Field.ValueStyle.HorizontalAlign == HorizontalAlign.Right)
                        {
                            e.Appearance.TextHorizontalAlignment = HorzAlignment.Far;
                        }
                        if (Convert.ToString(e.Value) == "XANH")
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml("#008000");
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("#008000");
                        }
                        else if (Convert.ToString(e.Value) == "VÀNG")
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml("#FFFF00");
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("#FFFF00");
                        }
                        else if (Convert.ToString(e.Value) == "ĐỎ")
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml("#FF0000");
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("#FF0000");
                        }
                    }
                    if (e.DataField != null)
                    {
                        if (e.DataField.ValueStyle.HorizontalAlign == HorizontalAlign.Left)
                        {
                            e.Appearance.TextHorizontalAlignment = HorzAlignment.Near;
                        }
                    }
                }
            };
            //custom grand total row value
            settings.SettingsExport.CustomExportCell = (sender, e) =>
            {
                if (e.RowValue.ValueType == PivotGridValueType.GrandTotal)
                {
                    e.Appearance.BackColor = Color.Azure;
                }
                if (e.RowValue.ValueType == PivotGridValueType.Total)
                {
                    e.Appearance.BackColor = Color.Beige;
                }
                else
                {
                    PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
                    foreach (PivotDrillDownDataRow dr in ds)
                    {
                        string warning = (string)ds.GetValue(0, "Warning");
                        if (warning == "ĐỎ" && e.DataField != null && e.DataField.FieldName == "CapacityDetail")
                        {
                            e.Appearance.BackColor = Color.Red;
                        }
                    }
                }
                if (e.DataField != null)
                {
                    if (e.DataField.ValueStyle.HorizontalAlign == HorizontalAlign.Left)
                    {
                        e.Appearance.TextHorizontalAlignment = HorzAlignment.Near;
                    }
                    if (e.DataField.ValueStyle.HorizontalAlign == HorizontalAlign.Center)
                    {
                        e.Appearance.TextHorizontalAlignment = HorzAlignment.Center;
                    }
                }
            };
            return settings;
        }
        public static List<FieldSettingModel> GetCurrentSetting(ASPxPivotGrid PivotGrid)
        {
            List<FieldSettingModel> fieldSettingList = new List<FieldSettingModel>();
            var listField = PivotGrid.Fields;
            foreach (PivotGridField field in listField)
            {
                FieldSettingModel fieldSetting = new FieldSettingModel();
                fieldSetting.Caption = field.Caption;
                fieldSetting.AreaIndex = field.AreaIndex;
                fieldSetting.FieldName = field.FieldName;
                if (field.Area == PivotArea.RowArea)
                {
                    fieldSetting.PivotArea = 0;

                }
                else
                {
                    if (field.Area == PivotArea.ColumnArea)
                    {
                        fieldSetting.PivotArea = 1;
                    }
                    else
                    {
                        if (field.Area == PivotArea.FilterArea)
                        {
                            fieldSetting.PivotArea = 2;
                        }
                        else
                        {
                            fieldSetting.PivotArea = 3;
                        }
                    }
                }
                if (field.ValueFormat.FormatType == FormatType.Numeric || field.ValueFormat.FormatType == FormatType.DateTime)
                {
                    fieldSetting.CellFormat_FormatString = field.ValueFormat.FormatString;
                    fieldSetting.CellFormat_FormatType = field.ValueFormat.FormatType.ToString();
                }
                fieldSetting.Visible = field.Visible;
                fieldSettingList.Add(fieldSetting);
            }
            return fieldSettingList;
        }
    }
}
