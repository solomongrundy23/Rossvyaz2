using System;
using System.IO;
using JSONHelper;

namespace Rossvyaz2
{
    public class Options
    {
        public string[] Files = new string[] { };
        public int WorkStyle = 0;
        public int Splitter = 0;
        public string[] Urls = new string[] { };
    }

    public static class OptionsWorker
    {
        public static string DataPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\";
        public static Options Options;
        public static void Load() =>
            Options = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\" + "RSParser.json").FromJson<Options>();
        public static void Save() => 
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\" + "RSParser.json", Options.ToJson());
    }
}
