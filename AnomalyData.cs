using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DesktopApp
{
    public class AnomalyData : INotifyPropertyChanged
    {
        private TimeSeries _test; //the test TimeSeries fight file to run.
        private SimpleAnomalyDetector detector; //the AnomalyDetector that learn the flight.
        private List<DataPoint> _currentDataPoints; //list of the current column Points.
        public List<DataPoint> _CurrentDataPoints
        {
            get
            {
                return _currentDataPoints;
            }
            set
            {
                _currentDataPoints = value;
                NotifyPropertyChanged("_CurrentDataPoints");
            }
        }
        private List<DataPoint> _correlatedDataPoints; //list of the most correlated column Points.
        public List<DataPoint> _CorrelatedDataPoints
        {
            get
            {
                return _correlatedDataPoints;
            }
            set
            {
                _correlatedDataPoints = value;
                NotifyPropertyChanged("_CorrelatedDataPoints");
            }
        }
        private List<DataPoint> _multiDataPoints; //list of the current column and the most correlative column Points.
        public List<DataPoint> _MultiDataPoints
        {
            get
            {
                return _multiDataPoints;
            }
            set
            {
                _multiDataPoints = value;
                NotifyPropertyChanged("_MultiDataPoints");
            }
        }
        private List<DataPoint> _lastMultiDataPoints; //list of the 10 last current column and the most correlative column Points.
        public List<DataPoint> _LastMultiDataPoints
        {
            get
            {
                return _lastMultiDataPoints;
            }
            set
            {
                _lastMultiDataPoints = value;
                NotifyPropertyChanged("_LastMultiDataPoints");
            }
        }
        private List<DataPoint> _linearReg; //list of 2 points declaring the regression line.
        public List<DataPoint> _LinearReg
        {
            get
            {
                return _linearReg;
            }
            set
            {
                _linearReg = value;
                NotifyPropertyChanged("_LinearReg");
            }
        }
        private string _currColumn; //the current selected column.
        public string _Curr_column
        {
            get { return _currColumn; }
            set
            {
                _currColumn = value;
                _Correlated_column = MostCorrelative(value);
                UpdateCurrList();
                updateMultiList();
                UpdateLine();
                if (_dllPath != "")
                    Draw();
                NotifyPropertyChanged("_Curr_Column");
            }
        }
        private string _correlatedColumn; //the current correlated column.
        public string _Correlated_column
        {
            get { return _correlatedColumn; }
            set
            {
                _correlatedColumn = value;
                UpdateCorrelatedList();
                NotifyPropertyChanged("_Correlated_column");
            }
        }
        private int _currIndex; //the line number that currently being sent.
        public int _Num_line
        {
            get { return _currIndex; }
            set
            {
                _currIndex = value;
                UpdateCurrList();
                UpdateCorrelatedList();
                updateMultiList();
                UpdateLine();
                UpdateJoystickCurrFeatures();
                NotifyPropertyChanged("_Num_line");
            }
        }
        private List<string> allFeatures; //list of all the XML features.
        public List<string> _AllFeatures
        {
            get
            {
                return allFeatures;
            }
            set
            {
                allFeatures = new List<string>(value);
                NotifyPropertyChanged("_AllFeatures");
            }
        }
        private float minMulti; //the minimum X-value between all the _multiDataPoints.
        private float maxMulti; //the maximum X-value between all the _multiDataPoints.

        public event PropertyChangedEventHandler PropertyChanged;
        //Constructor
        public AnomalyData(SimpleAnomalyDetector detector)
        {
            this.detector = detector;
            _currentDataPoints = new List<DataPoint>();
            _currColumn = "";
            _correlatedDataPoints = new List<DataPoint>();
            _correlatedColumn = "";
            _multiDataPoints = new List<DataPoint>();
            _lastMultiDataPoints = new List<DataPoint>();
            _linearReg = new List<DataPoint>();
            _dllPath = "";
        }
        
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        //puts all points up to the last point sent in _CurrentDataPoints.
        public void UpdateCurrList()
        {
            List<DataPoint> tmp = new List<DataPoint>();
            for(int i = 0; i <= _currIndex; i++)
            {
                tmp.Add(new DataPoint(i, GetTestAt(_currColumn, i)));
            }
            _CurrentDataPoints = tmp;
        }
        //puts all points up to the last point sent in _CorrelatedDataPoints.
        public void UpdateCorrelatedList()
        {
            List<DataPoint> tmp = new List<DataPoint>();
            for (int i = 0; i < _currIndex; i++)
            {
                tmp.Add(new DataPoint(i, GetTestAt(_correlatedColumn, i)));
            }
            _CorrelatedDataPoints = tmp;
        }
        /*
         * puts all points up to the last point sent in _MultiDataPoints.
         * put the last 10 Points in _LastMultiDataPoints.
         * initializes the minMulti and the maxMulti values.
         */
        public void updateMultiList()
        {
            List<DataPoint> tmp = new List<DataPoint>();
            List<DataPoint> tmp2 = new List<DataPoint>();
            double minX = float.MaxValue;
            double maxX = float.MinValue;
            for (int i =Math.Max(0,_currIndex-300); i < _currIndex; i++)
            {
                tmp.Add(new DataPoint(GetTestAt(_currColumn, i), GetTestAt(_correlatedColumn, i)));
                if (GetTestAt(_currColumn, i) > maxX)
                    maxX = GetTestAt(_currColumn, i);
                if (GetTestAt(_currColumn, i) < minX)
                    minX = GetTestAt(_currColumn, i);
            }
            for (int i = Math.Max(0, _currIndex - 10); i <= _currIndex; i++)
            {
                tmp2.Add(new DataPoint(GetTestAt(_currColumn, i), GetTestAt(_correlatedColumn, i)));
            }
            minMulti = (float)minX;
            maxMulti = (float)maxX;
            _MultiDataPoints = tmp;
            _LastMultiDataPoints = tmp2;
        }
        //update the _LinearReg.
        public void UpdateLine()
        {
            List<DataPoint> tmp = new List<DataPoint>();
            tmp.Add(new DataPoint(minMulti, LinearReg(_currColumn, _Correlated_column).f((float)minMulti)));
            tmp.Add(new DataPoint(maxMulti, LinearReg(_currColumn, _Correlated_column).f((float)maxMulti)));
            _LinearReg = tmp;
        }
        //learn the _test TimeSeries flight file.
        public void Learn()
        {
            detector.LearnNormal(_test);
            if (_dllPath != "")
            {
                Start();
                Draw();
            }
        }
        //return the _test TimeSeries column value at index.
        public float GetTestAt(string column, int index)
        {
            if (_test != null && _test.GetColumn(column) != null)
            {
                try { return _test.GetColumn(column)[index]; }
                catch (Exception)
                {

                }
            }
            return 0;
        }
        //return the most correlative feature to the column.
        public string MostCorrelative(string column)
        {
            List<CorrelatedFeatures> cf = detector.GetNormalModel();
            foreach (CorrelatedFeatures corr in cf)
            {
                if (corr.Feature1 == column)
                {
                    return corr.Feature2;
                }
            }
            return "";
        }
        //return the regression line of the column (with the most correlative column).
        public Line LinearReg(string column, string corr)
        {
            List<CorrelatedFeatures> cf = detector.GetNormalModel();
            foreach (CorrelatedFeatures c in cf)
            {
                if (c.Feature1 == column)
                {
                    return c.LinReg;
                }
            }
            return new Line();
        }
        //return list of all the Points of the column (with the most correlative column).
        public List<Point> GetAllPoints(string column, string corr)
        {
            List<CorrelatedFeatures> cf = detector.GetNormalModel();
            foreach (CorrelatedFeatures c in cf)
            {
                if (c.Feature1 == column)
                {
                    return c.AllPoints;
                }
            }

            return new List<Point>();
        }
        //analyzes the XML file and initializes the _AllFeatures
        public void AnalyzeXML(string path)
        {
           _AllFeatures = XmlAnalyzer.Analyzer(path);
        }

        //Joystick:
        // Joystick features
        private float _aieleron;
        private float _elevator;
        private float _rudder;
        private float _throttle;
        private float _yaw;
        private float _roll;
        private float _pitch;
        private float _airSpeed;
        private float _alimeter;
        private float _direction;

        public float _Throttle
        {
            get { return this._throttle; }
            set
            {
                this._throttle = value;
                NotifyPropertyChanged("_Throttle");
            }
        }
        public float _Rudder
        {
            get { return this._rudder; }
            set
            {
                this._rudder = value;
                NotifyPropertyChanged("_Rudder");
            }
        }
        public float _Elevator
        {
            get { return this._elevator; }
            set
            {
                this._elevator = value;
                NotifyPropertyChanged("_Elevator");
            }
        }
        public float _Aileron
        {
            get { return this._aieleron; }
            set
            {
                this._aieleron = value;
                NotifyPropertyChanged("_Aileron");
            }
        }
        public float _AirSpeed
        {
            get { return this._airSpeed; }
            set
            {
                this._airSpeed = value;
                NotifyPropertyChanged("_AirSpeed");
            }
        }
        public float _Roll
        {
            get { return this._roll; }
            set
            {
                this._roll = value;
                NotifyPropertyChanged("_Roll");
            }
        }
        public float _Pitch
        {
            get { return this._pitch; }
            set
            {
                this._pitch = value;
                NotifyPropertyChanged("_Pitch");
            }
        }
        public float _Alimeter
        {
            get { return this._alimeter; }
            set
            {
                this._alimeter = value;
                NotifyPropertyChanged("_Alimeter");
            }
        }
        public float _Yaw
        {
            get { return this._yaw; }
            set
            {
                this._yaw = value;
                NotifyPropertyChanged("_Yaw");
            }
        }
        public float _Direction
        {
            get { return this._direction; }
            set
            {
                this._direction = value;
                NotifyPropertyChanged("_Direction");
            }
        }

        // lists for Joystick feature 
        private List<float> _aieleronList;
        private List<float> _elevatorList;
        private List<float> _rudderList;
        private List<float> _throttleList;
        private List<float> _yawList;
        private List<float> _rollList;
        private List<float> _pitchList;
        private List<float> _airSpeedList;
        private List<float> _alimeterList;

        public void UpdateJoystickCurrFeatures()
        {
            try
            {
                if (_aieleronList != null)
                    _Aileron = _aieleronList[_Num_line];
                else
                {
                    _Aileron = 125;
                }
                if (_elevatorList != null)
                    _Elevator = _elevatorList[_Num_line];
                else
                {
                    _Elevator = 125;
                }
                if (_rudderList != null)
                    _Rudder = _rudderList[_Num_line];
                else
                {
                    _Rudder = -1;
                }
                if (_throttleList != null)
                    _Throttle = _throttleList[_Num_line];
                else
                {
                    _Throttle = 0;
                }


                if (_airSpeedList != null)
                    _AirSpeed = _airSpeedList[_Num_line];
                else
                {
                    _AirSpeed = 0;
                }
                if (_yawList != null)
                {
                    _Yaw = _yawList[_Num_line];
                    _Direction = _yawList[_Num_line];
                }
                else
                {
                    _Yaw = 0;
                    _Direction = 0;
                }
                if (_pitchList != null)
                    _Pitch = _pitchList[_Num_line];
                else
                {
                    _Pitch = 0;
                }
                if (_rollList != null)
                    _Roll = _rollList[_Num_line];
                else
                {
                    _Roll = 0;
                }
                if (_alimeterList != null)
                    _Alimeter = _alimeterList[_Num_line];
                else
                {
                    _Alimeter = 0;
                }
            }
            catch (Exception)
            {

            }
        }
        public void UpdateJoystickFeatures()
        {
            _aieleronList = GetCol("aileron");
            _elevatorList = GetCol("elevator");
            _rudderList = GetCol("rudder");
            _throttleList = GetCol("throttle1");
            _airSpeedList = GetCol("airspeed-indicator_indicated-speed-kt");
            _alimeterList = GetCol("altimeter_indicated-altitude-ft");
            //_directionList = GetCol("heading-deg");
            _yawList = GetCol("heading-deg");
            _pitchList = GetCol("pitch-deg");
            _rollList = GetCol("roll-deg");
        }

        public List<float> GetCol(string s)
        {
            if (_test != null)
            {
                return _test.GetColumn(s);
            }
            return null;
        }
        //DLL part:
        private string _dllPath; //the dll path.
        private string _trainFile; //the train csv file.
        private string _testFile; //the test csv file.
        private object _detector; //the DLL's AnomalyManager Object.
        private MethodInfo _shapeMethod; //the method that returns the PlotModel
        private MethodInfo _corrMethod; //the method that returns the most correlative according to the DLL.
        private MethodInfo _anomaliesMethod; //the method that returns List of the anomalies.
        private string _dllTitle; //the Title above the DLL's graphs.
        public string _DllTitle
        {
            get
            {
                return _dllTitle;
            }
            set
            {
                _dllTitle = value;
                NotifyPropertyChanged("_DllTitle");
            }
        }
        private PlotModel _dllPlotModel; //the DLL's PlotModel.
        public PlotModel _DllPlotModel
        {
            get
            {
                return _dllPlotModel;
            }
            set
            {
                _dllPlotModel = value;
                NotifyPropertyChanged("_DllPlotModel");
            }
        }
        private List<int> _dllAnomaiesLines; //list of the lines of the anomalies.
        private List<string> _dllAnomaiesList; //list of the strings of the anomalies.
        public List<string> _DllAnomaiesList
        {
            get
            {
                return _dllAnomaiesList;
            }
            set
            {
                _dllAnomaiesList = value;
                NotifyPropertyChanged("_DllAnomaiesList");
            }
        }
        //set the DLL's path.
        public void PathSetter(string path)
        {
            _dllPath = path;
        }
        //upload the train file.
        public void UploadTrain(string file)
        { 
            _trainFile = file;
        }
        //upload the test file and create the TimeSeries test.
        public void UploadTest(string file)
        { 
            _test = new TimeSeries(file);
            _testFile = file;
            UpdateJoystickFeatures();
        }
        //load th DLL, learn and detect the train and test files.
        public void Start()
        {
            var assembly = Assembly.LoadFile(_dllPath);

            string Namespace = _dllPath.Split('\\')[^1].Split('.')[0];

            var type = assembly.GetType(Namespace + ".AnomalyManager");
            _detector = Activator.CreateInstance(type);

            var uploadTrain = type.GetMethod("UploadTrain");
            uploadTrain.Invoke(_detector, new object[] { _trainFile });
            var uploadTest = type.GetMethod("UploadTest");
            uploadTest.Invoke(_detector, new object[] { _testFile });

            var learn = type.GetMethod("Learn");
            learn.Invoke(_detector, new object[] { });

            var detect = type.GetMethod("Detect");
            detect.Invoke(_detector, new object[] { });

            _shapeMethod = type.GetMethod("GetShape");
            _corrMethod = type.GetMethod("GetCorrelated");
            _anomaliesMethod = type.GetMethod("GetAnomalies");
        }
        //set the _DllPlotModel, the _DllTitle and the _DllAnomaiesList.
        public void Draw()
        {
            //PlotModel tmp = (PlotModel)_shapeMethod.Invoke(_detector, new object[] { _currColumn });
            _DllPlotModel = null;
            GC.Collect();
            _DllPlotModel = (PlotModel)_shapeMethod.Invoke(_detector, new object[] { _currColumn });
            _DllTitle = "\"" + _currColumn + "\" is most correlative to: \"" + (string)_corrMethod.Invoke(_detector, new object[] { _currColumn }) + "\"\n according to the train flight";

            Tuple<List<string>, List<int>> tuple = ((Tuple<List<string>, List<int>>)_anomaliesMethod.Invoke(_detector, new object[] { _currColumn }));
            _DllAnomaiesList = tuple.Item1;
            _dllAnomaiesLines = tuple.Item2;
        }
        //return the line of the anomaly string.
        public int GetTimeStepFromString(string anomaly)
        {
            return _dllAnomaiesLines[_dllAnomaiesList.IndexOf(anomaly)];
        }
    }
}