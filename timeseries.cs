using System;
using System.Collections.Generic;

namespace DesktopApp
{
    //TimeSeries class
    public class TimeSeries
    {
        private Dictionary<string, List<float>> csvMap; //the dictionary
        private int _columnsSize;
        private int _rowsSize;
        List<string> featuresList;

        //Constructor. Processes the CSV file into a dictionary.
        public TimeSeries(string file)
        {
            featuresList = new List<string>();
            csvMap = new Dictionary<string, List<float>>();

            string[] lines = file.Split('\n');

            for (var index = 0; index < lines.Length; index++)
            {
                lines[index] = lines[index].Split('\r')[0];
            }

            _rowsSize = lines.Length - 2;
            int lineIndex = 0;
            string line = lines[lineIndex];
            _columnsSize = 0;
            string temp = "";
            foreach (char ch in line)
            {
                if (ch == ',')
                {
                    featuresList.Add(temp);
                    _columnsSize++;
                    temp = "";
                }
                else
                {
                    temp += ch;
                }
            }

            featuresList.Add(temp);
            _columnsSize++;

            line = lines[lineIndex++]; // features

            for (int j = 0; j < _columnsSize; j++)
            {
                string key = featuresList[j];
                csvMap[key] = new List<float>();
            }
            
            for (int i = 0; i < _rowsSize; i++)
            {
                line = lines[lineIndex++];
                string[] stringValues = line.Split(',');
                List<float> floatValues = new List<float>();
                foreach (string str in stringValues)
                {
                    floatValues.Add(float.Parse(str));
                }

                for (int j = 0; j < _columnsSize; j++)
                {
                    string key = featuresList[j];
                    csvMap[key].Add(floatValues[j]);
                }

                floatValues.Clear();
            }
            Console.WriteLine("");
        }
        //returns column by its name.
        public List<float> GetColumn(string key)
        {
            if(csvMap.ContainsKey(key))
                return csvMap[key];
            return new List<float>();
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
            return featuresList;
        }
    }
}