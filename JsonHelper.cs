using System.Web.Script.Serialization;

namespace JSONHelper
{
    /// <summary>
    /// Помощник для работы с JSON
    /// </summary>
    public static class JSONHelper
    {
        /// <summary>
        /// Перевести в JSON
        /// </summary>>
        /// <param name="obj">класс</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj) => Serialize(obj);
        /// <summary>
        /// В класс из JSON
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static T FromJson<T>(this string json) => Deserialize<T>(json);
        /// <summary>
        /// В класс из JSON, не выдает Exception, если не удалось, то возвращает значение по умолчанию
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static T FromJsonWithoutException<T>(this string json)
        {
            try
            {
                return Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }
        /// <summary>
        /// Класс в JSON
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        /// <returns></returns>
        private static string Serialize<T>(T obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        /// <summary>
        /// Из JSON в класс
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        private static T Deserialize<T>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}