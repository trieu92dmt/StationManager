using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using Reports.Models;
using Reports.Helpers;
namespace Reports.Options
{
    public class ChartBar3DDemoOptions : Chart3DDemoOptions
    {
        bool showFacet = true;
        Bar3DModel bar3DModel = Bar3DModel.Box;
        byte transparency = 45;

        public bool ShowFacet
        {
            get { return showFacet; }
            set { showFacet = value; }
        }
        [DisplayName("3D Model")]
        public Bar3DModel Bar3DModel
        {
            get { return bar3DModel; }
            set { bar3DModel = value; }
        }
        public byte Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }
    }

    public class ChartBarFullStacked3DDemoOptions : ChartBar3DDemoOptions
    {
        bool valueAsPercent = true;

        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartBarDemoOptions : ChartDemoOptions
    {
        BarSeriesLabelPosition labelPosition = BarSeriesLabelPosition.Center;
        TextOrientation textOrientation = TextOrientation.Horizontal;
        int labelIndent = 2;

        public BarSeriesLabelPosition LabelPosition
        {
            get { return labelPosition; }
            set { labelPosition = value; }
        }
        [DisplayName("Orientation")]
        public TextOrientation TextOrientation
        {
            get { return textOrientation; }
            set { textOrientation = value; }
        }
        public int LabelIndent
        {
            get { return labelIndent; }
            set { labelIndent = value; }
        }
    }

    public class ChartStackedBarDemoOptions : ChartBarDemoOptions
    {
        bool showTotalLabels = true;

        public bool ShowTotalLabels
        {
            get { return showTotalLabels; }
            set { showTotalLabels = value; }
        }
    }

    public class ChartBarFullStackedDemoOptions : ChartStackedBarDemoOptions
    {
        bool valueAsPercent = true;

        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartSideBySideBarDemoOptions : ChartBarFullStackedDemoOptions
    {
        GroupBy group = GroupBy.Sex;

        [DisplayName("Group Series by")]
        public GroupBy Group
        {
            get { return group; }
            set { group = value; }
        }
    }

    public class ChartSideBySideBar3DDemoOptions : ChartBarFullStacked3DDemoOptions
    {
        GroupBy group = GroupBy.Sex;

        [DisplayName("Group Series by")]
        public GroupBy Group
        {
            get { return group; }
            set { group = value; }
        }
    }

    public class ChartStepLineDemoOptions : ChartMarkerDemoOptions
    {
        bool invertedStep = false;

        protected override MarkerKind DefaultMarkerKind { get { return MarkerKind.Square; } }

        public bool InvertedStep
        {
            get { return invertedStep; }
            set { invertedStep = value; }
        }
    }

    public class ChartStepLine3DDemoOptions : Chart3DDemoOptions
    {
        bool invertedStep = false;

        public bool InvertedStep
        {
            get { return invertedStep; }
            set { invertedStep = value; }
        }
    }

    public class ChartSplineDemoOptions : ChartMarkerDemoOptions
    {
        int tension = 75;

        [DisplayName("Line Tension, %")]
        public int Tension
        {
            get { return tension; }
            set { tension = value; }
        }
    }

    public class ChartDrillDownDemoOptions : ChartDemoOptions
    {
        bool showSummaryFunction = true;
        Summary summaryFunction = Summary.Min;


        public Summary SummaryFunction
        {
            get { return summaryFunction; }
            set { summaryFunction = value; }
        }
        public bool ShowSummaryFunction
        {
            get { return showSummaryFunction; }
            set { showSummaryFunction = value; }
        }

        public ChartDrillDownDemoOptions()
        {
        }
    }

    public class ChartBoxPlotDemoOptions : ChartDemoOptions
    {
        public BoxPlotData BoxPlotData
        {
            get { return (BoxPlotData)Data; }
        }
        public ChartBoxPlotDemoOptions()
        {
        }
    }

    public class ChartToolTipDemoOptions : ChartDemoOptions
    {
        ToolTipOpenMode openingMode = ToolTipOpenMode.OnHover;
        Dictionary<string, int> toolTipImages = new Dictionary<string, int>();
        ToolTipPositions position = ToolTipPositions.Mouse;

        public ToolTipOpenMode OpeningMode
        {
            get { return openingMode; }
            set { openingMode = value; }
        }
        public Dictionary<string, int> ToolTipImages
        {
            get { return toolTipImages; }
            set { toolTipImages = value; }
        }
        public ToolTipPositions Position
        {
            get { return position; }
            set { position = value; }
        }
    }

    public class ChartRenderFormatOptions : ChartDemoOptions
    {
        public ViewType SeriesView { get; set; }
        public RenderFormat RenderFormat { get; set; }
        public SeriesDataType DataType { get; set; }
    }

    public class ChartSegmentColorizerOptions : ChartDemoOptions
    {
        public ViewType SeriesView { get; set; }
        public string SegmentColorizer { get; set; }
    }

    public class ChartHistogramOptions : ChartDemoOptions
    {
        IntervalDivisionMode divisionMode = IntervalDivisionMode.Width;
        double width = 5;
        int count = 30;

        public IntervalDivisionMode IntervalDivisionMode { get { return divisionMode; } set { divisionMode = value; } }
        public double Width { get { return width; } set { width = value; } }
        public int Count { get { return count; } set { count = value; } }
        public int SeriesCount { get; set; }
        public ViewType HistogramView { get; set; }
    }

    public class ChartTimeSpanScaleDemoOptions : ChartDemoOptions
    {
    }

    public class ChartRangeDemoOptions : ChartMarkerDemoOptions
    {
        RangeAreaLabelKind labelKinds = RangeAreaLabelKind.TwoLabels;
        bool showMarkers2 = true;
        string marker2KindString = MarkerKind.Circle.ToString();
        MarkerKind marker2Kind = MarkerKind.Circle;
        int marker2Size = 10;
        int starPoint2Count = 3;

        [DisplayName("Marker Kind")]
        public string Marker2KindString
        {
            get { return marker2KindString; }
            set
            {
                if (value == null)
                    marker2Kind = MarkerKind.Circle;
                else if (value.Contains(ChartDemoHelper.StarKey))
                {
                    starPoint2Count = Int32.Parse(value.Remove(0, ChartDemoHelper.StarKey.Length));
                    marker2Kind = MarkerKind.Star;
                }
                else
                    marker2Kind = (MarkerKind)Enum.Parse(typeof(MarkerKind), value);
                marker2KindString = value;
            }
        }
        public MarkerKind Marker2Kind { get { return marker2Kind; } }
        [DisplayName("Show Markers")]
        public bool ShowMarkers2
        {
            get { return showMarkers2; }
            set { showMarkers2 = value; }
        }
        public int StarPoint2Count { get { return starPoint2Count; } }
        [DisplayName("Marker Size")]
        public int Marker2Size
        {
            get { return marker2Size; }
            set { marker2Size = value; }
        }
        public RangeAreaLabelKind LabelKind
        {
            get { return labelKinds; }
            set { labelKinds = value; }
        }
    }

    public class ChartRange3DDemoOptions : Chart3DDemoOptions
    {
        RangeAreaLabelKind labelKinds = RangeAreaLabelKind.TwoLabels;

        public RangeAreaLabelKind LabelKind
        {
            get { return labelKinds; }
            set { labelKinds = value; }
        }
    }

    public class ChartPieDoughnutDemoOptions : ChartDemoOptions
    {
        static List<string> listExplodeModes = new List<string>(){
            PieExplodeMode.None.ToString(),
            PieExplodeMode.All.ToString(),
            PieExplodeMode.MinValue.ToString(),
            PieExplodeMode.MaxValue.ToString()
        };
        int holeRadiusPercent = 60;
        string explodedPoints = PieExplodeMode.None.ToString();
        string explodePoint;
        PieExplodeMode explodeMode = PieExplodeMode.None;
        PieSeriesLabelPosition labelPosition = PieSeriesLabelPosition.Radial;
        int explodeDistance = 10;
        bool valueAsPercent = true;
        bool showTotalLabel = true;

        public PieSeriesLabelPosition LabelPosition
        {
            get { return labelPosition; }
            set { labelPosition = value; }
        }
        public string ExplodedPoints
        {
            get { return explodedPoints; }
            set
            {
                explodedPoints = value;
                if (listExplodeModes.Contains(value))
                {
                    explodePoint = null;
                    explodeMode = (PieExplodeMode)Enum.Parse(typeof(PieExplodeMode), value);
                }
                else
                    explodePoint = value;
            }
        }
        public int ExplodeDistance
        {
            get { return explodeDistance; }
            set { explodeDistance = value; }
        }
        public string ExplodePoint { get { return explodePoint; } }
        public PieExplodeMode ExplodeMode { get { return explodeMode; } }
        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
        [DisplayName("Hole Radius, %")]
        public int HoleRadiusPercent
        {
            get { return holeRadiusPercent; }
            set { holeRadiusPercent = value; }
        }
        public bool ShowTotalLabel
        {
            get { return showTotalLabel; }
            set { showTotalLabel = value; }
        }
    }

    public class ChartNestedDoughnutDemoOptions : ChartDemoOptions
    {
        bool showAgeStruct = true;
        int holeRadiusPercent = 40;
        double innerIndent = 5;

        [DisplayName("Group Series by")]
        public bool ShowAgeStruct
        {
            get { return showAgeStruct; }
            set { showAgeStruct = value; }
        }
        [DisplayName("Hole Radius, %")]
        public int HoleRadiusPercent
        {
            get { return holeRadiusPercent; }
            set { holeRadiusPercent = value; }
        }
        public double InnerIndent
        {
            get { return innerIndent; }
            set { innerIndent = value; }
        }
    }

    public class ChartBubbleDemoOptions : ChartMarkerDemoOptions
    {
        decimal minBubbleSize = 1.0M;
        decimal maxBubbleSize = 3.5M;
        byte transparency = 90;

        protected override PointLabelPosition DefaultLabelPosition { get { return PointLabelPosition.Center; } }

        public decimal MinBubbleSize
        {
            get { return minBubbleSize; }
            set
            {
                if (value < maxBubbleSize)
                    minBubbleSize = value;
            }
        }
        public decimal MaxBubbleSize
        {
            get { return maxBubbleSize; }
            set
            {
                if (value > minBubbleSize)
                    maxBubbleSize = value;
            }
        }
        public byte Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }
    }

    public class ChartScatterDemoOptions : ChartMarkerDemoOptions
    {
        ScatterFunctions functionType = ScatterFunctions.ArchimedeanSpiral;

        protected override int DefaultMarkerSize { get { return 8; } }

        public ScatterFunctions FunctionType
        {
            get { return functionType; }
            set { functionType = value; }
        }
    }

    public class ChartAreaDemoOptions : ChartMarkerDemoOptions
    {
        byte transparency = 135;

        public byte Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }
    }

    public class ChartAreaFullStckedDemoOptions : ChartAreaDemoOptions
    {
        bool valueAsPercent;

        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartStepAreaFullStckedDemoOptions : ChartAreaFullStckedDemoOptions
    {
        bool invertedStep = false;

        public bool InvertedStep
        {
            get { return invertedStep; }
            set { invertedStep = value; }
        }
    }

    public class ChartArea3DDemoOptions : Chart3DDemoOptions
    {
        byte transparency = 135;

        public byte Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }
    }

    public class ChartAreaFullStcked3DDemoOptions : ChartArea3DDemoOptions
    {
        bool valueAsPercent = true;

        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartStepAreaDemoOptions : ChartAreaDemoOptions
    {
        bool invertedStep = false;

        public bool InvertedStep
        {
            get { return invertedStep; }
            set { invertedStep = value; }
        }
    }

    public class ChartStepArea3DDemoOptions : ChartArea3DDemoOptions
    {
        bool invertedStep = false;

        public bool InvertedStep
        {
            get { return invertedStep; }
            set { invertedStep = value; }
        }
    }

    public class ChartLineFullStckedDemoOptions : ChartMarkerDemoOptions
    {
        bool valueAsPercent = true;

        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartFunnelDemoOptions : ChartDemoOptions
    {
        FunnelSeriesLabelPosition labelPosition = FunnelSeriesLabelPosition.Right;
        int pointDistance = 1;
        double heightToWidthRatio = 1;
        bool valueAsPercent = true;

        public FunnelSeriesLabelPosition LabelPosition
        {
            get { return labelPosition; }
            set { labelPosition = value; }
        }
        public int PointDistance
        {
            get { return pointDistance; }
            set { pointDistance = value; }
        }
        [DisplayName("Height / Width")]
        public double HeightToWidthRatio
        {
            get { return heightToWidthRatio; }
            set { heightToWidthRatio = value; }
        }
        [DisplayName("Value as Percent")]
        public bool ValueAsPercent
        {
            get { return valueAsPercent; }
            set { valueAsPercent = value; }
        }
    }

    public class ChartFunnel2DDemoOptions : ChartFunnelDemoOptions
    {
        bool alignToCenter = false;
        bool heightToWidthRatioAuto = false;

        [DisplayName("Align to Center")]
        public bool AlignToCenter
        {
            get { return alignToCenter; }
            set { alignToCenter = value; }
        }
        [DisplayName("Auto Height / Width")]
        public bool HeightToWidthRatioAuto
        {
            get { return heightToWidthRatioAuto; }
            set { heightToWidthRatioAuto = value; }
        }
    }

    public class ChartFunnel3DDemoOptions : ChartFunnelDemoOptions
    {
        int holeRadius = 90;

        [DisplayName("Hole Radius, %")]
        public int HoleRadius
        {
            get { return holeRadius; }
            set { holeRadius = value; }
        }
    }

    public class ChartRadarDemoOptions : ChartMarkerDemoOptions
    {
        RadarDiagramDrawingStyle diagramType = RadarDiagramDrawingStyle.Circle;
        PolarFunctions polarFunction = PolarFunctions.Circles;
        RadarAxisXLabelTextDirection textDirection = RadarAxisXLabelTextDirection.Radial;
        ScatterRadarFunctions scatterFunction = ScatterRadarFunctions.ArchimedeanSpiral;

        protected override int DefaultMarkerSize { get { return 8; } }

        [DisplayName("Function Type")]
        public PolarFunctions PolarFunction
        {
            get { return polarFunction; }
            set { polarFunction = value; }
        }
        public RadarDiagramDrawingStyle DiagramType
        {
            get { return diagramType; }
            set { diagramType = value; }
        }
        public RadarAxisXLabelTextDirection TextDirection
        {
            get { return textDirection; }
            set { textDirection = value; }
        }
        [DisplayName("Function Type")]
        public ScatterRadarFunctions ScatterFunction
        {
            get { return scatterFunction; }
            set { scatterFunction = value; }
        }

        public ChartRadarDemoOptions()
        {
            MarkerSize = 8;
        }
    }

    public class ChartRadarRangeDemoOptions : ChartRadarDemoOptions
    {
        RangeAreaLabelKind labelKinds = RangeAreaLabelKind.TwoLabels;
        bool showMarkers2 = true;

        public bool ShowMarkers2
        {
            get { return showMarkers2; }
            set { showMarkers2 = value; }
        }
        public RangeAreaLabelKind LabelKind
        {
            get { return labelKinds; }
            set { labelKinds = value; }
        }
    }

    public abstract class ChartFinancialDemoOptions : ChartDemoOptions
    {
        StockLevel reductionLevel = StockLevel.Close;
        StockLevel labelLevel = StockLevel.Close;

        public StockLevel ReductionLevel
        {
            get { return reductionLevel; }
            set { reductionLevel = value; }
        }
        public StockLevel LabelLevel
        {
            get { return labelLevel; }
            set { labelLevel = value; }
        }
    }
    public class ChartStockDemoOptions : ChartFinancialDemoOptions
    {
        bool workDaysOnly = true;

        public bool WorkDaysOnly
        {
            get { return workDaysOnly; }
            set { workDaysOnly = value; }
        }
    }
    public class ChartCandleDemoOptions : ChartFinancialDemoOptions
    {
        bool skipRangesWithoutPoints = true;

        public bool SkipRangesWithoutPoints
        {
            get { return skipRangesWithoutPoints; }
            set { skipRangesWithoutPoints = value; }
        }
    }

    public class Chart3DDemoOptions : ChartDemoOptions
    {
        int perspectiveAngle = 45;
        int zoomPercent = 140;

        public int PerspectiveAngle
        {
            get { return perspectiveAngle; }
            set { perspectiveAngle = value; }
        }
        public int ZoomPercent
        {
            get { return zoomPercent; }
            set { zoomPercent = value; }
        }
    }

    public class ChartMarkerDemoOptions : ChartDemoOptions
    {
        int markerSize;
        string markerKindString;
        MarkerKind markerKind;
        PointLabelPosition labelPosition;
        int starPointCount = 3;
        bool showMarkers = true;
        int labelAngle = 45;

        protected virtual int DefaultMarkerSize { get { return 10; } }
        protected virtual PointLabelPosition DefaultLabelPosition { get { return PointLabelPosition.Outside; } }
        protected virtual MarkerKind DefaultMarkerKind { get { return MarkerKind.Circle; } }

        [DisplayName("Marker Kind")]
        public string MarkerKindString
        {
            get { return markerKindString; }
            set
            {
                if (value == null)
                    markerKind = MarkerKind.Circle;
                else if (value.Contains(ChartDemoHelper.StarKey))
                {
                    starPointCount = Int32.Parse(value.Remove(0, ChartDemoHelper.StarKey.Length));
                    markerKind = MarkerKind.Star;
                }
                else
                    markerKind = (MarkerKind)Enum.Parse(typeof(MarkerKind), value);
                markerKindString = value;
            }
        }
        public MarkerKind MarkerKind { get { return markerKind; } }
        public PointLabelPosition LabelPosition
        {
            get { return labelPosition; }
            set { labelPosition = value; }
        }
        public int StarPointCount { get { return starPointCount; } }
        public int MarkerSize
        {
            get { return markerSize; }
            set { markerSize = value; }
        }
        public bool ShowMarkers
        {
            get { return showMarkers; }
            set { showMarkers = value; }
        }
        public int LabelAngle
        {
            get { return labelAngle; }
            set { labelAngle = value; }
        }
        public ChartMarkerDemoOptions()
        {
            this.labelPosition = DefaultLabelPosition;
            this.markerSize = DefaultMarkerSize;
            this.markerKind = DefaultMarkerKind;
            this.markerKindString = this.markerKind.ToString();
        }
    }

    public class ChartDemoOptions
    {
        bool showLabels;
        object data;

        public bool ShowLabels
        {
            get { return showLabels; }
            set { showLabels = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public ChartDemoOptions()
        {
        }
    }

    public class WaterfallChartDemoOptions : ChartDemoOptions
    {
        public DataType DataType { get; set; }

        public WaterfallChartDemoOptions()
        {
        }
    }

    public enum GroupBy
    {
        Sex,
        Age
    }
    public enum DataType
    {
        AggregatedData,
        DetailedData
    }
    public enum ScatterFunctions
    {
        ArchimedeanSpiral,
        Cardioid,
        CartesianFolium
    }
    public enum PolarFunctions
    {
        Circles,
        Cardioid,
        Lemniscate
    }
    public enum ScatterRadarFunctions
    {
        ArchimedeanSpiral,
        PolarRose,
        PolarFolium
    }
    public enum ToolTipPositions
    {
        Relative,
        Free,
        Mouse
    }
    public enum Summary
    {
        Min,
        Max,
        Average
    }
}