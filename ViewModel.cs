using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;

namespace DesktopApp
{
    //ViewModel class
    public class ViewModel : INotifyPropertyChanged
    {
        private AnomalyData _data; //AnomalyData Model.
        private Panel _panel; //Panel Model.
        private string _trainPath; //the train csv file path.
        private string _testPath; //the test csv file path.
        private string _trainFile; //the entire train file
        private string _testFile; //the entire test file
        private Thread _th;
        //the Title above the DLL's graphs.
        public string VM_DllTitle
        {
            get
            {
                return _data._DllTitle;
            }
        }
        //the DLL's PlotModel.
        public PlotModel VM_DllPlotModel
        {
            get
            {
                return _data._DllPlotModel;
            }
        }
        //list of all the XML features.
        public List<string> VM_AllFeatures
        {
            get
            {
                return _data._AllFeatures;
            }
        }
        //the current selected column.
        public string VM_Curr_Column
        {
            get
            {
                return _data._Curr_column;
            }
            set
            {
                _data._Curr_column = value;
            }
        }
        //the current correlated column.
        public string VM_Correlated_column
        {
            get
            {
                return _data._Correlated_column;
            }
            set
            {
                _data._Correlated_column = value;
            }
        }
        //list of the current column Points.
        public List<DataPoint> VM_CurrentDataPoints
        {
            get
            {
                return new List<DataPoint>(_data._CurrentDataPoints);
            }
        }
        //list of the most correlated column Points.
        public List<DataPoint> VM_CorrelatedDataPoints
        {
            get
            {
                return new List<DataPoint>(_data._CorrelatedDataPoints);
            }
        }
        //Stop sending
        public void CloseALL()
        {
            _panel.Close();
        }
        public float VM_Throttle
        {
            get { return this._data._Throttle; }
            set { this._data._Throttle = value; }
        }
        public float VM_Rudder
        {
            get { return this._data._Rudder; }
            set { this._data._Rudder = value; }
        }
        public float VM_Elevator
        {
            get { return ConvertToJoy(this._data._Elevator); }
            set { this._data._Elevator = value; }
        }
        public float VM_Aileron
        {
            get { return ConvertToJoy(this._data._Aileron); }
            set { this._data._Aileron = value; }
        }
        public float VM_AirSpeed
        {
            get { return this._data._AirSpeed; }
            set { this._data._AirSpeed = value; }
        }
        public float VM_Roll
        {
            get { return this._data._Roll; }
            set { this._data._Roll = value; }
        }
        public float VM_Pitch
        {
            get { return this._data._Pitch; }
            set { this._data._Pitch = value; }
        }
        public float VM_Alimeter
        {
            get { return this._data._Alimeter; }
            set { this._data._Alimeter = value; }
        }
        public float VM_Yaw
        {
            get { return this._data._Yaw; }
            set { this._data._Yaw = value; }
        }
        public float VM_Direction
        {
            get { return this._data._Direction; }
            set { this._data._Direction = value; }
        }
        //list of the current column and the most correlative column Points.
        public List<DataPoint> VM_MultiDataPoints
        {
            get
            {
                return new List<DataPoint>(_data._MultiDataPoints);
            }
        }
        //list of the 10 last current column and the most correlative column Points.
        public List<DataPoint> VM_LastMultiDataPoints
        {
            get
            {
                return new List<DataPoint>(_data._LastMultiDataPoints);
            }
        }
        //list of 2 points declaring the regression line.
        public List<DataPoint> VM_LinearReg
        {
            get
            {
                return new List<DataPoint>(_data._LinearReg);
            }
        }       
        public event PropertyChangedEventHandler PropertyChanged;
        //the line number that currently being sent.
        public double VM_Num_line
        {
            get
            {
                return _panel._Num_line;
            }
            set
            {
                _panel._Num_line = (int)(value+1);                
            }
        }
        //number of rows.
        public double VM_LinesN
        {
            get { return _panel._LinesN; }
            set { _panel._LinesN = (int)value; }
        }
        //Constructor
        public ViewModel(Panel p, AnomalyData data)
        {
            _data = data;
            _panel = p;
            _panel.PropertyChanged += 
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyPropertyChanged("VM" + e.PropertyName);
                };
            _panel.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyNumLineChanged("VM" + e.PropertyName);
                };

