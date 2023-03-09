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

    public static class Config
    {
        public static readonly long animDuration = 250;
        public static readonly long blurLevel = 7;
    }

    public static class OptionsWorker
    {
        public static string DataPath { get; private set; } = AppDomain.CurrentDomain.BaseDirectory + "Files\\";
        public static Options Options { get; set; }
        public static void Load() =>
            Options = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\" + "RSParser.json").FromJson<Options>();
        public static void Save() => 
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\" + "RSParser.json", Options.ToJson());
    }
}
