using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using ISD.ViewModels;
using Reports.Options;

namespace Reports.Helpers
{
    public static class ChartDemoHelper
    {
        public const string CategoryKey = "Category";
        public const string StarKey = "star:";
        public const string CompletedDateKey = "CompletedDate";
        public const string ModelKey = "Model";

        public static List<ListEditItem> GetPieLabelPositions()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Radial", PieSeriesLabelPosition.Radial),
                new ListEditItem("Inside", PieSeriesLabelPosition.Inside),
                new ListEditItem("Outside", PieSeriesLabelPosition.Outside),
                new ListEditItem("Two Columns", PieSeriesLabelPosition.TwoColumns),
            };
        }
        public static List<ListEditItem> GetPieExplodePoints()
        {
            List<ListEditItem> explodePoints = new List<ListEditItem>() {
                new ListEditItem("None", PieExplodeMode.None),
                new ListEditItem("All", PieExplodeMode.All),
                new ListEditItem("Min Value", PieExplodeMode.MinValue),
                new ListEditItem("Max Value", PieExplodeMode.MaxValue),
            };
            return explodePoints;
        }
        public static List<ListEditItem> GetFunnelLabelPositions()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Right", FunnelSeriesLabelPosition.Right),
                new ListEditItem("Left", FunnelSeriesLabelPosition.Left),
                new ListEditItem("Center", FunnelSeriesLabelPosition.Center),
                new ListEditItem("Right Column", FunnelSeriesLabelPosition.RightColumn),
                new ListEditItem("Left Column", FunnelSeriesLabelPosition.LeftColumn)
            };
        }
        public static List<ListEditItem> GetMarkerKinds()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Circle", DevExpress.XtraCharts.MarkerKind.Circle),
                new ListEditItem("Cross", DevExpress.XtraCharts.MarkerKind.Cross),
                new ListEditItem("Diamond", DevExpress.XtraCharts.MarkerKind.Diamond),
                new ListEditItem("Hexagon", DevExpress.XtraCharts.MarkerKind.Hexagon),
                new ListEditItem("Inverted Triangle", DevExpress.XtraCharts.MarkerKind.InvertedTriangle),
                new ListEditItem("Pentagon", DevExpress.XtraCharts.MarkerKind.Pentagon),
                new ListEditItem("Plus", DevExpress.XtraCharts.MarkerKind.Plus),
                new ListEditItem("Square", DevExpress.XtraCharts.MarkerKind.Square),
                new ListEditItem("Triangle", DevExpress.XtraCharts.MarkerKind.Triangle),
                new ListEditItem("Star 3-points", StarKey + "3"),
                new ListEditItem("Star 4-points", StarKey + "4"),
                new ListEditItem("Star 5-points", StarKey + "5"),
                new ListEditItem("Star 6-points", StarKey + "6"),
                new ListEditItem("Star 10-points", StarKey + "10")
            };
        }
        public static List<ListEditItem> GetBarLabelPositions(bool isStacked)
        {
            List<ListEditItem> barLabelPositions = new List<ListEditItem>() {
                new ListEditItem("Top Inside", BarSeriesLabelPosition.TopInside),
                new ListEditItem("Center", BarSeriesLabelPosition.Center),
                new ListEditItem("Bottom Inside", BarSeriesLabelPosition.BottomInside),
            };
            if (!isStacked)
                barLabelPositions.Insert(0, new ListEditItem("Top", BarSeriesLabelPosition.Top));
            return barLabelPositions;
        }

        public static List<ListEditItem> GetSortValues()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Products", SeriesPointKey.Argument),
                new ListEditItem("Price", SeriesPointKey.Value_1)
            };
        }
        public static string[] GetSeriesDataMembers()
        {
            return new string[] { ChartSeriesTemplateBindingDemoOptions.Year, ChartSeriesTemplateBindingDemoOptions.Country };
        }
        public static List<ListEditItem> GetAgeStructureModes()
        {
            return new List<ListEditItem>() { new ListEditItem("Age", "true"), new ListEditItem("Sex", "false") };
        }
        public static string[] GetTransparency()
        {
            return new string[] { "0", "45", "90", "135", "180", "225", "255" };
        }
        public static string[] GetPerspectiveAngles()
        {
            return new string[] { "0", "30", "45", "60", "90", "120", "135", "150" };
        }

        public static string[] GetLabelAngles()
        {
            return new string[] { "0", "45", "90", "135", "180", "225", "270", "315" };
        }
        public static List<ListEditItem> GetLabelKinds()
        {
            return new List<ListEditItem>() {
                new ListEditItem("One Label", RangeAreaLabelKind.OneLabel),
                new ListEditItem("Two Labels", RangeAreaLabelKind.TwoLabels),
                new ListEditItem("Min Value", RangeAreaLabelKind.MinValueLabel),
                new ListEditItem("Max Value", RangeAreaLabelKind.MaxValueLabel),
                new ListEditItem("Value 1 Label", RangeAreaLabelKind.Value1Label),
                new ListEditItem("Value 2 Label", RangeAreaLabelKind.Value2Label),
            };
        }
        public static string[] GetMarkerSizes()
        {
            return new string[] { "8", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30" };
        }
        public static string[] GetHeightToWidthRatio()
        {
            return new string[] { "0.1", "0.25", "0.5", "0.75", "1", "2", "4", "6", "8", "10" };
        }
        public static string[] GetLineTensions()
        {
            return new string[] { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        }
        public static string[] GetDoughnutHoleRadiuses()
        {
            return new string[] { "0", "15", "30", "50", "60", "75", "90", "100" };
        }
        public static List<ListEditItem> GetScatterFunctions()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Archimedean Spiral", ScatterFunctions.ArchimedeanSpiral),
                new ListEditItem("Cardioid", ScatterFunctions.Cardioid),
                new ListEditItem("Cartesian Folium", ScatterFunctions.CartesianFolium),
            };
        }
        public static List<ListEditItem> GetScatterRadarFunctions()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Archimedean Spiral", ScatterRadarFunctions.ArchimedeanSpiral.ToString()),
                new ListEditItem("Polar Rose", ScatterRadarFunctions.PolarRose.ToString()),
                new ListEditItem("Polar Folium", ScatterRadarFunctions.PolarFolium.ToString())
            };
        }
        public static List<ListEditItem> GetToolTipOpenModes()
        {
            return new List<ListEditItem> {
                new ListEditItem("On Hover", ToolTipOpenMode.OnHover),
                new ListEditItem("On Click", ToolTipOpenMode.OnClick)
            };
        }
        public static string[] GetZoomPercents()
        {
            return new string[] { "50", "75", "100", "120", "140", "170", "200", "250", "300" };
        }
        public static string[] GetPredefinedHoleRadiuses()
        {
            const int minRadius = 30;
            const int step = 5;
            const int itemsCount = 8;
            List<string> items = new List<string>();
            for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
            {
                int radius = minRadius + step * itemIndex;
                items.Add(radius.ToString());
            }
            return items.ToArray();
        }
        public static string[] GetPredefinedInnerIndents()
        {
            const int itemsCount = 11;
            List<string> items = new List<string>();
            for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
            {
                items.Add(itemIndex.ToString());
            }
            return items.ToArray();
        }
        public static List<string> GetExportFormats()
        {
            return new List<string>() { "pdf", "xls", "xlsx", "rtf", "mht", "png", "jpeg", "bmp", "tiff", "gif", "svg" };
        }
        public static List<ListEditItem> GetSegmentColorizers()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Range Segment Colorizer"),
                new ListEditItem("Trend Segment Colorizer"),
                new ListEditItem("Point Based Colorizer")
            };
        }
        public static List<ListEditItem> GetIntervalDivisionModes()
        {
            return new List<ListEditItem> {
                new ListEditItem("Auto", IntervalDivisionMode.Auto),
                new ListEditItem("Width", IntervalDivisionMode.Width),
                new ListEditItem("Count", IntervalDivisionMode.Count)
            };
        }
        public static List<ListEditItem> GetDataTypes()
        {
            return new List<ListEditItem>() {
                new ListEditItem("Aggregated Data", DataType.AggregatedData),
                new ListEditItem("Detailed Data", DataType.DetailedData),
            };
        }
    }
    public static class XMLUtils
    {
        public static DataTable LoadDataTableFromXml(string fileName, string tableName)
        {
            DataSet xmlDataSet = new DataSet();
            using (Stream xmlStream = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/" + fileName)))
            {
                xmlDataSet.ReadXml(xmlStream);
                xmlStream.Close();
            }
            return xmlDataSet.Tables[tableName];
        }
    }
}