            _data.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyPropertyChanged("VM" + e.PropertyName);
                };

            VM_Aileron = 100;
            VM_Elevator = 100;
            VM_Rudder = -1;
            VM_Throttle = 0;
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public void NotifyNumLineChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                if (propName == "VM_Num_line")
                {
                    _data._Num_line = _panel._Num_line;
                }
            }
        }
        //open file dialog to upload train csv.
        public bool ClickCSVTrain()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                _trainPath = openFileDialog1.FileName;
                return true;
            }
            return false;
        }

        //open file dialog to upload test csv.
        public bool ClickCSVTest()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                _testPath = openFileDialog1.FileName;

                if (_th != null)
                    _panel.setPath(_testPath);
                else
                {
                    _th = new Thread(() => _panel.CSV_Sender(_testPath));
                    _th.Start();
                }
                return true;
            }
            return false;
        }
        //upload the files to the Model, and learn the test.
        public void LearnProcess()
        {
            string file = File.ReadAllText(_trainPath);
            string features = "";
            for (int i = 0; i < VM_AllFeatures.Count; i++)
            {
                string col = VM_AllFeatures[i];
                features += col;
                if (i != VM_AllFeatures.Count - 1)
                {
                    features += ",";
                }
            }
            features += "\r\n";
            _trainFile = features + file;
            _data.UploadTrain(_trainFile); // send AnomalyData string of the file

            file = File.ReadAllText(_testPath);
            _testFile = features + file;
            _data.UploadTest(_testFile);

            _data.Learn();
        }
        //open file dialog to upload the xml file.
        public void clickXML()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "XML (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                string path = openFileDialog1.FileName;
                _data.AnalyzeXML(path);
            }
        }
        //the Panel controls buttons
        public void PlaybackControl(MediaEventArgs m)
        {
            string desctiption = m.getDescription();
            if(desctiption == "submit")
            {
                _panel.SetSpeed(m.getValue());
            }
            else if(desctiption == "skip back")
            {
                VM_Num_line = 0;
            }
            else if (desctiption == "back")
            {
                if(VM_Num_line >= 75)
                {
                    VM_Num_line -= 75;
                }
                else
                {
                    VM_Num_line = 0;
                }
            }
            else if (desctiption == "pause")
            {
                _panel.Pause();
            }
            else if(desctiption == "stop")
            {
                VM_Num_line = 0;
                Thread.Sleep(10);
                _panel.Pause();
            }
            else if (desctiption == "play")
            {
                _panel.Play();
            }
            else if (desctiption == "forward")
            {
                if(VM_Num_line < VM_LinesN - 75)
                {
                    VM_Num_line += 75;
                }
                else
                {
                    VM_Num_line = VM_LinesN - 1;
                }
            }
            else if (desctiption == "skip end")
            {
                VM_Num_line = VM_LinesN - 1;
            }
        }
        //open file dialog to upload the DLL.
        public void ClickDLL()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "DLL (*.dll)|*.dll|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                string path = openFileDialog1.FileName;
                _data.PathSetter(path);

/*
                string file = File.ReadAllText(_testPath);
                string features = "";
                for (int i = 0; i < VM_AllFeatures.Count; i++)
                {
                    string col = VM_AllFeatures[i];
                    features += col;
                    if (i != VM_AllFeatures.Count - 1)
                    {
                        features += ",";
                    }
                }
                features += "\r\n";
                _testFile = features + file;
*/

                _data.Start();
                _data.Draw();
            }
        }
        //list of anomalies.
        public List<string> VM_DllAnomaiesList
        {
            get
            {
                return _data._DllAnomaiesList;
            }
        }
        //convert anomalies atring to their index.
        public void SelectedAnomaly(string v)
        {
            VM_Num_line = _data.GetTimeStepFromString(v);
        }

        //denormelize the Joystick value
        public int ConvertToJoy(float value)
        {
            int oldRange = 2;
            int newRange = 90;
            int newValue = (int)(((value - (-1)) * newRange) / oldRange) + 60;
            return newValue;
        }

        public void GraphControls(GraphEventArgs m)
        {
            string desctiption = m.getDescription();
            if (desctiption == "ClickDLL")
            {
                ClickDLL();
            }
            else if (desctiption == "SelectedAnomaly")
            {
                SelectedAnomaly(m.getValue());
            }
        }
    }
}
