using System.Collections.Generic;

namespace DesktopApp
{
    //CorrelatedFeatures struct.
    public struct CorrelatedFeatures
    {
        public string Feature1, Feature2; // names of the correlated features
        public float Correlation;
        public readonly Line LinReg;
        public List<Point> AllPoints;

        public CorrelatedFeatures(string feature1, string feature2, float correlation, Line line)
            : this()
        {
            Feature1 = feature1;
            Feature2 = feature2;
            Correlation = correlation;
            LinReg = line;
            AllPoints ??= new List<Point>();
        }
    }

    //SimpleAnomalyDetector class
    public class SimpleAnomalyDetector
    {
        private readonly List<CorrelatedFeatures> _cf; //list of all correlated features
        private readonly float _threshold; //threshold determination correlation.        //Constructor

        public SimpleAnomalyDetector(float threshold)
        {
            _cf = new List<CorrelatedFeatures>();
            _threshold = threshold;
        }

        //return the CorrelatedFeatures List.
        public IEnumerable<CorrelatedFeatures> GetNormalModel()
        {
            return _cf;
        }

        //learn the train csv (TimeSeries).
        public void LearnNormal(TimeSeries ts)
        {
            CreateCf(ts);
            CreateForm(ts);
        }

        //create the CorrelatedFeatures List.
        private void CreateCf(TimeSeries ts)
        {
            var currentStruct = new CorrelatedFeatures();
            int i;
            for (i = 0; i < ts.GetColumnSize(); i++)
            {
                currentStruct.Feature1 = ts.GetFeatures()[i];
                currentStruct.Feature2 = "";
                var biggestPearson = _threshold;
                for (var j = 0; j < ts.GetColumnSize(); j++)
                {
                    if (j == i)
                        continue;
                    var absPearson = AnomalyDetectionUtil.Pearson(ts.GetColumn(currentStruct.Feature1).ToArray(),
                        ts.GetColumn(ts.GetFeatures()[j]).ToArray());
                    absPearson = absPearson > 0 ? absPearson : -absPearson;
                    if (!(absPearson >= biggestPearson)) continue;
                    biggestPearson = absPearson;
                    currentStruct.Feature2 = ts.GetFeatures()[j];
                    currentStruct.Correlation = biggestPearson;
                }

                if (currentStruct.Feature2 == "") continue;
                currentStruct.AllPoints = new List<Point>();
                _cf.Add(currentStruct);
            }
        }

        //calculating the Line for each element in the List.
        private void CreateForm(TimeSeries ts)
        {
            for (var i = 0; i < _cf.Count; i++)
            {
                var array = new Point[ts.GetRowSize()];
                for (var j = 0; j < ts.GetRowSize(); j++)
                {
                    array[j] = new Point(ts.GetColumn(_cf[i].Feature1)[j], ts.GetColumn(_cf[i].Feature2)[j]);
                    _cf[i].AllPoints.Add(array[j]);
                }

                var line = CreateCorrelativeForm(array);

                _cf[i] = new CorrelatedFeatures(_cf[i].Feature1, _cf[i].Feature2, _cf[i].Correlation, line);
                //float th = FindThreshold(array, Cf[i]);
                //Cf[i] = new CorrelatedFeatures(Cf[i].Feature1, Cf[i].Feature2, Cf[i].Correlation, th, line);
            }
        }

        // create linear regression from Point[].
        private static Line CreateCorrelativeForm(Point[] array)
        {
            return AnomalyDetectionUtil.LinearReg(array);
        }
    }
}