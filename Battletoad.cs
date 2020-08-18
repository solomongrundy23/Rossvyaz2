using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace Rossvyaz2
{
    /// <summary>
    /// Цивилизованная многопоточная очередь
    /// </summary>
    public class CivilizedQueue : ConcurrentQueue<string> { }
    /// <summary>
    /// Расширения для различных классов
    /// </summary>
    public static class Extensions
    {
        public static string RemoveText(this string str, string text) => str.Replace(text, "");
        public static string RemoveTextStartEnd(this string str, string text) => str.RemoveTextStart(text).RemoveTextEnd(text);
        public static string RemoveTextStart(this string str, string text) => str.StartsWith(text) ? str.Substring(text.Length) : text;
        public static string RemoveTextEnd(this string str, string text) => str.EndsWith(text) ? str.Substring(0, str.Length - text.Length) : text;
        /// <summary>
        /// Возвращается выравненную строку до определенной длины
        /// </summary>
        /// <param name="str">исходная  строка</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string ToStringAndPad(this int num, int length, char ch = ' ') => num.ToString().Pad(length, ' ');
        /// <summary>
        /// Возвращается выравненную указанным символом строку до определенной длины
        /// </summary>
        /// <param name="str">исходная  строка</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string Pad(this string str, int length, char ch = ' ')
        {
            if (length - str.Length <= 0) return str;
            return str.PadLeft(str.Length + (length - str.Length) / 2, ch).PadRight(length, ch);
        }
        /// <summary>
        /// Получить строку повторение данной строки
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="repeat">количество повторений</param>
        /// <returns>новая строка</returns>
        public static string Repeat(this string str, int repeat) => Addons.StringRepeat(str, repeat);
        /// <summary>
        /// Распарсить дату и время
        /// </summary>
        /// <param name="str">строка</param>
        /// <returns>DateTime, null - при неуспешной операции</returns>
        public static DateTime? DateTimeParse(this string str) => Addons.DateTimeParse(str);
        /// <summary>
        /// Получить n символов с конца строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="length">количество символов с конца</param>
        /// <returns></returns>
        public static string SubStringFromEnd(this string text, int length)
        {
            if (text.Length < length) throw new Exception("Длина подстроки не может быть больше искомой");
            return text.Substring(text.Length - length);
        }
        /// <summary>
        /// Добавить в начало и конец строки строки
        /// </summary>
        /// <param name="text">исходный текст</param>
        /// <param name="start">текст в начало</param>
        /// <param name="end">текст в конец</param>
        /// <returns></returns>
        public static string Wrap(this string text, string start, string end)
            => $"{start}{text}{end}";
        //String
        /// <summary>
        /// Быстро и безболезнено перевести в int
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static int ToIntOrDefault(this string text, int default_value = int.MinValue)
           => Addons.ToIntOrDeafault(text, default_value);
        /// <summary>
        /// Быстро перевести в int, если не удается, то вернёт Null
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int? ToInt(this string text, int default_value = int.MinValue)
           => Addons.ToInt(text, default_value);
        /// <summary>
        /// Получить Base64 из строки
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns></returns>
        public static string GetBase64String(this string text)
            => GetBase64String(text, Encoding.Default);
        /// <summary>
        /// Получить Base64 из строки
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="encoding">Кодировка</param>
        /// <returns></returns>
        public static string GetBase64String(this string text, Encoding encoding)
            => encoding.GetBytes(text).GetBase64String();
        /// <summary>
        /// Быстро и безболезнено перевести в long
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static long ToLong(this string text, long default_value = long.MinValue)
            => Addons.ToLong(text, default_value);
        /// <summary>
        /// Перевести кириллицу в транслит
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static string ToTraslite(this string text) => Translitiration.ConvertToLatin(text);
        /// <summary>
        /// Разбивает текст на массив строк
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="withEmpty">Пропустить пустые строки</param>
        /// <returns></returns>
        public static string[] GetLines(this string text, bool withEmpty = true)
            => text.Split(
                new[] { "\r\n", "\r", "\n" },
                withEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries
               );
        /// <summary>
        /// Разбить текст на слова
        /// Любой не буквенный символ - разделитель
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns></returns>
        public static string[] GetWords(this string text)
            => Regex.Split(text, @"[^(\w)|\-)]").Where(x => x != string.Empty).ToArray();
        /// <summary>
        /// Совпадает ли с регулярым выражением
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="matcher">регулярное выражение</param>
        /// <returns></returns>
        public static bool IsRegexMatch(this string text, string matcher) => Regex.IsMatch(text, matcher);
        /// <summary>
        /// Получить только цифры из строки
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns></returns>
        public static string GetJustNums(this string text)
            => Regex.Replace(text, @"[^\d]", "");
        public static string GetJustLetters(this string text)
            => Regex.Replace(text, @"([^\w])|\d", "");
        /// <summary>
        /// Обрезать строку до определенного количества символов
        /// </summary>
        /// <param name="text">Строк</param>
        /// <param name="count">Количество символов</param>
        /// <param name="end">что поставить в конце, по умолчанию многоточие</param>
        /// <returns></returns>
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="splitter">символ для стыковки</param>
        /// <returns></returns>
        public static string ToText<T>(this T[] array, string splitter = "\r\n")
               => string.Join(splitter, array);
        public static string Cut(this string text, int count, string end = "...")
            => text.Length > count ? text.Substring(0, count) + end : text;
        /// <summary>
        /// Является ли строка IP-адресом(IPv4)
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static bool IsIP(this string text)
            => Addons.IsIP(text);
        /// <summary>
        /// Получить хэш от строки, Type указывает на тип хэша по умолчанию Hash.Type.MD5
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="Type">тип хэша</param>
        /// <returns></returns>
        public static string GetHashString(this string text, Hash.Type Type = Hash.Type.MD5)
            => Hash.GetHash(Encoding.Unicode.GetBytes(text), Type);
        /// <summary>
        /// Найди значения по регулярному выражения
        /// </summary>
        /// <param name="DATA">строка</param>
        /// <param name="TAG">регулярное выражение</param>
        /// <param name="registr">регистрозависимость</param>
        /// <param name="singleline">считать весь текст единой строкой</param>
        /// <param name="group">вывести группу</param>
        /// <returns></returns>
        public static List<string> FindByRegex(this string DATA, string TAG, bool registr = false,
                                               bool singleline = false, int group = 0)
            => Addons.FindByRegex(DATA, TAG, registr, singleline, group);
        /// <summary>
        /// Сравнить со строкой
        /// </summary>
        /// <param name="string_data">Строка</param>
        /// <param name="text">Строка для сравнения</param>
        /// <returns></returns>
        public static bool EqualString(this string this_string, string other_string)
            => string.Equals(this_string, other_string, StringComparison.Ordinal);
        /// <summary>
        /// Сравнить со строкой, игнорируя регистр
        /// </summary>
        /// <param name="string_data">Строка</param>
        /// <param name="text">Строка для сравнения</param>
        /// <returns></returns>
        public static bool EqualStringIgnoreCase(this string this_string, string other_string)
        => string.Equals(this_string, other_string, StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// Получить последние count символов из строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="count">количество символов</param>
        /// <returns></returns>
        public static string FromTheEnd(this string text, int count)
        {
            return
            count >= text.Length || text.Length == 0
            ?
            text
            :
            text.Substring(text.Length - count);
        }
        //String[] and List<string>
        public static string ToText(this List<string> text, string splitter = "\r\n")
            => string.Join(splitter, text);
        /// <summary>
        /// Получить только те строки, которые содержат подстроку
        /// </summary>
        /// <param name="lines">массив строк</param>
        /// <param name="filter">подстрока</param>
        /// <returns></returns>
        public static string[] FilterContains(this string[] lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые содержат подстроку
        /// </summary>
        /// <param name="lines">список строк</param>
        /// <param name="filter">подстрока</param>
        /// <returns></returns>
        public static string[] FilterContains(this List<string> lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые удолетворяют регулярному выражению
        /// </summary>
        /// <param name="lines">массив строк</param>
        /// <param name="filter">регулярное выражение</param>
        /// <returns></returns>
        public static string[] FilterRegex(this string[] lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые удолетворяют регулярному выражению
        /// </summary>
        /// <param name="lines">список строк</param>
        /// <param name="filter">регулярное выражение</param>
        /// <returns></returns>
        public static string[] FilterRegex(this List<string> lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        //Dictionary
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="dictionary">словарь</param>
        /// <param name="KeyValueSplitter">символ(ы) для стыковки</param>
        /// <returns></returns>
        public static string ToText<K, V>(this Dictionary<K, V> dictionary, 
            string KeyValueSplitter = "\t")
            => string.Join(Environment.NewLine, 
                dictionary.Select(x => $"{x.Key}{KeyValueSplitter}{x.Value}").ToArray());
        //<T>
        /// <summary>
        /// Вывести значение
        /// </summary>
        /// <param name="value">пересенная</param>
        /// <param name="key">указать имя переменной</param>
        /// <returns></returns>
        public static string PrintValue<T>(this T value, string key = "value", 
            bool multi_line_value = false)
            => 
            multi_line_value ? 
            $"{key}=\"{Environment.NewLine}{value}{Environment.NewLine}\""
            :
            $"{key}=\"{value}\"";
        /// <summary>
        /// Вывести значение c типом переменной
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string PrintValueWithType<T>(this T value, string key = "value")
            => $"{typeof(T)}: {key}=\"{value}\"";
        /// <summary>
        /// Вывести переменные класса
        /// </summary>
        /// <param name="obj">класс</param>
        /// <param name="ShowPrivate">отображать приватные</param>
        /// <param name="ShowTypes">отображать тип переменных</param>
        /// <returns></returns>
        public static string[] PrintClassValues<T>(this T obj, bool Sort = false, bool ShowPrivate = false, 
            bool ShowTypes = false)
            => Addons.PrintValuesInClass<T>(obj, Sort, ShowPrivate, ShowTypes);
        /// <summary>
        /// Получить Base64 из массива байт
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="encoding">Кодировка</param>
        /// <returns></returns>
        public static string GetBase64String(this byte[] data)
            => Convert.ToBase64String(data);
    }
    /// <summary>
    /// Класс для упрощения работы с хэшированием
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// Алгоритм хэширования
        /// </summary>
        public enum Type { MD5, SHA256, SHA512, SHA1, SHA384 };
        /// <summary>
        /// Получить ХЭШ из массивая байт
        /// </summary>
        /// <param name="bytes">массив байт</param>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        private static HashAlgorithm GetAlgorithm(Type type)
        {
            switch (type)
            {
                case Type.MD5: return new MD5CryptoServiceProvider();
                case Type.SHA256: return new SHA256CryptoServiceProvider();
                case Type.SHA512: return new SHA512CryptoServiceProvider();
                case Type.SHA1: return new SHA1CryptoServiceProvider();
                case Type.SHA384: return new SHA384CryptoServiceProvider();
                default: return new MD5CryptoServiceProvider();
            }
        }
        public static string GetHash(byte[] bytes, Type type = Type.MD5)
        {
            using (HashAlgorithm algorithm = GetAlgorithm(type))
            {
                bytes = algorithm.ComputeHash(bytes);
                string result = BitConverter.ToString(bytes).Replace("-", string.Empty);
                algorithm.Dispose();
                return result;
            }
        }
        public static byte[] GetHashBytes(byte[] bytes, Type type = Type.MD5)
        {
            using (HashAlgorithm algorithm = GetAlgorithm(type))
            {
                return algorithm.ComputeHash(bytes);
            }
        }
        /// <summary>
        /// Получить ХЭШ из строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHashFromString(string text, Type type = Type.MD5, Encoding encoding = null)
            => GetHash(GetBytes(text, encoding), type);
        private static byte[] GetBytes(string text, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.Default;
            return encoding.GetBytes(text);
        }
        /// <summary>
        /// Получить ХЭШ от времени сейчас
        /// </summary>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHashFromDateTime(Type type = Type.MD5, Encoding encoding = null)
            => GetHashFromString(Addons.GetNow(), type, encoding);
        /// <summary>
        /// Получить ХЭШ файла
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="type">тип ХЭШа</param>
        /// <returns></returns>
        public static string ComputeFileChecksum(string path, Type type = Type.MD5)
        {
            using (FileStream fs = File.OpenRead(path))
            using (HashAlgorithm algorithm = GetAlgorithm(type))
            {
                byte[] checkSum = algorithm.ComputeHash(fs);
                string result = BitConverter.ToString(checkSum).Replace("-", string.Empty);
                return result;
            }
        }
        /// <summary>
        /// Получить ХЭШ файла ассинхронно
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <returns></returns>
        public static async Task<string> ComputeFileChecksumAsync(string path)
            => await Task.Run(() => ComputeFileChecksum(path));
    }

    /// <summary>
    /// Дополнительный функционал
    /// </summary>
    public static class Addons
    {
        /// <summary>
        /// Получить строку повторение данной строки
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="repeat">количество повторений</param>
        /// <returns>новая строка</returns>
        public static string StringRepeat(this string str, int repeat)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < repeat; i++) builder.Append(str);
            return builder.ToString();
        }
        /// <summary>
        /// Распарсить дату и время
        /// </summary>
        /// <param name="str">строка</param>
        /// <returns>DateTime, null - при неуспешной операции</returns>
        public static DateTime? DateTimeParse(string str)
        {
            if (DateTime.TryParse(str, out DateTime date))
            {
                return date;
            }
            else
                return null;
        }
        /// <summary>
        /// Поменять переменные значениями
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }
        /// <summary>
        /// Проверить, совпадает ли строка с маской даты 31.12.2019, где день 00-31, месяц 01-12, год 1900-2199
        /// </summary>
        /// <param name="str">проверяемая строка</param>
        /// <returns>true, если совпадает, инача false</returns>
        public static bool DateMatchRegex(string str)
            => Regex.IsMatch(str, 
                "((0[1-9])|([12]\\d)|(3[01])).((0[1-9])|(1[012])).(19|20|21)\\d\\d");

        public static DateTime? GetDateFromDayMonthYear(int Day, int Month, int Year)
        {
            if (DateTime.TryParse($"{Day}.{Month}.{Year}", out DateTime result))
                return result;
            else
                return null;
        }

        public static DateTime? GetDateFromDayMonthYear(string Day, string Month, string Year)
        {
            if (DateTime.TryParse($"{Day}.{Month}.{Year}", out DateTime result))
                return result;
            else
                return null;
        }
        /// <summary>
        /// Выравневание строк текста для более читабельного вида
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="max_line_length">максимальное количество линий в строках</param>
        /// <param name="indent">добавлять префикс в начало каждой строки</param>
        /// <returns></returns>
        public static string Decor(string text, int max_line_length = 1024, string indent = "")
        {
            if (max_line_length <= indent.Length) 
                throw new Exception("Максимальная длина не может быть меньше длины отступа");
            max_line_length -= indent.Length;
            var result = new StringBuilder();
            string[] records = text.GetLines();
            for (int i = 0; i < records.Length; i++)
                while (records[i] != string.Empty)
                    result.AppendLine($"{indent}{CutBySubstring(ref records[i], max_line_length)}");
            return result.ToString();
        }
        /// <summary>
        /// Извлечь и удалить подстроку из строки
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="length">количество символов</param>
        /// <param name="start">позиция</param>
        /// <returns></returns>
        public static string CutBySubstring(ref string str, int length, int start = 0)
        {
            if (start > str.Length)
            {
                throw new Exception("Позиция больше длины строки");
            }
            else
            {
                string result = 
                    str.Substring(start, length + start > str.Length ? 
                    str.Length - start : length );
                string temp = "";
                if (start > 0) temp += str.Substring(0, start);
                if (length + start < str.Length) temp += str.Substring(length + start, str.Length - length - start);
                str = temp;
                return result;
            }
        }
        /// <summary>
        /// Получить количество лет и месяцев между двумя датами
        /// </summary>
        /// <param name="start">дата начала</param>
        /// <param name="end">дата завершения, если null, то берется актуальная дата</param>
        /// <returns></returns>
        public static string GetAge(DateTime start, DateTime? end = null)
        {
            try
            {
                end = end ?? DateTime.Now;
                DateTime dt1 = new DateTime(start.Year, start.Month, start.Day);
                DateTime dt2 = new DateTime(end.Value.Year, end.Value.Month, end.Value.Day);

                if (dt1 > dt2 || dt1 == dt2)
                    return "0 месяцев";

                double days = (dt2 - dt1).TotalDays;
                double mnt = 0;

                while (days != 0)
                {
                    int inMnt = DateTime.DaysInMonth(dt1.Year, dt1.Month);
                    if (days >= inMnt)
                    {
                        days -= inMnt;
                        ++mnt;
                        dt1 = dt1.AddMonths(1);
                    }
                    else
                    {
                        mnt += days / inMnt;
                        days = 0;
                    }
                }
                int mntx = (int)mnt;
                string years = (mntx / 12).ToString();
                if ("1234".Contains(years.Last())) years += " года"; else years += " лет";
                string month = (mntx % 12).ToString();
                if ("234".Contains(month.Last())) month += " месяца";
                else
                    if ('1' == month.Last()) month += " месяц";
                else
                    if ('0' == month.Last()) month = "";
                else
                    month += " месяцев";
                return month == "" ? years : $"{years} и {month}";
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Быстро и безболезнено перевести в int
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static int ToIntOrDeafault(string text, int default_value = int.MinValue)
            => int.TryParse(text, out int x) ? x : default_value;
        /// <summary>
        /// Быстро перевести в int, если не удастся, то вернёт Null
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int? ToInt(string text, int default_value = int.MinValue)
            => int.TryParse(text, out int x) ? (int?)x : null;
        /// <summary>
        /// Быстро и безболезнено перевести в long
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static long ToLong(string text, long default_value = long.MinValue)
            => long.TryParse(text, out long x) ? x : default_value;
        /// <summary>
        /// Вывести переменные класса
        /// </summary>
        /// <param name="obj">класс</param>
        /// <param name="ShowPrivate">отображать приватные</param>
        /// <param name="ShowTypes">отображать тип переменных</param>
        /// <returns></returns>
        public static string[] PrintValuesInClass<T>(T obj, bool Sort = false, 
            bool ShowPrivate = false, bool ShowTypes = false)
        {
            if (obj == null) return new string[]{"Null"};
            List<string> result = new List<string>();
            BindingFlags flags = ShowPrivate ?
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                :
                BindingFlags.Public | BindingFlags.Instance;
            try
            {
                PropertyInfo[] pi = obj.GetType().GetProperties(flags);
                string[] properties = ShowTypes ?
                   pi.Select(x => $"{x.PropertyType.Name}: {x.Name}=\"{x.GetValue(obj)}\"").ToArray()
                   :
                   pi.Select(x => $"{x.Name}=\"{x.GetValue(obj)}\"").ToArray();
                result.AddRange(properties);
            }
            catch { }
            try
            {
                FieldInfo[] fi = obj.GetType().GetFields(flags);
                string[] fields = ShowTypes ?
                   fi.Select(x => $"{x.FieldType.Name}: {x.Name}=\"{x.GetValue(obj)}\"").ToArray()
                   :
                   fi.Select(x => $"{x.Name}=\"{x.GetValue(obj)}\"").ToArray();
                result.AddRange(fields);
            }
            catch { }
            if (Sort) result.Sort();
            return result.ToArray();
        }
        /// <summary>
        /// Запущена ли программа от Администратора
        /// </summary>
        /// <returns>True, если администратор</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
        public static string GetWindowsUser() => WindowsIdentity.GetCurrent().Name;
        /// <summary>
        /// Получить время в региональном формате
        /// </summary>
        /// <param name="Culture">регион</param>
        /// <returns></returns>
        public static string GetNow(string Culture = "Ru-ru")
            => DateTime.Now.ToString(new CultureInfo(Culture));
        /// <summary>
        /// Найди значения по регулярному выражения
        /// </summary>
        /// <param name="DATA">строка</param>
        /// <param name="TAG">регулярное выражение</param>
        /// <param name="registr">регистрозависимость</param>
        /// <param name="singleline">считать весь текст единой строкой</param>
        /// <param name="group">вывести группу</param>
        /// <returns></returns>
        public static List<string> FindByRegex(string DATA, string TAG, bool registr = false,
                                              bool singleline = false, int group = 0)
        {
            List<string> result = new List<string>();
            try
            {
                RegexOptions ro = singleline ? RegexOptions.Singleline : RegexOptions.Multiline;
                ro = !registr ? ro | RegexOptions.IgnoreCase : ro;
                MatchCollection re = Regex.Matches(DATA, TAG, ro);
                foreach (Match rem in re)
                    result.Add(rem.Groups[group].ToString());
            }
            catch { }
            return result;
        }
        /// <summary>
        /// Является ли строка IP-адресом(IPv4)
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static bool IsIP(string text)
            => Regex.IsMatch(text, "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        /// <summary>
        /// Перевести Hex-строку в integer
        /// </summary>
        /// <param name="hexString">строка с hex</param>
        /// <returns></returns>
        public static int GetByteFromHexString(string hexString) => 
            int.Parse(hexString, NumberStyles.HexNumber);
    }

    /// <summary>
    /// В транслит
    /// </summary>
    public static class Translitiration
    {
        private static readonly Dictionary<char, string> ConvertedLetters = new Dictionary<char, string>
            {
                {'а', "a"},
                {'б', "b"},
                {'в', "v"},
                {'г', "g"},
                {'д', "d"},
                {'е', "e"},
                {'ё', "yo"},
                {'ж', "zh"},
                {'з', "z"},
                {'и', "i"},
                {'й', "j"},
                {'к', "k"},
                {'л', "l"},
                {'м', "m"},
                {'н', "n"},
                {'о', "o"},
                {'п', "p"},
                {'р', "r"},
                {'с', "s"},
                {'т', "t"},
                {'у', "u"},
                {'ф', "f"},
                {'х', "h"},
                {'ц', "c"},
                {'ч', "ch"},
                {'ш', "sh"},
                {'щ', "sch"},
                {'ъ', "j"},
                {'ы', "i"},
                {'ь', "j"},
                {'э', "e"},
                {'ю', "yu"},
                {'я', "ya"},
                {'А', "A"},
                {'Б', "B"},
                {'В', "V"},
                {'Г', "G"},
                {'Д', "D"},
                {'Е', "E"},
                {'Ё', "Yo"},
                {'Ж', "Zh"},
                {'З', "Z"},
                {'И', "I"},
                {'Й', "J"},
                {'К', "K"},
                {'Л', "L"},
                {'М', "M"},
                {'Н', "N"},
                {'О', "O"},
                {'П', "P"},
                {'Р', "R"},
                {'С', "S"},
                {'Т', "T"},
                {'У', "U"},
                {'Ф', "F"},
                {'Х', "H"},
                {'Ц', "C"},
                {'Ч', "Ch"},
                {'Ш', "Sh"},
                {'Щ', "Sch"},
                {'Ъ', "J"},
                {'Ы', "I"},
                {'Ь', "J"},
                {'Э', "E"},
                {'Ю', "Yu"},
                {'Я', "Ya"}
            };
        /// <summary>
        /// Перевести русский текст в транслит
        /// </summary>
        /// <param name="source">исходная строка</param>
        /// <returns>строка в транслите</returns>
        public static string ConvertToLatin(string source)
        {
            var result = new StringBuilder();
            foreach (var letter in source)
            {
                if (ConvertedLetters.ContainsKey(letter))
                    result.Append(ConvertedLetters[letter]);
                else
                    result.Append(letter);
            }
            return result.ToString();
        }
    }

    /// <summary>
    /// Класс диапозона, содержащий минимум и максимум
    /// </summary>
    public class Range
    {
        public Range() { }
        public Range(ulong min, ulong max)
        {
            Min = min;
            Max = max;
        }
        public ulong Min;
        public ulong Max;
        public virtual string ToString(string splitter = "\t")
            => Min == Max ? $"{Min}" : $"{Min}{splitter}{Max}";
    }

    /// <summary>
    /// Проверка и получение телефонных номеров РФ
    /// </summary>
    public static class RusPhoneNumbers
    {
        /// <summary>
        /// Получить номер телефона из текста
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="prefix">Префикс в зависимости от желамого результата рекомедуется: "", "+7", "8" или "7"</param>
        /// <returns>номер телефона в виде строки</returns>
        public static string GetPhoneNumber(string text, string prefix = "")
        {
            List<char> cha = text.Where(x => IsNum(x)).ToList();
            if (cha.Count < 10) return null;
            if (cha.Count > 11) return null;
            if (cha.Count == 11 && (!(cha[0] == '7' || cha[0] == '8'))) return null;
            if (cha.Count == 11) cha.RemoveAt(0);
            return $"{prefix}{string.Join("", cha)}";
        }

        public static class Mask
        {
            /// <summary>
            /// Пример: 79123456789
            /// </summary>
            public static string E164 = @"^7\d{10}$";
            /// <summary>
            /// Пример: 89123456789
            /// </summary>
            public static string National = @"^8\d{10}$";
            /// <summary>
            /// Пример: +79123456789
            /// </summary>
            public static string Mobile = @"^\+7\d{10}$";
            /// <summary>
            /// Примеры: 79123456789, 89123456789, +79123456789
            /// </summary>
            public static string Simple = @"^((\+7)|7|8)\d{10}$";
            /// <summary>
            /// Примеры: 7(912)3456789, 8(912)3456789, +7(912)3456789
            /// </summary>
            public static string ForPrint = @"^((\+7)|7|8)\(\d\d\d\)\d\d\d\d\d\d\d$";
            /// <summary>
            /// Примеры: 7(912)345-67-89, 8(912)345-67-89, +7(912)345-67-89
            /// </summary>
            public static string ForPrintWithMinus1 = @"^((\+7)|7|8)\(\d\d\d\)\d\d\d-\d\d-\d\d$";
            /// <summary>
            /// Примеры: 7(912)345-6789, 8(912)345-6789, +7(912)345-6789
            /// </summary>
            public static string ForPrintWithMinus2 = @"^((\+7)|7|8)\(\d\d\d\)\d\d\d-\d\d\d\d$";
        }
        /// <summary>
        /// Проверить, является ли строка телефонным номеров по указанной маске
        /// </summary>
        /// <param name="number">номер телефона</param>
        /// <param name="mask">маска, регулярное выражение, можно взять из класса Mask или написать свое выражение</param>
        public static bool IsPhoneMask(string number, string mask) => Regex.IsMatch(number, mask);
        /// <summary>
        /// Проверить, является ли строка телефонным номеров
        /// </summary>
        /// <param name="number">номер телефона</param>
        public static bool IsPhoneNumber(string number)
        {
            return
                (
                Regex.IsMatch(number, Mask.Simple)
                ||
                Regex.IsMatch(number, Mask.ForPrint)
                ||
                Regex.IsMatch(number, Mask.ForPrintWithMinus1)
                ||
                Regex.IsMatch(number, Mask.ForPrintWithMinus2)
                );
        }
        public static string[] GetPhoneNumbersIgnoreOneSpace(string text, string prefix = "")
            => GetPhoneNumbersIgnoreSpaces(text.Replace("  ", "\t"), prefix);
        public static string[] GetPhoneNumbersIgnoreSpaces(string text, string prefix = "")
            => GetPhoneNumbers(text.Replace(" ", ""), prefix);
        public static string[] GetPhoneNumbers(string text, string prefix = "")
        {
            text = Regex.Replace(text, @"[\(\)-]", "");
            string[] texts = Regex.Split(text, @"[\W+]");
            texts = texts.Select(x => GetPhoneNumber(x, prefix)).Where(x => x != null).ToArray();
            return texts;
        }
        private static bool IsNum(char ch) => ch >= '0' && ch <= '9';
    }
    /// <summary>
    /// Проверка условий
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Возвращает значение True, если переменная имеет значение, отличное от Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>Еrue, если переменная имеет значение, отличное от Null</returns>
        public static bool NotNull<T>(this T obj) => obj != null;
        /// <summary>
        /// Возвращает значение True, если переменная имеет значение, равное Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>true, если переменная имеет значение, равное Null</returns>
        public static bool Null<T>(this T obj) => obj == null;
        /// <summary>
        /// Возвращает True, если строка пустая
        /// </summary>
        /// <param name="text"></param>
        /// <returns>True, если строка пустая или Null</returns>
        public static bool IsEmpty(this string text) => text == "";
        /// <summary>
        /// Возвращает True, если строка не пустая
        /// </summary>
        /// <param name="text"></param>
        /// <returns>True, если строка пустая или Null</returns>
        public static bool IsNotEmpty(this string text) => text != "";
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this int number) => number == 0;
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this long number) => number == 0;
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this byte number) => number == 0;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this int a, int b) => a > b;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this long a, long b) => a > b;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this byte a, long b) => a > b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this int a, int b) => a < b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this long a, long b) => a < b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this byte a, long b) => a < b;
        /// <summary>
        /// Длиннее
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthMore(this string a, int b) => a.Length > b;
        /// <summary>
        /// Короче
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthLess(this string a, int b) => a.Length < b;
        /// <summary>
        /// Длиннее, чем строка
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthMore(this string a, string b) => a.Length > b.Length;
        /// <summary>
        /// Короче, чем строка
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthLess(this string a, string b) => a.Length < b.Length;
        /// <summary>
        /// Возвращает True, если число не четное
        /// </summary>
        /// <param name="number"></param>
        /// <returns>number % 2 == 1</returns>
        public static bool IsOdd(this int number) => number % 2 == 1;
        /// <summary>
        /// Возвращает True, если число четное
        /// </summary>
        /// <param name="number"></param>
        /// <returns>number % 2 == 0</returns>
        public static bool IsEven(this int number) => number % 2 == 0;
    }
    /// <summary>
    /// Имя файла
    /// </summary>
    public class FileName
    {
        /// <summary>
        /// Создать экземпляр имени класса
        /// </summary>
        /// <param name="file_name"></param>
        public FileName(string file_name)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(file_name);
            Extension = System.IO.Path.GetExtension(file_name);
            Path = System.IO.Path.GetDirectoryName(file_name);
        }
        /// <summary>
        /// Имя файла без расширения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Полный путь к файлу
        /// </summary>
        public string Full => Path == "" ? $"{Name}{Extension}" : $"{Path}\\{Name}{Extension}";
        /// <summary>
        /// Имя файла с расширением
        /// </summary>
        public string NameWithExtension => $"{Name}{Extension}";
        public string[] SplitPath => Path.Split('\\');
    }
    /// <summary>
    /// Перевести int в числительные строки, к примеру 1ый, 2ой, 103ий. Также указать возраст N год/лет.
    /// </summary>
    public static class OneToFirst
    {
        /// <summary>
        /// Вывод возраста, добавляет к числу слово год/лет в зависимости от числа
        /// </summary>
        /// <param name="years">количество лет</param>
        /// <returns>строка с числом и словом год/лет</returns>
        public static string YearsToText(this int years)
        {
            if (years >= 10 && years < 20) return $"{years} лет";
            char last = years.ToString().Last();
            switch (last)
            {
                case '1': 
                    return $"{years} год";
                case '2': 
                case '3': 
                case '4': 
                    return $"{years} года";
                case '0':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return $"{years} лет";
                default: return years.ToString();
            }
        }
        /// <summary>
        /// Перевести int в числительные строки на русском языке
        /// </summary>
        /// <param name="number">число</param>
        /// <returns>числительное строка</returns>
        public static string OneToFirstRus(this int number)
        {
            if (number >= 10 && number < 20) return $"{number}ый";
            string text = number.ToString();
            switch (text.LastOrDefault())
            {
                case '0': return text == "0" ? $"{text}ой" : $"{text}ый";
                case '1': return $"{text}ый";
                case '2': return $"{text}ой";
                case '3': return $"{text}ий";
                case '4': return $"{text}ый";
                case '5': return $"{text}ый";
                case '6': return $"{text}ой";
                case '7': return $"{text}ой";
                case '8': return $"{text}ой";
                case '9': return $"{text}ый";
                default: return text;
            }
        }
        /// <summary>
        /// Перевести int в числительные строки на английском языке
        /// </summary>
        /// <param name="number">число</param>
        /// <returns>числительное строка</returns>
        public static string OneToFirstEng(this int number)
        {
            if (number >= 10 && number < 20) return $"{number}th";
            string text = number.ToString();
            switch (text.LastOrDefault())
            {
                case '0': return text == "0" ? text : $"{text}th";
                case '1': return $"{text}st";
                case '2': return $"{text}nd";
                case '3': return $"{text}rd";
                default: return $"{text}th";
            }
        }
    }

    /// <summary>
    /// Работа с очередью уведомлений
    /// </summary>
    public static class Notifies
    {
        public enum NotifyType
        {
            Normal  = 0,
            Message = 1,
            Warning = 2,
            Error   = 3
        }
        /// <summary>
        /// Класс уведомления
        /// </summary>
        public class Notify
        {
            /// <summary>
            /// Создать экземпляр класса Notify
            /// </summary>
            /// <param name="text">текст уведомления</param>
            /// <param name="title">название уведомления</param>
            /// <param name="type">тип уведомления</param>
            public Notify(string text, string title = "", NotifyType type = NotifyType.Normal)
            {
                Title = title;
                Text  = text;
                Type  = type;
            }
            /// <summary>
            /// текст уведомления
            /// </summary>
            public readonly string Title;
            /// <summary>
            /// название уведомления
            /// </summary>
            public readonly string Text;
            /// <summary>
            /// Тип уведомления: Normal = 0, Message = 1, Warning = 2, Error = 3
            /// </summary>
            public readonly NotifyType Type;
        }

        public class Notifier
        {
            /// <summary>
            /// Создать экземпляр класса Notifier
            /// </summary>
            public Notifier()
            {
                Notifies = new ConcurrentQueue<Notify>();
                Instants = new ConcurrentQueue<Notify>();
            }
            private static Notifier instance;
            /// <summary>
            /// Получить сущность Notifier, если сущность не задана, то создается новый экзепляр
            /// </summary>
            /// <returns></returns>
            public static Notifier GetInstance()
            {
                if (instance == null) instance = new Notifier();
                return instance;
            }

            private readonly ConcurrentQueue<Notify> Notifies;
            private readonly ConcurrentQueue<Notify> Instants;
            /// <summary>
            /// Получить следующее уведомление, но не убирать его из очереди
            /// </summary>
            /// <returns></returns>
            public Notify Next()
            {
                if (!Instants.TryPeek(out Notify notify))
                    Notifies.TryPeek(out notify);
                return notify;
            }
            /// <summary>
            /// Получить следующее уведомление и убирать его из очереди
            /// </summary>
            /// <returns></returns>
            public Notify Get()
            {
                if (!Instants.TryDequeue(out Notify notify))
                    Notifies.TryDequeue(out notify);
                return notify;
            }
            /// <summary>
            /// Добавить уведомление
            /// </summary>
            /// <param name="notify">уведомление</param>
            public void Add(Notify notify) => Notifies.Enqueue(notify);
            /// <summary>
            /// Добавить уведомление
            /// </summary>
            /// <param name="text">текст уведомления</param>
            /// <param name="title">название уведомления</param>
            public void Add(string text, string title = "", NotifyType type = NotifyType.Normal)
                => Add(new Notify(text, title));
            /// <summary>
            /// Добавить приоритетное уведомление
            /// </summary>
            /// <param name="notify">уведомление</param>
            public void Instant(Notify notify) => Instants.Enqueue(notify);
            /// <summary>
            /// Добавить приоритетное уведомление
            /// </summary>
            /// <param name="text">текст уведомления</param>
            /// <param name="title">название уведомления</param>
            public void Instant(string text, string title = "", 
                                NotifyType type = NotifyType.Normal)
                => Instant(new Notify(text, title, type));
        }
    }

    /// <summary>
    /// Заготовленные регулярные выражения
    /// </summary>
    public static class RegexPresets
    {
        public static readonly string Words = @"[^(\w)|\-)]";
        public static readonly string Digits = @"[^\d]";
        public static readonly string DigitsAndLetters = @"([^\w])|\d";
        public static readonly string Date 
            = @"((0[1-9])|([12]\\d)|(3[01])).((0[1-9])|(1[012])).(19|20|21)\\d\\d";
        public static readonly string IPv4 
            = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        public static readonly string KeyValue = @"^(\w+)=(.*)$";
    }
}