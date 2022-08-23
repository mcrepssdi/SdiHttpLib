using System.Net;

namespace SdiHttpLib;

public interface IHttp
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <param name="url"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Task<(HttpStatusCode httpStatusCode, string responseText)> Get<T>(T parameters, string url,
        List<(string key, string value)>? headers);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="url"></param>
    /// <param name="headers"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="Exception"></exception>
    public Task<(HttpStatusCode httpStatusCode, string responseText)> Put(string data, string url,
        List<(string key, string value)>? headers, string mediaType = "application/json");
    
    /// <summary>
    /// Makes a HttpPost.
    /// </summary>
    /// <param name="data">Data string to POST in any media type</param>
    /// <param name="url">End Point</param>
    /// <param name="headers">Any Required Http Headers</param>
    /// <param name="mediaType">Media Type, defauls to "application/json"</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="Exception"></exception>
    public Task<(HttpStatusCode httpStatusCode, string responseText)> Post(string data, string url,
        List<(string key, string value)>? headers, string mediaType = "application/json");
    
    /// <summary>
    /// Makes a HttpPost and Clears the Character Set.
    /// </summary>
    /// <param name="data">Data string to POST in any media type</param>
    /// <param name="url">End Point</param>
    /// <param name="headers">Any Required Http Headers</param>
    /// <param name="mediaType">Media Type, defauls to "application/json"</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="Exception"></exception>
    public Task<(HttpStatusCode httpStatusCode, string responseText)> SaiPost(string data, string url,
        List<(string key, string value)>? headers, string mediaType = "application/json");
}