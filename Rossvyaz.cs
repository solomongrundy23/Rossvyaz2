using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rossvyaz2
{
    public class Records : IDisposable
    {
        /// <summary>
        /// Создать экземпляр класса Records
        /// </summary>
        public Records()
        {
        }
        /// <summary>
        /// Создать экземпляр класса Records и загрузить данные из файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public Records(string FileName)
        {
            Load(FileName);
        }
        /// <summary>
        /// Список распарсиных записей
        /// </summary>
        public List<Record> Items = new List<Record>();
        /// <summary>
        /// Загрузить и распарсить файлы
        /// </summary>
        public void Load()
        {
            string[] filelist = new string[]
            {
                "Kody_ABC-3kh.csv",
                "Kody_ABC-4kh.csv",
                "Kody_ABC-8kh.csv",
                "Kody_DEF-9kh.csv"
            };
            foreach (string file_name in filelist) Load(file_name, false);
        }
        /// <summary>
        /// Загрузить и распрасить указанный файл
        /// </summary>
        /// <param name="file_name">Имя файла</param>
        /// <param name="clear_old_records">Очищать ли старые данные</param>
        public void Load(string file_name, bool clear_old_records = true)
        {
            try
            {
                if (clear_old_records) Items.Clear();
                string[] data = File.ReadAllLines(file_name);
                for (int i = 1; i < data.Length; i++)
                    if (!string.IsNullOrEmpty(data[i])) Items.Add(new Record(data[i]));
            }
            catch (Exception ex) 
            {
                WorkWindow.Error(ex.Message);
            }
        }
        /// <summary>
        /// Загрузить и распарсить файлы асинхронно
        /// </summary>
        public async Task LoadAsync()
        {
            await Task.Run(() => Load());
        }
        /// <summary>
        ///  Загрузить и распрасить указанный файл асинхронно
        /// </summary>
        /// <param name="file_name">Имя файла</param>
        /// <param name="clear_old_records">Очищать ли старые данные</param>
        public async Task LoadAsync(string file_name, bool clear_old_records = true)
        {
            await Task.Run(() => Load(file_name, clear_old_records));
        }
        /// <summary>
        ///  Загрузить и распрасить указанный файл асинхронно
        /// </summary>
        /// <param name="file_name">Имя файла</param>
        /// <param name="clear_old_records">Очищать ли старые данные</param>
        public async Task LoadAsync(string[] file_names, bool clear_old_records = true)
        {
            if (clear_old_records) this.Items.Clear();
            foreach (string file_name in file_names) await Task.Run(() => Load(file_name, false));
        }
        /// <summary>
        /// Получить запись по телефонному номеру, если в диапозон входит этот файл, то вернет Record, иначе null
        /// </summary>
        /// <param name="number">Номер телефона</param>
        /// <returns>Запись Record</returns>
        public Record GetByNumber(string number)
        {
            if (!ulong.TryParse(number, out ulong num)) return null;
            return Items.Where(x => num >= x.Min && num <= x.Max).FirstOrDefault();
        }
        /// <summary>
        /// Получить список регионов
        /// </summary>
        /// <param name="filter">фильтр по содержится без учета регистра</param>
        /// <returns></returns>
        public string[] GetRegionsList(string filter = "")
        {
            var list = Items.Select(x => x.Region).Distinct().ToArray();
            if (filter == "")
                return list.ToArray();
            else
            {
                filter = filter.ToLower();
                return list.Where(x => x.ToLower().Contains(filter)).Distinct().ToArray();
            }
        }
        /// <summary>
        /// Получить список операторов
        /// </summary>
        /// <param name="filter">фильтр по содержится без учета регистра</param>
        /// <returns></returns>
        public string[] GetOperatorsList(string filter = "")
        {
            var list = Items.Select(x => x.Operator).Distinct();
            if (filter == "")
                return list.ToArray();
            else
            {
                filter = filter.ToLower();
                return list.Where(x => x.ToLower().Contains(filter)).Distinct().ToArray();
            }
        }
        /// <summary>
        /// Получить оператора по номеру телефона
        /// </summary>
        /// <param name="number">номера телефона</param>
        /// <returns></returns>
        public string GetOperatorByNumber(string number)
        {
            var record = GetByNumber(number);
            return record?.Operator;
        }
        /// <summary>
        /// Получить регион по номеру телефона
        /// </summary>
        /// <param name="number">номера телефона</param>
        /// <returns></returns>
        public string GetRegionByNumber(string number)
        {
            var record = GetByNumber(number);
            return record?.Region;
        }
        /// <summary>
        /// Получить записи по операторам и регионам
        /// </summary>
        /// <param name="operator_name">оператор</param>
        /// <param name="region_name">регион</param>
        /// <returns>массив записей</returns>
        public Record[] GetRecords(string[] operator_name, string[] region_name, bool operatorNotContains, bool regionNotContains)
        {
            if (operator_name.Length == 0 && region_name.Length == 0) 
                return Items.ToArray();
            if (operatorNotContains && regionNotContains)
            {
                if (operator_name.Length > 0 && region_name.Length == 0)
                    return Items.Where(x => !operator_name.Contains(x.Operator)).ToArray();
                if (operator_name.Length == 0 && region_name.Length > 0)
                    return Items.Where(x => !region_name.Contains(x.Region)).ToArray();
                return Items.Where(x => !operator_name.Contains(x.Operator) && !region_name.Contains(x.Region)).ToArray();
            }
            else
            if (operatorNotContains && !regionNotContains)
            {
                if (operator_name.Length > 0 && region_name.Length == 0)
                    return Items.Where(x => !operator_name.Contains(x.Operator)).ToArray();
                if (operator_name.Length == 0 && region_name.Length > 0)
                    return Items.Where(x => region_name.Contains(x.Region)).ToArray();
                return Items.Where(x => !operator_name.Contains(x.Operator) && region_name.Contains(x.Region)).ToArray();
            }
            if (!operatorNotContains && regionNotContains)
            {
                if (operator_name.Length > 0 && region_name.Length == 0)
                    return Items.Where(x => operator_name.Contains(x.Operator)).ToArray();
                if (operator_name.Length == 0 && region_name.Length > 0)
                    return Items.Where(x => !region_name.Contains(x.Region)).ToArray();
                return Items.Where(x => operator_name.Contains(x.Operator) && !region_name.Contains(x.Region)).ToArray();
            }
            else
            {
                if (operator_name.Length > 0 && region_name.Length == 0)
                    return Items.Where(x => operator_name.Contains(x.Operator)).ToArray();
                if (operator_name.Length == 0 && region_name.Length > 0)
                    return Items.Where(x => region_name.Contains(x.Region)).ToArray();
                return Items.Where(x => operator_name.Contains(x.Operator) && region_name.Contains(x.Region)).ToArray();
            }
        }
        /// <summary>
        /// Получить записи по оператору и региону
        /// </summary>
        /// <param name="operator_name">оператор</param>
        /// <param name="region_name">регион</param>
        /// <returns>массив записей</returns>
        public Record[] GetRecords(string operator_name, string region_name)
        {
            if (operator_name == "" && region_name == "")
                return Items.ToArray();
            if (operator_name != "" && region_name == "")
                return Items.Where(x => x.Operator == operator_name).ToArray();
            if (operator_name == "" && region_name != "")
                return Items.Where(x => x.Region == region_name).ToArray();
            return Items.Where(x => x.Operator == operator_name && x.Region == region_name).ToArray();
        }
        /// <summary>
        /// Убить экземпляр класса
        /// </summary>
        public void Dispose()
        {
            Items = null;
            GC.SuppressFinalize(this);
            GC.Collect(GC.MaxGeneration);
        }
    }

    public class Record : Range
    {
        public Record(string param_string)
        {
            try
            {
                string[] recs = param_string.Split(';');
                Min = ulong.Parse("7" + recs[0] + recs[1]);
                Max = ulong.Parse("7" + recs[0] + recs[2]);
                Operator = recs[4];
                Region = recs[5];
            }
            catch
            {
                throw new FormatException("Ошибка обработки записи");
            }
        }
        public string Operator;
        public string Region;
        public override string ToString(string splitter = "\t") 
            => $"{Min}{splitter}{Max}{splitter}{Operator}{splitter}{Region}";
    }

    public static class CSVLoader
    {
        public static Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            {"Kody_DEF-9kh.csv", "https://www.rossvyaz.ru/docs/articles/Kody_DEF-9kh.csv"},
            {"Kody_ABC-3kh.csv", "https://www.rossvyaz.ru/docs/articles/Kody_ABC-3kh.csv"},
            {"Kody_ABC-4kh.csv", "https://www.rossvyaz.ru/docs/articles/Kody_ABC-4kh.csv"},
            {"Kody_ABC-8kh.csv", "https://www.rossvyaz.ru/docs/articles/Kody_ABC-8kh.csv"}
        };

        public static async Task DownloadAsync() => await Task.Run(() => Download());
        public static void Download()
        {
            var Errors = new StringBuilder();
            foreach (KeyValuePair<string, string> url in urls)
            {
                string ex = Download_file(url.Value, url.Key);
                if (ex != null) Errors.AppendLine($"{url.Value} : {ex}");
            } 
            if (Errors.Length > 0) throw new Exception(Errors.ToString());
        }

        private static string Download_file(string input, string output)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(input, output);
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}