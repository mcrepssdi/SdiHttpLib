using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using NLog;
using SdiHttpLib.Utilities;

namespace SdiHttpLib;

public class SdiHttp :IHttp
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    /// </summary>
    
    
    private readonly Logger? _logger;
    public SdiHttp(Logger? logger = null)
    {
        _logger = logger ?? LogManager.CreateNullLogger();
    }
    
    /// <inheritdoc cref="IHttp.Get" />
    public async Task<(HttpStatusCode httpStatusCode, string responseText)> Get<T>(T parameters, string url,
        List<(string key, string value)>? headers)
    {
        _logger?.Trace("GET...");
        if (url.Equals(string.Empty))
        {
            throw new ArgumentNullException(url, "Endpoint is required.");
        }
        
        /* Use Reflection to get values and Name */
        Dictionary<string, string> queryBag = parameters.GetQueryBag();
        string? uri = QueryHelpers.AddQueryString(url, queryBag);

        // Start the Clinet and Make Post
        string errorMsg;
        HttpClient httpClient = new ();
        HttpResponseMessage response = new();
        try
        {
            response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            _logger?.Info($"HttpResponseCode: {response.StatusCode.ToString()}");
            string responseText = await response.Content.ReadAsStringAsync();

            _logger?.Trace(responseText);
            return (response.StatusCode, responseText);
        }
        catch (HttpRequestException ex)
        {
            errorMsg = $"Error: {ex.HResult:X}\tMessage: {ex.Message}";
            _logger?.Error(errorMsg);  
        }
        catch (Exception e)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            errorMsg = $"Error: {e.HResult:X}\tMessage: {e.Message}";
            _logger?.Error(errorMsg);
        }
        finally
        {
            httpClient.Dispose();
        }
        
        return (response.StatusCode, errorMsg);
    }

    /// <inheritdoc cref="IHttp.Put" />
    public async Task<(HttpStatusCode httpStatusCode, string responseText)> Put(string data, string url, List<(string key, string value)>? headers, string mediaType = "application/json")
    {
        _logger?.Trace("PUT...");
        if (url.Equals(string.Empty))
        {
            throw new ArgumentNullException(url, "Endpoint is required.");
        }
        StringContent content = new (data, Encoding.UTF8, mediaType);

        // Start the Clinet and Make Post
        string errorMsg;
        HttpClient httpClient = new ();
        HttpResponseMessage response = new();
        try
        {
            response = await httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            _logger?.Info($"HttpResponseCode: {response.StatusCode.ToString()}");
            string responseText = await response.Content.ReadAsStringAsync();
            
            _logger?.Trace(responseText);
            return (response.StatusCode, responseText);
        }
        catch (HttpRequestException ex)
        {
            errorMsg = $"Error: {ex.HResult:X}\tMessage: {ex.Message}";
            _logger?.Error(errorMsg);  
        }
        catch (Exception e)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            errorMsg = $"Error: {e.HResult:X}\tMessage: {e.Message}";
            _logger?.Error(errorMsg);
        }
        finally
        {
            httpClient.Dispose();
        }
        
        return (HttpStatusCode.InternalServerError, errorMsg);
    }

    /// <inheritdoc cref="IHttp.Post" />
    public Task<(HttpStatusCode httpStatusCode, string responseText)> Post(string data, string url,
        List<(string key, string value)>? headers, string mediaType = "application/json")
    {
        _logger?.Trace("POSTING...");
        if (url.Equals(string.Empty))
        {
            throw new ArgumentNullException(url, "Endpoint is required.");
        }
        
        /* Create the Data Packet, using the specified mediaType */
        string filteredData = Regex.Replace(data, @"\p{C}+", string.Empty);
        StringContent content = new (filteredData, Encoding.UTF8, mediaType);
        
        return Post(content, url, headers);
    }
    
    /// <inheritdoc cref="IHttp.SaiPost" />
    public Task<(HttpStatusCode httpStatusCode, string responseText)> SaiPost(string data, string url,
        List<(string key, string value)>? headers, string mediaType = "application/json")
    {
        _logger?.Trace("POSTING...");
        if (url.Equals(string.Empty))
        {
            throw new ArgumentNullException(url, "Endpoint is required.");
        }
        
        /* Create the Data Packet, using the specified mediaType
         * To remove all control and other non-printable characters
         */
        string filteredData = Regex.Replace(data, @"\p{C}+", string.Empty);
        StringContent content = new (filteredData, Encoding.UTF8, mediaType);
        
        /* This is a Hack as the SAI Validation of Content-Type only check if equals "application/json" */
        content.Headers.ContentType!.CharSet = "";
        
        return Post(content, url, headers);
    }
    
    #region Private Methods
    
    private async Task<(HttpStatusCode httpStatusCode, string responseText)> Post(HttpContent content, string url,
        List<(string key, string value)>? headers)
    {
        
        // Start the Clinet and Make Post
        string errorMsg;
        HttpClient httpClient = new ();
        HttpResponseMessage response = new();
        try
        {
            /* Add the Headers if we have any, here becuase if duplicates or mis-used will throw exception */
            if (headers is not null)
            {
                foreach ((string key, string value) in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(key, value);
                }
            }

            response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            _logger?.Info($"HttpResponseCode: {response.StatusCode.ToString()}");
            string responseText = await response.Content.ReadAsStringAsync();
            
            _logger?.Trace(responseText);
            return (response.StatusCode, responseText);
        }
        catch (HttpRequestException ex)
        {
            errorMsg = $"Error: {ex.HResult:X}\tMessage: {ex.Message}";
            _logger?.Error(errorMsg);  
        }
        catch (Exception e)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            errorMsg = $"Error: {e.HResult:X}\tMessage: {e.Message}";
            _logger?.Error(errorMsg);
        }
        finally
        {
            httpClient.Dispose();
        }
        
        return (response.StatusCode, errorMsg);
    }
    
    #endregion
    
    
}