using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.ComponentModel;

namespace DesktopApp
{
    //Client class
    public class Client
    {
        private int _port;
        protected readonly TcpClient TcpClient;
        protected readonly NetworkStream Stream;
        protected bool IsRunning = false;

        protected Client(int port)
        {
            _port = port;
            try
            {
                TcpClient = new TcpClient("127.0.0.1", port);
                Stream = TcpClient.GetStream();
                IsRunning = true;
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
        protected double SendSpeed;
        public event PropertyChangedEventHandler PropertyChanged;

        private int _numLine; //the line number that currently being sent.

        public int _NumLine
        {
            get => _numLine;
            set
            {
                //m.WaitOne();
                _numLine = value;
                //m.ReleaseMutex();
                NotifyPropertyChanged("_Num_line");
            }
        }

        protected static readonly Mutex M = new Mutex();

        //Constructor
        protected FlightGearClient() : base(5400)
        {
            SendSpeed = 10;
            _NumLine = 0;
        }

        //Constructor
        protected FlightGearClient(int port) : base(port)
        {
            SendSpeed = 10;
            _NumLine = 0;
        }

        //Destructor
        ~FlightGearClient()
        {
            if (!IsRunning) return;
            // Close socket.
            Stream.Close();
            TcpClient.Close();
        }

        //Sets the sending speed.
        public void SetSpeed(double d)
        {
            M.WaitOne();
            SendSpeed = d;
            M.ReleaseMutex();
        }

        protected void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}