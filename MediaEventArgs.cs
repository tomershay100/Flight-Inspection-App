using System;

namespace DesktopApp
{
    public class MediaEventArgs : EventArgs
    {
        private readonly string _description;
        private readonly double _value;

        public MediaEventArgs(string s)
        {
            _description = s;
            _value = 0;
        }

        public MediaEventArgs(string s, double val)
        {
            _description = s;
            _value = val;
        }

        public string GetDescription()
        {
            return _description;
        }

        public double GetValue()
        {
            return _value;
        }
    }

    public class GraphEventArgs : EventArgs
    {
        private readonly string _description;
        private readonly string _value;

        public GraphEventArgs(string s)
        {
            _description = s;
            _value = "";
        }

        public GraphEventArgs(string s, string val)
        {
            _description = s;
            _value = val;
        }

        public string GetDescription()
        {
            return _description;
        }

        public string GetValue()
        {
            return _value;
        }
    }
}