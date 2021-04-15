using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DesktopApp
{
    public class AnomalyData : INotifyPropertyChanged
    {
        private TimeSeries _test; //the test TimeSeries fight file to run.
        private readonly SimpleAnomalyDetector _detector; //the AnomalyDetector that learn the flight.
        private List<DataPoint> _currentDataPoints; //list of the current column Points.

        public List<DataPoint> _CurrentDataPoints
        {
            get => _currentDataPoints;
            private set
            {
                _currentDataPoints = value;
                NotifyPropertyChanged("_CurrentDataPoints");
            }
        }

        private List<DataPoint> _correlatedDataPoints; //list of the most correlated column Points.

        public List<DataPoint> _CorrelatedDataPoints
        {
            get => _correlatedDataPoints;
            private set
            {
                _correlatedDataPoints = value;
                NotifyPropertyChanged("_CorrelatedDataPoints");
            }
        }

        private List<DataPoint> _multiDataPoints; //list of the current column and the most correlative column Points.

        public List<DataPoint> _MultiDataPoints
        {
            get => _multiDataPoints;
            private set
            {
                _multiDataPoints = value;
                NotifyPropertyChanged("_MultiDataPoints");
            }
        }

        private List<DataPoint>
            _lastMultiDataPoints; //list of the 10 last current column and the most correlative column Points.

        public List<DataPoint> _LastMultiDataPoints
        {
            get => _lastMultiDataPoints;
            private set
            {
                _lastMultiDataPoints = value;
                NotifyPropertyChanged("_LastMultiDataPoints");
            }
        }

        private List<DataPoint> _linearReg; //list of 2 points declaring the regression line.

        public List<DataPoint> _LinearReg
        {
            get => _linearReg;
            private set
            {
                _linearReg = value;
                NotifyPropertyChanged("_LinearReg");
            }
        }

        private string _currColumn; //the current selected column.

        public string _CurrColumn
        {
            get => _currColumn;
            set
            {
                _currColumn = value;
                _CorrelatedColumn = MostCorrelative(value);
                UpdateCurrList();
                UpdateMultiList();
                UpdateLine();
                if (_dllPath != "")
                    Draw();
                NotifyPropertyChanged("_CurrColumn");
            }
        }

        private string _correlatedColumn; //the current correlated column.

        public string _CorrelatedColumn
        {
            get => _correlatedColumn;
            set
            {
                _correlatedColumn = value;
                UpdateCorrelatedList();
                NotifyPropertyChanged("_CorrelatedColumn");
            }
        }

        private int _currIndex; //the line number that currently being sent.

        public int _NumLine
        {
            get => _currIndex;
            set
            {
                _currIndex = value;
                UpdateCurrList();
                UpdateCorrelatedList();
                UpdateMultiList();
                UpdateLine();
                UpdateJoystickCurrFeatures();
                NotifyPropertyChanged("_NumLine");
            }
        }

        private List<string> _allFeatures; //list of all the XML features.

        public List<string> _AllFeatures
        {
            get => _allFeatures;
            private set
            {
                _allFeatures = new List<string>(value);
                NotifyPropertyChanged("_AllFeatures");
            }
        }

        private float _minMulti; //the minimum X-value between all the _multiDataPoints.
        private float _maxMulti; //the maximum X-value between all the _multiDataPoints.

        public event PropertyChangedEventHandler PropertyChanged;

        //Constructor
        public AnomalyData(SimpleAnomalyDetector detector)
        {
            _detector = detector;
            _currentDataPoints = new List<DataPoint>();
            _currColumn = "";
            _correlatedDataPoints = new List<DataPoint>();
            _correlatedColumn = "";
            _multiDataPoints = new List<DataPoint>();
            _lastMultiDataPoints = new List<DataPoint>();
            _linearReg = new List<DataPoint>();
            _dllPath = "";
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        //puts all points up to the last point sent in _CurrentDataPoints.
        private void UpdateCurrList()
        {
            List<DataPoint> tmp = new List<DataPoint>();
            for (int i = 0; i <= _currIndex; i++)
            {
                tmp.Add(new DataPoint(i, GetTestAt(_currColumn, i)));
            }

            _CurrentDataPoints = tmp;
        }

        //puts all points up to the last point sent in _CorrelatedDataPoints.
        private void UpdateCorrelatedList()
        {
            var tmp = new List<DataPoint>();
            for (var i = 0; i < _currIndex; i++)
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
        private void UpdateMultiList()
        {
            var tmp = new List<DataPoint>();
            var tmp2 = new List<DataPoint>();
            double minX = float.MaxValue;
            double maxX = float.MinValue;
            for (var i = Math.Max(0, _currIndex - 300); i < _currIndex; i++)
            {
                tmp.Add(new DataPoint(GetTestAt(_currColumn, i), GetTestAt(_correlatedColumn, i)));
                if (GetTestAt(_currColumn, i) > maxX)
                    maxX = GetTestAt(_currColumn, i);
                if (GetTestAt(_currColumn, i) < minX)
                    minX = GetTestAt(_currColumn, i);
            }

            for (var i = Math.Max(0, _currIndex - 10); i <= _currIndex; i++)
            {
                tmp2.Add(new DataPoint(GetTestAt(_currColumn, i), GetTestAt(_correlatedColumn, i)));
            }

            _minMulti = (float) minX;
            _maxMulti = (float) maxX;
            _MultiDataPoints = tmp;
            _LastMultiDataPoints = tmp2;
        }

        //update the _LinearReg.
        private void UpdateLine()
        {
            var tmp = new List<DataPoint>
            {
                new DataPoint(_minMulti, LinearReg(_currColumn).F(_minMulti)),
                new DataPoint(_maxMulti, LinearReg(_currColumn).F(_maxMulti))
            };
            _LinearReg = tmp;
        }

        //learn the _test TimeSeries flight file.
        public void Learn()
        {
            _detector.LearnNormal(_test);
            if (_dllPath == "") return;
            Start();
            Draw();
        }

        //return the _test TimeSeries column value at index.
        private float GetTestAt(string column, int index)
        {
            if (_test?.GetColumn(column) == null) return 0;
            try
            {
                return _test.GetColumn(column)[index];
            }
            catch (Exception)
            {
                // ignored
            }

            return 0;
        }

        //return the most correlative feature to the column.
        private string MostCorrelative(string column)
        {
            var cf = _detector.GetNormalModel();
            foreach (var corr in cf.Where(corr => corr.Feature1 == column))
            {
                return corr.Feature2;
            }

            return "";
        }

        //return the regression line of the column (with the most correlative column).
        private Line LinearReg(string column)
        {
            var cf = _detector.GetNormalModel();
            foreach (var c in cf.Where(c => c.Feature1 == column))
            {
                return c.LinReg;
            }

            return new Line();
        }

        //return list of all the Points of the column (with the most correlative column).
        public List<Point> GetAllPoints(string column)
        {
            var cf = _detector.GetNormalModel();
            foreach (var c in cf.Where(c => c.Feature1 == column))
            {
                return c.AllPoints;
            }

            return new List<Point>();
        }

        //analyzes the XML file and initializes the _AllFeatures
        public void AnalyzeXml(string path)
        {
            _AllFeatures = XmlAnalyzer.Analyzer(path);
        }

        //Joystick:
        // Joystick features
        private float _aileron;
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
            get => _throttle;
            set
            {
                _throttle = value;
                NotifyPropertyChanged("_Throttle");
            }
        }

        public float _Rudder
        {
            get => _rudder;
            set
            {
                _rudder = value;
                NotifyPropertyChanged("_Rudder");
            }
        }

        public float _Elevator
        {
            get => _elevator;
            set
            {
                _elevator = value;
                NotifyPropertyChanged("_Elevator");
            }
        }

        public float _Aileron
        {
            get => _aileron;
            set
            {
                _aileron = value;
                NotifyPropertyChanged("_Aileron");
            }
        }

        public float _AirSpeed
        {
            get => _airSpeed;
            set
            {
                _airSpeed = value;
                NotifyPropertyChanged("_AirSpeed");
            }
        }

        public float _Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                NotifyPropertyChanged("_Roll");
            }
        }

        public float _Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                NotifyPropertyChanged("_Pitch");
            }
        }

        public float _Alimeter
        {
            get => _alimeter;
            set
            {
                _alimeter = value;
                NotifyPropertyChanged("_Alimeter");
            }
        }

        public float _Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                NotifyPropertyChanged("_Yaw");
            }
        }

        public float _Direction
        {
            get => this._direction;
            set
            {
                this._direction = value;
                NotifyPropertyChanged("_Direction");
            }
        }

        // lists for Joystick feature 
        private List<float> _aileronList;
        private List<float> _elevatorList;
        private List<float> _rudderList;
        private List<float> _throttleList;
        private List<float> _yawList;
        private List<float> _rollList;
        private List<float> _pitchList;
        private List<float> _airSpeedList;
        private List<float> _alimeterList;

        private void UpdateJoystickCurrFeatures()
        {
            try
            {
                if (_aileronList != null)
                    _Aileron = _aileronList[_NumLine];
                else
                {
                    _Aileron = 125;
                }

                if (_elevatorList != null)
                    _Elevator = _elevatorList[_NumLine];
                else
                {
                    _Elevator = 125;
                }

                if (_rudderList != null)
                    _Rudder = _rudderList[_NumLine];
                else
                {
                    _Rudder = -1;
                }

                if (_throttleList != null)
                    _Throttle = _throttleList[_NumLine];
                else
                {
                    _Throttle = 0;
                }


                if (_airSpeedList != null)
                    _AirSpeed = _airSpeedList[_NumLine];
                else
                {
                    _AirSpeed = 0;
                }

                if (_yawList != null)
                {
                    _Yaw = _yawList[_NumLine];
                    _Direction = _yawList[_NumLine];
                }
                else
                {
                    _Yaw = 0;
                    _Direction = 0;
                }

                if (_pitchList != null)
                    _Pitch = _pitchList[_NumLine];
                else
                {
                    _Pitch = 0;
                }

                if (_rollList != null)
                    _Roll = _rollList[_NumLine];
                else
                {
                    _Roll = 0;
                }

                if (_alimeterList != null)
                    _Alimeter = _alimeterList[_NumLine];
                else
                {
                    _Alimeter = 0;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UpdateJoystickFeatures()
        {
            _aileronList = GetCol("aileron");
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

        private List<float> GetCol(string s)
        {
            return _test?.GetColumn(s);
        }

        //DLL part:
        private string _dllPath; //the dll path.
        private string _trainFile; //the train csv file.
        private string _testFile; //the test csv file.
        private object _dllAnomalyManager; //the DLL's AnomalyManager Object.
        private MethodInfo _shapeMethod; //the method that returns the PlotModel
        private MethodInfo _corrMethod; //the method that returns the most correlative according to the DLL.
        private MethodInfo _anomaliesMethod; //the method that returns List of the anomalies.
        private string _dllTitle; //the Title above the DLL's graphs.

        public string _DllTitle
        {
            get => _dllTitle;
            private set
            {
                _dllTitle = value;
                NotifyPropertyChanged("_DllTitle");
            }
        }

        private PlotModel _dllPlotModel; //the DLL's PlotModel.

        public PlotModel _DllPlotModel
        {
            get => _dllPlotModel;
            private set
            {
                _dllPlotModel = value;
                NotifyPropertyChanged("_DllPlotModel");
            }
        }

        private List<int> _dllAnomaliesLines; //list of the lines of the anomalies.
        private List<string> _dllAnomaliesList; //list of the strings of the anomalies.

        public List<string> _DllAnomaliesList
        {
            get => _dllAnomaliesList;
            private set
            {
                _dllAnomaliesList = value;
                NotifyPropertyChanged("_DllAnomaliesList");
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

            var @namespace = _dllPath.Split('\\')[^1].Split('.')[0];

            var type = assembly.GetType(@namespace + ".AnomalyManager");
            _dllAnomalyManager = Activator.CreateInstance(type!);

            var uploadTrain = type.GetMethod("UploadTrain");
            uploadTrain?.Invoke(_dllAnomalyManager, new object[] {_trainFile});
            var uploadTest = type.GetMethod("UploadTest");
            uploadTest?.Invoke(_dllAnomalyManager, new object[] {_testFile});

            var learn = type.GetMethod("Learn");
            learn?.Invoke(_dllAnomalyManager, new object[] { });

            var detect = type.GetMethod("Detect");
            detect?.Invoke(_dllAnomalyManager, new object[] { });

            _shapeMethod = type.GetMethod("GetShape");
            _corrMethod = type.GetMethod("GetCorrelated");
            _anomaliesMethod = type.GetMethod("GetAnomalies");
        }

        //set the _DllPlotModel, the _DllTitle and the _DllAnomaliesList.
        public void Draw()
        {
            //PlotModel tmp = (PlotModel)_shapeMethod.Invoke(_detector, new object[] { _currColumn });
            _DllPlotModel = null;
            GC.Collect();
            _DllPlotModel = (PlotModel) _shapeMethod.Invoke(_dllAnomalyManager, new object[] {_currColumn});
            _DllTitle = "\"" + _currColumn + "\" is most correlative to: \"" +
                        (string) _corrMethod.Invoke(_dllAnomalyManager, new object[] {_currColumn}) +
                        "\"\n according to the train flight";

            var tuple = ((Tuple<List<string>, List<int>>) _anomaliesMethod.Invoke(_dllAnomalyManager,
                new object[] {_currColumn}));
            _DllAnomaliesList = tuple?.Item1;
            _dllAnomaliesLines = tuple?.Item2;
        }

        //return the line of the anomaly string.
        public int GetTimeStepFromString(string anomaly)
        {
            return _dllAnomaliesLines[_dllAnomaliesList.IndexOf(anomaly)];
        }
    }
}