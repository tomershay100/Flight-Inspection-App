using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DesktopApp
{
    //Panel Class (Model)
    public class Panel : FlightGearClient
    {
        private static EventWaitHandle wait = new ManualResetEvent(initialState: true);
        private int _linesN; //number of rows.
        private IEnumerable<string> lines;
        public void setPath(string path)
        {
            lines = File.ReadLines(path);
            m.WaitOne();
            _LinesN = lines.Count();
            m.ReleaseMutex();
            _Num_line = 0;
        }
        public int _LinesN
        {
            get { return _linesN; }
            set
            {
                _linesN = value;
                NotifyPropertyChanged("_LinesN");
            }
        }
        private bool needToClose;
        //Constructor
        public Panel() : base()
        {
            needToClose = false;
            _LinesN = 500;
        }
        //Constructor
        public Panel(int port) : base(port)
        {
            needToClose = false;
            _linesN = 500;
        }
        //send the csv file line-by-line to the FlightGear server.
        public void CSV_Sender(string path)
        {
            try {
                lines = File.ReadLines(path);
                m.WaitOne();
                _LinesN = lines.Count();
                m.ReleaseMutex();

                string line = lines.ElementAt(this._Num_line);
                while (line != null && !needToClose)
                {
                    Byte[] data = Encoding.ASCII.GetBytes(line + "\r\n");
                    _stream.Write(data, 0, data.Length);
                    while (_sendSpeed == 0)
                    {
                    }
                    Thread.Sleep((int)(1000 / this._sendSpeed));
                    m.WaitOne();
                    if (_Num_line > _linesN)
                        Pause();
                    this._Num_line++;
                    m.ReleaseMutex();
                    if (this._Num_line < lines.Count())
                    {
                        line = lines.ElementAt(this._Num_line);
                    }
                    wait.WaitOne();
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        //Pause the sending.
        public void Pause()
        {
            if (isRunning)
            {
                wait.Reset();
                isRunning = false;
            }
        }
        //Play the sending.
        public void Play()
        {
            if (!isRunning)
            {
                wait.Set();
                isRunning = true;
            }
        }
        public void Close()
        {
            isRunning = false;
            try
            {
                _stream.Close();
                _client.Close();
            }
            catch (Exception) { }
            needToClose = true;
        }
    }
}
