using System.Collections.Generic;

namespace DesktopApp
{
    //CorrelatedFeatures struct.
    public struct CorrelatedFeatures
    {
        public string Feature1, Feature2; // names of the correlated features
        public float Correlation;
        public Line LinReg;
        public float Threshold;
        public List<Point> AllPoints;
        public CorrelatedFeatures(string feature1, string feature2, float correlation, float threshold, Line line)
            : this()
        {
            Feature1 = feature1;
            Feature2 = feature2;
            Correlation = correlation;
            LinReg = line;
            Threshold = threshold;
            if (AllPoints == null)
                AllPoints = new List<Point>();
        }
    }
    //SimpleAnomalyDetector class
    public class SimpleAnomalyDetector
    {
        protected List<CorrelatedFeatures> Cf; //list of all correlated features
        public float Threshold; //threshold determination correlation.        //Constructor
        public SimpleAnomalyDetector(float threshold)
        {
            Cf = new List<CorrelatedFeatures>();
            Threshold = threshold;
        }
        //return the CorrelatedFeatures List.
        public List<CorrelatedFeatures> GetNormalModel()
        {
            return Cf;
        }
        //learn the train csv (TimeSeries).
        public void LearnNormal(TimeSeries ts)
        {
            CreateCf(ts);
            CreateForm(ts);
        }
        //create the CorrelatedFeatures List.
        public void CreateCf(TimeSeries ts)
        {
            float biggestPearson;
            CorrelatedFeatures currentStruct = new CorrelatedFeatures();
            int i;
            for (i = 0; i < ts.GetColumnSize(); i++)
            {
                currentStruct.Feature1 = ts.GetFeatures()[i];
                currentStruct.Feature2 = "";
                biggestPearson = Threshold; //minimum for correlation
                for (int j = 0; j < ts.GetColumnSize(); j++)
                {
                    if (j == i)
                        continue;
                    float absPearson = AnomalyDetectionUtil.Pearson(ts.GetColumn(currentStruct.Feature1).ToArray(),
                        ts.GetColumn(ts.GetFeatures()[j]).ToArray());
                    absPearson = absPearson > 0 ? absPearson : -absPearson;
                    if (absPearson >= biggestPearson)
                    {
                        biggestPearson = absPearson;
                        currentStruct.Feature2 = ts.GetFeatures()[j];
                        currentStruct.Correlation = biggestPearson;
                    }
                }

                if (currentStruct.Feature2 != "")
                {
                    currentStruct.AllPoints = new List<Point>();
                    Cf.Add(currentStruct);
                }
            }
            /* //for the last item
             i = ts.GetColumnSize() - 1;
             currentStruct.Feature1 = ts.GetFeatures()[i];
             currentStruct.Feature2 = "";
             biggestPearson = Threshold; //minimum for correlation
             for (int j = 0; j < i; j++)
             {
                 float absPearson = AnomalyDetectionUtil.Pearson(ts.GetColumn(currentStruct.Feature1).ToArray(),
                     ts.GetColumn(ts.GetFeatures()[j]).ToArray());
                 absPearson = absPearson > 0 ? absPearson : -absPearson;
                 if (absPearson >= biggestPearson)
                 {
                     biggestPearson = absPearson;
                     currentStruct.Feature2 = ts.GetFeatures()[j];
                     currentStruct.Correlation = biggestPearson;
                 }
             }

             if (currentStruct.Feature2 != "")
             {
                 currentStruct.AllPoints = new List<Point>();
                 Cf.Add(currentStruct);
             }*/
        }
        //calculating the Line for each element in the List.
        public void CreateForm(TimeSeries ts)
        {
            for (var i = 0; i < Cf.Count; i++)
            {
                Point[] array = new Point[ts.GetRowSize()];
                for (int j = 0; j < ts.GetRowSize(); j++)
                {
                    array[j] = new Point(ts.GetColumn(Cf[i].Feature1)[j], ts.GetColumn(Cf[i].Feature2)[j]);
                    Cf[i].AllPoints.Add(array[j]);
                }

                Line line = CreateCorrelativeForm(array);

                Cf[i] = new CorrelatedFeatures(Cf[i].Feature1, Cf[i].Feature2, Cf[i].Correlation, 0, line);
                //float th = FindThreshold(array, Cf[i]);
                //Cf[i] = new CorrelatedFeatures(Cf[i].Feature1, Cf[i].Feature2, Cf[i].Correlation, th, line);
            }
        }
        // create linear regression from Point[].
        public Line CreateCorrelativeForm(Point[] array)
        {
            return AnomalyDetectionUtil.LinearReg(array);
        }
        /*
        public float FindThreshold(Point[] array, CorrelatedFeatures s)
        {
            float max = 0;
            for (int j = 0; j < array.Length - 1; j++)
            {
                float val = devFromForm(array[j], s);
                if (max <= val)
                {
                    max = val;
                    //s.Threshold = max;
                }
            }

            return max;
        }

        public float devFromForm(Point p, CorrelatedFeatures s)
        {
            return AnomalyDetectionUtil.Dev(p, s.LinReg);
        }
        */
    }
}