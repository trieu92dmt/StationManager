using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reports.Models
{
    public static class BoxPlotDataProvider
    {
        static BoxPlotData BoxPlotData
        {
            get { return (BoxPlotData)HttpContext.Current.Session["BoxPlotData"]; }
            set { HttpContext.Current.Session["BoxPlotData"] = value; }
        }

        public static BoxPlotData GetData()
        {
            BoxPlotData boxPlotData = new BoxPlotData();
            BoxPlotData = boxPlotData;
            return boxPlotData;
        }
        public static BoxPlotData GetModifiedData()
        {
            BoxPlotData boxPlotData = BoxPlotData;
            boxPlotData.AddNewDataSet();
            return boxPlotData;
        }
    }

    public class BoxPlotData
    {
        const int InitialExperimentCount = 6;
        const int RandomValuesPerExperiment = 300;
        const double SecondPointSeriesArgumentOffset = 2d;

        Random Random { get; set; }

        public List<BoxPlotPoint> ExperimentResults1 { get; private set; }
        public List<BoxPlotPoint> ExperimentResults2 { get; private set; }
        public List<PointData> CurrentExperimentRandomValues1 { get; private set; }
        public List<PointData> CurrentExperimentRandomValues2 { get; private set; }
        public int ExperimentNumber { get; private set; }

        public BoxPlotData()
        {
            ExperimentNumber = 0;
            Random = new Random(3);
            CurrentExperimentRandomValues1 = new List<PointData>(RandomValuesPerExperiment);
            CurrentExperimentRandomValues2 = new List<PointData>(RandomValuesPerExperiment);
            ExperimentResults1 = new List<BoxPlotPoint>(InitialExperimentCount);
            ExperimentResults2 = new List<BoxPlotPoint>(InitialExperimentCount);
            GenerateInitialResults();
        }

        void GenerateInitialResults()
        {
            for (int i = InitialExperimentCount - 1; i > 0; i--)
            {
                List<double> randomValues1 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
                List<double> randomValues2 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
                ExperimentNumber++;
                BoxPlotPoint point1 = new BoxPlotPoint(ExperimentNumber, randomValues1);
                BoxPlotPoint point2 = new BoxPlotPoint(ExperimentNumber, randomValues2);
                ExperimentResults1.Add(point1);
                ExperimentResults2.Add(point2);
            }
            List<double> lastRandomValues1 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
            List<double> lastRandomValues2 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
            for (int i = 0; i < RandomValuesPerExperiment; i++)
            {
                CurrentExperimentRandomValues1.Add(new PointData(lastRandomValues1[i], Random));
                CurrentExperimentRandomValues2.Add(new PointData(lastRandomValues2[i], Random, SecondPointSeriesArgumentOffset));
            }
            ExperimentNumber++;
            ExperimentResults1.Add(new BoxPlotPoint(ExperimentNumber, lastRandomValues1));
            ExperimentResults2.Add(new BoxPlotPoint(ExperimentNumber, lastRandomValues2));
        }
        public void AddNewDataSet()
        {
            ExperimentResults1.RemoveAt(0);
            ExperimentResults2.RemoveAt(0);
            List<double> randomValues1 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
            List<double> randomValues2 = RandomSequenceGenerator.GenerateRandomSequence(Random, RandomValuesPerExperiment);
            CurrentExperimentRandomValues1.Clear();
            CurrentExperimentRandomValues2.Clear();
            for (int i = 0; i < RandomValuesPerExperiment; i++)
            {
                CurrentExperimentRandomValues1.Add(new PointData(randomValues1[i], Random));
                CurrentExperimentRandomValues2.Add(new PointData(randomValues2[i], Random, SecondPointSeriesArgumentOffset));
            }
            ExperimentNumber++;
            ExperimentResults1.Add(new BoxPlotPoint(ExperimentNumber, randomValues1));
            ExperimentResults2.Add(new BoxPlotPoint(ExperimentNumber, randomValues2));
        }
    }

    public class PointData
    {
        public double Argument { get; private set; }
        public double Value { get; private set; }

        public PointData(double val, Random rnd, double argumentOffset = 0)
        {
            Argument = rnd.NextDouble() + argumentOffset;
            Value = val;
        }
    }


    public class BoxPlotPoint
    {
        public int ExperimentNumber { get; private set; }
        public double Min { get; private set; }
        public double Quartile1 { get; private set; }
        public double Median { get; private set; }
        public double Quartile3 { get; private set; }
        public double Max { get; private set; }
        public double Mean { get; private set; }
        public List<double> Outliers { get; private set; }

        public BoxPlotPoint(int number, List<double> randomSequence)
        {
            ExperimentNumber = number;
            Tuple<double, double, double> quartiles = CalculateQuartiles(randomSequence);
            Quartile1 = quartiles.Item1;
            Quartile3 = quartiles.Item3;
            Median = quartiles.Item2;
            Tuple<double, double, double> averageAndMinMax = CalculateAverageAndMinMax(randomSequence);
            Mean = averageAndMinMax.Item1;
            Min = Math.Max(averageAndMinMax.Item2, Quartile1 - 1.5 * (Quartile3 - Quartile1));
            Max = Math.Min(averageAndMinMax.Item3, Quartile3 + 1.5 * (Quartile3 - Quartile1));
            Outliers = randomSequence.Where(d => d > Max || d < Min).ToList();
        }

        Tuple<double, double, double> CalculateAverageAndMinMax(List<double> randomSequence)
        {
            double average = 0;
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (double d in randomSequence)
            {
                average += d;
                if (d > max)
                    max = d;
                if (d < min)
                    min = d;
            }
            average = average / randomSequence.Count;
            return new Tuple<double, double, double>(average, min, max);
        }
        Tuple<double, double, double> CalculateQuartiles(List<double> randomSequence)
        {
            randomSequence.Sort();
            int middleIndex = (int)(randomSequence.Count / 2); //for correct vb conversion
            double quartile1 = 0;
            double quartile2;
            double quartile3 = 0;
            if (randomSequence.Count % 2 == 0)
            {
                quartile2 = (randomSequence[middleIndex - 1] + randomSequence[middleIndex]) / 2;
                int middleIndexOfHalf = middleIndex / 2;
                if (middleIndex % 2 == 0)
                {
                    quartile1 = (randomSequence[middleIndexOfHalf - 1] + randomSequence[middleIndexOfHalf]) / 2;
                    quartile3 = (randomSequence[middleIndex + middleIndexOfHalf - 1] + randomSequence[middleIndex + middleIndexOfHalf]) / 2;
                }
                else
                {
                    quartile1 = randomSequence[middleIndexOfHalf];
                    quartile3 = randomSequence[middleIndexOfHalf + middleIndex];
                }
            }
            else if (randomSequence.Count == 1)
            {
                quartile1 = randomSequence[0];
                quartile2 = randomSequence[0];
                quartile3 = randomSequence[0];
            }
            else
            {
                quartile2 = randomSequence[middleIndex];
                if ((randomSequence.Count - 1) % 4 == 0)
                {
                    int quarterIndex = (int)((randomSequence.Count - 1) / 4); //for correct vb conversion
                    quartile1 = (randomSequence[quarterIndex - 1] * .25) + (randomSequence[quarterIndex] * .75);
                    quartile3 = (randomSequence[3 * quarterIndex] * .75) + (randomSequence[3 * quarterIndex + 1] * .25);
                }
                else if ((randomSequence.Count - 3) % 4 == 0)
                {
                    int quarterIndex = (int)((randomSequence.Count - 3) / 4); //for correct vb conversion;
                    quartile1 = (randomSequence[quarterIndex] * .75) + (randomSequence[quarterIndex + 1] * .25);
                    quartile3 = (randomSequence[3 * quarterIndex + 1] * .25) + (randomSequence[3 * quarterIndex + 2] * .75);
                }
            }
            return new Tuple<double, double, double>(quartile1, quartile2, quartile3);
        }
    }


    static class RandomSequenceGenerator
    {
        public static List<double> GenerateRandomSequence(Random random, int length)
        {
            double selector = random.NextDouble();
            if (selector < 0.33)
                return GenerateExponentialDistribution(random, length);
            if (selector < 0.66)
                return GenerateSpecialDistribution(random, length);
            else
                return GenerateNormalDistribution(random, length);
        }

        static List<double> GenerateNormalDistribution(Random random, int length)
        {
            List<double> list = new List<double>(length);
            //Box-Muller transform
            double mean = random.Next(450, 550);
            double stdDev = random.Next(50, 70);
            for (int i = 0; i < length; i++)
            {
                double u1 = 1.0 - random.NextDouble();
                double u2 = 1.0 - random.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                list.Add(mean + stdDev * randStdNormal);
            }
            return list;
        }
        static List<double> GenerateExponentialDistribution(Random random, int length)
        {
            List<double> list = new List<double>(length);
            double minVal = random.Next(250, 300);
            double maxVal = minVal + 300;
            int generatedCount = 0;
            double lambda = random.NextDouble() * 2;
            while (generatedCount < length)
            {
                double u = random.NextDouble();
                double t = -Math.Log(u) / lambda;
                double increment = (maxVal - minVal) / 6.0;
                double result = minVal + (t * increment);
                if (result < maxVal)
                {
                    list.Add(result);
                    generatedCount++;
                }
            }
            return list;
        }
        static List<double> GenerateSpecialDistribution(Random random, int length)
        {
            List<double> list = new List<double>(length);
            int min = random.Next(100, 250);
            int step = random.Next(30, 70);
            for (int i = 0; i < (int)(length * 0.05); i++)
                list.Add(random.Next(min, min + step));
            for (int i = 0; i < (int)(length * 0.025); i++)
                list.Add(random.Next(min + step + 1, min + 2 * step));
            for (int i = 0; i < (int)(length * 0.075); i++)
                list.Add(random.Next(min + 2 * step + 1, min + 3 * step));
            for (int i = 0; i < (int)(length * 0.10); i++)
                list.Add(random.Next(min + 3 * step + 1, min + 4 * step));
            for (int i = 0; i < (int)(length * 0.20); i++)
                list.Add(random.Next(min + 4 * step + 1, min + 5 * step));
            for (int i = 0; i < (int)(length * 0.30); i++)
                list.Add(random.Next(min + 5 * step + 1, min + 6 * step));
            for (int i = 0; i < (int)(length * 0.20); i++)
                list.Add(random.Next(min + 6 * step + 1, min + 7 * step));
            for (int i = 0; i < (int)(length * 0.05) + 1; i++)
                list.Add(random.Next(min + 7 * step + 1, min + 8 * step));
            return list;
        }

    }
}