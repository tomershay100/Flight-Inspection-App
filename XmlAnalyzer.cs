using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp
{
    //XmlAnalyzer class
    class XmlAnalyzer
    {
        //Analyzes the XML file and returns a list of name tags in the file.
        public static List<string> Analyzer(string path)
        {
            List<string> names = new List<string>();
            string currentName;
            var lines = File.ReadLines(path);
            int lineIndex = 0;

            string line = lines.ElementAt(lineIndex);
            while (!line.Contains("</output>"))
            {
                currentName = "";
                if (line.Contains("<name>"))
                {
                    int index = 0;
                    while (line[index++] != '>') ;
                    while (line[index] != '<')
                    {
                        currentName += line[index++];
                    }
                    if (names.Contains(currentName))
                    {
                        names[names.IndexOf(currentName)] = currentName + "1";
                        currentName += "2";
                    }
                    names.Add(currentName);
                }

                line = lines.ElementAt(++lineIndex);
            }
            return names;
        }

    }
}
