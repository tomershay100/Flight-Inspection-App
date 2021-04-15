using System;
using System.Collections.Generic;
using System.Linq;

namespace DesktopApp
{
    //TimeSeries class
    public class TimeSeries
    {
        private readonly Dictionary<string, List<float>> _csvMap; //the dictionary
        private readonly int _columnsSize;
        private readonly int _rowsSize;
        private readonly List<string> _featuresList;

        //Constructor. Processes the CSV file into a dictionary.
        public TimeSeries(string file)
        {
            _featuresList = new List<string>();
            _csvMap = new Dictionary<string, List<float>>();

            var lines = file.Split('\n');

            for (var index = 0; index < lines.Length; index++)
            {
                lines[index] = lines[index].Split('\r')[0];
            }

            _rowsSize = lines.Length - 2;
            var lineIndex = 0;
            var line = lines[lineIndex];
            _columnsSize = 0;
            var temp = "";
            foreach (var ch in line)
            {
                if (ch == ',')
                {
                    _featuresList.Add(temp);
                    _columnsSize++;
                    temp = "";
                }
                else
                {
                    temp += ch;
                }
            }

            _featuresList.Add(temp);
            _columnsSize++;

            lineIndex++; // features

            for (var j = 0; j < _columnsSize; j++)
            {
                var key = _featuresList[j];
                _csvMap[key] = new List<float>();
            }

            for (var i = 0; i < _rowsSize; i++)
            {
                line = lines[lineIndex++];
                var stringValues = line.Split(',');
                var floatValues = stringValues.Select(float.Parse).ToList();

                for (var j = 0; j < _columnsSize; j++)
                {
                    var key = _featuresList[j];
                    _csvMap[key].Add(floatValues[j]);
                }

                floatValues.Clear();
            }

            Console.WriteLine("");
        }

        //returns column by its name.
        public List<float> GetColumn(string key)
        {
            return _csvMap.ContainsKey(key) ? _csvMap[key] : new List<float>();
        }

        //returns the column size.
        public int GetColumnSize()
        {
            return _columnsSize;
        }

        //returns row size.
        public int GetRowSize()
        {
            return _rowsSize;
        }

        //returns all the Features
        public List<string> GetFeatures()
        {
            return _featuresList;
        }
    }
}