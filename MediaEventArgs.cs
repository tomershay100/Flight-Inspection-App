using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp
{
    public class MediaEventArgs: EventArgs
    {
        private string _description;
        private double _value;

        public MediaEventArgs(string s): base()
        {
            _description = s;
            _value = 0;
        }

        public MediaEventArgs(string s, double val) : base()
        {
            _description = s;
            _value = val;
        }

        public string getDescription()
        {
            return _description;
        }

        public double getValue()
        {
            return _value;
        }
    }
    public class GraphEventArgs : EventArgs
    {
        private string _description;
        private string _value;

        public GraphEventArgs(string s) : base()
        {
            _description = s;
            _value = "";
        }

        public GraphEventArgs(string s, string val) : base()
        {
            _description = s;
            _value = val;
        }

        public string getDescription()
        {
            return _description;
        }

        public string getValue()
        {
            return _value;
        }
    }
}
