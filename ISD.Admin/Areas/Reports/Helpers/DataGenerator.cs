using System.Collections.Generic;
using System;

namespace Reports.Helpers
{
    class DataGenerator
    {
        const int RndMax = 20;
        const int RndMin = 10;

        Random rnd;
        int integer = 0;
        double financialClose;
        double value;
        double value2 = double.NaN;
        int multiplier;
        int seriesIndex;

        public DataGenerator(int seriesIndex, double startValue)
        {
            this.seriesIndex = seriesIndex;
            this.value = startValue;
            if (seriesIndex % 2 == 0)
                multiplier = 1;
            else
                multiplier = -1;
            rnd = new Random(seriesIndex);
            financialClose = rnd.Next(RndMax);
        }
        public DataGenerator(int rndSeek)
            : this(rndSeek, 0) { }
        public DataGenerator()
            : this(0, 0) { }

        DataPoint GenerateDataPoint(SeriesDataType seriesType)
        {
            DataPoint dataPoint;
            switch (seriesType)
            {
                case SeriesDataType.Financial:
                    dataPoint = GenerateFinancialDataPoint();
                    break;
                case SeriesDataType.ArgumentValue:
                    dataPoint = GenerateArgValueDataPoint();
                    break;
                case SeriesDataType.Bubble:
                    dataPoint = GenerateBubblePoint();
                    break;
                case SeriesDataType.Range:
                    dataPoint = GenerateRangeDataPoint();
                    break;
                case SeriesDataType.Funnel:
                    dataPoint = GenerateFunnelPoint();
                    break;
                default:
                    throw new NotImplementedException("Data generator for " + seriesType + "has not implemented");
            }
            return dataPoint;
        }

        DataPoint GenerateFunnelPoint()
        {
            DataPoint dataPoint = GenerateDataPointWithArgument();
            value -= rnd.Next(1, 7);
            dataPoint.Value = value;
            return dataPoint;
        }
        DataPoint GenerateBubblePoint()
        {
            DataPoint dataPoint = GenerateDataPointWithArgument();
            dataPoint.Value = rnd.Next(RndMax);
            dataPoint.Weight = rnd.Next(RndMax);
            return dataPoint;
        }
        DataPoint GenerateArgValueDataPoint()
        {
            DataPoint dataPoint = GenerateDataPointWithArgument();
            value += multiplier * Math.Round(5 * rnd.NextDouble() - 2.2);
            dataPoint.Value = value;
            return dataPoint;
        }
        DataPoint GenerateFinancialDataPoint()
        {
            DataPoint dataPoint = GenerateDataPointWithArgument();
            value += rnd.Next(-6, 5);
            dataPoint.Low = value;
            dataPoint.Open = value + rnd.Next(1, 5);
            double close = value + rnd.Next(1, 5);
            if (dataPoint.Open == close)
                close += 1.5;
            dataPoint.Close = close;
            dataPoint.High = Math.Max(dataPoint.Close, dataPoint.Open) + rnd.Next(1, 4);
            return dataPoint;
        }
        DataPoint GenerateRangeDataPoint()
        {
            if (double.IsNaN(value2))
                value2 = value + 20;
            DataPoint dataPoint = GenerateDataPointWithArgument();
            value += rnd.Next(-3, 3);
            value2 += rnd.Next(-9, 3);
            dataPoint.Value = value;
            dataPoint.Value2 = value2;
            return dataPoint;
        }
        DataPoint GenerateDataPointWithArgument()
        {
            integer++;
            var point = CreateDataPoint();
            point.Argument = integer;
            return point;
        }
        protected DataPoint CreateDataPoint()
        {
            return new DataPoint() { Series = String.Format("Series {0}", seriesIndex + 1) };
        }

        public List<DataPoint> GenerateData(SeriesDataType seriesType, int pointCount)
        {
            var data = new List<DataPoint>();
            for (int i = 0; i < pointCount; i++)
            {
                DataPoint point = GenerateDataPoint(seriesType);
                data.Add(point);
            }
            return data;
        }
        public List<DataPoint> GeneratePolarData(SeriesDataType seriesType, int pointCount)
        {
            double step = 360.0 / pointCount;
            double argument = -step;
            var data = new List<DataPoint>();
            for (int i = 0; i < pointCount; i++)
            {
                DataPoint point = CreateDataPoint();
                argument += step + rnd.Next(-10, 10);
                point.Argument = argument;
                point.Value = rnd.Next(RndMin, RndMax);
                if (seriesType == SeriesDataType.Range)
                    point.Value2 = rnd.Next(RndMin + 10, RndMax + 7);
                data.Add(point);
            }
            return data;
        }
        public List<DataPoint> GenerateXyScatterData()
        {
            var data = new List<DataPoint>();
            for (int i = 0; i < 720; i += 10)
            {
                double t = (double)i / 180 * Math.PI;
                double x = t * Math.Cos(t);
                double y = t * Math.Sin(t);
                DataPoint dataPoint = CreateDataPoint();
                dataPoint.Argument = x;
                dataPoint.Value = y;
                data.Add(dataPoint);
            }
            return data;
        }
    }


    public class DataPoint
    {
        public string Series { get; set; }
        public double Argument { get; set; }
        public double Value { get; set; }
        public double Value2 { get; set; }
        public double Weight { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
    }


    public enum SeriesDataType
    {
        ArgumentValue,
        Financial,
        Bubble,
        Range,
        Funnel,
    }

    static class LargeDataGenerator
    {
        public static List<SimpleDataPoint> GenerateSeriesDataSource(int pointsCount)
        {
            List<SimpleDataPoint> points = new List<SimpleDataPoint>(pointsCount);
            Random random = new Random(DateTime.Now.Millisecond);
            double value = 0;
            double delta = (random.NextDouble()) / 75;
            for (int i = 0; i < pointsCount; i++)
            {
                points.Add(new SimpleDataPoint(i, value));
                value = value + random.NextDouble() - 0.5 + delta;
            }
            return points;
        }
    }

    class SimpleDataPoint
    {
        public double Argument { get; private set; }
        public double Value { get; private set; }

        public SimpleDataPoint(double argument, double value)
        {
            Argument = argument;
            Value = value;
        }
    }
}