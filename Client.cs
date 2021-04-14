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
using System.Windows.Markup;
using System.ComponentModel;

namespace DesktopApp
{
    //Client class
    public class Client
    {
        protected int _port;
        protected TcpClient _client;
        protected NetworkStream _stream;
        protected bool isRunning = false;

        public Client(int port)
        {
            _port = port;
            try
            {
                _client = new TcpClient("127.0.0.1", port);
                _stream = _client.GetStream();
                isRunning = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Connection Error", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    //connect to the FlightGear app
    public class FlightGearClient : Client, INotifyPropertyChanged
    {
        protected double _sendSpeed;
        public event PropertyChangedEventHandler PropertyChanged;

        private int _num_line; //the line number that currently being sent.
        public int _Num_line
        {
            get { return _num_line; }
            set
            {
                //m.WaitOne();
                _num_line = value;
                //m.ReleaseMutex();
                NotifyPropertyChanged("_Num_line");
            }
        }
        public static Mutex m = new Mutex();
        //Constructor
        public FlightGearClient() : base(5400)
        {
            _sendSpeed = 10;
            _Num_line = 0;
        }
        //Constructor
        public FlightGearClient(int port) : base(port)
        {
            _sendSpeed = 10;
            _Num_line = 0;
        }
        //Distructor
        ~FlightGearClient()
        {
            if (isRunning)
            {
                // Close socket.
                _stream.Close();
                _client.Close();
            }
        }
        //Sets the sending speed.
        public void SetSpeed(double d)
        {
            m.WaitOne();
            this._sendSpeed = d;
            m.ReleaseMutex();
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
