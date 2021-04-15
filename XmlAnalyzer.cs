using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DesktopApp
{
    //XmlAnalyzer class
    internal static class XmlAnalyzer
    {
        //Analyzes the XML file and returns a list of name tags in the file.
        public static List<string> Analyzer(string path)
        {
            var names = new List<string>();
            var lines = File.ReadLines(path);
            var lineIndex = 0;

            IEnumerable<string> enumerable = lines.ToList();
            var line = enumerable.ElementAt(lineIndex);
            while (!line.Contains("</output>"))
            {
                var currentName = "";
                if (line.Contains("<name>"))
                {
                    var index = 0;
                    while (line[index++] != '>')
                    {
                    }

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

                line = enumerable.ElementAt(++lineIndex);
            }

            return names;
        }
    }
}