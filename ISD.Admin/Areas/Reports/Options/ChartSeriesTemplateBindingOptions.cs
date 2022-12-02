using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reports.Options
{
    public class ChartSeriesTemplateBindingDemoOptions
    {
        public const string Country = "Country";
        public const string Year = "Year";

        bool showLabels;
        object data;
        string seriesDataMember = Year;

        public bool ShowLabels
        {
            get { return showLabels; }
            set { showLabels = value; }
        }
        public string SeriesDataMember
        {
            get { return seriesDataMember; }
            set { seriesDataMember = string.IsNullOrEmpty(value) ? Year : value; }
        }
        public object Data
        {
            get { return data; }
            set { data = value; }
        }
        public string ArgumentDataMember { get { return seriesDataMember == Country ? Year : Country; } }

        public ChartSeriesTemplateBindingDemoOptions() { }
    }
}