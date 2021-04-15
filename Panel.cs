using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace DesktopApp
{
    //Panel Class (Model)
    public class Panel : FlightGearClient
    {
        private static readonly EventWaitHandle Wait = new ManualResetEvent(initialState: true);
        private int _linesN; //number of rows.
        private IEnumerable<string> _lines;
        public void SetPath(string path)
        {
            _lines = File.ReadLines(path);
            M.WaitOne();
            _LinesN = _lines.Count();
            M.ReleaseMutex();
            _NumLine = 0;
        }
        public int _LinesN
        {
            get => _linesN;
            set
            {
                _linesN = value;
                NotifyPropertyChanged("_LinesN");
            }
        }
        private bool _needToClose;
        //Constructor
        public Panel()
        {
            _needToClose = false;
            _LinesN = 500;
        }
        //Constructor
        public Panel(int port) : base(port)
        {
            _needToClose = false;
            _linesN = 500;
        }
        //send the csv file line-by-line to the FlightGear server.
        public void CSV_Sender(string path)
        {
            try {
                _lines = File.ReadLines(path);
                M.WaitOne();
                var lines = _lines.ToList();
                _LinesN = lines.Count();
                M.ReleaseMutex();

                var line = lines.ElementAt(this._NumLine);
                while (line != null && !_needToClose)
                {
                    var data = Encoding.ASCII.GetBytes(line + "\r\n");
                    Stream.Write(data, 0, data.Length);
                    while (SendSpeed == 0)
                    {
                    }
                    Thread.Sleep((int)(1000 / this.SendSpeed));
                    M.WaitOne();
                    if (_NumLine > _linesN)
                        Pause();
                    this._NumLine++;
                    M.ReleaseMutex();
                    if (this._NumLine < lines.Count())
                    {
                        line = lines.ElementAt(this._NumLine);
                    }
                    Wait.WaitOne();
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        //Pause the sending.
        public void Pause()
        {
            if (IsRunning)
            {
                Wait.Reset();
                IsRunning = false;
            }
        }
        //Play the sending.
        public void Play()
        {
            if (!IsRunning)
            {
                Wait.Set();
                IsRunning = true;
            }
        }
        public void Close()
        {
            IsRunning = false;
            try
            {
                Stream.Close();
                TcpClient.Close();
            }
            catch (Exception)
            {
                // ignored
            }

            _needToClose = true;
        }
    }
}
