using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace DesktopApp
{
    //ViewModel class
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly AnomalyData _data; //AnomalyData Model.
        private readonly Panel _panel; //Panel Model.
        private string _trainPath; //the train csv file path.
        private string _testPath; //the test csv file path.
        private string _trainFile; //the entire train file
        private string _testFile; //the entire test file

        private Thread _th;

        //the Title above the DLL's graphs.
        public string VM_DllTitle => _data._DllTitle;

        //the DLL's PlotModel.
        public PlotModel VM_DllPlotModel => _data._DllPlotModel;

        //list of all the XML features.
        public List<string> VM_AllFeatures => _data._AllFeatures;

        //the current selected column.
        public string VM_CurrColumn
        {
            get => _data._CurrColumn;
            set => _data._CurrColumn = value;
        }

        //the current correlated column.
        public string VM_CorrelatedColumn
        {
            get => _data._CorrelatedColumn;
            set => _data._CorrelatedColumn = value;
        }

        //list of the current column Points.
        public List<DataPoint> VM_CurrentDataPoints => new List<DataPoint>(_data._CurrentDataPoints);

        //list of the most correlated column Points.
        public List<DataPoint> VM_CorrelatedDataPoints => new List<DataPoint>(_data._CorrelatedDataPoints);

        //Stop sending
        public void CloseAll()
        {
            _panel.Close();
        }

        public float VM_Throttle
        {
            get => _data._Throttle;
            set => _data._Throttle = value;
        }

        public float VM_Rudder
        {
            get => _data._Rudder;
            set => _data._Rudder = value;
        }

        public float VM_Elevator
        {
            get => ConvertToJoy(_data._Elevator);
            set => _data._Elevator = value;
        }

        public float VM_Aileron
        {
            get => ConvertToJoy(_data._Aileron);
            set => _data._Aileron = value;
        }

        public float VM_AirSpeed
        {
            get => _data._AirSpeed;
            set => _data._AirSpeed = value;
        }

        public float VM_Roll
        {
            get => _data._Roll;
            set => _data._Roll = value;
        }

        public float VM_Pitch
        {
            get => _data._Pitch;
            set => _data._Pitch = value;
        }

        public float VM_Alimeter
        {
            get => _data._Alimeter;
            set => _data._Alimeter = value;
        }

        public float VM_Yaw
        {
            get => _data._Yaw;
            set => _data._Yaw = value;
        }

        public float VM_Direction
        {
            get => _data._Direction;
            set => _data._Direction = value;
        }

        //list of the current column and the most correlative column Points.
        public List<DataPoint> VM_MultiDataPoints => new List<DataPoint>(_data._MultiDataPoints);

        //list of the 10 last current column and the most correlative column Points.
        public List<DataPoint> VM_LastMultiDataPoints => new List<DataPoint>(_data._LastMultiDataPoints);

        //list of 2 points declaring the regression line.
        public List<DataPoint> VM_LinearReg => new List<DataPoint>(_data._LinearReg);

        public event PropertyChangedEventHandler PropertyChanged;

        //the line number that currently being sent.
        public double VM_NumLine
        {
            get => _panel._NumLine;
            set => _panel._NumLine = (int) (value + 1);
        }

        //number of rows.
        public double VM_LinesN
        {
            get => _panel._LinesN;
            set => _panel._LinesN = (int) value;
        }

        //Constructor
        public ViewModel(Panel p, AnomalyData data)
        {
            _data = data;
            _panel = p;
            _panel.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e) { NotifyPropertyChanged("VM" + e.PropertyName); };
            _panel.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e) { NotifyNumLineChanged("VM" + e.PropertyName); };

            _data.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e) { NotifyPropertyChanged("VM" + e.PropertyName); };

            VM_Aileron = 100;
            VM_Elevator = 100;
            VM_Rudder = -1;
            VM_Throttle = 0;
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void NotifyNumLineChanged(string propName)
        {
            if (PropertyChanged == null) return;
            if (propName == "VM_Num_line")
            {
                _data._NumLine = _panel._NumLine;
            }
        }

        //open file dialog to upload train csv.
        public bool ClickCsvTrain()
        {
            var openFileDialog1 = new Microsoft.Win32.OpenFileDialog {Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*"};
            if (openFileDialog1.ShowDialog() != true) return false;
            _trainPath = openFileDialog1.FileName;
            return true;
        }

        //open file dialog to upload test csv.
        public bool ClickCsvTest()
        {
            var openFileDialog1 = new Microsoft.Win32.OpenFileDialog {Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*"};
            if (openFileDialog1.ShowDialog() != true) return false;
            _testPath = openFileDialog1.FileName;

            if (_th != null)
                _panel.SetPath(_testPath);
            else
            {
                _th = new Thread(() => _panel.CSV_Sender(_testPath));
                _th.Start();
            }

            return true;
        }

        //upload the files to the Model, and learn the test.
        public void LearnProcess()
        {
            var file = File.ReadAllText(_trainPath);
            var features = "";
            for (var i = 0; i < VM_AllFeatures.Count; i++)
            {
                var col = VM_AllFeatures[i];
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
        public void ClickXml()
        {
            var openFileDialog1 = new Microsoft.Win32.OpenFileDialog {Filter = "XML (*.xml)|*.xml|All Files (*.*)|*.*"};
            if (openFileDialog1.ShowDialog() != true) return;
            var path = openFileDialog1.FileName;
            _data.AnalyzeXml(path);
        }

        //the Panel controls buttons
        public void PlaybackControl(MediaEventArgs m)
        {
            var description = m.GetDescription();
            switch (description)
            {
                case "submit":
                    _panel.SetSpeed(m.GetValue());
                    break;
                case "skip back":
                    VM_NumLine = 0;
                    break;
                case "back" when VM_NumLine >= 75:
                    VM_NumLine -= 75;
                    break;
                case "back":
                    VM_NumLine = 0;
                    break;
                case "pause":
                    _panel.Pause();
                    break;
                case "stop":
                    VM_NumLine = 0;
                    Thread.Sleep(10);
                    _panel.Pause();
                    break;
                case "play":
                    _panel.Play();
                    break;
                case "forward" when VM_NumLine < VM_LinesN - 75:
                    VM_NumLine += 75;
                    break;
                case "forward":
                case "skip end":
                    VM_NumLine = VM_LinesN - 1;
                    break;
            }
        }

        //open file dialog to upload the DLL.
        private void ClickDll()
        {
            var openFileDialog1 = new Microsoft.Win32.OpenFileDialog {Filter = "DLL (*.dll)|*.dll|All Files (*.*)|*.*"};
            if (openFileDialog1.ShowDialog() != true) return;
            var path = openFileDialog1.FileName;
            _data.PathSetter(path);
            _data.Start();
            _data.Draw();
        }

        //list of anomalies.
        public List<string> VM_DllAnomaliesList => _data._DllAnomaliesList;

        //convert anomalies string to their index.
        private void SelectedAnomaly(string v)
        {
            VM_NumLine = _data.GetTimeStepFromString(v);
        }

        //denormalize the Joystick value
        private static int ConvertToJoy(float value)
        {
            const int oldRange = 2;
            const int newRange = 90;
            var newValue = (int) (((value - (-1)) * newRange) / oldRange) + 60;
            return newValue;
        }

        public void GraphControls(GraphEventArgs m)
        {
            var description = m.GetDescription();
            switch (description)
            {
                case "ClickDLL":
                    ClickDll();
                    break;
                case "SelectedAnomaly":
                    SelectedAnomaly(m.GetValue());
                    break;
            }
        }
    }
}