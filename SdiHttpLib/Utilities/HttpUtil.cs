using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using NLog;

namespace SdiHttpLib.Utilities;

public static class HttpUtil
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <param name="logger"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? CastReponse<T>(this string? json, Logger? logger = null)
    {
        if (json is null || json.Equals(string.Empty)) return default;
        logger ??= LogManager.CreateNullLogger();
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception e)
        {
            logger.Error($"Unable to cast ResponseWrapper.  Msg: {e.Message}");
        }
        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="logger"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? CastReponse<T>(this Task<(HttpStatusCode httpStatusCode, string responseText)> response, Logger? logger = null)
    {
        logger ??= LogManager.CreateNullLogger();
        try
        {
        
            T? wrapper = JsonConvert.DeserializeObject<T>(response.Result.responseText);
            logger.Trace($"POST Status: {response.Result.httpStatusCode.ToString()}");
            return response.Result.httpStatusCode != HttpStatusCode.OK ? default : wrapper;
        }
        catch (Exception e)
        {
            logger.Error($"Unable to cast response object.\tMessage:{e.Message}");
        }
        return default;
    }

    /// <summary>
    /// Converts a Generic Object of Type T into a Dictionary<string, string>
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, string> GetQueryBag<T>(this T obj)
    {
        PropertyInfo[] props = typeof(T).GetProperties();
        return props.ToDictionary(prop => prop.Name, prop => GetValue(prop, obj));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetValue<T>(PropertyInfo prop, T obj)
    {
        Type? type = prop.PropertyType.GetDataType();
        if (type == typeof(string))
        {
            object value = prop.GetValue(obj) ?? string.Empty;
            return (string) value;
        }

        if (type == typeof(int))
        {
            object value = prop.GetValue(obj) ?? 0;
            return ((int) value).ToString();
        }

        if (type == typeof(long))
        {
            object value = prop.GetValue(obj) ?? 0;
            return ((long) value).ToString();
        }

        if (type == typeof(DateTime))
        {
            object? value = prop.GetValue(obj) ?? null;
            if (value is null)
            {
                return "NULL";
            }

            DateTime dt = (DateTime) value;
            return $"{dt:MM/dd/yyyy}";
        }

        if (type == typeof(decimal))
        {
            object value = prop.GetValue(obj) ?? 0;
            return ((decimal) value).ToString();
        }

        if (type == typeof(double))
        {
            object value = prop.GetValue(obj) ?? 0;
            return ((double) value).ToString();
        }

        if (type == typeof(bool))
        {
            object value = prop.GetValue(obj) ?? string.Empty;
            return ((bool) value).ToString();
        }

        return string.Empty;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type? GetDataType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Type? nullableType =  Nullable.GetUnderlyingType(type);
            return nullableType;
        }

        return type;
    }
}